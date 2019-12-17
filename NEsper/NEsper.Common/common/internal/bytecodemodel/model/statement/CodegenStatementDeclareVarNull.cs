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

using com.espertech.esper.common.@internal.bytecodemodel.util;

using static com.espertech.esper.common.@internal.bytecodemodel.core.CodeGenerationHelper;

namespace com.espertech.esper.common.@internal.bytecodemodel.model.statement
{
    public class CodegenStatementDeclareVarNull : CodegenStatementBase
    {
        private readonly Type clazz;
        private readonly string var;

        public CodegenStatementDeclareVarNull(
            Type clazz,
            string var)
        {
            this.clazz = clazz;
            this.var = var;
        }

        public override void RenderStatement(
            StringBuilder builder,
            bool isInnerClass)
        {
            AppendClassName(builder, clazz);
            builder
                .Append(" ")
                .Append(var)
                .Append("=null");
        }

        public override void MergeClasses(ISet<Type> classes)
        {
            classes.AddToSet(clazz);
        }
    }
} // end of namespace