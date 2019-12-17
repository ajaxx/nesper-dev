///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

using com.espertech.esper.common.client;

namespace com.espertech.esper.common.@internal.epl.index.sorted
{
    /// <summary>
    /// Iterator for use by <seealso cref="PropertySortedEventTable" />.
    /// </summary>
    public class PropertySortedEventTableEnumerator
    {
        public static IEnumerator<EventBean> For<T>(IDictionary<T, ISet<EventBean>> window)
        {
            foreach (var entry in window) {
                foreach (var value in entry.Value) {
                    yield return value;
                }
            }
        }
    }
} // end of namespace