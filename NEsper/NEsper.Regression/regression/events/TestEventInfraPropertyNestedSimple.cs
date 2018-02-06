///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2015 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Xml;

using Avro;
using Avro.Generic;

using com.espertech.esper.client;
using com.espertech.esper.client.annotation;
using com.espertech.esper.client.scopetest;
using com.espertech.esper.compat;
using com.espertech.esper.compat.collections;
using com.espertech.esper.metrics.instrumentation;
using com.espertech.esper.supportregression.client;
using com.espertech.esper.supportregression.util;
using com.espertech.esper.supportregression.events;
using com.espertech.esper.util;
using com.espertech.esper.util.support;

using NEsper.Avro;
using NEsper.Avro.Extensions;

using Newtonsoft.Json.Linq;

using NUnit.Framework;

namespace com.espertech.esper.regression.events
{
    [TestFixture]
	public class TestEventInfraPropertyNestedSimple
    {
	    private static readonly string BEAN_TYPENAME = typeof(InfraNestedSimplePropTop).Name;

	    private static readonly FunctionSendEvent4Int FMAP = (epService, lvl1, lvl2, lvl3, lvl4) => {
            var l4 = Collections.SingletonDataMap("lvl4", lvl4);
            IDictionary<string, object> l3 = SupportEventInfra.TwoEntryMap("l4", l4, "lvl3", lvl3);
            IDictionary<string, object> l2 = SupportEventInfra.TwoEntryMap("l3", l3, "lvl2", lvl2);
            IDictionary<string, object> l1 = SupportEventInfra.TwoEntryMap("l2", l2, "lvl1", lvl1);
            var top = Collections.SingletonDataMap("l1", l1);
            epService.EPRuntime.SendEvent(top, SupportEventInfra.MAP_TYPENAME);
	    };

	    private static readonly FunctionSendEvent4Int FOA = (epService, lvl1, lvl2, lvl3, lvl4) => {
	        var l4 = new object[] {lvl4};
	        var l3 = new object[] {l4, lvl3};
	        var l2 = new object[] {l3, lvl2};
	        var l1 = new object[] {l2, lvl1};
	        var top = new object[] {l1};
            epService.EPRuntime.SendEvent(top, SupportEventInfra.OA_TYPENAME);
	    };

	    private static readonly FunctionSendEvent4Int FBEAN = (epService, lvl1, lvl2, lvl3, lvl4) => {
	        var l4 = new InfraNestedSimplePropLvl4(lvl4);
	        var l3 = new InfraNestedSimplePropLvl3(l4, lvl3);
	        var l2 = new InfraNestedSimplePropLvl2(l3, lvl2);
	        var l1 = new InfraNestedSimplePropLvl1(l2, lvl1);
	        var top = new InfraNestedSimplePropTop(l1);
	        epService.EPRuntime.SendEvent(top);
	    };

	    private static readonly FunctionSendEvent4Int FXML = (epService, lvl1, lvl2, lvl3, lvl4) => {
	        var xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" +
	        "<myevent>\n" +
	        "\t<l1 lvl1=\"${lvl1}\">\n" +
	        "\t\t<l2 lvl2=\"${lvl2}\">\n" +
	        "\t\t\t<l3 lvl3=\"${lvl3}\">\n" +
	        "\t\t\t\t<l4 lvl4=\"${lvl4}\">\n" +
	        "\t\t\t\t</l4>\n" +
	        "\t\t\t</l3>\n" +
	        "\t\t</l2>\n" +
	        "\t</l1>\n" +
	        "</myevent>";
	        xml = xml.Replace("${lvl1}", lvl1.ToString());
	        xml = xml.Replace("${lvl2}", lvl2.ToString());
	        xml = xml.Replace("${lvl3}", lvl3.ToString());
	        xml = xml.Replace("${lvl4}", lvl4.ToString());
	        try {
	            SupportXML.SendEvent(epService.EPRuntime, xml);
	        } catch (Exception e) {
	            throw new EPException(e);
	        }
	    };

