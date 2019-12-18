///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2015 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using com.espertech.esper.common.client.scopetest;
using com.espertech.esper.compat;
using com.espertech.esper.regressionlib.framework;
using com.espertech.esper.regressionlib.support.rowrecog;

using NUnit.Framework;

namespace com.espertech.esper.regressionlib.suite.rowrecog
{
    public class RowRecogIntervalOrTerminated : RegressionExecution
    {
        public void Run(RegressionEnvironment env)
        {
            var milestone = new AtomicLong();
            RunAssertionDocSample(env, milestone);

            RunAssertion_A_Bstar(env, milestone, false);

            RunAssertion_A_Bstar(env, milestone, true);

            RunAssertion_Astar(env, milestone);

            RunAssertion_A_Bplus(env, milestone);

            RunAssertion_A_Bstar_or_Cstar(env, milestone);

            RunAssertion_A_B_Cstar(env, milestone);

            RunAssertion_A_B(env, milestone);

            RunAssertion_A_Bstar_or_C(env, milestone);

            RunAssertion_A_parenthesisBstar(env, milestone);
        }

        private void RunAssertion_A_Bstar_or_C(
            RegressionEnvironment env,
            AtomicLong milestone)
        {
            SendTimer(env, 0);

            var fields = new [] { "a","b0","b1","b2","c" };
            var text = "@Name('s0') select * from SupportRecogBean#keepall " +
                       "match_recognize (" +
                       " measures A.TheString as a, B[0].TheString as b0, B[1].TheString as b1, B[2].TheString as b2, C.TheString as c " +
                       " pattern (A (B* | C))" +
                       " interval 10 seconds or terminated" +
                       " define" +
                       " A as A.TheString like 'A%'," +
                       " B as B.TheString like 'B%'," +
                       " C as C.TheString like 'C%'" +
                       ")";

            env.CompileDeploy(text).AddListener("s0");

            env.SendEventBean(new SupportRecogBean("A1"));
            env.SendEventBean(new SupportRecogBean("C1"));
            EPAssertionUtil.AssertProps(
                env.Listener("s0").AssertOneGetNewAndReset(),
                fields,
                new object[] {"A1", null, null, null, "C1"});

            env.SendEventBean(new SupportRecogBean("A2"));
            Assert.IsFalse(env.Listener("s0").IsInvoked);
            env.SendEventBean(new SupportRecogBean("B1"));
            EPAssertionUtil.AssertProps(
                env.Listener("s0").AssertOneGetNewAndReset(),
                fields,
                new object[] {"A2", null, null, null, null});

            env.MilestoneInc(milestone);

            env.SendEventBean(new SupportRecogBean("B2"));
            Assert.IsFalse(env.Listener("s0").IsInvoked);
            env.SendEventBean(new SupportRecogBean("X1"));
            EPAssertionUtil.AssertProps(
                env.Listener("s0").AssertOneGetNewAndReset(),
                fields,
                new object[] {"A2", "B1", "B2", null, null});

            env.SendEventBean(new SupportRecogBean("A3"));
            SendTimer(env, 10000);
            EPAssertionUtil.AssertProps(
                env.Listener("s0").AssertOneGetNewAndReset(),
                fields,
                new object[] {"A3", null, null, null, null});

            SendTimer(env, int.MaxValue);
            Assert.IsFalse(env.Listener("s0").IsInvoked);

            // destroy
            env.UndeployAll();
        }

