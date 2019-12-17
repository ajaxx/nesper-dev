///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2015 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using com.espertech.esper.common.@internal.support;
using com.espertech.esper.regressionlib.framework;

using NUnit.Framework;

namespace com.espertech.esper.regressionlib.suite.rowrecog
{
    public class RowRecogIntervalResolution : RegressionExecution
    {
        private readonly long flipTime;

        public RowRecogIntervalResolution(long flipTime)
        {
            this.flipTime = flipTime;
        }

        public void Run(RegressionEnvironment env)
        {
            env.AdvanceTime(0);

            var text = "@Name('s0') select * from SupportBean " +
                       "match_recognize (" +
                       " measures A as a" +
                       " pattern (A*)" +
                       " interval 10 seconds" +
                       ")";
            env.CompileDeploy(text).AddListener("s0");

            env.SendEventBean(new SupportBean("E1", 1));

            env.AdvanceTime(flipTime - 1);
            Assert.IsFalse(env.Listener("s0").IsInvokedAndReset());

            env.Milestone(0);

            env.AdvanceTime(flipTime);
            Assert.IsTrue(env.Listener("s0").IsInvokedAndReset());

            env.UndeployAll();
        }
    }
} // end of namespace