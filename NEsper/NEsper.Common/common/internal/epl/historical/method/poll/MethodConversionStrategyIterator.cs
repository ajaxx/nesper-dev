///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;

using com.espertech.esper.common.client;
using com.espertech.esper.common.@internal.context.util;
using com.espertech.esper.compat.collections;

namespace com.espertech.esper.common.@internal.epl.historical.method.poll
{
    public abstract class MethodConversionStrategyIterator : MethodConversionStrategyBase
    {
        protected abstract EventBean GetEventBean(
            object value,
            AgentInstanceContext agentInstanceContext);

        public override IList<EventBean> Convert(
            object invocationResult,
            MethodTargetStrategy origin,
            AgentInstanceContext agentInstanceContext)
        {
            var enumerator = (IEnumerator) invocationResult;
            if (enumerator == null || !enumerator.MoveNext()) {
                return Collections.GetEmptyList<EventBean>();
            }

            var rowResult = new List<EventBean>();
            do {
                object value = enumerator.Current;
                if (CheckNonNullArrayValue(value, origin)) {
                    var @event = GetEventBean(value, agentInstanceContext);
                    rowResult.Add(@event);
                }
            } while (enumerator.MoveNext());

            return rowResult;
        }
    }
} // end of namespace