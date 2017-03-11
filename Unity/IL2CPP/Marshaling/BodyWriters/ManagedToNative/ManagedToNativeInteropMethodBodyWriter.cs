namespace Unity.IL2CPP.Marshaling.BodyWriters.ManagedToNative
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Marshaling;
    using Unity.IL2CPP.Marshaling.BodyWriters;
    using Unity.IL2CPP.Marshaling.MarshalInfoWriters;

    internal abstract class ManagedToNativeInteropMethodBodyWriter : InteropMethodBodyWriter
    {
        public ManagedToNativeInteropMethodBodyWriter(MethodReference interopMethod, MethodReference methodForParameterNames, MarshalType marshalType, bool useUnicodeCharset) : base(interopMethod, methodForParameterNames, new ManagedToNativeMarshaler(TypeResolver.For(interopMethod.DeclaringType, interopMethod), marshalType, useUnicodeCharset))
        {
        }

        protected string GetFunctionCallParametersExpression(string[] localVariableNames)
        {
            List<string> elements = new List<string>();
            for (int i = 0; i < localVariableNames.Length; i++)
            {
                DefaultMarshalInfoWriter writer = base.MarshalInfoWriterFor(base.Parameters[i]);
                foreach (MarshaledType type in writer.MarshaledTypes)
                {
                    string marshaledVariableName = localVariableNames[i] + type.VariableName;
                    string item = writer.DecorateVariable(base.Parameters[i].NameInGeneratedCode, marshaledVariableName);
                    elements.Add(item);
                }
            }
            DefaultMarshalInfoWriter writer2 = base.MarshalInfoWriterFor(base.GetMethodReturnType());
            MarshaledType[] marshaledTypes = base.MarshalInfoWriterFor(base.GetMethodReturnType()).MarshaledTypes;
            for (int j = 0; j < (marshaledTypes.Length - 1); j++)
            {
                string str3 = InteropMethodInfo.Naming.ForInteropReturnValue() + marshaledTypes[j].VariableName;
                elements.Add("&" + writer2.DecorateVariable(null, str3));
            }
            return elements.AggregateWithComma();
        }

        protected override void WriteMethodEpilogue(CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
        {
        }
    }
}

