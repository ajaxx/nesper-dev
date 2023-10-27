///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using com.espertech.esper.common.@internal.bytecodemodel.@base;
using com.espertech.esper.common.@internal.bytecodemodel.model.expression;

namespace com.espertech.esper.common.@internal.rettype
{
    public class EPChainableTypeNull : EPChainableType
    {
        public static readonly EPChainableTypeNull INSTANCE = new EPChainableTypeNull();

        public CodegenExpression Codegen(
            CodegenMethod method,
            CodegenClassScope classScope,
            CodegenExpression typeInitSvcRef)
        {
            // TODO: Shouldn't this be a static field reference?
            return CodegenExpressionBuilder.EnumValue(typeof(EPChainableTypeNull), "INSTANCE");
        }
    }
}