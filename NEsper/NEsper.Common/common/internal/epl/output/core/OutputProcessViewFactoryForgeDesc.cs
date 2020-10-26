///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2015 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

using com.espertech.esper.common.@internal.compile.stage3;

namespace com.espertech.esper.common.@internal.epl.output.core
{
    /// <summary>
    ///     Factory for factories for output processing views.
    /// </summary>
    public class OutputProcessViewFactoryForgeDesc
    {
        public OutputProcessViewFactoryForgeDesc(
            OutputProcessViewFactoryForge forge,
            IList<StmtClassForgeableFactory> additionalForgeables)
        {
            Forge = forge;
            AdditionalForgeables = additionalForgeables;
        }

        public OutputProcessViewFactoryForge Forge { get; }

        public IList<StmtClassForgeableFactory> AdditionalForgeables { get; }
    }
} // end of namespace