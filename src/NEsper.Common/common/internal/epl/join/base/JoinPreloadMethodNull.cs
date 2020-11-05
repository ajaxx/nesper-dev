///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using com.espertech.esper.common.@internal.epl.expression.core;
using com.espertech.esper.common.@internal.epl.resultset.core;
using com.espertech.esper.common.@internal.view.util;

namespace com.espertech.esper.common.@internal.epl.join.@base
{
    /// <summary>
    ///     Implements a method for pre-loading (initializing) join that does not return any events.
    /// </summary>
    public class JoinPreloadMethodNull : JoinPreloadMethod
    {
        public void PreloadFromBuffer(
            int stream,
            ExprEvaluatorContext exprEvaluatorContext)
        {
        }

        public void PreloadAggregation(ResultSetProcessor resultSetProcessor)
        {
        }

        public void SetBuffer(
            BufferView buffer,
            int i)
        {
        }

        public bool IsPreloading => false;
    }
} // end of namespace