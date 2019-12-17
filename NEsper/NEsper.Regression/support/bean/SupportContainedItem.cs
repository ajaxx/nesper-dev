///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2015 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

namespace com.espertech.esper.regressionlib.support.bean
{
    [Serializable]
    public class SupportContainedItem
    {
        public SupportContainedItem(
            string itemId,
            string selected)
        {
            ItemId = itemId;
            Selected = selected;
        }

        public string ItemId { get; }

        public string Selected { get; }
    }
} // end of namespace