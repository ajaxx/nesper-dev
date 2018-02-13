///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2017 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using com.espertech.esper.client;
using com.espertech.esper.client.scopetest;
using com.espertech.esper.compat;
using com.espertech.esper.compat.collections;
using com.espertech.esper.compat.logging;
using com.espertech.esper.supportregression.bean;
using com.espertech.esper.supportregression.bean.lambda;
using com.espertech.esper.supportregression.bean.sales;
using com.espertech.esper.supportregression.execution;

// using static org.junit.Assert.assertEquals;

using NUnit.Framework;

namespace com.espertech.esper.regression.expr.enummethod
{
    public class ExecEnumChained : RegressionExecution {
    
        public override void Configure(Configuration configuration) {
            configuration.AddEventType("Bean", typeof(SupportBean_ST0_Container));
            configuration.AddEventType("PersonSales", typeof(PersonSales));
        }
    
        public override void Run(EPServiceProvider epService) {
    
            string eplFragment = "select Sales.Where(x => x.cost > 1000).min(y => y.buyer.age) as val from PersonSales";
            EPStatement stmtFragment = epService.EPAdministrator.CreateEPL(eplFragment);
            var listener = new SupportUpdateListener();
            stmtFragment.AddListener(listener);
            LambdaAssertionUtil.AssertTypes(stmtFragment.EventType, "val".Split(','), new Type[]{typeof(int?)});
    
            PersonSales bean = PersonSales.Make();
            epService.EPRuntime.SendEvent(bean);
            Assert.AreEqual(50, listener.AssertOneGetNewAndReset().Get("val"));
        }
    }
} // end of namespace