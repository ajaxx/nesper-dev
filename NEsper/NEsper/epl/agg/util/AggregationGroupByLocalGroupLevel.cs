///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2015 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

using com.espertech.esper.epl.agg.service;
using com.espertech.esper.epl.expression.core;

namespace com.espertech.esper.epl.agg.util
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

        public ExprNode[] PartitionExpr { get; private set; }

        public IList<AggregationServiceAggExpressionDesc> Expressions { get; private set; }
    }
} // end of namespace