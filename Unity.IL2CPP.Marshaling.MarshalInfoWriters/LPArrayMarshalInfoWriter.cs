using Mono.Cecil;
using System;
using System.Collections.Generic;

namespace Unity.IL2CPP.Marshaling.MarshalInfoWriters
{
	internal sealed class LPArrayMarshalInfoWriter : ArrayMarshalInfoWriter
	{
		public LPArrayMarshalInfoWriter(ArrayType arrayType, MarshalType marshalType, MarshalInfo marshalInfo) : base(arrayType, marshalType, marshalInfo)
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
				string arraySizeVariable = base.WriteArraySizeFromManagedArray(writer, sourceVariable, destinationVariable);
				base.AllocateAndStoreNativeArray(writer, destinationVariable, arraySizeVariable);
				base.WriteMarshalToNativeLoop(writer, sourceVariable, destinationVariable, managedVariableName, metadataAccess, (CppCodeWriter bodyWriter) => arraySizeVariable);
			}
			writer.WriteLine("else");
			using (new BlockWriter(writer, false))
			{
				base.WriteAssignNullArray(writer, destinationVariable);
			}
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
				writer.WriteLine("if ({0} == {1})", new object[]
				{
					destinationVariable.Load(),
					DefaultMarshalInfoWriter.Naming.Null
				});
				using (new BlockWriter(writer, false))
				{
					base.AllocateAndStoreManagedArray(writer, destinationVariable, metadataAccess, base.MarshaledArraySizeFor(variableName, methodParameters));
				}
				base.WriteMarshalFromNativeLoop(writer, variableName, destinationVariable, methodParameters, returnValue, forNativeWrapperOfManagedMethod, metadataAccess, delegate(CppCodeWriter bodyWriter)
				{
					writer.WriteLine("int32_t {0} = ({1})->max_length;", new object[]
					{
						"_arrayLength",
						destinationVariable.Load()
					});
					return "_arrayLength";
				});
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
			writer.WriteLine("if ({0} != {1})", new object[]
			{
				variableName,
				DefaultMarshalInfoWriter.Naming.Null
			});
			using (new BlockWriter(writer, false))
			{
				base.WriteCleanupLoop(writer, variableName, metadataAccess, delegate(CppCodeWriter bodyWriter)
				{
					string arg = DefaultMarshalInfoWriter.CleanVariableName(variableName);
					string text = string.Format("{0}_CleanupLoopCount", arg);
					string text2 = (managedVariableName != null) ? string.Format("({0} != {1}) ? ({0})->max_length : 0", managedVariableName, DefaultMarshalInfoWriter.Naming.Null) : string.Format("{0}", this._arraySize);
					bodyWriter.WriteLine("const int32_t {0} = {1};", new object[]
					{
						text,
						text2
					});
					return text;
				});
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
}
