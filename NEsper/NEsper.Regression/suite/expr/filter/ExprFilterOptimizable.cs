///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2015 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

using com.espertech.esper.common.client.hook.expr;
using com.espertech.esper.common.@internal.filterspec;
using com.espertech.esper.common.@internal.support;
using com.espertech.esper.common.@internal.util;
using com.espertech.esper.compat;
using com.espertech.esper.compat.collections;
using com.espertech.esper.compat.concurrency;
using com.espertech.esper.compat.logging;
using com.espertech.esper.compat.threading;
using com.espertech.esper.regressionlib.framework;
using com.espertech.esper.regressionlib.support.bean;
using com.espertech.esper.regressionlib.support.epl;
using com.espertech.esper.regressionlib.support.filter;
using com.espertech.esper.runtime.client;
using com.espertech.esper.runtime.client.option;
using com.espertech.esper.runtime.client.scopetest;
using com.espertech.esper.runtime.@internal.filtersvcimpl;
using com.espertech.esper.runtime.@internal.kernel.statement;

using NUnit.Framework;

using static com.espertech.esper.common.@internal.compile.stage2.FilterSpecCompilerPlanner;
using static com.espertech.esper.regressionlib.framework.SupportMessageAssertUtil;


namespace com.espertech.esper.regressionlib.suite.expr.filter
{
    public class ExprFilterOptimizable
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static EPLMethodInvocationContext methodInvocationContextFilterOptimized;

        public static IList<RegressionExecution> Executions()
        {
            var executions = new List<RegressionExecution>();
            executions.Add(new ExprFilterInAndNotInKeywordMultivalue());
            executions.Add(new ExprFilterOptimizablePerf());
            executions.Add(new ExprFilterOptimizableInspectFilter());
            executions.Add(new ExprFilterOrRewrite());
            executions.Add(new ExprFilterOrToInRewrite());
            executions.Add(new ExprFilterOrPerformance());
            executions.Add(new ExprFilterOrContext());
            executions.Add(new ExprFilterPatternUDFFilterOptimizable());
            executions.Add(new ExprFilterDeployTimeConstant()); // substitution and variables are here
            return executions;
        }

        private static void RunAssertionBetweenWSubsWNumeric(
            RegressionEnvironment env,
            string epl)
        {
            CompileDeployWSubstitution(env, epl, CollectionUtil.BuildMap("p0", 10, "p1", 11));
            AssertFilterSingle(env.Statement("s0"), epl, "IntPrimitive", FilterOperator.RANGE_CLOSED);
            TryAssertionWSubsFrom9To12(env);
            env.UndeployAll();
        }

        private static void RunAssertionBetweenWVariableWNumeric(
            RegressionEnvironment env,
            string epl)
        {
            env.CompileDeploy("@Name('s0') " + epl).AddListener("s0");
            AssertFilterSingle(env.Statement("s0"), epl, "IntPrimitive", FilterOperator.RANGE_CLOSED);
            TryAssertionWSubsFrom9To12(env);
            env.UndeployAll();
        }

        private static void RunAssertionBetweenWSubsWString(
            RegressionEnvironment env,
            string epl)
        {
            CompileDeployWSubstitution(env, epl, CollectionUtil.BuildMap("p0", "c", "p1", "d"));
            TryAssertionBetweenDeplotTimeConst(env, epl);
        }

        private static void RunAssertionBetweenWVariableWString(
            RegressionEnvironment env,
            string epl)
        {
            env.CompileDeploy("@Name('s0') " + epl).AddListener("s0");
            TryAssertionBetweenDeplotTimeConst(env, epl);
        }

        private static void TryAssertionBetweenDeplotTimeConst(
            RegressionEnvironment env,
            string epl)
        {
            AssertFilterSingle(env.Statement("s0"), epl, "TheString", FilterOperator.RANGE_CLOSED);

            env.SendEventBean(new SupportBean("b", 0));
            Assert.IsFalse(env.Listener("s0").GetAndClearIsInvoked());

            env.SendEventBean(new SupportBean("c", 0));
            Assert.IsTrue(env.Listener("s0").GetAndClearIsInvoked());

            env.SendEventBean(new SupportBean("d", 0));
            Assert.IsTrue(env.Listener("s0").GetAndClearIsInvoked());

            env.SendEventBean(new SupportBean("e", 0));
            Assert.IsFalse(env.Listener("s0").GetAndClearIsInvoked());

            env.UndeployAll();
        }

        private static void RunAssertionInWSubsWArray(
            RegressionEnvironment env,
            string epl)
        {
            CompileDeployWSubstitution(env, epl, CollectionUtil.BuildMap("p0", new[] {10, 11}));
            AssertFilterSingle(env.Statement("s0"), epl, "IntPrimitive", FilterOperator.IN_LIST_OF_VALUES);
            TryAssertionWSubsFrom9To12(env);
            env.UndeployAll();
        }

        private static void RunAssertionInWVariableWArray(
            RegressionEnvironment env,
            string epl)
        {
            env.CompileDeploy("@Name('s0') " + epl).AddListener("s0");
            AssertFilterSingle(env.Statement("s0"), epl, "IntPrimitive", FilterOperator.IN_LIST_OF_VALUES);
            TryAssertionWSubsFrom9To12(env);
            env.UndeployAll();
        }

        private static void TryAssertionWSubsFrom9To12(RegressionEnvironment env)
        {
            env.SendEventBean(new SupportBean("E1", 9));
            Assert.IsFalse(env.Listener("s0").GetAndClearIsInvoked());

            env.SendEventBean(new SupportBean("E2", 10));
            Assert.IsTrue(env.Listener("s0").GetAndClearIsInvoked());

            env.SendEventBean(new SupportBean("E3", 11));
            Assert.IsTrue(env.Listener("s0").GetAndClearIsInvoked());

            env.SendEventBean(new SupportBean("E1", 12));
            Assert.IsFalse(env.Listener("s0").GetAndClearIsInvoked());
        }

        private static void RunAssertionInWSubs(
            RegressionEnvironment env,
            string epl)
        {
            CompileDeployWSubstitution(env, epl, CollectionUtil.BuildMap("p0", 10, "p1", 11));
            AssertFilterSingle(env.Statement("s0"), epl, "IntPrimitive", FilterOperator.IN_LIST_OF_VALUES);
            TryAssertionWSubsFrom9To12(env);
            env.UndeployAll();
        }

        private static void RunAssertionInWVariable(
            RegressionEnvironment env,
            string epl)
        {
            env.CompileDeploy("@Name('s0') " + epl).AddListener("s0");
            AssertFilterSingle(env.Statement("s0"), epl, "IntPrimitive", FilterOperator.IN_LIST_OF_VALUES);
            TryAssertionWSubsFrom9To12(env);
            env.UndeployAll();
        }

        private static void RunAssertionRelOpWSubs(
            RegressionEnvironment env,
            string epl)
        {
            CompileDeployWSubstitution(env, epl, CollectionUtil.BuildMap("p0", 10));
            TryAssertionRelOpWDeployTimeConst(env, epl);
        }

        private static void RunAssertionRelOpWVariable(
            RegressionEnvironment env,
            string epl)
        {
            env.CompileDeploy("@Name('s0') " + epl).AddListener("s0");
            TryAssertionRelOpWDeployTimeConst(env, epl);
        }

        private static void TryAssertionRelOpWDeployTimeConst(
            RegressionEnvironment env,
            string epl)
        {
            AssertFilterSingle(env.Statement("s0"), epl, "IntPrimitive", FilterOperator.GREATER);

            env.SendEventBean(new SupportBean("E1", 10));
            Assert.IsFalse(env.Listener("s0").GetAndClearIsInvoked());

            env.SendEventBean(new SupportBean("E2", 11));
            Assert.IsTrue(env.Listener("s0").GetAndClearIsInvoked());

            env.UndeployAll();
        }

        private static void RunAssertionEqualsWSubs(
            RegressionEnvironment env,
            string epl)
        {
            CompileDeployWSubstitution(env, epl, CollectionUtil.BuildMap("p0", "abc"));
            TryAssertionEqualsWDeployTimeConst(env, epl);
        }

