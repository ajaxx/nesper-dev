///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2017 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using com.espertech.esper.client;
using com.espertech.esper.compat;
using com.espertech.esper.compat.collections;
using com.espertech.esper.compat.logging;
using com.espertech.esper.supportregression.bean;
using com.espertech.esper.supportregression.execution;

// using static org.junit.Assert.assertEquals;
// using static org.junit.Assert.fail;

using NUnit.Framework;

namespace com.espertech.esper.regression.expr.datetime
{
    public class ExecDTIntervalOpsInvalidConfig : RegressionExecution {
    
        public override void Run(EPServiceProvider epService) {
            var configBean = new ConfigurationEventTypeLegacy();
    
            configBean.StartTimestampPropertyName = null;
            configBean.EndTimestampPropertyName = "caldate";
            TryInvalidConfig(epService, typeof(SupportDateTime), configBean, "Declared end timestamp property requires that a start timestamp property is also declared");
    
            configBean.StartTimestampPropertyName = "xyz";
            configBean.EndTimestampPropertyName = null;
            TryInvalidConfig(epService, typeof(SupportBean), configBean, "Declared start timestamp property name 'xyz' was not found");
    
            configBean.StartTimestampPropertyName = "longPrimitive";
            configBean.EndTimestampPropertyName = "xyz";
            TryInvalidConfig(epService, typeof(SupportBean), configBean, "Declared end timestamp property name 'xyz' was not found");
    
            configBean.EndTimestampPropertyName = null;
            configBean.StartTimestampPropertyName = "theString";
            TryInvalidConfig(epService, typeof(SupportBean), configBean, "Declared start timestamp property 'theString' is expected to return a Date, Calendar or long-typed value but returns 'java.lang.string'");
    
            configBean.StartTimestampPropertyName = "longPrimitive";
            configBean.EndTimestampPropertyName = "theString";
            TryInvalidConfig(epService, typeof(SupportBean), configBean, "Declared end timestamp property 'theString' is expected to return a Date, Calendar or long-typed value but returns 'java.lang.string'");
    
            configBean.StartTimestampPropertyName = "longdate";
            configBean.EndTimestampPropertyName = "caldate";
            TryInvalidConfig(epService, typeof(SupportDateTime), configBean, "Declared end timestamp property 'caldate' is expected to have the same property type as the start-timestamp property 'longdate'");
        }
    
        private void TryInvalidConfig(EPServiceProvider epService, Type clazz, ConfigurationEventTypeLegacy config, string message) {
            try {
                epService.EPAdministrator.Configuration.AddEventType(clazz.Name, clazz.Name, config);
                Assert.Fail();
            } catch (ConfigurationException ex) {
                Assert.AreEqual(message, ex.Message);
            }
        }
    }
} // end of namespace