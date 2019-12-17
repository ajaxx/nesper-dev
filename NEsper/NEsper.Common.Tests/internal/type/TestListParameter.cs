///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2015 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

using com.espertech.esper.common.client.scopetest;

using NUnit.Framework;

namespace com.espertech.esper.common.@internal.type
{
    [TestFixture]
    public class TestListParameter : AbstractCommonTest
    {
        private ListParameter listParam;

        [SetUp]
        public void SetUp()
        {
            listParam = new ListParameter();
            listParam.Add(new IntParameter(5));
            listParam.Add(new FrequencyParameter(3));
        }

        [Test]
        public void TestIsWildcard()
        {
            // Wildcard is expected to make only a best-guess effort, not be perfect
            Assert.IsTrue(listParam.IsWildcard(5, 5));
            Assert.IsFalse(listParam.IsWildcard(6, 10));
        }

        [Test]
        public void TestGetValues()
        {
            var result = listParam.GetValuesInRange(1, 8);
            EPAssertionUtil.AssertEqualsAnyOrder(new int[] { 3, 5, 6 }, result);
        }

        [Test]
        public void TestContainsPoint()
        {
            Assert.IsTrue(listParam.ContainsPoint(0));
            Assert.IsFalse(listParam.ContainsPoint(1));
            Assert.IsFalse(listParam.ContainsPoint(2));
            Assert.IsTrue(listParam.ContainsPoint(3));
            Assert.IsFalse(listParam.ContainsPoint(4));
            Assert.IsTrue(listParam.ContainsPoint(5));
        }

        [Test]
        public void TestFormat()
        {
            Assert.AreEqual("5, */3", listParam.Formatted());
        }
    }
} // end of namespace
