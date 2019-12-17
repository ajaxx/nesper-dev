///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using com.espertech.esper.common.@internal.context.util;
using com.espertech.esper.common.@internal.epl.lookupplansubord;
using com.espertech.esper.common.@internal.epl.namedwindow.core;
using com.espertech.esper.common.@internal.epl.ontrigger;
using com.espertech.esper.common.@internal.epl.table.core;

namespace com.espertech.esper.common.@internal.context.aifactory.ontrigger.ontrigger
{
    /// <summary>
    ///     View for the on-delete statement that handles removing events from a named window.
    /// </summary>
    public interface InfraOnExprFactory
    {
        InfraOnExprBaseViewResult MakeNamedWindow(
            SubordWMatchExprLookupStrategy lookupStrategy,
            NamedWindowRootViewInstance namedWindowRootViewInstance,
            AgentInstanceContext agentInstanceContext);

        InfraOnExprBaseViewResult MakeTable(
            SubordWMatchExprLookupStrategy lookupStrategy,
            TableInstance tableInstance,
            AgentInstanceContext agentInstanceContext);
    }
} // end of namespace