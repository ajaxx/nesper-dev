///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2015 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Numerics;

using com.espertech.esper.common.client.configuration;
using com.espertech.esper.common.client.configuration.compiler;
using com.espertech.esper.common.@internal.support;
using com.espertech.esper.compat;
using com.espertech.esper.compat.collections;
using com.espertech.esper.regressionlib.suite.client.extension;
using com.espertech.esper.regressionlib.support.bean;
using com.espertech.esper.regressionlib.support.client;
using com.espertech.esper.regressionlib.support.extend.aggfunc;
using com.espertech.esper.regressionlib.support.extend.aggmultifunc;
using com.espertech.esper.regressionlib.support.extend.pattern;
using com.espertech.esper.regressionlib.support.extend.vdw;
using com.espertech.esper.regressionlib.support.extend.view;
using com.espertech.esper.regressionlib.support.util;
using com.espertech.esper.regressionrun.runner;

using NUnit.Framework;

using SupportBean_A = com.espertech.esper.regressionlib.support.bean.SupportBean_A;

namespace com.espertech.esper.regressionrun.suite.client
{
    [TestFixture]
    public class TestSuiteClientExtension
    {
        [SetUp]
        public void SetUp()
        {
            _session = RegressionRunner.Session();
            Configure(_session.Configuration);
        }

        [TearDown]
        public void TearDown()
        {
            _session.Dispose();
            _session = null;
        }

        private RegressionSession _session;

