///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Reflection;

using com.espertech.esper.common.client;
using com.espertech.esper.common.client.collection;
using com.espertech.esper.common.@internal.compile.stage2;
using com.espertech.esper.common.@internal.compile.stage3;
using com.espertech.esper.common.@internal.epl.expression.core;
using com.espertech.esper.common.@internal.util;
using com.espertech.esper.compat;
using com.espertech.esper.compat.collections;
using com.espertech.esper.compat.function;

namespace com.espertech.esper.common.@internal.rettype
{
    /// <summary>
    ///     Carries return type information related to the return values returned by expressions.
    ///     <para>
    ///         Use factory methods to initialize return type information according to the return values
    ///         that your expression is going to provide.
    ///     </para>
    ///     <ol>
    ///         <li>
    ///             Use <see cref="EPTypeHelper.CollectionOfEvents(com.espertech.esper.common.client.EventType)" />
    ///             to indicate that the expression returns a collection of events.
    ///         </li>
    ///         <li>
    ///             Use <see cref="EPTypeHelper.SingleEvent(com.espertech.esper.common.client.EventType)" />
    ///             to indicate that the expression returns a single event.
    ///         </li>
    ///         <li>
    ///             Use <see cref="EPTypeHelper.CollectionOfSingleValue(Type)" />
    ///             to indicate that the expression returns a collection of single values.
    ///             A single value can be any object including null.
    ///         </li>
    ///         <li>
    ///             Use <see cref="EPTypeHelper.Array(Type)" />
    ///             to indicate that the expression returns an array of single values.
    ///             A single value can be any object including null.
    ///         </li>
    ///         <li>
    ///             Use <see cref="EPTypeHelper.SingleValue(Type)" />
    ///             to indicate that the expression returns a single value.
    ///             A single value can be any object including null.
    ///             Such expression results cannot be used as input to enumeration methods, for example.
    ///         </li>
    ///     </ol>
    /// </summary>
    public static class EPTypeHelper
    {
        public static EventType GetEventTypeSingleValued(this EPType type)
        {
            return (type as EventEPType)?.EventType;
        }

        public static EventType GetEventTypeMultiValued(this EPType type)
        {
            return (type as EventMultiValuedEPType)?.Component;
        }

        public static Type GetClassMultiValued(this EPType type)
        {
            return (type as ClassMultiValuedEPType)?.Component;
        }

        public static Type GetClassMultiValuedContainer(this EPType type)
        {
            return (type as ClassMultiValuedEPType)?.Container;
        }

        public static Type GetClassSingleValued(this EPType type)
        {
            return (type as ClassEPType)?.Clazz;
        }

        public static bool IsCarryEvent(this EPType epType)
        {
            return epType is EventMultiValuedEPType || epType is EventEPType;
        }

        public static EventType GetEventType(this EPType epType)
        {
            if (epType is EventMultiValuedEPType) {
                return ((EventMultiValuedEPType) epType).Component;
            }

            return (epType as EventEPType)?.EventType;
        }

        /// <summary>
        ///     Indicate that the expression return type is an array of a given component type.
        /// </summary>
        /// <param name="arrayComponentType">array component type</param>
        /// <returns>array of single value expression result type</returns>
        public static EPType Array(Type arrayComponentType)
        {
            if (arrayComponentType == null) {
                throw new ArgumentException("Invalid null array component type");
            }

            var arrayType = TypeHelper.GetArrayType(arrayComponentType);
            return new ClassMultiValuedEPType(
                arrayType,
                arrayComponentType);
        }

        /// <summary>
        ///     Indicate that the expression return type is a single (non-enumerable) value of the given type.
        ///     The expression can still return an array or collection or events however
        ///     since the runtime would not know the type of such objects and may not use runtime reflection
        ///     it may not allow certain operations on expression results.
        /// </summary>
        /// <param name="singleValueType">
        ///     type of single value returned, or null to indicate that the expression always returns
        ///     null
        /// </param>
        /// <returns>single-value expression result type</returns>
        public static EPType SingleValue(Type singleValueType)
        {
            // null value allowed
            if (singleValueType != null && singleValueType.IsArray) {
                return new ClassMultiValuedEPType(
                    singleValueType,
                    singleValueType.GetElementType());
            }

            return new ClassEPType(singleValueType);
        }

        public static EPType NullValue()
        {
            return NullEPType.INSTANCE;
        }

