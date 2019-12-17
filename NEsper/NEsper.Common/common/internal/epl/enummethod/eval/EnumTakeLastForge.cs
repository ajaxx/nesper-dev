///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using com.espertech.esper.common.@internal.bytecodemodel.@base;
using com.espertech.esper.common.@internal.bytecodemodel.model.expression;
using com.espertech.esper.common.@internal.epl.enummethod.codegen;
using com.espertech.esper.common.@internal.epl.expression.core;
using com.espertech.esper.compat;
using com.espertech.esper.compat.collections;

namespace com.espertech.esper.common.@internal.epl.enummethod.eval
{
    public class EnumTakeLastForge : EnumForge
    {
        internal ExprForge sizeEval;
        internal int numStreams;
        internal bool scalar;

        public EnumTakeLastForge(
            ExprForge sizeEval,
            int numStreams,
            bool scalar)
        {
            this.sizeEval = sizeEval;
            this.numStreams = numStreams;
            this.scalar = scalar;
        }

        public int StreamNumSize {
            get => numStreams;
        }

        public virtual EnumEval EnumEvaluator {
            get => new EnumTakeLastForgeEval(sizeEval.ExprEvaluator);
        }

        public virtual CodegenExpression Codegen(
            EnumForgeCodegenParams premade,
            CodegenMethodScope codegenMethodScope,
            CodegenClassScope codegenClassScope)
        {
            return EnumTakeLastForgeEval.Codegen(this, premade, codegenMethodScope, codegenClassScope);
        }
    }
} // end of namespace