using Mono.Cecil;
using System;
using System.Collections.Generic;

namespace Unity.IL2CPP.Marshaling.MarshalInfoWriters
{
	public class ComSafeArrayMarshalInfoWriter : MarshalableMarshalInfoWriter
	{
		private readonly TypeReference _elementType;

		private readonly SafeArrayMarshalInfo _marshalInfo;

		private readonly DefaultMarshalInfoWriter _elementTypeMarshalInfoWriter;

		private readonly string _marshaledTypeName;

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

		public ComSafeArrayMarshalInfoWriter(ArrayType type, MarshalInfo marshalInfo) : base(type)
		{
			this._elementType = type.ElementType;
			this._marshalInfo = (marshalInfo as SafeArrayMarshalInfo);
			if (this._marshalInfo == null)
			{
				throw new InvalidOperationException(string.Format("SafeArray type '{0}' has invalid MarshalAsAttribute.", type.FullName));
			}
			if (this._marshalInfo.ElementType == VariantType.BStr && this._elementType.MetadataType != MetadataType.String)
			{
				throw new InvalidOperationException(string.Format("SafeArray(BSTR) type '{0}' has invalid MarshalAsAttribute.", type.FullName));
			}
			NativeType nativeElementType = this.GetNativeElementType();
			this._elementTypeMarshalInfoWriter = MarshalDataCollector.MarshalInfoWriterFor(this._elementType, MarshalType.COM, new MarshalInfo(nativeElementType), false, false);
			this._marshaledTypeName = string.Format("Il2CppSafeArray/*{0}*/*", this._marshalInfo.ElementType.ToString().ToUpper());
		}

		private NativeType GetNativeElementType()
		{
			switch (this._marshalInfo.ElementType)
			{
			case VariantType.I2:
			{
				NativeType result = NativeType.I2;
				return result;
			}
			case VariantType.I4:
			{
				NativeType result = NativeType.I4;
				return result;
			}
			case VariantType.R4:
			{
				NativeType result = NativeType.R4;
				return result;
			}
			case VariantType.R8:
			{
				NativeType result = NativeType.R8;
				return result;
			}
			case VariantType.BStr:
			{
				NativeType result = NativeType.BStr;
				return result;
			}
			case VariantType.Dispatch:
			{
				NativeType result = NativeType.IDispatch;
				return result;
			}
			case VariantType.Bool:
			{
				NativeType result = NativeType.VariantBool;
				return result;
			}
			case VariantType.Unknown:
			{
				NativeType result = NativeType.IUnknown;
				return result;
			}
			case VariantType.I1:
			{
				NativeType result = NativeType.I1;
				return result;
			}
			case VariantType.UI1:
			{
				NativeType result = NativeType.U1;
				return result;
			}
			case VariantType.UI2:
			{
				NativeType result = NativeType.U2;
				return result;
			}
			case VariantType.UI4:
			{
				NativeType result = NativeType.U4;
				return result;
			}
			case VariantType.Int:
			{
				NativeType result = NativeType.Int;
				return result;
			}
			case VariantType.UInt:
			{
				NativeType result = NativeType.UInt;
				return result;
			}
			}
			throw new NotSupportedException(string.Format("SafeArray element type {0} is not supported.", this._marshalInfo.ElementType));
		}

		public override void WriteMarshaledTypeForwardDeclaration(CppCodeWriter writer)
		{
			this._elementTypeMarshalInfoWriter.WriteMarshaledTypeForwardDeclaration(writer);
		}

		public override void WriteIncludesForFieldDeclaration(CppCodeWriter writer)
		{
			this._elementTypeMarshalInfoWriter.WriteIncludesForFieldDeclaration(writer);
		}

		public override void WriteIncludesForMarshaling(CppCodeWriter writer)
		{
			this._elementTypeMarshalInfoWriter.WriteIncludesForMarshaling(writer);
			base.WriteIncludesForMarshaling(writer);
		}

		public override void WriteMarshalVariableToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string destinationVariable, string managedVariableName, IRuntimeMetadataAccess metadataAccess)
		{
			if (this._marshalInfo.ElementType == VariantType.BStr)
			{
				writer.WriteLine("{0} = il2cpp_codegen_com_marshal_safe_array_bstring((Il2CppCodeGenArray*){1});", new object[]
				{
					destinationVariable,
					sourceVariable.Load()
				});
			}
			else
			{
				writer.WriteLine("{0} = il2cpp_codegen_com_marshal_safe_array(IL2CPP_VT_{1}, (Il2CppCodeGenArray*){2});", new object[]
				{
					destinationVariable,
					this._marshalInfo.ElementType.ToString().ToUpper(),
					sourceVariable.Load()
				});
			}
		}

		public override void WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
		{
			if (this._marshalInfo.ElementType == VariantType.BStr)
			{
				writer.WriteLine(destinationVariable.Store("({0}*)il2cpp_codegen_com_marshal_safe_array_bstring_result({1}, {2})", new object[]
				{
					DefaultMarshalInfoWriter.Naming.ForType(this._typeRef),
					metadataAccess.TypeInfoFor(this._elementType),
					variableName
				}));
			}
			else
			{
				writer.WriteLine(destinationVariable.Store("({0}*)il2cpp_codegen_com_marshal_safe_array_result(IL2CPP_VT_{1}, {2}, {3})", new object[]
				{
					DefaultMarshalInfoWriter.Naming.ForType(this._typeRef),
					this._marshalInfo.ElementType.ToString().ToUpper(),
					metadataAccess.TypeInfoFor(this._elementType),
					variableName
				}));
			}
		}

		public override void WriteMarshalCleanupVariable(CppCodeWriter writer, string variableName, IRuntimeMetadataAccess metadataAccess, string managedVariableName = null)
		{
			writer.WriteLine("il2cpp_codegen_com_destroy_safe_array({0});", new object[]
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
