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
    public class CodegenStatementAssignArrayElement : CodegenStatementBase
    {
        private readonly CodegenExpression array;
        private readonly CodegenExpression expression;
        private readonly CodegenExpression index;

        public CodegenStatementAssignArrayElement(
            CodegenExpression array,
            CodegenExpression index,
            CodegenExpression expression)
        {
            this.array = array;
            this.index = index;
            this.expression = expression;
        }

        public override void RenderStatement(
            StringBuilder builder,
            bool isInnerClass)
        {
            var indent = new CodegenIndent(true);
            array.Render(builder, isInnerClass, 1, indent);
            builder.Append("[");
            index.Render(builder, isInnerClass, 1, indent);
            builder.Append("]=");
            expression.Render(builder, isInnerClass, 1, indent);
        }

        public override void MergeClasses(ISet<Type> classes)
        {
            array.MergeClasses(classes);
            index.MergeClasses(classes);
            expression.MergeClasses(classes);
        }
    }
} // end of namespace