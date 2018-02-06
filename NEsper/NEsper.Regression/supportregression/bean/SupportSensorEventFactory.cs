///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2017 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

namespace com.espertech.esper.supportregression.bean
{
    public class SupportSensorEventFactory
    {
        public static SupportSensorEvent GetInstance()
        {
            return new SupportSensorEvent(0, null, null, 0, 0);
        }
    }
}
