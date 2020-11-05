///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

namespace com.espertech.esper.common.@internal.epl.dataflow.realize
{
    public class ObjectBindingPair
    {
        public ObjectBindingPair(
            object target,
            string operatorPrettyPrint,
            LogicalChannelBinding binding)
        {
            Target = target;
            OperatorPrettyPrint = operatorPrettyPrint;
            Binding = binding;
        }

        public string OperatorPrettyPrint { get; }

        public object Target { get; }

        public LogicalChannelBinding Binding { get; }
    }
} // end of namespace