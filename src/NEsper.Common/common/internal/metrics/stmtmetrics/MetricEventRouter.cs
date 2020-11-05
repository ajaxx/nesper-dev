///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using com.espertech.esper.common.client.metric;

namespace com.espertech.esper.common.@internal.metrics.stmtmetrics
{
    /// <summary>
    ///     Interface for routing metric events for processing.
    /// </summary>
    public interface MetricEventRouter
    {
        /// <summary>
        ///     Process metric event.
        /// </summary>
        /// <param name="metricEvent">metric event to process</param>
        void Route(MetricEvent metricEvent);
    }
} // end of namespace