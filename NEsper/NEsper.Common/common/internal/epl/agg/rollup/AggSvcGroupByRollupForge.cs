///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using com.espertech.esper.common.@internal.bytecodemodel.@base;
using com.espertech.esper.common.@internal.bytecodemodel.core;
using com.espertech.esper.common.@internal.bytecodemodel.model.expression;
using com.espertech.esper.common.@internal.context.module;
using com.espertech.esper.common.@internal.epl.agg.core;
using com.espertech.esper.common.@internal.epl.expression.core;
using com.espertech.esper.common.@internal.serde;
using com.espertech.esper.common.@internal.util;

using static com.espertech.esper.common.@internal.epl.agg.core.AggregationServiceCodegenNames;
using static com.espertech.esper.common.@internal.bytecodemodel.model.expression.CodegenExpressionBuilder;
using static com.espertech.esper.common.@internal.bytecodemodel.model.expression.CodegenExpressionRelational.
    CodegenRelational;
using static com.espertech.esper.common.@internal.epl.expression.codegen.ExprForgeCodegenNames;

namespace com.espertech.esper.common.@internal.epl.agg.rollup
{
    /// <summary>
    ///     Implementation for handling aggregation with grouping by group-keys.
    /// </summary>
    public class AggSvcGroupByRollupForge : AggregationServiceFactoryForgeWMethodGen
    {
        private static readonly CodegenExpressionRef REF_AGGREGATORSPERGROUP = Ref("aggregatorsPerGroup");
        private static readonly CodegenExpressionRef REF_AGGREGATORTOPGROUP = Ref("aggregatorTopGroup");
        private static readonly CodegenExpressionRef REF_CURRENTROW = Ref("currentRow");
        private static readonly CodegenExpressionRef REF_CURRENTGROUPKEY = Ref("currentGroupKey");
        private static readonly CodegenExpressionRef REF_HASREMOVEDKEY = Ref("hasRemovedKey");
        private static readonly CodegenExpressionRef REF_REMOVEDKEYS = Ref("removedKeys");
        internal readonly ExprNode[] groupByNodes;
        internal readonly AggregationGroupByRollupDesc rollupDesc;

        internal readonly AggregationRowStateForgeDesc rowStateForgeDesc;

        public AggSvcGroupByRollupForge(
            AggregationRowStateForgeDesc rowStateForgeDesc,
            AggregationGroupByRollupDesc rollupDesc,
            ExprNode[] groupByNodes)
        {
            this.rowStateForgeDesc = rowStateForgeDesc;
            this.rollupDesc = rollupDesc;
            this.groupByNodes = groupByNodes;
        }

        public AggregationCodegenRowLevelDesc RowLevelDesc =>
            AggregationCodegenRowLevelDesc.FromTopOnly(rowStateForgeDesc);

        public void ProviderCodegen(
            CodegenMethod method,
            CodegenClassScope classScope,
            AggregationClassNames classNames)
        {
            Type[] groupByTypes = ExprNodeUtilityQuery.GetExprResultTypes(groupByNodes);
            method.Block
                .DeclareVar<AggregationServiceFactory>(
                    "svcFactory",
                    NewInstance(classNames.ServiceFactory, Ref("this")))
                .DeclareVar<AggregationRowFactory>(
                    "rowFactory",
                    NewInstance(classNames.RowFactoryTop, Ref("this")))
                .DeclareVar<DataInputOutputSerdeWCollation<AggregationRow>>(
                    "rowSerde",
                    NewInstance(classNames.RowSerdeTop, Ref("this")))
                .MethodReturn(
                    ExprDotMethodChain(EPStatementInitServicesConstants.REF)
                        .Get(EPStatementInitServicesConstants.AGGREGATIONSERVICEFACTORYSERVICE)
                        .Add(
                            "GroupByRollup",
                            Ref("svcFactory"),
                            rollupDesc.Codegen(),
                            Ref("rowFactory"),
                            rowStateForgeDesc.UseFlags.ToExpression(),
                            Ref("rowSerde"),
                            Constant(groupByTypes)));
        }

        public void RowCtorCodegen(AggregationRowCtorDesc rowCtorDesc)
        {
            AggregationServiceCodegenUtil.GenerateIncidentals(true, false, rowCtorDesc);
        }

        public void MakeServiceCodegen(
            CodegenMethod method,
            CodegenClassScope classScope,
            AggregationClassNames classNames)
        {
            method.Block.MethodReturn(NewInstance(classNames.Service, Ref("o")));
        }

