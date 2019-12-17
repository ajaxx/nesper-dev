///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using com.espertech.esper.common.client;

namespace com.espertech.esper.common.@internal.epl.pattern.core
{
    /// <summary>
    ///     Callback interface for anything that requires to be informed of matching events which would be stored
    ///     in the MatchedEventMap structure passed to the implementation.
    /// </summary>
    public interface PatternMatchCallback
    {
        /// <summary>
        ///     Indicate matching events.
        /// </summary>
        /// <param name="matchEvent">contains a map of event tags and event objects</param>
        /// <param name="optionalTriggeringEvent">in case the pattern fired as a result of an event arriving, provides the event</param>
        void MatchFound(
            IDictionary<string, object> matchEvent,
            EventBean optionalTriggeringEvent);
    }

    public class ProxyPatternMatchCallback : PatternMatchCallback
    {
        public Action<IDictionary<string, object>, EventBean> ProcMatchFound;

        public void MatchFound(
            IDictionary<string, object> matchEvent,
            EventBean optionalTriggeringEvent)
        {
            ProcMatchFound?.Invoke(matchEvent, optionalTriggeringEvent);
        }
    }
} // end of namespace