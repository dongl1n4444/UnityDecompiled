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
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;
    using Unity.IL2CPP.Marshaling;

    internal sealed class WindowsRuntimeDelegateMarshalInfoWriter : MarshalableMarshalInfoWriter
    {
        private readonly string _comCallableWrapperClassName;
        private readonly string _comCallableWrapperInterfaceName;
        private readonly MethodReference _invokeMethod;
        private readonly MarshaledType[] _marshaledTypes;
        private readonly string _nativeInvokerName;
        private readonly string _nativeInvokerSignature;
        private readonly string _parameterList;
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
            if (!Extensions.IsDelegate(definition))
            {
                throw new ArgumentException(string.Format("WindowsRuntimeDelegateMarshalInfoWriter cannot marshal non-delegate type {0}.", type.FullName));
            }
            this._typeResolver = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(type);
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<MethodDefinition, bool>(null, (IntPtr) <WindowsRuntimeDelegateMarshalInfoWriter>m__0);
            }
            this._invokeMethod = this._typeResolver.Resolve(Enumerable.Single<MethodDefinition>(definition.Methods, <>f__am$cache0));
            this._comCallableWrapperClassName = DefaultMarshalInfoWriter.Naming.ForWindowsRuntimeDelegateComCallableWrapperClass(type);
            this._comCallableWrapperInterfaceName = DefaultMarshalInfoWriter.Naming.ForWindowsRuntimeDelegateComCallableWrapperInterface(type);
            this._nativeInvokerName = DefaultMarshalInfoWriter.Naming.ForWindowsRuntimeDelegateNativeInvokerMethod(this._invokeMethod);
            this._parameterList = ComInterfaceWriter.BuildMethodParameterList(this._invokeMethod, this._invokeMethod, this._typeResolver, MarshalType.WindowsRuntime, true);
            this._marshaledTypes = new MarshaledType[] { new MarshaledType(this._comCallableWrapperInterfaceName, this._comCallableWrapperInterfaceName + '*') };
            string returnType = DefaultMarshalInfoWriter.Naming.ForVariable(this._typeResolver.Resolve(Unity.IL2CPP.GenericParameterResolver.ResolveReturnTypeIfNeeded(this._invokeMethod)));
            string parameters = string.Format("{0} {1}, {2}", DefaultMarshalInfoWriter.Naming.ForVariable(DefaultMarshalInfoWriter.TypeProvider.Il2CppComObjectTypeReference), DefaultMarshalInfoWriter.Naming.ThisParameterName, MethodSignatureWriter.FormatParameters(this._invokeMethod, ParameterFormat.WithTypeAndNameNoThis, false, true));
            this._nativeInvokerSignature = MethodSignatureWriter.GetMethodSignature(this._nativeInvokerName, returnType, parameters, "extern \"C\"", string.Empty);
        }

        [CompilerGenerated]
        private static bool <WindowsRuntimeDelegateMarshalInfoWriter>m__0(MethodDefinition m)
        {
            return (m.Name == "Invoke");
        }

        private void WriteCreateComCallableWrapperFunction(CppCodeWriter writer)
        {
            writer.WriteCommentedLine(string.Format("Create COM Callable Wrapper function for {0}", base._typeRef.FullName));
            object[] args = new object[] { DefaultMarshalInfoWriter.Naming.ForCreateComCallableWrapperFunction(base._typeRef) };
            writer.WriteLine("extern \"C\" Il2CppIManagedObjectHolder* {0}(Il2CppObject* obj)", args);
            using (new BlockWriter(writer, false))
            {
                object[] objArray2 = new object[] { this._comCallableWrapperClassName };
                writer.WriteLine("return {0}::__CreateInstance(obj);", objArray2);
            }
        }

        private void WriteManagedInvoker(CppCodeWriter writer)
        {
            writer.WriteCommentedLine(string.Format("COM Callable invoker for {0}", base._typeRef.FullName));
            string methodSignature = string.Format("il2cpp_hresult_t STDCALL {0}::Invoke({1})", this._comCallableWrapperClassName, this._parameterList);
            MethodWriter.WriteMethodWithMetadataInitialization(writer, methodSignature, this._invokeMethod.FullName, new Action<CppCodeWriter, MetadataUsage, MethodUsage>(this, (IntPtr) this.<WriteManagedInvoker>m__2), DefaultMarshalInfoWriter.Naming.ForMethod(this._invokeMethod) + "_WindowsRuntimeManagedInvoker");
        }

        public override void WriteMarshalCleanupVariable(CppCodeWriter writer, string variableName, IRuntimeMetadataAccess metadataAccess, [Optional, DefaultParameterValue(null)] string managedVariableName)
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

        public override string WriteMarshalEmptyVariableToNative(CppCodeWriter writer, ManagedMarshalValue variableName, IList<MarshaledParameter> methodParameters)
        {
            return DefaultMarshalInfoWriter.Naming.Null;
        }

        public override void WriteMarshalFunctionDeclarations(CppCodeWriter writer)
        {
            writer.WriteStatement(this._nativeInvokerSignature);
        }

        public override void WriteMarshalFunctionDefinitions(CppCodeWriter writer, IMethodCollector methodCollector)
        {
            object[] args = new object[] { this._comCallableWrapperInterfaceName, Extensions.ToInitializer(Extensions.GetGuid(WindowsRuntimeProjections.ProjectToWindowsRuntime(base._typeRef))) };
            writer.WriteLine("const Il2CppGuid {0}::IID = {1};", args);
            this.WriteCreateComCallableWrapperFunction(writer);
            this.WriteNativeInvoker(writer);
            this.WriteManagedInvoker(writer);
            methodCollector.AddCCWMarshallingFunction(base._typeRef.Resolve());
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
                writer.WriteLine("il2cpp_hresult_t {0} = {1}->QueryInterface(Il2CppIManagedObjectHolder::IID, reinterpret_cast<void**>(&imanagedObject));", objArray3);
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
                        <>f__am$cache3 = new Func<FieldDefinition, bool>(null, (IntPtr) <WriteMarshalVariableFromNative>m__5);
                    }
                    FieldDefinition field = Enumerable.Single<FieldDefinition>(DefaultMarshalInfoWriter.TypeProvider.SystemDelegate.Fields, <>f__am$cache3);
                    string str = DefaultMarshalInfoWriter.Naming.ForFieldSetter(field);
                    if (<>f__am$cache4 == null)
                    {
                        <>f__am$cache4 = new Func<FieldDefinition, bool>(null, (IntPtr) <WriteMarshalVariableFromNative>m__6);
                    }
                    FieldDefinition definition2 = Enumerable.Single<FieldDefinition>(DefaultMarshalInfoWriter.TypeProvider.SystemDelegate.Fields, <>f__am$cache4);
                    string str2 = DefaultMarshalInfoWriter.Naming.ForFieldSetter(definition2);
                    if (<>f__am$cache5 == null)
                    {
                        <>f__am$cache5 = new Func<FieldDefinition, bool>(null, (IntPtr) <WriteMarshalVariableFromNative>m__7);
                    }
                    FieldDefinition definition3 = Enumerable.Single<FieldDefinition>(DefaultMarshalInfoWriter.TypeProvider.SystemDelegate.Fields, <>f__am$cache5);
                    string str3 = DefaultMarshalInfoWriter.Naming.ForFieldSetter(definition3);
                    writer.WriteLine(destinationVariable.Store(Emit.NewObj(base._typeRef, metadataAccess)));
                    object[] objArray6 = new object[] { destinationVariable.Load(), str, this._nativeInvokerName };
                    writer.WriteLine("{0}->{1}((Il2CppMethodPointer){2});", objArray6);
                    object[] objArray7 = new object[] { DefaultMarshalInfoWriter.Naming.ForVariable(DefaultMarshalInfoWriter.TypeProvider.SystemIntPtr) };
                    writer.WriteLine("{0} methodInfo;", objArray7);
                    object[] objArray8 = new object[2];
                    if (<>f__am$cache6 == null)
                    {
                        <>f__am$cache6 = new Func<FieldDefinition, bool>(null, (IntPtr) <WriteMarshalVariableFromNative>m__8);
                    }
                    objArray8[0] = DefaultMarshalInfoWriter.Naming.ForFieldSetter(Enumerable.Single<FieldDefinition>(DefaultMarshalInfoWriter.TypeProvider.SystemIntPtr.Fields, <>f__am$cache6));
                    objArray8[1] = metadataAccess.MethodInfo(this._invokeMethod);
                    writer.WriteLine("methodInfo.{0}((void*){1});", objArray8);
                    object[] objArray9 = new object[] { destinationVariable.Load(), str2 };
                    writer.WriteLine("{0}->{1}(methodInfo);", objArray9);
                    object[] objArray10 = new object[] { destinationVariable.Load(), str3, DefaultMarshalInfoWriter.Naming.ForTypeNameOnly(DefaultMarshalInfoWriter.TypeProvider.Il2CppComDelegateTypeReference), variableName, metadataAccess.TypeInfoFor(DefaultMarshalInfoWriter.TypeProvider.Il2CppComDelegateTypeReference) };
                    writer.WriteLine("{0}->{1}(il2cpp_codegen_com_get_or_create_rcw_for_sealed_class<{2}>({3}, {4}));", objArray10);
                    writer.AddIncludeForTypeDefinition(DefaultMarshalInfoWriter.TypeProvider.Il2CppComDelegateTypeReference);
                    writer.AddIncludeForMethodDeclarations(base._typeRef);
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
                    <>f__am$cache1 = new Func<FieldDefinition, bool>(null, (IntPtr) <WriteMarshalVariableToNative>m__3);
                }
                FieldDefinition field = Enumerable.Single<FieldDefinition>(DefaultMarshalInfoWriter.TypeProvider.SystemDelegate.Fields, <>f__am$cache1);
                string str = DefaultMarshalInfoWriter.Naming.ForFieldGetter(field);
                if (<>f__am$cache2 == null)
                {
                    <>f__am$cache2 = new Func<FieldDefinition, bool>(null, (IntPtr) <WriteMarshalVariableToNative>m__4);
                }
                FieldDefinition definition2 = Enumerable.Single<FieldDefinition>(DefaultMarshalInfoWriter.TypeProvider.SystemMulticastDelegate.Fields, <>f__am$cache2);
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
                    writer.WriteStatement(Emit.Call("il2cpp_codegen_com_raise_exception_if_failed", DefaultMarshalInfoWriter.Naming.ForInteropHResultVariable()));
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
            writer.WriteCommentedLine(string.Format("Native invoker for {0}", base._typeRef.FullName));
            MethodWriter.WriteMethodWithMetadataInitialization(writer, this._nativeInvokerSignature, this._invokeMethod.FullName, new Action<CppCodeWriter, MetadataUsage, MethodUsage>(this, (IntPtr) this.<WriteNativeInvoker>m__1), this._nativeInvokerName);
        }

        public override void WriteNativeStructDefinition(CppCodeWriter writer)
        {
            foreach (ParameterDefinition definition in this._invokeMethod.Parameters)
            {
                MarshalDataCollector.MarshalInfoWriterFor(this._typeResolver.Resolve(definition.ParameterType), MarshalType.WindowsRuntime, null, true, false, false, null).WriteMarshaledTypeForwardDeclaration(writer);
            }
            MarshalDataCollector.MarshalInfoWriterFor(this._typeResolver.Resolve(this._invokeMethod.ReturnType), MarshalType.WindowsRuntime, null, true, false, false, null).WriteMarshaledTypeForwardDeclaration(writer);
            writer.WriteCommentedLine(string.Format("COM Callable Wrapper interface definition for {0}", base._typeRef.FullName));
            object[] args = new object[] { this._comCallableWrapperInterfaceName };
            writer.WriteLine("struct {0} : Il2CppIUnknown", args);
            using (new BlockWriter(writer, true))
            {
                writer.WriteLine("static const Il2CppGuid IID;");
                object[] objArray2 = new object[] { this._parameterList };
                writer.WriteLine("virtual il2cpp_hresult_t STDCALL Invoke({0}) = 0;", objArray2);
            }
            writer.WriteLine();
            string str = string.Format("il2cpp::vm::ComObjectBase<{0}, {1}, Il2CppIInspectable>", this._comCallableWrapperClassName, this._comCallableWrapperInterfaceName);
            writer.WriteCommentedLine(string.Format("COM Callable Wrapper class definition for {0}", base._typeRef.FullName));
            writer.WriteLine(string.Format("struct {0} : {1}", this._comCallableWrapperClassName, str));
            using (new BlockWriter(writer, true))
            {
                object[] objArray3 = new object[] { this._comCallableWrapperClassName, DefaultMarshalInfoWriter.Naming.ForVariable(DefaultMarshalInfoWriter.TypeProvider.ObjectTypeReference) };
                writer.WriteLine("inline {0}({1} obj) : ", objArray3);
                writer.Indent(1);
                writer.WriteLine(string.Format("{0}(obj)", str));
                writer.Dedent(1);
                using (new BlockWriter(writer, false))
                {
                }
                object[] objArray4 = new object[] { this._parameterList };
                writer.WriteLine("virtual il2cpp_hresult_t STDCALL Invoke({0});", objArray4);
            }
        }

        public override void WriteNativeVariableDeclarationOfType(CppCodeWriter writer, string variableName)
        {
            object[] args = new object[] { this._comCallableWrapperInterfaceName, variableName, DefaultMarshalInfoWriter.Naming.Null };
            writer.WriteLine("{0}* {1} = {2};", args);
        }

        public override MarshaledType[] MarshaledTypes
        {
            get
            {
                return this._marshaledTypes;
            }
        }
    }
}

