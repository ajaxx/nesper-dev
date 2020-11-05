///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2015 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using com.espertech.esper.compat.logging;

namespace com.espertech.esper.common.@internal.supportunit.util
{
    /// <summary>
    ///     Utility class for comparing double values up to a given precision
    /// </summary>
    public class DoubleValueAssertionUtil
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static bool Equals(
            double valueActual,
            double valueExpected,
            int precision)
        {
            if (precision < 1)
            {
                throw new ArgumentException("Invalid precision value of " + precision + " supplied");
            }

            if (double.IsNaN(valueActual) && double.IsNaN(valueExpected))
            {
                return true;
            }

            if (double.IsNaN(valueActual) && !double.IsNaN(valueExpected) ||
                !double.IsNaN(valueActual) && double.IsNaN(valueExpected))
            {
                Log.Debug(
                    ".equals Compare failed, " +
                    "  valueActual=" + valueActual +
                    "  valueExpected=" + valueExpected);
                return false;
            }

            var factor = Math.Pow(10, precision);
            var val1 = valueActual * factor;
            var val2 = valueExpected * factor;

            // Round to closest integer
            double d1 = Math.Round(val1, MidpointRounding.ToEven);
            double d2 = Math.Round(val2, MidpointRounding.ToEven);

            if (d1 != d2)
            {
                Log.Debug(
                    ".equals Compare failed, " +
                    "  valueActual=" + valueActual +
                    "  valueExpected=" + valueExpected +
                    "  precision=" + precision
                );
                return false;
            }

            return true;
        }
    }
} // end of namespace
