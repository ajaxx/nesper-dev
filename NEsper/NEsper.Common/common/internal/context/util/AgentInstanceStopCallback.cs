///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

namespace com.espertech.esper.common.@internal.context.util
{
    public interface AgentInstanceStopCallback
    {
        void Stop(AgentInstanceStopServices services);
    }

    public class AgentInstanceStopCallbackConstants
    {
        public static readonly AgentInstanceStopCallback INSTANCE_NO_ACTION = new ProxyAgentInstanceStopCallback() {
            ProcStop = _ => { }
        };
    }

    public class ProxyAgentInstanceStopCallback : AgentInstanceStopCallback
    {
        public Action<AgentInstanceStopServices> ProcStop;

        public ProxyAgentInstanceStopCallback()
        {
        }

        public ProxyAgentInstanceStopCallback(Action<AgentInstanceStopServices> procStop)
        {
            ProcStop = procStop;
        }

        public void Stop(AgentInstanceStopServices services)
        {
            ProcStop.Invoke(services);
        }
    }
} // end of namespace