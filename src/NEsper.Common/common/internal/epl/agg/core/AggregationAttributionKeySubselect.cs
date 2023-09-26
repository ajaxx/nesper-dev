namespace com.espertech.esper.common.@internal.epl.agg.core
{
///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2015 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////
/*
	 ***************************************************************************************
	 *  Copyright (C) 2006 EsperTech, Inc. All rights reserved.                            *
	 *  http://www.espertech.com/esper                                                     *
	 *  http://www.espertech.com                                                           *
	 *  ---------------------------------------------------------------------------------- *
	 *  The software in this package is published under the terms of the GPL license       *
	 *  a copy of which has been included with this distribution in the license.txt file.  *
	 ***************************************************************************************
	 */
    public class AggregationAttributionKeySubselect : AggregationAttributionKey
    {
        private readonly int subqueryNumber;

        public AggregationAttributionKeySubselect(int subqueryNumber)
        {
            this.subqueryNumber = subqueryNumber;
        }

        public T Accept<T>(AggregationAttributionKeyVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public int SubqueryNumber => subqueryNumber;
    }
} // end of namespace