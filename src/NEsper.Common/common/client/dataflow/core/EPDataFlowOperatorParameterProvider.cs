///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

namespace com.espertech.esper.common.client.dataflow.core
{
    /// <summary>
    /// Handles setting or overriding properties for operators in a data flow.
    /// </summary>
    public interface EPDataFlowOperatorParameterProvider
    {
        /// <summary>
        /// Return new value for operator
        /// </summary>
        /// <param name="context">operator and parameter information</param>
        /// <returns>value</returns>
        object Provide(EPDataFlowOperatorParameterProviderContext context);
    }
}