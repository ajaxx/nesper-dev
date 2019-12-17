///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using com.espertech.esper.common.client.util;

namespace com.espertech.esper.common.client.annotation
{
    /// <summary>
    /// Annotation that can be attached to specify which underlying event representation to use for events.
    /// </summary>
    public class EventRepresentationAttribute : Attribute
    {
        /// <summary>
        /// Define the event underlying type.
        /// </summary>
        /// <value>
        /// The event underlying type.
        /// </value>
        public virtual EventUnderlyingType Value { get; set; }
    }
} // end of namespace