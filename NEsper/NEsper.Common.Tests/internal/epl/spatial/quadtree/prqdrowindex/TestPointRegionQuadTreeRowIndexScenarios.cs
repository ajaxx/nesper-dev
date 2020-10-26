///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2015 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

using com.espertech.esper.common.@internal.epl.spatial.quadtree.pointregion;
using com.espertech.esper.compat.collections;

using NUnit.Framework;

using static com.espertech.esper.common.@internal.epl.spatial.quadtree.pointregion.SupportPointRegionQuadTreeUtil;
using static com.espertech.esper.common.@internal.epl.spatial.quadtree.prqdrowindex.SupportPointRegionQuadTreeRowIndexUtil;

namespace com.espertech.esper.common.@internal.epl.spatial.quadtree.prqdrowindex
{
    [TestFixture]
    public class TestPointRegionQuadTreeRowIndexScenarios : AbstractCommonTest
    {
        [Test, RunInApplicationDomain]
        public void TestSubdivideAdd()
        {
            PointRegionQuadTree<object> tree = PointRegionQuadTreeFactory<object>.Make(0, 0, 100, 100, 2, 3);
            AddNonUnique(tree, 0, 0, "P1");
            AddNonUnique(tree, 0, 0, "P2");
            AddNonUnique(tree, 0, 0, "P3");
            Assert.AreEqual(3, NavigateLeaf(tree, "nw,nw").Count);
        }

        [Test, RunInApplicationDomain]
        public void TestDimension()
        {
            PointRegionQuadTree<object> tree = PointRegionQuadTreeFactory<object>.Make(1000, 100000, 9000, 900000);
            Assert.IsFalse(AddNonUnique(tree, 10, 90, "P1"));
            Assert.IsFalse(AddNonUnique(tree, 10999999, 90, "P2"));
            Assert.IsTrue(AddNonUnique(tree, 5000, 800000, "P3"));

            AssertFound(tree, 0, 0, 10000000, 10000000, "P3");
            AssertFound(tree, 4000, 790000, 1200, 11000, "P3");
            AssertFound(tree, 4000, 790000, 900, 9000, "");
        }

        [Test, RunInApplicationDomain]
        public void TestSuperSlim()
        {
            PointRegionQuadTree<object> tree = PointRegionQuadTreeFactory<object>.Make(0, 0, 100, 100, 1, 100);
            AddNonUnique(tree, 10, 90, "P1");
            AddNonUnique(tree, 10, 95, "P2");
            PointRegionQuadTreeNodeLeaf<object> ne = NavigateLeaf(tree, "sw,sw,sw,ne");
            Compare(10, 90, "\"P1\"", (XYPointMultiType) ne.Points);
            PointRegionQuadTreeNodeLeaf<object> se = NavigateLeaf(tree, "sw,sw,sw,se");
            Compare(10, 95, "\"P2\"", (XYPointMultiType) se.Points);
        }

        [Test, RunInApplicationDomain]
        public void TestSubdivideMultiChild()
        {
            PointRegionQuadTree<object> tree = PointRegionQuadTreeFactory<object>.Make(0, 0, 100, 100, 4, 3);
            AddNonUnique(tree, 60, 10, "P1");
            AddNonUnique(tree, 60, 40, "P2");
            AddNonUnique(tree, 70, 30, "P3");
            AddNonUnique(tree, 60, 10, "P4");
            AddNonUnique(tree, 90, 45, "P5");

            NavigateLeaf(tree, "nw");
            NavigateLeaf(tree, "se");
            NavigateLeaf(tree, "sw");
            PointRegionQuadTreeNodeBranch ne = NavigateBranch(tree, "ne");
            Assert.AreEqual(2, ne.Level);

            PointRegionQuadTreeNodeLeaf<object> nw = NavigateLeaf(ne, "nw");
            Compare(60, 10, "[\"P1\", \"P4\"]", (XYPointMultiType) nw.Points);
            Assert.AreEqual(2, nw.Count);

            PointRegionQuadTreeNodeLeaf<object> se = NavigateLeaf(ne, "se");
            Compare(90, 45, "\"P5\"", (XYPointMultiType) se.Points);
            Assert.AreEqual(1, se.Count);

            PointRegionQuadTreeNodeLeaf<object> sw = NavigateLeaf(ne, "sw");
            var collection = AssertPointCollection(sw);
            Compare(60, 40, "\"P2\"", collection[0]);
            Compare(70, 30, "\"P3\"", collection[1]);
            Assert.AreEqual(2, sw.Count);

            Remove(tree, 60, 10, "P1");
            Remove(tree, 60, 40, "P2");

            PointRegionQuadTreeNodeLeaf<object> root = NavigateLeaf(tree, "");
            collection = AssertPointCollection(root);
            Assert.AreEqual(3, root.Count);
            Assert.AreEqual(3, collection.Length);
            Compare(60, 10, "[\"P4\"]", collection[0]);
            Compare(70, 30, "\"P3\"", collection[1]);
            Compare(90, 45, "\"P5\"", collection[2]);
        }

