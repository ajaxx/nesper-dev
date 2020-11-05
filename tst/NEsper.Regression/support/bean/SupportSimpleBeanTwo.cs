///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2015 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

namespace com.espertech.esper.regressionlib.support.bean
{
    [Serializable]
    public class SupportSimpleBeanTwo
    {
        public SupportSimpleBeanTwo(
            string s2,
            int i2)
        {
            S2 = s2;
            I2 = i2;
        }

        public SupportSimpleBeanTwo(
            string s2,
            int i2,
            double d2,
            long l2)
        {
            S2 = s2;
            I2 = i2;
            D2 = d2;
            L2 = l2;
        }

        public string S2 { get; }

        public int I2 { get; }

        public double D2 { get; }

        public long L2 { get; }
    }
} // end of namespace