        /// <summary>
        ///     Indicate that the expression return type is a collection of a given component type.
        /// </summary>
        /// <param name="collectionComponentType">collection component type</param>
        /// <param name="collectionType"></param>
        /// <returns>collection of single value expression result type</returns>
        public static EPType CollectionOfSingleValue(
            Type collectionComponentType,
            Type collectionType)
        {
            if (collectionComponentType == null) {
                throw new ArgumentException("Invalid null collection component type");
            }

            var containerType = typeof(FlexCollection);

#if false
            // Have not yet deduced the black magic of why and when we decide the type of
            // collection.  It's not entirely clear and is a work-in-progress.
            // TBD: fix the general determinism of this algorithm.
            
            if (collectionComponentType == typeof(object)) {
                containerType = typeof(ICollection<object>);
            } else if (collectionType == null) {
                containerType = typeof(ICollection<object>);
            } else if (collectionType == typeof(EventBean)) {
                // WTF
                containerType = typeof(ICollection<object>);
                //containerType = typeof(ICollection<EventBean>);
            } else if (collectionComponentType == typeof(object[])) {
                containerType = typeof(ICollection<object[]>);
            } else {
                containerType = typeof(ICollection<object>);
            }
#endif

            return new ClassMultiValuedEPType(
                containerType,
                collectionComponentType);
        }

        /// <summary>
        ///     Indicate that the expression return type is a collection of a given type of events.
        /// </summary>
        /// <param name="eventTypeOfCollectionEvents">the event type of the events that are part of the collection</param>
        /// <returns>collection of events expression result type</returns>
        public static EPType CollectionOfEvents(EventType eventTypeOfCollectionEvents)
        {
            if (eventTypeOfCollectionEvents == null) {
                throw new ArgumentException("Invalid null event type");
            }

            return new EventMultiValuedEPType(typeof(FlexCollection), eventTypeOfCollectionEvents);
            //return new EventMultiValuedEPType(typeof(ICollection<EventBean>), eventTypeOfCollectionEvents);
        }

        /// <summary>
        ///Indicate that the expression return type is an array of events of given type.
        /// </summary>
        /// <param name="eventTypeOfArrayEvents">the event type of the events that are part of the array</param>
        /// <returns>array of events expression result type</returns> 
        public static EPType ArrayOfEvents(EventType eventTypeOfArrayEvents)
        {
            if (eventTypeOfArrayEvents == null) {
                throw new ArgumentException("Invalid null event type");
            }

            return new EventMultiValuedEPType(typeof(EventBean[]), eventTypeOfArrayEvents);
        }

        /// <summary>
        ///     Indicate that the expression return type is single event of a given event type.
        /// </summary>
        /// <param name="eventTypeOfSingleEvent">the event type of the event returned</param>
        /// <returns>single-event expression result type</returns>
        public static EPType SingleEvent(EventType eventTypeOfSingleEvent)
        {
            if (eventTypeOfSingleEvent == null) {
                throw new ArgumentException("Invalid null event type");
            }

            return new EventEPType(eventTypeOfSingleEvent);
        }

        /// <summary>
        ///     Interrogate the provided method and determine whether it returns
        ///     single-value, array of single-value or collection of single-value and
        ///     their component type.
        /// </summary>
        /// <param name="method">the class methods</param>
        /// <returns>expression return type</returns>
        public static EPType FromMethod(MethodInfo method)
        {
            var returnType = method.ReturnType;
            if (returnType.IsArray) {
                var componentType = method.ReturnType.GetElementType();
                return Array(componentType);
            }

            if (returnType.IsGenericCollection()) {
                var componentType = TypeHelper.GetGenericReturnType(method, true);
                return CollectionOfSingleValue(componentType, returnType);
            }

            return SingleValue(method.ReturnType.GetBoxedType());
        }

        /// <summary>
        ///     Returns a nice text detailing the expression result type.
        /// </summary>
        /// <param name="epType">type</param>
        /// <returns>descriptive text</returns>
        public static string ToTypeDescriptive(this EPType epType)
        {
            if (epType is EventEPType) {
                var type = (EventEPType) epType;
                return "event type '" + type.EventType.Name + "'";
            }

            if (epType is EventMultiValuedEPType eventMultiValuedEpType) {
                if (eventMultiValuedEpType.Container == typeof(EventType[])) {
                    return "array of events of type '" + eventMultiValuedEpType.Component.Name + "'";
                }

                return "collection of events of type '" + eventMultiValuedEpType.Component.Name + "'";
            }

            if (epType is ClassMultiValuedEPType classMultiValuedEpType) {
                if (classMultiValuedEpType.Container.IsArray) {
                    return "array of " + classMultiValuedEpType.Component.CleanName();
                }

                return "collection of " + classMultiValuedEpType.Component.CleanName();
            }

            if (epType is ClassEPType classEpType) {
                return "class " + classEpType.Clazz.CleanName();
            }

            if (epType is NullEPType) {
                return "null type";
            }

            throw new ArgumentException("Unrecognized type " + epType);
        }

