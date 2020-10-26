///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2015 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

using com.espertech.esper.common.client;
using com.espertech.esper.compat;
using com.espertech.esper.compat.collections;
using com.espertech.esper.runtime.client;

namespace com.espertech.esper.regressionlib.support.util
{
    public class SupportListenerTimerHRes : UpdateListener
    {
        public IList<Pair<long, EventBean[]>> NewEvents { get; } =
            new List<Pair<long, EventBean[]>>().AsSyncList();

        public void Update(
            object sender,
            UpdateEventArgs eventArgs)
        {
            var time = PerformanceObserver.NanoTime;
            NewEvents.Add(new Pair<long, EventBean[]>(time, eventArgs.NewEvents));
        }
    }
} // end of namespace