        public void CtorCodegen(
            CodegenCtor ctor,
            IList<CodegenTypedParam> explicitMembers,
            CodegenClassScope classScope,
            AggregationClassNames classNames)
        {
            explicitMembers.Add(
                new CodegenTypedParam(typeof(IDictionary<object, object>[]), REF_AGGREGATORSPERGROUP.Ref));
            explicitMembers.Add(new CodegenTypedParam(typeof(IList<object>[]), REF_REMOVEDKEYS.Ref));
            ctor.Block.AssignRef(
                    REF_AGGREGATORSPERGROUP,
                    NewArrayByLength(typeof(IDictionary<object, object>), Constant(rollupDesc.NumLevelsAggregation)))
                .AssignRef(
                    REF_REMOVEDKEYS,
                    NewArrayByLength(typeof(IList<object>), Constant(rollupDesc.NumLevelsAggregation)));
            for (var i = 0; i < rollupDesc.NumLevelsAggregation; i++) {
                ctor.Block.AssignArrayElement(
                    REF_AGGREGATORSPERGROUP,
                    Constant(i),
                    NewInstance(typeof(Dictionary<object, object>)));
                ctor.Block.AssignArrayElement(REF_REMOVEDKEYS, Constant(i), NewInstance<List<object>>(Constant(4)));
            }

            explicitMembers.Add(new CodegenTypedParam(classNames.RowTop, REF_AGGREGATORTOPGROUP.Ref));
            ctor.Block.AssignRef(REF_AGGREGATORTOPGROUP, NewInstance(classNames.RowTop, Ref("o")))
                .ExprDotMethod(REF_AGGREGATORTOPGROUP, "DecreaseRefcount");

            explicitMembers.Add(new CodegenTypedParam(typeof(AggregationRow), REF_CURRENTROW.Ref));
            explicitMembers.Add(new CodegenTypedParam(typeof(object), REF_CURRENTGROUPKEY.Ref));
            explicitMembers.Add(new CodegenTypedParam(typeof(bool), REF_HASREMOVEDKEY.Ref));
        }

        public void GetValueCodegen(
            CodegenMethod method,
            CodegenClassScope classScope,
            CodegenNamedMethods namedMethods)
        {
            method.Block.DebugStack();
            method.Block.MethodReturn(
                ExprDotMethod(REF_CURRENTROW, "GetValue", REF_COLUMN, REF_EPS, REF_ISNEWDATA, REF_EXPREVALCONTEXT));
        }

        public void GetCollectionOfEventsCodegen(
            CodegenMethod method,
            CodegenClassScope classScope,
            CodegenNamedMethods namedMethods)
        {
            method.Block.MethodReturn(
                ExprDotMethod(
                    REF_CURRENTROW,
                    "GetCollectionOfEvents",
                    REF_COLUMN,
                    REF_EPS,
                    REF_ISNEWDATA,
                    REF_EXPREVALCONTEXT));
        }

        public void GetEventBeanCodegen(
            CodegenMethod method,
            CodegenClassScope classScope,
            CodegenNamedMethods namedMethods)
        {
            method.Block.MethodReturn(
                ExprDotMethod(REF_CURRENTROW, "GetEventBean", REF_COLUMN, REF_EPS, REF_ISNEWDATA, REF_EXPREVALCONTEXT));
        }

        public void GetCollectionScalarCodegen(
            CodegenMethod method,
            CodegenClassScope classScope,
            CodegenNamedMethods namedMethods)
        {
            method.Block.MethodReturn(
                ExprDotMethod(
                    REF_CURRENTROW,
                    "GetCollectionScalar",
                    REF_COLUMN,
                    REF_EPS,
                    REF_ISNEWDATA,
                    REF_EXPREVALCONTEXT));
        }

        public void ApplyEnterCodegen(
            CodegenMethod method,
            CodegenClassScope classScope,
            CodegenNamedMethods namedMethods,
            AggregationClassNames classNames)
        {
            ApplyCodegen(true, method, classScope, classNames);
        }

        public void ApplyLeaveCodegen(
            CodegenMethod method,
            CodegenClassScope classScope,
            CodegenNamedMethods namedMethods,
            AggregationClassNames classNames)
        {
            ApplyCodegen(false, method, classScope, classNames);
        }

        public void StopMethodCodegen(
            AggregationServiceFactoryForgeWMethodGen forge,
            CodegenMethod method)
        {
            // no action
        }

        public void SetRemovedCallbackCodegen(CodegenMethod method)
        {
            // no action
        }

