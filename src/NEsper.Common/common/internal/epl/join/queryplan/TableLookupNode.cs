///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

using com.espertech.esper.common.client;
using com.espertech.esper.common.@internal.context.util;
using com.espertech.esper.common.@internal.epl.index.@base;
using com.espertech.esper.common.@internal.epl.join.exec.@base;
using com.espertech.esper.common.@internal.epl.join.strategy;
using com.espertech.esper.common.@internal.epl.virtualdw;
using com.espertech.esper.common.@internal.util;
using com.espertech.esper.common.@internal.view.core;
using com.espertech.esper.compat.threading.locks;

namespace com.espertech.esper.common.@internal.epl.join.queryplan
{
    /// <summary>
    ///     Specifies exection of a table lookup using the supplied plan for performing the lookup.
    /// </summary>
    public class TableLookupNode : QueryPlanNode
    {
        /// <summary>
        ///     Ctor.
        /// </summary>
        /// <param name="tableLookupPlan">plan for performing lookup</param>
        public TableLookupNode(TableLookupPlan tableLookupPlan)
        {
            TableLookupPlan = tableLookupPlan;
        }

        public TableLookupPlan TableLookupPlan { get; }

        /// <summary>
        ///     Returns lookup plan.
        /// </summary>
        /// <value>lookup plan</value>
        protected TableLookupPlan LookupStrategySpec => TableLookupPlan;

        public override ExecNode MakeExec(
            AgentInstanceContext agentInstanceContext,
            IDictionary<TableLookupIndexReqKey, EventTable>[] indexesPerStream,
            EventType[] streamTypes,
            Viewable[] streamViews,
            VirtualDWView[] viewExternal,
            ILockable[] tableSecondaryIndexLocks)
        {
            var lookupStrategy = TableLookupPlan.MakeStrategy(
                agentInstanceContext,
                indexesPerStream,
                streamTypes,
                viewExternal);
            var indexedStream = TableLookupPlan.IndexedStream;
            if (tableSecondaryIndexLocks[indexedStream] != null) {
                return new TableLookupExecNodeTableLocking(
                    indexedStream,
                    lookupStrategy,
                    tableSecondaryIndexLocks[indexedStream]);
            }

            return new TableLookupExecNode(indexedStream, lookupStrategy);
        }

        public void Print(IndentWriter writer)
        {
            writer.WriteLine(
                "TableLookupNode " +
                " tableLookupPlan=" +
                TableLookupPlan);
        }
    }
} // end of namespace