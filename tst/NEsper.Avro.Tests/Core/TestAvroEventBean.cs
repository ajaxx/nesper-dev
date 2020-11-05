///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2015 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System.Reflection;

using Avro.Generic;

using com.espertech.esper.common.client;
using com.espertech.esper.compat.logging;

using NEsper.Avro.Extensions;
using NEsper.Avro.Util.Support;

using NUnit.Framework;

namespace NEsper.Avro.Core
{
    [TestFixture]
	public class TestAvroEventBean  {
        [Test]
	    public void TestGet()
	    {
		    var schema = SchemaBuilder
			    .Record("typename", TypeBuilder.RequiredInt("myInt"));

	        var eventType = SupportAvroUtil.MakeAvroSupportEventType(schema);

	        var record = new GenericRecord(schema);
	        record.Put("myInt", 99);
	        var eventBean = new AvroGenericDataEventBean(record, eventType);

	        Assert.AreEqual(eventType, eventBean.EventType);
	        Assert.AreEqual(record, eventBean.Underlying);
	        Assert.AreEqual(99, eventBean.Get("myInt"));

	        // test wrong property name
	        try {
	            eventBean.Get("dummy");
	            Assert.Fail();
	        } catch (PropertyAccessException ex) {
	            // Expected
	            Log.Debug(".testGetter Expected exception, msg=" + ex.Message);
	        }
	    }

	    private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
	}
} // end of namespace
