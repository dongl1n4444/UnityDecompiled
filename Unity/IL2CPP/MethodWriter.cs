namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using Unity.IL2CPP.Debugger;
    using Unity.IL2CPP.ILPreProcessor;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;
    using Unity.IL2CPP.Marshaling.BodyWriters.ManagedToNative;
    using Unity.IL2CPP.Marshaling.BodyWriters.NativeToManaged;
    using Unity.IL2CPP.Metadata;

    public class MethodWriter
    {
        private readonly TypeReference _type;
        private readonly VTableBuilder _vTableBuilder;
        private readonly CppCodeWriter _writer;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<StringMetadataToken, string> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache4;
        [Inject]
        public static IGenericSharingAnalysisService GenericSharingAnalysis;
        [Inject]
        public static IIcallMappingService IcallMap;
        [Inject]
        public static IIl2CppGenericMethodCollectorWriterService Il2CppGenericMethodCollector;
        [Inject]
        public static IIl2CppTypeCollectorWriterService Il2CppTypeCollector;
        [Inject]
        public static IMetadataUsageCollectorWriterService MetadataUsageCollector;
        [Inject]
        public static INamingService Naming;
        [Inject]
        public static IRuntimeImplementedMethodWriter RuntimeImplementedMethodWriter;
        [Inject]
        public static IStatsService StatsService;
        [Inject]
        public static ITypeProviderService TypeProvider;
        [Inject]
        public static IWindowsRuntimeProjections WindowsRuntimeProjections;

        public MethodWriter(TypeReference type, CppCodeWriter writer, VTableBuilder vTableBuilder)
        {
            this._type = type;
            this._writer = writer;
            this._vTableBuilder = vTableBuilder;
        }

        private void AddRetInstructionAtTheEndIfNeeded(MethodDefinition method)
        {
            if ((method.HasBody && method.Body.HasExceptionHandlers) && (method.Body.Instructions[method.Body.Instructions.Count - 1].OpCode != OpCodes.Ret))
            {
                ExceptionHandler handler = method.Body.ExceptionHandlers[method.Body.ExceptionHandlers.Count - 1];
                if (handler.HandlerEnd == null)
                {
                    if (method.ReturnType.MetadataType != MetadataType.Void)
                    {
                        this.InjectEmptyVariableToTheStack(method.ReturnType, method.Body);
                    }
                    Instruction instruction = method.Body.Instructions[method.Body.Instructions.Count - 1];
                    Instruction item = Instruction.Create(OpCodes.Ret);
                    item.Offset = instruction.Offset + instruction.GetSize();
                    handler.HandlerEnd = item;
                    method.Body.Instructions.Add(item);
                }
            }
        }

        private static void CallBaseTypeFinalizer(MethodDefinition finalizer, CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
        {
            MethodReference method = null;
            TypeDefinition definition;
            Unity.IL2CPP.ILPreProcessor.TypeResolver resolver;
            for (TypeReference reference2 = finalizer.DeclaringType.BaseType; reference2 != null; reference2 = resolver.Resolve(definition.BaseType))
            {
                definition = reference2.Resolve();
                resolver = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(reference2);
                foreach (MethodDefinition definition2 in definition.Methods)
                {
                    if (definition2.IsFinalizerMethod())
                    {
                        method = resolver.Resolve(definition2);
                        break;
                    }
                }
            }
            if (method != null)
            {
                List<string> argumentArray = new List<string>(2) {
                    Naming.ThisParameterName
                };
                if (MethodSignatureWriter.NeedsHiddenMethodInfo(method, MethodCallType.Normal, false))
                {
                    argumentArray.Add(metadataAccess.HiddenMethodInfo(method));
                }
                Unity.IL2CPP.ILPreProcessor.TypeResolver typeResolverForMethodToCall = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(finalizer.DeclaringType);
                string block = MethodBodyWriter.GetMethodCallExpression(finalizer, typeResolverForMethodToCall.Resolve(method), method, typeResolverForMethodToCall, MethodCallType.Normal, metadataAccess, new VTableBuilder(), argumentArray, false, null);
                writer.WriteStatement(block);
            }
        }

        private static void EmitDirectICallInvocation(MethodReference method, CppCodeWriter writer, string ret, string icall, MethodDefinition methodDefinition)
        {
            object[] args = new object[] { MethodSignatureWriter.GetICallMethodVariable(methodDefinition) };
            writer.WriteLine("typedef {0};", args);
            writer.WriteLine("using namespace il2cpp::icalls;");
            object[] objArray2 = new object[] { ret, Naming.ForMethodNameOnly(method), icall, MethodSignatureWriter.FormatParameters(method, ParameterFormat.WithName, methodDefinition.IsStatic, false, false) };
            writer.WriteLine("{0} (({1}_ftn){2}) ({3});", objArray2);
        }

        private static void EmitFunctionPointerICallInvocation(MethodReference method, CppCodeWriter writer, MethodDefinition methodDefinition, string ret)
        {
            writer.WriteInternalCallResolutionStatement(methodDefinition);
            if ((CodeGenOptions.MonoRuntime && !string.IsNullOrEmpty(ret)) && MethodSignatureWriter.UsesMonoCodegenICallHandle(method.Resolve()))
            {
                string str = Naming.ForVariable(method.ReturnType);
                object[] args = new object[] { str, str, "_il2cpp_icall_func", MethodSignatureWriter.FormatParameters(method, ParameterFormat.WithName, methodDefinition.IsStatic, false, true) };
                writer.WriteLine("{0} retVal = ({1})MONO_TYPED_HANDLE_GET_INNER_OBJECT({2}({3}));", args);
                if (CodeGenOptions.MonoRuntime)
                {
                    writer.WriteLine("MonoException *exc = mono_unity_thread_check_exception();");
                    writer.WriteLine("if (exc) mono_raise_exception(exc);");
                }
                writer.WriteLine("return retVal;");
            }
            else if (string.IsNullOrEmpty(ret))
            {
                object[] objArray2 = new object[] { "_il2cpp_icall_func", MethodSignatureWriter.FormatParameters(method, ParameterFormat.WithName, methodDefinition.IsStatic, false, true) };
                writer.WriteLine("{0}({1});", objArray2);
                if (CodeGenOptions.MonoRuntime)
                {
                    writer.WriteLine("MonoException *exc = mono_unity_thread_check_exception();");
                    writer.WriteLine("if (exc) mono_raise_exception(exc);");
                }
            }
            else
            {
                object[] objArray3 = new object[] { Naming.ForVariable(method.ReturnType), "_il2cpp_icall_func", MethodSignatureWriter.FormatParameters(method, ParameterFormat.WithName, methodDefinition.IsStatic, false, true) };
                writer.WriteLine("{0} retVal = {1}({2});", objArray3);
                if (CodeGenOptions.MonoRuntime)
                {
                    writer.WriteLine("MonoException *exc = mono_unity_thread_check_exception();");
                    writer.WriteLine("if (exc) mono_raise_exception(exc);");
                }
                writer.WriteLine("return retVal;");
            }
        }

        public static IRuntimeMetadataAccess GetDefaultRuntimeMetadataAccess(MethodReference method, MetadataUsage metadataUsage, MethodUsage methodUsage, IMethodVerifier methodVerifier = null)
        {
            DefaultRuntimeMetadataAccess defaultRuntimeMetadataAccess = new DefaultRuntimeMetadataAccess(method, metadataUsage, methodUsage, methodVerifier);
            if ((method != null) && GenericSharingAnalysis.IsSharedMethod(method))
            {
                SharedRuntimeMetadataAccess defaultSharedRuntimeMetadataAccess = new SharedRuntimeMetadataAccess(method, defaultRuntimeMetadataAccess);
                return (!CodeGenOptions.MonoRuntime ? ((IRuntimeMetadataAccess) defaultSharedRuntimeMetadataAccess) : ((IRuntimeMetadataAccess) new MonoSharedRuntimeMetadataAccess(method, defaultSharedRuntimeMetadataAccess)));
            }
            return (!CodeGenOptions.MonoRuntime ? ((IRuntimeMetadataAccess) defaultRuntimeMetadataAccess) : ((IRuntimeMetadataAccess) new MonoRuntimeMetadataAccess(defaultRuntimeMetadataAccess, method, metadataUsage, methodUsage)));
        }

        private void InjectEmptyVariableToTheStack(TypeReference type, MethodBody body)
        {
            Instruction instruction;
            VariableDefinition definition;
            if (!type.IsValueType())
            {
                instruction = Instruction.Create(OpCodes.Ldnull);
                goto Label_00F3;
            }
            if (type.IsPrimitive && (type.MetadataType != MetadataType.UIntPtr))
            {
                switch (type.MetadataType)
                {
                    case MetadataType.Boolean:
                    case MetadataType.Char:
                    case MetadataType.SByte:
                    case MetadataType.Byte:
                    case MetadataType.Int16:
                    case MetadataType.UInt16:
                    case MetadataType.Int32:
                    case MetadataType.UInt32:
                    case MetadataType.Int64:
                    case MetadataType.UInt64:
                        instruction = Instruction.Create(OpCodes.Ldc_I4_0);
                        goto Label_00F3;

                    case MetadataType.Single:
                        instruction = Instruction.Create(OpCodes.Ldc_R4, (float) 0f);
                        goto Label_00F3;

                    case MetadataType.Double:
                        instruction = Instruction.Create(OpCodes.Ldc_R8, (double) 0.0);
                        goto Label_00F3;

                    case MetadataType.IntPtr:
                        goto Label_00D2;
                }
                throw new Exception();
            }
        Label_00D2:
            definition = new VariableDefinition(type);
            body.Variables.Add(definition);
            instruction = Instruction.Create(OpCodes.Ldloc, definition);
        Label_00F3:
            body.Instructions.Add(instruction);
            instruction.Offset = instruction.Previous.Offset + instruction.Previous.GetSize();
        }

        internal static bool IsGenericVirtualMethod(MethodDefinition method) => 
            (method.IsVirtual && method.HasGenericParameters);

        internal static bool IsGetOrSetGenericValueImplOnArray(MethodReference method) => 
            (method.DeclaringType.IsSystemArray() && ((method.Name == "GetGenericValueImpl") || (method.Name == "SetGenericValueImpl")));

        private static bool IsUnconstructibleWindowsRuntimeClass(TypeReference type, out string errorMessage)
        {
            if (type.IsAttribute())
            {
                errorMessage = $"Cannot construct type '{type.FullName}'. Windows Runtime attribute types are not constructable.";
                return true;
            }
            TypeReference reference = WindowsRuntimeProjections.ProjectToCLR(type);
            if (reference != type)
            {
                errorMessage = $"Cannot construct type '{type.FullName}'. It has no managed representation. Instead, use '{reference.FullName}'.";
                return true;
            }
            errorMessage = null;
            return false;
        }

        internal static bool MethodCanBeDirectlyCalled(MethodReference method) => 
            ((!method.DeclaringType.IsInterface() && !method.Resolve().IsAbstract) || method.DeclaringType.IsComOrWindowsRuntimeInterface());

        internal static bool MethodNeedsInvoker(MethodDefinition method)
        {
            if (method.IsConstructor && method.DeclaringType.IsDelegate())
            {
                return false;
            }
            return MethodCanBeDirectlyCalled(method);
        }

        private static bool MethodNeedsWritten(MethodReference method)
        {
            if (IsGetOrSetGenericValueImplOnArray(method))
            {
                return false;
            }
            if (GenericsUtilities.IsGenericInstanceOfCompareExchange(method))
            {
                return false;
            }
            if (GenericsUtilities.IsGenericInstanceOfExchange(method))
            {
                return false;
            }
            if (method.IsStripped())
            {
                return false;
            }
            return MethodCanBeDirectlyCalled(method);
        }

        private static void ReleaseCachedInterfaces(CppCodeWriter writer, TypeDefinition declaringType)
        {
            TypeReference[] referenceArray = declaringType.ImplementedComOrWindowsRuntimeInterfaces().ToArray<TypeReference>();
            if (referenceArray.Length != 0)
            {
                bool flag = declaringType.GetComposableFactoryTypes().Count<TypeReference>() > 0;
                if (flag)
                {
                    writer.WriteLine($"if ({Naming.ThisParameterName}->klass->is_import_or_windows_runtime)");
                    writer.BeginBlock();
                }
                foreach (TypeReference reference in referenceArray)
                {
                    string str = Naming.ForComTypeInterfaceFieldName(reference);
                    writer.WriteLine($"if ({Naming.ThisParameterName}->{str} != {Naming.Null})");
                    using (new BlockWriter(writer, false))
                    {
                        writer.WriteLine($"{Naming.ThisParameterName}->{str}->Release();");
                    }
                }
                if (flag)
                {
                    writer.EndBlock(false);
                }
                writer.WriteLine();
                foreach (TypeReference reference2 in referenceArray)
                {
                    string str2 = Naming.ForComTypeInterfaceFieldName(reference2);
                    writer.WriteLine($"{Naming.ThisParameterName}->{str2} = {Naming.Null};");
                }
                writer.WriteLine();
            }
        }

        private static void ReleaseIl2CppObjectIdentity(CppCodeWriter writer)
        {
            string str = Naming.ForIl2CppComObjectIdentityField();
            writer.WriteLine($"if ({Naming.ThisParameterName}->{str} != {Naming.Null})");
            using (new BlockWriter(writer, false))
            {
                writer.WriteLine($"if ({Naming.ThisParameterName}->klass->is_import_or_windows_runtime)");
                using (new BlockWriter(writer, false))
                {
                    writer.WriteLine($"il2cpp_codegen_il2cpp_com_object_cleanup({Naming.ThisParameterName});");
                }
                writer.WriteLine($"{Naming.ThisParameterName}->{str}->Release();");
                writer.WriteLine($"{Naming.ThisParameterName}->{str} = {Naming.Null};");
            }
            writer.WriteLine();
        }

        private static bool ReplaceWithHardcodedAlternativeIfPresent(MethodReference method, CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
        {
            MethodDefinition definition = method.Resolve();
            switch (definition.FullName)
            {
                case "System.Int32 System.Double::GetHashCode()":
                {
                    TypeDefinition type = definition.Module.GetType("System.Int64");
                    if (<>f__am$cache4 == null)
                    {
                        <>f__am$cache4 = m => m.Name == "GetHashCode";
                    }
                    MethodDefinition definition3 = type.Methods.Single<MethodDefinition>(<>f__am$cache4);
                    object[] args = new object[] { Emit.Call(metadataAccess.Method(definition3), Emit.Cast(new ByReferenceType(type), Naming.ThisParameterName), metadataAccess.HiddenMethodInfo(definition3)) };
                    writer.WriteLine("return {0};", args);
                    return true;
                }
                case "R System.Array::UnsafeMov(S)":
                {
                    TypeReference variableType = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(method.DeclaringType, method).Resolve(method.ReturnType);
                    object[] objArray2 = new object[] { Naming.ForVariable(variableType), Naming.ForParameterName(method.Parameters.First<ParameterDefinition>()) };
                    writer.WriteLine("return static_cast<{0}>({1});", objArray2);
                    return true;
                }
                case "System.Void Microsoft.FSharp.Core.CompilerMessageAttribute::.ctor(System.Object,System.Object)":
                    return true;
            }
            return false;
        }

        private static bool ReturnsVoid(MethodReference method) => 
            (method.ReturnType.MetadataType == MetadataType.Void);

        private static bool ShouldWriteIncludeForParameter(TypeReference resolvedParameterType)
        {
            ByReferenceType type = resolvedParameterType as ByReferenceType;
            if (type != null)
            {
                return ShouldWriteIncludeForParameter(type.ElementType);
            }
            PointerType type2 = resolvedParameterType as PointerType;
            if (type2 != null)
            {
                return ShouldWriteIncludeForParameter(type2.ElementType);
            }
            return (((!(resolvedParameterType is TypeSpecification) || (resolvedParameterType is GenericInstanceType)) || (resolvedParameterType is ArrayType)) && !resolvedParameterType.IsGenericParameter);
        }

        private void WriteAdjustorThunk(MethodReference method)
        {
            <WriteAdjustorThunk>c__AnonStorey1 storey = new <WriteAdjustorThunk>c__AnonStorey1 {
                method = method
            };
            storey.typeResolver = new Unity.IL2CPP.ILPreProcessor.TypeResolver(storey.method.DeclaringType as GenericInstanceType, storey.method as GenericInstanceMethod);
            string parameters = MethodSignatureWriter.FormatParameters(storey.method, ParameterFormat.WithTypeAndNameThisObject, false, true, false);
            string methodSignature = MethodSignatureWriter.GetMethodSignature(Naming.ForMethodAdjustorThunk(storey.method), Naming.ForVariable(storey.typeResolver.Resolve(Unity.IL2CPP.GenericParameterResolver.ResolveReturnTypeIfNeeded(storey.method))), parameters, "extern \"C\"", "");
            WriteMethodWithMetadataInitialization(this._writer, methodSignature, Naming.ForMethodAdjustorThunk(storey.method), new Action<CppCodeWriter, MetadataUsage, MethodUsage>(storey.<>m__0), Naming.ForMethodAdjustorThunk(storey.method));
        }

        private static void WriteExternMethodeDeclarationForInternalPInvokeImpl(CppCodeWriter writer, MethodReference method)
        {
            new PInvokeMethodBodyWriter(method).WriteExternMethodeDeclarationForInternalPInvoke(writer);
        }

        private void WriteMethodBody(MethodReference method, CppCodeWriter methodBodyWriter, IRuntimeMetadataAccess metadataAccess)
        {
            MethodDefinition definition = method.Resolve();
            if (!ReplaceWithHardcodedAlternativeIfPresent(method, methodBodyWriter, metadataAccess))
            {
                if (!definition.HasBody)
                {
                    WriteMethodBodyForMethodWithoutBody(method, methodBodyWriter, metadataAccess);
                }
                else
                {
                    this.AddRetInstructionAtTheEndIfNeeded(definition);
                    new MethodBodyWriter(methodBodyWriter, method, new Unity.IL2CPP.ILPreProcessor.TypeResolver(this._type as GenericInstanceType, method as GenericInstanceMethod), metadataAccess, this._vTableBuilder).Generate();
                }
            }
        }

        private static void WriteMethodBodyForComObjectConstructor(MethodReference method, CppCodeWriter writer)
        {
            object[] args = new object[] { Naming.ForTypeNameOnly(method.DeclaringType), Naming.ThisParameterName, Naming.ForIl2CppComObjectIdentityField() };
            writer.WriteLine("il2cpp_codegen_com_create_instance({0}::CLSID, &{1}->{2});", args);
            object[] objArray2 = new object[] { Naming.ThisParameterName };
            writer.WriteLine("il2cpp_codegen_com_register_rcw({0});", objArray2);
        }

        private static void WriteMethodBodyForComOrWindowsRuntimeFinalizer(MethodDefinition finalizer, CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
        {
            TypeDefinition declaringType = finalizer.DeclaringType;
            if (!declaringType.IsIl2CppComObject())
            {
                ReleaseCachedInterfaces(writer, declaringType);
            }
            else
            {
                ReleaseIl2CppObjectIdentity(writer);
            }
            CallBaseTypeFinalizer(finalizer, writer, metadataAccess);
        }

        private static void WriteMethodBodyForComOrWindowsRuntimeMethod(MethodReference method, CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
        {
            MethodDefinition definition = method.Resolve();
            if (definition.IsConstructor)
            {
                if (definition.DeclaringType.IsImport && !definition.DeclaringType.IsWindowsRuntimeProjection())
                {
                    WriteMethodBodyForComObjectConstructor(method, writer);
                }
                else
                {
                    WriteMethodBodyForWindowsRuntimeObjectConstructor(method, writer, metadataAccess);
                }
            }
            else if (definition.IsFinalizerMethod())
            {
                WriteMethodBodyForComOrWindowsRuntimeFinalizer(definition, writer, metadataAccess);
            }
            else if (method.DeclaringType.IsIl2CppComObject() && (method.Name == "ToString"))
            {
                WriteMethodBodyForIl2CppComObjectToString(definition, writer, metadataAccess);
            }
            else if (method.HasThis)
            {
                WriteMethodBodyForDirectComOrWindowsRuntimeCall(method, writer, metadataAccess);
            }
            else
            {
                new ComStaticMethodBodyWriter(method).WriteMethodBody(writer, metadataAccess);
            }
        }

        private static void WriteMethodBodyForDirectComOrWindowsRuntimeCall(MethodReference method, CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
        {
            MethodDefinition definition = method.Resolve();
            if (!definition.IsComOrWindowsRuntimeMethod())
            {
                throw new InvalidOperationException("WriteMethodBodyForDirectComOrWindowsRuntimeCall called for non-COM and non-Windows Runtime method");
            }
            MethodReference interfaceMethod = !definition.DeclaringType.IsInterface ? method.GetOverridenInterfaceMethod(method.DeclaringType.GetInterfaces()) : method;
            if (interfaceMethod == null)
            {
                writer.WriteStatement(Emit.RaiseManagedException($"il2cpp_codegen_get_missing_method_exception("The method '{method.FullName}' has no implementation.")"));
            }
            else if (!interfaceMethod.DeclaringType.IsComOrWindowsRuntimeInterface())
            {
                WriteMethodBodyForProjectedInterfaceMethod(writer, method, interfaceMethod, metadataAccess);
            }
            else
            {
                new ComInstanceMethodBodyWriter(method).WriteMethodBody(writer, metadataAccess);
            }
        }

        private static void WriteMethodBodyForIl2CppComObjectToString(MethodDefinition methodDefinition, CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
        {
            string str = Naming.ForInteropInterfaceVariable(TypeProvider.IStringableType);
            string str2 = Naming.ForTypeNameOnly(TypeProvider.IStringableType);
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = m => m.Name == "ToString";
            }
            MethodDefinition interfaceMethod = TypeProvider.IStringableType.Methods.Single<MethodDefinition>(<>f__am$cache2);
            writer.AddIncludeForTypeDefinition(TypeProvider.IStringableType);
            writer.WriteLine($"{str2}* {str} = {Naming.Null};");
            writer.WriteLine($"if (IL2CPP_HR_SUCCEEDED({Naming.ThisParameterName}->{Naming.ForIl2CppComObjectIdentityField()}->QueryInterface({str2}::IID, reinterpret_cast<void**>(&{str}))))");
            using (new BlockWriter(writer, false))
            {
                new ComMethodWithPreOwnedInterfacePointerMethodBodyWriter(interfaceMethod, true).WriteMethodBody(writer, metadataAccess);
            }
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = m => m.Name == "ToString";
            }
            MethodDefinition method = TypeProvider.SystemObject.Methods.Single<MethodDefinition>(<>f__am$cache3);
            List<string> argumentArray = new List<string> {
                Naming.ThisParameterName
            };
            if (MethodSignatureWriter.NeedsHiddenMethodInfo(method, MethodCallType.Normal, false))
            {
                argumentArray.Add(metadataAccess.HiddenMethodInfo(method));
            }
            string str3 = MethodBodyWriter.GetMethodCallExpression(methodDefinition, method, method, Unity.IL2CPP.ILPreProcessor.TypeResolver.Empty, MethodCallType.Normal, metadataAccess, new VTableBuilder(), argumentArray, true, null);
            writer.WriteLine($"return {str3};");
        }

        private static void WriteMethodBodyForInternalCall(MethodReference method, CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
        {
            MethodDefinition methodToCall = method.Resolve();
            if (!methodToCall.IsInternalCall)
            {
                throw new Exception();
            }
            if (IntrinsicRemap.ShouldRemap(methodToCall))
            {
                string str = IntrinsicRemap.MappedNameFor(methodToCall);
                IEnumerable<string> arguments = MethodSignatureWriter.ParametersFor(methodToCall, ParameterFormat.WithName, methodToCall.IsStatic, false, false, false);
                arguments = !IntrinsicRemap.HasCustomArguments(methodToCall) ? arguments : IntrinsicRemap.GetCustomArguments(methodToCall, methodToCall, metadataAccess, arguments);
                if (methodToCall.ReturnType.MetadataType != MetadataType.Void)
                {
                    object[] args = new object[] { str, arguments.AggregateWithComma() };
                    writer.WriteLine("return {0}({1});", args);
                }
                else
                {
                    object[] objArray2 = new object[] { str, arguments.AggregateWithComma() };
                    writer.WriteLine("{0}({1});", objArray2);
                }
            }
            else
            {
                if (methodToCall.HasGenericParameters)
                {
                    throw new NotSupportedException($"Internal calls cannot have generic parameters: {methodToCall.FullName}");
                }
                string ret = !ReturnsVoid(methodToCall) ? "return " : string.Empty;
                string icall = method.FullName.Substring(method.FullName.IndexOf(" ") + 1);
                string str4 = IcallMap.ResolveICallFunction(icall);
                if ((str4 != null) && !CodeGenOptions.MonoRuntime)
                {
                    EmitDirectICallInvocation(method, writer, ret, str4, methodToCall);
                }
                else
                {
                    EmitFunctionPointerICallInvocation(method, writer, methodToCall, ret);
                }
            }
        }

        private static void WriteMethodBodyForMethodWithoutBody(MethodReference method, CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
        {
            MethodDefinition definition = method.Resolve();
            if (!MethodCanBeDirectlyCalled(definition))
            {
                throw new InvalidOperationException($"Trying to generate a body for method '{method.FullName}'");
            }
            DebuggerSupportFactory.GetDebuggerSupport().WriteCallStackInformation(writer, method, new KeyValuePair<string, TypeReference>[0], metadataAccess);
            if (((definition.IsRuntime && !definition.IsInternalCall) && !definition.DeclaringType.IsInterface) && (method.DeclaringType.Resolve().BaseType.FullName == "System.MulticastDelegate"))
            {
                new DelegateMethodsWriter(writer).WriteMethodBodyForIsRuntimeMethod(method, metadataAccess);
            }
            else if (definition.IsComOrWindowsRuntimeMethod())
            {
                WriteMethodBodyForComOrWindowsRuntimeMethod(method, writer, metadataAccess);
            }
            else if (definition.IsInternalCall)
            {
                WriteMethodBodyForInternalCall(method, writer, metadataAccess);
            }
            else if (definition.IsPInvokeImpl)
            {
                WriteMethodBodyForPInvokeImpl(writer, definition, metadataAccess);
            }
            else if (RuntimeImplementedMethodWriter.IsRuntimeImplementedMethod(definition))
            {
                RuntimeImplementedMethodWriter.WriteMethodBody(writer, method, metadataAccess);
            }
            else
            {
                writer.WriteStatement(Emit.RaiseManagedException($"il2cpp_codegen_get_missing_method_exception("The method '{method.FullName}' has no implementation.")"));
            }
        }

        private static void WriteMethodBodyForPInvokeImpl(CppCodeWriter writer, MethodReference method, IRuntimeMetadataAccess metadataAccess)
        {
            new PInvokeMethodBodyWriter(method).WriteMethodBody(writer, metadataAccess);
        }

        private static void WriteMethodBodyForProjectedInterfaceMethod(CppCodeWriter writer, MethodReference method, MethodReference interfaceMethod, IRuntimeMetadataAccess metadataAccess)
        {
            <WriteMethodBodyForProjectedInterfaceMethod>c__AnonStorey2 storey = new <WriteMethodBodyForProjectedInterfaceMethod>c__AnonStorey2();
            Unity.IL2CPP.ILPreProcessor.TypeResolver resolver = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(interfaceMethod.DeclaringType);
            storey.interfaceMethodDef = interfaceMethod.Resolve();
            TypeDefinition nativeToManagedAdapterClassFor = WindowsRuntimeProjections.GetNativeToManagedAdapterClassFor(interfaceMethod.DeclaringType.Resolve());
            TypeReference typeReference = resolver.Resolve(nativeToManagedAdapterClassFor);
            MethodDefinition definition2 = nativeToManagedAdapterClassFor.Methods.First<MethodDefinition>(new Func<MethodDefinition, bool>(storey.<>m__0));
            MethodReference reference2 = resolver.Resolve(definition2);
            writer.AddForwardDeclaration(typeReference);
            writer.AddIncludeForMethodDeclaration(reference2);
            List<string> arguments = new List<string> {
                $"reinterpret_cast<{Naming.ForVariable(typeReference)}>({Naming.ThisParameterName})"
            };
            foreach (ParameterDefinition definition3 in method.Parameters)
            {
                arguments.Add(Naming.ForParameterName(definition3));
            }
            if (MethodSignatureWriter.NeedsHiddenMethodInfo(method, MethodCallType.Normal, false))
            {
                arguments.Add("method");
            }
            string block = Emit.Call(Naming.ForMethod(reference2), arguments);
            if (resolver.Resolve(interfaceMethod.ReturnType).MetadataType != MetadataType.Void)
            {
                block = "return " + block;
            }
            writer.WriteStatement(block);
        }

        private static void WriteMethodBodyForWindowsRuntimeObjectConstructor(MethodReference method, CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
        {
            string str;
            if (method.Resolve().HasGenericParameters)
            {
                throw new InvalidOperationException("Cannot construct generic Windows Runtime objects.");
            }
            if (IsUnconstructibleWindowsRuntimeClass(method.DeclaringType, out str))
            {
                writer.WriteStatement(Emit.RaiseManagedException($"il2cpp_codegen_get_invalid_operation_exception("{str}")"));
            }
            else
            {
                new WindowsRuntimeConstructorMethodBodyWriter(method).WriteMethodBody(writer, metadataAccess);
            }
        }

        public void WriteMethodDefinition(MethodReference method, IMethodCollector methodCollector, IMethodVerifier methodVerifier)
        {
            <WriteMethodDefinition>c__AnonStorey0 storey = new <WriteMethodDefinition>c__AnonStorey0 {
                method = method,
                methodVerifier = methodVerifier,
                $this = this
            };
            if (MethodNeedsWritten(storey.method))
            {
                string sharedMethodSignature;
                MethodDefinition definition = storey.method.Resolve();
                if (definition.IsPInvokeImpl)
                {
                    WriteExternMethodeDeclarationForInternalPInvokeImpl(this._writer, definition);
                }
                this._writer.WriteCommentedLine(storey.method.FullName);
                if (storey.method.IsGenericInstance || storey.method.DeclaringType.IsGenericInstance)
                {
                    Il2CppGenericMethodCollector.Add(storey.method);
                }
                if (GenericSharingAnalysis.CanShareMethod(storey.method))
                {
                    StatsService.ShareableMethods++;
                    GenericSharingAnalysis.AddSharedMethod(GenericSharingAnalysis.GetSharedMethod(storey.method), storey.method);
                    if (!GenericSharingAnalysis.IsSharedMethod(storey.method))
                    {
                        return;
                    }
                    sharedMethodSignature = MethodSignatureWriter.GetSharedMethodSignature(this._writer, storey.method);
                }
                else
                {
                    sharedMethodSignature = MethodSignatureWriter.GetMethodSignature(this._writer, storey.method);
                }
                methodCollector.AddMethod(storey.method);
                StatsService.RecordMethod(storey.method);
                Unity.IL2CPP.ILPreProcessor.TypeResolver resolver = new Unity.IL2CPP.ILPreProcessor.TypeResolver(storey.method.DeclaringType as GenericInstanceType, storey.method as GenericInstanceMethod);
                TypeReference typeReference = resolver.Resolve(Unity.IL2CPP.GenericParameterResolver.ResolveReturnTypeIfNeeded(storey.method));
                this._writer.AddIncludeForTypeDefinition(typeReference);
                foreach (ParameterDefinition definition2 in storey.method.Parameters)
                {
                    TypeReference resolvedParameterType = Unity.IL2CPP.GenericParameterResolver.ResolveParameterTypeIfNeeded(storey.method, definition2);
                    if (ShouldWriteIncludeForParameter(resolvedParameterType))
                    {
                        this._writer.AddIncludeForTypeDefinition(resolver.Resolve(resolvedParameterType));
                    }
                }
                WriteMethodWithMetadataInitialization(this._writer, sharedMethodSignature, storey.method.FullName, new Action<CppCodeWriter, MetadataUsage, MethodUsage>(storey.<>m__0), Naming.ForMethod(storey.method));
                if (storey.method.HasThis && storey.method.DeclaringType.IsValueType())
                {
                    this.WriteAdjustorThunk(storey.method);
                }
            }
        }

        internal void WriteMethodDefinitions(IMethodCollector methodCollector, IMethodVerifier methodVerifier)
        {
            TypeDefinition definition = this._type.Resolve();
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = m => !m.HasGenericParameters;
            }
            foreach (MethodDefinition definition2 in definition.Methods.Where<MethodDefinition>(<>f__am$cache0))
            {
                ErrorInformation.CurrentlyProcessing.Method = definition2;
                if (CodeGenOptions.EnableErrorMessageTest)
                {
                    ErrorTypeAndMethod.ThrowIfIsErrorMethod(definition2);
                }
                MethodReference method = definition2;
                GenericInstanceType genericInstanceType = this._type as GenericInstanceType;
                if (genericInstanceType != null)
                {
                    method = VTableBuilder.CloneMethodReference(genericInstanceType, definition2);
                }
                this.WriteMethodDefinition(method, methodCollector, methodVerifier);
            }
        }

        internal static void WriteMethodForDelegatePInvokeIfNeeded(CppCodeWriter _writer, MethodReference method, IInteropDataCollector interopDataCollector)
        {
            <WriteMethodForDelegatePInvokeIfNeeded>c__AnonStorey3 storey = new <WriteMethodForDelegatePInvokeIfNeeded>c__AnonStorey3 {
                method = method
            };
            storey.delegatePInvokeMethodBodyWriter = new DelegatePInvokeMethodBodyWriter(storey.method);
            if (storey.delegatePInvokeMethodBodyWriter.IsDelegatePInvokeWrapperNecessary())
            {
                Unity.IL2CPP.ILPreProcessor.TypeResolver resolver = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(storey.method.DeclaringType, storey.method);
                string name = Naming.ForDelegatePInvokeWrapper(storey.method.DeclaringType);
                string methodSignature = MethodSignatureWriter.GetMethodSignature(name, Naming.ForVariable(resolver.Resolve(storey.method.ReturnType)), MethodSignatureWriter.FormatParameters(storey.method, ParameterFormat.WithTypeAndName, false, true, false), "extern \"C\"", "");
                WriteMethodWithMetadataInitialization(_writer, methodSignature, storey.method.FullName, new Action<CppCodeWriter, MetadataUsage, MethodUsage>(storey.<>m__0), name);
                interopDataCollector.AddWrapperForDelegateFromManagedToNative(storey.method);
            }
        }

        private static void WriteMethodMetadataInitialization(CppCodeWriter writer, string identifier)
        {
            writer.WriteStatement("static bool s_Il2CppMethodInitialized");
            object[] args = new object[] { "s_Il2CppMethodInitialized" };
            writer.WriteLine("if (!{0})", args);
            writer.BeginBlock();
            writer.WriteStatement($"il2cpp_codegen_initialize_method ({identifier})");
            writer.WriteStatement(Emit.Assign("s_Il2CppMethodInitialized", "true"));
            writer.EndBlock(false);
        }

        private static void WriteMethodMetadataInitializationDeclarations(CppCodeWriter writer, string identifier, IEnumerable<TypeReference> types, IEnumerable<TypeReference> typeInfos, IEnumerable<MethodReference> methods, IEnumerable<FieldReference> fields, IEnumerable<string> stringLiterals)
        {
            foreach (TypeReference reference in types)
            {
                writer.AddForwardDeclaration($"extern const {"RuntimeType"}* " + Naming.ForRuntimeIl2CppType(reference));
            }
            foreach (TypeReference reference2 in typeInfos)
            {
                writer.AddForwardDeclaration($"extern {"RuntimeClass"}* " + Naming.ForRuntimeTypeInfo(reference2));
            }
            foreach (MethodReference reference3 in methods)
            {
                writer.AddForwardDeclaration($"extern const {"RuntimeMethod"}* " + Naming.ForRuntimeMethodInfo(reference3));
            }
            foreach (FieldReference reference4 in fields)
            {
                writer.AddForwardDeclaration($"extern {"RuntimeField"}* " + Naming.ForRuntimeFieldInfo(reference4));
            }
            foreach (string str in stringLiterals)
            {
                writer.AddForwardDeclaration("extern Il2CppCodeGenString* " + Naming.ForStringLiteralIdentifier(str));
            }
            writer.AddForwardDeclaration("extern const uint32_t " + identifier);
        }

        internal static void WriteMethodWithMetadataInitialization(CppCodeWriter writer, string methodSignature, string methodFullName, Action<CppCodeWriter, MetadataUsage, MethodUsage> writeMethodBody, string uniqueIdentifier)
        {
            string identifier = uniqueIdentifier + "_MetadataUsageId";
            MetadataUsage usage = new MetadataUsage();
            MethodUsage usage2 = new MethodUsage();
            using (CppCodeWriter writer2 = new InMemoryCodeWriter())
            {
                using (CppCodeWriter writer3 = new InMemoryCodeWriter())
                {
                    writer3.Indent(writer.IndentationLevel + 1);
                    writer2.Indent(writer.IndentationLevel + 1);
                    writeMethodBody(writer3, usage, usage2);
                    if (usage.UsesMetadata)
                    {
                        WriteMethodMetadataInitialization(writer2, identifier);
                    }
                    writer3.Dedent(writer.IndentationLevel + 1);
                    writer2.Dedent(writer.IndentationLevel + 1);
                    foreach (MethodReference reference in usage2.GetMethods())
                    {
                        writer.AddIncludeForMethodDeclaration(reference);
                    }
                    if (usage.UsesMetadata)
                    {
                        if (<>f__am$cache1 == null)
                        {
                            <>f__am$cache1 = s => s.Literal;
                        }
                        WriteMethodMetadataInitializationDeclarations(writer, identifier, usage.GetIl2CppTypes(), usage.GetTypeInfos(), usage.GetInflatedMethods(), usage.GetFieldInfos(), usage.GetStringLiterals().Select<StringMetadataToken, string>(<>f__am$cache1));
                    }
                    using (new OptimizationWriter(writer, methodFullName))
                    {
                        writer.WriteLine(methodSignature);
                        using (new BlockWriter(writer, false))
                        {
                            writer.Write(writer2);
                            writer.Write(writer3);
                        }
                    }
                }
            }
            if (usage.UsesMetadata)
            {
                MetadataUsageCollector.Add(identifier, usage);
            }
        }

        private static void WritePrologue(MethodReference method, CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
        {
            if (CodeGenOptions.EnableStacktrace && !DebuggerOptions.Enabled)
            {
                if (CodeGenOptions.MonoRuntime)
                {
                    object[] args = new object[] { metadataAccess.MethodInfo(method) };
                    writer.WriteLine("StackTraceSentry _stackTraceSentry((MonoMethod*){0});", args);
                }
                else
                {
                    object[] objArray2 = new object[] { metadataAccess.MethodInfo(method) };
                    writer.WriteLine("StackTraceSentry _stackTraceSentry({0});", objArray2);
                }
            }
            if (CodeGenOptions.EnableDeepProfiler)
            {
                object[] objArray3 = new object[] { metadataAccess.MethodInfo(method) };
                writer.WriteLine("ProfilerMethodSentry _profilerMethodSentry({0});", objArray3);
            }
        }

        private static void WriteReversePInvokeMethodDeclaration(CppCodeWriter writer, MethodReference method)
        {
            ReversePInvokeMethodBodyWriter.Create(method).WriteMethodDeclaration(writer);
        }

        internal static void WriteReversePInvokeMethodDefinition(CppCodeWriter writer, MethodReference method, IInteropDataCollector interopDataCollector)
        {
            ReversePInvokeMethodBodyWriter.Create(method).WriteMethodDefinition(writer, interopDataCollector);
        }

        [CompilerGenerated]
        private sealed class <WriteAdjustorThunk>c__AnonStorey1
        {
            private static Func<FieldDefinition, bool> <>f__am$cache0;
            private static Func<FieldDefinition, bool> <>f__am$cache1;
            private static Func<FieldDefinition, bool> <>f__am$cache2;
            internal MethodReference method;
            internal Unity.IL2CPP.ILPreProcessor.TypeResolver typeResolver;

            internal void <>m__0(CppCodeWriter bodyWriter, MetadataUsage metadataUsage, MethodUsage methodUsage)
            {
                string str;
                IRuntimeMetadataAccess access = MethodWriter.GetDefaultRuntimeMetadataAccess(this.method, metadataUsage, methodUsage, null);
                if (this.method.DeclaringType.IsNullable())
                {
                    object[] args = new object[] { MethodWriter.Naming.ForVariable(this.method.DeclaringType) };
                    bodyWriter.WriteLine("{0} _thisAdjusted;", args);
                    object[] objArray2 = new object[3];
                    if (<>f__am$cache0 == null)
                    {
                        <>f__am$cache0 = f => f.Name == "value";
                    }
                    objArray2[0] = MethodWriter.Naming.ForFieldSetter(this.method.DeclaringType.Resolve().Fields.Single<FieldDefinition>(<>f__am$cache0));
                    objArray2[1] = MethodWriter.Naming.ForVariable(((GenericInstanceType) this.method.DeclaringType).GenericArguments[0]);
                    objArray2[2] = MethodWriter.Naming.ThisParameterName;
                    bodyWriter.WriteLine("_thisAdjusted.{0}(*reinterpret_cast<{1}*>({2} + 1));", objArray2);
                    object[] objArray3 = new object[1];
                    if (<>f__am$cache1 == null)
                    {
                        <>f__am$cache1 = f => f.Name == "has_value";
                    }
                    objArray3[0] = MethodWriter.Naming.ForFieldSetter(this.method.DeclaringType.Resolve().Fields.Single<FieldDefinition>(<>f__am$cache1));
                    bodyWriter.WriteLine("_thisAdjusted.{0}(true);", objArray3);
                    str = "&_thisAdjusted";
                }
                else
                {
                    object[] objArray4 = new object[] { MethodWriter.Naming.ForVariable(this.method.DeclaringType), MethodWriter.Naming.ThisParameterName };
                    bodyWriter.WriteLine("{0}* _thisAdjusted = reinterpret_cast<{0}*>({1} + 1);", objArray4);
                    str = "_thisAdjusted";
                }
                List<string> arguments = new List<string> {
                    str
                };
                for (int i = 0; i < this.method.Parameters.Count; i++)
                {
                    arguments.Add(MethodWriter.Naming.ForParameterName(this.method.Parameters[i]));
                }
                arguments.Add("method");
                if (this.method.DeclaringType.IsNullable())
                {
                    if (this.method.ReturnType.MetadataType != MetadataType.Void)
                    {
                        object[] objArray5 = new object[] { MethodWriter.Naming.ForVariable(this.typeResolver.Resolve(this.method.ReturnType)), Emit.Call(access.Method(this.method), arguments) };
                        bodyWriter.WriteLine("{0} _returnValue = {1};", objArray5);
                    }
                    else
                    {
                        bodyWriter.WriteStatement(Emit.Call(access.Method(this.method), arguments));
                    }
                    object[] objArray6 = new object[3];
                    if (<>f__am$cache2 == null)
                    {
                        <>f__am$cache2 = f => f.Name == "value";
                    }
                    objArray6[0] = MethodWriter.Naming.ForFieldGetter(this.method.DeclaringType.Resolve().Fields.Single<FieldDefinition>(<>f__am$cache2));
                    objArray6[1] = MethodWriter.Naming.ForVariable(((GenericInstanceType) this.method.DeclaringType).GenericArguments[0]);
                    objArray6[2] = MethodWriter.Naming.ThisParameterName;
                    bodyWriter.WriteLine("*reinterpret_cast<{1}*>({2} + 1) = _thisAdjusted.{0}();", objArray6);
                    if (this.method.ReturnType.MetadataType != MetadataType.Void)
                    {
                        bodyWriter.WriteLine("return _returnValue;");
                    }
                }
                else
                {
                    string str2 = (this.method.ReturnType.MetadataType == MetadataType.Void) ? string.Empty : "return ";
                    object[] objArray7 = new object[] { str2, Emit.Call(access.Method(this.method), arguments) };
                    bodyWriter.WriteLine("{0}{1};", objArray7);
                }
            }

            private static bool <>m__1(FieldDefinition f) => 
                (f.Name == "value");

            private static bool <>m__2(FieldDefinition f) => 
                (f.Name == "has_value");

            private static bool <>m__3(FieldDefinition f) => 
                (f.Name == "value");
        }

        [CompilerGenerated]
        private sealed class <WriteMethodBodyForProjectedInterfaceMethod>c__AnonStorey2
        {
            internal MethodDefinition interfaceMethodDef;

            internal bool <>m__0(MethodDefinition m) => 
                m.Overrides.Any<MethodReference>(o => (o.Resolve() == this.interfaceMethodDef));

            internal bool <>m__1(MethodReference o) => 
                (o.Resolve() == this.interfaceMethodDef);
        }

        [CompilerGenerated]
        private sealed class <WriteMethodDefinition>c__AnonStorey0
        {
            internal MethodWriter $this;
            internal MethodReference method;
            internal IMethodVerifier methodVerifier;

            internal void <>m__0(CppCodeWriter bodyWriter, MetadataUsage metadataUsage, MethodUsage methodUsage)
            {
                MethodWriter.WritePrologue(this.method, bodyWriter, MethodWriter.GetDefaultRuntimeMetadataAccess(this.method, metadataUsage, methodUsage, this.methodVerifier));
                this.$this.WriteMethodBody(this.method, bodyWriter, MethodWriter.GetDefaultRuntimeMetadataAccess(this.method, metadataUsage, methodUsage, this.methodVerifier));
            }
        }

        [CompilerGenerated]
        private sealed class <WriteMethodForDelegatePInvokeIfNeeded>c__AnonStorey3
        {
            internal DelegatePInvokeMethodBodyWriter delegatePInvokeMethodBodyWriter;
            internal MethodReference method;

            internal void <>m__0(CppCodeWriter bodyWriter, MetadataUsage metadataUsage, MethodUsage methodUsage)
            {
                IRuntimeMetadataAccess metadataAccess = MethodWriter.GetDefaultRuntimeMetadataAccess(this.method, metadataUsage, methodUsage, null);
                this.delegatePInvokeMethodBodyWriter.WriteMethodBody(bodyWriter, metadataAccess);
            }
        }
    }
}

