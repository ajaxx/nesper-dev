///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2015 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using com.espertech.esper.regressionlib.framework;

namespace com.espertech.esper.regressionlib.suite.@event.xml
{
    public class EventXMLSchemaDOMGetterBacked : RegressionExecution
    {
        public void Run(RegressionEnvironment env)
        {
            EventXMLSchemaXPathBacked.RunAssertion(env, false, "XMLSchemaConfigTwo");
        }
    }
} // end of namespace