        [Test, RunInApplicationDomain]
        public void TestRemoveNonExistent()
        {
            PointRegionQuadTree<object> tree = PointRegionQuadTreeFactory<object>.Make(0, 0, 100, 100, 20, 20);
            Remove(tree, 10, 61, "P1");
            AddNonUnique(tree, 10, 60, "P1");
            Remove(tree, 10, 61, "P1");
            AddNonUnique(tree, 10, 80, "P2");
            AddNonUnique(tree, 20, 70, "P3");
            AddNonUnique(tree, 10, 80, "P4");
            Assert.AreEqual(4, NavigateLeaf(tree, "").Count);
            AssertFound(tree, 10, 60, 10000, 10000, "P1,P2,P3,P4");

            Remove(tree, 10, 61, "P1");
            Remove(tree, 9, 60, "P1");
            Remove(tree, 10, 60, "P2");
            Remove(tree, 10, 80, "P1");
            Assert.AreEqual(4, NavigateLeaf(tree, "").Count);
            AssertFound(tree, 10, 60, 10000, 10000, "P1,P2,P3,P4");

            Remove(tree, 10, 80, "P4");
            Assert.AreEqual(3, NavigateLeaf(tree, "").Count);
            AssertFound(tree, 10, 60, 10000, 10000, "P1,P2,P3");

            Remove(tree, 10, 80, "P2");
            Assert.AreEqual(2, NavigateLeaf(tree, "").Count);
            AssertFound(tree, 10, 60, 10000, 10000, "P1,P3");

            Remove(tree, 10, 60, "P1");
            Assert.AreEqual(1, NavigateLeaf(tree, "").Count);
            AssertFound(tree, 10, 60, 10000, 10000, "P3");

            Remove(tree, 20, 70, "P3");
            Assert.AreEqual(0, NavigateLeaf(tree, "").Count);
            AssertFound(tree, 10, 60, 10000, 10000, "");
        }

        [Test, RunInApplicationDomain]
        public void TestSubdivideSingleMerge()
        {
            PointRegionQuadTree<object> tree = PointRegionQuadTreeFactory<object>.Make(0, 0, 100, 100, 3, 2);
            AddNonUnique(tree, 65, 75, "P1");
            AddNonUnique(tree, 80, 75, "P2");
            AddNonUnique(tree, 80, 60, "P3");
            AddNonUnique(tree, 80, 60, "P4");
            AssertFound(tree, 60, 60, 21, 21, "P1,P2,P3,P4");

            Assert.IsFalse(tree.Root is PointRegionQuadTreeNodeLeaf<object>);
            Assert.AreEqual(4, NavigateLeaf(tree, "se").Count);

            var collection = AssertPointCollection(NavigateLeaf(tree, "se"));
            Assert.That(collection, Has.Length.EqualTo(3));
            Compare(65, 75, "\"P1\"", collection[0]);
            Compare(80, 75, "\"P2\"", collection[1]);
            Compare(80, 60, "[\"P3\", \"P4\"]", collection[2]);

            AddNonUnique(tree, 66, 78, "P5");
            Remove(tree, 65, 75, "P1");
            Remove(tree, 80, 60, "P3");

            var leaf = NavigateLeaf(tree, "se");
            collection = AssertPointCollection(leaf);
            Assert.That(leaf.Count, Is.EqualTo(3));
            AssertFound(tree, 60, 60, 21, 21, "P2,P4,P5");
            Assert.AreEqual(3, collection.Length);
            Compare(80, 75, "\"P2\"", collection[0]);
            Compare(80, 60, "[\"P4\"]", collection[1]);
            Compare(66, 78, "\"P5\"", collection[2]);

            Remove(tree, 66, 78, "P5");

            AssertFound(tree, 60, 60, 21, 21, "P2,P4");

            leaf = NavigateLeaf(tree, "");
            Assert.AreEqual(2, leaf.Count);
            collection = AssertPointCollection(NavigateLeaf(tree, ""));
            Assert.That(collection, Has.Length.EqualTo(2));
            Compare(80, 75, "\"P2\"", collection[0]);
            Compare(80, 60, "[\"P4\"]", collection[1]);
        }