        private static void Configure(Configuration configuration)
        {
            foreach (var clazz in new[] {
                typeof(SupportBean),
                typeof(SupportBean_A),
                typeof(SupportBean_S0),
                typeof(SupportMarketDataBean),
                typeof(SupportSimpleBeanOne),
                typeof(SupportBean_ST0),
                typeof(SupportBeanRange),
                typeof(SupportDateTime),
                typeof(SupportCollection),
                typeof(SupportBean_ST0_Container)
            }) {
                configuration.Common.AddEventType(clazz.Name, clazz);
            }

            IDictionary<string, object> mapType = new Dictionary<string, object>();
            mapType.Put("col1", "string");
            mapType.Put("col2", "string");
            mapType.Put("col3", "int");
            configuration.Common.AddEventType("MapType", mapType);

            var configurationCompiler = configuration.Compiler;
            configurationCompiler.AddPlugInSingleRowFunction(
                "singlerow",
                typeof(SupportSingleRowFunctionTwo),
                "TestSingleRow");
            configurationCompiler.AddPlugInSingleRowFunction(
                "power3",
                typeof(SupportSingleRowFunction),
                "ComputePower3");
            configurationCompiler.AddPlugInSingleRowFunction(
                "chainTop",
                typeof(SupportSingleRowFunction),
                "GetChainTop");
            configurationCompiler.AddPlugInSingleRowFunction(
                "throwExceptionLogMe",
                typeof(SupportSingleRowFunction),
                "Throwexception",
                ConfigurationCompilerPlugInSingleRowFunction.ValueCacheEnum.DISABLED,
                ConfigurationCompilerPlugInSingleRowFunction.FilterOptimizableEnum.ENABLED,
                false);
            configurationCompiler.AddPlugInSingleRowFunction(
                "throwExceptionRethrow",
                typeof(SupportSingleRowFunction),
                "Throwexception",
                ConfigurationCompilerPlugInSingleRowFunction.ValueCacheEnum.DISABLED,
                ConfigurationCompilerPlugInSingleRowFunction.FilterOptimizableEnum.ENABLED,
                true);
            configurationCompiler.AddPlugInSingleRowFunction(
                "power3Rethrow",
                typeof(SupportSingleRowFunction),
                "ComputePower3",
                ConfigurationCompilerPlugInSingleRowFunction.ValueCacheEnum.DISABLED,
                ConfigurationCompilerPlugInSingleRowFunction.FilterOptimizableEnum.ENABLED,
                true);
            configurationCompiler.AddPlugInSingleRowFunction(
                "power3Context",
                typeof(SupportSingleRowFunction),
                "ComputePower3WithContext",
                ConfigurationCompilerPlugInSingleRowFunction.ValueCacheEnum.DISABLED,
                ConfigurationCompilerPlugInSingleRowFunction.FilterOptimizableEnum.ENABLED,
                true);
            foreach (var method in Collections.List(
                "Surroundx",
                "IsNullValue",
                "GetValueAsString",
                "EventsCheckStrings",
                "VarargsOnlyInt",
                "VarargsOnlyString",
                "VarargsOnlyObject",
                "VarargsOnlyNumber",
                "VarargsOnlyISupportBaseAB",
                "VarargsW1Param",
                "VarargsW2Param",
                "VarargsOnlyWCtx",
                "VarargsW1ParamWCtx",
                "VarargsW2ParamWCtx",
                "VarargsObjectsWCtx",
                "VarargsW1ParamObjectsWCtx",
                "VarargsOnlyBoxedFloat",
                "VarargsOnlyBoxedShort",
                "VarargsOnlyBoxedByte",
                "VarargOverload")
            ) {
                configurationCompiler.AddPlugInSingleRowFunction(method, typeof(SupportSingleRowFunction), method);
            }

            configurationCompiler.AddPlugInSingleRowFunction("extractNum", typeof(ClientExtendEnumMethod), "ExtractNum");

            AddEventTypeUDF(
                "MyItemProducerEventBeanArray",
                "MyItem",
                "MyItemProducerEventBeanArray",
                configuration);
            AddEventTypeUDF(
                "MyItemProducerEventBeanCollection",
                "MyItem",
                "MyItemProducerEventBeanCollection",
                configuration);
            AddEventTypeUDF(
                "MyItemProducerInvalidNoType",
                null,
                "MyItemProducerEventBeanArray",
                configuration);
            AddEventTypeUDF(
                "MyItemProducerInvalidWrongType",
                "dummy",
                "MyItemProducerEventBeanArray",
                configuration);

            configurationCompiler.AddPlugInAggregationFunctionForge(
                "concatstring",
                typeof(SupportConcatWManagedAggregationFunctionForge));
            configurationCompiler.AddPlugInAggregationFunctionForge(
                "myagg",
                typeof(SupportSupportBeanAggregationFunctionForge));
            configurationCompiler.AddPlugInAggregationFunctionForge(
                "countback",
                typeof(SupportCountBackAggregationFunctionForge));
            configurationCompiler.AddPlugInAggregationFunctionForge(
                "countboundary",
                typeof(SupportLowerUpperCompareAggregationFunctionForge));
            configurationCompiler.AddPlugInAggregationFunctionForge(
                "concatWCodegen",
                typeof(SupportConcatWCodegenAggregationFunctionForge));
            configurationCompiler.AddPlugInAggregationFunctionForge("invalidAggFuncForge", typeof(TimeSpan));
            configurationCompiler.AddPlugInAggregationFunctionForge("nonExistAggFuncForge", "com.NoSuchClass");

            var configGeneral = new ConfigurationCompilerPlugInAggregationMultiFunction(
                new[] {"ss", "sa", "sc", "se1", "se2", "ee"},
                typeof(SupportAggMFMultiRTForge));
            configGeneral.AdditionalConfiguredProperties = Collections.SingletonDataMap("someinfokey", "someinfovalue");

            configurationCompiler.AddPlugInAggregationMultiFunction(configGeneral);
            var codegenTestAccum = new ConfigurationCompilerPlugInAggregationMultiFunction(
                new[] {"collectEvents"},
                typeof(SupportAggMFEventsAsListForge));
            configurationCompiler.AddPlugInAggregationMultiFunction(codegenTestAccum);
            // For use with the inlined-class example when disabled, comment-in when needed:
            // ConfigurationCompilerPlugInAggregationMultiFunction codegenTestTrie = new ConfigurationCompilerPlugInAggregationMultiFunction("".Split(","), ClientExtendAggregationMultiFunctionInlinedClass.TrieAggForge.class.getName());
            // configurationCompiler.addPlugInAggregationMultiFunction(codegenTestTrie);

            configuration.Compiler.AddPlugInView("mynamespace", "flushedsimple", typeof(MyFlushedSimpleViewForge));
            configuration.Compiler.AddPlugInView("mynamespace", "invalid", typeof(string));
            configuration.Compiler.AddPlugInView("mynamespace", "trendspotter", typeof(MyTrendSpotterViewForge));

            configurationCompiler.AddPlugInVirtualDataWindow("test", "vdwnoparam", typeof(SupportVirtualDWForge));
            configurationCompiler.AddPlugInVirtualDataWindow(
                "test",
                "vdwwithparam",
                typeof(SupportVirtualDWForge),
                SupportVirtualDW.ITERATE); // configure with iteration
            configurationCompiler.AddPlugInVirtualDataWindow("test", "vdw", typeof(SupportVirtualDWForge));
            configurationCompiler.AddPlugInVirtualDataWindow("invalid", "invalid", typeof(SupportBean));
            configurationCompiler.AddPlugInVirtualDataWindow(
                "test",
                "testnoindex",
                typeof(SupportVirtualDWInvalidForge));
            configurationCompiler.AddPlugInVirtualDataWindow(
                "test",
                "exceptionvdw",
                typeof(SupportVirtualDWExceptionForge));

            configurationCompiler.AddPlugInPatternGuard(
                "myplugin",
                "count_to",
                typeof(MyCountToPatternGuardForge));
            configurationCompiler.AddPlugInPatternGuard("namespace", "name", typeof(string));

            configurationCompiler.AddPlugInDateTimeMethod("roll", typeof(ClientExtendDateTimeMethod.MyLocalDTMForgeFactoryRoll));
            configurationCompiler.AddPlugInDateTimeMethod("asArrayOfString", typeof(ClientExtendDateTimeMethod.MyLocalDTMForgeFactoryArrayOfString));
            configurationCompiler.AddPlugInDateTimeMethod(
                "dtmInvalidMethodNotExists",
                typeof(ClientExtendDateTimeMethod.MyLocalDTMForgeFactoryInvalidMethodNotExists));
            configurationCompiler.AddPlugInDateTimeMethod("dtmInvalidNotProvided", typeof(ClientExtendDateTimeMethod.MyLocalDTMForgeFactoryInvalidNotProvided));
            configurationCompiler.AddPlugInDateTimeMethod("someDTMInvalidReformat", typeof(ClientExtendDateTimeMethod.MyLocalDTMForgeFactoryInvalidReformat));
            configurationCompiler.AddPlugInDateTimeMethod("someDTMInvalidNoOp", typeof(ClientExtendDateTimeMethod.MyLocalDTMForgeFactoryInvalidNoOp));

            configurationCompiler.AddPlugInEnumMethod("enumPlugInMedian", typeof(ClientExtendEnumMethod.MyLocalEnumMethodForgeMedian));
            configurationCompiler.AddPlugInEnumMethod("enumPlugInOne", typeof(ClientExtendEnumMethod.MyLocalEnumMethodForgeOne));
            configurationCompiler.AddPlugInEnumMethod("enumPlugInEarlyExit", typeof(ClientExtendEnumMethod.MyLocalEnumMethodForgeEarlyExit));
            configurationCompiler.AddPlugInEnumMethod("enumPlugInReturnEvents", typeof(ClientExtendEnumMethod.MyLocalEnumMethodForgePredicateReturnEvents));
            configurationCompiler.AddPlugInEnumMethod(
                "enumPlugInReturnSingleEvent",
                typeof(ClientExtendEnumMethod.MyLocalEnumMethodForgePredicateReturnSingleEvent));
            configurationCompiler.AddPlugInEnumMethod("enumPlugInTwoLambda", typeof(ClientExtendEnumMethod.MyLocalEnumMethodForgeTwoLambda));
            configurationCompiler.AddPlugInEnumMethod("enumPlugInLambdaEventWPredicateAndIndex", typeof(ClientExtendEnumMethod.MyLocalEnumMethodForgeThree));
            configurationCompiler.AddPlugInEnumMethod("enumPlugInLambdaScalarWPredicateAndIndex", typeof(ClientExtendEnumMethod.MyLocalEnumMethodForgeThree));
            configurationCompiler.AddPlugInEnumMethod("enumPlugInLambdaScalarWStateAndValue", typeof(ClientExtendEnumMethod.MyLocalEnumMethodForgeStateWValue));


            configuration.Common.AddImportType(typeof(ClientExtendSingleRowFunction));
            configuration.Common.AddImportType(typeof(BigInteger));

            configuration.Runtime.Threading.IsRuntimeFairlock = true;
            configuration.Common.Logging.IsEnableQueryPlan = true;
        }