	    private static readonly FunctionSendEvent4Int FAVRO = (epService, lvl1, lvl2, lvl3, lvl4) => {
	        var schema = GetAvroSchema();
            
	        RecordSchema lvl1Schema = schema.GetField("l1").Schema.AsRecordSchema();
            RecordSchema lvl2Schema = lvl1Schema.GetField("l2").Schema.AsRecordSchema();
            RecordSchema lvl3Schema = lvl2Schema.GetField("l3").Schema.AsRecordSchema();
            RecordSchema lvl4Schema = lvl3Schema.GetField("l4").Schema.AsRecordSchema();

	        var lvl4Rec = new GenericRecord(lvl4Schema);
	        lvl4Rec.Put("lvl4", lvl4);
	        var lvl3Rec = new GenericRecord(lvl3Schema);
	        lvl3Rec.Put("l4", lvl4Rec);
	        lvl3Rec.Put("lvl3", lvl3);
	        var lvl2Rec = new GenericRecord(lvl2Schema);
	        lvl2Rec.Put("l3", lvl3Rec);
	        lvl2Rec.Put("lvl2", lvl2);
	        var lvl1Rec = new GenericRecord(lvl1Schema);
	        lvl1Rec.Put("l2", lvl2Rec);
	        lvl1Rec.Put("lvl1", lvl1);
	        var datum = new GenericRecord(schema);
	        datum.Put("l1", lvl1Rec);
	        epService.EPRuntime.SendEventAvro(datum, SupportEventInfra.AVRO_TYPENAME);
	    };

	    private EPServiceProvider _epService;

        [SetUp]
        public void SetUp()
        {
            var configuration = SupportConfigFactory.GetConfiguration();
	        AddXMLEventType(configuration);

	        _epService = EPServiceProviderManager.GetDefaultProvider(configuration);
	        _epService.Initialize();

	        AddMapEventType();
	        AddOAEventType();
	        _epService.EPAdministrator.Configuration.AddEventType(BEAN_TYPENAME, typeof(InfraNestedSimplePropTop));
	        AddAvroEventType();

	        if (InstrumentationHelper.ENABLED) {
	            InstrumentationHelper.StartTest(_epService, GetType(), GetType().FullName);
	        }
	    }

        [TearDown]
	    public void TearDown() {
	        if (InstrumentationHelper.ENABLED) {
	            InstrumentationHelper.EndTest();
	        }
	    }

        [Test]
	    public void TestNested() {
	        RunAssertion(BEAN_TYPENAME, FBEAN, typeof(InfraNestedSimplePropLvl1), typeof(InfraNestedSimplePropLvl1).Name);
            RunAssertion(SupportEventInfra.MAP_TYPENAME, FMAP, typeof(IDictionary<string, object>), SupportEventInfra.MAP_TYPENAME + "_1");
            RunAssertion(SupportEventInfra.OA_TYPENAME, FOA, typeof(object[]), SupportEventInfra.OA_TYPENAME + "_1");
            RunAssertion(SupportEventInfra.XML_TYPENAME, FXML, typeof(XmlNode), "MyXMLEvent.l1");
            RunAssertion(SupportEventInfra.AVRO_TYPENAME, FAVRO, typeof(GenericRecord), "MyAvroEvent_1");
	    }

	    private void RunAssertion(string typename, FunctionSendEvent4Int send, Type nestedClass, string fragmentTypeName) {
	        RunAssertionSelectNested(typename, send);
	        RunAssertionBeanNav(typename, send);
	        RunAssertionTypeValidProp(typename, send, nestedClass, fragmentTypeName);
	        RunAssertionTypeInvalidProp(typename);
	    }

	    private void RunAssertionBeanNav(string typename, FunctionSendEvent4Int send) {
	        var epl = "select * from " + typename;
	        var statement = _epService.EPAdministrator.CreateEPL(epl);
	        var listener = new SupportUpdateListener();
	        statement.AddListener(listener);

	        send.Invoke(_epService, 1, 2, 3, 4);
	        var @event = listener.AssertOneGetNewAndReset();
	        EPAssertionUtil.AssertProps(@event, "l1.lvl1,l1.l2.lvl2,l1.l2.l3.lvl3,l1.l2.l3.l4.lvl4".SplitCsv(), new object[] {1, 2, 3, 4});
	        SupportEventTypeAssertionUtil.AssertConsistency(@event);
	        SupportEventTypeAssertionUtil.AssertFragments(@event, typename.Equals(BEAN_TYPENAME), false, "l1,l1.l2,l1.l2.l3,l1.l2.l3.l4");
	        RunAssertionEventInvalidProp(@event);

	        statement.Dispose();
	    }