        public void SetCurrentAccessCodegen(
            CodegenMethod method,
            CodegenClassScope classScope,
            AggregationClassNames classNames)
        {
            method.Block.IfCondition(ExprDotName(AggregationServiceCodegenNames.REF_ROLLUPLEVEL, "IsAggregationTop"))
                .AssignRef(REF_CURRENTROW, REF_AGGREGATORTOPGROUP)
                .IfElse()
                .AssignRef(
                    REF_CURRENTROW,
                    Cast(
                        typeof(AggregationRow),
                        ExprDotMethod(
                            ArrayAtIndex(
                                REF_AGGREGATORSPERGROUP,
                                ExprDotName(AggregationServiceCodegenNames.REF_ROLLUPLEVEL, "AggregationOffset")),
                            "Get",
                            AggregationServiceCodegenNames.REF_GROUPKEY)))
                .IfCondition(EqualsNull(REF_CURRENTROW))
                .AssignRef(REF_CURRENTROW, NewInstance(classNames.RowTop, Ref("o")))
                .BlockEnd()
                .BlockEnd()
                .AssignRef(REF_CURRENTGROUPKEY, AggregationServiceCodegenNames.REF_GROUPKEY);
        }

        public void ClearResultsCodegen(
            CodegenMethod method,
            CodegenClassScope classScope)
        {
            method.Block.ExprDotMethod(REF_AGGREGATORTOPGROUP, "Clear");
            for (var i = 0; i < rollupDesc.NumLevelsAggregation; i++) {
                method.Block.ExprDotMethod(ArrayAtIndex(REF_AGGREGATORSPERGROUP, Constant(i)), "Clear");
            }
        }

        public void AcceptCodegen(
            CodegenMethod method,
            CodegenClassScope classScope)
        {
            method.Block.ExprDotMethod(
                REF_AGGVISITOR,
                "VisitAggregations",
                GetGroupKeyCountCodegen(method, classScope),
                REF_AGGREGATORSPERGROUP,
                REF_AGGREGATORTOPGROUP);
        }

        public void GetGroupKeysCodegen(
            CodegenMethod method,
            CodegenClassScope classScope)
        {
            method.Block.MethodThrowUnsupported();
        }

        public void GetGroupKeyCodegen(
            CodegenMethod method,
            CodegenClassScope classScope)
        {
            method.Block.MethodReturn(REF_CURRENTGROUPKEY);
        }

        public void AcceptGroupDetailCodegen(
            CodegenMethod method,
            CodegenClassScope classScope)
        {
            method.Block.ExprDotMethod(REF_AGGVISITOR, "VisitGrouped", GetGroupKeyCountCodegen(method, classScope))
                .ForEach(typeof(IDictionary<object, object>), "anAggregatorsPerGroup", REF_AGGREGATORSPERGROUP)
                .ForEach(
                    typeof(KeyValuePair<object, object>),
                    "entry",
                    Ref("anAggregatorsPerGroup"))
                .ExprDotMethod(
                    REF_AGGVISITOR,
                    "VisitGroup",
                    ExprDotName(Ref("entry"), "Key"),
                    ExprDotName(Ref("entry"), "Value"))
                .BlockEnd()
                .BlockEnd()
                .ExprDotMethod(
                    REF_AGGVISITOR,
                    "VisitGroup",
                    PublicConstValue(typeof(CollectionUtil), "OBJECTARRAY_EMPTY"),
                    REF_AGGREGATORTOPGROUP);
        }

        public void IsGroupedCodegen(
            CodegenProperty property,
            CodegenClassScope classScope)
        {
            property.GetterBlock.BlockReturn(ConstantTrue());
        }

        public void RowWriteMethodCodegen(
            CodegenMethod method,
            int level)
        {
            method.Block.ExprDotMethod(Ref("output"), "WriteInt", Ref("row.refcount"));
        }

        public void RowReadMethodCodegen(
            CodegenMethod method,
            int level)
        {
            method.Block.AssignRef("row.refcount", ExprDotMethod(Ref("input"), "ReadInt"));
        }

        private CodegenExpression GetGroupKeyCountCodegen(
            CodegenMethodScope parent,
            CodegenClassScope classScope)
        {
            var method = parent.MakeChild(typeof(int), typeof(AggSvcGroupByRollupForge), classScope);
            method.Block.DeclareVar<int>("size", Constant(1));
            for (var i = 0; i < rollupDesc.NumLevelsAggregation; i++) {
                method.Block.AssignCompound(
                    "size",
                    "+",
                    ExprDotName(ArrayAtIndex(REF_AGGREGATORSPERGROUP, Constant(i)), "Count"));
            }

            method.Block.MethodReturn(Ref("size"));
            return LocalMethod(method);
        }

