///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2015 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

namespace com.espertech.esper.regressionlib.support.util
{
    public class SupportDataSourceFactory
    {
#if NOT_IN_USE
        public static DataSource CreateDataSource(Properties properties)
        {
            return new SupportDriverManagerDataSource(properties);
        }
#endif
    }
} // end of namespace