        private static void AddEventTypeUDF(
            string name,
            string eventTypeName,
            string functionMethodName,
            Configuration configuration)
        {
            var entry = new ConfigurationCompilerPlugInSingleRowFunction();
            entry.Name = name;
            entry.FunctionClassName = typeof(ClientExtendUDFReturnTypeIsEvents).FullName;
            entry.FunctionMethodName = functionMethodName;
            entry.EventTypeName = eventTypeName;
            configuration.Compiler.AddPlugInSingleRowFunction(entry);
        }

        [Test, RunInApplicationDomain]

        public void TestClientExtendUDFReturnTypeIsEvents()
        {
            RegressionRunner.Run(_session, new ClientExtendUDFReturnTypeIsEvents());
        }

        [Test, RunInApplicationDomain]
        public void TestClientExtendUDFVarargs()
        {
            RegressionRunner.Run(_session, new ClientExtendUDFVarargs());
        }

        [Test, RunInApplicationDomain]
        public void TestClientExtendUDFInlinedClass()
        {
            RegressionRunner.Run(_session, ClientExtendUDFInlinedClass.Executions());
        }

        [Test, RunInApplicationDomain]
        public void TestClientExtendAggregationFunction()
        {
            RegressionRunner.Run(_session, ClientExtendAggregationFunction.Executions());
        }