        private void RunAssertion_A_B(
            RegressionEnvironment env,
            AtomicLong milestone)
        {
            SendTimer(env, 0);

            // the interval is not effective
            var fields = new [] { "a","b" };
            var text = "@Name('s0') select * from SupportRecogBean#keepall " +
                       "match_recognize (" +
                       " measures A.TheString as a, B.TheString as b" +
                       " pattern (A B)" +
                       " interval 10 seconds or terminated" +
                       " define" +
                       " A as A.TheString like 'A%'," +
                       " B as B.TheString like 'B%'" +
                       ")";

            env.CompileDeploy(text).AddListener("s0");

            env.SendEventBean(new SupportRecogBean("A1"));
            env.SendEventBean(new SupportRecogBean("B1"));
            EPAssertionUtil.AssertProps(
                env.Listener("s0").AssertOneGetNewAndReset(),
                fields,
                new object[] {"A1", "B1"});

            env.MilestoneInc(milestone);

            env.SendEventBean(new SupportRecogBean("A2"));
            env.SendEventBean(new SupportRecogBean("A3"));
            env.SendEventBean(new SupportRecogBean("B2"));
            EPAssertionUtil.AssertProps(
                env.Listener("s0").AssertOneGetNewAndReset(),
                fields,
                new object[] {"A3", "B2"});

            // destroy
            env.UndeployAll();
        }

        private void RunAssertionDocSample(
            RegressionEnvironment env,
            AtomicLong milestone)
        {
            SendTimer(env, 0);

            var fields = new [] { "a_Id","count_b","first_b","last_b" };
            var text = "@Name('s0') select * from TemperatureSensorEvent\n" +
                       "match_recognize (\n" +
                       "  partition by Device\n" +
                       "  measures A.Id as a_Id, count(B.Id) as count_b, first(B.Id) as first_b, last(B.Id) as last_b\n" +
                       "  pattern (A B*)\n" +
                       "  interval 5 seconds or terminated\n" +
                       "  define\n" +
                       "    A as A.Temp > 100,\n" +
                       "    B as B.Temp > 100)";

            env.CompileDeploy(text).AddListener("s0");

            SendTemperatureEvent(env, "E1", 1, 98);

            env.MilestoneInc(milestone);

            SendTemperatureEvent(env, "E2", 1, 101);
            SendTemperatureEvent(env, "E3", 1, 102);
            SendTemperatureEvent(env, "E4", 1, 101); // falls below
            Assert.IsFalse(env.Listener("s0").IsInvoked);

            SendTemperatureEvent(env, "E5", 1, 100); // falls below
            EPAssertionUtil.AssertProps(
                env.Listener("s0").AssertOneGetNewAndReset(),
                fields,
                new object[] {"E2", 2L, "E3", "E4"});

            env.MilestoneInc(milestone);

            SendTimer(env, int.MaxValue);
            Assert.IsFalse(env.Listener("s0").IsInvoked);

            // destroy
            env.UndeployAll();
        }

        private void RunAssertion_A_B_Cstar(
            RegressionEnvironment env,
            AtomicLong milestone)
        {
            SendTimer(env, 0);

            var fields = new [] { "a","b","c0","c1","c2" };
            var text = "@Name('s0') select * from SupportRecogBean#keepall " +
                       "match_recognize (" +
                       " measures A.TheString as a, B.TheString as b, " +
                       "C[0].TheString as c0, C[1].TheString as c1, C[2].TheString as c2 " +
                       " pattern (A B C*)" +
                       " interval 10 seconds or terminated" +
                       " define" +
                       " A as A.TheString like 'A%'," +
                       " B as B.TheString like 'B%'," +
                       " C as C.TheString like 'C%'" +
                       ")";
            env.CompileDeploy(text).AddListener("s0");

            env.SendEventBean(new SupportRecogBean("A1"));
            env.SendEventBean(new SupportRecogBean("B1"));
            env.SendEventBean(new SupportRecogBean("C1"));
            env.SendEventBean(new SupportRecogBean("C2"));
            Assert.IsFalse(env.Listener("s0").IsInvoked);

            env.MilestoneInc(milestone);

            env.SendEventBean(new SupportRecogBean("B2"));
            EPAssertionUtil.AssertProps(
                env.Listener("s0").AssertOneGetNewAndReset(),
                fields,
                new object[] {"A1", "B1", "C1", "C2", null});

            env.SendEventBean(new SupportRecogBean("A2"));
            env.SendEventBean(new SupportRecogBean("X1"));
            env.SendEventBean(new SupportRecogBean("B3"));
            env.SendEventBean(new SupportRecogBean("X2"));
            Assert.IsFalse(env.Listener("s0").IsInvoked);

            env.SendEventBean(new SupportRecogBean("A3"));
            env.SendEventBean(new SupportRecogBean("B4"));
            env.SendEventBean(new SupportRecogBean("X3"));
            EPAssertionUtil.AssertProps(
                env.Listener("s0").AssertOneGetNewAndReset(),
                fields,
                new object[] {"A3", "B4", null, null, null});

            env.MilestoneInc(milestone);

            SendTimer(env, 20000);
            env.SendEventBean(new SupportRecogBean("A4"));
            env.SendEventBean(new SupportRecogBean("B5"));
            env.SendEventBean(new SupportRecogBean("C3"));
            Assert.IsFalse(env.Listener("s0").IsInvoked);

            env.MilestoneInc(milestone);

            SendTimer(env, 30000);
            EPAssertionUtil.AssertProps(
                env.Listener("s0").AssertOneGetNewAndReset(),
                fields,
                new object[] {"A4", "B5", "C3", null, null});

            SendTimer(env, int.MaxValue);
            Assert.IsFalse(env.Listener("s0").IsInvoked);

            // destroy
            env.UndeployAll();
        }