        private static void RunAssertionEqualsWVariable(
            RegressionEnvironment env,
            string epl)
        {
            env.CompileDeploy("@Name('s0') " + epl).AddListener("s0");
            TryAssertionEqualsWDeployTimeConst(env, epl);
        }

        private static void TryAssertionEqualsWDeployTimeConst(
            RegressionEnvironment env,
            string epl)
        {
            AssertFilterSingle(env.Statement("s0"), epl, "TheString", FilterOperator.EQUAL);

            env.SendEventBean(new SupportBean("abc", 0));
            Assert.IsTrue(env.Listener("s0").GetAndClearIsInvoked());

            env.SendEventBean(new SupportBean("x", 0));
            Assert.IsFalse(env.Listener("s0").GetAndClearIsInvoked());

            env.UndeployAll();
        }

        private static void RunAssertionEqualsWSubsWCoercion(
            RegressionEnvironment env,
            string epl)
        {
            CompileDeployWSubstitution(env, epl, CollectionUtil.BuildMap("p0", 100));
            AssertFilterSingle(env.Statement("s0"), epl, "LongPrimitive", FilterOperator.EQUAL);

            var sb = new SupportBean();
            sb.LongPrimitive = 100;
            env.SendEventBean(sb);
            Assert.IsTrue(env.Listener("s0").GetAndClearIsInvoked());

            env.UndeployAll();
        }

        private static void TryOrRewriteHint(
            RegressionEnvironment env,
            AtomicLong milestone)
        {
            var epl = "@Hint('MAX_FILTER_WIDTH=0') @Name('s0') select * from SupportBean_IntAlphabetic((B=1 or C=1) and (D=1 or E=1))";
            env.CompileDeployAddListenerMile(epl, "s0", milestone.GetAndIncrement());
            AssertFilterSingle(env.Statement("s0"), epl, ".boolean_expression", FilterOperator.BOOLEAN_EXPRESSION);
            env.UndeployAll();
        }

        private static void TryOrRewriteSubquery(
            RegressionEnvironment env,
            AtomicLong milestone)
        {
            var epl = "@Name('s0') select (select * from SupportBean_IntAlphabetic(A=1 or B=1)#keepall) as c0 from SupportBean";
            env.CompileDeployAddListenerMile(epl, "s0", milestone.GetAndIncrement());

            var iaOne = IntEvent(1, 1);
            env.SendEventBean(iaOne);
            env.SendEventBean(new SupportBean());
            Assert.AreEqual(iaOne, env.Listener("s0").AssertOneGetNewAndReset().Get("c0"));

            env.UndeployAll();
        }

        private static void TryOrRewriteContextPartitionedCategory(
            RegressionEnvironment env,
            AtomicLong milestone)
        {
            var epl = "@Name('ctx') create context MyContext \n" +
                      "  group A=1 or B=1 as g1,\n" +
                      "  group C=1 as g1\n" +
                      "  from SupportBean_IntAlphabetic;" +
                      "@Name('s0') context MyContext select * from SupportBean_IntAlphabetic(D=1 or E=1)";
            env.CompileDeployAddListenerMile(epl, "s0", milestone.GetAndIncrement());

            SendAssertEvents(
                env,
                new object[] {
                    IntEvent(1, 0, 0, 0, 1),
                    IntEvent(0, 1, 0, 1, 0), 
                    IntEvent(0, 0, 1, 1, 1)
                },
                new object[] {
                    IntEvent(0, 0, 0, 1, 0), 
                    IntEvent(1, 0, 0, 0, 0), 
                    IntEvent(0, 0, 1, 0, 0)
                }
            );

            env.UndeployAll();
        }

        private static void TryOrRewriteContextPartitionedHash(
            RegressionEnvironment env,
            AtomicLong milestone)
        {
            var epl = "create context MyContext " +
                      "coalesce by consistent_hash_crc32(A) from SupportBean_IntAlphabetic(B=1) granularity 16 preallocate;" +
                      "@Name('s0') context MyContext select * from SupportBean_IntAlphabetic(C=1 or D=1)";
            env.CompileDeployAddListenerMile(epl, "s0", milestone.GetAndIncrement());

            SendAssertEvents(
                env,
                new object[] {
                    IntEvent(100, 1, 0, 1),
                    IntEvent(100, 1, 1, 0)
                },
                new object[] {
                    IntEvent(100, 0, 0, 1), 
                    IntEvent(100, 1, 0, 0)
                }
            );
            env.UndeployAll();
        }

        private static void TryOrRewriteContextPartitionedSegmented(
            RegressionEnvironment env,
            AtomicLong milestone)
        {
            var epl = "create context MyContext partition by A from SupportBean_IntAlphabetic(B=1 or C=1);" +
                      "@Name('s0') context MyContext select * from SupportBean_IntAlphabetic(D=1)";
            env.CompileDeployAddListenerMile(epl, "s0", milestone.GetAndIncrement());

            SendAssertEvents(
                env,
                new object[] {
                    IntEvent(100, 1, 0, 1), 
                    IntEvent(100, 0, 1, 1)
                },
                new object[] {
                    IntEvent(100, 0, 0, 1),
                    IntEvent(100, 1, 0, 0)
                }
            );
            env.UndeployAll();
        }

        private static void TryOrRewriteBooleanExprAnd(
            RegressionEnvironment env,
            AtomicLong milestone)
        {
            string[] filters = {
                "(A='a' or A like 'A%') and (B='b' or B like 'B%')"
            };
            foreach (var filter in filters) {
                var epl = $"@Name('s0') select * from SupportBean_StringAlphabetic({filter})";
                env.CompileDeployAddListenerMile(epl, "s0", milestone.GetAndIncrement());
                SupportFilterHelper.AssertFilterMulti(
                    env.Statement("s0"),
                    "SupportBean_StringAlphabetic",
                    new[] {
                        new[] {
                            new FilterItem("A", FilterOperator.EQUAL),
                            new FilterItem("B", FilterOperator.EQUAL)
                        },
                        new[] {
                            new FilterItem("A", FilterOperator.EQUAL),
                            FilterItem.BoolExprFilterItem
                        },
                        new[] {
                            new FilterItem("B", FilterOperator.EQUAL),
                            FilterItem.BoolExprFilterItem
                        },
                        new[] {
                            FilterItem.BoolExprFilterItem
                        }
                    });

                SendAssertEvents(
                    env,
                    new object[] {
                        StringEvent("a", "b"), 
                        StringEvent("A1", "b"), 
                        StringEvent("a", "B1"), 
                        StringEvent("A1", "B1")
                    },
                    new object[] {
                        StringEvent("x", "b"), 
                        StringEvent("a", "x"),
                        StringEvent("A1", "C"),
                        StringEvent("C", "B1")
                    }
                );
                env.UndeployAll();
            }
        }

        private static void TryOrRewriteBooleanExprSimple(
            RegressionEnvironment env,
            AtomicLong milestone)
        {
            string[] filters = {
                "A like 'a%' and (B='b' or C='c')"
            };
            foreach (var filter in filters) {
                var epl = $"@Name('s0') select * from SupportBean_StringAlphabetic({filter})";
                env.CompileDeployAddListenerMile(epl, "s0", milestone.GetAndIncrement());
                SupportFilterHelper.AssertFilterMulti(
                    env.Statement("s0"),
                    "SupportBean_StringAlphabetic",
                    new[] {
                        new[] {
                            new FilterItem("B", FilterOperator.EQUAL), FilterItem.BoolExprFilterItem
                        },
                        new[] {
                            new FilterItem("C", FilterOperator.EQUAL), FilterItem.BoolExprFilterItem
                        }
                    });

                SendAssertEvents(
                    env,
                    new object[] {
                        StringEvent("a1", "b", null), 
                        StringEvent("a1", null, "c")
                    },
                    new object[] {
                        StringEvent("x", "b", null),
                        StringEvent("a1", null, null),
                        StringEvent("a1", null, "x")
                    }
                );
                env.UndeployAll();
            }
        }

