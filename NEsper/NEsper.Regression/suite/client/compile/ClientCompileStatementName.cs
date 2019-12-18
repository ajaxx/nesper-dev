///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2015 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

using com.espertech.esper.compiler.client;
using com.espertech.esper.compiler.client.option;
using com.espertech.esper.regressionlib.framework;

using NUnit.Framework;

namespace com.espertech.esper.regressionlib.suite.client.compile
{
    public class ClientCompileStatementName
    {
        public static IList<RegressionExecution> Executions()
        {
            IList<RegressionExecution> execs = new List<RegressionExecution>();
            execs.Add(new ClientCompileStatementNameResolve());
            return execs;
        }

        internal class ClientCompileStatementNameResolve : RegressionExecution
        {
            public void Run(RegressionEnvironment env)
            {
                MyStatementNameResolver.Contexts.Clear();
                var args = new CompilerArguments(env.Configuration);
                args.Options.StatementName = new MyStatementNameResolver().GetValue;
                var epl = "select * from SupportBean";
                var compiled = env.Compile(epl, args);

                var ctx = MyStatementNameResolver.Contexts[0];
                Assert.AreEqual(epl, ctx.EplSupplier.Invoke());
                Assert.AreEqual(null, ctx.StatementName);
                Assert.AreEqual(null, ctx.ModuleName);
                Assert.AreEqual(0, ctx.Annotations.Length);
                Assert.AreEqual(0, ctx.StatementNumber);

                env.Deploy(compiled);
                Assert.AreEqual("hello", env.Statement("hello").Name);
                env.UndeployAll();
            }
        }

        internal class MyStatementNameResolver
        {
            public static IList<StatementNameContext> Contexts { get; } = new List<StatementNameContext>();

            public string GetValue(StatementNameContext env)
            {
                Contexts.Add(env);
                return "hello";
            }
        }
    }
} // end of namespace