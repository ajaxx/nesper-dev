///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2017 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;

using com.espertech.esper.codegen.core;

namespace com.espertech.esper.codegen.model.statement
{
    public abstract class CodegenStatementWBlockBase : CodegenStatement
    {
        private readonly CodegenBlock _parent;

        public CodegenStatementWBlockBase(CodegenBlock parent)
        {
            _parent = parent;
        }

        public CodegenBlock Parent => _parent;

        public abstract void MergeClasses(ICollection<Type> classes);
        public abstract void Render(StringBuilder builder, IDictionary<Type, string> imports);
    }
} // end of namespace