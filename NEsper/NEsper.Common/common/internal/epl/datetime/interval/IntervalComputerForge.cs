///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using com.espertech.esper.common.@internal.bytecodemodel.@base;
using com.espertech.esper.common.@internal.bytecodemodel.model.expression;
using com.espertech.esper.common.@internal.epl.expression.codegen;

namespace com.espertech.esper.common.@internal.epl.datetime.interval
{
    public interface IntervalComputerForge
    {
        IntervalComputerEval MakeComputerEval();

        CodegenExpression Codegen(
            CodegenExpression leftStart,
            CodegenExpression leftEnd,
            CodegenExpression rightStart,
            CodegenExpression rightEnd,
            CodegenMethodScope codegenMethodScope,
            ExprForgeCodegenSymbol exprSymbol,
            CodegenClassScope codegenClassScope);
    }
} // end of namespace