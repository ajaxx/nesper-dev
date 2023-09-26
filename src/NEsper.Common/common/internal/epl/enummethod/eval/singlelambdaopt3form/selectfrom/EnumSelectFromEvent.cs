///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2015 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using com.espertech.esper.common.client;
using com.espertech.esper.common.client.collection;
using com.espertech.esper.common.@internal.bytecodemodel.@base;
using com.espertech.esper.common.@internal.bytecodemodel.model.expression;
using com.espertech.esper.common.@internal.epl.enummethod.codegen;
using com.espertech.esper.common.@internal.epl.enummethod.dot;
using com.espertech.esper.common.@internal.epl.enummethod.eval.singlelambdaopt3form.@base;
using com.espertech.esper.common.@internal.epl.expression.codegen;
using com.espertech.esper.compat.collections;

using static com.espertech.esper.common.@internal.bytecodemodel.model.expression.CodegenExpressionBuilder;

namespace com.espertech.esper.common.@internal.epl.enummethod.eval.singlelambdaopt3form.selectfrom
{
    public class EnumSelectFromEvent : ThreeFormEventPlain
    {
        public EnumSelectFromEvent(ExprDotEvalParamLambda lambda) : base(lambda)
        {
        }

        public override EnumEval EnumEvaluator {
            get {
                var inner = InnerExpression.ExprEvaluator;
                return new ProxyEnumEval() {
                    ProcEvaluateEnumMethod = (
                        eventsLambda,
                        enumcoll,
                        isNewData,
                        context) => {
                        if (enumcoll.IsEmpty()) {
                            return enumcoll;
                        }

                        var beans = (ICollection<EventBean>)enumcoll;
                        var result = new ArrayDeque<object>(enumcoll.Count);
                        foreach (var next in beans) {
                            eventsLambda[StreamNumLambda] = next;

                            var item = inner.Evaluate(eventsLambda, isNewData, context);
                            if (item != null) {
                                result.Add(item);
                            }
                        }

                        return result;
                    }
                };
            }
        }

        public override Type ReturnTypeOfMethod()
        {
            return typeof(FlexCollection);
        }

        public override CodegenExpression ReturnIfEmptyOptional()
        {
            return EnumForgeCodegenNames.REF_ENUMCOLL;
        }

        public override void InitBlock(
            CodegenBlock block,
            CodegenMethod methodNode,
            ExprForgeCodegenSymbol scope,
            CodegenClassScope codegenClassScope)
        {
            block.DeclareVar(
                typeof(ArrayDeque<object>),
                "result",
                NewInstance<ArrayDeque<object>>(ExprDotName(EnumForgeCodegenNames.REF_ENUMCOLL, "Count")));
        }

        public override void ForEachBlock(
            CodegenBlock block,
            CodegenMethod methodNode,
            ExprForgeCodegenSymbol scope,
            CodegenClassScope codegenClassScope)
        {
            block.DeclareVar<object>(
                    "item",
                    InnerExpression.EvaluateCodegen(typeof(object), methodNode, scope, codegenClassScope))
                .IfCondition(NotEqualsNull(Ref("item")))
                .Expression(ExprDotMethod(Ref("result"), "Add", Ref("item")));
        }

        public override void ReturnResult(CodegenBlock block)
        {
            block.MethodReturn(FlexWrap(Ref("result")));
        }
    }
} // end of namespace