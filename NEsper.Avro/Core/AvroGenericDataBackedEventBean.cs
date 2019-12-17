///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2015 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using Avro.Generic;

using com.espertech.esper.common.client;
using com.espertech.esper.common.@internal.@event.core;

namespace NEsper.Avro.Core
{
    /// <summary>
    /// For events that are array of properties.
    /// </summary>
    public interface AvroGenericDataBackedEventBean : EventBean,
        AvroBackedBean
    {
        GenericRecord Properties { get; }
    }
} // end of namespace