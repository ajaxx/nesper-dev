///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2015 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using com.espertech.esper.common.@internal.support;
using com.espertech.esper.regressionlib.framework;

using NUnit.Framework;

namespace com.espertech.esper.regressionlib.suite.@event.xml
{
    public class EventXMLSchemaEventTypes : RegressionExecution
    {
        public void Run(RegressionEnvironment env)
        {
            var stmtSelectWild = "@Name('s0') select * from TestTypesEvent";
            env.CompileDeploy(stmtSelectWild).AddListener("s0");
            var type = env.Statement("s0").EventType;
            SupportEventTypeAssertionUtil.AssertConsistency(type);

            object[][] types = {
                new object[] {"attrNonPositiveInteger", typeof(int?)},
                new object[] {"attrNonNegativeInteger", typeof(int?)},
                new object[] {"attrNegativeInteger", typeof(int?)},
                new object[] {"attrPositiveInteger", typeof(int?)},
                new object[] {"attrLong", typeof(long?)},
                new object[] {"attrUnsignedLong", typeof(ulong?)},
                new object[] {"attrInt", typeof(int?)},
                new object[] {"attrUnsignedInt", typeof(uint?)},
                new object[] {"attrDecimal", typeof(double?)},
                new object[] {"attrInteger", typeof(int?)},
                new object[] {"attrFloat", typeof(float?)},
                new object[] {"attrDouble", typeof(double?)},
                new object[] {"attrString", typeof(string)},
                new object[] {"attrShort", typeof(short?)},
                new object[] {"attrUnsignedShort", typeof(ushort?)},
                new object[] {"attrByte", typeof(byte?)},
                new object[] {"attrUnsignedByte", typeof(byte?)},
                new object[] {"attrBoolean", typeof(bool?)},
                new object[] {"attrDateTime", typeof(string)},
                new object[] {"attrDate", typeof(string)},
                new object[] {"attrTime", typeof(string)}
            };

            for (var i = 0; i < types.Length; i++) {
                var name = types[i][0].ToString();
                var desc = type.GetPropertyDescriptor(name);
                var expected = (Type) types[i][1];
                Assert.AreEqual(expected, desc.PropertyType, "Failed for " + name);
            }

            env.UndeployAll();
        }
    }
} // end of namespace