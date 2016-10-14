using Mono.Cecil;
using System;
using System.Collections.Generic;

namespace Unity.IL2CPP.Marshaling.MarshalInfoWriters
{
	internal sealed class StringArrayMarshalInfoWriter : ArrayMarshalInfoWriter
	{
		public StringArrayMarshalInfoWriter(ArrayType arrayType, MarshalType marshalType, MarshalInfo marshalInfo) : base(arrayType, marshalType, marshalInfo)
		{
			if (this._elementType.MetadataType != MetadataType.String)
			{
				throw new NotSupportedException(string.Format("Unknown element type in StringArrayMarshalInfoWriter: {0}", this._elementType));
			}
		}

		public override void WriteMarshalVariableToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string destinationVariable, string managedVariableName, IRuntimeMetadataAccess metadataAccess)
		{
			string text;
			switch (this._nativeElementType)
			{
			case NativeType.BStr:
				text = "il2cpp_codegen_marshal_bstring_array";
				break;
			case NativeType.LPStr:
				text = "il2cpp_codegen_marshal_string_array";
				break;
			case NativeType.LPWStr:
				text = "il2cpp_codegen_marshal_wstring_array";
				break;
			default:
				throw new InvalidOperationException(string.Format("Unexpected string marshaling type '{0}'.", this._nativeElementType));
			}
			writer.WriteLine("{0} = {1}({2});", new object[]
			{
				destinationVariable,
				text,
				sourceVariable.Load()
			});
		}

		public override void WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
		{
			string text;
			switch (this._nativeElementType)
			{
			case NativeType.BStr:
				text = "il2cpp_codegen_marshal_bstring_array_result";
				break;
			case NativeType.LPStr:
				text = "il2cpp_codegen_marshal_string_array_result";
				break;
			case NativeType.LPWStr:
				text = "il2cpp_codegen_marshal_wstring_array_result";
				break;
			default:
				throw new InvalidOperationException(string.Format("Unexpected string marshaling type '{0}'.", this._nativeElementType));
			}
			writer.WriteLine(destinationVariable.Store("({0}*){1}({2}, {3})", new object[]
			{
				DefaultMarshalInfoWriter.Naming.ForType(this._typeRef),
				text,
				variableName,
				base.MarshaledArraySizeFor(methodParameters)
			}));
		}

		public override void WriteMarshalOutParameterFromNative(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
		{
			string arg;
			switch (this._nativeElementType)
			{
			case NativeType.BStr:
				arg = "il2cpp_codegen_marshal_bstring_array_out";
				break;
			case NativeType.LPStr:
				arg = "il2cpp_codegen_marshal_string_array_out";
				break;
			case NativeType.LPWStr:
				arg = "il2cpp_codegen_marshal_wstring_array_out";
				break;
			default:
				throw new InvalidOperationException(string.Format("Unexpected string marshaling type '{0}'.", this._nativeElementType));
			}
			MarshalingUtils.EmitNullCheckFor(writer, variableName, string.Format("{0}({1}, {2});", arg, variableName, destinationVariable.Load()));
		}

		public override string WriteMarshalEmptyVariableToNative(CppCodeWriter writer, string variableName)
		{
			string text = string.Format("_{0}_marshaled", variableName);
			string text2;
			switch (this._nativeElementType)
			{
			case NativeType.BStr:
				text2 = "il2cpp_codegen_marshal_allocate_native_bstring_array";
				break;
			case NativeType.LPStr:
				text2 = "il2cpp_codegen_marshal_allocate_native_string_array";
				break;
			case NativeType.LPWStr:
				text2 = "il2cpp_codegen_marshal_allocate_native_wstring_array";
				break;
			default:
				throw new InvalidOperationException(string.Format("Unexpected string marshaling type '{0}'.", this._nativeElementType));
			}
			writer.WriteLine("{0} {1} = {2} != NULL ? {3}({4}) : NULL;", new object[]
			{
				this.MarshaledTypeName,
				text,
				variableName,
				text2,
				base.ManagedArraySizeFor(variableName)
			});
			return text;
		}

		public override void WriteMarshalCleanupVariable(CppCodeWriter writer, string variableName, IRuntimeMetadataAccess metadataAccess, string managedVariableName = null)
		{
			string text = (managedVariableName != null) ? string.Format("((Il2CppCodeGenArray*){0})->max_length", managedVariableName) : this._arraySize.ToString();
			string text2 = (managedVariableName != null) ? string.Format("if ({0} != NULL) ", managedVariableName) : string.Empty;
			if (this._nativeElementType == NativeType.BStr)
			{
				writer.WriteLine("{0}il2cpp_codegen_marshal_free_bstring_array({1}, {2});", new object[]
				{
					text2,
					variableName,
					text
				});
			}
			else
			{
				writer.WriteLine("{0}il2cpp_codegen_marshal_free_string_array((void**){1}, {2});", new object[]
				{
					text2,
					variableName,
					text
				});
			}
			writer.WriteLine("{0} = NULL;", new object[]
			{
				variableName
			});
		}
	}
}
