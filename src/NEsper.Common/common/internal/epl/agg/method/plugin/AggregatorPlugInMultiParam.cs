///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using com.espertech.esper.common.client.hook.aggfunc;
using com.espertech.esper.common.client.hook.forgeinject;
using com.espertech.esper.common.@internal.bytecodemodel.@base;
using com.espertech.esper.common.@internal.bytecodemodel.core;
using com.espertech.esper.common.@internal.bytecodemodel.model.expression;
using com.espertech.esper.common.@internal.epl.agg.method.core;
using com.espertech.esper.common.@internal.epl.expression.codegen;
using com.espertech.esper.common.@internal.epl.expression.core;

using static com.espertech.esper.common.@internal.bytecodemodel.model.expression.CodegenExpressionBuilder;
using static com.espertech.esper.common.@internal.epl.agg.method.core.AggregatorCodegenUtil;

namespace com.espertech.esper.common.@internal.epl.agg.method.plugin
{
    public class AggregatorPlugInMultiParam : AggregatorMethod
    {
        private readonly AggregationFunctionModeMultiParam mode;

        internal CodegenExpressionMember plugin;

        public AggregatorPlugInMultiParam(
            AggregationForgeFactoryPlugin factory,
            int col,
            CodegenCtor rowCtor,
            CodegenMemberCol membersColumnized,
            CodegenClassScope classScope,
            AggregationFunctionModeMultiParam mode)
        {
            this.mode = mode;
            var injectionStrategy =
                (InjectionStrategyClassNewInstance) mode.InjectionStrategyAggregationFunctionFactory;
            var factoryField = classScope.AddDefaultFieldUnshared<AggregationFunctionFactory>(
                true,
                injectionStrategy.GetInitializationExpression(classScope));

            plugin = membersColumnized.AddMember(col, typeof(AggregationFunction), "plugin");
            rowCtor.Block.AssignRef(plugin, ExprDotMethod(factoryField, "NewAggregator", ConstantNull()));
        }

        public void ApplyEvalEnterCodegen(
            CodegenMethod method,
            ExprForgeCodegenSymbol symbols,
            ExprForge[] forges,
            CodegenClassScope classScope)
        {
            Apply(true, method, symbols, forges, classScope);
        }

        public void ApplyEvalLeaveCodegen(
            CodegenMethod method,
            ExprForgeCodegenSymbol symbols,
            ExprForge[] forges,
            CodegenClassScope classScope)
        {
            Apply(false, method, symbols, forges, classScope);
        }

        public void ApplyTableEnterCodegen(
            CodegenExpressionRef value,
            Type[] evaluationTypes,
            CodegenMethod method,
            CodegenClassScope classScope)
        {
            method.Block.ExprDotMethod(plugin, "Enter", value);
        }

        public void ApplyTableLeaveCodegen(
            CodegenExpressionRef value,
            Type[] evaluationTypes,
            CodegenMethod method,
            CodegenClassScope classScope)
        {
            method.Block.ExprDotMethod(plugin, "Leave", value);
        }

        public void ClearCodegen(
            CodegenMethod method,
            CodegenClassScope classScope)
        {
            method.Block.ExprDotMethod(plugin, "Clear");
        }

        public void GetValueCodegen(
            CodegenMethod method,
            CodegenClassScope classScope)
        {
            method.Block.MethodReturn(ExprDotName(plugin, "Value"));
        }

        public void WriteCodegen(
            CodegenExpressionRef row,
            int col,
            CodegenExpressionRef output,
            CodegenExpressionRef unitKey,
            CodegenExpressionRef writer,
            CodegenMethod method,
            CodegenClassScope classScope)
        {
            if (mode.HasHA) {
                method.Block.StaticMethod(mode.Serde, "Write", output, RowDotMember(row, plugin));
            }
        }

        public void ReadCodegen(
            CodegenExpressionRef row,
            int col,
            CodegenExpressionRef input,
            CodegenExpressionRef unitKey,
            CodegenMethod method,
            CodegenClassScope classScope)
        {
            if (mode.HasHA) {
                method.Block.AssignRef(RowDotMember(row, plugin), StaticMethod(mode.Serde, "Read", input));
            }
        }

        private void Apply(
            bool enter,
            CodegenMethod method,
            ExprForgeCodegenSymbol symbols,
            ExprForge[] forges,
            CodegenClassScope classScope)
        {
            CodegenExpression expression;
            if (forges.Length == 0) {
                expression = ConstantNull();
            }
            else if (forges.Length == 1) {
                expression = forges[0].EvaluateCodegen(typeof(object), method, symbols, classScope);
            }
            else {
                method.Block.DeclareVar<object[]>(
                    "parameters",
                    NewArrayByLength(typeof(object), Constant(forges.Length)));
                for (var i = 0; i < forges.Length; i++) {
                    method.Block.AssignArrayElement(
                        "parameters",
                        Constant(i),
                        forges[i].EvaluateCodegen(typeof(object), method, symbols, classScope));
                }

                expression = Ref("parameters");
            }

            method.Block.ExprDotMethod(plugin, enter ? "Enter" : "Leave", expression);
        }
    }
} // end of namespace