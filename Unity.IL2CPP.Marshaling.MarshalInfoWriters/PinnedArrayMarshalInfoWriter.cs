using Mono.Cecil;
using System;
using System.Collections.Generic;

namespace Unity.IL2CPP.Marshaling.MarshalInfoWriters
{
	internal class PinnedArrayMarshalInfoWriter : ArrayMarshalInfoWriter
	{
		public PinnedArrayMarshalInfoWriter(ArrayType arrayType, MarshalType marshalType, MarshalInfo marshalInfo) : base(arrayType, marshalType, marshalInfo)
		{
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
				if (this._arraySizeSelection == ArrayMarshalInfoWriter.ArraySizeOptions.UseFirstMarshaledType)
				{
					writer.WriteLine("{0}{1} = static_cast<uint32_t>(({2})->max_length);", new object[]
					{
						destinationVariable,
						this.MarshaledTypes[0].VariableName,
						sourceVariable.Load()
					});
				}
				writer.WriteLine("{0} = reinterpret_cast<{1}>(({2})->{3}(0));", new object[]
				{
					destinationVariable,
					this._arrayMarshaledTypeName,
					sourceVariable.Load(),
					DefaultMarshalInfoWriter.Naming.ForArrayItemAddressGetter(false)
				});
			}
		}

		public override void WriteMarshalOutParameterToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string destinationVariable, string managedVariableName, IList<MarshaledParameter> methodParameters, IRuntimeMetadataAccess metadataAccess)
		{
			writer.WriteLine("if ({0} != {1})", new object[]
			{
				sourceVariable.Load(),
				DefaultMarshalInfoWriter.Naming.Null
			});
			using (new BlockWriter(writer, false))
			{
				base.WriteMarshalToNativeLoop(writer, sourceVariable, destinationVariable, managedVariableName, metadataAccess, (CppCodeWriter bodyWriter) => this.WriteArraySizeFromManagedArray(bodyWriter, sourceVariable, destinationVariable));
			}
		}

		public override string WriteMarshalReturnValueToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, IRuntimeMetadataAccess metadataAccess)
		{
			string text = string.Format("_{0}_marshaled", sourceVariable.GetNiceName());
			this.WriteNativeVariableDeclarationOfType(writer, text);
			writer.WriteLine("if ({0} != {1})", new object[]
			{
				sourceVariable.Load(),
				DefaultMarshalInfoWriter.Naming.Null
			});
			using (new BlockWriter(writer, false))
			{
				string arraySizeVariable = base.WriteArraySizeFromManagedArray(writer, sourceVariable, text);
				base.AllocateAndStoreNativeArray(writer, text, arraySizeVariable);
				base.WriteMarshalToNativeLoop(writer, sourceVariable, text, null, metadataAccess, (CppCodeWriter bodyWriter) => arraySizeVariable);
			}
			return text;
		}

		public override void WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
		{
			writer.WriteLine("if ({0} != {1})", new object[]
			{
				variableName,
				DefaultMarshalInfoWriter.Naming.Null
			});
			using (new BlockWriter(writer, false))
			{
				string arraySize = base.MarshaledArraySizeFor(variableName, methodParameters);
				base.AllocateAndStoreManagedArray(writer, destinationVariable, metadataAccess, arraySize);
				base.WriteMarshalFromNativeLoop(writer, variableName, destinationVariable, methodParameters, returnValue, forNativeWrapperOfManagedMethod, metadataAccess, (CppCodeWriter bodyWriter) => arraySize);
			}
		}

		public override void WriteMarshalOutParameterFromNative(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
		{
			writer.WriteLine("if ({0} != {1})", new object[]
			{
				variableName,
				DefaultMarshalInfoWriter.Naming.Null
			});
			using (new BlockWriter(writer, false))
			{
				base.WriteMarshalFromNativeLoop(writer, variableName, destinationVariable, methodParameters, returnValue, forNativeWrapperOfManagedMethod, metadataAccess, (CppCodeWriter bodyWriter) => this.WriteArraySizeFromManagedArray(bodyWriter, destinationVariable, variableName));
			}
		}

		public override void WriteMarshalCleanupVariable(CppCodeWriter writer, string variableName, IRuntimeMetadataAccess metadataAccess, string managedVariableName = null)
		{
		}

		public override void WriteMarshalCleanupEmptyVariable(CppCodeWriter writer, string variableName, IRuntimeMetadataAccess metadataAccess, string managedVariableName = null)
		{
			writer.WriteLine("il2cpp_codegen_marshal_free({0});", new object[]
			{
				variableName
			});
			writer.WriteLine("{0} = {1};", new object[]
			{
				variableName,
				DefaultMarshalInfoWriter.Naming.Null
			});
		}

		public override void WriteMarshalCleanupReturnValue(CppCodeWriter writer, string variableName, IRuntimeMetadataAccess metadataAccess)
		{
			writer.WriteLine("il2cpp_codegen_marshal_free({0});", new object[]
			{
				variableName
			});
			writer.WriteLine("{0} = {1};", new object[]
			{
				variableName,
				DefaultMarshalInfoWriter.Naming.Null
			});
		}
	}
}
