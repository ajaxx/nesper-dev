///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using com.espertech.esper.compat;
using com.espertech.esper.compat.collections;

namespace com.espertech.esper.common.client.dataflow.annotations
{
    /// <summary>
    /// Annotation for use with data flow operator forges to provide output type information
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class OutputTypesAttribute : Attribute
    {
        /// <summary>
        /// Port number
        /// </summary>
        /// <returns>port number</returns>
        public int PortNumber { get; set; }

        public OutputTypesAttribute()
        {
            PortNumber = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OutputTypesAttribute"/> class.
        /// </summary>
        /// <param name="portNumber">The port number.</param>
        public OutputTypesAttribute(
            int portNumber)
        {
            PortNumber = portNumber;
        }
    }
} // end of namespace