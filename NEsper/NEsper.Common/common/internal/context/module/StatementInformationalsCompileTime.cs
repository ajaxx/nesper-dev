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
using com.espertech.esper.common.client.annotation;
using com.espertech.esper.common.client.dataflow.core;
using com.espertech.esper.common.client.util;
using com.espertech.esper.common.@internal.bytecodemodel.@base;
using com.espertech.esper.common.@internal.bytecodemodel.core;
using com.espertech.esper.common.@internal.bytecodemodel.model.expression;
using com.espertech.esper.common.@internal.context.util;
using com.espertech.esper.common.@internal.epl.expression.core;
using com.espertech.esper.common.@internal.epl.pattern.core;
using com.espertech.esper.common.@internal.filterspec;
using com.espertech.esper.common.@internal.metrics.audit;
using com.espertech.esper.common.@internal.metrics.instrumentation;
using com.espertech.esper.common.@internal.schedule;
using com.espertech.esper.common.@internal.util;
using com.espertech.esper.common.@internal.view.core;
using com.espertech.esper.compat;
using com.espertech.esper.compat.collections;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using static com.espertech.esper.common.@internal.bytecodemodel.model.expression.CodegenExpressionBuilder;
using static com.espertech.esper.common.@internal.epl.annotation.AnnotationUtil;
using static com.espertech.esper.common.@internal.epl.expression.codegen.ExprForgeCodegenNames;
using static com.espertech.esper.common.@internal.epl.resultset.codegen.ResultSetProcessorCodegenNames;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace com.espertech.esper.common.@internal.context.module
{
    public class StatementInformationalsCompileTime
    {
        private readonly bool _allowSubscriber;
        private readonly bool _alwaysSynthesizeOutputEvents; // set when insert-into/for-clause/select-distinct
        private readonly Attribute[] _annotations;
        private readonly bool _canSelfJoin;
        private readonly bool _forClauseDelivery;
        private readonly ExprNode[] _groupDelivery;
        private readonly bool _hasMatchRecognize;
        private readonly bool _hasSubquery;
        private readonly bool _hasTableAccess;
        private readonly bool _hasVariables;
        private readonly string _insertIntoLatchName;
        private readonly bool _instrumented;
        private readonly bool _needDedup;
        private readonly int _numFilterCallbacks;
        private readonly int _numNamedWindowCallbacks;
        private readonly int _numScheduleCallbacks;
        private readonly string _optionalContextModuleName;
        private readonly string _optionalContextName;
        private readonly NameAccessModifier? _optionalContextVisibility;
        private readonly CodegenNamespaceScope _namespaceScope;
        private readonly bool _preemptive;
        private readonly int _priority;
        private readonly IDictionary<StatementProperty, object> _properties;
        private readonly string[] _selectClauseColumnNames;
        private readonly Type[] _selectClauseTypes;
        private readonly bool _stateless;
        private readonly string _statementNameCompileTime;
        private readonly StatementType _statementType;
        private readonly object _userObjectCompileTime;
        private readonly bool _writesToTables;

        public StatementInformationalsCompileTime(
            string statementNameCompileTime,
            bool alwaysSynthesizeOutputEvents,
            string optionalContextName,
            string optionalContextModuleName,
            NameAccessModifier? optionalContextVisibility,
            bool canSelfJoin,
            bool hasSubquery,
            bool needDedup,
            Attribute[] annotations,
            bool stateless,
            object userObjectCompileTime,
            int numFilterCallbacks,
            int numScheduleCallbacks,
            int numNamedWindowCallbacks,
            StatementType statementType,
            int priority,
            bool preemptive,
            bool hasVariables,
            bool writesToTables,
            bool hasTableAccess,
            Type[] selectClauseTypes,
            string[] selectClauseColumnNames,
            bool forClauseDelivery,
            ExprNode[] groupDelivery,
            IDictionary<StatementProperty, object> properties,
            bool hasMatchRecognize,
            bool instrumented,
            CodegenNamespaceScope namespaceScope,
            string insertIntoLatchName,
            bool allowSubscriber)
        {
            _statementNameCompileTime = statementNameCompileTime;
            _alwaysSynthesizeOutputEvents = alwaysSynthesizeOutputEvents;
            _optionalContextName = optionalContextName;
            _optionalContextModuleName = optionalContextModuleName;
            _optionalContextVisibility = optionalContextVisibility;
            _canSelfJoin = canSelfJoin;
            _hasSubquery = hasSubquery;
            _needDedup = needDedup;
            _annotations = annotations;
            _stateless = stateless;
            _userObjectCompileTime = userObjectCompileTime;
            _numFilterCallbacks = numFilterCallbacks;
            _numScheduleCallbacks = numScheduleCallbacks;
            _numNamedWindowCallbacks = numNamedWindowCallbacks;
            _statementType = statementType;
            _priority = priority;
            _preemptive = preemptive;
            _hasVariables = hasVariables;
            _writesToTables = writesToTables;
            _hasTableAccess = hasTableAccess;
            _selectClauseTypes = selectClauseTypes;
            _selectClauseColumnNames = selectClauseColumnNames;
            _forClauseDelivery = forClauseDelivery;
            _groupDelivery = groupDelivery;
            _properties = properties;
            _hasMatchRecognize = hasMatchRecognize;
            _instrumented = instrumented;
            _namespaceScope = namespaceScope;
            _insertIntoLatchName = insertIntoLatchName;
            _allowSubscriber = allowSubscriber;
        }

        public CodegenExpression Make(
            CodegenMethodScope parent,
            CodegenClassScope classScope)
        {
            var method = parent.MakeChild(typeof(StatementInformationalsRuntime), GetType(), classScope);
            var info = Ref("info");
            method.Block
                .DeclareVar<StatementInformationalsRuntime>(
                    info.Ref,
                    NewInstance(typeof(StatementInformationalsRuntime)))
                .SetProperty(info, "StatementNameCompileTime", Constant(_statementNameCompileTime))
                .SetProperty(info, "IsAlwaysSynthesizeOutputEvents", Constant(_alwaysSynthesizeOutputEvents))
                .SetProperty(info, "OptionalContextName", Constant(_optionalContextName))
                .SetProperty(info, "OptionalContextModuleName", Constant(_optionalContextModuleName))
                .SetProperty(info, "OptionalContextVisibility", Constant(_optionalContextVisibility))
                .SetProperty(info, "IsCanSelfJoin", Constant(_canSelfJoin))
                .SetProperty(info, "HasSubquery", Constant(_hasSubquery))
                .SetProperty(info, "IsNeedDedup", Constant(_needDedup))
                .SetProperty(info, "IsStateless", Constant(_stateless))
                .SetProperty(
                    info,
                    "Annotations",
                    _annotations == null
                        ? ConstantNull()
                        : LocalMethod(MakeAnnotations(typeof(Attribute[]), _annotations, method, classScope)))
                .SetProperty(
                    info,
                    "UserObjectCompileTime",
                    SerializerUtil.ExpressionForUserObject(_userObjectCompileTime))
                .SetProperty(info, "NumFilterCallbacks", Constant(_numFilterCallbacks))
                .SetProperty(info, "NumScheduleCallbacks", Constant(_numScheduleCallbacks))
                .SetProperty(info, "NumNamedWindowCallbacks", Constant(_numNamedWindowCallbacks))
                .SetProperty(info, "StatementType", Constant(_statementType))
                .SetProperty(info, "Priority", Constant(_priority))
                .SetProperty(info, "IsPreemptive", Constant(_preemptive))
                .SetProperty(info, "HasVariables", Constant(_hasVariables))
                .SetProperty(info, "IsWritesToTables", Constant(_writesToTables))
                .SetProperty(info, "HasTableAccess", Constant(_hasTableAccess))
                .SetProperty(info, "SelectClauseTypes", Constant(_selectClauseTypes))
                .SetProperty(info, "SelectClauseColumnNames", Constant(_selectClauseColumnNames))
                .SetProperty(info, "IsForClauseDelivery", Constant(_forClauseDelivery))
                .SetProperty(
                    info,
                    "GroupDeliveryEval",
                    _groupDelivery == null
                        ? ConstantNull()
                        : ExprNodeUtilityCodegen.CodegenEvaluatorMayMultiKeyWCoerce(
                            ExprNodeUtilityQuery.GetForges(_groupDelivery),
                            null,
                            method,
                            GetType(),
                            classScope))
                .SetProperty(info, "Properties", MakeProperties(_properties, method, classScope))
                .SetProperty(info, "HasMatchRecognize", Constant(_hasMatchRecognize))
                .SetProperty(info, "AuditProvider", MakeAuditProvider(method, classScope))
                .SetProperty(info, "IsInstrumented", Constant(_instrumented))
                .SetProperty(info, "InstrumentationProvider", MakeInstrumentationProvider(method, classScope))
                .SetProperty(info, "SubstitutionParamTypes", MakeSubstitutionParamTypes())
                .SetProperty(info, "SubstitutionParamNames", MakeSubstitutionParamNames(method, classScope))
                .SetProperty(info, "InsertIntoLatchName", Constant(_insertIntoLatchName))
                .SetProperty(info, "IsAllowSubscriber", Constant(_allowSubscriber))
                .MethodReturn(info);
            return LocalMethod(method);
        }

        private CodegenExpression MakeSubstitutionParamTypes()
        {
            var numbered = _namespaceScope.SubstitutionParamsByNumber;
            var named = _namespaceScope.SubstitutionParamsByName;
            if (numbered.IsEmpty() && named.IsEmpty()) {
                return ConstantNull();
            }

            if (!numbered.IsEmpty() && !named.IsEmpty()) {
                throw new IllegalStateException("Both named and numbered substitution parameters are non-empty");
            }

            Type[] types;
            if (!numbered.IsEmpty()) {
                types = new Type[numbered.Count];
                for (var i = 0; i < numbered.Count; i++) {
                    types[i] = numbered[i].Type;
                }
            }
            else {
                types = new Type[named.Count];
                var count = 0;
                foreach (var entry in named) {
                    types[count++] = entry.Value.Type;
                }
            }

            return Constant(types);
        }

        private CodegenExpression MakeSubstitutionParamNames(
            CodegenMethodScope parent,
            CodegenClassScope classScope)
        {
            var named = _namespaceScope.SubstitutionParamsByName;
            if (named.IsEmpty()) {
                return ConstantNull();
            }

            var method = parent.MakeChild(typeof(IDictionary<string, int>), GetType(), classScope);
            method.Block.DeclareVar<IDictionary<string, int>>(
                "names",
                NewInstance(typeof(Dictionary<string, int>)));
            var count = 1;
            foreach (var entry in named) {
                method.Block.ExprDotMethod(Ref("names"), "Put", Constant(entry.Key), Constant(count++));
            }

            method.Block.MethodReturn(Ref("names"));
            return LocalMethod(method);
        }

        private CodegenExpression MakeInstrumentationProvider(
            CodegenMethod method,
            CodegenClassScope classScope)
        {
            if (!_instrumented) {
                return ConstantNull();
            }

            var instrumentation = Ref("instrumentation");
            method.Block.AssignRef(instrumentation, NewInstance<ProxyInstrumentationCommon>());
            
            //var anonymousClass = NewAnonymousClass(
            //    method.Block,
            //    typeof(InstrumentationCommon));

            //var activated = CodegenMethod.MakeMethod(typeof(bool), GetType(), classScope);
            //activated.Block.MethodReturn(ConstantTrue());

            method.Block.SetProperty(
                instrumentation,
                "ProcActivated",
                new CodegenExpressionLambda(method.Block)
                    .WithBody(
                        block => block.BlockReturn(
                            ConstantTrue())));

            foreach (var forwarded in typeof(InstrumentationCommon).GetMethods()) {
                if (forwarded.DeclaringType == typeof(object)) {
                    continue;
                }

                if (forwarded.Name == "Activated") {
                    continue;
                }

                IList<CodegenNamedParam> @params = new List<CodegenNamedParam>();
                var forwardedParameters = forwarded.GetParameters();
                var expressions = new CodegenExpression[forwardedParameters.Length];

                var num = 0;
                foreach (var param in forwardedParameters) {
                    @params.Add(new CodegenNamedParam(param.ParameterType, param.Name));
                    expressions[num] = Ref(param.Name);
                    num++;
                }

                //var m = CodegenMethod.MakeMethod(typeof(void), GetType(), classScope)
                //    .AddParam(@params);

                // Now we need a lambda to associate with the instrumentation and tie them together
                var proc = $"Proc{forwarded.Name}";

                method.Block.SetProperty(
                    instrumentation,
                    "ProcActivated",
                    new CodegenExpressionLambda(method.Block)
                        .WithParams(@params)
                        .WithBody(
                            block => block
                                .Apply(
                                    InstrumentationCode.Instblock(
                                        classScope,
                                        forwarded.Name,
                                        expressions))));

                //instrumentation.AddMethod(forwarded.Name, m);
            }

            return instrumentation;
        }

        private CodegenExpression MakeAuditProvider(
            CodegenMethod method,
            CodegenClassScope classScope)
        {
            if (FindAnnotation(_annotations, typeof(AuditAttribute)) == null) {
                return PublicConstValue(typeof(AuditProviderDefault), "INSTANCE");
            }

            var auditProviderVar = method.Block.DeclareVar<ProxyAuditProvider>(
                "auditProvider",
                NewInstance<ProxyAuditProvider>());

            //var auditProvider = NewAnonymousClass(method.Block, typeof(ProxyAuditProvider));

            //var activated = CodegenMethod.MakeMethod(typeof(bool), GetType(), classScope);
            //auditProvider.AddMethod("Activated", activated);
            //activated.Block.MethodReturn(ConstantTrue());

            method.Block.SetProperty(
                Ref("auditProvider"),
                "ProcActivated",
                new CodegenExpressionLambda(method.Block)
                    .WithBody(block => block.BlockReturn(ConstantTrue())));

            method.Block.SetProperty(
                Ref("auditProvider"),
                "ProcView",
                new CodegenExpressionLambda(method.Block)
                    .WithParam<EventBean[]>("newData")
                    .WithParam<EventBean[]>("oldData")
                    .WithParam<AgentInstanceContext>(REF_AGENTINSTANCECONTEXT.Ref)
                    .WithParam<ViewFactory>("viewFactory")
                    .WithBody(
                        block => block.StaticMethod(
                            typeof(AuditPath),
                            "AuditView",
                            Ref("newData"),
                            Ref("oldData"),
                            REF_AGENTINSTANCECONTEXT,
                            Ref("viewFactory"))));

            method.Block.SetProperty(
                Ref("auditProvider"),
                "ProcStreamSingle",
                new CodegenExpressionLambda(method.Block)
                    .WithParam<EventBean>("@event")
                    .WithParam<ExprEvaluatorContext>(REF_EXPREVALCONTEXT.Ref)
                    .WithParam<string>("filterText")
                    .WithBody(
                        block => {
                            if (AuditEnum.STREAM.GetAudit(_annotations) != null) {
                                block.StaticMethod(
                                    typeof(AuditPath),
                                    "AuditStream",
                                    Ref("@event"),
                                    REF_EXPREVALCONTEXT,
                                    Ref("filterText"));
                            }
                        }));

            method.Block.SetProperty(
                Ref("auditProvider"),
                "ProcStreamMulti",
                new CodegenExpressionLambda(method.Block)
                    .WithParam<EventBean[]>("newData")
                    .WithParam<EventBean[]>("oldData")
                    .WithParam<ExprEvaluatorContext>(REF_EXPREVALCONTEXT.Ref)
                    .WithParam<string>("filterText")
                    .WithBody(
                        block => {
                            if (AuditEnum.STREAM.GetAudit(_annotations) != null) {
                                block.StaticMethod(
                                    typeof(AuditPath),
                                    "AuditStream",
                                    Ref("newData"),
                                    Ref("oldData"),
                                    REF_EXPREVALCONTEXT,
                                    Ref("filterText"));
                            }
                        }
                    ));

            method.Block.SetProperty(
                Ref("auditProvider"),
                "ProcScheduleAdd",
                new CodegenExpressionLambda(method.Block)
                    .WithParam<long>("time")
                    .WithParam<AgentInstanceContext>(REF_AGENTINSTANCECONTEXT.Ref)
                    .WithParam<ScheduleHandle>("scheduleHandle")
                    .WithParam<ScheduleObjectType>("type")
                    .WithParam<string>("name")
                    .WithBody(
                        block => {
                            if (AuditEnum.SCHEDULE.GetAudit(_annotations) != null) {
                                block.StaticMethod(
                                    typeof(AuditPath),
                                    "AuditScheduleAdd",
                                    Ref("time"),
                                    REF_AGENTINSTANCECONTEXT,
                                    Ref("scheduleHandle"),
                                    Ref("type"),
                                    Ref("name"));
                            }
                        }));

            method.Block.SetProperty(
                Ref("auditProvider"),
                "ProcScheduleRemove",
                new CodegenExpressionLambda(method.Block)
                    .WithParam<AgentInstanceContext>(REF_AGENTINSTANCECONTEXT.Ref)
                    .WithParam<ScheduleHandle>("scheduleHandle")
                    .WithParam<ScheduleObjectType>("type")
                    .WithParam<string>("name")
                    .WithBody(
                        block => {
                            if (AuditEnum.SCHEDULE.GetAudit(_annotations) != null) {
                                block.StaticMethod(
                                    typeof(AuditPath),
                                    "AuditScheduleRemove",
                                    REF_AGENTINSTANCECONTEXT,
                                    Ref("scheduleHandle"),
                                    Ref("type"),
                                    Ref("name"));
                            }
                        }));

            method.Block.SetProperty(
                Ref("auditProvider"),
                "ProcScheduleFire",
                new CodegenExpressionLambda(method.Block)
                    .WithParam<AgentInstanceContext>(REF_AGENTINSTANCECONTEXT.Ref)
                    .WithParam<ScheduleObjectType>("type")
                    .WithParam<string>("name")
                    .WithBody(
                        block => {
                            if (AuditEnum.SCHEDULE.GetAudit(_annotations) != null) {
                                block.StaticMethod(
                                    typeof(AuditPath),
                                    "AuditScheduleFire",
                                    REF_AGENTINSTANCECONTEXT,
                                    Ref("type"),
                                    Ref("name"));
                            }
                        }));

            method.Block.SetProperty(
                Ref("auditProvider"),
                "ProcProperty",
                new CodegenExpressionLambda(method.Block)
                    .WithParam<string>("name")
                    .WithParam<object>("value")
                    .WithParam<ExprEvaluatorContext>(REF_EXPREVALCONTEXT.Ref)
                    .WithBody(
                        block => {
                            if (AuditEnum.PROPERTY.GetAudit(_annotations) != null) {
                                block.StaticMethod(
                                    typeof(AuditPath),
                                    "AuditProperty",
                                    Ref("name"),
                                    Ref("value"),
                                    REF_EXPREVALCONTEXT);
                            }
                        }));

            method.Block.SetProperty(
                Ref("auditProvider"),
                "ProcInsert",
                new CodegenExpressionLambda(method.Block)
                    .WithParam<EventBean>("@event")
                    .WithParam<ExprEvaluatorContext>(REF_EXPREVALCONTEXT.Ref)
                    .WithBody(
                        block => {
                            if (AuditEnum.INSERT.GetAudit(_annotations) != null) {
                                block.StaticMethod(
                                    typeof(AuditPath),
                                    "AuditInsert",
                                    Ref("@event"),
                                    REF_EXPREVALCONTEXT);
                            }
                        }));

            method.Block.SetProperty(
                Ref("auditProvider"),
                "ProcExpression",
                new CodegenExpressionLambda(method.Block)
                    .WithParam<string>("text")
                    .WithParam<object>("value")
                    .WithParam<ExprEvaluatorContext>(REF_EXPREVALCONTEXT.Ref)
                    .WithBody(
                        block => {
                            if (AuditEnum.EXPRESSION.GetAudit(_annotations) != null ||
                                AuditEnum.EXPRESSION_NESTED.GetAudit(_annotations) != null) {
                                block.StaticMethod(
                                    typeof(AuditPath),
                                    "AuditExpression",
                                    Ref("text"),
                                    Ref("value"),
                                    REF_EXPREVALCONTEXT);
                            }
                        }));

            method.Block.SetProperty(
                Ref("auditProvider"),
                "ProcPatternTrue",
                new CodegenExpressionLambda(method.Block)
                    .WithParam<EvalFactoryNode>("factoryNode")
                    .WithParam<object>("from")
                    .WithParam<MatchedEventMapMinimal>("matchEvent")
                    .WithParam<bool>("isQuitted")
                    .WithParam<AgentInstanceContext>(NAME_AGENTINSTANCECONTEXT)
                    .WithBody(
                        block => {
                            if (AuditEnum.PATTERN.GetAudit(_annotations) != null) {
                                block.StaticMethod(
                                    typeof(AuditPath),
                                    "AuditPatternTrue",
                                    Ref("factoryNode"),
                                    Ref("from"),
                                    Ref("matchEvent"),
                                    Ref("isQuitted"),
                                    REF_AGENTINSTANCECONTEXT);
                            }
                        }));

            method.Block.SetProperty(
                Ref("auditProvider"),
                "ProcPatternFalse",
                new CodegenExpressionLambda(method.Block)
                    .WithParam<EvalFactoryNode>("factoryNode")
                    .WithParam<object>("from")
                    .WithParam<AgentInstanceContext>(NAME_AGENTINSTANCECONTEXT)
                    .WithBody(
                        block => {
                            if (AuditEnum.PATTERN.GetAudit(_annotations) != null) {
                                block.StaticMethod(
                                    typeof(AuditPath),
                                    "AuditPatternFalse",
                                    Ref("factoryNode"),
                                    Ref("from"),
                                    REF_AGENTINSTANCECONTEXT);
                            }
                        }));

            method.Block.SetProperty(
                Ref("auditProvider"),
                "ProcPatternInstance",
                new CodegenExpressionLambda(method.Block)
                    .WithParam<bool>("increase")
                    .WithParam<EvalFactoryNode>("factoryNode")
                    .WithParam<AgentInstanceContext>(NAME_AGENTINSTANCECONTEXT)
                    .WithBody(
                        block => {
                            if (AuditEnum.PATTERNINSTANCES.GetAudit(_annotations) != null) {
                                block.StaticMethod(
                                    typeof(AuditPath),
                                    "AuditPatternInstance",
                                    Ref("increase"),
                                    Ref("factoryNode"),
                                    REF_AGENTINSTANCECONTEXT);
                            }
                        }));

            method.Block.SetProperty(
                Ref("auditProvider"),
                "ProcExprdef",
                new CodegenExpressionLambda(method.Block)
                    .WithParam<string>("name")
                    .WithParam<object>("value")
                    .WithParam<ExprEvaluatorContext>(REF_EXPREVALCONTEXT.Ref)
                    .WithBody(
                        block => {
                            if (AuditEnum.EXPRDEF.GetAudit(_annotations) != null) {
                                block.StaticMethod(
                                    typeof(AuditPath),
                                    "AuditExprDef",
                                    Ref("name"),
                                    Ref("value"),
                                    REF_EXPREVALCONTEXT);
                            }
                        }));

            method.Block.SetProperty(
                Ref("auditProvider"),
                "ProcDataflowTransition",
                new CodegenExpressionLambda(method.Block)
                    .WithParam<string>("name")
                    .WithParam<string>("instance")
                    .WithParam<EPDataFlowState>("state")
                    .WithParam<EPDataFlowState>("newState")
                    .WithParam<AgentInstanceContext>(REF_AGENTINSTANCECONTEXT.Ref)
                    .WithBody(
                        block => {
                            if (AuditEnum.DATAFLOW_TRANSITION.GetAudit(_annotations) != null) {
                                block.StaticMethod(
                                    typeof(AuditPath),
                                    "AuditDataflowTransition",
                                    Ref("name"),
                                    Ref("instance"),
                                    Ref("state"),
                                    Ref("newState"),
                                    REF_AGENTINSTANCECONTEXT);
                            }
                        }));

            method.Block.SetProperty(
                Ref("auditProvider"),
                "ProcDataflowSource",
                new CodegenExpressionLambda(method.Block)
                    .WithParam<string>("name")
                    .WithParam<string>("instance")
                    .WithParam<string>("operatorName")
                    .WithParam<int>("operatorNum")
                    .WithParam<AgentInstanceContext>(REF_AGENTINSTANCECONTEXT.Ref)
                    .WithBody(
                        block => {
                            if (AuditEnum.DATAFLOW_SOURCE.GetAudit(_annotations) != null) {
                                block.StaticMethod(
                                    typeof(AuditPath),
                                    "AuditDataflowSource",
                                    Ref("name"),
                                    Ref("instance"),
                                    Ref("operatorName"),
                                    Ref("operatorNum"),
                                    REF_AGENTINSTANCECONTEXT);
                            }
                        }));

            method.Block.SetProperty(
                Ref("auditProvider"),
                "ProcDataflowOp",
                new CodegenExpressionLambda(method.Block)
                    .WithParam<string>("name")
                    .WithParam<string>("instance")
                    .WithParam<string>("operatorName")
                    .WithParam<int>("operatorNum")
                    .WithParam<object[]>("parameters")
                    .WithParam<AgentInstanceContext>(REF_AGENTINSTANCECONTEXT.Ref)
                    .WithBody(
                        block => {
                            if (AuditEnum.DATAFLOW_OP.GetAudit(_annotations) != null) {
                                block.StaticMethod(
                                    typeof(AuditPath),
                                    "AuditDataflowOp",
                                    Ref("name"),
                                    Ref("instance"),
                                    Ref("operatorName"),
                                    Ref("operatorNum"),
                                    Ref("parameters"),
                                    REF_AGENTINSTANCECONTEXT);
                            }
                        }));

            method.Block.SetProperty(
                Ref("auditProvider"),
                "ProcContextPartition",
                new CodegenExpressionLambda(method.Block)
                    .WithParam<bool>("allocate")
                    .WithParam<AgentInstanceContext>(REF_AGENTINSTANCECONTEXT.Ref)
                    .WithBody(
                        block => {
                            if (AuditEnum.CONTEXTPARTITION.GetAudit(_annotations) != null) {
                                block.StaticMethod(
                                    typeof(AuditPath),
                                    "AuditContextPartition",
                                    Ref("allocate"),
                                    REF_AGENTINSTANCECONTEXT);
                            }
                        }));

            return Ref("auditProvider");
        }

        private CodegenExpression MakeProperties(
            IDictionary<StatementProperty, object> properties,
            CodegenMethodScope parent,
            CodegenClassScope classScope)
        {
            if (properties.IsEmpty()) {
                return StaticMethod(typeof(Collections), "GetEmptyDataMap");
            }

            Func<StatementProperty, CodegenExpression> field = x => EnumValue(typeof(StatementProperty), x.GetName());
            Func<object, CodegenExpression> value = Constant;
            if (properties.Count == 1) {
                var first = properties.First();
                return StaticMethod(
                    typeof(Collections),
                    "SingletonMap",
                    field.Invoke(first.Key),
                    Cast(typeof(object), value.Invoke(first.Value)));
            }

            var method = parent.MakeChild(
                typeof(IDictionary<object, object>),
                typeof(StatementInformationalsCompileTime),
                classScope);
            method.Block
                .DeclareVar<IDictionary<StatementProperty, object>>(
                    "properties",
                    NewInstance(typeof(Dictionary<StatementProperty, object>)));
            foreach (var entry in properties) {
                method.Block.ExprDotMethod(
                    Ref("Properties"),
                    "Put",
                    field.Invoke(entry.Key),
                    value.Invoke(entry.Value));
            }

            return LocalMethod(method);
        }
    }
} // end of namespace