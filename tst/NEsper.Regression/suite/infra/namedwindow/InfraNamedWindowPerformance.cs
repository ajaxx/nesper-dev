///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2015 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

using com.espertech.esper.common.client.scopetest;
using com.espertech.esper.common.@internal.support;
using com.espertech.esper.compat;
using com.espertech.esper.regressionlib.framework;
using com.espertech.esper.regressionlib.support.bean;
using com.espertech.esper.regressionlib.support.util;

using NUnit.Framework;

using SupportBean_A = com.espertech.esper.regressionlib.support.bean.SupportBean_A;

namespace com.espertech.esper.regressionlib.suite.infra.namedwindow
{
    /// <summary>
    ///     NOTE: More namedwindow-related tests in "nwtable"
    /// </summary>
    public class InfraNamedWindowPerformance
    {
        public static IList<RegressionExecution> Executions()
        {
            var execs = new List<RegressionExecution>();
            execs.Add(new InfraOnSelectInKeywordPerformance());
            execs.Add(new InfraOnSelectEqualsAndRangePerformance());
            execs.Add(new InfraDeletePerformance());
            execs.Add(new InfraDeletePerformanceCoercion());
            execs.Add(new InfraDeletePerformanceTwoDeleters());
            execs.Add(new InfraDeletePerformanceIndexReuse());
            return execs;
        }

        private static void RunOnDemandAssertion(
            RegressionEnvironment env,
            RegressionPath path,
            string epl,
            int numIndexes,
            object theEvent,
            int? expected)
        {
            Assert.AreEqual(0, GetIndexCount(env));

            env.CompileDeploy("@Name('s0')" + epl, path).AddListener("s0");
            Assert.AreEqual(numIndexes, GetIndexCount(env));

            var start = PerformanceObserver.MilliTime;
            var loops = 1000;

            for (var i = 0; i < loops; i++) {
                env.SendEventBean(theEvent);
                Assert.AreEqual(expected, env.Listener("s0").AssertOneGetNewAndReset().Get("sumi"));
            }

            var end = PerformanceObserver.MilliTime;
            var delta = end - start;
            Assert.That(delta, Is.LessThan(1000), "delta=" + delta);

            env.UndeployModuleContaining("s0");
            Assert.AreEqual(0, GetIndexCount(env));
        }

        private static int GetIndexCount(RegressionEnvironment env)
        {
            return SupportInfraUtil.GetIndexCountNoContext(env, true, "create", "MyWindow");
        }

        private static void SendSupportBean_A(
            RegressionEnvironment env,
            string id)
        {
            var bean = new SupportBean_A(id);
            env.SendEventBean(bean);
        }

        private static void SendMarketBean(
            RegressionEnvironment env,
            string symbol,
            double price)
        {
            var bean = new SupportMarketDataBean(symbol, price, 0L, null);
            env.SendEventBean(bean);
        }

        private static void SendSupportBean(
            RegressionEnvironment env,
            string theString,
            long longPrimitive)
        {
            var bean = new SupportBean();
            bean.TheString = theString;
            bean.LongPrimitive = longPrimitive;
            env.SendEventBean(bean);
        }

        private static void SendSupportBean(
            RegressionEnvironment env,
            string theString,
            int intPrimitive)
        {
            var bean = new SupportBean();
            bean.TheString = theString;
            bean.IntPrimitive = intPrimitive;
            env.SendEventBean(bean);
        }

        internal class InfraOnSelectInKeywordPerformance : RegressionExecution
        {
            public void Run(RegressionEnvironment env)
            {
                var path = new RegressionPath();
                env.CompileDeploy(
                    "@Name('create') create window MyWindow#keepall as SupportBean_S0;\n" +
                    "insert into MyWindow select * from SupportBean_S0;\n",
                    path);

                var maxRows = 10000; // for performance testing change to int maxRows = 100000;
                for (var i = 0; i < maxRows; i++) {
                    env.SendEventBean(new SupportBean_S0(i, "P00_" + i));
                }

                var eplSingleIdx =
                    "on SupportBean_S1 select sum(mw.Id) as sumi from MyWindow mw where P00 in (P10, P11)";
                RunOnDemandAssertion(env, path, eplSingleIdx, 1, new SupportBean_S1(0, "x", "P00_6523"), 6523);

                var eplMultiIndex =
                    "on SupportBean_S1 select sum(mw.Id) as sumi from MyWindow mw where P10 in (P00, P01)";
                RunOnDemandAssertion(env, path, eplMultiIndex, 2, new SupportBean_S1(0, "P00_6524"), 6524);

                env.UndeployAll();
            }
        }

