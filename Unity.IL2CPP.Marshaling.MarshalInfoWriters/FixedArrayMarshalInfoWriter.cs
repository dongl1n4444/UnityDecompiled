using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Unity.IL2CPP.Marshaling.MarshalInfoWriters
{
	internal sealed class FixedArrayMarshalInfoWriter : ArrayMarshalInfoWriter
	{
		public FixedArrayMarshalInfoWriter(ArrayType arrayType, MarshalType marshalType, MarshalInfo marshalInfo) : base(arrayType, marshalType, marshalInfo)
		{
		}

		public override void WriteFieldDeclaration(CppCodeWriter writer, FieldReference field, string fieldNameSuffix = null)
		{
			string text = DefaultMarshalInfoWriter.Naming.ForField(field) + fieldNameSuffix;
			writer.WriteLine("{0} {1}[{2}];", new object[]
			{
				this._elementTypeMarshalInfoWriter.MarshaledTypes[0].DecoratedName,
				text,
				this._arraySize
			});
		}

		public override void WriteMarshalVariableToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string destinationVariable, string managedVariableName, IRuntimeMetadataAccess metadataAccess)
		{
			writer.WriteLine("if ({0} != {1})", new object[]
			{
				sourceVariable.Load(),
				DefaultMarshalInfoWriter.Naming.Null
			});
			using (new BlockWriter(writer, false))
			{
				writer.WriteLine("if ({0} > ({1})->max_length)", new object[]
				{
					this._arraySize,
					sourceVariable.Load()
				});
				using (new BlockWriter(writer, false))
				{
					writer.WriteStatement(Emit.RaiseManagedException("il2cpp_codegen_get_argument_exception(\"\", \"Type could not be marshaled because the length of an embedded array instance does not match the declared length in the layout.\")"));
				}
				writer.WriteLine();
				base.WriteMarshalToNativeLoop(writer, sourceVariable, destinationVariable, managedVariableName, metadataAccess, (CppCodeWriter bodyWriter) => this._arraySize.ToString(CultureInfo.InvariantCulture));
			}
		}

		public override void WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
		{
			string arraySize = base.MarshaledArraySizeFor(variableName, methodParameters);
			base.AllocateAndStoreManagedArray(writer, destinationVariable, metadataAccess, arraySize);
			base.WriteMarshalFromNativeLoop(writer, variableName, destinationVariable, methodParameters, returnValue, forNativeWrapperOfManagedMethod, metadataAccess, (CppCodeWriter bodyWriter) => arraySize);
		}

		public override void WriteMarshalOutParameterFromNative(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
		{
			base.WriteMarshalFromNativeLoop(writer, variableName, destinationVariable, methodParameters, returnValue, forNativeWrapperOfManagedMethod, metadataAccess, (CppCodeWriter bodyWriter) => this.MarshaledArraySizeFor(variableName, methodParameters));
		}

		public override void WriteMarshalCleanupVariable(CppCodeWriter writer, string variableName, IRuntimeMetadataAccess metadataAccess, string managedVariableName = null)
		{
			base.WriteCleanupLoop(writer, variableName, metadataAccess, (CppCodeWriter bodyWriter) => this._arraySize.ToString(CultureInfo.InvariantCulture));
		}

		public override void WriteIncludesForFieldDeclaration(CppCodeWriter writer)
		{
			base.WriteIncludesForFieldDeclaration(writer);
			writer.AddIncludeForTypeDefinition(this._elementType);
		}
	}
}