        private static void TryOrRewriteAndRewriteNotEquals(
            RegressionEnvironment env,
            AtomicLong milestone)
        {
            TryOrRewriteAndRewriteNotEqualsOr(env, milestone);

            TryOrRewriteAndRewriteNotEqualsConsolidate(env, milestone);

            TryOrRewriteAndRewriteNotEqualsWithOrConsolidateSecond(env, milestone);
        }

        private static void TryOrRewriteAndRewriteNotEqualsWithOrConsolidateSecond(
            RegressionEnvironment env,
            AtomicLong milestone)
        {
            string[] filters = {
                "A!=1 and A!=2 and ((A!=3 and A!=4) or (A!=5 and A!=6))"
            };
            foreach (var filter in filters) {
                var epl = $"@Name('s0') select * from SupportBean_IntAlphabetic({filter})";
                env.CompileDeployAddListenerMile(epl, "s0", milestone.GetAndIncrement());
                SupportFilterHelper.AssertFilterMulti(
                    env.Statement("s0"),
                    "SupportBean_IntAlphabetic",
                    new[] {
                        new[] {
                            new FilterItem("A", FilterOperator.NOT_IN_LIST_OF_VALUES), FilterItem.BoolExprFilterItem
                        },
                        new[] {
                            new FilterItem("A", FilterOperator.NOT_IN_LIST_OF_VALUES), FilterItem.BoolExprFilterItem
                        }
                    });

                SendAssertEvents(
                    env,
                    new object[] {
                        IntEvent(3), 
                        IntEvent(4), 
                        IntEvent(0)
                    },
                    new object[] {
                        IntEvent(2), 
                        IntEvent(1)
                    }
                );
                env.UndeployAll();
            }
        }

        private static void TryOrRewriteAndRewriteNotEqualsConsolidate(
            RegressionEnvironment env,
            AtomicLong milestone)
        {
            string[] filters = {
                "A!=1 and A!=2 and (A!=3 or A!=4)"
            };
            foreach (var filter in filters) {
                var epl = $"@Name('s0') select * from SupportBean_IntAlphabetic({filter})";
                env.CompileDeployAddListenerMile(epl, "s0", milestone.GetAndIncrement());
                SupportFilterHelper.AssertFilterMulti(
                    env.Statement("s0"),
                    "SupportBean_IntAlphabetic",
                    new[] {
                        new[] {
                            new FilterItem("A", FilterOperator.NOT_IN_LIST_OF_VALUES),
                            new FilterItem("A", FilterOperator.NOT_EQUAL)
                        },
                        new[] {
                            new FilterItem("A", FilterOperator.NOT_IN_LIST_OF_VALUES),
                            new FilterItem("A", FilterOperator.NOT_EQUAL)
                        }
                    });

                SendAssertEvents(
                    env,
                    new object[] {
                        IntEvent(3), 
                        IntEvent(4), 
                        IntEvent(0)
                    },
                    new object[] {
                        IntEvent(2), 
                        IntEvent(1)
                    }
                );
                env.UndeployAll();
            }
        }

        private static void TryOrRewriteAndRewriteNotEqualsOr(
            RegressionEnvironment env,
            AtomicLong milestone)
        {
            string[] filters = {
                "A!=1 and A!=2 and (B=1 or C=1)"
            };
            foreach (var filter in filters) {
                var epl = $"@Name('s0') select * from SupportBean_IntAlphabetic({filter})";
                env.CompileDeployAddListenerMile(epl, "s0", milestone.GetAndIncrement());
                SupportFilterHelper.AssertFilterMulti(
                    env.Statement("s0"),
                    "SupportBean_IntAlphabetic",
                    new[] {
                        new[] {
                            new FilterItem("A", FilterOperator.NOT_IN_LIST_OF_VALUES),
                            new FilterItem("B", FilterOperator.EQUAL)
                        },
                        new[] {
                            new FilterItem("A", FilterOperator.NOT_IN_LIST_OF_VALUES),
                            new FilterItem("C", FilterOperator.EQUAL)
                        }
                    });

                SendAssertEvents(
                    env,
                    new object[] {
                        IntEvent(3, 1, 0), 
                        IntEvent(3, 0, 1), 
                        IntEvent(0, 1, 0)
                    },
                    new object[] {
                        IntEvent(2, 0, 0), 
                        IntEvent(1, 0, 0), 
                        IntEvent(3, 0, 0)
                    }
                );
                env.UndeployAll();
            }
        }

        private static void TryOrRewriteAndRewriteInnerOr(
            RegressionEnvironment env,
            AtomicLong milestone)
        {
            string[] filtersAB = {
                "TheString='a' and (IntPrimitive=1 or LongPrimitive=10)"
            };
            foreach (var filter in filtersAB) {
                var epl = $"@Name('s0') select * from SupportBean({filter})";
                env.CompileDeployAddListenerMile(epl, "s0", milestone.GetAndIncrement());
                SupportFilterHelper.AssertFilterMulti(
                    env.Statement("s0"),
                    "SupportBean",
                    new[] {
                        new[] {
                            new FilterItem("TheString", FilterOperator.EQUAL),
                            new FilterItem("IntPrimitive", FilterOperator.EQUAL)
                        },
                        new[] {
                            new FilterItem("TheString", FilterOperator.EQUAL),
                            new FilterItem("LongPrimitive", FilterOperator.EQUAL)
                        }
                    });

                SendAssertEvents(
                    env,
                    new[] {
                        MakeEvent("a", 1, 0),
                        MakeEvent("a", 0, 10),
                        MakeEvent("a", 1, 10)
                    },
                    new[] {
                        MakeEvent("x", 0, 0),
                        MakeEvent("a", 2, 20),
                        MakeEvent("x", 1, 10)
                    }
                );
                env.UndeployAll();
            }
        }

        private static void TryOrRewriteOrRewriteAndOrMulti(
            RegressionEnvironment env,
            AtomicLong milestone)
        {
            string[] filtersAB = {
                "A=1 and (B=1 or C=1) and (D=1 or E=1)"
            };
            foreach (var filter in filtersAB) {
                var epl = $"@Name('s0') select * from SupportBean_IntAlphabetic({filter})";
                env.CompileDeployAddListenerMile(epl, "s0", milestone.GetAndIncrement());
                SupportFilterHelper.AssertFilterMulti(
                    env.Statement("s0"),
                    "SupportBean_IntAlphabetic",
                    new[] {
                        new[] {
                            new FilterItem("A", FilterOperator.EQUAL), new FilterItem("B", FilterOperator.EQUAL),
                            new FilterItem("D", FilterOperator.EQUAL)
                        },
                        new[] {
                            new FilterItem("A", FilterOperator.EQUAL), new FilterItem("C", FilterOperator.EQUAL),
                            new FilterItem("D", FilterOperator.EQUAL)
                        },
                        new[] {
                            new FilterItem("A", FilterOperator.EQUAL), new FilterItem("C", FilterOperator.EQUAL),
                            new FilterItem("E", FilterOperator.EQUAL)
                        },
                        new[] {
                            new FilterItem("A", FilterOperator.EQUAL), new FilterItem("B", FilterOperator.EQUAL),
                            new FilterItem("E", FilterOperator.EQUAL)
                        }
                    });

                SendAssertEvents(
                    env,
                    new object[] {
                        IntEvent(1, 1, 0, 1, 0), 
                        IntEvent(1, 0, 1, 0, 1),
                        IntEvent(1, 1, 0, 0, 1),
                        IntEvent(1, 0, 1, 1, 0)
                    },
                    new object[] {
                        IntEvent(1, 0, 0, 1, 0), 
                        IntEvent(1, 0, 0, 1, 0), 
                        IntEvent(1, 1, 1, 0, 0),
                        IntEvent(0, 1, 1, 1, 1)
                    }
                );
                env.UndeployAll();
            }
        }

