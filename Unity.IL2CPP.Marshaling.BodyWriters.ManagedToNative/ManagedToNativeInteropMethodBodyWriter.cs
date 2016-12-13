using Mono.Cecil;
using System;
using System.Collections.Generic;
using Unity.IL2CPP.ILPreProcessor;
using Unity.IL2CPP.Marshaling.MarshalInfoWriters;

namespace Unity.IL2CPP.Marshaling.BodyWriters.ManagedToNative
{
	internal abstract class ManagedToNativeInteropMethodBodyWriter : InteropMethodBodyWriter
	{
		public ManagedToNativeInteropMethodBodyWriter(MethodReference interopMethod, MethodReference methodForParameterNames, MarshalType marshalType, bool useUnicodeCharset) : base(interopMethod, methodForParameterNames, new ManagedToNativeMarshaler(TypeResolver.For(interopMethod.DeclaringType, interopMethod), marshalType, useUnicodeCharset))
		{
		}

		protected override void WriteMethodEpilogue(CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
		{
		}

		protected string GetFunctionCallParametersExpression(string[] localVariableNames)
		{
			List<string> list = new List<string>();
			for (int i = 0; i < localVariableNames.Length; i++)
			{
				DefaultMarshalInfoWriter defaultMarshalInfoWriter = base.MarshalInfoWriterFor(this._parameters[i]);
				MarshaledType[] marshaledTypes = defaultMarshalInfoWriter.MarshaledTypes;
				for (int j = 0; j < marshaledTypes.Length; j++)
				{
					MarshaledType marshaledType = marshaledTypes[j];
					string marshaledVariableName = localVariableNames[i] + marshaledType.VariableName;
					string item = defaultMarshalInfoWriter.DecorateVariable(this._parameters[i].NameInGeneratedCode, marshaledVariableName);
					list.Add(item);
				}
			}
			DefaultMarshalInfoWriter defaultMarshalInfoWriter2 = base.MarshalInfoWriterFor(this.GetMethodReturnType());
			MarshaledType[] marshaledTypes2 = base.MarshalInfoWriterFor(this.GetMethodReturnType()).MarshaledTypes;
			for (int k = 0; k < marshaledTypes2.Length - 1; k++)
			{
				string marshaledVariableName2 = InteropMethodBodyWriter.Naming.ForInteropReturnValue() + marshaledTypes2[k].VariableName;
				string str = defaultMarshalInfoWriter2.DecorateVariable(null, marshaledVariableName2);
				list.Add("&" + str);
			}
			return list.AggregateWithComma();
		}
	}
}
