///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using com.espertech.esper.common.@internal.epl.spatial.quadtree.core;

namespace com.espertech.esper.common.@internal.epl.spatial.quadtree.pointregion
{
    public class PointRegionQuadTreeNodeLeaf<TL> : PointRegionQuadTreeNodeLeafOpaque
    {
        public PointRegionQuadTreeNodeLeaf(
            BoundingBox bb,
            int level,
            TL points,
            int count)
            : base(bb, level)
        {
            Points = points;
            Count = count;
        }

        public override object OpaquePoints => Points;

        public TL Points { get; set; }

        public int Count { get; set; }

        public void IncCount(int numAdded)
        {
            Count += numAdded;
        }

        public void DecCount()
        {
            Count--;
        }
    }
} // end of namespace