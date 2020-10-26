///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using com.espertech.esper.common.@internal.epl.expression.time.eval;

namespace com.espertech.esper.common.@internal.epl.expression.time.node
{
    public interface ExprTimePeriodEvalDeltaConst
    {
        long DeltaAdd(long fromTime);

        long DeltaSubtract(long fromTime);

        TimePeriodDeltaResult DeltaAddWReference(
            long fromTime,
            long reference);
    }
} // end of namespace