namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using Mono.Collections.Generic;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    using Unity.IL2CPP.ILPreProcessor;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;

    public class MethodSignatureWriter
    {
        [Inject]
        public static INamingService Naming;
        [Inject]
        public static ITypeProviderService TypeProvider;

        private static string BuildMethodAttributes(MethodReference method)
        {
            string str = string.Empty;
            if (method.Resolve().NoInlining)
            {
                str = "IL2CPP_NO_INLINE";
            }
            return str;
        }

        public static bool CanDevirtualizeMethodCall(MethodDefinition method) => 
            ((!method.IsVirtual || method.DeclaringType.IsSealed) || method.IsFinal);

        private static string FormatHiddenMethodArgument(ParameterFormat format)
        {
            switch (format)
            {
                case ParameterFormat.WithTypeAndName:
                case ParameterFormat.WithTypeAndNameNoThis:
                case ParameterFormat.WithTypeAndNameThisObject:
                    return "const MethodInfo* method";

                case ParameterFormat.WithType:
                case ParameterFormat.WithTypeNoThis:
                case ParameterFormat.WithTypeThisObject:
                    return "const MethodInfo*";

                case ParameterFormat.WithName:
                case ParameterFormat.WithNameCastThis:
                case ParameterFormat.WithNameUnboxThis:
                    return "method";
            }
            throw new ArgumentOutOfRangeException("format");
        }

        private static string FormatParameterAsVoidPointer(string parameterName) => 
            ("void* " + parameterName);

        private static string FormatParameterName(TypeReference parameterType, string parameterName, ParameterFormat format)
        {
            string str = string.Empty;
            if ((((format == ParameterFormat.WithTypeAndName) || (format == ParameterFormat.WithTypeAndNameNoThis)) || ((format == ParameterFormat.WithType) || (format == ParameterFormat.WithTypeNoThis))) || ((format == ParameterFormat.WithTypeAndNameThisObject) || (format == ParameterFormat.WithTypeThisObject)))
            {
                str = str + Naming.ForVariable(parameterType);
            }
            if (((format == ParameterFormat.WithTypeAndName) || (format == ParameterFormat.WithTypeAndNameNoThis)) || (format == ParameterFormat.WithTypeAndNameThisObject))
            {
                str = str + " ";
            }
            if ((((format != ParameterFormat.WithTypeAndName) && (format != ParameterFormat.WithTypeAndNameNoThis)) && ((format != ParameterFormat.WithName) && (format != ParameterFormat.WithTypeAndNameThisObject))) && (((format != ParameterFormat.WithNameNoThis) && (format != ParameterFormat.WithNameCastThis)) && (format != ParameterFormat.WithNameUnboxThis)))
            {
                return str;
            }
            return (str + parameterName);
        }

        public static string FormatParameters(MethodReference method, ParameterFormat format = 0, bool forceNoStaticThis = false, bool includeHiddenMethodInfo = false)
        {
            List<string> elements = ParametersFor(method, format, forceNoStaticThis, includeHiddenMethodInfo, false).ToList<string>();
            return ((elements.Count != 0) ? elements.AggregateWithComma() : string.Empty);
        }

        private static string FormatThis(ParameterFormat format, TypeReference thisType)
        {
            if (format == ParameterFormat.WithNameCastThis)
            {
                return $"({Naming.ForVariable(thisType)}){Naming.ThisParameterName}";
            }
            if (format == ParameterFormat.WithNameUnboxThis)
            {
                return $"({Naming.ForVariable(thisType)})UnBox({Naming.ThisParameterName})";
            }
            return FormatParameterName(thisType, Naming.ThisParameterName, format);
        }

        public static string GetICallMethodVariable(MethodDefinition method) => 
            $"{Naming.ForVariable(method.ReturnType)} (*{Naming.ForMethodNameOnly(method)}_ftn) ({FormatParameters(method, ParameterFormat.WithType, method.IsStatic, false)})";

        public static string GetMethodPointer(MethodReference method) => 
            GetMethodPointer(method, ParameterFormat.WithType);

        public static string GetMethodPointer(MethodReference method, ParameterFormat parameterFormat)
        {
            Unity.IL2CPP.ILPreProcessor.TypeResolver resolver = new Unity.IL2CPP.ILPreProcessor.TypeResolver(method.DeclaringType as GenericInstanceType, method as GenericInstanceMethod);
            return GetMethodSignature("(*)", Naming.ForVariable(resolver.ResolveReturnType(method)), FormatParameters(method, parameterFormat, false, true), string.Empty, "");
        }

        public static string GetMethodPointerForVTable(MethodReference method)
        {
            ParameterFormat parameterFormat = (!method.DeclaringType.IsValueType || !method.HasThis) ? ParameterFormat.WithType : ParameterFormat.WithTypeThisObject;
            return GetMethodPointer(method, parameterFormat);
        }

        internal static string GetMethodSignature(CppCodeWriter writer, MethodReference method)
        {
            Unity.IL2CPP.ILPreProcessor.TypeResolver typeResolver = new Unity.IL2CPP.ILPreProcessor.TypeResolver(method.DeclaringType as GenericInstanceType, method as GenericInstanceMethod);
            RecordIncludes(writer, method, typeResolver);
            string attributes = BuildMethodAttributes(method);
            return GetMethodSignature(Naming.ForMethodNameOnly(method), Naming.ForVariable(typeResolver.Resolve(Unity.IL2CPP.GenericParameterResolver.ResolveReturnTypeIfNeeded(method))), FormatParameters(method, ParameterFormat.WithTypeAndName, false, true), "extern \"C\"", attributes);
        }

        internal static string GetMethodSignature(string name, string returnType, string parameters, string specifiers = "", string attributes = "") => 
            $"{specifiers} {attributes} {returnType} {name} ({parameters})";

        public static string GetSharedMethodSignature(CppCodeWriter writer, MethodReference method)
        {
            Unity.IL2CPP.ILPreProcessor.TypeResolver typeResolver = new Unity.IL2CPP.ILPreProcessor.TypeResolver(method.DeclaringType as GenericInstanceType, method as GenericInstanceMethod);
            TypeReference variableType = typeResolver.Resolve(Unity.IL2CPP.GenericParameterResolver.ResolveReturnTypeIfNeeded(method));
            RecordIncludes(writer, method, typeResolver);
            string attributes = BuildMethodAttributes(method);
            return GetMethodSignature(Naming.ForMethodNameOnly(method) + "_gshared", Naming.ForVariable(variableType), FormatParameters(method, ParameterFormat.WithTypeAndName, false, true), "extern \"C\"", attributes);
        }

        public static string GetSharedMethodSignatureRaw(MethodReference method)
        {
            TypeReference variableType = new Unity.IL2CPP.ILPreProcessor.TypeResolver(method.DeclaringType as GenericInstanceType, method as GenericInstanceMethod).Resolve(Unity.IL2CPP.GenericParameterResolver.ResolveReturnTypeIfNeeded(method));
            string attributes = BuildMethodAttributes(method);
            return GetMethodSignature(Naming.ForMethodNameOnly(method) + "_gshared", Naming.ForVariable(variableType), FormatParameters(method, ParameterFormat.WithTypeAndName, false, true), "extern \"C\"", attributes);
        }

        public static bool NeedsHiddenMethodInfo(MethodReference method, MethodCallType callType, bool isConstructor)
        {
            if (IntrinsicRemap.ShouldRemap(method))
            {
                return false;
            }
            if (method.DeclaringType.IsArray && ((isConstructor || (method.Name == "Set")) || ((method.Name == "Get") || (method.Name == "Address"))))
            {
                return false;
            }
            if (method.DeclaringType.IsSystemArray() && ((method.Name == "GetGenericValueImpl") || (method.Name == "SetGenericValueImpl")))
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
            if ((callType == MethodCallType.Virtual) && !CanDevirtualizeMethodCall(method.Resolve()))
            {
                return false;
            }
            return true;
        }

        private static bool NeedsUnusedThisParameterForStaticMethod(MethodReference methodDefinition) => 
            methodDefinition.Resolve().IsStatic;

        [DebuggerHidden]
        public static IEnumerable<string> ParametersFor(MethodReference methodDefinition, ParameterFormat format = 0, bool forceNoStaticThis = false, bool includeHiddenMethodInfo = false, bool useVoidPointerForThis = false) => 
            new <ParametersFor>c__Iterator0 { 
                methodDefinition = methodDefinition,
                forceNoStaticThis = forceNoStaticThis,
                format = format,
                useVoidPointerForThis = useVoidPointerForThis,
                includeHiddenMethodInfo = includeHiddenMethodInfo,
                $PC = -2
            };

        private static string ParameterStringFor(MethodReference methodDefinition, ParameterFormat format, ParameterDefinition parameterDefinition) => 
            FormatParameterName(Unity.IL2CPP.ILPreProcessor.TypeResolver.For(methodDefinition.DeclaringType).Resolve(Unity.IL2CPP.GenericParameterResolver.ResolveParameterTypeIfNeeded(methodDefinition, parameterDefinition)), Naming.ForParameterName(parameterDefinition), format);

        private static void RecordIncludes(CppCodeWriter writer, MethodReference method, Unity.IL2CPP.ILPreProcessor.TypeResolver typeResolver)
        {
            if (method.HasThis)
            {
                writer.AddIncludesForTypeReference(!method.DeclaringType.IsComOrWindowsRuntimeInterface() ? method.DeclaringType : TypeProvider.SystemObject, false);
            }
            if (method.ReturnType.MetadataType != MetadataType.Void)
            {
                writer.AddIncludesForTypeReference(typeResolver.ResolveReturnType(method), false);
            }
            foreach (ParameterDefinition definition in method.Parameters)
            {
                writer.AddIncludesForTypeReference(typeResolver.ResolveParameterType(method, definition), true);
            }
        }

        public static void WriteMethodSignature(CppCodeWriter writer, MethodReference method)
        {
            writer.Write(GetMethodSignature(writer, method));
        }

        [CompilerGenerated]
        private sealed class <ParametersFor>c__Iterator0 : IEnumerable, IEnumerable<string>, IEnumerator, IDisposable, IEnumerator<string>
        {
            internal string $current;
            internal bool $disposing;
            internal Collection<ParameterDefinition>.Enumerator $locvar0;
            internal int $PC;
            internal ParameterDefinition <parameterDefinition>__1;
            internal TypeReference <thisType>__0;
            internal bool forceNoStaticThis;
            internal ParameterFormat format;
            internal bool includeHiddenMethodInfo;
            internal MethodReference methodDefinition;
            internal bool useVoidPointerForThis;

            [DebuggerHidden]
            public void Dispose()
            {
                uint num = (uint) this.$PC;
                this.$disposing = true;
                this.$PC = -1;
                switch (num)
                {
                    case 6:
                        try
                        {
                        }
                        finally
                        {
                            this.$locvar0.Dispose();
                        }
                        break;
                }
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                bool flag = false;
                switch (num)
                {
                    case 0:
                        if (!MethodSignatureWriter.NeedsUnusedThisParameterForStaticMethod(this.methodDefinition) || this.forceNoStaticThis)
                        {
                            break;
                        }
                        this.$current = $"{MethodSignatureWriter.FormatThis(this.format, this.methodDefinition.Module.TypeSystem.Object)} {Formatter.Comment("static, unused")}";
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        goto Label_0321;

                    case 1:
                        break;

                    case 2:
                    case 3:
                    case 4:
                    case 5:
                        goto Label_024E;

                    case 6:
                        goto Label_0268;

                    case 7:
                        goto Label_0318;

                    default:
                        goto Label_031F;
                }
                if (this.format == ParameterFormat.WithTypeAndNameThisObject)
                {
                    if (this.useVoidPointerForThis)
                    {
                        this.$current = MethodSignatureWriter.FormatParameterAsVoidPointer(MethodSignatureWriter.Naming.ThisParameterName);
                        if (!this.$disposing)
                        {
                            this.$PC = 2;
                        }
                    }
                    else
                    {
                        this.$current = MethodSignatureWriter.FormatParameterName(this.methodDefinition.Module.TypeSystem.Object, MethodSignatureWriter.Naming.ThisParameterName, this.format);
                        if (!this.$disposing)
                        {
                            this.$PC = 3;
                        }
                    }
                    goto Label_0321;
                }
                if (this.format == ParameterFormat.WithTypeThisObject)
                {
                    this.$current = !this.useVoidPointerForThis ? MethodSignatureWriter.Naming.ForVariable(MethodSignatureWriter.TypeProvider.ObjectTypeReference) : "void*";
                    if (!this.$disposing)
                    {
                        this.$PC = 4;
                    }
                    goto Label_0321;
                }
                if (((this.format != ParameterFormat.WithNameNoThis) && (this.format != ParameterFormat.WithTypeNoThis)) && ((this.format != ParameterFormat.WithTypeAndNameNoThis) && this.methodDefinition.HasThis))
                {
                    this.<thisType>__0 = this.methodDefinition.DeclaringType;
                    if (this.<thisType>__0.IsValueType())
                    {
                        this.<thisType>__0 = new PointerType(this.<thisType>__0);
                    }
                    else if (this.<thisType>__0.IsSpecialSystemBaseType())
                    {
                        this.<thisType>__0 = this.methodDefinition.Module.TypeSystem.Object;
                    }
                    this.$current = MethodSignatureWriter.FormatThis(this.format, this.<thisType>__0);
                    if (!this.$disposing)
                    {
                        this.$PC = 5;
                    }
                    goto Label_0321;
                }
            Label_024E:
                this.$locvar0 = this.methodDefinition.Parameters.GetEnumerator();
                num = 0xfffffffd;
            Label_0268:
                try
                {
                    while (this.$locvar0.MoveNext())
                    {
                        this.<parameterDefinition>__1 = this.$locvar0.Current;
                        this.$current = MethodSignatureWriter.ParameterStringFor(this.methodDefinition, this.format, this.<parameterDefinition>__1);
                        if (!this.$disposing)
                        {
                            this.$PC = 6;
                        }
                        flag = true;
                        goto Label_0321;
                    }
                }
                finally
                {
                    if (!flag)
                    {
                    }
                    this.$locvar0.Dispose();
                }
                if (this.includeHiddenMethodInfo)
                {
                    this.$current = MethodSignatureWriter.FormatHiddenMethodArgument(this.format);
                    if (!this.$disposing)
                    {
                        this.$PC = 7;
                    }
                    goto Label_0321;
                }
            Label_0318:
                this.$PC = -1;
            Label_031F:
                return false;
            Label_0321:
                return true;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<string> IEnumerable<string>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new MethodSignatureWriter.<ParametersFor>c__Iterator0 { 
                    methodDefinition = this.methodDefinition,
                    forceNoStaticThis = this.forceNoStaticThis,
                    format = this.format,
                    useVoidPointerForThis = this.useVoidPointerForThis,
                    includeHiddenMethodInfo = this.includeHiddenMethodInfo
                };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<string>.GetEnumerator();

            string IEnumerator<string>.Current =>
                this.$current;

            object IEnumerator.Current =>
                this.$current;
        }
    }
}

