///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2015 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using com.espertech.esper.common.client.annotation;

namespace com.espertech.esper.common.@internal.type
{
    public class AnnotationJsonSchemaField : JsonSchemaFieldAttribute
    {
        public AnnotationJsonSchemaField(
            string name,
            string adapter)
        {
            Name = name;
            Adapter = adapter;
        }

        public Type AnnotationType => typeof(JsonSchemaFieldAttribute);
    }
} // end of namespace