///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2015 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

using com.espertech.esper.common.client.scopetest;
using com.espertech.esper.compat.collections;
using com.espertech.esper.regressionlib.framework;
using com.espertech.esper.regressionlib.support.bean;

using NUnit.Framework;

using static com.espertech.esper.regressionlib.suite.@event.map.EventMapCore;

namespace com.espertech.esper.regressionlib.suite.@event.objectarray
{
    public class EventObjectArrayEventNestedPono : RegressionExecution
    {
        public void Run(RegressionEnvironment env)
        {
            var statementText = "@Name('s0') select " +
                                "simple, " +
                                "object, " +
                                "nodefmap, " +
                                "map, " +
                                "object.Id as a1, " +
                                "nodefmap.key1? as a2, " +
                                "nodefmap.key2? as a3, " +
                                "nodefmap.key3?.key4 as a4, " +
                                "map.objectOne as b1, " +
                                "map.simpleOne as b2, " +
                                "map.nodefmapOne.key2? as b3, " +
                                "map.mapOne.simpleTwo? as b4, " +
                                "map.objectOne.Indexed[1] as c1, " +
                                "map.objectOne.Nested.NestedValue as c2," +
                                "map.mapOne.simpleTwo as d1, " +
                                "map.mapOne.objectTwo as d2, " +
                                "map.mapOne.nodefmapTwo as d3, " +
                                "map.mapOne.mapTwo as e1, " +
                                "map.mapOne.mapTwo.simpleThree as e2, " +
                                "map.mapOne.mapTwo.objectThree as e3, " +
                                "map.mapOne.objectTwo.Array[1].Mapped('1ma').Value as f1, " +
                                "map.mapOne.mapTwo.objectThree.Id as f2" +
                                " from NestedObjectArr";
            env.CompileDeploy(statementText).AddListener("s0");

            var testdata = GetTestData();
            env.SendEventObjectArray(testdata, "NestedObjectArr");

            // test all properties exist
            var received = env.Listener("s0").AssertOneGetNewAndReset();
            EPAssertionUtil.AssertProps(
                received,
                new [] { "simple","object","nodefmap","map" },
                new[] { "abc", new SupportBean_A("A1"), testdata[2], testdata[3] });
            EPAssertionUtil.AssertProps(
                received,
                new [] { "a1","a2","a3","a4" },
                new object[] {"A1", "val1", null, null});
            EPAssertionUtil.AssertProps(
                received,
                new [] { "b1","b2","b3","b4" },
                new[] {EventObjectArrayCore.GetNestedKeyOA(testdata, 3, "objectOne"), 10, "val2", 300});
            EPAssertionUtil.AssertProps(
                received,
                new [] { "c1","c2" },
                new object[] {2, "NestedValue"});
            EPAssertionUtil.AssertProps(
                received,
                new [] { "d1","d2","d3" },
                new[] {
                    300,
                    EventObjectArrayCore.GetNestedKeyOA(testdata, 3, "mapOne", "objectTwo"),
                    EventObjectArrayCore.GetNestedKeyOA(testdata, 3, "mapOne", "nodefmapTwo")
                });
            EPAssertionUtil.AssertProps(
                received,
                new [] { "e1","e2","e3" },
                new[] {
                    EventObjectArrayCore.GetNestedKeyOA(testdata, 3, "mapOne", "mapTwo"), 4000L, new SupportBean_B("B1")
                });
            EPAssertionUtil.AssertProps(
                received,
                new [] { "f1","f2" },
                new object[] {"1ma0", "B1"});
            env.UndeployModuleContaining("s0");

            // assert type info
            env.CompileDeploy("@Name('s0') select * from NestedObjectArr").AddListener("s0");
            var eventType = env.Statement("s0").EventType;

            var propertiesReceived = eventType.PropertyNames;
            string[] propertiesExpected = {"simple", "object", "nodefmap", "map"};
            EPAssertionUtil.AssertEqualsAnyOrder(propertiesReceived, propertiesExpected);
            Assert.AreEqual(typeof(string), eventType.GetPropertyType("simple"));
            Assert.AreEqual(typeof(IDictionary<string, object>), eventType.GetPropertyType("map"));
            Assert.AreEqual(typeof(IDictionary<string, object>), eventType.GetPropertyType("nodefmap"));
            Assert.AreEqual(typeof(SupportBean_A), eventType.GetPropertyType("object"));

            Assert.IsNull(eventType.GetPropertyType("map.mapOne.simpleOne"));

            // nested PONO with generic return type
            env.UndeployModuleContaining("s0");
            env.CompileDeploy("@Name('s0') select * from MyNested(bean.Insides.anyOf(i->Id = 'A'))").AddListener("s0");

            env.SendEventObjectArray(new object[] {new MyNested(Arrays.AsList(new MyInside("A")))}, "MyNested");
            Assert.IsTrue(env.Listener("s0").IsInvoked);

            env.UndeployAll();
        }

        private object[] GetTestData()
        {
            var levelThree = MakeMap(
                new[] {
                    new object[] {"simpleThree", 4000L},
                    new object[] {"objectThree", new SupportBean_B("B1")}
                });

            var levelTwo = MakeMap(
                new[] {
                    new object[] {"simpleTwo", 300},
                    new object[] {"objectTwo", SupportBeanCombinedProps.MakeDefaultBean()},
                    new object[] {"nodefmapTwo", MakeMap(new[] {new object[] {"key3", "val3"}})},
                    new object[] {"mapTwo", levelThree}
                });

            var levelOne = MakeMap(
                new[] {
                    new object[] {"simpleOne", 10},
                    new object[] {"objectOne", SupportBeanComplexProps.MakeDefaultBean()},
                    new object[] {"nodefmapOne", MakeMap(new[] {new object[] {"key2", "val2"}})},
                    new object[] {"mapOne", levelTwo}
                });

            object[] levelZero = {
                "abc", new SupportBean_A("A1"), MakeMap(
                    new[] {
                        new object[] {"key1", "val1"}
                    }),
                levelOne
            };
            return levelZero;
        }

        public class MyNested
        {
            public MyNested(IList<MyInside> insides)
            {
                Insides = insides;
            }

            public IList<MyInside> Insides { get; }
        }

        public class MyInside
        {
            public MyInside(string id)
            {
                Id = id;
            }

            public string Id { get; }
        }
    }
} // end of namespace