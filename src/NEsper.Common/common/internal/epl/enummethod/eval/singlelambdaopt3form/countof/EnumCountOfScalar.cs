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
using com.espertech.esper.common.@internal.epl.expression.core;
using com.espertech.esper.common.@internal.@event.arr;
using com.espertech.esper.compat.collections;
using static com.espertech.esper.common.@internal.bytecodemodel.model.expression.CodegenExpressionBuilder;

namespace com.espertech.esper.common.@internal.epl.enummethod.eval.singlelambdaopt3form.countof {
    public class EnumCountOfScalar : ThreeFormScalar {
        public EnumCountOfScalar(
            ExprDotEvalParamLambda lambda,
            ObjectArrayEventType resultEventType,
            int numParameters)
            : base(lambda, resultEventType, numParameters)
        {
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
                            return 0;
                        }

                        var rowcount = 0;
                        var evalEvent = new ObjectArrayEventBean(new object[3], fieldEventType);
                        eventsLambda[StreamNumLambda] = evalEvent;
                        var props = evalEvent.Properties;
                        props[2] = enumcoll.Count;

                        var count = -1;
                        foreach (var next in enumcoll) {
                            count++;
                            props[0] = next;
                            props[1] = count;

                            var pass = inner.Evaluate(eventsLambda, isNewData, context);
                            if (pass == null || false.Equals(pass)) {
                                continue;
                            }

                            rowcount++;
                        }

                        return rowcount;
                    });
            }
        }

        public override Type ReturnTypeOfMethod(Type inputCollectionType)
        {
            return typeof(int);
        }

        public override CodegenExpression ReturnIfEmptyOptional(Type inputCollectionType)
        {
            return Constant(0);
        }

        public override void InitBlock(
            CodegenBlock block,
            CodegenMethod methodNode,
            ExprForgeCodegenSymbol scope,
            CodegenClassScope codegenClassScope,
            Type inputCollectionType)
        {
            block.DeclareVar<int>("rowcount", Constant(0));
        }

        public override void ForEachBlock(
            CodegenBlock block,
            CodegenMethod methodNode,
            ExprForgeCodegenSymbol scope,
            CodegenClassScope codegenClassScope, Type inputCollectionType)
        {
            CodegenLegoBooleanExpression.CodegenContinueIfNotNullAndNotPass(
                block,
                InnerExpression.EvaluationType,
                InnerExpression.EvaluateCodegen(typeof(bool?), methodNode, scope, codegenClassScope));
            block.IncrementRef("rowcount");
        }

        public override void ReturnResult(CodegenBlock block)
        {
            block.MethodReturn(Ref("rowcount"));
        }
    }
} // end of namespace