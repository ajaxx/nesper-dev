///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

namespace com.espertech.esper.common.@internal.compile.stage1.spec
{
    public class PluggableObjectEntry
    {
        public PluggableObjectType PluggableType { get; private set; }

        public object CustomConfigs { get; private set; }

        public PluggableObjectEntry(
            PluggableObjectType type,
            Object customConfigs)
        {
            PluggableType = type;
            CustomConfigs = customConfigs;
        }
    }
}