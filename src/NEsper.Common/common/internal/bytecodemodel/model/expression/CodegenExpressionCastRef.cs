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
using com.espertech.esper.common.@internal.bytecodemodel.util;
using com.espertech.esper.compat.function;

using static com.espertech.esper.common.@internal.bytecodemodel.core.CodeGenerationHelper;

namespace com.espertech.esper.common.@internal.bytecodemodel.model.expression
{
    public class CodegenExpressionCastRef : CodegenExpression
    {
        private readonly Type _clazz;
        private readonly string _ref;

        public CodegenExpressionCastRef(
            Type clazz,
            string @ref)
        {
            _clazz = clazz;
            _ref = @ref;
        }

        public void Render(
            StringBuilder builder,
            bool isInnerClass,
            int level,
            CodegenIndent indent)
        {
            builder.Append("((");
            AppendClassName(builder, _clazz);
            builder.Append(")").Append(_ref).Append(")");
        }

        public void MergeClasses(ISet<Type> classes)
        {
            classes.AddToSet(_clazz);
        }

        public void TraverseExpressions(Consumer<CodegenExpression> consumer)
        {
        }
    }
} // end of namespace