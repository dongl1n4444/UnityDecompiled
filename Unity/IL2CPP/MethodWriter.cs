namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using Unity.IL2CPP.Com;
    using Unity.IL2CPP.Debugger;
    using Unity.IL2CPP.ILPreProcessor;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;
    using Unity.IL2CPP.Marshaling;
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
        private static Func<FieldDefinition, bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<FieldDefinition, bool> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<FieldDefinition, bool> <>f__am$cache3;
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

        public static IRuntimeMetadataAccess GetDefaultRuntimeMetadataAccess(MethodReference method, MetadataUsage metadataUsage, MethodUsage methodUsage)
        {
            DefaultRuntimeMetadataAccess defaultRuntimeMetadataAccess = new DefaultRuntimeMetadataAccess(method, metadataUsage, methodUsage);
            return (((method == null) || !GenericSharingAnalysis.IsSharedMethod(method)) ? ((IRuntimeMetadataAccess) defaultRuntimeMetadataAccess) : ((IRuntimeMetadataAccess) new SharedRuntimeMetadataAccess(method, defaultRuntimeMetadataAccess)));
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

        internal static bool MethodCanBeDirectlyCalled(MethodDefinition method) => 
            ((!method.DeclaringType.IsInterface && !method.IsAbstract) || method.DeclaringType.IsComOrWindowsRuntimeInterface());

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
            return MethodCanBeDirectlyCalled(method.Resolve());
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
                    writer.AddIncludeForMethodDeclarations(type);
                    object[] args = new object[] { Emit.Call(Naming.ForMethod(definition3), Emit.Cast(new ByReferenceType(type), Naming.ThisParameterName), metadataAccess.HiddenMethodInfo(definition3)) };
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

        private void WriteAdjustorThunk(MethodReference method, IMethodCollector methodCollector)
        {
            Unity.IL2CPP.ILPreProcessor.TypeResolver resolver = new Unity.IL2CPP.ILPreProcessor.TypeResolver(method.DeclaringType as GenericInstanceType, method as GenericInstanceMethod);
            string parameters = MethodSignatureWriter.FormatParameters(method, ParameterFormat.WithTypeAndNameThisObject, false, true);
            string block = MethodSignatureWriter.GetMethodSignature(Naming.ForMethodAdjustorThunk(method), Naming.ForVariable(resolver.Resolve(Unity.IL2CPP.GenericParameterResolver.ResolveReturnTypeIfNeeded(method))), parameters, "extern \"C\"", "");
            this._writer.WriteLine(block);
            using (new BlockWriter(this._writer, false))
            {
                string str3;
                if (method.DeclaringType.IsNullable())
                {
                    object[] args = new object[] { Naming.ForVariable(method.DeclaringType) };
                    this._writer.WriteLine("{0} _thisAdjusted;", args);
                    object[] objArray2 = new object[3];
                    if (<>f__am$cache1 == null)
                    {
                        <>f__am$cache1 = f => f.Name == "value";
                    }
                    objArray2[0] = Naming.ForFieldSetter(method.DeclaringType.Resolve().Fields.Single<FieldDefinition>(<>f__am$cache1));
                    objArray2[1] = Naming.ForVariable(((GenericInstanceType) method.DeclaringType).GenericArguments[0]);
                    objArray2[2] = Naming.ThisParameterName;
                    this._writer.WriteLine("_thisAdjusted.{0}(*reinterpret_cast<{1}*>({2} + 1));", objArray2);
                    object[] objArray3 = new object[1];
                    if (<>f__am$cache2 == null)
                    {
                        <>f__am$cache2 = f => f.Name == "has_value";
                    }
                    objArray3[0] = Naming.ForFieldSetter(method.DeclaringType.Resolve().Fields.Single<FieldDefinition>(<>f__am$cache2));
                    this._writer.WriteLine("_thisAdjusted.{0}(true);", objArray3);
                    str3 = "&_thisAdjusted";
                }
                else
                {
                    object[] objArray4 = new object[] { Naming.ForVariable(method.DeclaringType), Naming.ThisParameterName };
                    this._writer.WriteLine("{0}* _thisAdjusted = reinterpret_cast<{0}*>({1} + 1);", objArray4);
                    str3 = "_thisAdjusted";
                }
                List<string> arguments = new List<string> {
                    str3
                };
                for (int i = 0; i < method.Parameters.Count; i++)
                {
                    arguments.Add(Naming.ForParameterName(method.Parameters[i]));
                }
                arguments.Add("method");
                if (method.DeclaringType.IsNullable())
                {
                    if (method.ReturnType.MetadataType != MetadataType.Void)
                    {
                        object[] objArray5 = new object[] { Naming.ForVariable(resolver.Resolve(method.ReturnType)), Emit.Call(Naming.ForMethodNameOnly(method), arguments) };
                        this._writer.WriteLine("{0} _returnValue = {1};", objArray5);
                    }
                    else
                    {
                        this._writer.WriteStatement(Emit.Call(Naming.ForMethodNameOnly(method), arguments));
                    }
                    object[] objArray6 = new object[3];
                    if (<>f__am$cache3 == null)
                    {
                        <>f__am$cache3 = f => f.Name == "value";
                    }
                    objArray6[0] = Naming.ForFieldGetter(method.DeclaringType.Resolve().Fields.Single<FieldDefinition>(<>f__am$cache3));
                    objArray6[1] = Naming.ForVariable(((GenericInstanceType) method.DeclaringType).GenericArguments[0]);
                    objArray6[2] = Naming.ThisParameterName;
                    this._writer.WriteLine("*reinterpret_cast<{1}*>({2} + 1) = _thisAdjusted.{0}();", objArray6);
                    if (method.ReturnType.MetadataType != MetadataType.Void)
                    {
                        this._writer.WriteLine("return _returnValue;");
                    }
                }
                else
                {
                    string str4 = (method.ReturnType.MetadataType == MetadataType.Void) ? string.Empty : "return ";
                    object[] objArray7 = new object[] { str4, Emit.Call(Naming.ForMethodNameOnly(method), arguments) };
                    this._writer.WriteLine("{0}{1};", objArray7);
                }
            }
        }

        private static void WriteExternMethodeDeclarationForInternalPInvokeImpl(CppCodeWriter writer, MethodReference method)
        {
            new PInvokeMethodBodyWriter(method).WriteExternMethodeDeclarationForInternalPInvoke(writer);
        }

        private void WriteLine(string line)
        {
            this._writer.WriteLine(line);
        }

        private void WriteLine(string format, params object[] args)
        {
            this._writer.WriteLine(format, args);
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

        private static void WriteMethodBodyForComObjectConstructor(MethodReference method, CppCodeWriter writer, MethodDefinition methodDefinition)
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
            MethodDefinition methodDefinition = method.Resolve();
            if (methodDefinition.IsConstructor)
            {
                if (methodDefinition.DeclaringType.IsImport && !methodDefinition.DeclaringType.IsWindowsRuntimeProjection())
                {
                    WriteMethodBodyForComObjectConstructor(method, writer, methodDefinition);
                }
                else
                {
                    WriteMethodBodyForWindowsRuntimeObjectConstructor(method, writer, methodDefinition, metadataAccess);
                }
            }
            else if (methodDefinition.IsFinalizerMethod())
            {
                WriteMethodBodyForComOrWindowsRuntimeFinalizer(methodDefinition, writer, metadataAccess);
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
            MethodReference reference = !definition.DeclaringType.IsInterface ? method.GetOverridenInterfaceMethod(method.DeclaringType.GetInterfaces()) : method;
            if (reference == null)
            {
                writer.WriteStatement(Emit.RaiseManagedException($"il2cpp_codegen_get_missing_method_exception("The method '{method.FullName}' has no implementation.")"));
            }
            else if (!reference.DeclaringType.IsComOrWindowsRuntimeInterface())
            {
                writer.WriteStatement(Emit.RaiseManagedException($"il2cpp_codegen_get_not_supported_exception("Cannot call method '{method.FullName}' (overriding '{reference.FullName}'). IL2CPP does not yet support calling projected methods.")"));
            }
            else
            {
                new ComInstanceMethodBodyWriter(method).WriteMethodBody(writer, metadataAccess);
            }
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
                IEnumerable<string> arguments = MethodSignatureWriter.ParametersFor(methodToCall, ParameterFormat.WithName, methodToCall.IsStatic, false, false);
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
                string str2 = !ReturnsVoid(methodToCall) ? "return " : string.Empty;
                string icall = method.FullName.Substring(method.FullName.IndexOf(" ") + 1);
                string str4 = IcallMap.ResolveICallFunction(icall);
                if (str4 != null)
                {
                    writer.WriteLine("using namespace il2cpp::icalls;");
                    object[] objArray3 = new object[] { MethodSignatureWriter.GetICallMethodVariable(methodToCall) };
                    writer.WriteLine("typedef {0};", objArray3);
                    object[] objArray4 = new object[] { str2, Naming.ForMethodNameOnly(method), str4, MethodSignatureWriter.FormatParameters(method, ParameterFormat.WithName, methodToCall.IsStatic, false) };
                    writer.WriteLine("{0} (({1}_ftn){2}) ({3});", objArray4);
                }
                else
                {
                    writer.WriteInternalCallResolutionStatement(methodToCall);
                    object[] objArray5 = new object[] { str2, "_il2cpp_icall_func", MethodSignatureWriter.FormatParameters(method, ParameterFormat.WithName, methodToCall.IsStatic, false) };
                    writer.WriteLine("{0}{1}({2});", objArray5);
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
            else
            {
                if (!definition.IsPInvokeImpl)
                {
                    throw new NotSupportedException();
                }
                WriteMethodBodyForPInvokeImpl(writer, definition, metadataAccess);
            }
        }

        private static void WriteMethodBodyForPInvokeImpl(CppCodeWriter writer, MethodReference method, IRuntimeMetadataAccess metadataAccess)
        {
            new PInvokeMethodBodyWriter(method).WriteMethodBody(writer, metadataAccess);
        }

        private static void WriteMethodBodyForWindowsRuntimeObjectConstructor(MethodReference method, CppCodeWriter writer, MethodDefinition methodDefinition, IRuntimeMetadataAccess metadataAccess)
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

        internal void WriteMethodDeclaration(MethodReference method)
        {
            if (MethodNeedsWritten(method))
            {
                this._writer.WriteCommentedLine(method.FullName);
                if (GenericSharingAnalysis.CanShareMethod(method))
                {
                    bool flag = GenericSharingAnalysis.IsSharedMethod(method);
                    StatsService.ShareableMethods++;
                    MethodReference sharedMethod = GenericSharingAnalysis.GetSharedMethod(method);
                    if (flag)
                    {
                        object[] objArray1 = new object[] { MethodSignatureWriter.GetSharedMethodSignature(this._writer, method) };
                        this._writer.WriteLine("{0};", objArray1);
                    }
                    else if (!method.IsGenericInstance)
                    {
                        this._writer.AddIncludeForMethodDeclarations(sharedMethod.DeclaringType);
                    }
                    string str = MethodSignatureWriter.FormatParameters(method, ParameterFormat.WithName, false, true);
                    object[] args = new object[] { Naming.ForMethodNameOnly(method), str, MethodSignatureWriter.GetMethodPointer(method), Naming.ForMethod(sharedMethod) + "_gshared", str };
                    this.WriteLine("#define {0}({1}) (({2}){3})({4})", args);
                }
                else
                {
                    MethodSignatureWriter.WriteMethodSignature(this._writer, method);
                    this.WriteLine(" IL2CPP_METHOD_ATTR;");
                }
                if (ReversePInvokeMethodBodyWriter.IsReversePInvokeWrapperNecessary(method))
                {
                    WriteReversePInvokeMethodDeclaration(this._writer, method);
                }
            }
        }

        internal void WriteMethodDeclarationsFor(Func<MethodDefinition, bool> filter)
        {
            TypeDefinition definition = this._type.Resolve();
            foreach (MethodDefinition definition2 in definition.Methods.Where<MethodDefinition>(filter))
            {
                MethodReference method = definition2;
                GenericInstanceType genericInstanceType = this._type as GenericInstanceType;
                if (genericInstanceType != null)
                {
                    method = VTableBuilder.CloneMethodReference(genericInstanceType, definition2);
                }
                this.WriteMethodDeclaration(method);
            }
            foreach (MarshalType type2 in MarshalingUtils.GetMarshalTypesForMarshaledType(definition))
            {
                MarshalDataCollector.MarshalInfoWriterFor(this._type, type2, null, false, false, false, null).WriteMarshalFunctionDeclarations(this._writer);
            }
            if (definition.NeedsComCallableWrapper())
            {
                new CCWWriter(definition).WriteCreateCCWDeclaration(this._writer);
            }
        }

        internal void WriteMethodDefinition(MethodReference method, IMethodCollector methodCollector)
        {
            <WriteMethodDefinition>c__AnonStorey0 storey = new <WriteMethodDefinition>c__AnonStorey0 {
                method = method,
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
                this.WriteMethodForDelegatePInvokeIfNeeded(this._writer, storey.method, methodCollector);
                if (storey.method.HasThis && storey.method.DeclaringType.IsValueType())
                {
                    this.WriteAdjustorThunk(storey.method, methodCollector);
                }
                if (ReversePInvokeMethodBodyWriter.IsReversePInvokeWrapperNecessary(storey.method))
                {
                    WriteReversePInvokeMethodDefinition(this._writer, storey.method, methodCollector);
                }
            }
        }

        internal void WriteMethodDefinitions(IMethodCollector methodCollector)
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
                this.WriteMethodDefinition(method, methodCollector);
            }
            foreach (MarshalType type2 in MarshalingUtils.GetMarshalTypesForMarshaledType(definition))
            {
                MarshalDataCollector.MarshalInfoWriterFor(this._type, type2, null, false, false, false, null).WriteMarshalFunctionDefinitions(this._writer, methodCollector);
            }
            if (definition.NeedsComCallableWrapper())
            {
                methodCollector.AddCCWMarshallingFunction(definition);
                new CCWWriter(definition).WriteCreateCCWDefinition(this._writer);
            }
        }

        private void WriteMethodForDelegatePInvokeIfNeeded(CppCodeWriter _writer, MethodReference method, IMethodCollector methodCollector)
        {
            <WriteMethodForDelegatePInvokeIfNeeded>c__AnonStorey1 storey = new <WriteMethodForDelegatePInvokeIfNeeded>c__AnonStorey1 {
                method = method
            };
            storey.delegatePInvokeMethodBodyWriter = new DelegatePInvokeMethodBodyWriter(storey.method);
            if (storey.delegatePInvokeMethodBodyWriter.IsDelegatePInvokeWrapperNecessary())
            {
                Unity.IL2CPP.ILPreProcessor.TypeResolver resolver = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(storey.method.DeclaringType, storey.method);
                string name = Naming.ForDelegatePInvokeWrapper(storey.method.DeclaringType);
                string methodSignature = MethodSignatureWriter.GetMethodSignature(name, Naming.ForVariable(resolver.Resolve(storey.method.ReturnType)), MethodSignatureWriter.FormatParameters(storey.method, ParameterFormat.WithTypeAndName, false, true), "extern \"C\"", "");
                WriteMethodWithMetadataInitialization(_writer, methodSignature, storey.method.FullName, new Action<CppCodeWriter, MetadataUsage, MethodUsage>(storey.<>m__0), name);
                methodCollector.AddWrapperForDelegateFromManagedToNative(storey.method);
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
                writer.WriteStatement("extern const Il2CppType* " + Naming.ForRuntimeIl2CppType(reference));
            }
            foreach (TypeReference reference2 in typeInfos)
            {
                writer.WriteStatement("extern Il2CppClass* " + Naming.ForRuntimeTypeInfo(reference2));
            }
            foreach (MethodReference reference3 in methods)
            {
                writer.WriteStatement("extern const MethodInfo* " + Naming.ForRuntimeMethodInfo(reference3));
            }
            foreach (FieldReference reference4 in fields)
            {
                writer.WriteStatement("extern FieldInfo* " + Naming.ForRuntimeFieldInfo(reference4));
            }
            foreach (string str in stringLiterals)
            {
                writer.WriteStatement("extern Il2CppCodeGenString* " + Naming.ForStringLiteralIdentifier(str));
            }
            writer.WriteStatement("extern const uint32_t " + identifier);
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
                    writer3.Indent(1);
                    writer2.Indent(1);
                    writeMethodBody(writer3, usage, usage2);
                    if (usage.UsesMetadata)
                    {
                        WriteMethodMetadataInitialization(writer2, identifier);
                    }
                    writer3.Dedent(1);
                    writer2.Dedent(1);
                    foreach (MethodReference reference in usage2.GetMethods())
                    {
                        writer.AddIncludeForMethodDeclarations(reference.DeclaringType);
                    }
                    if (usage.UsesMetadata)
                    {
                        WriteMethodMetadataInitializationDeclarations(writer, identifier, usage.GetIl2CppTypes(), usage.GetTypeInfos(), usage.GetInflatedMethods(), usage.GetFieldInfos(), usage.GetStringLiterals());
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
                object[] args = new object[] { metadataAccess.MethodInfo(method) };
                writer.WriteLine("StackTraceSentry _stackTraceSentry({0});", args);
            }
            if (CodeGenOptions.EnableDeepProfiler)
            {
                object[] objArray2 = new object[] { metadataAccess.MethodInfo(method) };
                writer.WriteLine("ProfilerMethodSentry _profilerMethodSentry({0});", objArray2);
            }
        }

        private static void WriteReversePInvokeMethodDeclaration(CppCodeWriter writer, MethodReference method)
        {
            ReversePInvokeMethodBodyWriter.Create(method).WriteMethodDeclaration(writer);
        }

        private static void WriteReversePInvokeMethodDefinition(CppCodeWriter writer, MethodReference method, IMethodCollector methodCollector)
        {
            ReversePInvokeMethodBodyWriter.Create(method).WriteMethodDefinition(writer, methodCollector);
        }

        [CompilerGenerated]
        private sealed class <WriteMethodDefinition>c__AnonStorey0
        {
            internal MethodWriter $this;
            internal MethodReference method;

            internal void <>m__0(CppCodeWriter bodyWriter, MetadataUsage metadataUsage, MethodUsage methodUsage)
            {
                MethodWriter.WritePrologue(this.method, bodyWriter, MethodWriter.GetDefaultRuntimeMetadataAccess(this.method, metadataUsage, methodUsage));
                this.$this.WriteMethodBody(this.method, bodyWriter, MethodWriter.GetDefaultRuntimeMetadataAccess(this.method, metadataUsage, methodUsage));
            }
        }

        [CompilerGenerated]
        private sealed class <WriteMethodForDelegatePInvokeIfNeeded>c__AnonStorey1
        {
            internal DelegatePInvokeMethodBodyWriter delegatePInvokeMethodBodyWriter;
            internal MethodReference method;

            internal void <>m__0(CppCodeWriter bodyWriter, MetadataUsage metadataUsage, MethodUsage methodUsage)
            {
                IRuntimeMetadataAccess metadataAccess = MethodWriter.GetDefaultRuntimeMetadataAccess(this.method, metadataUsage, methodUsage);
                this.delegatePInvokeMethodBodyWriter.WriteMethodBody(bodyWriter, metadataAccess);
            }
        }
    }
}

