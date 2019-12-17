///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using com.espertech.esper.compat;
using com.espertech.esper.compat.collections;

namespace com.espertech.esper.common.client.hook.vdw
{
    /// <summary>
    /// A factory that the runtime invokes at deployment time to obtain the virtual data window factory.
    /// </summary>
    public interface VirtualDataWindowFactoryFactory
    {
        /// <summary>
        /// Return the virtual data window factory
        /// </summary>
        /// <param name="ctx">context information</param>
        /// <returns>factory</returns>
        VirtualDataWindowFactory CreateFactory(VirtualDataWindowFactoryFactoryContext ctx);
    }
} // end of namespace