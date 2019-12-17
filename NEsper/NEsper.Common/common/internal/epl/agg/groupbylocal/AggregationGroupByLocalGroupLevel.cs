///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

using com.espertech.esper.common.@internal.epl.agg.core;
using com.espertech.esper.common.@internal.epl.expression.core;

namespace com.espertech.esper.common.@internal.epl.agg.groupbylocal
{
    public class AggregationGroupByLocalGroupLevel
    {
        public AggregationGroupByLocalGroupLevel(
            ExprNode[] partitionExpr,
            IList<AggregationServiceAggExpressionDesc> expressions)
        {
            PartitionExpr = partitionExpr;
            Expressions = expressions;
        }

        public ExprNode[] PartitionExpr { get; }

        public IList<AggregationServiceAggExpressionDesc> Expressions { get; }
    }
} // end of namespace