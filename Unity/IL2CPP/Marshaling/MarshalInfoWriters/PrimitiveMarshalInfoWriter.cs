namespace Unity.IL2CPP.Marshaling.MarshalInfoWriters
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Marshaling;

    internal sealed class PrimitiveMarshalInfoWriter : DefaultMarshalInfoWriter
    {
        private readonly string _marshaledTypeName;
        private readonly MarshaledType[] _marshaledTypes;
        private readonly string _nativeSize;
        private readonly int _nativeSizeWithoutPointers;

        public PrimitiveMarshalInfoWriter(TypeReference type, MarshalInfo marshalInfo, MarshalType marshalType) : base(type)
        {
            this._marshaledTypeName = DefaultMarshalInfoWriter.Naming.ForVariable(type);
            switch (type.MetadataType)
            {
                case MetadataType.Void:
                    this._nativeSizeWithoutPointers = 1;
                    this._nativeSize = "1";
                    break;

                case MetadataType.Boolean:
                    if (marshalType == MarshalType.WindowsRuntime)
                    {
                        this._nativeSizeWithoutPointers = 1;
                        this._nativeSize = "1";
                        this._marshaledTypeName = "bool";
                        break;
                    }
                    this._nativeSizeWithoutPointers = 4;
                    this._nativeSize = "4";
                    this._marshaledTypeName = "int32_t";
                    break;

                case MetadataType.Char:
                    if (marshalType == MarshalType.WindowsRuntime)
                    {
                        this._nativeSizeWithoutPointers = 2;
                        this._nativeSize = "2";
                        this._marshaledTypeName = "Il2CppChar";
                        break;
                    }
                    this._nativeSizeWithoutPointers = 1;
                    this._nativeSize = "1";
                    this._marshaledTypeName = "uint8_t";
                    break;

                case MetadataType.SByte:
                case MetadataType.Byte:
                    this._nativeSizeWithoutPointers = 1;
                    break;

                case MetadataType.Int16:
                case MetadataType.UInt16:
                    this._nativeSizeWithoutPointers = 2;
                    break;

                case MetadataType.Int32:
                case MetadataType.UInt32:
                case MetadataType.Single:
                    this._nativeSizeWithoutPointers = 4;
                    break;

                case MetadataType.Int64:
                case MetadataType.UInt64:
                case MetadataType.Double:
                    this._nativeSizeWithoutPointers = 8;
                    break;

                case MetadataType.Pointer:
                    this._nativeSizeWithoutPointers = 0;
                    break;

                case MetadataType.IntPtr:
                    this._marshaledTypeName = DefaultMarshalInfoWriter.Naming.ForIntPtrT;
                    this._nativeSizeWithoutPointers = 0;
                    break;

                case MetadataType.UIntPtr:
                    this._marshaledTypeName = DefaultMarshalInfoWriter.Naming.ForUIntPtrT;
                    this._nativeSizeWithoutPointers = 0;
                    break;
            }
            if (marshalInfo != null)
            {
                switch (marshalInfo.NativeType)
                {
                    case NativeType.Boolean:
                    case NativeType.I4:
                        this._nativeSize = "4";
                        this._nativeSizeWithoutPointers = 4;
                        this._marshaledTypeName = "int32_t";
                        break;

                    case NativeType.I1:
                        this._nativeSize = "1";
                        this._nativeSizeWithoutPointers = 1;
                        this._marshaledTypeName = "int8_t";
                        break;

                    case NativeType.U1:
                        this._nativeSize = "1";
                        this._nativeSizeWithoutPointers = 1;
                        this._marshaledTypeName = "uint8_t";
                        break;

                    case NativeType.I2:
                        this._nativeSize = "2";
                        this._nativeSizeWithoutPointers = 2;
                        this._marshaledTypeName = "int16_t";
                        break;

                    case NativeType.U2:
                        this._nativeSize = "2";
                        this._nativeSizeWithoutPointers = 2;
                        this._marshaledTypeName = "uint16_t";
                        break;

                    case NativeType.U4:
                        this._nativeSize = "4";
                        this._nativeSizeWithoutPointers = 4;
                        this._marshaledTypeName = "uint32_t";
                        break;

                    case NativeType.I8:
                        this._nativeSize = "8";
                        this._nativeSizeWithoutPointers = 8;
                        this._marshaledTypeName = "int64_t";
                        break;

                    case NativeType.U8:
                        this._nativeSize = "8";
                        this._nativeSizeWithoutPointers = 8;
                        this._marshaledTypeName = "uint64_t";
                        break;

                    case NativeType.R4:
                        this._nativeSize = "4";
                        this._nativeSizeWithoutPointers = 4;
                        this._marshaledTypeName = "float";
                        break;

                    case NativeType.R8:
                        this._nativeSize = "8";
                        this._nativeSizeWithoutPointers = 8;
                        this._marshaledTypeName = "double";
                        break;

                    case NativeType.Int:
                        this._nativeSize = "sizeof(void*)";
                        this._nativeSizeWithoutPointers = 0;
                        this._marshaledTypeName = DefaultMarshalInfoWriter.Naming.ForIntPtrT;
                        break;

                    case NativeType.UInt:
                        this._nativeSize = "sizeof(void*)";
                        this._nativeSizeWithoutPointers = 0;
                        this._marshaledTypeName = DefaultMarshalInfoWriter.Naming.ForUIntPtrT;
                        break;

                    case NativeType.VariantBool:
                        this._nativeSize = "2";
                        this._nativeSizeWithoutPointers = 2;
                        this._marshaledTypeName = "IL2CPP_VARIANT_BOOL";
                        break;
                }
            }
            this._marshaledTypes = new MarshaledType[] { new MarshaledType(this._marshaledTypeName, this._marshaledTypeName) };
            if (this._nativeSize == null)
            {
                this._nativeSize = base.NativeSize;
            }
        }

        private string GetIntPtrTypeName() => 
            ((base._typeRef.MetadataType != MetadataType.IntPtr) ? DefaultMarshalInfoWriter.Naming.ForUIntPtrT : DefaultMarshalInfoWriter.Naming.ForIntPtrT);

        private string GetIntPtrValueSetterName()
        {
            <GetIntPtrValueSetterName>c__AnonStorey1 storey = new <GetIntPtrValueSetterName>c__AnonStorey1 {
                fieldManagedName = (base._typeRef.MetadataType != MetadataType.IntPtr) ? DefaultMarshalInfoWriter.Naming.UIntPtrPointerField : DefaultMarshalInfoWriter.Naming.IntPtrValueField
            };
            return DefaultMarshalInfoWriter.Naming.ForFieldSetter(base._typeRef.Resolve().Fields.First<FieldDefinition>(new Func<FieldDefinition, bool>(storey.<>m__0)));
        }

        private static string MarshalVariantBoolFromNative(string variableName) => 
            $"(({variableName}) != IL2CPP_VARIANT_FALSE)";

        private static string MarshalVariantBoolToNative(string variableName) => 
            $"(({variableName}) ? IL2CPP_VARIANT_TRUE : IL2CPP_VARIANT_FALSE)";

        public override void WriteIncludesForFieldDeclaration(CppCodeWriter writer)
        {
        }

        public override void WriteIncludesForMarshaling(CppCodeWriter writer)
        {
        }

        private void WriteIntPtrFieldAssignmentToLocalVariable(CppCodeWriter writer, string variableName, string destinationVariable)
        {
            object[] args = new object[] { destinationVariable, this.GetIntPtrValueSetterName(), this.GetIntPtrTypeName(), variableName };
            writer.WriteLine("{0}.{1}(reinterpret_cast<void*>(({2})({3})));", args);
        }

        private void WriteIntPtrFieldAssignmentToManagedValue(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable)
        {
            string str = destinationVariable.GetNiceName() + "_temp";
            object[] args = new object[] { DefaultMarshalInfoWriter.Naming.ForType(base._typeRef), str };
            writer.WriteLine("{0} {1};", args);
            this.WriteIntPtrFieldAssignmentToLocalVariable(writer, variableName, str);
            writer.WriteLine(destinationVariable.Store(str));
        }

        public override void WriteMarshaledTypeForwardDeclaration(CppCodeWriter writer)
        {
        }

        public override string WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
        {
            if ((base._typeRef.MetadataType == MetadataType.Boolean) && (this._marshaledTypeName == "IL2CPP_VARIANT_BOOL"))
            {
                return MarshalVariantBoolFromNative(variableName);
            }
            if ((base._typeRef.MetadataType == MetadataType.IntPtr) || (base._typeRef.MetadataType == MetadataType.UIntPtr))
            {
                string destinationVariable = $"_{DefaultMarshalInfoWriter.CleanVariableName(variableName)}_unmarshaled";
                object[] args = new object[] { DefaultMarshalInfoWriter.Naming.ForVariable(base._typeRef), destinationVariable };
                writer.WriteLine("{0} {1};", args);
                this.WriteIntPtrFieldAssignmentToLocalVariable(writer, variableName, destinationVariable);
                return destinationVariable;
            }
            string str3 = DefaultMarshalInfoWriter.Naming.ForVariable(base._typeRef);
            if (str3 != this._marshaledTypeName)
            {
                return $"static_cast<{str3}>({variableName})";
            }
            return variableName;
        }

        public override void WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
        {
            if ((base._typeRef.MetadataType == MetadataType.IntPtr) || (base._typeRef.MetadataType == MetadataType.UIntPtr))
            {
                this.WriteIntPtrFieldAssignmentToManagedValue(writer, variableName, destinationVariable);
            }
            else
            {
                writer.WriteLine(destinationVariable.Store(this.WriteMarshalVariableFromNative(writer, variableName, methodParameters, returnValue, forNativeWrapperOfManagedMethod, metadataAccess)));
            }
        }

        public override string WriteMarshalVariableToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string managedVariableName, IRuntimeMetadataAccess metadataAccess)
        {
            if ((base._typeRef.MetadataType == MetadataType.IntPtr) || (base._typeRef.MetadataType == MetadataType.UIntPtr))
            {
                <WriteMarshalVariableToNative>c__AnonStorey0 storey = new <WriteMarshalVariableToNative>c__AnonStorey0 {
                    fieldManagedName = (base._typeRef.MetadataType != MetadataType.IntPtr) ? DefaultMarshalInfoWriter.Naming.UIntPtrPointerField : DefaultMarshalInfoWriter.Naming.IntPtrValueField
                };
                string str = DefaultMarshalInfoWriter.Naming.ForFieldGetter(base._typeRef.Resolve().Fields.First<FieldDefinition>(new Func<FieldDefinition, bool>(storey.<>m__0)));
                if (this._marshaledTypeName == "intptr_t")
                {
                    return $"reinterpret_cast<intptr_t>(({sourceVariable.Load()}).{str}())";
                }
                return string.Format("static_cast<{2}>(reinterpret_cast<intptr_t>(({0}).{1}()))", sourceVariable.Load(), str, this._marshaledTypeName);
            }
            if ((base._typeRef.MetadataType == MetadataType.Boolean) && (this._marshaledTypeName == "IL2CPP_VARIANT_BOOL"))
            {
                return MarshalVariantBoolToNative(sourceVariable.Load());
            }
            if (DefaultMarshalInfoWriter.Naming.ForVariable(base._typeRef) != this._marshaledTypeName)
            {
                return $"static_cast<{this._marshaledTypeName}>({sourceVariable.Load()})";
            }
            return sourceVariable.Load();
        }

        public override void WriteMarshalVariableToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string destinationVariable, string managedVariableName, IRuntimeMetadataAccess metadataAccess)
        {
            object[] args = new object[] { destinationVariable, this.WriteMarshalVariableToNative(writer, sourceVariable, managedVariableName, metadataAccess) };
            writer.WriteLine("{0} = {1};", args);
        }

        public override void WriteNativeVariableDeclarationOfType(CppCodeWriter writer, string variableName)
        {
            if (base._typeRef.IsPointer)
            {
                base.WriteNativeVariableDeclarationOfType(writer, variableName);
            }
            else
            {
                string str = "0";
                switch (this._marshaledTypeName)
                {
                    case "float":
                        str = "0.0f";
                        break;

                    case "double":
                        str = "0.0";
                        break;

                    case "IL2CPP_VARIANT_BOOL":
                        str = "IL2CPP_VARIANT_FALSE";
                        break;
                }
                object[] args = new object[] { this._marshaledTypeName, variableName, str };
                writer.WriteLine("{0} {1} = {2};", args);
            }
        }

        public override MarshaledType[] MarshaledTypes =>
            this._marshaledTypes;

        public override string NativeSize =>
            this._nativeSize;

        public override int NativeSizeWithoutPointers =>
            this._nativeSizeWithoutPointers;

        [CompilerGenerated]
        private sealed class <GetIntPtrValueSetterName>c__AnonStorey1
        {
            internal string fieldManagedName;

            internal bool <>m__0(FieldDefinition f) => 
                (f.Name == this.fieldManagedName);
        }

        [CompilerGenerated]
        private sealed class <WriteMarshalVariableToNative>c__AnonStorey0
        {
            internal string fieldManagedName;

            internal bool <>m__0(FieldDefinition f) => 
                (f.Name == this.fieldManagedName);
        }
    }
}