	    private void RunAssertionSelectNested(string typename, FunctionSendEvent4Int send) {
	        var epl = "select " +
	                     "l1.lvl1 as c0, " +
	                     "exists(l1.lvl1) as exists_c0, " +
	                     "l1.l2.lvl2 as c1, " +
	                     "exists(l1.l2.lvl2) as exists_c1, " +
	                     "l1.l2.l3.lvl3 as c2, " +
	                     "exists(l1.l2.l3.lvl3) as exists_c2, " +
	                     "l1.l2.l3.l4.lvl4 as c3, " +
	                     "exists(l1.l2.l3.l4.lvl4) as exists_c3 " +
	                     "from " + typename;
	        var statement = _epService.EPAdministrator.CreateEPL(epl);
	        var listener = new SupportUpdateListener();
	        statement.AddListener(listener);
	        var fields = "c0,exists_c0,c1,exists_c1,c2,exists_c2,c3,exists_c3".SplitCsv();

	        foreach (var property in fields) {
	            Assert.AreEqual(property.StartsWith("exists") ? typeof(bool?) : typeof(int?), TypeHelper.GetBoxedType(statement.EventType.GetPropertyType(property)));
	        }

	        send.Invoke(_epService, 1, 2, 3, 4);
	        var @event = listener.AssertOneGetNewAndReset();
	        EPAssertionUtil.AssertProps(@event, fields, new object[] {1, true, 2, true, 3, true, 4, true});
	        SupportEventTypeAssertionUtil.AssertConsistency(@event);

	        send.Invoke(_epService, 10, 5, 50, 400);
	        EPAssertionUtil.AssertProps(listener.AssertOneGetNewAndReset(), fields, new object[] {10, true, 5, true, 50, true, 400, true});

	        statement.Dispose();
	    }

	    private void AddMapEventType() {
            _epService.EPAdministrator.Configuration.AddEventType(SupportEventInfra.MAP_TYPENAME + "_4", Collections.SingletonDataMap("lvl4", typeof(int)));
            _epService.EPAdministrator.Configuration.AddEventType(SupportEventInfra.MAP_TYPENAME + "_3", SupportEventInfra.TwoEntryMap("l4", SupportEventInfra.MAP_TYPENAME + "_4", "lvl3", typeof(int)));
            _epService.EPAdministrator.Configuration.AddEventType(SupportEventInfra.MAP_TYPENAME + "_2", SupportEventInfra.TwoEntryMap("l3", SupportEventInfra.MAP_TYPENAME + "_3", "lvl2", typeof(int)));
            _epService.EPAdministrator.Configuration.AddEventType(SupportEventInfra.MAP_TYPENAME + "_1", SupportEventInfra.TwoEntryMap("l2", SupportEventInfra.MAP_TYPENAME + "_2", "lvl1", typeof(int)));
            _epService.EPAdministrator.Configuration.AddEventType(SupportEventInfra.MAP_TYPENAME, Collections.SingletonDataMap("l1", SupportEventInfra.MAP_TYPENAME + "_1"));
	    }

	    private void AddOAEventType() {
            var type_4 = SupportEventInfra.OA_TYPENAME + "_4";
	        string[] names_4 = {"lvl4"};
	        object[] types_4 = {typeof(int)};
	        _epService.EPAdministrator.Configuration.AddEventType(type_4, names_4, types_4);
            var type_3 = SupportEventInfra.OA_TYPENAME + "_3";
	        string[] names_3 = {"l4", "lvl3"};
	        object[] types_3 = {type_4, typeof(int)};
	        _epService.EPAdministrator.Configuration.AddEventType(type_3, names_3, types_3);
            var type_2 = SupportEventInfra.OA_TYPENAME + "_2";
	        string[] names_2 = {"l3", "lvl2"};
	        object[] types_2 = {type_3, typeof(int)};
	        _epService.EPAdministrator.Configuration.AddEventType(type_2, names_2, types_2);
            var type_1 = SupportEventInfra.OA_TYPENAME + "_1";
	        string[] names_1 = {"l2", "lvl1"};
	        object[] types_1 = {type_2, typeof(int)};
	        _epService.EPAdministrator.Configuration.AddEventType(type_1, names_1, types_1);
	        string[] names = {"l1"};
	        object[] types = {type_1};
            _epService.EPAdministrator.Configuration.AddEventType(SupportEventInfra.OA_TYPENAME, names, types);
	    }

