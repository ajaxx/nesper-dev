///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2017 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using com.espertech.esper.compat;

namespace NEsper.Examples.ATM
{
	public class FraudWarning
	{
	    public FraudWarning(long accountNumber, string warning)
	    {
	        AccountNumber = accountNumber;
	        Warning = warning;
	        Timestamp = DateTimeHelper.CurrentTimeMillis;
	    }

	    public long AccountNumber { get; }

	    public string Warning { get; }

	    public long Timestamp { get; }
	}
}
