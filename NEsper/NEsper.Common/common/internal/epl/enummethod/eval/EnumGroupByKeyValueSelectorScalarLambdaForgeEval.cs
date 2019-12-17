///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using com.espertech.esper.common.client;
using com.espertech.esper.common.@internal.bytecodemodel.@base;
using com.espertech.esper.common.@internal.bytecodemodel.model.expression;
using com.espertech.esper.common.@internal.context.module;
using com.espertech.esper.common.@internal.epl.enummethod.codegen;
using com.espertech.esper.common.@internal.epl.expression.codegen;
using com.espertech.esper.common.@internal.epl.expression.core;
using com.espertech.esper.common.@internal.@event.arr;
using com.espertech.esper.common.@internal.@event.core;
using com.espertech.esper.compat;
using com.espertech.esper.compat.collections;

using static com.espertech.esper.common.@internal.bytecodemodel.model.expression.CodegenExpressionBuilder;

namespace com.espertech.esper.common.@internal.epl.enummethod.eval
{
    public class EnumGroupByKeyValueSelectorScalarLambdaForgeEval : EnumEval
    {
        private readonly EnumGroupByKeyValueSelectorScalarLambdaForge _forge;
        private readonly ExprEvaluator _innerExpression;
        private readonly ExprEvaluator _secondExpression;

        public EnumGroupByKeyValueSelectorScalarLambdaForgeEval(
            EnumGroupByKeyValueSelectorScalarLambdaForge forge,
            ExprEvaluator innerExpression,
            ExprEvaluator secondExpression)
        {
            _forge = forge;
            _innerExpression = innerExpression;
            _secondExpression = secondExpression;
        }

        public object EvaluateEnumMethod(
            EventBean[] eventsLambda,
            ICollection<object> enumcoll,
            bool isNewData,
            ExprEvaluatorContext context)
        {
            if (enumcoll.IsEmpty()) {
                return Collections.GetEmptyMap<object, object>();
            }

            IDictionary<object, ICollection<object>> result = new LinkedHashMap<object, ICollection<object>>();

            var resultEvent = new ObjectArrayEventBean(new object[1], _forge.resultEventType);
            var props = resultEvent.Properties;
            eventsLambda[_forge.StreamNumLambda] = resultEvent;
            var values = (ICollection<object>) enumcoll;

            foreach (var next in values) {
                props[0] = next;
                var key = _innerExpression.Evaluate(eventsLambda, isNewData, context);
                var entry = _secondExpression.Evaluate(eventsLambda, isNewData, context);

                var value = result.Get(key);
                if (value == null) {
                    value = new List<object>();
                    result.Put(key, value);
                }

                value.Add(entry);
            }

            return result;
        }

        public static CodegenExpression Codegen(
            EnumGroupByKeyValueSelectorScalarLambdaForge forge,
            EnumForgeCodegenParams args,
            CodegenMethodScope codegenMethodScope,
            CodegenClassScope codegenClassScope)
        {
            var resultTypeMember = codegenClassScope.AddDefaultFieldUnshared(
                true,
                typeof(ObjectArrayEventType),
                Cast(
                    typeof(ObjectArrayEventType),
                    EventTypeUtility.ResolveTypeCodegen(forge.resultEventType, EPStatementInitServicesConstants.REF)));
            var paramTypes = EnumForgeCodegenNames.PARAMS;
            var scope = new ExprForgeCodegenSymbol(false, null);
            var methodNode = codegenMethodScope.MakeChildWithScope(
                    typeof(IDictionary<object, object>),
                    typeof(EnumGroupByKeyValueSelectorScalarLambdaForgeEval),
                    scope,
                    codegenClassScope)
                .AddParam(paramTypes);

            var block = methodNode.Block
                .IfCondition(ExprDotMethod(EnumForgeCodegenNames.REF_ENUMCOLL, "IsEmpty"))
                .BlockReturn(StaticMethod(typeof(Collections), "GetEmptyMap", new[] { typeof(object), typeof(object) }))
                .DeclareVar<IDictionary<object, object>>("result", NewInstance(typeof(LinkedHashMap<object, object>)))
                .DeclareVar<ObjectArrayEventBean>(
                    "resultEvent",
                    NewInstance<ObjectArrayEventBean>(NewArrayByLength(typeof(object), Constant(1)), resultTypeMember))
                .AssignArrayElement(EnumForgeCodegenNames.REF_EPS, Constant(forge.StreamNumLambda), Ref("resultEvent"))
                .DeclareVar<object[]>("props", ExprDotName(Ref("resultEvent"), "Properties"));

            block.ForEach(typeof(object), "next", EnumForgeCodegenNames.REF_ENUMCOLL)
                .AssignArrayElement("props", Constant(0), Ref("next"))
                .DeclareVar<object>(
                    "key",
                    forge.InnerExpression.EvaluateCodegen(typeof(object), methodNode, scope, codegenClassScope))
                .DeclareVar<object>(
                    "entry",
                    forge.secondExpression.EvaluateCodegen(typeof(object), methodNode, scope, codegenClassScope))
                .DeclareVar<ICollection<object>>(
                    "value",
                    Cast(typeof(ICollection<object>), ExprDotMethod(Ref("result"), "Get", Ref("key"))))
                .IfRefNull("value")
                .AssignRef("value", NewInstance(typeof(List<object>)))
                .Expression(ExprDotMethod(Ref("result"), "Put", Ref("key"), Ref("value")))
                .BlockEnd()
                .Expression(ExprDotMethod(Ref("value"), "Add", Ref("entry")));
            block.MethodReturn(Ref("result"));
            return LocalMethod(methodNode, args.Eps, args.Enumcoll, args.IsNewData, args.ExprCtx);
        }
    }
} // end of namespace