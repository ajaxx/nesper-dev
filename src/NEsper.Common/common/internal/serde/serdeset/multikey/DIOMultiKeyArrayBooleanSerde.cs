///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2015 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using com.espertech.esper.common.client.serde;
using com.espertech.esper.common.@internal.collection;
using com.espertech.esper.compat.io;

namespace com.espertech.esper.common.@internal.serde.serdeset.multikey
{
	public class DIOMultiKeyArrayBooleanSerde : DataInputOutputSerdeBase<MultiKeyArrayBoolean>
	{
		public static readonly DIOMultiKeyArrayBooleanSerde INSTANCE = new DIOMultiKeyArrayBooleanSerde();

		public override void Write(
			MultiKeyArrayBoolean mk,
			DataOutput output,
			byte[] unitKey,
			EventBeanCollatedWriter writer)
		{
			WriteInternal(mk.Keys, output);
		}

		public override MultiKeyArrayBoolean ReadValue(
			DataInput input,
			byte[] unitKey)
		{
			return new MultiKeyArrayBoolean(ReadInternal(input));
		}

		private void WriteInternal(
			bool[] @object,
			DataOutput output)
		{
			if (@object == null) {
				output.WriteInt(-1);
				return;
			}

			output.WriteInt(@object.Length);
			foreach (bool i in @object) {
				output.WriteBoolean(i);
			}
		}

		private bool[] ReadInternal(DataInput input)
		{
			int len = input.ReadInt();
			if (len == -1) {
				return null;
			}

			bool[] array = new bool[len];
			for (int i = 0; i < len; i++) {
				array[i] = input.ReadBoolean();
			}

			return array;
		}
	}
} // end of namespace
