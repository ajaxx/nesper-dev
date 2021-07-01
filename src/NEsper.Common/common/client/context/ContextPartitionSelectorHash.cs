///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

namespace com.espertech.esper.common.client.context
{
    /// <summary>
    /// Selects context partitions based on hash codes, for use with hashed context.
    /// </summary>
    public interface ContextPartitionSelectorHash : ContextPartitionSelector
    {
        /// <summary>
        /// Returns a set of hashes.
        /// </summary>
        /// <value>hashes</value>
        ICollection<int> Hashes { get; }
    }
}