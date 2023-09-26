///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2015 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using com.espertech.esper.common.@internal.bytecodemodel.@base;
using com.espertech.esper.common.@internal.bytecodemodel.model.expression;
using com.espertech.esper.common.@internal.epl.enummethod.dot;
using com.espertech.esper.common.@internal.epl.enummethod.eval.singlelambdaopt3form.@base;
using com.espertech.esper.common.@internal.epl.expression.codegen;
using com.espertech.esper.common.@internal.@event.arr;
using com.espertech.esper.compat.collections;

using static com.espertech.esper.common.@internal.bytecodemodel.model.expression.CodegenExpressionBuilder; //constant;

namespace com.espertech.esper.common.@internal.epl.enummethod.eval.singlelambdaopt3form.allofanyof
{
    public class EnumAllOfAnyOfScalar : ThreeFormScalar
    {
        private readonly bool all;

        public EnumAllOfAnyOfScalar(
            ExprDotEvalParamLambda lambda,
            ObjectArrayEventType resultEventType,
            int numParameters,
            bool all)
            : base(lambda, resultEventType, numParameters)
        {
            this.all = all;
        }

        public override EnumEval EnumEvaluator {
            get {
                var inner = InnerExpression.ExprEvaluator;

                return new ProxyEnumEval(
                    (
                        eventsLambda,
                        enumcoll,
                        isNewData,
                        context) => {
                        if (enumcoll.IsEmpty()) {
                            return all;
                        }

                        var evalEvent = new ObjectArrayEventBean(new object[3], fieldEventType);
                        eventsLambda[StreamNumLambda] = evalEvent;
                        var props = evalEvent.Properties;
                        var count = -1;
                        props[2] = enumcoll.Count;

                        foreach (var next in enumcoll) {
                            count++;
                            props[0] = next;
                            props[1] = count;

                            var pass = inner.Evaluate(eventsLambda, isNewData, context);
                            if (all) {
                                if (pass == null || false.Equals(pass)) {
                                    return false;
                                }
                            }
                            else {
                                if (pass != null && (bool)pass) {
                                    return true;
                                }
                            }
                        }

                        return all;
                    });
            }
        }

        public override Type ReturnTypeOfMethod()
        {
            return typeof(bool?);
        }

        public override CodegenExpression ReturnIfEmptyOptional()
        {
            return Constant(all);
        }

        public override void InitBlock(
            CodegenBlock block,
            CodegenMethod methodNode,
            ExprForgeCodegenSymbol scope,
            CodegenClassScope codegenClassScope)
        {
        }

        public override void ForEachBlock(
            CodegenBlock block,
            CodegenMethod methodNode,
            ExprForgeCodegenSymbol scope,
            CodegenClassScope codegenClassScope)
        {
            CodegenLegoBooleanExpression.CodegenReturnBoolIfNullOrBool(
                block,
                InnerExpression.EvaluationType,
                InnerExpression.EvaluateCodegen(typeof(bool?), methodNode, scope, codegenClassScope),
                all,
                all ? false : (bool?)null,
                !all,
                !all);
        }

        public override void ReturnResult(CodegenBlock block)
        {
            block.MethodReturn(Constant(all));
        }
    }
} // end of namespace