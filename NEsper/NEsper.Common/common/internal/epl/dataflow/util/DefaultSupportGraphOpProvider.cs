///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using com.espertech.esper.common.client.dataflow.core;
using com.espertech.esper.compat;
using com.espertech.esper.compat.collections;

namespace com.espertech.esper.common.@internal.epl.dataflow.util
{
    public class DefaultSupportGraphOpProvider : EPDataFlowOperatorProvider
    {
        private readonly object[] ops;

        public DefaultSupportGraphOpProvider(object op)
        {
            this.ops = new object[] {op};
        }

        public DefaultSupportGraphOpProvider(params object[] ops)
        {
            this.ops = ops;
        }

        public object Provide(EPDataFlowOperatorProviderContext context)
        {
            foreach (object op in ops) {
                if (context.OperatorName.Equals(op.GetType().Name)) {
                    return op;
                }
            }

            return null;
        }
    }
} // end of namespace