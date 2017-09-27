///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2017 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using com.espertech.esper.client;
using com.espertech.esper.compat;
using com.espertech.esper.compat.collections;
using com.espertech.esper.compat.logging;
using com.espertech.esper.core.context.util;
using com.espertech.esper.epl.join.plan;
using com.espertech.esper.epl.lookup;
using com.espertech.esper.util;

namespace com.espertech.esper.epl.join.table
{
    public class EventTableUtil {
        /// <summary>
        /// Build an index/table instance using the event properties for the event type.
        /// </summary>
        /// <param name="indexedStreamNum">- number of stream indexed</param>
        /// <param name="eventType">- type of event to expect</param>
        /// <param name="optionalIndexName">index name</param>
        /// <param name="agentInstanceContext">context</param>
        /// <param name="item">plan item</param>
        /// <param name="optionalSerde">serde if any</param>
        /// <param name="isFireAndForget">indicates fire-and-forget</param>
        /// <param name="unique">indicates unique</param>
        /// <param name="coerceOnAddOnly">indicator whether to coerce on value-add</param>
        /// <returns>table build</returns>
        public static EventTable BuildIndex(AgentInstanceContext agentInstanceContext, int indexedStreamNum, QueryPlanIndexItem item, EventType eventType, bool coerceOnAddOnly, bool unique, string optionalIndexName, Object optionalSerde, bool isFireAndForget) {
            var indexProps = item.IndexProps;
            var indexCoercionTypes = Normalize(item.OptIndexCoercionTypes);
            var rangeProps = item.RangeProps;
            var rangeCoercionTypes = Normalize(item.OptRangeCoercionTypes);
            var ident = new EventTableFactoryTableIdentAgentInstance(agentInstanceContext);
            EventTableIndexService eventTableIndexService = agentInstanceContext.StatementContext.EventTableIndexService;
    
            EventTable table;
            if (rangeProps == null || rangeProps.Count == 0) {
                if (indexProps == null || indexProps.Count == 0)
                {
                    EventTableFactory factory = eventTableIndexService.CreateUnindexed(indexedStreamNum, optionalSerde, isFireAndForget);
                    table = factory.MakeEventTables(ident)[0];
                } else {
                    // single index key
                    if (indexProps.Count == 1)
                    {
                        if (indexCoercionTypes == null || indexCoercionTypes.Count == 0) {
                            EventTableFactory factory = eventTableIndexService.CreateSingle(indexedStreamNum, eventType, indexProps[0], unique, optionalIndexName, optionalSerde, isFireAndForget);
                            table = factory.MakeEventTables(ident)[0];
                        } else {
                            if (coerceOnAddOnly) {
                                EventTableFactory factory = eventTableIndexService.CreateSingleCoerceAdd(indexedStreamNum, eventType, indexProps[0], indexCoercionTypes[0], optionalSerde, isFireAndForget);
                                table = factory.MakeEventTables(ident)[0];
                            } else {
                                EventTableFactory factory = eventTableIndexService.CreateSingleCoerceAll(indexedStreamNum, eventType, indexProps[0], indexCoercionTypes[0], optionalSerde, isFireAndForget);
                                table = factory.MakeEventTables(ident)[0];
                            }
                        }
                    } else {
                        // Multiple index keys
                        if (indexCoercionTypes == null || indexCoercionTypes.Count == 0) {
                            EventTableFactory factory = eventTableIndexService.CreateMultiKey(indexedStreamNum, eventType, indexProps, unique, optionalIndexName, optionalSerde, isFireAndForget);
                            table = factory.MakeEventTables(ident)[0];
                        } else {
                            if (coerceOnAddOnly) {
                                EventTableFactory factory = eventTableIndexService.CreateMultiKeyCoerceAdd(indexedStreamNum, eventType, indexProps, indexCoercionTypes, isFireAndForget);
                                table = factory.MakeEventTables(ident)[0];
                            } else {
                                EventTableFactory factory = eventTableIndexService.CreateMultiKeyCoerceAll(indexedStreamNum, eventType, indexProps, indexCoercionTypes, isFireAndForget);
                                table = factory.MakeEventTables(ident)[0];
                            }
                        }
                    }
                }
            } else {
                if ((rangeProps.Count == 1) && (indexProps == null || indexProps.Count == 0))
                {
                    if (rangeCoercionTypes == null) {
                        EventTableFactory factory = eventTableIndexService.CreateSorted(indexedStreamNum, eventType, rangeProps[0], isFireAndForget);
                        return factory.MakeEventTables(ident)[0];
                    } else {
                        EventTableFactory factory = eventTableIndexService.CreateSortedCoerce(indexedStreamNum, eventType, rangeProps[0], rangeCoercionTypes[0], isFireAndForget);
                        return factory.MakeEventTables(ident)[0];
                    }
                } else {
                    EventTableFactory factory = eventTableIndexService.CreateComposite(indexedStreamNum, eventType, indexProps, indexCoercionTypes, rangeProps, rangeCoercionTypes, isFireAndForget);
                    return factory.MakeEventTables(ident)[0];
                }
            }
            return table;
        }
    
        private static IList<Type> Normalize(IList<Type> types) {
            if (types == null) {
                return null;
            }
            if (CollectionUtil.IsAllNullArray(types)) {
                return null;
            }
            return types;
        }
    }
} // end of namespace
