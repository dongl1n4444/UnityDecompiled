namespace Unity.IL2CPP.Marshaling.MarshalInfoWriters
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Marshaling;

    public class ComSafeArrayMarshalInfoWriter : MarshalableMarshalInfoWriter
    {
        private readonly TypeReference _elementType;
        private readonly DefaultMarshalInfoWriter _elementTypeMarshalInfoWriter;
        private readonly MarshaledType[] _marshaledTypes;
        private readonly SafeArrayMarshalInfo _marshalInfo;

        public ComSafeArrayMarshalInfoWriter(ArrayType type, MarshalInfo marshalInfo) : base(type)
        {
            this._elementType = type.ElementType;
            this._marshalInfo = marshalInfo as SafeArrayMarshalInfo;
            if (this._marshalInfo == null)
            {
                throw new InvalidOperationException($"SafeArray type '{type.FullName}' has invalid MarshalAsAttribute.");
            }
            if ((this._marshalInfo.ElementType == VariantType.BStr) && (this._elementType.MetadataType != MetadataType.String))
            {
                throw new InvalidOperationException($"SafeArray(BSTR) type '{type.FullName}' has invalid MarshalAsAttribute.");
            }
            NativeType nativeElementType = this.GetNativeElementType();
            this._elementTypeMarshalInfoWriter = MarshalDataCollector.MarshalInfoWriterFor(this._elementType, MarshalType.COM, new MarshalInfo(nativeElementType), false, false, false, null);
            string name = $"Il2CppSafeArray/*{this._marshalInfo.ElementType.ToString().ToUpper()}*/*";
            this._marshaledTypes = new MarshaledType[] { new MarshaledType(name, name) };
        }

        private NativeType GetNativeElementType()
        {
            switch (this._marshalInfo.ElementType)
            {
                case VariantType.I2:
                    return NativeType.I2;

                case VariantType.I4:
                    return NativeType.I4;

                case VariantType.R4:
                    return NativeType.R4;

                case VariantType.R8:
                    return NativeType.R8;

                case VariantType.BStr:
                    return NativeType.BStr;

                case VariantType.Dispatch:
                    return NativeType.IDispatch;

                case VariantType.Bool:
                    return NativeType.VariantBool;

                case VariantType.Unknown:
                    return NativeType.IUnknown;

                case VariantType.I1:
                    return NativeType.I1;

                case VariantType.UI1:
                    return NativeType.U1;

                case VariantType.UI2:
                    return NativeType.U2;

                case VariantType.UI4:
                    return NativeType.U4;

                case VariantType.Int:
                    return NativeType.Int;

                case VariantType.UInt:
                    return NativeType.UInt;
            }
            throw new NotSupportedException($"SafeArray element type {this._marshalInfo.ElementType} is not supported.");
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

        public override void WriteMarshalCleanupVariable(CppCodeWriter writer, string variableName, IRuntimeMetadataAccess metadataAccess, string managedVariableName = null)
        {
            object[] args = new object[] { variableName };
            writer.WriteLine("il2cpp_codegen_com_destroy_safe_array({0});", args);
            object[] objArray2 = new object[] { variableName, DefaultMarshalInfoWriter.Naming.Null };
            writer.WriteLine("{0} = {1};", objArray2);
        }

        public override void WriteMarshaledTypeForwardDeclaration(CppCodeWriter writer)
        {
            this._elementTypeMarshalInfoWriter.WriteMarshaledTypeForwardDeclaration(writer);
        }

        public override void WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
        {
            if (this._marshalInfo.ElementType == VariantType.BStr)
            {
                object[] args = new object[] { DefaultMarshalInfoWriter.Naming.ForType(base._typeRef), metadataAccess.TypeInfoFor(this._elementType), variableName };
                writer.WriteLine(destinationVariable.Store("({0}*)il2cpp_codegen_com_marshal_safe_array_bstring_result({1}, {2})", args));
            }
            else
            {
                object[] objArray2 = new object[] { DefaultMarshalInfoWriter.Naming.ForType(base._typeRef), this._marshalInfo.ElementType.ToString().ToUpper(), metadataAccess.TypeInfoFor(this._elementType), variableName };
                writer.WriteLine(destinationVariable.Store("({0}*)il2cpp_codegen_com_marshal_safe_array_result(IL2CPP_VT_{1}, {2}, {3})", objArray2));
            }
        }

        public override void WriteMarshalVariableToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string destinationVariable, string managedVariableName, IRuntimeMetadataAccess metadataAccess)
        {
            if (this._marshalInfo.ElementType == VariantType.BStr)
            {
                object[] args = new object[] { destinationVariable, sourceVariable.Load() };
                writer.WriteLine("{0} = il2cpp_codegen_com_marshal_safe_array_bstring({1});", args);
            }
            else
            {
                object[] objArray2 = new object[] { destinationVariable, this._marshalInfo.ElementType.ToString().ToUpper(), sourceVariable.Load() };
                writer.WriteLine("{0} = il2cpp_codegen_com_marshal_safe_array(IL2CPP_VT_{1}, {2});", objArray2);
            }
        }

        public override MarshaledType[] MarshaledTypes =>
            this._marshaledTypes;

        public override string NativeSize =>
            "-1";
    }
}

