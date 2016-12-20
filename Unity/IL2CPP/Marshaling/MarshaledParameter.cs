namespace Unity.IL2CPP.Marshaling
{
    using Mono.Cecil;
    using System;

    public class MarshaledParameter
    {
        public readonly bool IsIn;
        public readonly bool IsOut;
        public readonly Mono.Cecil.MarshalInfo MarshalInfo;
        public readonly string Name;
        public readonly string NameInGeneratedCode;
        public readonly TypeReference ParameterType;

        public MarshaledParameter(string name, string nameInGeneratedCode, TypeReference parameterType, Mono.Cecil.MarshalInfo marshalInfo, bool isIn, bool isOut)
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

