///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

namespace com.espertech.esper.common.@internal.epl.agg.core
{
    public interface AggregationResultFutureAssignable
    {
        void Assign(AggregationResultFuture future);
    }

    public class ProxyAggregationResultFutureAssignable : AggregationResultFutureAssignable
    {
        public delegate void AssignFunc(AggregationResultFuture future);

        public AssignFunc ProcAssign { get; set; }

        public ProxyAggregationResultFutureAssignable()
        {
        }

        public ProxyAggregationResultFutureAssignable(AssignFunc procAssign)
        {
            ProcAssign = procAssign;
        }

        public void Assign(AggregationResultFuture future)
        {
            ProcAssign(future);
        }
    }
} // end of namespace