        private static void TryOrRewriteOrRewriteEightOr(
            RegressionEnvironment env,
            AtomicLong milestone)
        {
            string[] filtersAB = {
                "TheString = 'a' or IntPrimitive=1 or LongPrimitive=10 or DoublePrimitive=100 or BoolPrimitive=true or " +
                "IntBoxed=2 or LongBoxed=20 or DoubleBoxed=200",
                "LongBoxed=20 or TheString = 'a' or BoolPrimitive=true or IntBoxed=2 or LongPrimitive=10 or DoublePrimitive=100 or " +
                "IntPrimitive=1 or DoubleBoxed=200"
            };
            foreach (var filter in filtersAB) {
                var epl = $"@Name('s0') select * from SupportBean({filter})";
                env.CompileDeployAddListenerMile(epl, "s0", milestone.GetAndIncrement());
                SupportFilterHelper.AssertFilterMulti(
                    env.Statement("s0"),
                    "SupportBean",
                    new[] {
                        new[] {new FilterItem("TheString", FilterOperator.EQUAL)},
                        new[] {new FilterItem("IntPrimitive", FilterOperator.EQUAL)},
                        new[] {new FilterItem("LongPrimitive", FilterOperator.EQUAL)},
                        new[] {new FilterItem("DoublePrimitive", FilterOperator.EQUAL)},
                        new[] {new FilterItem("BoolPrimitive", FilterOperator.EQUAL)},
                        new[] {new FilterItem("IntBoxed", FilterOperator.EQUAL)},
                        new[] {new FilterItem("LongBoxed", FilterOperator.EQUAL)},
                        new[] {new FilterItem("DoubleBoxed", FilterOperator.EQUAL)}
                    });

                SendAssertEvents(
                    env,
                    new[] {
                        MakeEvent("a", 1, 10, 100, true, 2, 20, 200),
                        MakeEvent("a", 0, 0, 0, true, 0, 0, 0),
                        MakeEvent("a", 0, 0, 0, true, 0, 20, 0),
                        MakeEvent("x", 0, 0, 100, false, 0, 0, 0),
                        MakeEvent("x", 1, 0, 0, false, 0, 0, 200),
                        MakeEvent("x", 0, 0, 0, false, 0, 0, 200)
                    },
                    new[] {MakeEvent("x", 0, 0, 0, false, 0, 0, 0)}
                );
                env.UndeployAll();
            }
        }

        private static void TryOrRewriteOrRewriteFourOr(
            RegressionEnvironment env,
            AtomicLong milestone)
        {
            string[] filtersAB = {
                "TheString = 'a' or IntPrimitive=1 or LongPrimitive=10 or DoublePrimitive=100"
            };
            foreach (var filter in filtersAB) {
                var epl = $"@Name('s0') select * from SupportBean({filter})";
                env.CompileDeployAddListenerMile(epl, "s0", milestone.GetAndIncrement());
                SupportFilterHelper.AssertFilterMulti(
                    env.Statement("s0"),
                    "SupportBean",
                    new[] {
                        new[] {
                            new FilterItem("TheString", FilterOperator.EQUAL)
                        },
                        new[] {
                            new FilterItem("IntPrimitive", FilterOperator.EQUAL)
                        },
                        new[] {
                            new FilterItem("LongPrimitive", FilterOperator.EQUAL)
                        },
                        new[] {
                            new FilterItem("DoublePrimitive", FilterOperator.EQUAL)
                        }
                    });

                SendAssertEvents(
                    env,
                    new[] {
                        MakeEvent("a", 1, 10, 100),
                        MakeEvent("x", 0, 0, 100), 
                        MakeEvent("x", 0, 10, 100),
                        MakeEvent("a", 0, 0, 0)
                    },
                    new[] {
                        MakeEvent("x", 0, 0, 0)
                    }
                );
                env.UndeployAll();
            }
        }

        private static void AssertFilterSingle(
            EPStatement stmt,
            string epl,
            string expression,
            FilterOperator op)
        {
            var statementSPI = (EPStatementSPI) stmt;
            var param = SupportFilterHelper.GetFilterSingle(statementSPI);
            Assert.AreEqual(op, param.Op, "failed for '" + epl + "'");
            Assert.AreEqual(expression, param.Name);
        }

        private static void TryOrRewriteContextPartitionedInitiated(
            RegressionEnvironment env,
            AtomicLong milestone)
        {
            var epl =
                "@Name('ctx') create context MyContext initiated by SupportBean(TheString='A' or IntPrimitive=1) terminated after 24 hours;\n" +
                "@Name('s0') context MyContext select * from SupportBean;\n";
            env.CompileDeployAddListenerMile(epl, "s0", milestone.GetAndIncrement());

            env.SendEventBean(new SupportBean("A", 1));
            env.Listener("s0").AssertOneGetNewAndReset();

            env.UndeployAll();
        }

        private static void TryOrRewriteContextPartitionedInitiatedSameEvent(
            RegressionEnvironment env,
            AtomicLong milestone)
        {
            var epl = "create context MyContext initiated by SupportBean terminated after 24 hours;" +
                      "@Name('s0') context MyContext select * from SupportBean(TheString='A' or IntPrimitive=1)";
            env.CompileDeployAddListenerMile(epl, "s0", milestone.GetAndIncrement());

            env.SendEventBean(new SupportBean("A", 1));
            env.Listener("s0").AssertOneGetNewAndReset();

            env.UndeployAll();
        }

        private static void TryInKeyword(
            RegressionEnvironment env,
            string field,
            SupportInKeywordBean prototype,
            AtomicLong milestone)
        {
            TryInKeywordPlain(env, field, prototype, milestone);
            TryInKeywordPattern(env, field, prototype, milestone);
        }

        private static void TryOrRewriteOrRewriteThreeOr(
            RegressionEnvironment env,
            AtomicLong milestone)
        {
            string[] filtersAB = {
                "TheString = 'a' or IntPrimitive = 1 or LongPrimitive = 2",
                "2 = LongPrimitive or 1 = IntPrimitive or TheString = 'a'"
            };
            foreach (var filter in filtersAB) {
                var epl = $"@Name('s0') select * from SupportBean({filter})";
                env.CompileDeployAddListenerMile(epl, "s0", milestone.GetAndIncrement());
                SupportFilterHelper.AssertFilterMulti(
                    env.Statement("s0"),
                    "SupportBean",
                    new[] {
                        new[] {
                            new FilterItem("IntPrimitive", FilterOperator.EQUAL)
                        },
                        new[] {
                            new FilterItem("TheString", FilterOperator.EQUAL)
                        },
                        new[] {
                            new FilterItem("LongPrimitive", FilterOperator.EQUAL)
                        }
                    });

                SendAssertEvents(
                    env,
                    new[] {
                        MakeEvent("a", 0, 0),
                        MakeEvent("b", 1, 0), 
                        MakeEvent("c", 0, 2),
                        MakeEvent("c", 0, 2)
                    },
                    new[] {
                        MakeEvent("v", 0, 0), 
                        MakeEvent("c", 2, 1)
                    }
                );

                env.UndeployAll();
            }
        }

        private static void SendAssertEvents(
            RegressionEnvironment env,
            object[] matches,
            object[] nonMatches)
        {
            env.Listener("s0").Reset();
            foreach (var match in matches) {
                env.SendEventBean(match);
                Assert.AreSame(match, env.Listener("s0").AssertOneGetNewAndReset().Underlying);
            }

            env.Listener("s0").Reset();
            foreach (var nonMatch in nonMatches) {
                env.SendEventBean(nonMatch);
                Assert.IsFalse(env.Listener("s0").IsInvoked);
            }
        }

        private static void TryInKeywordPattern(
            RegressionEnvironment env,
            string field,
            SupportInKeywordBean prototype,
            AtomicLong milestone)
        {
            var epl = $"@Name('s0') select * from pattern[every a=SupportInKeywordBean -> SupportBean(IntPrimitive in (a.{field}))]";
            env.CompileDeployAddListenerMile(epl, "s0", milestone.GetAndIncrement());

            AssertInKeywordReceivedPattern(env, env.CopyMayFail(prototype), 1, true);
            AssertInKeywordReceivedPattern(env, env.CopyMayFail(prototype), 2, true);
            AssertInKeywordReceivedPattern(env, env.CopyMayFail(prototype), 3, false);

            SupportFilterHelper.AssertFilterMulti(
                env.Statement("s0"),
                "SupportBean",
                new[] {
                    new[] {new FilterItem("IntPrimitive", FilterOperator.IN_LIST_OF_VALUES)}
                });

            env.UndeployAll();
        }

