///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

namespace com.espertech.esper.common.@internal.filterspec
{
    public interface FilterSharedBoolExprRepository
    {
        void RegisterBoolExpr(
            int statementId,
            FilterSpecParamExprNode node);

        FilterSpecParamExprNode GetFilterBoolExpr(
            int statementId,
            int filterBoolExprNum);

        void RemoveStatement(int statementId);
    }
} // end of namespace