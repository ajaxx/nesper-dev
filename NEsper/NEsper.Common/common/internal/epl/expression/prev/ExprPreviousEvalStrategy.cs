///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

using com.espertech.esper.common.client;
using com.espertech.esper.common.@internal.epl.expression.core;

namespace com.espertech.esper.common.@internal.epl.expression.prev
{
    public interface ExprPreviousEvalStrategy
    {
        object Evaluate(
            EventBean[] eventsPerStream,
            ExprEvaluatorContext exprEvaluatorContext);

        ICollection<EventBean> EvaluateGetCollEvents(
            EventBean[] eventsPerStream,
            ExprEvaluatorContext context);

        ICollection<object> EvaluateGetCollScalar(
            EventBean[] eventsPerStream,
            ExprEvaluatorContext context);

        EventBean EvaluateGetEventBean(
            EventBean[] eventsPerStream,
            ExprEvaluatorContext context);
    }
} // end of namespace