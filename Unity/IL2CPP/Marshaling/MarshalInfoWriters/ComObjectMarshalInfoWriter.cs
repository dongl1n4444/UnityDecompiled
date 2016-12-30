namespace Unity.IL2CPP.Marshaling.MarshalInfoWriters
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using Unity.IL2CPP;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;
    using Unity.IL2CPP.Marshaling;

    internal sealed class ComObjectMarshalInfoWriter : MarshalableMarshalInfoWriter
    {
        private readonly TypeReference _defaultInterface;
        private readonly string _interfaceTypeName;
        private readonly bool _isClass;
        private readonly bool _isSealed;
        private readonly string _managedTypeName;
        private readonly bool _marshalAsInspectable;
        private readonly MarshaledType[] _marshaledTypes;
        private readonly MarshalType _marshalType;
        private readonly TypeReference _windowsRuntimeType;
        public const NativeType kNativeTypeIInspectable = (NativeType.CustomMarshaler | NativeType.Boolean);
        [Inject]
        public static IWindowsRuntimeProjections WindowsRuntimeProjections;

        public ComObjectMarshalInfoWriter(TypeReference type, MarshalType marshalType, MarshalInfo marshalInfo) : base(type)
        {
            this._marshalAsInspectable = (marshalType == MarshalType.WindowsRuntime) || ((marshalInfo != null) && (marshalInfo.NativeType == (NativeType.CustomMarshaler | NativeType.Boolean)));
            this._windowsRuntimeType = WindowsRuntimeProjections.ProjectToWindowsRuntime(type);
            TypeDefinition definition = this._windowsRuntimeType.Resolve();
            this._isSealed = definition.IsSealed;
            this._isClass = ((marshalType == MarshalType.WindowsRuntime) && !definition.IsInterface()) && !type.IsSystemObject();
            this._defaultInterface = !this._isClass ? this._windowsRuntimeType : definition.ExtractDefaultInterface();
            this._managedTypeName = !this._isClass ? DefaultMarshalInfoWriter.Naming.ForTypeNameOnly(DefaultMarshalInfoWriter.TypeProvider.SystemObject) : DefaultMarshalInfoWriter.Naming.ForTypeNameOnly(this._windowsRuntimeType);
            if (type.IsSystemObject())
            {
                this._interfaceTypeName = !this._marshalAsInspectable ? "Il2CppIUnknown" : "Il2CppIInspectable";
            }
            else
            {
                this._interfaceTypeName = DefaultMarshalInfoWriter.Naming.ForTypeNameOnly(this._defaultInterface);
            }
            this._marshaledTypes = new MarshaledType[] { new MarshaledType(this._interfaceTypeName + '*', this._interfaceTypeName + '*') };
            this._marshalType = marshalType;
        }

        public override void WriteIncludesForFieldDeclaration(CppCodeWriter writer)
        {
            this.WriteMarshaledTypeForwardDeclaration(writer);
        }

        public override void WriteIncludesForMarshaling(CppCodeWriter writer)
        {
            writer.AddIncludeForTypeDefinition(this._windowsRuntimeType);
        }

        public sealed override void WriteMarshalCleanupVariable(CppCodeWriter writer, string variableName, IRuntimeMetadataAccess metadataAccess, string managedVariableName)
        {
            if (!this._isSealed)
            {
                object[] args = new object[] { variableName, DefaultMarshalInfoWriter.Naming.Null };
                writer.WriteLine("if ({0} != {1})", args);
                using (new BlockWriter(writer, false))
                {
                    object[] objArray2 = new object[] { variableName };
                    writer.WriteLine("({0})->Release();", objArray2);
                    object[] objArray3 = new object[] { variableName, DefaultMarshalInfoWriter.Naming.Null };
                    writer.WriteLine("{0} = {1};", objArray3);
                }
            }
        }

        public override void WriteMarshaledTypeForwardDeclaration(CppCodeWriter writer)
        {
            if (!base._typeRef.IsSystemObject())
            {
                writer.AddForwardDeclaration($"struct {this._interfaceTypeName}");
            }
        }

        private void WriteMarshalToNativeForNonSealedType(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string destinationVariable)
        {
            object[] args = new object[] { sourceVariable.Load() };
            writer.WriteLine("if (({0})->klass->is_import_or_windows_runtime)", args);
            using (new BlockWriter(writer, false))
            {
                if (this._isClass)
                {
                    object[] objArray2 = new object[] { destinationVariable, sourceVariable.Load(), DefaultMarshalInfoWriter.Naming.ForComTypeInterfaceFieldGetter(this._defaultInterface) };
                    writer.WriteLine("{0} = ({1})->{2}();", objArray2);
                    object[] objArray3 = new object[] { destinationVariable };
                    writer.WriteLine("{0}->AddRef();", objArray3);
                }
                else
                {
                    object[] objArray4 = new object[] { DefaultMarshalInfoWriter.Naming.ForInteropHResultVariable(), DefaultMarshalInfoWriter.Naming.ForVariable(DefaultMarshalInfoWriter.TypeProvider.Il2CppComObjectTypeReference), sourceVariable.Load(), DefaultMarshalInfoWriter.Naming.ForIl2CppComObjectIdentityField(), this._interfaceTypeName, destinationVariable };
                    writer.WriteLine("il2cpp_hresult_t {0} = (({1}){2})->{3}->QueryInterface({4}::IID, reinterpret_cast<void**>(&{5}));", objArray4);
                    writer.WriteStatement(Emit.Call("il2cpp_codegen_com_raise_exception_if_failed", DefaultMarshalInfoWriter.Naming.ForInteropHResultVariable(), (this._marshalType != MarshalType.COM) ? "false" : "true"));
                }
            }
            writer.WriteLine("else");
            using (new BlockWriter(writer, false))
            {
                object[] objArray5 = new object[] { destinationVariable, this._interfaceTypeName, sourceVariable.Load() };
                writer.WriteLine("{0} = il2cpp_codegen_com_get_or_create_ccw<{1}>({2});", objArray5);
            }
        }

        public sealed override void WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
        {
            object[] args = new object[] { variableName, DefaultMarshalInfoWriter.Naming.Null };
            writer.WriteLine("if ({0} != {1})", args);
            using (new BlockWriter(writer, false))
            {
                TypeReference type = (!base._typeRef.IsInterface() && base._typeRef.Resolve().IsComOrWindowsRuntimeType()) ? base._typeRef : DefaultMarshalInfoWriter.TypeProvider.Il2CppComObjectTypeReference;
                if (this._isSealed)
                {
                    object[] objArray2 = new object[] { this._managedTypeName, variableName, metadataAccess.TypeInfoFor(base._typeRef) };
                    writer.WriteLine(destinationVariable.Store("il2cpp_codegen_com_get_or_create_rcw_for_sealed_class<{0}>({1}, {2})", objArray2));
                }
                else if (this._marshalAsInspectable)
                {
                    object[] objArray3 = new object[] { this._managedTypeName, variableName, metadataAccess.TypeInfoFor(type) };
                    writer.WriteLine(destinationVariable.Store("il2cpp_codegen_com_get_or_create_rcw_from_iinspectable<{0}>({1}, {2})", objArray3));
                }
                else
                {
                    object[] objArray4 = new object[] { this._managedTypeName, variableName, metadataAccess.TypeInfoFor(type) };
                    writer.WriteLine(destinationVariable.Store("il2cpp_codegen_com_get_or_create_rcw_from_iunknown<{0}>({1}, {2})", objArray4));
                }
            }
            writer.WriteLine("else");
            using (new BlockWriter(writer, false))
            {
                writer.WriteLine(destinationVariable.Store(DefaultMarshalInfoWriter.Naming.Null));
            }
        }

        public sealed override void WriteMarshalVariableToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string destinationVariable, string managedVariableName, IRuntimeMetadataAccess metadataAccess)
        {
            object[] args = new object[] { sourceVariable.Load(), DefaultMarshalInfoWriter.Naming.Null };
            writer.WriteLine("if ({0} != {1})", args);
            using (new BlockWriter(writer, false))
            {
                if (this._isSealed)
                {
                    object[] objArray2 = new object[] { destinationVariable, sourceVariable.Load(), DefaultMarshalInfoWriter.Naming.ForComTypeInterfaceFieldGetter(this._defaultInterface) };
                    writer.WriteLine("{0} = ({1})->{2}();", objArray2);
                }
                else
                {
                    this.WriteMarshalToNativeForNonSealedType(writer, sourceVariable, destinationVariable);
                }
            }
            writer.WriteLine("else");
            using (new BlockWriter(writer, false))
            {
                object[] objArray3 = new object[] { destinationVariable, DefaultMarshalInfoWriter.Naming.Null };
                writer.WriteLine("{0} = {1};", objArray3);
            }
        }

        public sealed override MarshaledType[] MarshaledTypes =>
            this._marshaledTypes;
    }
}

