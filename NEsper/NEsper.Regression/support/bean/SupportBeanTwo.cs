///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2015 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using com.espertech.esper.common.@internal.support;

namespace com.espertech.esper.regressionlib.support.bean
{
    [Serializable]
    public class SupportBeanTwo
    {
        public SupportBeanTwo()
        {
        }

        public SupportBeanTwo(
            string stringTwo,
            int intPrimitiveTwo)
        {
            StringTwo = stringTwo;
            IntPrimitiveTwo = intPrimitiveTwo;
        }

        public string StringTwo { get; set; }

        public bool BoolPrimitiveTwo { get; set; }

        public int IntPrimitiveTwo { get; set; }

        public long LongPrimitiveTwo { get; set; }

        public char CharPrimitiveTwo { get; set; }

        public short ShortPrimitiveTwo { get; set; }

        public byte BytePrimitiveTwo { get; set; }

        public float FloatPrimitiveTwo { get; set; }

        public double DoublePrimitiveTwo { get; set; }

        public bool? BoolBoxedTwo { get; set; }

        public int? IntBoxedTwo { get; set; }

        public long? LongBoxedTwo { get; set; }

        public char? CharBoxedTwo { get; set; }

        public short? ShortBoxedTwo { get; set; }

        public byte? ByteBoxedTwo { get; set; }

        public float? FloatBoxedTwo { get; set; }

        public double? DoubleBoxedTwo { get; set; }

        public SupportEnum EnumValueTwo { get; set; }

        public override string ToString()
        {
            return GetType().Name + "(" + StringTwo + ", " + IntPrimitiveTwo + ")";
        }
    }
} // end of namespace