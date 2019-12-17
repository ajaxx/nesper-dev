///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2015 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using System.Linq;

using com.espertech.esper.common.client.scopetest;
using com.espertech.esper.common.@internal.support;
using com.espertech.esper.compat;
using com.espertech.esper.regressionlib.framework;
using com.espertech.esper.regressionlib.support.bean;

using NUnit.Framework;

namespace com.espertech.esper.regressionlib.suite.resultset.querytype
{
    public class ResultSetQueryTypeRollupGroupingFuncs
    {
        public static IList<RegressionExecution> Executions()
        {
            var execs = new List<RegressionExecution>();
            execs.Add(new ResultSetQueryTypeDocSampleCarEventAndGroupingFunc());
            execs.Add(new ResultSetQueryTypeInvalid());
            execs.Add(new ResultSetQueryTypeFAFCarEventAndGroupingFunc());
            execs.Add(new ResultSetQueryTypeGroupingFuncExpressionUse());
            return execs;
        }

        internal class ResultSetQueryTypeFAFCarEventAndGroupingFunc : RegressionExecution
        {
            public void Run(RegressionEnvironment env)
            {
                var path = new RegressionPath();
                var epl = "create window CarWindow#keepall as SupportCarEvent;\n" +
                          "insert into CarWindow select * from SupportCarEvent;\n";
                env.CompileDeploy(epl, path);

                env.SendEventBean(new SupportCarEvent("skoda", "france", 10000));
                env.SendEventBean(new SupportCarEvent("skoda", "germany", 5000));
                env.SendEventBean(new SupportCarEvent("bmw", "france", 100));
                env.SendEventBean(new SupportCarEvent("bmw", "germany", 1000));
                env.SendEventBean(new SupportCarEvent("opel", "france", 7000));
                env.SendEventBean(new SupportCarEvent("opel", "germany", 7000));

                epl =
                    "@Name('s0') select Name, Place, sum(Count), grouping(Name), grouping(Place), grouping_id(Name, Place) as gId " +
                    "from CarWindow group by grouping sets((Name, Place),Name, Place,())";
                var result = env.CompileExecuteFAF(epl, path);

                Assert.AreEqual(typeof(int?), result.EventType.GetPropertyType("grouping(Name)"));
                Assert.AreEqual(typeof(int?), result.EventType.GetPropertyType("gId"));

                string[] fields = {"Name", "Place", "sum(Count)", "grouping(Name)", "grouping(Place)", "gId"};
                EPAssertionUtil.AssertPropsPerRow(
                    result.Array,
                    fields,
                    new[] {
                        new object[] {"skoda", "france", 10000, 0, 0, 0},
                        new object[] {"skoda", "germany", 5000, 0, 0, 0},
                        new object[] {"bmw", "france", 100, 0, 0, 0},
                        new object[] {"bmw", "germany", 1000, 0, 0, 0},
                        new object[] {"opel", "france", 7000, 0, 0, 0},
                        new object[] {"opel", "germany", 7000, 0, 0, 0},
                        new object[] {"skoda", null, 15000, 0, 1, 1},
                        new object[] {"bmw", null, 1100, 0, 1, 1},
                        new object[] {"opel", null, 14000, 0, 1, 1},
                        new object[] {null, "france", 17100, 1, 0, 2},
                        new object[] {null, "germany", 13000, 1, 0, 2},
                        new object[] {null, null, 30100, 1, 1, 3}
                    });

                env.UndeployAll();
            }
        }

        internal class ResultSetQueryTypeDocSampleCarEventAndGroupingFunc : RegressionExecution
        {
            public void Run(RegressionEnvironment env)
            {
                var milestone = new AtomicLong();

                // try simple
                var epl =
                    "@Name('s0') select Name, Place, sum(Count), grouping(Name), grouping(Place), grouping_id(Name,Place) as gId " +
                    "from SupportCarEvent group by grouping sets((Name, Place), Name, Place, ())";
                env.CompileDeploy(epl).AddListener("s0");

                TryAssertionDocSampleCarEvent(env, milestone);
                env.UndeployAll();

                // try audit
                env.CompileDeploy("@Audit " + epl).AddListener("s0");
                TryAssertionDocSampleCarEvent(env, milestone);
                env.UndeployAll();

                // try model
                env.EplToModelCompileDeploy(epl).AddListener("s0");

                TryAssertionDocSampleCarEvent(env, milestone);

                env.UndeployAll();
            }

