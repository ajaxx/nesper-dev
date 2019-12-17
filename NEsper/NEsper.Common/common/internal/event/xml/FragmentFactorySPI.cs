///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using com.espertech.esper.common.@internal.bytecodemodel.@base;
using com.espertech.esper.common.@internal.bytecodemodel.model.expression;

namespace com.espertech.esper.common.@internal.@event.xml
{
    /// <summary>
    ///     Factory for event fragments for use with DOM getters.
    /// </summary>
    public interface FragmentFactorySPI : FragmentFactory
    {
        CodegenExpression Make(
            CodegenMethodScope parent,
            CodegenClassScope classScope);
    }
} // end of namespace