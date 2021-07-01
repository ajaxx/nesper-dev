///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2015 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using com.espertech.esper.compat;

using com.espertech.esper.compat.logging;
using com.espertech.esper.container;
using com.espertech.esper.regressionlib.framework;
using com.espertech.esper.runtime.client;

using NEsper.Avro.Core;

namespace com.espertech.esper.regressionrun.runner
{
    public class RegressionRunner
    {
        private static readonly ILog LOG = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        // References required to ensure type loading
        private static readonly Type eventTypeAvroHandlerImpl = typeof(EventTypeAvroHandlerImpl);

        static RegressionRunner()
        {
            RegressionCore.Initialize();
        }

        public static void RunConfigurable(RegressionExecutionWithConfigure configurable)
        {
            var session = Session();
            configurable.Configure(session.Configuration);
            Run(session, configurable, true);
            session.Dispose();
        }

        public static RegressionSession Session()
        {
            return new RegressionSession(SupportConfigFactory.GetConfiguration());
        }

        public static void RunPerformanceSensitive<T>(
            RegressionSession session,
            ICollection<T> executions)
            where T : RegressionExecution
        {
            Run<T>(session, executions, true);
        }

        public static void Run<T>(
            RegressionSession session,
            ICollection<T> executions,
            bool usePerfContext = false)
            where T : RegressionExecution
        {
            foreach (var execution in RegressionFilter.FilterBySystemProperty(executions)) {
                using (PerformanceScope(usePerfContext)) {
                    RunInternal(session, execution);
                }
            }
        }

        public static void RunPerformanceSensitive(
            RegressionSession session,
            RegressionExecution execution)
        {
            Run(session, execution, true);
        }

        public static void Run(
            RegressionSession session,
            RegressionExecution execution,
            bool usePerfContext = false)
        {
            using (PerformanceScope(usePerfContext)) {
                RunInternal(session, execution);
            }
        }

        private static void RunInternal(
            RegressionSession session,
            RegressionExecution execution)
        {
                if (session.Runtime == null) {
                    var exists = EPRuntimeProvider.HasRuntime(EPRuntimeProvider.DEFAULT_RUNTIME_URI);
                    var runtime = EPRuntimeProvider.GetDefaultRuntime(session.Configuration);
                    if (exists) {
                        runtime.Initialize();
                    }

                    session.Runtime = runtime;
                }

                LOG.Info("Running test " + execution.Name());
                execution.Run(new RegressionEnvironmentEsper(session.Configuration, session.Runtime));
        }

        public static IDisposable PerformanceScope(bool usePerfContext)
        {
            return usePerfContext 
                ? (IDisposable) new PerformanceContext() 
                : (IDisposable) new VoidDisposable();
        }
    }
} // end of namespace