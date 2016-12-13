using Mono.Cecil;
using System;

namespace Unity.IL2CPP.Common.CFG
{
	public class CatchHandlerData
	{
		public readonly TypeReference Type;

		public readonly BlockRange Range;

		public CatchHandlerData(TypeReference type, BlockRange range)
		{
			this.Type = type;
			this.Range = range;
		}
	}
}