	    private void AddXMLEventType(Configuration configuration) {
	        var eventTypeMeta = new ConfigurationEventTypeXMLDOM();
	        eventTypeMeta.RootElementName = "myevent";
	        var schema = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" +
	                        "<xs:schema targetNamespace=\"http://www.espertech.com/schema/esper\" elementFormDefault=\"qualified\" xmlns:esper=\"http://www.espertech.com/schema/esper\" xmlns:xs=\"http://www.w3.org/2001/XMLSchema\">\n" +
	                        "\t<xs:element name=\"myevent\">\n" +
	                        "\t\t<xs:complexType>\n" +
	                        "\t\t\t<xs:sequence>\n" +
	                        "\t\t\t\t<xs:element ref=\"esper:l1\"/>\n" +
	                        "\t\t\t</xs:sequence>\n" +
	                        "\t\t</xs:complexType>\n" +
	                        "\t</xs:element>\n" +
	                        "\t<xs:element name=\"l1\">\n" +
	                        "\t\t<xs:complexType>\n" +
	                        "\t\t\t<xs:sequence>\n" +
	                        "\t\t\t\t<xs:element ref=\"esper:l2\"/>\n" +
	                        "\t\t\t</xs:sequence>\n" +
	                        "\t\t\t<xs:attribute name=\"lvl1\" type=\"xs:int\" use=\"required\"/>\n" +
	                        "\t\t</xs:complexType>\n" +
	                        "\t</xs:element>\n" +
	                        "\t<xs:element name=\"l2\">\n" +
	                        "\t\t<xs:complexType>\n" +
	                        "\t\t\t<xs:sequence>\n" +
	                        "\t\t\t\t<xs:element ref=\"esper:l3\"/>\n" +
	                        "\t\t\t</xs:sequence>\n" +
	                        "\t\t\t<xs:attribute name=\"lvl2\" type=\"xs:int\" use=\"required\"/>\n" +
	                        "\t\t</xs:complexType>\n" +
	                        "\t</xs:element>\n" +
	                        "\t<xs:element name=\"l3\">\n" +
	                        "\t\t<xs:complexType>\n" +
	                        "\t\t\t<xs:sequence>\n" +
	                        "\t\t\t\t<xs:element ref=\"esper:l4\"/>\n" +
	                        "\t\t\t</xs:sequence>\n" +
	                        "\t\t\t<xs:attribute name=\"lvl3\" type=\"xs:int\" use=\"required\"/>\n" +
	                        "\t\t</xs:complexType>\n" +
	                        "\t</xs:element>\n" +
	                        "\t<xs:element name=\"l4\">\n" +
	                        "\t\t<xs:complexType>\n" +
	                        "\t\t\t<xs:attribute name=\"lvl4\" type=\"xs:int\" use=\"required\"/>\n" +
	                        "\t\t</xs:complexType>\n" +
	                        "\t</xs:element>\n" +
	                        "</xs:schema>\n";
	        eventTypeMeta.SchemaText = schema;
            configuration.AddEventType(SupportEventInfra.XML_TYPENAME, eventTypeMeta);
	    }

	    private void AddAvroEventType() {
            _epService.EPAdministrator.Configuration.AddEventTypeAvro(SupportEventInfra.AVRO_TYPENAME, new ConfigurationEventTypeAvro(GetAvroSchema()));
	    }

	    private static RecordSchema GetAvroSchema()
        {
            var s4 = SchemaBuilder.Record(SupportEventInfra.AVRO_TYPENAME + "_4", 
                TypeBuilder.RequiredInt("lvl4"));
            var s3 = SchemaBuilder.Record(SupportEventInfra.AVRO_TYPENAME + "_3",
                TypeBuilder.Field("l4", s4),
                TypeBuilder.RequiredInt("lvl3"));
            var s2 = SchemaBuilder.Record(SupportEventInfra.AVRO_TYPENAME + "_2",
                TypeBuilder.Field("l3", s3),
                TypeBuilder.RequiredInt("lvl2"));
            var s1 = SchemaBuilder.Record(SupportEventInfra.AVRO_TYPENAME + "_1",
                TypeBuilder.Field("l2", s2),
                TypeBuilder.RequiredInt("lvl1"));
            return SchemaBuilder.Record(SupportEventInfra.AVRO_TYPENAME,
                TypeBuilder.Field("l1", s1));
	    }

	    private void RunAssertionEventInvalidProp(EventBean @event) {
	        foreach (var prop in Collections.List("l2", "l1.l3", "l1.xxx", "l1.l2.x", "l1.l2.l3.x", "l1.lvl1.x")) {
	            SupportMessageAssertUtil.TryInvalidProperty(@event, prop);
	            SupportMessageAssertUtil.TryInvalidGetFragment(@event, prop);
	        }
	    }

