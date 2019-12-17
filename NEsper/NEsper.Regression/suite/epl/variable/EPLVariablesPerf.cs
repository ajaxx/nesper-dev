///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2015 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using com.espertech.esper.common.client.scopetest;
using com.espertech.esper.common.@internal.support;
using com.espertech.esper.compat;
using com.espertech.esper.regressionlib.framework;

using NUnit.Framework;

namespace com.espertech.esper.regressionlib.suite.epl.variable
{
    public class EPLVariablesPerf : RegressionExecution
    {
        public void Run(RegressionEnvironment env)
        {
            var path = new RegressionPath();
            env.CompileDeploy("create window MyWindow#keepall as SupportBean", path);
            env.CompileDeploy("insert into MyWindow select * from SupportBean", path);
            env.CompileDeploy("create const variable String MYCONST = 'E331'", path);

            for (var i = 0; i < 10000; i++) {
                env.SendEventBean(new SupportBean("E" + i, i * -1));
            }

            // test join
            env.CompileDeploy(
                "@Name('s0') select * from SupportBean_S0 S0 unidirectional, MyWindow sb where TheString = MYCONST",
                path);
            env.AddListener("s0");

            var listener = env.Listener("s0");
            var start = PerformanceObserver.MilliTime;
            for (var i = 0; i < 10000; i++) {
                env.SendEventBean(new SupportBean_S0(i, "E" + i));
                EPAssertionUtil.AssertProps(
                    listener.AssertOneGetNewAndReset(),
                    new [] { "sb.TheString","sb.IntPrimitive" },
                    new object[] {"E331", -331});
            }

            var delta = PerformanceObserver.MilliTime - start;
            Assert.That(delta, Is.LessThan(500), "delta=" + delta);
            env.UndeployModuleContaining("s0");

            // test subquery
            env.CompileDeploy(
                "@Name('s0') select * from SupportBean_S0 where exists (select * from MyWindow where TheString = MYCONST)",
                path);
            env.AddListener("s0");
            listener = env.Listener("s0");

            start = PerformanceObserver.MilliTime;
            for (var i = 0; i < 10000; i++) {
                env.SendEventBean(new SupportBean_S0(i, "E" + i));
                Assert.IsTrue(listener.GetAndClearIsInvoked());
            }

            delta = PerformanceObserver.MilliTime - start;
            Assert.That(delta, Is.LessThan(500), "delta=" + delta);

            env.UndeployModuleContaining("s0");
            env.UndeployAll();
        }
    }
} // end of namespace