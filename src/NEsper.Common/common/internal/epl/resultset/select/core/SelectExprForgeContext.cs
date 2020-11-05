///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using com.espertech.esper.common.client;
using com.espertech.esper.common.@internal.epl.expression.core;
using com.espertech.esper.common.@internal.@event.avro;
using com.espertech.esper.common.@internal.@event.core;

namespace com.espertech.esper.common.@internal.epl.resultset.select.core
{
    public class SelectExprForgeContext
    {
        public SelectExprForgeContext(
            ExprForge[] exprForges,
            string[] columnNames,
            EventBeanTypedEventFactory eventBeanTypedEventFactory,
            EventType[] eventTypes,
            EventTypeAvroHandler eventTypeAvroHandler)
        {
            ExprForges = exprForges;
            ColumnNames = columnNames;
            EventBeanTypedEventFactory = eventBeanTypedEventFactory;
            EventTypes = eventTypes;
            EventTypeAvroHandler = eventTypeAvroHandler;
        }

        public ExprForge[] ExprForges { get; }

        public string[] ColumnNames { get; }

        public EventBeanTypedEventFactory EventBeanTypedEventFactory { get; }

        public int NumStreams => EventTypes.Length;

        public EventType[] EventTypes { get; }

        public EventTypeAvroHandler EventTypeAvroHandler { get; }
    }
} // end of namespace