        internal class InfraOnSelectEqualsAndRangePerformance : RegressionExecution
        {
            public void Run(RegressionEnvironment env)
            {
                var path = new RegressionPath();
                env.CompileDeploy(
                    "@Name('create') create window MyWindow#keepall as SupportBean;\n" +
                    "insert into MyWindow select * from SupportBean",
                    path);

                // insert X rows
                var maxRows = 10000; //for performance testing change to int maxRows = 100000;
                for (var i = 0; i < maxRows; i++) {
                    var bean = new SupportBean(i < 5000 ? "A" : "B", i);
                    bean.LongPrimitive = i;
                    bean.LongBoxed = (long) i + 1;
                    env.SendEventBean(bean);
                }

                env.SendEventBean(new SupportBean("B", 100));

                var eplIdx1One =
                    "on SupportBeanRange sbr select sum(IntPrimitive) as sumi from MyWindow where IntPrimitive = sbr.RangeStart";
                RunOnDemandAssertion(env, path, eplIdx1One, 1, new SupportBeanRange("R", 5501, 0), 5501);

                var eplIdx1Two =
                    "on SupportBeanRange sbr select sum(IntPrimitive) as sumi from MyWindow where IntPrimitive between sbr.RangeStart and sbr.RangeEnd";
                RunOnDemandAssertion(
                    env,
                    path,
                    eplIdx1Two,
                    1,
                    new SupportBeanRange("R", 5501, 5503),
                    5501 + 5502 + 5503);

                var eplIdx1Three =
                    "on SupportBeanRange sbr select sum(IntPrimitive) as sumi from MyWindow where TheString = Key and IntPrimitive between sbr.RangeStart and sbr.RangeEnd";
                RunOnDemandAssertion(
                    env,
                    path,
                    eplIdx1Three,
                    1,
                    new SupportBeanRange("R", "A", 4998, 5503),
                    4998 + 4999);

                var eplIdx1Four = "on SupportBeanRange sbr select sum(IntPrimitive) as sumi from MyWindow " +
                                  "where TheString = Key and LongPrimitive = RangeStart and IntPrimitive between RangeStart and RangeEnd " +
                                  "and LongBoxed between RangeStart and RangeEnd";
                RunOnDemandAssertion(env, path, eplIdx1Four, 1, new SupportBeanRange("R", "A", 4998, 5503), 4998);

                var eplIdx1Five = "on SupportBeanRange sbr select sum(IntPrimitive) as sumi from MyWindow " +
                                  "where IntPrimitive between RangeStart and RangeEnd " +
                                  "and LongBoxed between RangeStart and RangeEnd";
                RunOnDemandAssertion(
                    env,
                    path,
                    eplIdx1Five,
                    1,
                    new SupportBeanRange("R", "A", 4998, 5001),
                    4998 + 4999 + 5000);

                env.UndeployAll();
            }
        }

        internal class InfraDeletePerformance : RegressionExecution
        {
            public void Run(RegressionEnvironment env)
            {
                var epl =
                    "@Name('create') create window MyWindow#keepall as select TheString as a, IntPrimitive as b from SupportBean;\n" +
                    "on SupportBean_A delete from MyWindow where Id = a;\n" +
                    "insert into MyWindow select TheString as a, IntPrimitive as b from SupportBean;\n";
                env.CompileDeploy(epl);

                // load window
                for (var i = 0; i < 50000; i++) {
                    SendSupportBean(env, "S" + i, i);
                }

                // delete rows
                env.AddListener("create");
                var startTime = PerformanceObserver.MilliTime;
                for (var i = 0; i < 10000; i++) {
                    SendSupportBean_A(env, "S" + i);
                }

                var endTime = PerformanceObserver.MilliTime;
                var delta = endTime - startTime;
                Assert.That(delta, Is.LessThan(500), "Delta=" + delta);

                // assert they are deleted
                Assert.AreEqual(50000 - 10000, EPAssertionUtil.EnumeratorCount(env.GetEnumerator("create")));
                Assert.AreEqual(10000, env.Listener("create").OldDataList.Count);

                env.UndeployAll();
            }
        }

