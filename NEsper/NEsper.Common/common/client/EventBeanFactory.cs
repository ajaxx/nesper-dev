///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

namespace com.espertech.esper.common.client
{
    /// <summary>
    /// Factory for <see cref="EventBean"/> instances given an underlying event object. 
    /// <para/>
    /// Not transferable between engine instances.
    /// </summary>
    public interface EventBeanFactory
    {
        /// <summary>Wraps the underlying event object. </summary>
        /// <param name="underlying">event to wrap</param>
        /// <returns>event bean</returns>
        EventBean Wrap(object underlying);
    }
}