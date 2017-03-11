namespace Unity.IL2CPP.Marshaling.MarshalInfoWriters
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP;
    using Unity.IL2CPP.ILPreProcessor;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;
    using Unity.IL2CPP.Marshaling;

    internal sealed class KeyValuePairMarshalInfoWriter : MarshalableMarshalInfoWriter
    {
        private readonly TypeReference _iKeyValuePair;
        private readonly string _iKeyValuePairTypeName;
        private readonly DefaultMarshalInfoWriter _keyMarshalInfoWriter;
        private readonly MarshaledType[] _marshaledTypes;
        private readonly DefaultMarshalInfoWriter _valueMarshalInfoWriter;
        [CompilerGenerated]
        private static Func<FieldDefinition, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<FieldDefinition, bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache3;
        [CompilerGenerated]
        private static Action<CppCodeWriter> <>f__am$cache4;
        [CompilerGenerated]
        private static Action<CppCodeWriter> <>f__am$cache5;
        [Inject]
        public static IWindowsRuntimeProjections WindowsRuntimeProjections;

        public KeyValuePairMarshalInfoWriter(GenericInstanceType type) : base(type)
        {
            this._keyMarshalInfoWriter = MarshalDataCollector.MarshalInfoWriterFor(type.GenericArguments[0], MarshalType.WindowsRuntime, null, false, false, true, null);
            this._valueMarshalInfoWriter = MarshalDataCollector.MarshalInfoWriterFor(type.GenericArguments[1], MarshalType.WindowsRuntime, null, false, false, true, null);
            this._iKeyValuePair = WindowsRuntimeProjections.ProjectToWindowsRuntime(type);
            this._iKeyValuePairTypeName = DefaultMarshalInfoWriter.Naming.ForTypeNameOnly(this._iKeyValuePair);
            this._marshaledTypes = new MarshaledType[] { new MarshaledType(this._iKeyValuePairTypeName + '*', this._iKeyValuePairTypeName + '*') };
        }

        public override bool CanMarshalTypeFromNative() => 
            (this._keyMarshalInfoWriter.CanMarshalTypeFromNative() && this._valueMarshalInfoWriter.CanMarshalTypeFromNative());

        public override string GetMarshalingException()
        {
            if (!this._keyMarshalInfoWriter.CanMarshalTypeFromNative())
            {
                return this._keyMarshalInfoWriter.GetMarshalingException();
            }
            return this._valueMarshalInfoWriter.GetMarshalingException();
        }

        public override void WriteIncludesForFieldDeclaration(CppCodeWriter writer)
        {
            this.WriteMarshaledTypeForwardDeclaration(writer);
        }

        public override void WriteIncludesForMarshaling(CppCodeWriter writer)
        {
            writer.AddIncludeForTypeDefinition(base._typeRef);
            writer.AddIncludeForTypeDefinition(this._iKeyValuePair);
            this._keyMarshalInfoWriter.WriteIncludesForMarshaling(writer);
            this._valueMarshalInfoWriter.WriteIncludesForMarshaling(writer);
        }

        public sealed override void WriteMarshalCleanupVariable(CppCodeWriter writer, string variableName, IRuntimeMetadataAccess metadataAccess, string managedVariableName)
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

        public override void WriteMarshaledTypeForwardDeclaration(CppCodeWriter writer)
        {
            writer.AddForwardDeclaration($"struct {this._iKeyValuePairTypeName}");
        }

        public sealed override void WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
        {
            <WriteMarshalVariableFromNative>c__AnonStorey0 storey = new <WriteMarshalVariableFromNative>c__AnonStorey0 {
                metadataAccess = metadataAccess,
                $this = this
            };
            string str = DefaultMarshalInfoWriter.Naming.ForInteropHResultVariable();
            TypeDefinition definition = base._typeRef.Resolve();
            Unity.IL2CPP.ILPreProcessor.TypeResolver resolver = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(base._typeRef);
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = f => f.Name == "key";
            }
            FieldReference field = resolver.Resolve(definition.Fields.Single<FieldDefinition>(<>f__am$cache0));
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = f => f.Name == "value";
            }
            FieldReference reference2 = resolver.Resolve(definition.Fields.Single<FieldDefinition>(<>f__am$cache1));
            TypeDefinition definition2 = this._iKeyValuePair.Resolve();
            Unity.IL2CPP.ILPreProcessor.TypeResolver resolver2 = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(this._iKeyValuePair);
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = m => m.Name == "get_Key";
            }
            MethodReference method = resolver2.Resolve(definition2.Methods.Single<MethodDefinition>(<>f__am$cache2));
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = m => m.Name == "get_Value";
            }
            MethodReference reference4 = resolver2.Resolve(definition2.Methods.Single<MethodDefinition>(<>f__am$cache3));
            string str2 = DefaultMarshalInfoWriter.CleanVariableName(variableName);
            storey.keyVariableName = str2 + "KeyNative";
            storey.valueVariableName = str2 + "ValueNative";
            string str3 = str2 + "Staging";
            writer.WriteStatement(Emit.NullCheck(variableName));
            writer.WriteLine();
            using (new BlockWriter(writer, false))
            {
                string str4 = str2 + "_imanagedObject";
                writer.WriteLine($"Il2CppIManagedObjectHolder* {str4} = {DefaultMarshalInfoWriter.Naming.Null};");
                writer.WriteLine($"il2cpp_hresult_t {str} = ({variableName})->QueryInterface(Il2CppIManagedObjectHolder::IID, reinterpret_cast<void**>(&{str4}));");
                writer.WriteLine($"if (IL2CPP_HR_SUCCEEDED({str}))");
                using (new BlockWriter(writer, false))
                {
                    writer.WriteLine(destinationVariable.Store($"*static_cast<{DefaultMarshalInfoWriter.Naming.ForVariable(base._typeRef)}*>(UnBox({str4}->GetManagedObject(), {storey.metadataAccess.TypeInfoFor(base._typeRef)}))"));
                    writer.WriteLine($"{str4}->Release();");
                }
                writer.WriteLine("else");
                using (new BlockWriter(writer, false))
                {
                    writer.WriteLine($"{DefaultMarshalInfoWriter.Naming.ForVariable(base._typeRef)} {str3};");
                    this._keyMarshalInfoWriter.WriteNativeVariableDeclarationOfType(writer, storey.keyVariableName);
                    writer.WriteLine($"{str} = ({variableName})->{DefaultMarshalInfoWriter.Naming.ForMethod(method)}(&{storey.keyVariableName});");
                    writer.WriteLine($"il2cpp_codegen_com_raise_exception_if_failed({str}, false);");
                    writer.WriteLine();
                    this._keyMarshalInfoWriter.WriteMarshalVariableFromNative(writer, storey.keyVariableName, new ManagedMarshalValue(str3, field), methodParameters, true, false, storey.metadataAccess);
                    if (<>f__am$cache4 == null)
                    {
                        <>f__am$cache4 = bodyWriter => bodyWriter.WriteLine();
                    }
                    writer.WriteIfNotEmpty(<>f__am$cache4, new Action<CppCodeWriter>(storey.<>m__0), null);
                    writer.WriteLine();
                    this._valueMarshalInfoWriter.WriteNativeVariableDeclarationOfType(writer, storey.valueVariableName);
                    writer.WriteLine($"{str} = ({variableName})->{DefaultMarshalInfoWriter.Naming.ForMethod(reference4)}(&{storey.valueVariableName});");
                    writer.WriteLine($"il2cpp_codegen_com_raise_exception_if_failed({str}, false);");
                    writer.WriteLine();
                    this._valueMarshalInfoWriter.WriteMarshalVariableFromNative(writer, storey.valueVariableName, new ManagedMarshalValue(str3, reference2), methodParameters, true, false, storey.metadataAccess);
                    if (<>f__am$cache5 == null)
                    {
                        <>f__am$cache5 = bodyWriter => bodyWriter.WriteLine();
                    }
                    writer.WriteIfNotEmpty(<>f__am$cache5, new Action<CppCodeWriter>(storey.<>m__1), null);
                    writer.WriteLine();
                    writer.WriteLine(destinationVariable.Store(str3));
                }
            }
        }

        public override void WriteMarshalVariableToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string destinationVariable, string managedVariableName, IRuntimeMetadataAccess metadataAccess)
        {
            writer.WriteStatement($"{destinationVariable} = il2cpp_codegen_com_get_or_create_ccw<{this._iKeyValuePairTypeName}>({Emit.Box(base._typeRef, sourceVariable.Load(), metadataAccess)})");
        }

        public sealed override MarshaledType[] MarshaledTypes =>
            this._marshaledTypes;

        [CompilerGenerated]
        private sealed class <WriteMarshalVariableFromNative>c__AnonStorey0
        {
            internal KeyValuePairMarshalInfoWriter $this;
            internal string keyVariableName;
            internal IRuntimeMetadataAccess metadataAccess;
            internal string valueVariableName;

            internal void <>m__0(CppCodeWriter bodyWriter)
            {
                this.$this._keyMarshalInfoWriter.WriteMarshalCleanupVariable(bodyWriter, this.keyVariableName, this.metadataAccess, null);
            }

            internal void <>m__1(CppCodeWriter bodyWriter)
            {
                this.$this._valueMarshalInfoWriter.WriteMarshalCleanupVariable(bodyWriter, this.valueVariableName, this.metadataAccess, null);
            }
        }
    }
}