        private static void TryInKeywordPlain(
            RegressionEnvironment env,
            string field,
            SupportInKeywordBean prototype,
            AtomicLong milestone)
        {
            var epl = $"@Name('s0') select * from SupportInKeywordBean#length(2) where 1 in ({field})";
            env.CompileDeployAddListenerMile(epl, "s0", milestone.GetAndIncrement());

            env.SendEventBean(env.CopyMayFail(prototype));
            Assert.IsTrue(env.Listener("s0").IsInvokedAndReset());

            env.UndeployAll();
        }

        private static void TryNotInKeyword(
            RegressionEnvironment env,
            string field,
            SupportInKeywordBean prototype,
            AtomicLong milestone)
        {
            TryNotInKeywordPlain(env, field, prototype, milestone);
            TryNotInKeywordPattern(env, field, prototype, milestone);
        }

        private static void TryNotInKeywordPlain(
            RegressionEnvironment env,
            string field,
            SupportInKeywordBean prototype,
            AtomicLong milestone)
        {
            var epl = $"@Name('s0') select * from SupportInKeywordBean#length(2) where 1 not in ({field})";
            env.CompileDeployAddListenerMile(epl, "s0", milestone.GetAndIncrement());

            env.SendEventBean(env.CopyMayFail(prototype));
            Assert.IsFalse(env.Listener("s0").IsInvokedAndReset());

            env.UndeployAll();
        }

        private static void TryNotInKeywordPattern(
            RegressionEnvironment env,
            string field,
            SupportInKeywordBean prototype,
            AtomicLong milestone)
        {
            var epl = $"@Name('s0') select * from pattern[every a=SupportInKeywordBean -> SupportBean(IntPrimitive not in (a.{field}))]";
            env.CompileDeployAddListenerMile(epl, "s0", milestone.GetAndIncrement());

            AssertInKeywordReceivedPattern(env, env.CopyMayFail(prototype), 0, true);
            AssertInKeywordReceivedPattern(env, env.CopyMayFail(prototype), 3, true);

            AssertInKeywordReceivedPattern(env, env.CopyMayFail(prototype), 1, false);
            SupportFilterHelper.AssertFilterMulti(
                env.Statement("s0"),
                "SupportBean",
                new[] {
                    new[] {
                        new FilterItem("IntPrimitive", FilterOperator.NOT_IN_LIST_OF_VALUES)
                    }
                });

            env.UndeployAll();
        }

        private static void TryOrRewriteOrRewriteThreeWithOverlap(
            RegressionEnvironment env,
            AtomicLong milestone)
        {
            string[] filtersAB = {
                "TheString = 'a' or TheString = 'b' or IntPrimitive=1",
                "IntPrimitive = 1 or TheString = 'b' or TheString = 'a'"
            };
            foreach (var filter in filtersAB) {
                var epl = $"@Name('s0') select * from SupportBean({filter})";
                env.CompileDeployAddListenerMile(epl, "s0", milestone.GetAndIncrement());
                SupportFilterHelper.AssertFilterMulti(
                    env.Statement("s0"),
                    "SupportBean",
                    new[] {
                        new[] {
                            new FilterItem("TheString", FilterOperator.EQUAL)
                        },
                        new[] {
                            new FilterItem("TheString", FilterOperator.EQUAL)
                        },
                        new[] {
                            new FilterItem("IntPrimitive", FilterOperator.EQUAL)
                        }
                    });

                SendAssertEvents(
                    env,
                    new[] {MakeEvent("a", 1), MakeEvent("b", 0), MakeEvent("x", 1)},
                    new[] {MakeEvent("x", 0)}
                );
                env.UndeployAll();
            }
        }

        private static void TryInArrayContextProvided(
            RegressionEnvironment env,
            AtomicLong milestone)
        {
            var epl = "create context MyContext initiated by SupportInKeywordBean as mie terminated after 24 hours;\n" +
                      "@Name('s1') context MyContext select * from SupportBean#keepall where IntPrimitive in (context.mie.Ints);\n" +
                      "@Name('s2') context MyContext select * from SupportBean(IntPrimitive in (context.mie.Ints));\n";
            env.CompileDeploy(epl).AddListener("s1").AddListener("s2");

            env.SendEventBean(new SupportInKeywordBean(new[] {1, 2}));

            env.SendEventBean(new SupportBean("E1", 1));
            Assert.IsTrue(env.Listener("s1").IsInvokedAndReset() && env.Listener("s2").IsInvokedAndReset());

            env.SendEventBean(new SupportBean("E2", 2));
            Assert.IsTrue(env.Listener("s1").IsInvokedAndReset() && env.Listener("s2").IsInvokedAndReset());

            env.SendEventBean(new SupportBean("E3", 3));
            Assert.IsFalse(env.Listener("s1").IsInvokedAndReset() || env.Listener("s2").IsInvokedAndReset());

            SupportFilterHelper.AssertFilterMulti(
                env.Statement("s2"),
                "SupportBean",
                new[] {
                    new[] {
                        new FilterItem("IntPrimitive", FilterOperator.IN_LIST_OF_VALUES)
                    }
                });

            env.UndeployAll();
        }

        private static void TryOptimizableEquals(
            RegressionEnvironment env,
            RegressionPath path,
            string epl,
            int numStatements,
            AtomicLong milestone)
        {
            // test function returns lookup value and "Equals"
            for (var i = 0; i < numStatements; i++) {
                var text = "@Name('s" + i + "') " + epl.Replace("!NUM!", Convert.ToString(i));
                env.CompileDeploy(text, path).AddListener("s" + i);
            }

            env.Milestone(milestone.GetAndIncrement());

            var startTime = PerformanceObserver.MilliTime;
            SupportStaticMethodLib.ResetCountInvoked();
            var loops = 1000;
            for (var i = 0; i < loops; i++) {
                var modulus = i % numStatements;
                env.SendEventBean(new SupportBean("E_" + modulus, 0));
                var listener = env.Listener("s" + modulus);
                Assert.That(listener.GetAndClearIsInvoked(), Is.True, $"Statement {modulus} failed for loop {i}");
            }

            var delta = PerformanceObserver.MilliTime - startTime;
            Assert.AreEqual(loops, SupportStaticMethodLib.CountInvoked);

            log.Info("Equals delta=" + delta);
            Assert.That(delta, Is.LessThan(1000), "Delta is " + delta);
            env.UndeployAll();
        }

        private static void TryOptimizableBoolean(
            RegressionEnvironment env,
            RegressionPath path,
            string epl,
            AtomicLong milestone)
        {
            // test function returns lookup value and "Equals"
            var count = 10;
            for (var i = 0; i < count; i++) {
                var compiled = env.Compile("@Name('s" + i + "')" + epl, path);
                var admin = env.Runtime.DeploymentService;
                try {
                    admin.Deploy(compiled);
                }
                catch (EPDeployException) {
                    //ex.PrintStackTrace();
                    Assert.Fail();
                }
            }

            env.MilestoneInc(milestone);

            var listener = new SupportUpdateListener();
            for (var i = 0; i < 10; i++) {
                env.Statement("s" + i).AddListener(listener);
            }

            var startTime = PerformanceObserver.MilliTime;
            SupportStaticMethodLib.ResetCountInvoked();
            var loops = 10000;
            for (var i = 0; i < loops; i++) {
                var key = "E_" + i % 100;
                env.SendEventBean(new SupportBean(key, 0));
                if (key.Equals("E_1")) {
                    Assert.AreEqual(count, listener.NewDataList.Count);
                    listener.Reset();
                }
                else {
                    Assert.IsFalse(listener.IsInvoked);
                }
            }

            var delta = PerformanceObserver.MilliTime - startTime;
            Assert.AreEqual(loops, SupportStaticMethodLib.CountInvoked);

            log.Info("Boolean delta=" + delta);
            Assert.That(delta, Is.LessThan(1000), "Delta is " + delta);
            env.UndeployAll();
        }