        private void RunAssertion_A_Bstar_or_Cstar(
            RegressionEnvironment env,
            AtomicLong milestone)
        {
            SendTimer(env, 0);

            var fields = new [] { "a","b0","b1","c0","c1" };
            var text = "@Name('s0') select * from SupportRecogBean#keepall " +
                       "match_recognize (" +
                       " measures A.TheString as a, " +
                       "B[0].TheString as b0, B[1].TheString as b1, " +
                       "C[0].TheString as c0, C[1].TheString as c1 " +
                       " pattern (A (B* | C*))" +
                       " interval 10 seconds or terminated" +
                       " define" +
                       " A as A.TheString like 'A%'," +
                       " B as B.TheString like 'B%'," +
                       " C as C.TheString like 'C%'" +
                       ")";

            env.CompileDeploy(text).AddListener("s0");

            env.SendEventBean(new SupportRecogBean("A1"));
            env.SendEventBean(new SupportRecogBean("X1"));
            EPAssertionUtil.AssertProps(
                env.Listener("s0").AssertOneGetNewAndReset(),
                fields,
                new object[] {"A1", null, null, null, null});

            env.MilestoneInc(milestone);

            env.SendEventBean(new SupportRecogBean("A2"));
            env.SendEventBean(new SupportRecogBean("C1"));
            EPAssertionUtil.AssertProps(
                env.Listener("s0").AssertOneGetNewAndReset(),
                fields,
                new object[] {"A2", null, null, null, null});

            env.SendEventBean(new SupportRecogBean("B1"));
            EPAssertionUtil.AssertPropsPerRow(
                env.Listener("s0").GetAndResetLastNewData(),
                fields,
                new[] {new object[] {"A2", null, null, "C1", null}});

            env.MilestoneInc(milestone);

            env.SendEventBean(new SupportRecogBean("C2"));
            Assert.IsFalse(env.Listener("s0").IsInvoked);

            // destroy
            env.UndeployAll();
        }

