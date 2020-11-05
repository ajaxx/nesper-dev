﻿///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

namespace com.espertech.esper.compat
{
    public struct ValueRange<T>
    {
        public T Minimum;
        public T Maximum;

        public ValueRange(
            T minimum,
            T maximum)
        {
            Minimum = minimum;
            Maximum = maximum;
        }
    }
}