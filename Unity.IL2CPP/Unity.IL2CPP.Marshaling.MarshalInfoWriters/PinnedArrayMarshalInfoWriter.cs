using Mono.Cecil;
using System;
using System.Collections.Generic;

namespace Unity.IL2CPP.Marshaling.MarshalInfoWriters
{
	public sealed class PinnedArrayMarshalInfoWriter : ArrayMarshalInfoWriter
	{
		public PinnedArrayMarshalInfoWriter(ArrayType arrayType, MarshalType marshalType, MarshalInfo marshalInfo) : base(arrayType, marshalType, marshalInfo)
		{
		}

		public override void WriteMarshalVariableToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string destinationVariable, string managedVariableName, IRuntimeMetadataAccess metadataAccess)
		{
			writer.WriteLine("{0} = il2cpp_codegen_marshal_array<{1}>((Il2CppCodeGenArray*){2});", new object[]
			{
				destinationVariable,
				this._elementTypeMarshalInfoWriter.MarshaledTypeName,
				sourceVariable.Load()
			});
		}

		public override void WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
		{
			writer.WriteLine(destinationVariable.Store("({0}*)il2cpp_codegen_marshal_array_result({1}, {2}, {3})", new object[]
			{
				DefaultMarshalInfoWriter.Naming.ForType(this._typeRef),
				metadataAccess.TypeInfoFor(this._elementType),
				variableName,
				base.MarshaledArraySizeFor(methodParameters)
			}));
		}

		public override void WriteMarshalOutParameterFromNative(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
		{
			MarshalingUtils.EmitNullCheckFor(writer, variableName, string.Format("il2cpp_codegen_marshal_array_out<{0}>({1}, {2});", this._elementTypeMarshalInfoWriter.MarshaledTypeName, variableName, destinationVariable.Load()));
		}

		public override void WriteMarshalCleanupEmptyVariable(CppCodeWriter writer, string variableName, IRuntimeMetadataAccess metadataAccess, string managedVariableName = null)
		{
			MarshalingUtils.EmitNullCheckFor(writer, variableName, string.Format("il2cpp_codegen_marshal_free({0});", variableName));
		}
	}
}
