///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2015 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using com.espertech.esper.regressionlib.framework;

namespace com.espertech.esper.regressionlib.suite.@event.objectarray
{
    public class EventObjectArrayInheritanceConfigRuntime : RegressionExecution
    {
        public void Run(RegressionEnvironment env)
        {
            var epl = "create objectarray schema RootEvent(base string);\n" +
                      "create objectarray schema Sub1Event(sub1 string) inherits RootEvent;\n" +
                      "create objectarray schema Sub2Event(sub2 string) inherits RootEvent;\n" +
                      "create objectarray schema SubAEvent(suba string) inherits Sub1Event;\n" +
                      "create objectarray schema SubBEvent(subb string) inherits SubAEvent;\n";
            var path = new RegressionPath();
            env.CompileDeployWBusPublicType(epl, path);

            EventObjectArrayInheritanceConfigInit.RunObjectArrInheritanceAssertion(env, path);
        }
    }
} // end of namespace