///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using com.espertech.esper.compat;

namespace com.espertech.esper.common.@internal.epl.enummethod.eval
{
    public partial class ExprDotForgeSumOf
    {
        internal class ExprDotEvalSumMethodDecimal : ExprDotEvalSumMethod
        {
            private long _cnt;
            private decimal _sum;

            public ExprDotEvalSumMethodDecimal()
            {
                _sum = decimal.Zero;
            }

            public void Enter(object @object)
            {
                if (@object == null) {
                    return;
                }

                _cnt++;
                _sum += @object.AsDecimal();
            }

            public object Value {
                get {
                    if (_cnt == 0) {
                        return null;
                    }

                    return _sum;
                }
            }
        }
    }
}