        private static void TryOptimizableTypeOf(
            RegressionEnvironment env,
            AtomicLong milestone)
        {
            var epl = "@Name('s0') select * from SupportOverrideBase(typeof(e) = 'SupportOverrideBase') as e";
            env.CompileDeployAddListenerMile(epl, "s0", milestone.GetAndIncrement());

            env.SendEventBean(new SupportOverrideBase(""));
            Assert.IsTrue(env.Listener("s0").GetAndClearIsInvoked());

            env.SendEventBean(new SupportOverrideOne("a", "b"));
            Assert.IsFalse(env.Listener("s0").GetAndClearIsInvoked());

            env.UndeployAll();
        }

        private static void TryOptimizableVariableAndSeparateThread(
            RegressionEnvironment env,
            AtomicLong milestone)
        {
            env.Runtime.VariableService.SetVariableValue(null, "myCheckServiceProvider", new MyCheckServiceProvider());

            env.CompileDeploy("@Name('s0') select * from SupportBean(myCheckServiceProvider.Check())")
                .AddListener("s0");
            var latch = new CountDownLatch(1);

            var executorService = Executors.NewSingleThreadExecutor();
            executorService.Submit(
                () => {
                    env.SendEventBean(new SupportBean());
                    Assert.IsTrue(env.Listener("s0").IsInvokedAndReset());
                    latch.CountDown();
                });

            try {
                Assert.IsTrue(latch.Await(10, TimeUnit.SECONDS));
            }
            catch (ThreadInterruptedException) {
                Assert.Fail();
            }

            env.UndeployAll();
        }

        private static void TryOrRewriteTwoOr(
            RegressionEnvironment env,
            AtomicLong milestone)
        {
            // test 'or' rewrite
            string[] filtersAB = {
                "select * from SupportBean(TheString = 'a' or IntPrimitive = 1)",
                "select * from SupportBean(TheString = 'a' or 1 = IntPrimitive)",
                "select * from SupportBean('a' = TheString or 1 = IntPrimitive)",
                "select * from SupportBean('a' = TheString or IntPrimitive = 1)"
            };

            foreach (var filter in filtersAB) {
                env.CompileDeployAddListenerMile($"@Name('s0'){filter}", "s0", milestone.GetAndIncrement());

                SupportFilterHelper.AssertFilterMulti(
                    env.Statement("s0"),
                    "SupportBean",
                    new[] {
                        new[] {new FilterItem("IntPrimitive", FilterOperator.EQUAL)},
                        new[] {new FilterItem("TheString", FilterOperator.EQUAL)}
                    });

                env.SendEventBean(new SupportBean("a", 0));
                env.Listener("s0").AssertOneGetNewAndReset();
                env.SendEventBean(new SupportBean("b", 1));
                env.Listener("s0").AssertOneGetNewAndReset();
                env.SendEventBean(new SupportBean("c", 0));
                Assert.IsFalse(env.Listener("s0").GetAndClearIsInvoked());

                env.UndeployAll();
            }
        }

        private static void TryOrRewriteOrRewriteWithAnd(
            RegressionEnvironment env,
            AtomicLong milestone)
        {
            string[] filtersAB = {
                "(TheString = 'a' and IntPrimitive = 1) or (TheString = 'b' and IntPrimitive = 2)",
                "(IntPrimitive = 1 and TheString = 'a') or (IntPrimitive = 2 and TheString = 'b')",
                "(TheString = 'b' and IntPrimitive = 2) or (TheString = 'a' and IntPrimitive = 1)"
            };
            foreach (var filter in filtersAB) {
                var epl = $"@Name('s0') select * from SupportBean({filter})";
                env.CompileDeployAddListenerMile(epl, "s0", milestone.GetAndIncrement());
                SupportFilterHelper.AssertFilterMulti(
                    env.Statement("s0"),
                    "SupportBean",
                    new[] {
                        new[] {
                            new FilterItem("TheString", FilterOperator.EQUAL),
                            new FilterItem("IntPrimitive", FilterOperator.EQUAL)
                        },
                        new[] {
                            new FilterItem("TheString", FilterOperator.EQUAL),
                            new FilterItem("IntPrimitive", FilterOperator.EQUAL)
                        }
                    });

                SendAssertEvents(
                    env,
                    new[] {
                        MakeEvent("a", 1),
                        MakeEvent("b", 2)
                    },
                    new[] {
                        MakeEvent("x", 0),
                        MakeEvent("a", 0),
                        MakeEvent("a", 2),
                        MakeEvent("b", 1)
                    }
                );
                env.UndeployAll();
            }
        }

        private static void TryOptimizableMethodInvocationContext(
            RegressionEnvironment env,
            AtomicLong milestone)
        {
            methodInvocationContextFilterOptimized = null;
            env.CompileDeployAddListenerMile(
                "@Name('s0') select * from SupportBean e where myCustomOkFunction(e) = \"OK\"",
                "s0",
                milestone.GetAndIncrement());
            env.SendEventBean(new SupportBean());
            Assert.AreEqual("default", methodInvocationContextFilterOptimized.RuntimeURI);
            Assert.AreEqual("myCustomOkFunction", methodInvocationContextFilterOptimized.FunctionName);
            Assert.IsNull(methodInvocationContextFilterOptimized.StatementUserObject);
            Assert.AreEqual("s0", methodInvocationContextFilterOptimized.StatementName);
            Assert.AreEqual(-1, methodInvocationContextFilterOptimized.ContextPartitionId);
            methodInvocationContextFilterOptimized = null;
            env.UndeployAll();
        }

        private static void AssertInKeywordReceivedPattern(
            RegressionEnvironment env,
            object @event,
            int intPrimitive,
            bool expected)
        {
            env.SendEventBean(@event);
            env.SendEventBean(new SupportBean(null, intPrimitive));
            Assert.AreEqual(expected, env.Listener("s0").IsInvokedAndReset());
        }

        private static void AssertFilterSingle(
            RegressionEnvironment env,
            RegressionPath path,
            string epl,
            string expression,
            FilterOperator op,
            AtomicLong milestone)
        {
            env.CompileDeploy("@Name('s0')" + epl, path).AddListener("s0").MilestoneInc(milestone);
            var statementSPI = (EPStatementSPI) env.Statement("s0");
            var param = SupportFilterHelper.GetFilterSingle(statementSPI);
            Assert.AreEqual(op, param.Op, "failed for '" + epl + "'");
            Assert.AreEqual(expression, param.Name);
            env.UndeployModuleContaining("s0");
        }

        private static SupportBean MakeEvent(
            string theString,
            int intPrimitive)
        {
            return MakeEvent(theString, intPrimitive, 0L);
        }

        private static SupportBean MakeEvent(
            string theString,
            int intPrimitive,
            long longPrimitive)
        {
            return MakeEvent(theString, intPrimitive, longPrimitive, 0d);
        }

        private static SupportBean_IntAlphabetic IntEvent(int a)
        {
            return new SupportBean_IntAlphabetic(a);
        }

        private static SupportBean_IntAlphabetic IntEvent(
            int a,
            int b)
        {
            return new SupportBean_IntAlphabetic(a, b);
        }

        private static SupportBean_IntAlphabetic IntEvent(
            int a,
            int b,
            int c,
            int d)
        {
            return new SupportBean_IntAlphabetic(a, b, c, d);
        }

        private static SupportBean_StringAlphabetic StringEvent(
            string a,
            string b)
        {
            return new SupportBean_StringAlphabetic(a, b);
        }

        private static SupportBean_StringAlphabetic StringEvent(
            string a,
            string b,
            string c)
        {
            return new SupportBean_StringAlphabetic(a, b, c);
        }

        private static SupportBean_IntAlphabetic IntEvent(
            int a,
            int b,
            int c)
        {
            return new SupportBean_IntAlphabetic(a, b, c);
        }

        private static SupportBean_IntAlphabetic IntEvent(
            int a,
            int b,
            int c,
            int d,
            int e)
        {
            return new SupportBean_IntAlphabetic(a, b, c, d, e);
        }

