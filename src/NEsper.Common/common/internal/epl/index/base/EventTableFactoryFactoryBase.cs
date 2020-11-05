///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using com.espertech.esper.common.client;

namespace com.espertech.esper.common.@internal.epl.index.@base
{
    public abstract class EventTableFactoryFactoryBase : EventTableFactoryFactory
    {
        internal readonly int indexedStreamNum;
        internal readonly bool isFireAndForget;
        internal readonly int? subqueryNum;

        public EventTableFactoryFactoryBase(
            int indexedStreamNum,
            int? subqueryNum,
            bool isFireAndForget)
        {
            this.indexedStreamNum = indexedStreamNum;
            this.subqueryNum = subqueryNum;
            this.isFireAndForget = isFireAndForget;
        }

        public abstract EventTableFactory Create(
            EventType eventType,
            EventTableFactoryFactoryContext eventTableFactoryContext);
    }
} // end of namespace