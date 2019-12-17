///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

using com.espertech.esper.common.client;
using com.espertech.esper.common.@internal.collection;
using com.espertech.esper.common.@internal.epl.resultset.core;

namespace com.espertech.esper.common.@internal.epl.resultset.rowforall
{
    public interface ResultSetProcessorRowForAllOutputAllHelper
        : ResultSetProcessorOutputHelper
    {
        void ProcessView(
            EventBean[] newData,
            EventBean[] oldData,
            bool isGenerateSynthetic);

        void ProcessJoin(
            ISet<MultiKey<EventBean>> newEvents,
            ISet<MultiKey<EventBean>> oldEvents,
            bool isGenerateSynthetic);

        UniformPair<EventBean[]> OutputView(bool isGenerateSynthetic);
        UniformPair<EventBean[]> OutputJoin(bool isGenerateSynthetic);
        void Destroy();
    }
} // end of namespace