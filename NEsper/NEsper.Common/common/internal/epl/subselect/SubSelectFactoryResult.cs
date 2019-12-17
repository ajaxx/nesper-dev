///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using com.espertech.esper.common.@internal.context.activator;
using com.espertech.esper.common.@internal.epl.agg.core;
using com.espertech.esper.common.@internal.epl.expression.prior;
using com.espertech.esper.common.@internal.epl.index.@base;
using com.espertech.esper.common.@internal.epl.lookup;
using com.espertech.esper.common.@internal.view.core;
using com.espertech.esper.common.@internal.view.previous;
using com.espertech.esper.compat;
using com.espertech.esper.compat.collections;

namespace com.espertech.esper.common.@internal.epl.subselect
{
    public class SubSelectFactoryResult
    {
        private readonly ViewableActivationResult subselectActivationResult;
        private readonly SubordTableLookupStrategy lookupStrategy;
        private readonly SubselectAggregationPreprocessorBase subselectAggregationPreprocessor;
        private readonly AggregationService aggregationService;
        private readonly PriorEvalStrategy priorStrategy;
        private readonly PreviousGetterStrategy previousStrategy;
        private readonly Viewable subselectView;
        private readonly EventTable[] indexes;

        public SubSelectFactoryResult(
            ViewableActivationResult subselectActivationResult,
            SubSelectStrategyRealization realization,
            SubordTableLookupStrategy lookupStrategy)
        {
            this.subselectActivationResult = subselectActivationResult;
            this.lookupStrategy = lookupStrategy;
            this.subselectAggregationPreprocessor = realization.SubselectAggregationPreprocessor;
            this.aggregationService = realization.AggregationService;
            this.priorStrategy = realization.PriorStrategy;
            this.previousStrategy = realization.PreviousStrategy;
            this.subselectView = realization.SubselectView;
            this.indexes = realization.Indexes;
        }

        public ViewableActivationResult SubselectActivationResult {
            get => subselectActivationResult;
        }

        public SubordTableLookupStrategy LookupStrategy {
            get => lookupStrategy;
        }

        public SubselectAggregationPreprocessorBase SubselectAggregationPreprocessor {
            get => subselectAggregationPreprocessor;
        }

        public AggregationService AggregationService {
            get => aggregationService;
        }

        public PriorEvalStrategy PriorStrategy {
            get => priorStrategy;
        }

        public PreviousGetterStrategy PreviousStrategy {
            get => previousStrategy;
        }

        public Viewable SubselectView {
            get => subselectView;
        }

        public EventTable[] GetIndexes()
        {
            return indexes;
        }
    }
} // end of namespace