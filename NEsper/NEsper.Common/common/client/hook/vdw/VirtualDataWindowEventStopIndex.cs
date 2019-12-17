///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

namespace com.espertech.esper.common.client.hook.vdw
{
    /// <summary>
    ///     Event to indicate that for a virtual data window an existing index is being stopped or destroyed.
    /// </summary>
    public class VirtualDataWindowEventStopIndex : VirtualDataWindowEvent
    {
        /// <summary>
        ///     Ctor.
        /// </summary>
        /// <param name="namedWindowName">named window name</param>
        /// <param name="indexName">index name</param>
        public VirtualDataWindowEventStopIndex(
            string namedWindowName,
            string indexName)
        {
            NamedWindowName = namedWindowName;
            IndexName = indexName;
        }

        /// <summary>
        ///     Returns the index name.
        /// </summary>
        /// <returns>index name</returns>
        public string IndexName { get; }

        /// <summary>
        ///     Returns the named window name.
        /// </summary>
        /// <returns>named window name</returns>
        public string NamedWindowName { get; }
    }
} // end of namespace