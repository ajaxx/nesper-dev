///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using com.espertech.esper.common.@internal.bytecodemodel.model.expression;
using com.espertech.esper.compat;
using com.espertech.esper.compat.collections;

using static com.espertech.esper.common.@internal.bytecodemodel.model.expression.CodegenExpressionBuilder;

namespace com.espertech.esper.common.@internal.epl.output.core
{
    public class OutputProcessViewCodegenNames
    {
        public const string NAME_RESULTSETPROCESSOR = "resultSetProcessor";
        public const string NAME_STATEMENTRESULTSVC = "statementResultService";
        public const string NAME_PARENTVIEW = "parentView";
        public const string NAME_JOINEXECSTRATEGY = "joinExecutionStrategy";
        public readonly static CodegenExpressionRef REF_CHILD = Ref("child");

        public readonly static CodegenExpressionRef REF_RESULTSETPROCESSOR =
            new CodegenExpressionRef(NAME_RESULTSETPROCESSOR);
    }
} // end of namespace