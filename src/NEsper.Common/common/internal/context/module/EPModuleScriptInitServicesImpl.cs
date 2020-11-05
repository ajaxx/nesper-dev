///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using com.espertech.esper.common.@internal.epl.script.core;

namespace com.espertech.esper.common.@internal.context.module
{
    public class EPModuleScriptInitServicesImpl : EPModuleScriptInitServices
    {
        public EPModuleScriptInitServicesImpl(ScriptCollector scriptCollector)
        {
            ScriptCollector = scriptCollector;
        }

        public ScriptCollector ScriptCollector { get; }
    }
} // end of namespace