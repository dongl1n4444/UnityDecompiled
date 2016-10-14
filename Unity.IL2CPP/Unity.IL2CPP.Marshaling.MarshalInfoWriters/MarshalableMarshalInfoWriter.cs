using Mono.Cecil;
using System;
using System.Collections.Generic;

namespace Unity.IL2CPP.Marshaling.MarshalInfoWriters
{
	public abstract class MarshalableMarshalInfoWriter : DefaultMarshalInfoWriter
	{
		protected MarshalableMarshalInfoWriter(TypeReference type) : base(type)
		{
		}

		public sealed override string WriteMarshalVariableToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string managedVariableName, IRuntimeMetadataAccess metadataAccess)
		{
			string text = string.Format("_{0}_marshaled", sourceVariable.GetNiceName());
			this.WriteNativeVariableDeclarationOfType(writer, text);
			this.WriteMarshalVariableToNative(writer, sourceVariable, text, managedVariableName, metadataAccess);
			return text;
		}

		public override string WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
		{
			string text = variableName.Replace("*", "");
			string text2 = string.Format("_{0}_unmarshaled", text);
			this.WriteDeclareAndAllocateObject(writer, text2, text, metadataAccess);
			this.WriteMarshalVariableFromNative(writer, variableName, new ManagedMarshalValue(text2), methodParameters, returnValue, forNativeWrapperOfManagedMethod, metadataAccess);
			return text2;
		}

		public override string WriteMarshalEmptyVariableToNative(CppCodeWriter writer, string variableName)
		{
			string text = string.Format("_{0}_marshaled", variableName.Replace("*", ""));
			this.WriteNativeVariableDeclarationOfType(writer, text);
			return text;
		}

		public override string WriteMarshalEmptyVariableFromNative(CppCodeWriter writer, string variableName)
		{
			string arg = variableName.Replace("*", "");
			string text = string.Format("_{0}_unmarshaled", arg);
			writer.WriteVariable(this._typeRef, text);
			return text;
		}
	}
}