            private static void TryAssertionDocSampleCarEvent(
                RegressionEnvironment env,
                AtomicLong milestone)
            {
                string[] fields = {"Name", "Place", "sum(Count)", "grouping(Name)", "grouping(Place)", "gId"};
                env.SendEventBean(new SupportCarEvent("skoda", "france", 100));
                EPAssertionUtil.AssertPropsPerRow(
                    env.Listener("s0").GetAndResetLastNewData(),
                    fields,
                    new[] {
                        new object[] {"skoda", "france", 100, 0, 0, 0},
                        new object[] {"skoda", null, 100, 0, 1, 1},
                        new object[] {null, "france", 100, 1, 0, 2},
                        new object[] {null, null, 100, 1, 1, 3}
                    });

                env.MilestoneInc(milestone);

                env.SendEventBean(new SupportCarEvent("skoda", "germany", 75));
                EPAssertionUtil.AssertPropsPerRow(
                    env.Listener("s0").GetAndResetLastNewData(),
                    fields,
                    new[] {
                        new object[] {"skoda", "germany", 75, 0, 0, 0},
                        new object[] {"skoda", null, 175, 0, 1, 1},
                        new object[] {null, "germany", 75, 1, 0, 2},
                        new object[] {null, null, 175, 1, 1, 3}
                    });
            }
        }

        internal class ResultSetQueryTypeGroupingFuncExpressionUse : RegressionExecution
        {
            public void Run(RegressionEnvironment env)
            {
                GroupingSupportFunc.Parameters.Clear();

                // test uncorrelated subquery and expression-declaration and single-row func
                var epl = "create expression myExpr {x-> '|' || x.Name || '|'};\n" +
                          "@Name('s0') select myfunc(" +
                          "  Name, Place, sum(Count), grouping(Name), grouping(Place), grouping_id(Name, Place)," +
                          "  (select RefId from SupportCarInfoEvent#lastevent), " +
                          "  myExpr(ce)" +
                          "  )" +
                          "from SupportCarEvent ce group by grouping sets((Name, Place),Name, Place,())";
                env.CompileDeploy(epl).AddListener("s0");

                env.SendEventBean(new SupportBean("E1", 1));
                env.SendEventBean(new SupportCarInfoEvent("a", "b", "c01"));

                env.SendEventBean(new SupportCarEvent("skoda", "france", 10000));
                EPAssertionUtil.AssertEqualsExactOrder(
                    new[] {
                        new object[] {"skoda", "france", 10000, 0, 0, 0, "c01", "|skoda|"},
                        new object[] {"skoda", null, 10000, 0, 1, 1, "c01", "|skoda|"},
                        new object[] {null, "france", 10000, 1, 0, 2, "c01", "|skoda|"},
                        new object[] {null, null, 10000, 1, 1, 3, "c01", "|skoda|"}
                    },
                    GroupingSupportFunc.AssertGetAndClear(4));
                env.UndeployAll();

                // test "prev" and "prior"
                var fields = new [] { "c0", "c1", "c2", "c3" };
                var eplTwo =
                    "@Name('s0') select prev(1, Name) as c0, prior(1, Name) as c1, Name as c2, sum(Count) as c3 " +
                    "from SupportCarEvent#keepall ce group by rollup(Name)";
                env.CompileDeploy(eplTwo).AddListener("s0");

                env.SendEventBean(new SupportCarEvent("skoda", "france", 10));
                EPAssertionUtil.AssertPropsPerRow(
                    env.Listener("s0").GetAndResetLastNewData(),
                    fields,
                    new[] {
                        new object[] {null, null, "skoda", 10}, new object[] {null, null, null, 10}
                    });

                env.SendEventBean(new SupportCarEvent("vw", "france", 15));
                EPAssertionUtil.AssertPropsPerRow(
                    env.Listener("s0").GetAndResetLastNewData(),
                    fields,
                    new[] {
                        new object[] {"skoda", "skoda", "vw", 15}, new object[] {"skoda", "skoda", null, 25}
                    });

                env.UndeployAll();
            }
        }

