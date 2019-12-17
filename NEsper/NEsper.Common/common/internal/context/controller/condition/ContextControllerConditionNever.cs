///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using com.espertech.esper.common.client;

namespace com.espertech.esper.common.@internal.context.controller.condition
{
    public class ContextControllerConditionNever : ContextControllerConditionNonHA
    {
        public static readonly ContextControllerConditionNever INSTANCE = new ContextControllerConditionNever();

        private ContextControllerConditionNever()
        {
        }

        public ContextConditionDescriptor Descriptor => ContextConditionDescriptorNever.INSTANCE;

        public bool Activate(
            EventBean optionalTriggeringEvent,
            ContextControllerEndConditionMatchEventProvider endConditionMatchEventProvider)
        {
            return false;
        }

        public void Deactivate()
        {
        }

        public bool IsImmediate => false;

        public bool IsRunning => false;

        public long? ExpectedEndTime => null;
    }
} // end of namespace