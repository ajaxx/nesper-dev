///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using com.espertech.esper.common.client;
using com.espertech.esper.common.@internal.epl.expression.core;

namespace com.espertech.esper.common.@internal.epl.expression.etc
{
    public class ExprEvalStreamNumEnumSingleEval : ExprEvaluator
    {
        private readonly ExprEnumerationEval _enumeration;

        public ExprEvalStreamNumEnumSingleEval(ExprEnumerationEval enumeration)
        {
            _enumeration = enumeration;
        }

        public object Evaluate(
            EventBean[] eventsPerStream,
            bool isNewData,
            ExprEvaluatorContext context)
        {
            return _enumeration.EvaluateGetEventBean(eventsPerStream, isNewData, context);
        }
    }
} // end of namespace