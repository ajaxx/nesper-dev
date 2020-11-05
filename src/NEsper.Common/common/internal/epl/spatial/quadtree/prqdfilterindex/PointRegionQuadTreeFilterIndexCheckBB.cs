///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using com.espertech.esper.common.client;
using com.espertech.esper.common.@internal.epl.spatial.quadtree.core;
using com.espertech.esper.compat.collections;

namespace com.espertech.esper.common.@internal.epl.spatial.quadtree.prqdfilterindex
{
    public class PointRegionQuadTreeFilterIndexCheckBB
    {
        public static void CheckBB(
            BoundingBox bb,
            double x,
            double y)
        {
            if (!bb.ContainsPoint(x, y)) {
                throw new EPException(string.Format("Point ({0},{1}) not in {2}", 
                    x.RenderAny(), 
                    y.RenderAny(),
                    bb));
            }
        }
    }
} // end of namespace