        internal class InfraDeletePerformanceCoercion : RegressionExecution
        {
            public void Run(RegressionEnvironment env)
            {
                var epl =
                    "@Name('create') create window MyWindow#keepall as select TheString as a, LongPrimitive as b from SupportBean;\n" +
                    "on SupportMarketDataBean delete from MyWindow where b = Price;\n" +
                    "insert into MyWindow select TheString as a, LongPrimitive as b from SupportBean;\n";
                env.CompileDeploy(epl);

                // load window
                for (var i = 0; i < 50000; i++) {
                    SendSupportBean(env, "S" + i, (long) i);
                }

                // delete rows
                env.AddListener("create");
                var startTime = PerformanceObserver.MilliTime;
                for (var i = 0; i < 10000; i++) {
                    SendMarketBean(env, "S" + i, i);
                }

                var endTime = PerformanceObserver.MilliTime;
                var delta = endTime - startTime;
                Assert.That(delta, Is.LessThan(500), "Delta=" + delta);

                // assert they are deleted
                Assert.AreEqual(50000 - 10000, EPAssertionUtil.EnumeratorCount(env.GetEnumerator("create")));
                Assert.AreEqual(10000, env.Listener("create").OldDataList.Count);

                env.UndeployAll();
            }
        }

        internal class InfraDeletePerformanceTwoDeleters : RegressionExecution
        {
            public void Run(RegressionEnvironment env)
            {
                var epl =
                    "@Name('create') create window MyWindow#keepall as select TheString as a, LongPrimitive as b from SupportBean;\n" +
                    "on SupportMarketDataBean delete from MyWindow where b = Price;\n" +
                    "on SupportBean_A delete from MyWindow where Id = a;\n" +
                    "insert into MyWindow select TheString as a, LongPrimitive as b from SupportBean;\n";
                env.CompileDeploy(epl);

                // load window
                for (var i = 0; i < 20000; i++) {
                    SendSupportBean(env, "S" + i, (long) i);
                }

                // delete all rows
                env.AddListener("create");
                var startTime = PerformanceObserver.MilliTime;
                for (var i = 0; i < 10000; i++) {
                    SendMarketBean(env, "S" + i, i);
                    SendSupportBean_A(env, "S" + (i + 10000));
                }

                var endTime = PerformanceObserver.MilliTime;
                var delta = endTime - startTime;
                Assert.That(delta, Is.LessThan(1500), "Delta=" + delta);

                // assert they are all deleted
                Assert.AreEqual(0, EPAssertionUtil.EnumeratorCount(env.GetEnumerator("create")));
                Assert.AreEqual(20000, env.Listener("create").OldDataList.Count);

                env.UndeployAll();
            }
        }

        internal class InfraDeletePerformanceIndexReuse : RegressionExecution
        {
            public void Run(RegressionEnvironment env)
            {
                var path = new RegressionPath();

                // create window
                var stmtTextCreate =
                    "@Name('create') create window MyWindow#keepall as select TheString as a, LongPrimitive as b from SupportBean";
                env.CompileDeploy(stmtTextCreate, path);

                // create delete stmt
                var statements = new string[50];
                for (var i = 0; i < statements.Length; i++) {
                    var name = "s" + i;
                    var stmtTextDelete =
                        "@Name('" + name + "') on SupportMarketDataBean delete from MyWindow where b = Price";
                    env.CompileDeploy(stmtTextDelete, path);
                    statements[i] = name;
                }

                // create insert into
                var stmtTextInsertOne =
                    "insert into MyWindow select TheString as a, LongPrimitive as b from SupportBean";
                env.CompileDeploy(stmtTextInsertOne, path);

                // load window
                var startTime = PerformanceObserver.MilliTime;
                for (var i = 0; i < 10000; i++) {
                    SendSupportBean(env, "S" + i, (long) i);
                }

                var endTime = PerformanceObserver.MilliTime;
                var delta = endTime - startTime;
                Assert.That(delta, Is.LessThan(1000), "Delta=" + delta);
                Assert.AreEqual(10000, EPAssertionUtil.EnumeratorCount(env.GetEnumerator("create")));

                // destroy all
                foreach (var statement in statements) {
                    env.UndeployModuleContaining(statement);
                }

                env.UndeployAll();
            }
        }
    }
} // end of namespace