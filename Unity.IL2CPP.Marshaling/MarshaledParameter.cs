using Mono.Cecil;
using System;

namespace Unity.IL2CPP.Marshaling
{
	public class MarshaledParameter
	{
		public readonly string Name;

		public readonly string NameInGeneratedCode;

		public readonly TypeReference ParameterType;

		public readonly MarshalInfo MarshalInfo;

		public readonly bool IsIn;

		public readonly bool IsOut;

		public MarshaledParameter(string name, string nameInGeneratedCode, TypeReference parameterType, MarshalInfo marshalInfo, bool isIn, bool isOut)
		{
			this.Name = name;
			this.NameInGeneratedCode = nameInGeneratedCode;
			this.ParameterType = parameterType;
			this.MarshalInfo = marshalInfo;
			this.IsIn = isIn;
			this.IsOut = isOut;
		}
	}
}
