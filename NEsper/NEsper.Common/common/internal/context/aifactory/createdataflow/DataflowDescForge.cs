///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

using com.espertech.esper.common.client;
using com.espertech.esper.common.@internal.bytecodemodel.@base;
using com.espertech.esper.common.@internal.bytecodemodel.model.expression;
using com.espertech.esper.common.@internal.compile.stage3;
using com.espertech.esper.common.@internal.context.aifactory.core;
using com.espertech.esper.common.@internal.epl.dataflow.interfaces;
using com.espertech.esper.common.@internal.epl.dataflow.realize;
using com.espertech.esper.common.@internal.epl.dataflow.util;
using com.espertech.esper.common.@internal.@event.core;
using com.espertech.esper.common.@internal.util;
using com.espertech.esper.compat.collections;

using static com.espertech.esper.common.@internal.bytecodemodel.model.expression.CodegenExpressionBuilder;

namespace com.espertech.esper.common.@internal.context.aifactory.createdataflow
{
    public class DataflowDescForge
    {
        private readonly string dataflowName;
        private readonly IDictionary<string, EventType> declaredTypes;
        private readonly IList<LogicalChannel> logicalChannels;
        private readonly ISet<int> operatorBuildOrder;
        private readonly IDictionary<int, OperatorMetadataDescriptor> operatorMetadata;

        public DataflowDescForge(
            string dataflowName,
            IDictionary<string, EventType> declaredTypes,
            IDictionary<int, OperatorMetadataDescriptor> operatorMetadata,
            ISet<int> operatorBuildOrder,
            IDictionary<int, DataFlowOperatorForge> operatorFactories,
            IList<LogicalChannel> logicalChannels,
            IList<StmtForgeMethodResult> additionalForgables)
        {
            this.dataflowName = dataflowName;
            this.declaredTypes = declaredTypes;
            this.operatorMetadata = operatorMetadata;
            this.operatorBuildOrder = operatorBuildOrder;
            OperatorFactories = operatorFactories;
            this.logicalChannels = logicalChannels;
            AdditionalForgables = additionalForgables;
        }

        public IDictionary<int, DataFlowOperatorForge> OperatorFactories { get; }

        public IList<StmtForgeMethodResult> AdditionalForgables { get; }

        public CodegenExpression Make(
            CodegenMethodScope parent,
            SAIFFInitializeSymbol symbols,
            CodegenClassScope classScope)
        {
            var method = parent.MakeChild(typeof(DataflowDesc), GetType(), classScope);
            method.Block
                .DeclareVar<DataflowDesc>("df", NewInstance(typeof(DataflowDesc)))
                .SetProperty(Ref("df"), "DataflowName", Constant(dataflowName))
                .SetProperty(Ref("df"), "DeclaredTypes", MakeTypes(declaredTypes, method, symbols, classScope))
                .SetProperty(Ref("df"), "OperatorMetadata", MakeOpMeta(operatorMetadata, method, symbols, classScope))
                .SetProperty(
                    Ref("df"),
                    "OperatorBuildOrder",
                    MakeOpBuildOrder(operatorBuildOrder, method, symbols, classScope))
                .SetProperty(
                    Ref("df"),
                    "OperatorFactories",
                    MakeOpFactories(OperatorFactories, method, symbols, classScope))
                .SetProperty(Ref("df"), "LogicalChannels", MakeOpChannels(logicalChannels, method, symbols, classScope))
                .MethodReturn(Ref("df"));
            return LocalMethod(method);
        }

        private static CodegenExpression MakeOpChannels(
            IList<LogicalChannel> logicalChannels,
            CodegenMethodScope parent,
            SAIFFInitializeSymbol symbols,
            CodegenClassScope classScope)
        {
            var method = parent.MakeChild(typeof(IList<LogicalChannel>), typeof(DataflowDescForge), classScope);
            method.Block.DeclareVar<IList<LogicalChannel>>(
                "chnl",
                NewInstance<List<LogicalChannel>>(Constant(logicalChannels.Count)));
            foreach (var channel in logicalChannels) {
                method.Block.ExprDotMethod(Ref("chnl"), "Add", channel.Make(method, symbols, classScope));
            }

            method.Block.MethodReturn(Ref("chnl"));
            return LocalMethod(method);
        }

        private static CodegenExpression MakeOpBuildOrder(
            ISet<int> operatorBuildOrder,
            CodegenMethodScope parent,
            SAIFFInitializeSymbol symbols,
            CodegenClassScope classScope)
        {
            var method = parent.MakeChild(
                typeof(LinkedHashSet<int>),
                typeof(DataflowDescForge),
                classScope);
            method.Block.DeclareVar<LinkedHashSet<int>>(
                "order",
                NewInstance<LinkedHashSet<int>>());
            foreach (var entry in operatorBuildOrder) {
                method.Block.ExprDotMethod(Ref("order"), "Add", Constant(entry));
            }

            method.Block.MethodReturn(Ref("order"));
            return LocalMethod(method);
        }

        private static CodegenExpression MakeOpFactories(
            IDictionary<int, DataFlowOperatorForge> operatorFactories,
            CodegenMethodScope parent,
            SAIFFInitializeSymbol symbols,
            CodegenClassScope classScope)
        {
            var method = parent.MakeChild(typeof(IDictionary<int, DataFlowOperatorFactory>), typeof(DataflowDescForge), classScope);
            method.Block.DeclareVar<IDictionary<int, DataFlowOperatorFactory>>(
                "fac",
                NewInstance(typeof(Dictionary<int, DataFlowOperatorFactory>)));
            foreach (var entry in operatorFactories) {
                method.Block.ExprDotMethod(
                    Ref("fac"),
                    "Put",
                    Constant(entry.Key),
                    entry.Value.Make(method, symbols, classScope));
            }

            method.Block.MethodReturn(Ref("fac"));
            return LocalMethod(method);
        }

        private static CodegenExpression MakeOpMeta(
            IDictionary<int, OperatorMetadataDescriptor> operatorMetadata,
            CodegenMethodScope parent,
            SAIFFInitializeSymbol symbols,
            CodegenClassScope classScope)
        {
            var method = parent.MakeChild(typeof(IDictionary<int, OperatorMetadataDescriptor>), typeof(DataflowDescForge), classScope);
            method.Block.DeclareVar<IDictionary<int, OperatorMetadataDescriptor>>(
                "op",
                NewInstance(typeof(Dictionary<int, OperatorMetadataDescriptor>)));
            foreach (var entry in operatorMetadata) {
                method.Block.ExprDotMethod(
                    Ref("op"),
                    "Put",
                    Constant(entry.Key),
                    entry.Value.Make(method, symbols, classScope));
            }

            method.Block.MethodReturn(Ref("op"));
            return LocalMethod(method);
        }

        private static CodegenExpression MakeTypes(
            IDictionary<string, EventType> declaredTypes,
            CodegenMethodScope parent,
            SAIFFInitializeSymbol symbols,
            CodegenClassScope classScope)
        {
            var method = parent.MakeChild(typeof(IDictionary<string, EventType>), typeof(DataflowDescForge), classScope);
            method.Block.DeclareVar<IDictionary<string, EventType>>(
                "types",
                NewInstance(typeof(Dictionary<string, EventType>)));
            foreach (var entry in declaredTypes) {
                method.Block.ExprDotMethod(
                    Ref("types"),
                    "Put",
                    Constant(entry.Key),
                    EventTypeUtility.ResolveTypeCodegen(entry.Value, symbols.GetAddInitSvc(method)));
            }

            method.Block.MethodReturn(Ref("types"));
            return LocalMethod(method);
        }
    }
} // end of namespace