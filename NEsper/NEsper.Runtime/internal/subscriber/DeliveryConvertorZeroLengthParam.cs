///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2015 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

namespace com.espertech.esper.runtime.@internal.subscriber
{
    /// <summary>
    /// Implementation that does not convert columns.
    /// </summary>
    public class DeliveryConvertorZeroLengthParam : DeliveryConvertor
    {
        public static readonly DeliveryConvertorZeroLengthParam INSTANCE = new DeliveryConvertorZeroLengthParam();

        private DeliveryConvertorZeroLengthParam()
        {
        }

        public object[] ConvertRow(object[] columns)
        {
            return null;
        }
    }
} // end of namespace