using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Unity.IL2CPP.Marshaling.MarshalInfoWriters
{
	public abstract class ArrayMarshalInfoWriter : MarshalableMarshalInfoWriter
	{
		protected enum ArraySizeOptions
		{
			UseArraySize,
			UseSizeParameterIndex
		}

		protected readonly ArrayType _arrayType;

		protected readonly int _arraySize;

		protected readonly int _sizeParameterIndex;

		protected readonly ArrayMarshalInfoWriter.ArraySizeOptions _arraySizeSelection;

		protected readonly TypeReference _elementType;

		protected readonly MarshalInfo _marshalInfo;

		protected readonly DefaultMarshalInfoWriter _elementTypeMarshalInfoWriter;

		protected readonly NativeType _nativeElementType;

		protected readonly string _marshaledTypeName;

		public override string MarshaledTypeName
		{
			get
			{
				return this._marshaledTypeName;
			}
		}

		public override string NativeSize
		{
			get
			{
				return "-1";
			}
		}

		protected ArrayMarshalInfoWriter(ArrayType type, MarshalType marshalType, MarshalInfo marshalInfo) : base(type)
		{
			this._marshalInfo = marshalInfo;
			this._arrayType = type;
			this._elementType = type.ElementType;
			MarshalInfo marshalInfo2 = null;
			ArrayMarshalInfo arrayMarshalInfo = marshalInfo as ArrayMarshalInfo;
			FixedArrayMarshalInfo fixedArrayMarshalInfo = marshalInfo as FixedArrayMarshalInfo;
			this._arraySize = 1;
			this._nativeElementType = NativeType.None;
			if (arrayMarshalInfo != null)
			{
				this._arraySize = arrayMarshalInfo.Size;
				this._sizeParameterIndex = arrayMarshalInfo.SizeParameterIndex;
				this._arraySizeSelection = ((this._arraySize != 0) ? ArrayMarshalInfoWriter.ArraySizeOptions.UseArraySize : ArrayMarshalInfoWriter.ArraySizeOptions.UseSizeParameterIndex);
				this._nativeElementType = arrayMarshalInfo.ElementType;
				marshalInfo2 = new MarshalInfo(this._nativeElementType);
			}
			else if (fixedArrayMarshalInfo != null)
			{
				this._arraySize = fixedArrayMarshalInfo.Size;
				this._nativeElementType = fixedArrayMarshalInfo.ElementType;
				marshalInfo2 = new MarshalInfo(this._nativeElementType);
			}
			if (this._arraySize == -1)
			{
				this._arraySize = 1;
			}
			this._elementTypeMarshalInfoWriter = MarshalDataCollector.MarshalInfoWriterFor(this._elementType, marshalType, marshalInfo2, false, false);
			this._marshaledTypeName = string.Format("{0}*", this._elementTypeMarshalInfoWriter.MarshaledDecoratedTypeName);
			StringMarshalInfoWriter stringMarshalInfoWriter = this._elementTypeMarshalInfoWriter as StringMarshalInfoWriter;
			if (stringMarshalInfoWriter != null)
			{
				this._nativeElementType = stringMarshalInfoWriter.NativeType;
			}
		}

		public override void WriteMarshaledTypeForwardDeclaration(CppCodeWriter writer)
		{
			this._elementTypeMarshalInfoWriter.WriteMarshaledTypeForwardDeclaration(writer);
		}

		public override void WriteIncludesForFieldDeclaration(CppCodeWriter writer)
		{
			this._elementTypeMarshalInfoWriter.WriteMarshaledTypeForwardDeclaration(writer);
		}

		public override void WriteIncludesForMarshaling(CppCodeWriter writer)
		{
			writer.AddIncludeForTypeDefinition(this._arrayType);
			this._elementTypeMarshalInfoWriter.WriteIncludesForMarshaling(writer);
			base.WriteIncludesForMarshaling(writer);
		}

		public override string WriteMarshalEmptyVariableToNative(CppCodeWriter writer, string variableName)
		{
			string text = string.Format("_{0}_marshaled", variableName);
			writer.WriteLine("{0} {1} = {2} != NULL ? il2cpp_codegen_marshal_allocate_array<{3}>({4}) : NULL;", new object[]
			{
				this.MarshaledTypeName,
				text,
				variableName,
				this._elementTypeMarshalInfoWriter.MarshaledDecoratedTypeName,
				this.ManagedArraySizeFor(variableName)
			});
			return text;
		}

		public abstract override void WriteMarshalVariableToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string destinationVariable, string managedVariableName, IRuntimeMetadataAccess metadataAccess);

		public abstract override void WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess);

		protected string MarshaledArraySizeFor(IList<MarshaledParameter> methodParameters)
		{
			return (this._arraySizeSelection != ArrayMarshalInfoWriter.ArraySizeOptions.UseSizeParameterIndex) ? this._arraySize.ToString(CultureInfo.InvariantCulture) : methodParameters[this._sizeParameterIndex].NameInGeneratedCode;
		}

		protected string ManagedArraySizeFor(string variableName)
		{
			return string.Format("((Il2CppCodeGenArray*){0})->max_length", variableName);
		}

		public override bool CanMarshalTypeToNative()
		{
			return this._elementTypeMarshalInfoWriter.CanMarshalTypeToNative();
		}

		public override bool CanMarshalTypeFromNative()
		{
			return this._elementTypeMarshalInfoWriter.CanMarshalTypeFromNative();
		}

		public override string GetMarshalingException()
		{
			return this._elementTypeMarshalInfoWriter.GetMarshalingException();
		}
	}
}
