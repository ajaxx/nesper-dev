///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

namespace com.espertech.esper.common.@internal.epl.join.hint
{
    public class SelectorInstructionPair
    {
        public SelectorInstructionPair(
            IList<IndexHintSelector> selector,
            IList<IndexHintInstruction> instructions)
        {
            Selector = selector;
            Instructions = instructions;
        }

        public IList<IndexHintSelector> Selector { get; private set; }

        public IList<IndexHintInstruction> Instructions { get; private set; }
    }
}