        private static SupportBean MakeEvent(
            string theString,
            int intPrimitive,
            long longPrimitive,
            double doublePrimitive)
        {
            var @event = new SupportBean(theString, intPrimitive);
            @event.LongPrimitive = longPrimitive;
            @event.DoublePrimitive = doublePrimitive;
            return @event;
        }

        private static SupportBean MakeEvent(
            string theString,
            int intPrimitive,
            long longPrimitive,
            double doublePrimitive,
            bool boolPrimitive,
            int intBoxed,
            long longBoxed,
            double doubleBoxed)
        {
            var @event = new SupportBean(theString, intPrimitive);
            @event.LongPrimitive = longPrimitive;
            @event.DoublePrimitive = doublePrimitive;
            @event.BoolPrimitive = boolPrimitive;
            @event.LongBoxed = longBoxed;
            @event.DoubleBoxed = doubleBoxed;
            @event.IntBoxed = intBoxed;
            return @event;
        }

        private static void CompileDeployWSubstitution(
            RegressionEnvironment env,
            string epl,
            IDictionary<string, object> @params)
        {
            var compiled = env.Compile("@Name('s0') " + epl);
            StatementSubstitutionParameterOption resolver = ctx => {
                foreach (var entry in @params) {
                    ctx.SetObject(entry.Key, entry.Value);
                }
            };
            env.Deploy(compiled, new DeploymentOptions().WithStatementSubstitutionParameter(resolver));
            env.AddListener("s0");
        }

        public static string MyCustomOkFunction(
            object e,
            EPLMethodInvocationContext ctx)
        {
            methodInvocationContextFilterOptimized = ctx;
            return "OK";
        }

        public static bool MyCustomDecimalEquals(
            decimal? first,
            decimal? second)
        {
            if (first == null && second == null) {
                return true;
            } else if (first == null || second == null) {
                return false;
            }
            else {
                return first.Value == second.Value;
            }
        }

        public class ExprFilterOrContext : RegressionExecution
        {
            public void Run(RegressionEnvironment env)
            {
                var epl =
                    "@Name('ctx') create context MyContext initiated by SupportBean terminated after 24 hours;\n" +
                    "@Name('select') context MyContext select * from SupportBean(TheString='A' or IntPrimitive=1)";
                env.CompileDeployAddListenerMileZero(epl, "select");

                env.SendEventBean(new SupportBean("A", 1), typeof(SupportBean).Name);
                env.Listener("select").AssertOneGetNewAndReset();

                env.UndeployAll();
            }
        }

        internal class ExprFilterInAndNotInKeywordMultivalue : RegressionExecution
        {
            public void Run(RegressionEnvironment env)
            {
                var milestone = new AtomicLong();

                TryInKeyword(env, "Ints", new SupportInKeywordBean(new[] {1, 2}), milestone);
                TryInKeyword(
                    env,
                    "MapOfIntKey",
                    new SupportInKeywordBean(CollectionUtil.TwoEntryMap(1, "x", 2, "y")),
                    milestone);
                TryInKeyword(env, "CollOfInt", new SupportInKeywordBean(Arrays.AsList(1, 2)), milestone);

                TryNotInKeyword(env, "Ints", new SupportInKeywordBean(new[] {1, 2}), milestone);
                TryNotInKeyword(
                    env,
                    "MapOfIntKey",
                    new SupportInKeywordBean(CollectionUtil.TwoEntryMap(1, "x", 2, "y")),
                    milestone);
                TryNotInKeyword(env, "CollOfInt", new SupportInKeywordBean(Arrays.AsList(1, 2)), milestone);

                TryInArrayContextProvided(env, milestone);

                TryInvalidCompile(
                    env,
                    "select * from pattern[every a=SupportInKeywordBean -> SupportBean(IntPrimitive in (a.Longs))]",
                    "Implicit conversion from datatype 'System.Int64' to 'System.Nullable<System.Int32>' for property 'IntPrimitive' is not allowed (strict filter type coercion)");
            }
        }

        internal class ExprFilterOptimizablePerf : RegressionExecution
        {
            public void Run(RegressionEnvironment env)
            {
                var milestone = new AtomicLong();
                var path = new RegressionPath();

                // func(...) = value
                TryOptimizableEquals(
                    env,
                    path,
                    "select * from SupportBean(libSplit(TheString) = !NUM!)",
                    10,
                    milestone);

                // func(...) implied true
                TryOptimizableBoolean(env, path, "select * from SupportBean(libE1True(TheString))", milestone);

                // with context
                TryOptimizableMethodInvocationContext(env, milestone);

                // typeof(e)
                TryOptimizableTypeOf(env, milestone);

                // declared expression (...) = value
                env.CompileDeploy(
                        "@Name('create-expr') create expression thesplit {TheString -> libSplit(TheString)}",
                        path)
                    .AddListener("create-expr");
                TryOptimizableEquals(env, path, "select * from SupportBean(thesplit(*) = !NUM!)", 10, milestone);

                // declared expression (...) implied true
                env.CompileDeploy(
                        "@Name('create-expr') create expression theE1Test {TheString -> libE1True(TheString)}",
                        path)
                    .AddListener("create-expr");
                TryOptimizableBoolean(env, path, "select * from SupportBean(theE1Test(*))", milestone);

                // with variable and separate thread
                TryOptimizableVariableAndSeparateThread(env, milestone);
            }
        }

        internal class ExprFilterOptimizableInspectFilter : RegressionExecution
        {
            public void Run(RegressionEnvironment env)
            {
                string epl;
                var milestone = new AtomicLong();
                var path = new RegressionPath();

                epl = "select * from SupportBean(funcOne(TheString) = 0)";
                AssertFilterSingle(
                    env,
                    path,
                    epl,
                    PROPERTY_NAME_BOOLEAN_EXPRESSION,
                    FilterOperator.BOOLEAN_EXPRESSION,
                    milestone);

                epl = "select * from SupportBean(funcOneWDefault(TheString) = 0)";
                AssertFilterSingle(env, path, epl, "funcOneWDefault(TheString)", FilterOperator.EQUAL, milestone);

                epl = "select * from SupportBean(funcTwo(TheString) = 0)";
                AssertFilterSingle(env, path, epl, "funcTwo(TheString)", FilterOperator.EQUAL, milestone);

                epl = "select * from SupportBean(libE1True(TheString))";
                AssertFilterSingle(env, path, epl, "libE1True(TheString)", FilterOperator.EQUAL, milestone);

                epl = "select * from SupportBean(funcTwo( TheString ) > 10)";
                AssertFilterSingle(env, path, epl, "funcTwo(TheString)", FilterOperator.GREATER, milestone);

                epl = "select * from SupportBean(libE1True(TheString))";
                AssertFilterSingle(env, path, epl, "libE1True(TheString)", FilterOperator.EQUAL, milestone);

                epl = "select * from SupportBean(typeof(e) = 'SupportBean') as e";
                AssertFilterSingle(env, path, epl, "typeof(e)", FilterOperator.EQUAL, milestone);

                env.CompileDeploy(
                        "@Name('create-expr') create expression thesplit {TheString -> funcOne(TheString)}",
                        path)
                    .AddListener("create-expr");
                epl = "select * from SupportBean(thesplit(*) = 0)";
                AssertFilterSingle(env, path, epl, "thesplit(*)", FilterOperator.EQUAL, milestone);

                epl = "select * from SupportBean(thesplit(*) > 10)";
                AssertFilterSingle(env, path, epl, "thesplit(*)", FilterOperator.GREATER, milestone);

                epl = "expression housenumber alias for {10} select * from SupportBean(IntPrimitive = housenumber)";
                AssertFilterSingle(env, path, epl, "IntPrimitive", FilterOperator.EQUAL, milestone);

                epl =
                    "expression housenumber alias for {IntPrimitive*10} select * from SupportBean(IntPrimitive = housenumber)";
                AssertFilterSingle(env, path, epl, ".boolean_expression", FilterOperator.BOOLEAN_EXPRESSION, milestone);

                env.UndeployAll();
            }
        }

