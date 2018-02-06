///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2017 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

namespace com.espertech.esper.supportregression.util
{
    public class ContextStateCacheHook
    {
        public static String CONTEXT_CACHE_HOOK = "@Hook(Type=HookType.CONTEXT_STATE_CACHE, Hook='" + typeof(SupportContextStateCacheImpl).FullName + "')\n";
    }
}
