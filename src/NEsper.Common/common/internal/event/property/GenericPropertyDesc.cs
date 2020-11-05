///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

namespace com.espertech.esper.common.@internal.@event.property
{
    /// <summary>
    ///     Descriptor for a type and its generic type, if any.
    /// </summary>
    public class GenericPropertyDesc
    {
        /// <summary>
        ///     Ctor.
        /// </summary>
        /// <param name="type">the type</param>
        /// <param name="generic">its generic type parameter, if any</param>
        public GenericPropertyDesc(
            Type type,
            Type generic)
        {
            GenericType = type;
            Generic = generic;
        }

        /// <summary>
        ///     Ctor.
        /// </summary>
        /// <param name="type">the type</param>
        public GenericPropertyDesc(Type type)
        {
            GenericType = type;
            Generic = null;
        }

        /// <summary>
        ///     typeof(Object) type.
        /// </summary>
        /// <value>type descriptor</value>
        public static GenericPropertyDesc ObjectGeneric { get; } = new GenericPropertyDesc(typeof(object));

        /// <summary>
        ///     Returns the type.
        /// </summary>
        /// <value>type</value>
        public Type GenericType { get; }

        /// <summary>
        ///     Returns the generic parameter, or null if none.
        /// </summary>
        /// <value>generic parameter</value>
        public Type Generic { get; }
    }
} // end of namespace