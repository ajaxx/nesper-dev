///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2017 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using com.espertech.esper.client;
using com.espertech.esper.compat.collections;
using com.espertech.esper.epl.expression.core;
using com.espertech.esper.epl.rettype;

namespace com.espertech.esper.epl.expression.dot.inner
{
    public class InnerEvaluatorArrPrimitiveToColl : ExprDotEvalRootChildInnerEval
    {
        private readonly ExprEvaluator _rootEvaluator;
    
        public InnerEvaluatorArrPrimitiveToColl(ExprEvaluator rootEvaluator)
        {
            _rootEvaluator = rootEvaluator;
        }

        public object Evaluate(EvaluateParams evaluateParams)
        {
            var array = _rootEvaluator.Evaluate(evaluateParams) as Array;
            if (array == null) {
                return null;
            }

            var len = array.Length;
            if (len == 0) {
                return Collections.GetEmptyList<object>();
            }
            if (len == 1)
            {
                return Collections.SingletonList<object>(array.GetValue(0));
            }

            return array.Unwrap<object>();
        }
    
        public ICollection<EventBean> EvaluateGetROCollectionEvents(EvaluateParams evaluateParams) {
            return null;
        }
    
        public ICollection<object> EvaluateGetROCollectionScalar(EvaluateParams evaluateParams) {
            return null;
        }

        public EventType EventTypeCollection
        {
            get { return null; }
        }

        public Type ComponentTypeCollection
        {
            get { return null; }
        }

        public EventBean EvaluateGetEventBean(EvaluateParams evaluateParams) {
            return null;
        }

        public EventType EventTypeSingle
        {
            get { return null; }
        }

        public EPType TypeInfo
        {
            get { return EPTypeHelper.CollectionOfSingleValue(_rootEvaluator.ReturnType.GetElementType()); }
        }
    }
}
