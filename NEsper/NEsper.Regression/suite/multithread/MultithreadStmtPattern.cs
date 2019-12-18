///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2015 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Threading;

using com.espertech.esper.common.client.configuration;
using com.espertech.esper.common.@internal.support;
using com.espertech.esper.compat.concurrency;
using com.espertech.esper.regressionlib.framework;
using com.espertech.esper.regressionlib.support.client;
using com.espertech.esper.regressionlib.support.multithread;
using com.espertech.esper.regressionlib.support.util;
using com.espertech.esper.runtime.client;

using NUnit.Framework;

namespace com.espertech.esper.regressionlib.suite.multithread
{
    /// <summary>
    ///     Test for pattern statement parallel execution by threads.
    /// </summary>
    public class MultithreadStmtPattern : RegressionExecutionWithConfigure
    {
        public void Configure(Configuration configuration)
        {
        }

        public void Run(RegressionEnvironment env)
        {
            var pattern = "a=SupportBean";
            TryPattern(env, pattern, 4, 20);

            pattern = "a=SupportBean or a=SupportBean";
            TryPattern(env, pattern, 2, 20);
        }

        public bool HAWithCOnly => true;

        public bool EnableHATest => true;

        private static void TryPattern(
            RegressionEnvironment env,
            string pattern,
            int numThreads,
            int numEvents)
        {
            var sendLock = new object();
            var executor = Executors.NewMultiThreadedExecutor(numThreads);
            // new SupportThreadFactory(typeof(MultithreadStmtPattern)).ThreadFactory);
            var futures = new List<IFuture<object>>();
            var callables = new SendEventWaitCallable[numThreads];
            for (var i = 0; i < numThreads; i++) {
                callables[i] = new SendEventWaitCallable(i, env.Runtime, sendLock, new GeneratorEnumerator(numEvents));
                futures.Add(executor.Submit(callables[i]));
            }

            var listener = new SupportMTUpdateListener[numEvents];
            var epl = "select * from pattern[" + pattern + "]";
            var compiled = env.Compile(epl);
            for (var i = 0; i < numEvents; i++) {
                var stmtName = "p" + i;
                env.Deploy(compiled, new DeploymentOptions().WithStatementNameRuntime(ctx => stmtName));
                var stmt = env.Statement(stmtName);
                listener[i] = new SupportMTUpdateListener();
                stmt.AddListener(listener[i]);

                lock (sendLock) {
                    Monitor.PulseAll(sendLock);
                }
            }

            foreach (var callable in callables) {
                callable.Shutdown();
            }

            lock (sendLock) {
                Monitor.PulseAll(sendLock);
            }
            
            executor.Shutdown();
            SupportCompileDeployUtil.ExecutorAwait(executor, TimeSpan.FromSeconds(10));

            for (var i = 0; i < numEvents; i++) {
                var theEventReceived = listener[i].AssertOneGetNewAndReset();
                Assert.NotNull(theEventReceived);
                var theEventValue = theEventReceived.Get("a");
                Assert.NotNull(theEventValue);
                Assert.IsInstanceOf<SupportBean>(theEventValue);
            }

            env.UndeployAll();
        }
    }
} // end of namespace