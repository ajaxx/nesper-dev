///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2015 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using com.espertech.esper.common.client.configuration;
using com.espertech.esper.common.@internal.support;
using com.espertech.esper.regressionlib.suite.view;
using com.espertech.esper.regressionlib.support.bean;
using com.espertech.esper.regressionrun.Runner;

using NUnit.Framework;

using SupportBean_A = com.espertech.esper.regressionlib.support.bean.SupportBean_A;
using SupportBean_N = com.espertech.esper.regressionlib.support.bean.SupportBean_N;
using SupportBeanComplexProps = com.espertech.esper.regressionlib.support.bean.SupportBeanComplexProps;

namespace com.espertech.esper.regressionrun.suite.view
{
    [TestFixture]
    public class TestSuiteView
    {
        [SetUp]
        public void SetUp()
        {
            session = RegressionRunner.Session();
            Configure(session.Configuration);
        }

        [TearDown]
        public void TearDown()
        {
            session.Destroy();
            session = null;
        }

        private RegressionSession session;

        private static void Configure(Configuration configuration)
        {
            foreach (var clazz in new[] {
                typeof(SupportMarketDataBean),
                typeof(SupportBeanComplexProps),
                typeof(SupportBean),
                typeof(SupportBeanWithEnum),
                typeof(SupportBeanTimestamp),
                typeof(SupportEventIdWithTimestamp),
                typeof(SupportSensorEvent),
                typeof(SupportBean_S0),
                typeof(SupportBean_S1),
                typeof(SupportBean_A),
                typeof(SupportBean_N),
                typeof(SupportContextInitEventWLength)
            }) {
                configuration.Common.AddEventType(clazz.Name, clazz);
            }

            configuration.Common.AddEventType(
                "OAEventStringInt",
                new[] {"P1", "P2"},
                new object[] {typeof(string), typeof(int)});

            configuration.Common.AddVariable("TIME_WIN_ONE", typeof(int), 4);
            configuration.Common.AddVariable("TIME_WIN_TWO", typeof(double), 4000);

            configuration.Compiler.AddPlugInSingleRowFunction(
                "udf",
                typeof(ViewExpressionWindow.LocalUDF),
                "EvaluateExpiryUDF");
        }

        [Test, RunInApplicationDomain]
        public void TestViewDerived()
        {
            RegressionRunner.Run(session, ViewDerived.Executions());
        }

        [Test, RunInApplicationDomain]
        public void TestViewExpressionBatch()
        {
            RegressionRunner.Run(session, ViewExpressionBatch.Executions());
        }

        [Test, RunInApplicationDomain]
        public void TestViewExpressionWindow()
        {
            RegressionRunner.Run(session, ViewExpressionWindow.Executions());
        }

        [Test, RunInApplicationDomain]
        public void TestViewExternallyBatched()
        {
            RegressionRunner.Run(session, ViewExternallyTimedBatched.Executions());
        }

        [Test, RunInApplicationDomain]
        public void TestViewExternallyTimedWin()
        {
            RegressionRunner.Run(session, ViewExternallyTimedWin.Executions());
        }

        [Test, RunInApplicationDomain]
        public void TestViewFirstEvent()
        {
            RegressionRunner.Run(session, ViewFirstEvent.Executions());
        }

        [Test, RunInApplicationDomain]
        public void TestViewFirstLength()
        {
            RegressionRunner.Run(session, ViewFirstLength.Executions());
        }

        [Test, RunInApplicationDomain]
        public void TestViewFirstTime()
        {
            RegressionRunner.Run(session, ViewFirstTime.Executions());
        }

        [Test, RunInApplicationDomain]
        public void TestViewFirstUnique()
        {
            RegressionRunner.Run(session, ViewFirstUnique.Executions());
        }

        [Test, RunInApplicationDomain]
        public void TestViewGroup()
        {
            RegressionRunner.Run(session, ViewGroup.Executions());
        }

        [Test, RunInApplicationDomain]
        public void TestViewIntersect()
        {
            RegressionRunner.Run(session, ViewIntersect.Executions());
        }

        [Test, RunInApplicationDomain]
        public void TestViewInvalid()
        {
            RegressionRunner.Run(session, new ViewInvalid());
        }

        [Test, RunInApplicationDomain]
        public void TestViewKeepAll()
        {
            RegressionRunner.Run(session, ViewKeepAll.Executions());
        }

        [Test, RunInApplicationDomain]
        public void TestViewLastEvent()
        {
            RegressionRunner.Run(session, ViewLastEvent.Executions());
        }

        [Test, RunInApplicationDomain]
        public void TestViewLengthBatch()
        {
            RegressionRunner.Run(session, ViewLengthBatch.Executions());
        }

        [Test, RunInApplicationDomain]
        public void TestViewLengthWin()
        {
            RegressionRunner.Run(session, ViewLengthWin.Executions());
        }

        [Test, RunInApplicationDomain]
        public void TestViewParameterizedByContext()
        {
            RegressionRunner.Run(session, ViewParameterizedByContext.Executions());
        }

        [Test, RunInApplicationDomain]
        public void TestViewRank()
        {
            RegressionRunner.Run(session, ViewRank.Executions());
        }

        [Test, RunInApplicationDomain]
        public void TestViewSort()
        {
            RegressionRunner.Run(session, ViewSort.Executions());
        }

        [Test, RunInApplicationDomain]
        public void TestViewTimeAccum()
        {
            RegressionRunner.Run(session, ViewTimeAccum.Executions());
        }

        [Test, RunInApplicationDomain]
        public void TestViewTimeBatch()
        {
            RegressionRunner.Run(session, ViewTimeBatch.Executions());
        }

        [Test, RunInApplicationDomain]
        public void TestViewTimeLengthBatch()
        {
            RegressionRunner.Run(session, ViewTimeLengthBatch.Executions());
        }

        [Test, RunInApplicationDomain]
        public void TestViewTimeOrderAndTimeToLive()
        {
            RegressionRunner.Run(session, ViewTimeOrderAndTimeToLive.Executions());
        }

        [Test, RunInApplicationDomain]
        public void TestViewTimeWin()
        {
            RegressionRunner.Run(session, ViewTimeWin.Executions());
        }

        [Test, RunInApplicationDomain]
        public void TestViewUnion()
        {
            RegressionRunner.Run(session, ViewUnion.Executions());
        }

        [Test, RunInApplicationDomain]
        public void TestViewUnique()
        {
            RegressionRunner.Run(session, ViewUnique.Executions());
        }
    }
} // end of namespace