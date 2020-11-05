///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2015 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System.Numerics;

namespace com.espertech.esper.common.@internal.@event.json.deserializers.array
{
    public class JsonDeserializerCollectionPrimitiveBigInteger : JsonDeserializerCollectionBase<BigInteger>
    {
        public JsonDeserializerCollectionPrimitiveBigInteger()
            : base(_ => _.GetBigInteger())
        {
        }
    }
} // end of namespace