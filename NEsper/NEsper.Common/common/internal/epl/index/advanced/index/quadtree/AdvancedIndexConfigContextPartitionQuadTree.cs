///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Globalization;
using System.IO;

using com.espertech.esper.common.@internal.bytecodemodel.model.expression;
using com.espertech.esper.common.@internal.epl.lookup;
using com.espertech.esper.compat.collections;

using static com.espertech.esper.common.@internal.bytecodemodel.model.expression.CodegenExpressionBuilder;

namespace com.espertech.esper.common.@internal.epl.index.advanced.index.quadtree
{
    public class AdvancedIndexConfigContextPartitionQuadTree : AdvancedIndexConfigContextPartition
    {
        public AdvancedIndexConfigContextPartitionQuadTree(
            double x,
            double y,
            double width,
            double height,
            int leafCapacity,
            int maxTreeHeight)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            LeafCapacity = leafCapacity;
            MaxTreeHeight = maxTreeHeight;
        }

        public double X { get; }

        public double Y { get; }

        public double Width { get; }

        public double Height { get; }

        public int LeafCapacity { get; }

        public int MaxTreeHeight { get; }

        public CodegenExpression Make()
        {
            return NewInstance<AdvancedIndexConfigContextPartitionQuadTree>(
                Constant(X),
                Constant(Y),
                Constant(Width),
                Constant(Height),
                Constant(LeafCapacity),
                Constant(MaxTreeHeight));
        }

        public void ToConfiguration(TextWriter builder)
        {
            builder.Write(X.RenderAny());
            builder.Write(",");
            builder.Write(Y.RenderAny());
            builder.Write(",");
            builder.Write(Width.RenderAny());
            builder.Write(",");
            builder.Write(Height.RenderAny());
            builder.Write(",");
            builder.Write(LeafCapacity.RenderAny());
            builder.Write(",");
            builder.Write(MaxTreeHeight.RenderAny());
        }
    }
} // end of namespace