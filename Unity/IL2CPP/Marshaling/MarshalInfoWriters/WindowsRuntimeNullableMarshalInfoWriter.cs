namespace Unity.IL2CPP.Marshaling.MarshalInfoWriters
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using Unity.IL2CPP;
    using Unity.IL2CPP.ILPreProcessor;
    using Unity.IL2CPP.Marshaling;

    internal class WindowsRuntimeNullableMarshalInfoWriter : MarshalableMarshalInfoWriter
    {
        private readonly TypeReference _boxedType;
        private readonly DefaultMarshalInfoWriter _boxedTypeMarshalInfoWriter;
        private readonly string _interfaceTypeName;
        private readonly GenericInstanceType _ireferenceInstance;
        private readonly MarshaledType[] _marshaledTypes;
        [CompilerGenerated]
        private static Func<FieldDefinition, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<FieldDefinition, bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache2;

        public WindowsRuntimeNullableMarshalInfoWriter(TypeReference type) : base(type)
        {
            this._boxedType = ((GenericInstanceType) type).GenericArguments[0];
            this._ireferenceInstance = new GenericInstanceType(DefaultMarshalInfoWriter.TypeProvider.IReferenceType);
            this._ireferenceInstance.GenericArguments.Add(this._boxedType);
            this._interfaceTypeName = DefaultMarshalInfoWriter.Naming.ForTypeNameOnly(this._ireferenceInstance);
            string name = this._interfaceTypeName + "*";
            this._marshaledTypes = new MarshaledType[] { new MarshaledType(name, name) };
            this._boxedTypeMarshalInfoWriter = MarshalDataCollector.MarshalInfoWriterFor(this._boxedType, MarshalType.WindowsRuntime, null, false, false, false, null);
        }

        public override bool CanMarshalTypeFromNative() => 
            this._boxedTypeMarshalInfoWriter.CanMarshalTypeFromNative();

        public override bool CanMarshalTypeToNative() => 
            this._boxedTypeMarshalInfoWriter.CanMarshalTypeToNative();

        private static FieldReference GetHasValueField(TypeDefinition nullableTypeDef, Unity.IL2CPP.ILPreProcessor.TypeResolver typeResolver)
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = f => !f.IsStatic && (f.FieldType.MetadataType == MetadataType.Boolean);
            }
            return typeResolver.Resolve(nullableTypeDef.Fields.Single<FieldDefinition>(<>f__am$cache0));
        }

        public override string GetMarshalingException() => 
            this._boxedTypeMarshalInfoWriter.GetMarshalingException();

        private static FieldReference GetValueField(TypeDefinition nullableTypeDef, Unity.IL2CPP.ILPreProcessor.TypeResolver typeResolver)
        {
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = f => !f.IsStatic && (f.FieldType.MetadataType == MetadataType.Var);
            }
            return typeResolver.Resolve(nullableTypeDef.Fields.Single<FieldDefinition>(<>f__am$cache1));
        }

        public override void WriteIncludesForFieldDeclaration(CppCodeWriter writer)
        {
            this.WriteMarshaledTypeForwardDeclaration(writer);
        }

        public override void WriteIncludesForMarshaling(CppCodeWriter writer)
        {
            writer.AddIncludeForTypeDefinition(base._typeRef);
            writer.AddIncludeForTypeDefinition(this._ireferenceInstance);
            this._boxedTypeMarshalInfoWriter.WriteIncludesForMarshaling(writer);
        }

        public override void WriteMarshalCleanupEmptyVariable(CppCodeWriter writer, string variableName, IRuntimeMetadataAccess metadataAccess, string managedVariableName)
        {
        }

        public override void WriteMarshalCleanupVariable(CppCodeWriter writer, string variableName, IRuntimeMetadataAccess metadataAccess, string managedVariableName = null)
        {
            writer.WriteLine($"if ({variableName} != {DefaultMarshalInfoWriter.Naming.Null})");
            using (new BlockWriter(writer, false))
            {
                writer.WriteLine($"{variableName}->Release();");
            }
        }

        public override void WriteMarshaledTypeForwardDeclaration(CppCodeWriter writer)
        {
            writer.AddForwardDeclaration($"struct {DefaultMarshalInfoWriter.Naming.ForTypeNameOnly(this._ireferenceInstance)}");
        }

        public override string WriteMarshalEmptyVariableFromNative(CppCodeWriter writer, string variableName, IList<MarshaledParameter> methodParameters, IRuntimeMetadataAccess metadataAccess)
        {
            string name = $"_{DefaultMarshalInfoWriter.CleanVariableName(variableName)}_empty";
            writer.WriteVariable(base._typeRef, name);
            return name;
        }

        public override void WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
        {
            Unity.IL2CPP.ILPreProcessor.TypeResolver typeResolver = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(base._typeRef);
            TypeDefinition nullableTypeDef = base._typeRef.Resolve();
            string str = DefaultMarshalInfoWriter.Naming.ForFieldSetter(GetHasValueField(nullableTypeDef, typeResolver));
            string str2 = DefaultMarshalInfoWriter.Naming.ForFieldSetter(GetValueField(nullableTypeDef, typeResolver));
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = m => m.Name == "get_Value";
            }
            MethodReference method = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(this._ireferenceInstance).Resolve(DefaultMarshalInfoWriter.TypeProvider.IReferenceType.Resolve().Methods.Single<MethodDefinition>(<>f__am$cache2));
            string str3 = DefaultMarshalInfoWriter.Naming.ForMethod(method);
            string str4 = $"{destinationVariable.GetNiceName()}_value_marshaled";
            string str5 = DefaultMarshalInfoWriter.Naming.ForInteropHResultVariable();
            writer.WriteLine($"if ({variableName} != {DefaultMarshalInfoWriter.Naming.Null})");
            using (new BlockWriter(writer, false))
            {
                writer.WriteLine($"Il2CppIManagedObjectHolder* imanagedObject = {DefaultMarshalInfoWriter.Naming.Null};");
                writer.WriteLine($"il2cpp_hresult_t {str5} = {variableName}->QueryInterface(Il2CppIManagedObjectHolder::IID, reinterpret_cast<void**>(&imanagedObject));");
                writer.WriteLine($"if (IL2CPP_HR_SUCCEEDED({str5}))");
                using (new BlockWriter(writer, false))
                {
                    writer.WriteLine($"{destinationVariable.Load()}.{str2}(*static_cast<{DefaultMarshalInfoWriter.Naming.ForVariable(this._boxedType)}*>(UnBox(imanagedObject->GetManagedObject())));");
                    writer.WriteLine("imanagedObject->Release();");
                }
                writer.WriteLine("else");
                using (new BlockWriter(writer, false))
                {
                    this._boxedTypeMarshalInfoWriter.WriteNativeVariableDeclarationOfType(writer, str4);
                    writer.WriteLine($"hr = {variableName}->{str3}(&{str4});");
                    writer.WriteLine($"il2cpp_codegen_com_raise_exception_if_failed({str5}, false);");
                    writer.WriteLine();
                    string str6 = this._boxedTypeMarshalInfoWriter.WriteMarshalVariableFromNative(writer, str4, methodParameters, false, forNativeWrapperOfManagedMethod, metadataAccess);
                    writer.WriteLine($"{destinationVariable.Load()}.{str2}({str6});");
                }
                writer.WriteLine();
                writer.WriteLine($"{destinationVariable.Load()}.{str}(true);");
            }
            writer.WriteLine("else");
            using (new BlockWriter(writer, false))
            {
                writer.WriteLine($"{destinationVariable.Load()}.{str}(false);");
            }
        }

        public override void WriteMarshalVariableToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string destinationVariable, string managedVariableName, IRuntimeMetadataAccess metadataAccess)
        {
            Unity.IL2CPP.ILPreProcessor.TypeResolver typeResolver = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(base._typeRef);
            TypeDefinition nullableTypeDef = base._typeRef.Resolve();
            string str = DefaultMarshalInfoWriter.Naming.ForFieldGetter(GetHasValueField(nullableTypeDef, typeResolver));
            string str2 = DefaultMarshalInfoWriter.Naming.ForFieldGetter(GetValueField(nullableTypeDef, typeResolver));
            string str3 = $"{sourceVariable.GetNiceName()}_value";
            string str4 = $"{sourceVariable.GetNiceName()}_boxed";
            writer.WriteLine($"if ({sourceVariable.Load()}.{str}())");
            using (new BlockWriter(writer, false))
            {
                writer.WriteLine($"{DefaultMarshalInfoWriter.Naming.ForVariable(this._boxedType)} {str3} = {sourceVariable.Load()}.{str2}();");
                writer.WriteLine($"{DefaultMarshalInfoWriter.Naming.ForVariable(DefaultMarshalInfoWriter.TypeProvider.SystemObject)} {str4} = Box({metadataAccess.TypeInfoFor(this._boxedType)}, &{str3});");
                writer.WriteLine($"{destinationVariable} = il2cpp_codegen_com_get_or_create_ccw<{this._interfaceTypeName}>({str4});");
            }
            writer.WriteLine("else");
            using (new BlockWriter(writer, false))
            {
                writer.WriteLine($"{destinationVariable} = {DefaultMarshalInfoWriter.Naming.Null};");
            }
        }

        public override MarshaledType[] MarshaledTypes =>
            this._marshaledTypes;
    }
}

