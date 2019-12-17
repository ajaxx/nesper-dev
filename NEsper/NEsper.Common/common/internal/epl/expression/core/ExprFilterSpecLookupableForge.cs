///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using com.espertech.esper.common.client;
using com.espertech.esper.common.@internal.bytecodemodel.@base;
using com.espertech.esper.common.@internal.bytecodemodel.core;
using com.espertech.esper.common.@internal.bytecodemodel.model.expression;
using com.espertech.esper.common.@internal.context.aifactory.core;
using com.espertech.esper.common.@internal.context.module;
using com.espertech.esper.common.@internal.@event.core;
using com.espertech.esper.compat;

using static com.espertech.esper.common.@internal.bytecodemodel.model.expression.CodegenExpressionBuilder;

namespace com.espertech.esper.common.@internal.epl.expression.core
{
    public class ExprFilterSpecLookupableForge
    {
        internal readonly string expression;
        internal readonly bool isNonPropertyGetter;
        internal readonly EventPropertyValueGetterForge optionalEventPropForge;
        internal readonly Type returnType;

        public ExprFilterSpecLookupableForge(
            string expression,
            EventPropertyValueGetterForge optionalEventPropForge,
            Type returnType,
            bool isNonPropertyGetter)
        {
            this.expression = expression;
            this.optionalEventPropForge = optionalEventPropForge;
            this.returnType =
                returnType.GetBoxedType(); // For type consistency for recovery and serde define as boxed type
            this.isNonPropertyGetter = isNonPropertyGetter;
        }

        public Type ReturnType => returnType;

        public string Expression => expression;

        public virtual CodegenMethod MakeCodegen(
            CodegenMethodScope parent,
            SAIFFInitializeSymbolWEventType symbols,
            CodegenClassScope classScope)
        {
            var method = parent.MakeChild(
                typeof(ExprFilterSpecLookupable),
                typeof(ExprFilterSpecLookupableForge),
                classScope);
            CodegenExpression getterExpr;
            if (optionalEventPropForge != null) {
                var get = new CodegenExpressionLambda(method.Block)
                    .WithParams(CodegenNamedParam.From(typeof(EventBean), "bean"));
                var anonymous = NewInstance<ProxyEventPropertyValueGetter>(get);

                //var anonymous = NewAnonymousClass(method.Block, typeof(EventPropertyValueGetter));
                //var get = CodegenMethod.MakeParentNode(typeof(object), GetType(), classScope)
                //    .AddParam(CodegenNamedParam.From(typeof(EventBean), "bean"));
                //anonymous.AddMethod("Get", get);

                get.Block.BlockReturn(optionalEventPropForge.EventBeanGetCodegen(Ref("bean"), method, classScope));
                getterExpr = anonymous;
            }
            else {
                getterExpr = ConstantNull();
            }

            method.Block.DeclareVar<EventPropertyValueGetter>("getter", getterExpr);

            method.Block
                .DeclareVar<ExprFilterSpecLookupable>(
                    "lookupable",
                    NewInstance<ExprFilterSpecLookupable>(
                        Constant(expression),
                        Ref("getter"),
                        Typeof(returnType),
                        Constant(isNonPropertyGetter)))
                .Expression(
                    ExprDotMethodChain(symbols.GetAddInitSvc(method))
                        .Get(EPStatementInitServicesConstants.FILTERSHAREDLOOKUPABLEREGISTERY)
                        .Add(
                            "RegisterLookupable",
                            symbols.GetAddEventType(method),
                            Ref("lookupable")))
                .MethodReturn(Ref("lookupable"));
            return method;
        }

        public override bool Equals(object o)
        {
            if (this == o) {
                return true;
            }

            if (o == null || GetType() != o.GetType()) {
                return false;
            }

            var that = (ExprFilterSpecLookupableForge) o;

            if (!expression.Equals(that.expression)) {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return expression.GetHashCode();
        }
    }
} // end of namespace