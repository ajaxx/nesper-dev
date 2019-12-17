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
    public class CodegenStatementAssignRef : CodegenStatementBase
    {
        private readonly CodegenExpression assignment;
        private readonly CodegenExpression @ref;

        public CodegenStatementAssignRef(
            CodegenExpression @ref,
            CodegenExpression assignment)
        {
            this.@ref = @ref;
            this.assignment = assignment;
        }

        public override void RenderStatement(
            StringBuilder builder,
            bool isInnerClass)
        {
            var indent = new CodegenIndent(true);
            @ref.Render(builder, isInnerClass, 1, indent);
            builder.Append("=");
            assignment.Render(builder, isInnerClass, 1, indent);
        }

        public override void MergeClasses(ISet<Type> classes)
        {
            assignment.MergeClasses(classes);
        }
    }
} // end of namespace