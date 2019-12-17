///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using com.espertech.esper.common.@internal.context.util;
using com.espertech.esper.common.@internal.filterspec;
using com.espertech.esper.common.@internal.filtersvc;
using com.espertech.esper.compat.threading.locks;
using com.espertech.esper.container;

namespace com.espertech.esper.common.@internal.context.activator
{
    public class ViewableActivatorFilterStopCallback : AgentInstanceStopCallback
    {
        private readonly FilterSpecActivatable _filterSpecActivatable;
        private readonly ILockable _lock;
        private FilterHandle _filterHandle;

        public ViewableActivatorFilterStopCallback(
            IContainer container,
            FilterHandle filterHandle,
            FilterSpecActivatable filterSpecActivatable)
        {
            _lock = container.LockManager().CreateLock(GetType());
            _filterHandle = filterHandle;
            _filterSpecActivatable = filterSpecActivatable;
        }

        public void Stop(AgentInstanceStopServices services)
        {
            using (_lock.Acquire()) {
                if (_filterHandle != null) {
                    FilterValueSetParam[][] addendum = null;
                    var agentInstanceContext = services.AgentInstanceContext;
                    if (agentInstanceContext.AgentInstanceFilterProxy != null) {
                        addendum = agentInstanceContext.AgentInstanceFilterProxy.GetAddendumFilters(
                            _filterSpecActivatable,
                            agentInstanceContext);
                    }

                    var filterValues = _filterSpecActivatable.GetValueSet(
                        null,
                        addendum,
                        agentInstanceContext,
                        agentInstanceContext.StatementContextFilterEvalEnv);
                    services.AgentInstanceContext.FilterService.Remove(
                        _filterHandle,
                        _filterSpecActivatable.FilterForEventType,
                        filterValues);
                }

                _filterHandle = null;
            }
        }
    }
} // end of namespace