        private void RunAssertion_A_Bplus(
            RegressionEnvironment env,
            AtomicLong milestone)
        {
            SendTimer(env, 0);

            var fields = new [] { "a","b0","b1","b2" };
            var text = "@Name('s0') select * from SupportRecogBean#keepall " +
                       "match_recognize (" +
                       " measures A.TheString as a, B[0].TheString as b0, B[1].TheString as b1, B[2].TheString as b2" +
                       " pattern (A B+)" +
                       " interval 10 seconds or terminated" +
                       " define" +
                       " A as A.TheString like 'A%'," +
                       " B as B.TheString like 'B%'" +
                       ")";

            env.CompileDeploy(text).AddListener("s0");

            env.SendEventBean(new SupportRecogBean("A1"));
            env.SendEventBean(new SupportRecogBean("X1"));

            env.MilestoneInc(milestone);

            env.SendEventBean(new SupportRecogBean("A2"));
            env.SendEventBean(new SupportRecogBean("B2"));
            Assert.IsFalse(env.Listener("s0").IsInvoked);
            env.SendEventBean(new SupportRecogBean("X2"));
            EPAssertionUtil.AssertProps(
                env.Listener("s0").AssertOneGetNewAndReset(),
                fields,
                new object[] {"A2", "B2", null, null});

            env.SendEventBean(new SupportRecogBean("A3"));
            env.SendEventBean(new SupportRecogBean("A4"));

            env.MilestoneInc(milestone);

            env.SendEventBean(new SupportRecogBean("B3"));
            env.SendEventBean(new SupportRecogBean("B4"));
            Assert.IsFalse(env.Listener("s0").IsInvoked);
            env.SendEventBean(new SupportRecogBean("X3", -1));
            EPAssertionUtil.AssertProps(
                env.Listener("s0").AssertOneGetNewAndReset(),
                fields,
                new object[] {"A4", "B3", "B4", null});

            // destroy
            env.UndeployAll();
        }

        private void RunAssertion_Astar(
            RegressionEnvironment env,
            AtomicLong milestone)
        {
            SendTimer(env, 0);

            var fields = new [] { "a0","a1","a2","a3","a4" };
            var text = "@Name('s0') select * from SupportRecogBean#keepall " +
                       "match_recognize (" +
                       " measures A[0].TheString as a0, A[1].TheString as a1, A[2].TheString as a2, A[3].TheString as a3, A[4].TheString as a4" +
                       " pattern (A*)" +
                       " interval 10 seconds or terminated" +
                       " define" +
                       " A as TheString like 'A%'" +
                       ")";

            env.CompileDeploy(text).AddListener("s0");

            env.SendEventBean(new SupportRecogBean("A1"));
            env.SendEventBean(new SupportRecogBean("A2"));
            Assert.IsFalse(env.Listener("s0").IsInvoked);
            env.SendEventBean(new SupportRecogBean("B1"));
            EPAssertionUtil.AssertProps(
                env.Listener("s0").AssertOneGetNewAndReset(),
                fields,
                new object[] {"A1", "A2", null, null, null});

            env.MilestoneInc(milestone);

            SendTimer(env, 2000);
            env.SendEventBean(new SupportRecogBean("A3"));
            env.SendEventBean(new SupportRecogBean("A4"));
            env.SendEventBean(new SupportRecogBean("A5"));
            Assert.IsFalse(env.Listener("s0").IsInvoked);
            SendTimer(env, 12000);
            EPAssertionUtil.AssertProps(
                env.Listener("s0").AssertOneGetNewAndReset(),
                fields,
                new object[] {"A3", "A4", "A5", null, null});

            env.SendEventBean(new SupportRecogBean("A6"));
            env.SendEventBean(new SupportRecogBean("B2"));
            EPAssertionUtil.AssertProps(
                env.Listener("s0").AssertOneGetNewAndReset(),
                fields,
                new object[] {"A3", "A4", "A5", "A6", null});
            env.SendEventBean(new SupportRecogBean("B3"));
            Assert.IsFalse(env.Listener("s0").IsInvoked);

            // destroy
            env.UndeployAll();
        }

