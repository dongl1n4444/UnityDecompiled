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
			string marshaledVariableName = variableName.Replace("*", "");
			string text = string.Format("_{0}_unmarshaled", DefaultMarshalInfoWriter.CleanVariableName(variableName));
			this.WriteDeclareAndAllocateObject(writer, text, marshaledVariableName, metadataAccess);
			this.WriteMarshalVariableFromNative(writer, variableName, new ManagedMarshalValue(text), methodParameters, returnValue, forNativeWrapperOfManagedMethod, metadataAccess);
			return text;
		}

		public override string WriteMarshalEmptyVariableToNative(CppCodeWriter writer, ManagedMarshalValue variableName, IList<MarshaledParameter> methodParameters)
		{
			string text = string.Format("_{0}_marshaled", variableName.GetNiceName());
			this.WriteNativeVariableDeclarationOfType(writer, text);
			return text;
		}
	}
}