        internal class ResultSetQueryTypeInvalid : RegressionExecution
        {
            public void Run(RegressionEnvironment env)
            {
                // invalid use of function
                var expected =
                    "Failed to validate select-clause expression 'grouping(TheString)': The grouping function requires the group-by clause to specify rollup, cube or grouping sets, and may only be used in the select-clause, having-clause or order-by-clause [select grouping(TheString) from SupportBean]";
                SupportMessageAssertUtil.TryInvalidCompile(
                    env,
                    "select grouping(TheString) from SupportBean",
                    expected);
                SupportMessageAssertUtil.TryInvalidCompile(
                    env,
                    "select TheString, sum(IntPrimitive) from SupportBean(grouping(TheString) = 1) group by rollup(TheString)",
                    "Failed to validate filter expression 'grouping(TheString)=1': The grouping function requires the group-by clause to specify rollup, cube or grouping sets, and may only be used in the select-clause, having-clause or order-by-clause [select TheString, sum(IntPrimitive) from SupportBean(grouping(TheString) = 1) group by rollup(TheString)]");
                SupportMessageAssertUtil.TryInvalidCompile(
                    env,
                    "select TheString, sum(IntPrimitive) from SupportBean where grouping(TheString) = 1 group by rollup(TheString)",
                    "Failed to validate filter expression 'grouping(TheString)=1': The grouping function requires the group-by clause to specify rollup, cube or grouping sets, and may only be used in the select-clause, having-clause or order-by-clause [select TheString, sum(IntPrimitive) from SupportBean where grouping(TheString) = 1 group by rollup(TheString)]");
                SupportMessageAssertUtil.TryInvalidCompile(
                    env,
                    "select TheString, sum(IntPrimitive) from SupportBean group by rollup(grouping(TheString))",
                    "The grouping function requires the group-by clause to specify rollup, cube or grouping sets, and may only be used in the select-clause, having-clause or order-by-clause [select TheString, sum(IntPrimitive) from SupportBean group by rollup(grouping(TheString))]");

                // invalid parameters
                SupportMessageAssertUtil.TryInvalidCompile(
                    env,
                    "select TheString, sum(IntPrimitive), grouping(LongPrimitive) from SupportBean group by rollup(TheString)",
                    "Failed to find expression 'LongPrimitive' among group-by expressions");
                SupportMessageAssertUtil.TryInvalidCompile(
                    env,
                    "select TheString, sum(IntPrimitive), grouping(TheString||'x') from SupportBean group by rollup(TheString)",
                    "Failed to find expression 'TheString||\"x\"' among group-by expressions [select TheString, sum(IntPrimitive), grouping(TheString||'x') from SupportBean group by rollup(TheString)]");

                SupportMessageAssertUtil.TryInvalidCompile(
                    env,
                    "select TheString, sum(IntPrimitive), grouping_id(TheString, TheString) from SupportBean group by rollup(TheString)",
                    "Duplicate expression 'TheString' among grouping function parameters [select TheString, sum(IntPrimitive), grouping_id(TheString, TheString) from SupportBean group by rollup(TheString)]");
            }
        }

        public class GroupingSupportFunc
        {
            public static IList<object[]> Parameters { get; } = new List<object[]>();

            public static void Myfunc(
                string name,
                string place,
                int? cnt,
                int? grpName,
                int? grpPlace,
                int? grpId,
                string refId,
                string namePlusDelim)
            {
                Parameters.Add(new object[] {name, place, cnt, grpName, grpPlace, grpId, refId, namePlusDelim});
            }

            public static object[][] AssertGetAndClear(int numRows)
            {
                Assert.AreEqual(numRows, Parameters.Count);
                var result = Parameters.ToArray();
                Parameters.Clear();
                return result;
            }
        }
    }
} // end of namespace