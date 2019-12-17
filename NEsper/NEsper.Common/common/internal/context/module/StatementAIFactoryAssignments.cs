///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using com.espertech.esper.common.@internal.epl.agg.core;
using com.espertech.esper.common.@internal.epl.expression.prior;
using com.espertech.esper.common.@internal.epl.lookup;
using com.espertech.esper.common.@internal.epl.rowrecog.core;
using com.espertech.esper.common.@internal.epl.table.strategy;
using com.espertech.esper.common.@internal.view.previous;

namespace com.espertech.esper.common.@internal.context.module
{
    public interface StatementAIFactoryAssignments
    {
        AggregationService AggregationResultFuture { get; }

        PriorEvalStrategy[] PriorStrategies { get; }

        PreviousGetterStrategy[] PreviousStrategies { get; }

        RowRecogPreviousStrategy RowRecogPreviousStrategy { get; }

        SubordTableLookupStrategy GetSubqueryLookup(int subqueryNumber);

        PriorEvalStrategy GetSubqueryPrior(int subqueryNumber);

        PreviousGetterStrategy GetSubqueryPrevious(int subqueryNumber);

        AggregationService GetSubqueryAggregation(int subqueryNumber);

        ExprTableEvalStrategy GetTableAccess(int tableAccessNumber);
    }
} // end of namespace