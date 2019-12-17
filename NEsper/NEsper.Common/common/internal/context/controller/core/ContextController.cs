///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

using com.espertech.esper.common.client;
using com.espertech.esper.common.client.context;
using com.espertech.esper.common.@internal.collection;
using com.espertech.esper.common.@internal.context.mgr;

namespace com.espertech.esper.common.@internal.context.controller.core
{
    public interface ContextController
    {
        ContextControllerFactory Factory { get; }

        ContextManagerRealization Realization { get; }

        void Activate(
            IntSeqKey path,
            object[] parentPartitionKeys,
            EventBean optionalTriggeringEvent,
            IDictionary<string, object> optionalTriggeringPattern);

        void Deactivate(
            IntSeqKey path,
            bool terminateChildContexts);

        void VisitSelectedPartitions(
            IntSeqKey path,
            ContextPartitionSelector selector,
            ContextPartitionVisitor visitor,
            ContextPartitionSelector[] selectorPerLevel);

        void Destroy();
    }
} // end of namespace