        private void RunAssertion_A_Bstar(
            RegressionEnvironment env,
            AtomicLong milestone,
            bool allMatches)
        {
            SendTimer(env, 0);

            var fields = new [] { "a","b0","b1","b2" };
            var text = "@Name('s0') select * from SupportRecogBean#keepall " +
                       "match_recognize (" +
                       " measures A.TheString as a, B[0].TheString as b0, B[1].TheString as b1, B[2].TheString as b2" +
                       (allMatches ? " all matches" : "") +
                       " pattern (A B*)" +
                       " interval 10 seconds or terminated" +
                       " define" +
                       " A as A.TheString like \"A%\"," +
                       " B as B.TheString like \"B%\"" +
                       ")";

            env.CompileDeploy(text).AddListener("s0");

            // test output by terminated because of misfit event
            env.SendEventBean(new SupportRecogBean("A1"));
            env.SendEventBean(new SupportRecogBean("B1"));
            Assert.IsFalse(env.Listener("s0").IsInvoked);
            env.SendEventBean(new SupportRecogBean("X1"));
            if (!allMatches) {
                EPAssertionUtil.AssertProps(
                    env.Listener("s0").AssertOneGetNewAndReset(),
                    fields,
                    new object[] {"A1", "B1", null, null});
            }
            else {
                EPAssertionUtil.AssertPropsPerRowAnyOrder(
                    env.Listener("s0").GetAndResetLastNewData(),
                    fields,
                    new[] {new object[] {"A1", "B1", null, null}, new object[] {"A1", null, null, null}});
            }

            env.MilestoneInc(milestone);

            SendTimer(env, 20000);
            Assert.IsFalse(env.Listener("s0").IsInvoked);

            // test output by timer expiry
            env.SendEventBean(new SupportRecogBean("A2"));
            env.SendEventBean(new SupportRecogBean("B2"));
            Assert.IsFalse(env.Listener("s0").IsInvoked);
            SendTimer(env, 29999);

            SendTimer(env, 30000);
            if (!allMatches) {
                EPAssertionUtil.AssertProps(
                    env.Listener("s0").AssertOneGetNewAndReset(),
                    fields,
                    new object[] {"A2", "B2", null, null});
            }
            else {
                EPAssertionUtil.AssertPropsPerRowAnyOrder(
                    env.Listener("s0").GetAndResetLastNewData(),
                    fields,
                    new[] {new object[] {"A2", "B2", null, null}, new object[] {"A2", null, null, null}});
            }

            // destroy
            env.UndeployAll();
        }

        private void RunAssertion_A_parenthesisBstar(
            RegressionEnvironment env,
            AtomicLong milestone)
        {
            SendTimer(env, 0);

            var fields = new [] { "a","b0","b1","b2" };
            var text = "@Name('s0') select * from SupportRecogBean#keepall " +
                       "match_recognize (" +
                       " measures A.TheString as a, B[0].TheString as b0, B[1].TheString as b1, B[2].TheString as b2" +
                       " pattern (A (B)*)" +
                       " interval 10 seconds or terminated" +
                       " define" +
                       " A as A.TheString like \"A%\"," +
                       " B as B.TheString like \"B%\"" +
                       ")";

            env.CompileDeploy(text).AddListener("s0");

            // test output by terminated because of misfit event
            env.SendEventBean(new SupportRecogBean("A1"));
            env.SendEventBean(new SupportRecogBean("B1"));
            Assert.IsFalse(env.Listener("s0").IsInvoked);
            env.SendEventBean(new SupportRecogBean("X1"));
            EPAssertionUtil.AssertProps(
                env.Listener("s0").AssertOneGetNewAndReset(),
                fields,
                new object[] {"A1", "B1", null, null});

            env.MilestoneInc(milestone);

            SendTimer(env, 20000);
            Assert.IsFalse(env.Listener("s0").IsInvoked);

            // test output by timer expiry
            env.SendEventBean(new SupportRecogBean("A2"));
            env.SendEventBean(new SupportRecogBean("B2"));
            Assert.IsFalse(env.Listener("s0").IsInvoked);
            SendTimer(env, 29999);

            SendTimer(env, 30000);
            EPAssertionUtil.AssertProps(
                env.Listener("s0").AssertOneGetNewAndReset(),
                fields,
                new object[] {"A2", "B2", null, null});

            // destroy
            env.UndeployAll();
        }

        private void SendTemperatureEvent(
            RegressionEnvironment env,
            string id,
            int device,
            double temp)
        {
            env.SendEventObjectArray(new object[] {id, device, temp}, "TemperatureSensorEvent");
        }

        private void SendTimer(
            RegressionEnvironment env,
            long time)
        {
            env.AdvanceTime(time);
        }
    }
} // end of namespace