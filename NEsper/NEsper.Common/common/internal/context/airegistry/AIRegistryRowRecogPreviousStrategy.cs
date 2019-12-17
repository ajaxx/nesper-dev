///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using com.espertech.esper.common.@internal.epl.rowrecog.core;

namespace com.espertech.esper.common.@internal.context.airegistry
{
    public interface AIRegistryRowRecogPreviousStrategy : RowRecogPreviousStrategy
    {
        int InstanceCount { get; }

        void AssignService(
            int serviceId,
            RowRecogPreviousStrategy strategy);

        void DeassignService(int serviceId);
    }
} // end of namespace