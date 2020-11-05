///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using com.espertech.esper.common.@internal.epl.expression.core;

namespace com.espertech.esper.common.@internal.compile.stage1.spec
{
    /// <summary>
    /// Specification for creating a named window index column.
    /// </summary>
    [Serializable]
    public class CreateIndexItem
    {
        private readonly IList<ExprNode> _expressions;
        private readonly string _type;
        private readonly IList<ExprNode> _parameters;

        public CreateIndexItem(
            IList<ExprNode> expressions,
            string type,
            IList<ExprNode> parameters)
        {
            _expressions = expressions;
            _type = type;
            _parameters = parameters;
        }

        public IList<ExprNode> Expressions => _expressions;

        public string IndexType => _type;

        public IList<ExprNode> Parameters => _parameters;
    }
} // end of namespace