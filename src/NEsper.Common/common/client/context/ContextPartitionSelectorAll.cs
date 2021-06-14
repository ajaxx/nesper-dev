///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////


namespace com.espertech.esper.common.client.context
{
    /// <summary>
    /// Selects all context paritions.
    /// </summary>
    public sealed class ContextPartitionSelectorAll : ContextPartitionSelector
    {
        /// <summary>
        /// Instance for selecting all context partitions.
        /// </summary>
        public static readonly ContextPartitionSelectorAll INSTANCE = new ContextPartitionSelectorAll();
    }
}