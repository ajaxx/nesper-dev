///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2015 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Reflection;
using System.Threading;

using com.espertech.esper.common.@internal.support;
using com.espertech.esper.compat;
using com.espertech.esper.compat.logging;
using com.espertech.esper.runtime.client;

using NUnit.Framework;

namespace com.espertech.esper.regressionlib.support.multithread
{
    public class StmtUpdateSendCallable : ICallable<object>
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly int numRepeats;
        private readonly EPRuntime runtime;
        private readonly int threadNum;

        public StmtUpdateSendCallable(
            int threadNum,
            EPRuntime runtime,
            int numRepeats)
        {
            this.threadNum = threadNum;
            this.runtime = runtime;
            this.numRepeats = numRepeats;
        }

        public object Call()
        {
            try {
                log.Info(".call Thread " + Thread.CurrentThread.ManagedThreadId + " sending " + numRepeats + " events");
                for (var loop = 0; loop < numRepeats; loop++) {
                    var id = Convert.ToString(threadNum * 100000000 + loop);
                    var bean = new SupportBean(id, 0);
                    runtime.EventService.SendEventBean(bean, bean.GetType().Name);
                }

                log.Info(".call Thread " + Thread.CurrentThread.ManagedThreadId + " completed.");
            }
            catch (AssertionException ex) {
                log.Error("Assertion error in thread " + Thread.CurrentThread.ManagedThreadId, ex);
                return false;
            }
            catch (Exception t) {
                log.Error("Error in thread " + Thread.CurrentThread.ManagedThreadId, t);
                return false;
            }

            return true;
        }
    }
} // end of namespace