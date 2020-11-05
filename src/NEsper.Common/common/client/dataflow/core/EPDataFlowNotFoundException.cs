///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

namespace com.espertech.esper.common.client.dataflow.core
{
    /// <summary>Thrown to indicate a data flow is not found. </summary>
    public class EPDataFlowNotFoundException : EPException
    {
        /// <summary>Ctor. </summary>
        /// <param name="message">error message</param>
        public EPDataFlowNotFoundException(string message)
            : base(message)
        {
        }
    }
}