namespace Unity.IL2CPP.Marshaling.MarshalInfoWriters
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Common;
    using Unity.IL2CPP.ILPreProcessor;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;
    using Unity.IL2CPP.Marshaling;
    using Unity.IL2CPP.Marshaling.BodyWriters.ManagedToNative;

    internal sealed class WindowsRuntimeDelegateMarshalInfoWriter : MarshalableMarshalInfoWriter
    {
        private readonly string _comCallableWrapperInterfaceName;
        private readonly MethodReference _invokeMethod;
        private readonly MarshaledType[] _marshaledTypes;
        private readonly string _nativeInvokerName;
        private readonly string _nativeInvokerSignature;
        private readonly Unity.IL2CPP.ILPreProcessor.TypeResolver _typeResolver;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<FieldDefinition, bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<FieldDefinition, bool> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<FieldDefinition, bool> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<FieldDefinition, bool> <>f__am$cache4;
        [CompilerGenerated]
        private static Func<FieldDefinition, bool> <>f__am$cache5;
        [CompilerGenerated]
        private static Func<FieldDefinition, bool> <>f__am$cache6;
        [Inject]
        public static IWindowsRuntimeProjections WindowsRuntimeProjections;

        public WindowsRuntimeDelegateMarshalInfoWriter(TypeReference type) : base(type)
        {
            TypeDefinition definition = type.Resolve();
            if (!definition.IsDelegate())
            {
                throw new ArgumentException($"WindowsRuntimeDelegateMarshalInfoWriter cannot marshal non-delegate type {type.FullName}.");
            }
            this._typeResolver = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(type);
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<MethodDefinition, bool>(WindowsRuntimeDelegateMarshalInfoWriter.<WindowsRuntimeDelegateMarshalInfoWriter>m__0);
            }
            this._invokeMethod = this._typeResolver.Resolve(definition.Methods.Single<MethodDefinition>(<>f__am$cache0));
            this._comCallableWrapperInterfaceName = DefaultMarshalInfoWriter.Naming.ForWindowsRuntimeDelegateComCallableWrapperInterface(type);
            this._nativeInvokerName = DefaultMarshalInfoWriter.Naming.ForWindowsRuntimeDelegateNativeInvokerMethod(this._invokeMethod);
            this._marshaledTypes = new MarshaledType[] { new MarshaledType(this._comCallableWrapperInterfaceName + '*', this._comCallableWrapperInterfaceName + '*') };
            string returnType = DefaultMarshalInfoWriter.Naming.ForVariable(this._typeResolver.Resolve(Unity.IL2CPP.GenericParameterResolver.ResolveReturnTypeIfNeeded(this._invokeMethod)));
            string parameters = $"{DefaultMarshalInfoWriter.Naming.ForVariable(DefaultMarshalInfoWriter.TypeProvider.Il2CppComObjectTypeReference)} {DefaultMarshalInfoWriter.Naming.ThisParameterName}, {MethodSignatureWriter.FormatParameters(this._invokeMethod, ParameterFormat.WithTypeAndNameNoThis, false, true, false)}";
            this._nativeInvokerSignature = MethodSignatureWriter.GetMethodSignature(this._nativeInvokerName, returnType, parameters, "extern \"C\"", string.Empty);
        }

        [CompilerGenerated]
        private static bool <WindowsRuntimeDelegateMarshalInfoWriter>m__0(MethodDefinition m) => 
            (m.Name == "Invoke");

        public override string GetDefaultCppValue(MarshaledType type) => 
            DefaultMarshalInfoWriter.Naming.Null;

        public override void WriteMarshalCleanupVariable(CppCodeWriter writer, string variableName, IRuntimeMetadataAccess metadataAccess, string managedVariableName = null)
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
            writer.AddForwardDeclaration($"struct {this._comCallableWrapperInterfaceName}");
        }

        public override string WriteMarshalEmptyVariableToNative(CppCodeWriter writer, ManagedMarshalValue variableName, IList<MarshaledParameter> methodParameters) => 
            DefaultMarshalInfoWriter.Naming.Null;

        public override void WriteMarshalFunctionDeclarations(CppCodeWriter writer)
        {
            writer.WriteStatement(this._nativeInvokerSignature);
        }

        public override void WriteMarshalFunctionDefinitions(CppCodeWriter writer, IInteropDataCollector interopDataCollector)
        {
            writer.AddIncludeForTypeDefinition(base._typeRef);
            object[] args = new object[] { this._comCallableWrapperInterfaceName, WindowsRuntimeProjections.ProjectToWindowsRuntime(base._typeRef).GetGuid().ToInitializer() };
            writer.WriteLine("const Il2CppGuid {0}::IID = {1};", args);
            interopDataCollector.AddGuid(base._typeRef);
            this.WriteNativeInvoker(writer);
            interopDataCollector.AddCCWMarshallingFunction(base._typeRef);
        }

        public override void WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess)
        {
            object[] args = new object[] { variableName, DefaultMarshalInfoWriter.Naming.Null };
            writer.WriteLine("if ({0} != {1})", args);
            using (new BlockWriter(writer, false))
            {
                object[] objArray2 = new object[] { DefaultMarshalInfoWriter.Naming.Null };
                writer.WriteLine("Il2CppIManagedObjectHolder* imanagedObject = {0};", objArray2);
                object[] objArray3 = new object[] { DefaultMarshalInfoWriter.Naming.ForInteropHResultVariable(), variableName };
                writer.WriteLine("il2cpp_hresult_t {0} = ({1})->QueryInterface(Il2CppIManagedObjectHolder::IID, reinterpret_cast<void**>(&imanagedObject));", objArray3);
                object[] objArray4 = new object[] { DefaultMarshalInfoWriter.Naming.ForInteropHResultVariable() };
                writer.WriteLine("if (IL2CPP_HR_SUCCEEDED({0}))", objArray4);
                using (new BlockWriter(writer, false))
                {
                    object[] objArray5 = new object[] { DefaultMarshalInfoWriter.Naming.ForVariable(base._typeRef) };
                    writer.WriteLine(destinationVariable.Store("static_cast<{0}>(imanagedObject->GetManagedObject())", objArray5));
                    writer.WriteLine("imanagedObject->Release();");
                }
                writer.WriteLine("else");
                using (new BlockWriter(writer, false))
                {
                    if (<>f__am$cache3 == null)
                    {
                        <>f__am$cache3 = f => f.Name == "method_ptr";
                    }
                    FieldDefinition field = DefaultMarshalInfoWriter.TypeProvider.SystemDelegate.Fields.Single<FieldDefinition>(<>f__am$cache3);
                    string str = DefaultMarshalInfoWriter.Naming.ForFieldSetter(field);
                    if (<>f__am$cache4 == null)
                    {
                        <>f__am$cache4 = f => f.Name == "method";
                    }
                    FieldDefinition definition2 = DefaultMarshalInfoWriter.TypeProvider.SystemDelegate.Fields.Single<FieldDefinition>(<>f__am$cache4);
                    string str2 = DefaultMarshalInfoWriter.Naming.ForFieldSetter(definition2);
                    if (<>f__am$cache5 == null)
                    {
                        <>f__am$cache5 = f => f.Name == "m_target";
                    }
                    FieldDefinition definition3 = DefaultMarshalInfoWriter.TypeProvider.SystemDelegate.Fields.Single<FieldDefinition>(<>f__am$cache5);
                    string str3 = DefaultMarshalInfoWriter.Naming.ForFieldSetter(definition3);
                    writer.WriteLine(destinationVariable.Store(Emit.NewObj(base._typeRef, metadataAccess)));
                    writer.AddMethodForwardDeclaration($"{this._nativeInvokerSignature};");
                    object[] objArray6 = new object[] { destinationVariable.Load(), str, this._nativeInvokerName };
                    writer.WriteLine("{0}->{1}((Il2CppMethodPointer){2});", objArray6);
                    object[] objArray7 = new object[] { DefaultMarshalInfoWriter.Naming.ForVariable(DefaultMarshalInfoWriter.TypeProvider.SystemIntPtr) };
                    writer.WriteLine("{0} methodInfo;", objArray7);
                    object[] objArray8 = new object[2];
                    if (<>f__am$cache6 == null)
                    {
                        <>f__am$cache6 = f => f.Name == DefaultMarshalInfoWriter.Naming.IntPtrValueField;
                    }
                    objArray8[0] = DefaultMarshalInfoWriter.Naming.ForFieldSetter(DefaultMarshalInfoWriter.TypeProvider.SystemIntPtr.Fields.Single<FieldDefinition>(<>f__am$cache6));
                    objArray8[1] = metadataAccess.MethodInfo(this._invokeMethod);
                    writer.WriteLine("methodInfo.{0}((void*){1});", objArray8);
                    object[] objArray9 = new object[] { destinationVariable.Load(), str2 };
                    writer.WriteLine("{0}->{1}(methodInfo);", objArray9);
                    object[] objArray10 = new object[] { destinationVariable.Load(), str3, DefaultMarshalInfoWriter.Naming.ForTypeNameOnly(DefaultMarshalInfoWriter.TypeProvider.Il2CppComDelegateTypeReference), variableName, metadataAccess.TypeInfoFor(DefaultMarshalInfoWriter.TypeProvider.Il2CppComDelegateTypeReference) };
                    writer.WriteLine("{0}->{1}(il2cpp_codegen_com_get_or_create_rcw_for_sealed_class<{2}>({3}, {4}));", objArray10);
                    writer.AddIncludeForTypeDefinition(DefaultMarshalInfoWriter.TypeProvider.Il2CppComDelegateTypeReference);
                }
            }
            writer.WriteLine("else");
            using (new BlockWriter(writer, false))
            {
                writer.WriteLine(destinationVariable.Store(DefaultMarshalInfoWriter.Naming.Null));
            }
        }

        public override void WriteMarshalVariableToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string destinationVariable, string managedVariableName, IRuntimeMetadataAccess metadataAccess)
        {
            object[] args = new object[] { sourceVariable.Load(), DefaultMarshalInfoWriter.Naming.Null };
            writer.WriteLine("if ({0} != {1})", args);
            using (new BlockWriter(writer, false))
            {
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = f => f.Name == "m_target";
                }
                FieldDefinition field = DefaultMarshalInfoWriter.TypeProvider.SystemDelegate.Fields.Single<FieldDefinition>(<>f__am$cache1);
                string str = DefaultMarshalInfoWriter.Naming.ForFieldGetter(field);
                if (<>f__am$cache2 == null)
                {
                    <>f__am$cache2 = f => f.Name == ((CodeGenOptions.Dotnetprofile != DotNetProfile.Net45) ? "prev" : "delegates");
                }
                FieldDefinition definition2 = DefaultMarshalInfoWriter.TypeProvider.SystemMulticastDelegate.Fields.Single<FieldDefinition>(<>f__am$cache2);
                string str2 = DefaultMarshalInfoWriter.Naming.ForFieldGetter(definition2);
                object[] objArray2 = new object[] { sourceVariable.Load(), str };
                writer.WriteLine("Il2CppObject* target = {0}->{1}();", objArray2);
                writer.WriteLine();
                object[] objArray3 = new object[] { DefaultMarshalInfoWriter.Naming.Null, sourceVariable.Load(), str2, metadataAccess.TypeInfoFor(DefaultMarshalInfoWriter.TypeProvider.Il2CppComDelegateTypeReference) };
                writer.WriteLine("if (target != {0} && {1}->{2}() == {0} && target->klass == {3})", objArray3);
                using (new BlockWriter(writer, false))
                {
                    object[] objArray4 = new object[] { DefaultMarshalInfoWriter.Naming.ForInteropHResultVariable(), DefaultMarshalInfoWriter.Naming.ForVariable(DefaultMarshalInfoWriter.TypeProvider.Il2CppComObjectTypeReference), DefaultMarshalInfoWriter.Naming.ForIl2CppComObjectIdentityField(), this._comCallableWrapperInterfaceName, destinationVariable };
                    writer.WriteLine("il2cpp_hresult_t {0} = static_cast<{1}>(target)->{2}->QueryInterface({3}::IID, reinterpret_cast<void**>(&{4}));", objArray4);
                    writer.WriteStatement(Emit.Call("il2cpp_codegen_com_raise_exception_if_failed", DefaultMarshalInfoWriter.Naming.ForInteropHResultVariable(), "false"));
                }
                writer.WriteLine("else");
                using (new BlockWriter(writer, false))
                {
                    object[] objArray5 = new object[] { destinationVariable, this._comCallableWrapperInterfaceName, sourceVariable.Load() };
                    writer.WriteLine("{0} = il2cpp_codegen_com_get_or_create_ccw<{1}>({2});", objArray5);
                }
            }
            writer.WriteLine("else");
            using (new BlockWriter(writer, false))
            {
                object[] objArray6 = new object[] { destinationVariable, DefaultMarshalInfoWriter.Naming.Null };
                writer.WriteLine("{0} = {1};", objArray6);
            }
        }

        private void WriteNativeInvoker(CppCodeWriter writer)
        {
            writer.WriteCommentedLine($"Native invoker for {base._typeRef.FullName}");
            MethodWriter.WriteMethodWithMetadataInitialization(writer, this._nativeInvokerSignature, this._invokeMethod.FullName, delegate (CppCodeWriter bodyWriter, MetadataUsage metadataUsage, MethodUsage methodUsage) {
                IRuntimeMetadataAccess metadataAccess = MethodWriter.GetDefaultRuntimeMetadataAccess(this._invokeMethod, metadataUsage, methodUsage, null);
                new WindowsRuntimeDelegateMethodBodyWriter(this._invokeMethod).WriteMethodBody(bodyWriter, metadataAccess);
            }, this._nativeInvokerName);
        }

        public override void WriteNativeStructDefinition(CppCodeWriter writer)
        {
            foreach (ParameterDefinition definition in this._invokeMethod.Parameters)
            {
                MarshalDataCollector.MarshalInfoWriterFor(this._typeResolver.Resolve(definition.ParameterType), MarshalType.WindowsRuntime, null, true, false, false, null).WriteMarshaledTypeForwardDeclaration(writer);
            }
            MarshalDataCollector.MarshalInfoWriterFor(this._typeResolver.Resolve(this._invokeMethod.ReturnType), MarshalType.WindowsRuntime, null, true, false, false, null).WriteMarshaledTypeForwardDeclaration(writer);
            writer.WriteCommentedLine($"COM Callable Wrapper interface definition for {base._typeRef.FullName}");
            object[] args = new object[] { this._comCallableWrapperInterfaceName };
            writer.WriteLine("struct {0} : Il2CppIUnknown", args);
            using (new BlockWriter(writer, true))
            {
                writer.WriteLine("static const Il2CppGuid IID;");
                string str = MethodSignatureWriter.FormatComMethodParameterList(this._invokeMethod, this._invokeMethod, this._typeResolver, MarshalType.WindowsRuntime, true);
                object[] objArray2 = new object[] { str };
                writer.WriteLine("virtual il2cpp_hresult_t STDCALL Invoke({0}) = 0;", objArray2);
            }
            writer.WriteLine();
        }

        public override void WriteNativeVariableDeclarationOfType(CppCodeWriter writer, string variableName)
        {
            object[] args = new object[] { this._comCallableWrapperInterfaceName, variableName, DefaultMarshalInfoWriter.Naming.Null };
            writer.WriteLine("{0}* {1} = {2};", args);
        }

        public override MarshaledType[] MarshaledTypes =>
            this._marshaledTypes;
    }
}

