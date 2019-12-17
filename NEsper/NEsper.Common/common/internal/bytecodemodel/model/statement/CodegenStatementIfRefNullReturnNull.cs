///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;

using com.espertech.esper.common.@internal.bytecodemodel.core;
using com.espertech.esper.common.@internal.bytecodemodel.model.expression;

namespace com.espertech.esper.common.@internal.bytecodemodel.model.statement
{
    public class CodegenStatementIfRefNullReturnNull : CodegenStatement
    {
        private readonly CodegenExpressionRef @ref;

        public CodegenStatementIfRefNullReturnNull(CodegenExpressionRef @ref)
        {
            this.@ref = @ref;
        }

        public void Render(
            StringBuilder builder,
            bool isInnerClass,
            int level,
            CodegenIndent indent)
        {
            builder.Append("if (");
            @ref.Render(builder, isInnerClass, level + 1, indent);
            builder.Append(" == null) {return null;}\n");
        }

        public void MergeClasses(ISet<Type> classes)
        {
        }
    }
} // end of namespace