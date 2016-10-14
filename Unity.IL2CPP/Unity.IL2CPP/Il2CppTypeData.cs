using Mono.Cecil;
using System;

namespace Unity.IL2CPP
{
	public struct Il2CppTypeData
	{
		public readonly int Attrs;

		public readonly TypeReference Type;

		public Il2CppTypeData(TypeReference type, int attrs)
		{
			this.Type = type;
			this.Attrs = attrs;
		}

		public override string ToString()
		{
			return string.Format("{0} [{1}]", this.Type.FullName, this.Attrs);
		}
	}
}
