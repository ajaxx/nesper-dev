///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

namespace com.espertech.esper.common.@internal.type
{
    /// <summary>Implementation match a string against a pattern. </summary>
    public interface StringPatternSet
    {
        /// <summary>Returns true for a match, false for no-match. </summary>
        /// <param name="stringToMatch">value to match</param>
        /// <returns>match result</returns>
        bool Match(string stringToMatch);
    }
}