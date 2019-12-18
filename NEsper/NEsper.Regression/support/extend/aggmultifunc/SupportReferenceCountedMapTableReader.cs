///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2015 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

using com.espertech.esper.common.client;
using com.espertech.esper.common.client.hook.aggmultifunc;
using com.espertech.esper.common.@internal.epl.agg.core;
using com.espertech.esper.common.@internal.epl.expression.core;
using com.espertech.esper.common.@internal.@event.render;
using com.espertech.esper.compat.collections;

namespace com.espertech.esper.regressionlib.support.extend.aggmultifunc
{
    public class SupportReferenceCountedMapTableReader : AggregationMultiFunctionTableReader
    {
        private readonly SupportReferenceCountedMapTableReaderFactory factory;

        public SupportReferenceCountedMapTableReader(SupportReferenceCountedMapTableReaderFactory factory)
        {
            this.factory = factory;
        }

        public object GetValue(
            int aggColNum,
            AggregationRow row,
            EventBean[] eventsPerStream,
            bool isNewData,
            ExprEvaluatorContext exprEvaluatorContext)
        {
            var state = (SupportReferenceCountedMapState) row.GetAccessState(aggColNum);
            var lookupKey = factory.Eval.Evaluate(eventsPerStream, isNewData, exprEvaluatorContext);
            if (state.CountPerReference.TryGetValue(lookupKey, out var referenceCount)) {
                return referenceCount;
            }

            return null;
        }

        public ICollection<EventBean> GetValueCollectionEvents(
            int aggColNum,
            AggregationRow row,
            EventBean[] eventsPerStream,
            bool isNewData,
            ExprEvaluatorContext exprEvaluatorContext)
        {
            return null;
        }

        public ICollection<object> GetValueCollectionScalar(
            int aggColNum,
            AggregationRow row,
            EventBean[] eventsPerStream,
            bool isNewData,
            ExprEvaluatorContext exprEvaluatorContext)
        {
            return null;
        }

        public EventBean GetValueEventBean(
            int aggColNum,
            AggregationRow row,
            EventBean[] eventsPerStream,
            bool isNewData,
            ExprEvaluatorContext exprEvaluatorContext)
        {
            return null;
        }
    }
} // end of namespace