        public static Type GetNormalizedClass(this EPType theType)
        {
            if (theType is EventMultiValuedEPType eventMultiValuedEpType) {
                return TypeHelper.GetArrayType(eventMultiValuedEpType.Component.UnderlyingType);
            }

            if (theType is EventEPType eventEpType) {
                return eventEpType.EventType.UnderlyingType;
            }

            if (theType is ClassMultiValuedEPType classMultiValuedEpType) {
                return classMultiValuedEpType.Container;
            }

            if (theType is ClassEPType classEpType) {
                return classEpType.Clazz;
            }

            if (theType is NullEPType) {
                return null;
            }

            throw new ArgumentException("Unrecognized type " + theType);
        }

        public static Type GetCodegenReturnType(this EPType theType)
        {
            if (theType is EventMultiValuedEPType eventMultiValuedEpType) {
                // TBD - Please review, may need to be turned back to FlexCollection
                return eventMultiValuedEpType.Container;
                //return typeof(ICollection<EventBean>);
            }

            if (theType is ClassMultiValuedEPType classMultiValuedEpType) {
                return classMultiValuedEpType.Container;
            }

            if (theType is EventEPType) {
                return typeof(EventBean);
            }

            if (theType is ClassEPType classEPType) {
                return classEPType.Clazz.GetBoxedType();
            }

            if (theType is NullEPType) {
                return null;
            }

            throw new ArgumentException("Unrecognized type " + theType);
        }

        public static EPType OptionalFromEnumerationExpr(
            StatementRawInfo raw,
            StatementCompileTimeServices services,
            ExprNode exprNode)
        {
            if (!(exprNode is ExprEnumerationForge)) {
                return null;
            }

            var enumInfo = (ExprEnumerationForge) exprNode;
            if (enumInfo.ComponentTypeCollection != null) {
                return CollectionOfSingleValue(
                    enumInfo.ComponentTypeCollection,
                    typeof(ICollection<>).MakeGenericType(enumInfo.ComponentTypeCollection));
            }

            var eventTypeSingle = enumInfo.GetEventTypeSingle(raw, services);
            if (eventTypeSingle != null) {
                return SingleEvent(eventTypeSingle);
            }

            var eventTypeColl = enumInfo.GetEventTypeCollection(raw, services);
            if (eventTypeColl != null) {
                return CollectionOfEvents(eventTypeColl);
            }

            return null;
        }

        public static EventType OptionalIsEventTypeColl(this EPType type)
        {
            if (type != null && type is EventMultiValuedEPType) {
                return ((EventMultiValuedEPType) type).Component;
            }

            return null;
        }

        public static Type OptionalIsComponentTypeColl(this EPType type)
        {
            if (type != null && type is ClassMultiValuedEPType) {
                return ((ClassMultiValuedEPType) type).Component;
            }

            return null;
        }

        public static EventType OptionalIsEventTypeSingle(this EPType type)
        {
            if (type != null && type is EventEPType) {
                return ((EventEPType) type).EventType;
            }

            return null;
        }
        
        
        public static void TraverseAnnotations<T>(
            IList<Type> classes, 
            BiConsumer<Type, T> consumer)
            where T : Attribute
        {
            var annotationClass = typeof(T);
            WalkAnnotations(classes, (annotation, clazz) => {
                if (annotation.GetType() == annotationClass) {
                    consumer.Invoke(clazz, (T) annotation);
                }
            });
        }

        private static void WalkAnnotations(
            IEnumerable<Type> classes,
            AnnotationConsumer consumer)
        {
            if (classes == null) {
                return;
            }
            foreach (var clazz in classes) {
                foreach (var annotation in clazz.UnwrapAttributes(true)) {
                    consumer.Invoke(annotation, clazz);
                }
            }
        }

        public delegate void AnnotationConsumer(Attribute annotation, Type type);

#if false
        public static EPType CollectionOfSingleValue(Type collectionComponentType)
        {
            throw new NotImplementedException();
        }
#endif
    }
} // end of namespace