///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using com.espertech.esper.common.@internal.bytecodemodel.@base;
using com.espertech.esper.common.@internal.bytecodemodel.model.expression;
using com.espertech.esper.common.@internal.compile.stage1.spec;
using com.espertech.esper.common.@internal.context.aifactory.core;
using com.espertech.esper.common.@internal.context.module;
using com.espertech.esper.common.@internal.epl.contained;
using com.espertech.esper.common.@internal.epl.expression.core;
using com.espertech.esper.common.@internal.epl.join.querygraph;
using com.espertech.esper.common.@internal.epl.namedwindow.core;
using com.espertech.esper.common.@internal.epl.namedwindow.path;
using com.espertech.esper.compat;

using static com.espertech.esper.common.@internal.bytecodemodel.model.expression.CodegenExpressionBuilder;

namespace com.espertech.esper.common.@internal.context.activator
{
    public class ViewableActivatorNamedWindowForge : ViewableActivatorForge
    {
        private readonly ExprNode filterEvaluator;
        private readonly QueryGraphForge filterQueryGraph;
        private readonly NamedWindowMetaData namedWindow;
        private readonly PropertyEvaluatorForge optPropertyEvaluator;

        private readonly NamedWindowConsumerStreamSpec spec;
        private readonly bool subquery;

        public ViewableActivatorNamedWindowForge(
            NamedWindowConsumerStreamSpec spec,
            NamedWindowMetaData namedWindow,
            ExprNode filterEvaluator,
            QueryGraphForge filterQueryGraph,
            bool subquery,
            PropertyEvaluatorForge optPropertyEvaluator)
        {
            this.spec = spec;
            this.namedWindow = namedWindow;
            this.filterEvaluator = filterEvaluator;
            this.filterQueryGraph = filterQueryGraph;
            this.subquery = subquery;
            this.optPropertyEvaluator = optPropertyEvaluator;
        }

        public CodegenExpression MakeCodegen(
            CodegenMethodScope parent,
            SAIFFInitializeSymbol symbols,
            CodegenClassScope classScope)
        {
            if (spec.NamedWindowConsumerId == -1) {
                throw new IllegalStateException("Unassigned named window consumer id");
            }

            var method = parent.MakeChild(typeof(ViewableActivatorNamedWindow), GetType(), classScope);

            CodegenExpression filter;
            if (filterEvaluator == null) {
                filter = ConstantNull();
            }
            else {
                filter = ExprNodeUtilityCodegen.CodegenEvaluator(filterEvaluator.Forge, method, GetType(), classScope);
            }

            method.Block
                .DeclareVar<ViewableActivatorNamedWindow>(
                    "activator",
                    ExprDotMethodChain(symbols.GetAddInitSvc(method))
                        .Get(EPStatementInitServicesConstants.VIEWABLEACTIVATORFACTORY)
                        .Add("CreateNamedWindow"))
                .SetProperty(
                    Ref("activator"),
                    "NamedWindow",
                    NamedWindowDeployTimeResolver.MakeResolveNamedWindow(namedWindow, symbols.GetAddInitSvc(method)))
                .SetProperty(Ref("activator"), "NamedWindowConsumerId", Constant(spec.NamedWindowConsumerId))
                .SetProperty(Ref("activator"), "FilterEvaluator", filter)
                .SetProperty(
                    Ref("activator"),
                    "FilterQueryGraph",
                    filterQueryGraph == null ? ConstantNull() : filterQueryGraph.Make(method, symbols, classScope))
                .SetProperty(Ref("activator"), "Subquery", Constant(subquery))
                .SetProperty(
                    Ref("activator"),
                    "OptPropertyEvaluator",
                    optPropertyEvaluator == null
                        ? ConstantNull()
                        : optPropertyEvaluator.Make(method, symbols, classScope))
                .ExprDotMethod(
                    symbols.GetAddInitSvc(method),
                    "AddReadyCallback",
                    Ref("activator")) // add ready-callback
                .MethodReturn(Ref("activator"));
            return LocalMethod(method);
        }
    }
} // end of namespace