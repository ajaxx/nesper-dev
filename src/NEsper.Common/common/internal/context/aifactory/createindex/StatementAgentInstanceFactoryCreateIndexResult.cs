///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using com.espertech.esper.common.@internal.context.aifactory.core;
using com.espertech.esper.common.@internal.context.util;
using com.espertech.esper.common.@internal.epl.subselect;
using com.espertech.esper.common.@internal.epl.table.strategy;
using com.espertech.esper.common.@internal.view.core;
using com.espertech.esper.compat.collections;

namespace com.espertech.esper.common.@internal.context.aifactory.createindex
{
    public class StatementAgentInstanceFactoryCreateIndexResult : StatementAgentInstanceFactoryResult
    {
        public StatementAgentInstanceFactoryCreateIndexResult(
            Viewable finalView,
            AgentInstanceMgmtCallback stopCallback,
            AgentInstanceContext agentInstanceContext)
            : base(
                finalView,
                stopCallback,
                agentInstanceContext,
                null,
                EmptyDictionary<int, SubSelectFactoryResult>.Instance,
                null,
                null,
                null,
                EmptyDictionary<int, ExprTableEvalStrategy>.Instance,
                EmptyList<StatementAgentInstancePreload>.Instance,
                null)
        {
        }
    }
} // end of namespace