        [Test, RunInApplicationDomain]
        public void TestSubdivideMultitypeMerge()
        {
            PointRegionQuadTree<object> tree = PointRegionQuadTreeFactory<object>.Make(0, 0, 100, 100, 6, 2);
            Assert.AreEqual(1, tree.Root.Level);
            AddNonUnique(tree, 10, 10, "P1");
            AddNonUnique(tree, 9.9, 10, "P2");
            AddNonUnique(tree, 10, 9.9, "P3");
            AddNonUnique(tree, 10, 10, "P4");
            AddNonUnique(tree, 10, 9.9, "P5");
            AddNonUnique(tree, 9.9, 10, "P6");
            Assert.IsInstanceOf<PointRegionQuadTreeNodeLeaf<object>>(tree.Root);
            AssertFound(tree, 9, 10, 1, 1, "P2,P6");
            AssertFound(tree, 10, 9, 1, 1, "P3,P5");
            AssertFound(tree, 10, 10, 1, 1, "P1,P4");
            AssertFound(tree, 9, 9, 2, 2, "P1,P2,P3,P4,P5,P6");

            AddNonUnique(tree, 10, 10, "P7");

            Assert.IsFalse(tree.Root is PointRegionQuadTreeNodeLeaf<object>);
            Assert.AreEqual(1, tree.Root.Level);
            Assert.AreEqual(7, NavigateLeaf(tree, "nw").Count);

            var collection = AssertPointCollection(NavigateLeaf(tree, "nw"));
            Assert.That(collection, Has.Length.EqualTo(3));
            Compare(10, 10, "[\"P1\", \"P4\", \"P7\"]", collection[0]);
            Compare(9.9, 10, "[\"P2\", \"P6\"]", collection[1]);
            Compare(10, 9.9, "[\"P3\", \"P5\"]", collection[2]);
            AssertFound(tree, 9, 10, 1, 1, "P2,P6");
            AssertFound(tree, 10, 9, 1, 1, "P3,P5");
            AssertFound(tree, 10, 10, 1, 1, "P1,P4,P7");
            AssertFound(tree, 9, 9, 2, 2, "P1,P2,P3,P4,P5,P6,P7");

            AddNonUnique(tree, 9.9, 10, "P8");
            AddNonUnique(tree, 10, 9.9, "P9");
            AddNonUnique(tree, 10, 10, "P10");
            AddNonUnique(tree, 10, 10, "P11");
            AddNonUnique(tree, 10, 10, "P12");

            Assert.AreEqual(12, NavigateLeaf(tree, "nw").Count);
            Assert.AreEqual(2, NavigateLeaf(tree, "nw").Level);
            collection = AssertPointCollection(NavigateLeaf(tree, "nw"));
            Assert.That(collection, Has.Length.EqualTo(3));
            Compare(10, 10, "[\"P1\", \"P4\", \"P7\", \"P10\", \"P11\", \"P12\"]", collection[0]);
            Compare(9.9, 10, "[\"P2\", \"P6\", \"P8\"]", collection[1]);
            Compare(10, 9.9, "[\"P3\", \"P5\", \"P9\"]", collection[2]);
            AssertFound(tree, 9, 10, 1, 1, "P2,P6,P8");
            AssertFound(tree, 10, 9, 1, 1, "P3,P5,P9");
            AssertFound(tree, 10, 10, 1, 1, "P1,P4,P7,P10,P11,P12");
            AssertFound(tree, 9, 9, 2, 2, "P1,P2,P3,P4,P5,P6,P7,P8,P9,P10,P11,P12");

            Remove(tree, 9.9, 10, "P8");
            Remove(tree, 10, 9.9, "P3");
            Remove(tree, 10, 9.9, "P5");
            Remove(tree, 10, 9.9, "P9");
            AssertFound(tree, 9, 10, 1, 1, "P2,P6");
            AssertFound(tree, 10, 9, 1, 1, "");
            AssertFound(tree, 10, 10, 1, 1, "P1,P4,P7,P10,P11,P12");
            AssertFound(tree, 9, 9, 2, 2, "P1,P2,P4,P6,P7,P10,P11,P12");

            Assert.AreEqual(8, NavigateLeaf(tree, "nw").Count);
            collection = AssertPointCollection(NavigateLeaf(tree, "nw"));
            Assert.That(collection, Has.Length.EqualTo(2));
            Compare(10, 10, "[\"P1\", \"P4\", \"P7\", \"P10\", \"P11\", \"P12\"]", collection[0]);
            Compare(9.9, 10, "[\"P2\", \"P6\"]", collection[1]);
            Assert.IsFalse(tree.Root is PointRegionQuadTreeNodeLeaf<object>);

            Remove(tree, 9.9, 10, "P2");
            Remove(tree, 10, 10, "P1");
            Remove(tree, 10, 10, "P10");
            Assert.IsInstanceOf<PointRegionQuadTreeNodeLeaf<object>>(tree.Root);
            Assert.AreEqual(5, NavigateLeaf(tree, "").Count);
            collection = AssertPointCollection(NavigateLeaf(tree, ""));
            Assert.That(collection, Has.Length.EqualTo(2));
            Compare(10, 10, "[\"P4\", \"P7\", \"P11\", \"P12\"]", collection[0]);
            Compare(9.9, 10, "[\"P6\"]", collection[1]);
            AssertFound(tree, 9, 10, 1, 1, "P6");
            AssertFound(tree, 10, 9, 1, 1, "");
            AssertFound(tree, 10, 10, 1, 1, "P4,P7,P11,P12");
            AssertFound(tree, 9, 9, 2, 2, "P4,P6,P7,P11,P12");
        }


        static XYPointMultiType[] AssertPointCollection(
            PointRegionQuadTreeNodeLeaf<object> leaf)
        {
            Assert.That(leaf, Is.Not.Null);
            Assert.That(leaf.Points, Is.InstanceOf<ICollection<XYPointMultiType>>());
            return leaf.Points.UnwrapIntoArray<XYPointMultiType>();
        }
    }
} // end of namespace
