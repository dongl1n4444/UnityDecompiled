using System;

namespace Unity.IL2CPP.IoCServices
{
	public struct Range
	{
		public int start;

		public int length;

		public Range(int start_, int length_)
		{
			this.start = start_;
			this.length = length_;
		}
	}
}