        internal class ExprFilterOrRewrite : RegressionExecution
        {
            public void Run(RegressionEnvironment env)
            {
                var milestone = new AtomicLong();
                
                TryOrRewriteTwoOr(env, milestone);

                TryOrRewriteOrRewriteThreeOr(env, milestone);

                TryOrRewriteOrRewriteWithAnd(env, milestone);

                TryOrRewriteOrRewriteThreeWithOverlap(env, milestone);

                TryOrRewriteOrRewriteFourOr(env, milestone);

                TryOrRewriteOrRewriteEightOr(env, milestone);

                TryOrRewriteAndRewriteNotEquals(env, milestone);

                TryOrRewriteAndRewriteInnerOr(env, milestone);

                TryOrRewriteOrRewriteAndOrMulti(env, milestone);

                TryOrRewriteBooleanExprSimple(env, milestone);

                TryOrRewriteBooleanExprAnd(env, milestone);

                TryOrRewriteSubquery(env, milestone);

                TryOrRewriteHint(env, milestone);

                TryOrRewriteContextPartitionedSegmented(env, milestone);

                TryOrRewriteContextPartitionedHash(env, milestone);

                TryOrRewriteContextPartitionedCategory(env, milestone);

                TryOrRewriteContextPartitionedInitiatedSameEvent(env, milestone);

                TryOrRewriteContextPartitionedInitiated(env, milestone);
            }
        }

        internal class ExprFilterPatternUDFFilterOptimizable : RegressionExecution
        {
            public void Run(RegressionEnvironment env)
            {
                var epl =
                    "@Name('s0') select * from pattern[a=SupportBean() -> b=SupportBean(myCustomDecimalEquals(a.DecimalBoxed, b.DecimalBoxed))]";
                env.CompileDeploy(epl).AddListener("s0");

                var beanOne = new SupportBean("E1", 0);
                beanOne.DecimalBoxed = 13.0m;
                env.SendEventBean(beanOne);

                var beanTwo = new SupportBean("E2", 0);
                beanTwo.DecimalBoxed = 13.0m;
                env.SendEventBean(beanTwo);

                Assert.IsTrue(env.Listener("s0").IsInvoked);

                env.UndeployAll();
            }
        }

        internal class ExprFilterOrToInRewrite : RegressionExecution
        {
            public void Run(RegressionEnvironment env)
            {
                var milestone = new AtomicLong();
                // test 'or' rewrite
                string[] filtersAB = {
                    "TheString = 'a' or TheString = 'b'",
                    "TheString = 'a' or 'b' = TheString",
                    "'a' = TheString or 'b' = TheString",
                    "'a' = TheString or TheString = 'b'"
                };

                string epl;

                foreach (var filter in filtersAB) {
                    epl = $"@Name('s0') select * from SupportBean({filter})";
                    env.CompileDeployAddListenerMile(epl, "s0", milestone.GetAndIncrement());
                    AssertFilterSingle(env.Statement("s0"), epl, "TheString", FilterOperator.IN_LIST_OF_VALUES);

                    env.SendEventBean(new SupportBean("a", 0));
                    Assert.IsTrue(env.Listener("s0").GetAndClearIsInvoked());
                    env.SendEventBean(new SupportBean("b", 0));
                    Assert.IsTrue(env.Listener("s0").GetAndClearIsInvoked());
                    env.SendEventBean(new SupportBean("c", 0));
                    Assert.IsFalse(env.Listener("s0").GetAndClearIsInvoked());

                    env.UndeployAll();
                }

                epl = "@Name('s0') select * from SupportBean(IntPrimitive = 1 and (TheString='a' or TheString='b'))";
                env.CompileDeployAddListenerMile(epl, "s0", milestone.GetAndIncrement());
                SupportFilterHelper.AssertFilterTwo(
                    env.Statement("s0"),
                    epl,
                    "IntPrimitive",
                    FilterOperator.EQUAL,
                    "TheString",
                    FilterOperator.IN_LIST_OF_VALUES);
                env.UndeployAll();
            }
        }

        internal class ExprFilterOrPerformance : RegressionExecution
        {
            public void Run(RegressionEnvironment env)
            {
                var listener = new SupportUpdateListener();
                for (var i = 0; i < 100; i++) {
                    var epl = "@Name('s" +
                              i +
                              "') select * from SupportBean(TheString = '" +
                              i +
                              "' or IntPrimitive=" +
                              i +
                              ")";
                    var compiled = env.Compile(epl);
                    env.Deploy(compiled).Statement("s" + i).AddListener(listener);
                }

                var start = PerformanceObserver.NanoTime;
                // System.out.println("Starting " + DateTime.print(new Date()));
                for (var i = 0; i < 10000; i++) {
                    env.SendEventBean(new SupportBean("100", 1));
                    Assert.IsTrue(listener.IsInvoked);
                    listener.Reset();
                }

                // System.out.println("Ending " + DateTime.print(new Date()));
                var delta = (PerformanceObserver.NanoTime - start) / 1000d / 1000d;
                // System.out.println("Delta=" + (delta + " msec"));
                Assert.That(delta, Is.LessThan(500));

                env.UndeployAll();
            }
        }

        internal class ExprFilterDeployTimeConstant : RegressionExecution
        {
            public void Run(RegressionEnvironment env)
            {
                RunAssertionEqualsWSubs(env, "select * from SupportBean(TheString=?:p0:string)");
                RunAssertionEqualsWSubs(env, "select * from SupportBean(?:p0:string=TheString)");
                RunAssertionEqualsWVariable(env, "select * from SupportBean(TheString=var_optimizable_equals)");
                RunAssertionEqualsWVariable(env, "select * from SupportBean(var_optimizable_equals=TheString)");
                RunAssertionEqualsWSubsWCoercion(env, "select * from SupportBean(LongPrimitive=?:p0:int)");
                RunAssertionEqualsWSubsWCoercion(env, "select * from SupportBean(?:p0:int=LongPrimitive)");

                TryInvalidCompile(
                    env,
                    "select * from SupportBean(IntPrimitive=?:p0:long)",
                    "Implicit conversion from datatype '" +
                    typeof(long?).CleanName() +
                    "' to '" +
                    typeof(int?).CleanName() +
                    "' for property 'IntPrimitive' is not allowed");

                RunAssertionRelOpWSubs(env, "select * from SupportBean(IntPrimitive>?:p0:int)");
                RunAssertionRelOpWSubs(env, "select * from SupportBean(?:p0:int<IntPrimitive)");
                RunAssertionRelOpWVariable(env, "select * from SupportBean(IntPrimitive>var_optimizable_relop)");
                RunAssertionRelOpWVariable(env, "select * from SupportBean(var_optimizable_relop<IntPrimitive)");

                RunAssertionInWSubs(env, "select * from SupportBean(IntPrimitive in (?:p0:int, ?:p1:int))");
                RunAssertionInWVariable(
                    env,
                    "select * from SupportBean(IntPrimitive in (var_optimizable_start, var_optimizable_end))");

                RunAssertionInWSubsWArray(env, "select * from SupportBean(IntPrimitive in (?:p0:int[primitive]))");
                RunAssertionInWVariableWArray(
                    env,
                    "select * from SupportBean(IntPrimitive in (var_optimizable_array))");

                RunAssertionBetweenWSubsWNumeric(
                    env,
                    "select * from SupportBean(IntPrimitive between ?:p0:int and ?:p1:int)");
                RunAssertionBetweenWVariableWNumeric(
                    env,
                    "select * from SupportBean(IntPrimitive between var_optimizable_start and var_optimizable_end)");

                RunAssertionBetweenWSubsWString(
                    env,
                    "select * from SupportBean(TheString between ?:p0:string and ?:p1:string)");
                RunAssertionBetweenWVariableWString(
                    env,
                    "select * from SupportBean(TheString between var_optimizable_start_string and var_optimizable_end_string)");
            }
        }

        [Serializable]
        public class MyCheckServiceProvider
        {
            public bool Check()
            {
                return true;
            }
        }
    }
} // end of namespace