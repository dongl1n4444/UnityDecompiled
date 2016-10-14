using System;

namespace Unity.IL2CPP
{
	internal struct InvokerData : IEquatable<InvokerData>
	{
		public readonly bool VoidReturn;

		public readonly int ParameterCount;

		public readonly bool Com;

		public InvokerData(bool voidReturn, int parameterCount, bool com = false)
		{
			this.VoidReturn = voidReturn;
			this.ParameterCount = parameterCount;
			this.Com = com;
		}

		public override int GetHashCode()
		{
			return this.VoidReturn.GetHashCode() ^ this.ParameterCount.GetHashCode() ^ this.Com.GetHashCode();
		}

		public bool Equals(InvokerData other)
		{
			return this.VoidReturn == other.VoidReturn && this.ParameterCount == other.ParameterCount && this.Com == other.Com;
		}
	}
}
