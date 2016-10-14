using Mono.Cecil;
using System;
using System.Text;
using Unity.IL2CPP.ILPreProcessor;
using Unity.IL2CPP.Marshaling.MarshalInfoWriters;

namespace Unity.IL2CPP.Marshaling.BodyWriters.ManagedToNative
{
	internal abstract class ManagedToNativeInteropMethodBodyWriter : InteropMethodBodyWriter
	{
		public ManagedToNativeInteropMethodBodyWriter(MethodReference interopMethod, MarshalType marshalType, bool useUnicodeCharset) : base(interopMethod, new ManagedToNativeMarshaler(TypeResolver.For(interopMethod.DeclaringType, interopMethod), marshalType, useUnicodeCharset))
		{
		}

		protected override void WriteMethodEpilogue(CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
		{
		}

		protected string GetFunctionCallParametersExpression(string[] localVariableNames)
		{
			string result;
			if (localVariableNames.Length == 0)
			{
				result = string.Empty;
			}
			else
			{
				MarshaledParameter[] parameters = this._parameters;
				StringBuilder stringBuilder = new StringBuilder(this.FunctionCallParameterFor(parameters[0], localVariableNames[0]));
				for (int i = 1; i < this._parameters.Length; i++)
				{
					stringBuilder.AppendFormat(", {0}", this.FunctionCallParameterFor(parameters[i], localVariableNames[i]));
				}
				result = stringBuilder.ToString();
			}
			return result;
		}

		private string FunctionCallParameterFor(MarshaledParameter parameter, string variableName)
		{
			DefaultMarshalInfoWriter defaultMarshalInfoWriter = base.MarshalInfoWriterFor(parameter);
			return defaultMarshalInfoWriter.DecorateVariable(parameter.NameInGeneratedCode, variableName);
		}
	}
}
