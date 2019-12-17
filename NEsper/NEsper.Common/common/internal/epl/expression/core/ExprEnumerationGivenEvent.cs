///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

using com.espertech.esper.common.client;

namespace com.espertech.esper.common.@internal.epl.expression.core
{
    /// <summary>
    ///     Interface for evaluating of an event re. enumeration.
    /// </summary>
    public interface ExprEnumerationGivenEvent
    {
        ICollection<EventBean> EvaluateEventGetROCollectionEvents(
            EventBean @event,
            ExprEvaluatorContext context);

        ICollection<object> EvaluateEventGetROCollectionScalar(
            EventBean @event,
            ExprEvaluatorContext context);

        EventBean EvaluateEventGetEventBean(
            EventBean @event,
            ExprEvaluatorContext context);
    }

    public class ProxyExprEnumerationGivenEvent : ExprEnumerationGivenEvent
    {
        public delegate ICollection<EventBean> EvaluateEventGetROCollectionEventsFunc(
            EventBean @event,
            ExprEvaluatorContext context);

        public delegate ICollection<object> EvaluateEventGetROCollectionScalarFunc(
            EventBean @event,
            ExprEvaluatorContext context);

        public delegate EventBean EvaluateEventGetEventBeanFunc(
            EventBean @event,
            ExprEvaluatorContext context);

        public EvaluateEventGetROCollectionEventsFunc ProcEvaluateEventGetROCollectionEvents;
        public EvaluateEventGetROCollectionScalarFunc ProcEvaluateEventGetRoCollectionScalar;
        public EvaluateEventGetEventBeanFunc ProcEvaluateEventGetEventBean;

        public ProxyExprEnumerationGivenEvent()
        {
        }

        public ProxyExprEnumerationGivenEvent(
            EvaluateEventGetROCollectionEventsFunc procEvaluateEventGetROCollectionEvents,
            EvaluateEventGetROCollectionScalarFunc procEvaluateEventGetROCollectionScalar,
            EvaluateEventGetEventBeanFunc procEvaluateEventGetEventBean)
        {
            ProcEvaluateEventGetROCollectionEvents = procEvaluateEventGetROCollectionEvents;
            ProcEvaluateEventGetRoCollectionScalar = procEvaluateEventGetROCollectionScalar;
            ProcEvaluateEventGetEventBean = procEvaluateEventGetEventBean;
        }

        public ICollection<EventBean> EvaluateEventGetROCollectionEvents(
            EventBean @event,
            ExprEvaluatorContext context)
        {
            return ProcEvaluateEventGetROCollectionEvents(@event, context);
        }

        public ICollection<object> EvaluateEventGetROCollectionScalar(
            EventBean @event,
            ExprEvaluatorContext context)
        {
            return ProcEvaluateEventGetRoCollectionScalar(@event, context);
        }

        public EventBean EvaluateEventGetEventBean(
            EventBean @event,
            ExprEvaluatorContext context)
        {
            return ProcEvaluateEventGetEventBean(@event, context);
        }
    }
} // end of namespace