        [Test, RunInApplicationDomain]
        public void TestClientExtendAggregationInlinedClass()
        {
            RegressionRunner.Run(_session, ClientExtendAggregationInlinedClass.Executions());
        }

        [Test, RunInApplicationDomain]
        public void TestClientExtendAggregationMultiFunction()
        {
            RegressionRunner.Run(_session, ClientExtendAggregationMultiFunction.Executions());
        }

        [Test, RunInApplicationDomain]
        public void TestClientExtendAggregationMultiFunctionInlinedClass()
        {
            RegressionRunner.Run(_session, ClientExtendAggregationMultiFunctionInlinedClass.Executions());
        }

        [Test, RunInApplicationDomain]
        public void TestClientExtendView()
        {
            RegressionRunner.Run(_session, new ClientExtendView());
        }

        [Test, RunInApplicationDomain]
        public void TestClientExtendVirtualDataWindow()
        {
            RegressionRunner.Run(_session, new ClientExtendVirtualDataWindow());
        }

        [Test, RunInApplicationDomain]
        public void TestClientExtendPatternGuard()
        {
            RegressionRunner.Run(_session, new ClientExtendPatternGuard());
        }

        [Test, RunInApplicationDomain]
        public void TestClientExtendSingleRowFunction()
        {
            RegressionRunner.Run(_session, ClientExtendSingleRowFunction.Executions());
        }

        [Test, RunInApplicationDomain]
        public void TestClientExtendAdapterLoaderLoad()
        {
            RegressionSession session = RegressionRunner.Session();

            Properties props = new Properties();
            props.Put("name", "val");
            session.Configuration.Runtime.AddPluginLoader("MyLoader", typeof(SupportPluginLoader), props);

            props = new Properties();
            props.Put("name2", "val2");
            session.Configuration.Runtime.AddPluginLoader("MyLoader2", typeof(SupportPluginLoader), props);

            RegressionRunner.Run(session, new ClientExtendAdapterLoader());

            session.Dispose();
        }

        [Test, RunInApplicationDomain]

        public void TestClientExtendDateTimeMethod()
        {
            RegressionRunner.Run(_session, ClientExtendDateTimeMethod.Executions());
        }

        [Test, RunInApplicationDomain]

        public void TestClientExtendEnumMethod()
        {
            RegressionRunner.Run(_session, ClientExtendEnumMethod.Executions());
        }
    }
} // end of namespace