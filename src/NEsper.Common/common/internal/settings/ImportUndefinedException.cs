///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Runtime.Serialization;

namespace com.espertech.esper.common.@internal.settings
{
    /// <summary>
    ///     Indicates a problem importing classes, aggregation functions and the like.
    /// </summary>
    [Serializable]
    public class ImportUndefinedException : Exception
    {
        /// <summary>
        ///     Ctor.
        /// </summary>
        /// <param name="msg">exception message</param>
        public ImportUndefinedException(string msg)
            : base(msg)
        {
        }

        /// <summary>
        ///     Ctor.
        /// </summary>
        /// <param name="msg">exception message</param>
        /// <param name="ex">inner exception</param>
        public ImportUndefinedException(
            string msg,
            Exception ex)
            : base(msg, ex)
        {
        }

        protected ImportUndefinedException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
} // end of namespace