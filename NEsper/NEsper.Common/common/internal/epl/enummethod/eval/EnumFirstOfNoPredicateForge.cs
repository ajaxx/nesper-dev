///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;

using com.espertech.esper.common.client;
using com.espertech.esper.common.@internal.bytecodemodel.@base;
using com.espertech.esper.common.@internal.bytecodemodel.model.expression;
using com.espertech.esper.common.@internal.epl.enummethod.codegen;
using com.espertech.esper.common.@internal.epl.expression.codegen;
using com.espertech.esper.common.@internal.epl.expression.core;
using com.espertech.esper.common.@internal.rettype;
using com.espertech.esper.compat;
using com.espertech.esper.compat.collections;

using static com.espertech.esper.common.@internal.bytecodemodel.model.expression.CodegenExpressionBuilder;

namespace com.espertech.esper.common.@internal.epl.enummethod.eval
{
    public class EnumFirstOfNoPredicateForge : EnumForgeBase,
        EnumForge,
        EnumEval
    {
        private readonly EPType _resultType;

        public EnumFirstOfNoPredicateForge(
            int streamCountIncoming,
            EPType resultType)
            : base(streamCountIncoming)
        {
            _resultType = resultType;
        }

        public override EnumEval EnumEvaluator {
            get => this;
        }

        public object EvaluateEnumMethod(
            EventBean[] eventsLambda,
            ICollection<object> enumcoll,
            bool isNewData,
            ExprEvaluatorContext context)
        {
            if (enumcoll == null || enumcoll.IsEmpty()) {
                return null;
            }

            return enumcoll.First();
        }

        public override CodegenExpression Codegen(
            EnumForgeCodegenParams args,
            CodegenMethodScope codegenMethodScope,
            CodegenClassScope codegenClassScope)
        {
            var type = EPTypeHelper.GetCodegenReturnType(_resultType);
            var paramTypes = EnumForgeCodegenNames.PARAMS;
            
            var method = codegenMethodScope
                .MakeChild(type, typeof(EnumFirstOfNoPredicateForge), codegenClassScope)
                .AddParam(paramTypes)
                .Block
                .IfCondition(
                    Or(
                        EqualsNull(EnumForgeCodegenNames.REF_ENUMCOLL),
                        ExprDotMethod(EnumForgeCodegenNames.REF_ENUMCOLL, "IsEmpty")))
                .BlockReturn(ConstantNull())
                .MethodReturn(
                    Cast(type, ExprDotMethodChain(EnumForgeCodegenNames.REF_ENUMCOLL).Add("First")));
            return LocalMethod(method, args.Expressions);
        }
    }
} // end of namespace