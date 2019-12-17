///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

namespace com.espertech.esper.common.client.util
{
    /// <summary>
    ///     Enum indicating what kind of references are used to store the cache map's keys and values.
    /// </summary>
    public enum CacheReferenceType
    {
        /// <summary>
        ///     Constant indicating that hard references should be used.
        ///     <para />
        ///     Does not allow garbage collection to remove cache entries.
        /// </summary>
        HARD,

        /// <summary>
        ///     Constant indicating that soft references should be used.
        ///     <para />
        ///     Allows garbage collection to remove cache entries only after all weak references have been collected.
        /// </summary>
        SOFT,

        /// <summary>
        ///     Constant indicating that weak references should be used.
        ///     <para />
        ///     Allows garbage collection to remove cache entries.
        /// </summary>
        WEAK,

        /// <summary>
        ///     The default policy is set to WEAK to reduce the chance that out-of-memory errors occur
        ///     as caches fill, and stay backwards compatible with prior Esper releases.
        /// </summary>
        /// <returns>default reference type</returns>
        DEFAULT = WEAK
    }
} // end of namespace