	    private void RunAssertionTypeValidProp(string typeName, FunctionSendEvent4Int send, Type nestedClass, string fragmentTypeName) {
	        var eventType = _epService.EPAdministrator.Configuration.GetEventType(typeName);

	        var expectedType = new object[][] { new object[] {"l1", nestedClass, fragmentTypeName, false} };
	        SupportEventTypeAssertionUtil.AssertEventTypeProperties(expectedType, eventType, SupportEventTypeAssertionEnumExtensions.GetSetWithFragment());

	        EPAssertionUtil.AssertEqualsAnyOrder(new string[] {"l1"}, eventType.PropertyNames);

	        foreach (var prop in Collections.List("l1", "l1.lvl1", "l1.l2", "l1.l2.lvl2")) {
	            Assert.IsNotNull(eventType.GetGetter(prop));
	            Assert.IsTrue(eventType.IsProperty(prop));
	        }

	        Assert.AreEqual(nestedClass, eventType.GetPropertyType("l1"));
	        foreach (var prop in Collections.List("l1.lvl1", "l1.l2.lvl2", "l1.l2.l3.lvl3")) {
	            Assert.AreEqual(typeof(int?), TypeHelper.GetBoxedType(eventType.GetPropertyType(prop)));
	        }

	        var lvl1Fragment = eventType.GetFragmentType("l1");
	        Assert.IsFalse(lvl1Fragment.IsIndexed);
	        Assert.AreEqual(send == FBEAN, lvl1Fragment.IsNative);
	        Assert.AreEqual(fragmentTypeName, lvl1Fragment.FragmentType.Name);

	        var lvl2Fragment = eventType.GetFragmentType("l1.l2");
	        Assert.IsFalse(lvl2Fragment.IsIndexed);
	        Assert.AreEqual(send == FBEAN, lvl2Fragment.IsNative);

	        Assert.AreEqual(new EventPropertyDescriptor("l1", nestedClass, null, false, false, false, false, true), eventType.GetPropertyDescriptor("l1"));
	    }

	    private void RunAssertionTypeInvalidProp(string typeName) {
	        var eventType = _epService.EPAdministrator.Configuration.GetEventType(typeName);

	        foreach (var prop in Collections.List("l2", "l1.l3", "l1.lvl1.lvl1", "l1.l2.l4", "l1.l2.xx", "l1.l2.l3.lvl5")) {
	            Assert.AreEqual(false, eventType.IsProperty(prop));
	            Assert.AreEqual(null, eventType.GetPropertyType(prop));
	            Assert.IsNull(eventType.GetPropertyDescriptor(prop));
	        }
	    }

        public delegate void FunctionSendEvent4Int(EPServiceProvider epService, int lvl1, int lvl2, int lvl3, int lvl4);

        private class InfraNestedSimplePropTop
        {
            public InfraNestedSimplePropTop(InfraNestedSimplePropLvl1 l1)
            {
                L1 = l1;
            }

            [PropertyName("l1")]
            public InfraNestedSimplePropLvl1 L1 { get; private set; }
        }

        private class InfraNestedSimplePropLvl1
        {
            public InfraNestedSimplePropLvl1(InfraNestedSimplePropLvl2 l2, int lvl1)
            {
                L2 = l2;
                Lvl1 = lvl1;
            }

            [PropertyName("l2")]
            public InfraNestedSimplePropLvl2 L2 { get; private set; }

            [PropertyName("lvl1")]
            public int Lvl1 { get; private set; }
        }

        private class InfraNestedSimplePropLvl2
        {
            public InfraNestedSimplePropLvl2(InfraNestedSimplePropLvl3 l3, int lvl2)
            {
                L3 = l3;
                Lvl2 = lvl2;
            }

            [PropertyName("l3")]
            public InfraNestedSimplePropLvl3 L3 { get; private set; }

            [PropertyName("lvl2")]
            public int Lvl2 { get; private set; }
        }

        private class InfraNestedSimplePropLvl3
        {
            public InfraNestedSimplePropLvl3(InfraNestedSimplePropLvl4 l4, int lvl3)
            {
                L4 = l4;
                Lvl3 = lvl3;
            }

            [PropertyName("l4")]
            public InfraNestedSimplePropLvl4 L4 { get; private set; }

            [PropertyName("lvl3")]
            public int Lvl3 { get; private set; }
        }

        private class InfraNestedSimplePropLvl4
        {
            public InfraNestedSimplePropLvl4(int lvl4)
            {
                Lvl4 = lvl4;
            }

            [PropertyName("lvl4")]
            public int Lvl4 { get; private set; }
        }
	}
} // end of namespace