        private void ApplyCodegen(
            bool enter,
            CodegenMethod method,
            CodegenClassScope classScope,
            AggregationClassNames classNames)
        {
            if (enter) {
                method.Block.InstanceMethod(HandleRemovedKeysCodegen(method, classScope));
            }

            method.Block.DeclareVar<object[]>(
                "groupKeyPerLevel",
                Cast(typeof(object[]), AggregationServiceCodegenNames.REF_GROUPKEY));
            for (var i = 0; i < rollupDesc.NumLevels; i++) {
                var level = rollupDesc.Levels[i];
                var groupKeyName = "groupKey_" + i;
                method.Block.DeclareVar<object>(
                    groupKeyName,
                    ArrayAtIndex(Ref("groupKeyPerLevel"), Constant(i)));

                if (level.IsAggregationTop) {
                    method.Block.AssignRef(REF_CURRENTROW, REF_AGGREGATORTOPGROUP)
                        .ExprDotMethod(REF_CURRENTROW, enter ? "IncreaseRefcount" : "DecreaseRefcount");
                }
                else {
                    if (enter) {
                        method.Block.AssignRef(
                                REF_CURRENTROW,
                                Cast(
                                    typeof(AggregationRow),
                                    ExprDotMethod(
                                        ArrayAtIndex(REF_AGGREGATORSPERGROUP, Constant(level.AggregationOffset)),
                                        "Get",
                                        Ref(groupKeyName))))
                            .IfCondition(EqualsNull(REF_CURRENTROW))
                            .AssignRef(REF_CURRENTROW, NewInstance(classNames.RowTop, Ref("o")))
                            .ExprDotMethod(
                                ArrayAtIndex(REF_AGGREGATORSPERGROUP, Constant(level.AggregationOffset)),
                                "Put",
                                Ref(groupKeyName),
                                REF_CURRENTROW)
                            .BlockEnd()
                            .ExprDotMethod(REF_CURRENTROW, "IncreaseRefcount");
                    }
                    else {
                        method.Block.AssignRef(
                                REF_CURRENTROW,
                                Cast(
                                    typeof(AggregationRow),
                                    ExprDotMethod(
                                        ArrayAtIndex(REF_AGGREGATORSPERGROUP, Constant(level.AggregationOffset)),
                                        "Get",
                                        Ref(groupKeyName))))
                            .IfCondition(EqualsNull(REF_CURRENTROW))
                            .AssignRef(REF_CURRENTROW, NewInstance(classNames.RowTop, Ref("o")))
                            .ExprDotMethod(
                                ArrayAtIndex(REF_AGGREGATORSPERGROUP, Constant(level.AggregationOffset)),
                                "Put",
                                Ref(groupKeyName),
                                REF_CURRENTROW)
                            .BlockEnd()
                            .ExprDotMethod(REF_CURRENTROW, "DecreaseRefcount");
                    }
                }

                method.Block.ExprDotMethod(
                    REF_CURRENTROW,
                    enter ? "ApplyEnter" : "ApplyLeave",
                    REF_EPS,
                    REF_EXPREVALCONTEXT);

                if (!enter && !level.IsAggregationTop) {
                    var ifCanDelete = method.Block.IfCondition(
                        Relational(ExprDotMethod(REF_CURRENTROW, "GetRefcount"), LE, Constant(0)));
                    ifCanDelete.AssignRef(REF_HASREMOVEDKEY, ConstantTrue());
                    if (!level.IsAggregationTop) {
                        var removedKeyForLevel = ArrayAtIndex(REF_REMOVEDKEYS, Constant(level.AggregationOffset));
                        ifCanDelete.ExprDotMethod(removedKeyForLevel, "Add", Ref(groupKeyName));
                    }
                }
            }
        }

        private CodegenMethod HandleRemovedKeysCodegen(
            CodegenMethod scope,
            CodegenClassScope classScope)
        {
            var method = scope.MakeChild(typeof(void), GetType(), classScope);
            method.Block.IfCondition(Not(REF_HASREMOVEDKEY))
                .BlockReturnNoValue()
                .AssignRef(REF_HASREMOVEDKEY, ConstantFalse())
                .ForLoopIntSimple("i", ArrayLength(REF_REMOVEDKEYS))
                .IfCondition(ExprDotMethod(ArrayAtIndex(REF_REMOVEDKEYS, Ref("i")), "IsEmpty"))
                .BlockContinue()
                .ForEach(typeof(object), "removedKey", ArrayAtIndex(REF_REMOVEDKEYS, Ref("i")))
                .ExprDotMethod(ArrayAtIndex(REF_AGGREGATORSPERGROUP, Ref("i")), "Remove", Ref("removedKey"))
                .BlockEnd()
                .ExprDotMethod(ArrayAtIndex(REF_REMOVEDKEYS, Ref("i")), "Clear");
            return method;
        }
    }
} // end of namespace