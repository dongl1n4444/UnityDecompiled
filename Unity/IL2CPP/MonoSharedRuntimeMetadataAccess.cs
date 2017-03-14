namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP.GenericSharing;
    using Unity.IL2CPP.ILPreProcessor;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;

    public class MonoSharedRuntimeMetadataAccess : IRuntimeMetadataAccess
    {
        private readonly SharedRuntimeMetadataAccess _defaultSharedRuntimeMetadataAccess;
        private readonly MethodReference _enclosingMethod;
        private readonly Unity.IL2CPP.ILPreProcessor.TypeResolver _typeResolver;
        [CompilerGenerated]
        private static Func<int, string> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<int, string> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<int, string> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<int, string> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<int, string> <>f__am$cache4;
        [CompilerGenerated]
        private static Func<int, string> <>f__am$cache5;
        [CompilerGenerated]
        private static Func<int, string> <>f__am$cache6;
        [CompilerGenerated]
        private static Func<int, string> <>f__am$cache7;
        [CompilerGenerated]
        private static Func<int, string> <>f__am$cache8;
        [CompilerGenerated]
        private static Func<int, string> <>f__am$cache9;
        [CompilerGenerated]
        private static Func<int, string> <>f__am$cacheA;
        [CompilerGenerated]
        private static Func<int, string> <>f__am$cacheB;
        [CompilerGenerated]
        private static Func<int, string> <>f__am$cacheC;
        [CompilerGenerated]
        private static Func<int, string> <>f__am$cacheD;
        [CompilerGenerated]
        private static Func<int, string> <>f__am$cacheE;
        [CompilerGenerated]
        private static Func<int, string> <>f__am$cacheF;
        [Inject]
        public static IGenericSharingAnalysisService GenericSharingAnalysis;
        [Inject]
        public static INamingService Naming;

        public MonoSharedRuntimeMetadataAccess(MethodReference enclosingMethod, SharedRuntimeMetadataAccess defaultSharedRuntimeMetadataAccess)
        {
            this._enclosingMethod = enclosingMethod;
            this._defaultSharedRuntimeMetadataAccess = defaultSharedRuntimeMetadataAccess;
            if (enclosingMethod != null)
            {
                this._typeResolver = new Unity.IL2CPP.ILPreProcessor.TypeResolver(enclosingMethod.DeclaringType as GenericInstanceType, enclosingMethod as GenericInstanceMethod);
            }
            else
            {
                this._typeResolver = Unity.IL2CPP.ILPreProcessor.TypeResolver.Empty;
            }
        }

        public string ArrayInfo(TypeReference elementType)
        {
            <ArrayInfo>c__AnonStorey3 storey = new <ArrayInfo>c__AnonStorey3 {
                elementType = elementType,
                $this = this
            };
            if (<>f__am$cache6 == null)
            {
                <>f__am$cache6 = index => $"(MonoClass*)il2cpp_codegen_mono_class_rgctx(mono_unity_method_get_class((MonoMethod*)method), IL2CPP_RGCTX_DATA_ARRAY, {index}, false)";
            }
            if (<>f__am$cache7 == null)
            {
                <>f__am$cache7 = index => $"(MonoClass*)il2cpp_codegen_mono_method_rgctx((MonoMethod*)method, IL2CPP_RGCTX_DATA_ARRAY, {index}, false)";
            }
            return SharedRuntimeMetadataAccess.RetrieveType(storey.elementType, this._enclosingMethod, new Func<string>(storey.<>m__0), <>f__am$cache6, <>f__am$cache7, RuntimeGenericContextInfo.Array);
        }

        public string FieldInfo(FieldReference field)
        {
            if (SharedRuntimeMetadataAccess.GetRGCTXAccess(field.DeclaringType, this._enclosingMethod) == RuntimeGenericAccess.None)
            {
                return this._defaultSharedRuntimeMetadataAccess.FieldInfo(field);
            }
            string str2 = this.TypeInfoFor(field.DeclaringType);
            return $"il2cpp_codegen_mono_class_get_field({str2}, {MetadataTokenUtils.FormattedMetadataTokenFor(field)})";
        }

        public string HiddenMethodInfo(MethodReference method)
        {
            <HiddenMethodInfo>c__AnonStorey8 storey = new <HiddenMethodInfo>c__AnonStorey8 {
                method = method,
                $this = this
            };
            if (<>f__am$cacheE == null)
            {
                <>f__am$cacheE = index => $"(const MethodInfo*)il2cpp_codegen_mono_class_rgctx(mono_unity_method_get_class((MonoMethod*)method), IL2CPP_RGCTX_DATA_METHOD, {index}, false)";
            }
            if (<>f__am$cacheF == null)
            {
                <>f__am$cacheF = index => $"(const MethodInfo*)il2cpp_codegen_mono_method_rgctx((MonoMethod*)method, IL2CPP_RGCTX_DATA_METHOD, {index}, false)";
            }
            return SharedRuntimeMetadataAccess.RetrieveMethod<string>(storey.method, this._enclosingMethod, new Func<string>(storey.<>m__0), <>f__am$cacheE, <>f__am$cacheF, RuntimeGenericContextInfo.Method);
        }

        public string Il2CppTypeFor(TypeReference type)
        {
            <Il2CppTypeFor>c__AnonStorey5 storey = new <Il2CppTypeFor>c__AnonStorey5 {
                type = type,
                $this = this
            };
            if (<>f__am$cacheA == null)
            {
                <>f__am$cacheA = index => $"(MonoType*)il2cpp_codegen_mono_class_rgctx(mono_unity_method_get_class((MonoMethod*)method), IL2CPP_RGCTX_DATA_TYPE, {index}, false)";
            }
            if (<>f__am$cacheB == null)
            {
                <>f__am$cacheB = index => $"(MonoType*)il2cpp_codegen_mono_method_rgctx((MonoMethod*)method, IL2CPP_RGCTX_DATA_TYPE, {index}, false)";
            }
            return SharedRuntimeMetadataAccess.RetrieveType(storey.type, this._enclosingMethod, new Func<string>(storey.<>m__0), <>f__am$cacheA, <>f__am$cacheB, RuntimeGenericContextInfo.Type);
        }

        public string Method(MethodReference method)
        {
            <Method>c__AnonStorey6 storey = new <Method>c__AnonStorey6 {
                method = method,
                $this = this
            };
            storey.methodReference = this._typeResolver.Resolve(storey.method);
            return SharedRuntimeMetadataAccess.RetrieveMethod<string>(storey.method, this._enclosingMethod, new Func<string>(storey.<>m__0), new Func<int, string>(storey.<>m__1), new Func<int, string>(storey.<>m__2), RuntimeGenericContextInfo.Method);
        }

        public string MethodInfo(MethodReference method)
        {
            <MethodInfo>c__AnonStorey7 storey = new <MethodInfo>c__AnonStorey7 {
                method = method,
                $this = this
            };
            if (<>f__am$cacheC == null)
            {
                <>f__am$cacheC = index => $"(MethodInfo*)il2cpp_codegen_mono_class_rgctx(mono_unity_method_get_class((MonoMethod*)method), IL2CPP_RGCTX_DATA_METHOD, {index}, false)";
            }
            if (<>f__am$cacheD == null)
            {
                <>f__am$cacheD = index => $"(MethodInfo*)il2cpp_codegen_mono_method_rgctx((MonoMethod*)method, IL2CPP_RGCTX_DATA_METHOD, {index}, false)";
            }
            return SharedRuntimeMetadataAccess.RetrieveMethod<string>(storey.method, this._enclosingMethod, new Func<string>(storey.<>m__0), <>f__am$cacheC, <>f__am$cacheD, RuntimeGenericContextInfo.Method);
        }

        public bool NeedsBoxingForValueTypeThis(MethodReference method) => 
            this._defaultSharedRuntimeMetadataAccess.NeedsBoxingForValueTypeThis(method);

        public string Newobj(MethodReference ctor)
        {
            <Newobj>c__AnonStorey4 storey = new <Newobj>c__AnonStorey4 {
                ctor = ctor,
                $this = this
            };
            if (<>f__am$cache8 == null)
            {
                <>f__am$cache8 = index => $"(MonoClass*)il2cpp_codegen_mono_class_rgctx(mono_unity_method_get_class((MonoMethod*)method), IL2CPP_RGCTX_DATA_CLASS, {index}, false)";
            }
            if (<>f__am$cache9 == null)
            {
                <>f__am$cache9 = index => $"(MonoClass*)il2cpp_codegen_mono_method_rgctx((MonoMethod*)method, IL2CPP_RGCTX_DATA_CLASS, {index}, false)";
            }
            return SharedRuntimeMetadataAccess.RetrieveType(storey.ctor.DeclaringType, this._enclosingMethod, new Func<string>(storey.<>m__0), <>f__am$cache8, <>f__am$cache9, RuntimeGenericContextInfo.Class);
        }

        public string SizeOf(TypeReference type)
        {
            <SizeOf>c__AnonStorey2 storey = new <SizeOf>c__AnonStorey2 {
                type = type,
                $this = this
            };
            if (<>f__am$cache4 == null)
            {
                <>f__am$cache4 = index => Emit.Call("il2cpp_codegen_sizeof", $"(MonoClass*)il2cpp_codegen_mono_class_rgctx(mono_unity_method_get_class((MonoMethod*)method), IL2CPP_RGCTX_DATA_CLASS, {index}, false)");
            }
            if (<>f__am$cache5 == null)
            {
                <>f__am$cache5 = index => Emit.Call("il2cpp_codegen_sizeof", $"(MonoClass*)il2cpp_codegen_mono_method_rgctx((MonoMethod*)method, IL2CPP_RGCTX_DATA_CLASS, {index}, false)");
            }
            return SharedRuntimeMetadataAccess.RetrieveType(storey.type, this._enclosingMethod, new Func<string>(storey.<>m__0), <>f__am$cache4, <>f__am$cache5, RuntimeGenericContextInfo.Class);
        }

        public string StaticData(TypeReference type)
        {
            <StaticData>c__AnonStorey0 storey = new <StaticData>c__AnonStorey0 {
                type = type,
                $this = this
            };
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = index => $"(MonoClass*)il2cpp_codegen_mono_class_rgctx(mono_unity_method_get_class((MonoMethod*)method), IL2CPP_RGCTX_DATA_CLASS, {index}, false)";
            }
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = index => $"(MonoClass*)il2cpp_codegen_mono_method_rgctx((MonoMethod*)method, IL2CPP_RGCTX_DATA_CLASS, {index}, false)";
            }
            return SharedRuntimeMetadataAccess.RetrieveType(storey.type, this._enclosingMethod, new Func<string>(storey.<>m__0), <>f__am$cache0, <>f__am$cache1, RuntimeGenericContextInfo.Static);
        }

        public string StringLiteral(string literal, MetadataToken token, AssemblyDefinition assemblyDefinition) => 
            this._defaultSharedRuntimeMetadataAccess.StringLiteral(literal, token, assemblyDefinition);

        public string TypeInfoFor(TypeReference type)
        {
            <TypeInfoFor>c__AnonStorey1 storey = new <TypeInfoFor>c__AnonStorey1 {
                type = type,
                $this = this
            };
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = index => $"(MonoClass*)il2cpp_codegen_mono_class_rgctx(mono_unity_method_get_class((MonoMethod*)method), IL2CPP_RGCTX_DATA_CLASS, {index}, false)";
            }
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = index => $"(MonoClass*)il2cpp_codegen_mono_method_rgctx((MonoMethod*)method, IL2CPP_RGCTX_DATA_CLASS, {index}, false)";
            }
            return SharedRuntimeMetadataAccess.RetrieveType(storey.type, this._enclosingMethod, new Func<string>(storey.<>m__0), <>f__am$cache2, <>f__am$cache3, RuntimeGenericContextInfo.Class);
        }

        public string UnresolvedTypeInfoFor(TypeReference type) => 
            this._defaultSharedRuntimeMetadataAccess.UnresolvedTypeInfoFor(type);

        [CompilerGenerated]
        private sealed class <ArrayInfo>c__AnonStorey3
        {
            internal MonoSharedRuntimeMetadataAccess $this;
            internal TypeReference elementType;

            internal string <>m__0() => 
                this.$this._defaultSharedRuntimeMetadataAccess.ArrayInfo(this.elementType);
        }

        [CompilerGenerated]
        private sealed class <HiddenMethodInfo>c__AnonStorey8
        {
            internal MonoSharedRuntimeMetadataAccess $this;
            internal MethodReference method;

            internal string <>m__0() => 
                ("(const MethodInfo *)" + this.$this._defaultSharedRuntimeMetadataAccess.HiddenMethodInfo(this.method));
        }

        [CompilerGenerated]
        private sealed class <Il2CppTypeFor>c__AnonStorey5
        {
            internal MonoSharedRuntimeMetadataAccess $this;
            internal TypeReference type;

            internal string <>m__0() => 
                this.$this._defaultSharedRuntimeMetadataAccess.Il2CppTypeFor(this.type);
        }

        [CompilerGenerated]
        private sealed class <Method>c__AnonStorey6
        {
            internal MonoSharedRuntimeMetadataAccess $this;
            internal MethodReference method;
            internal MethodReference methodReference;

            internal string <>m__0() => 
                this.$this._defaultSharedRuntimeMetadataAccess.Method(this.method);

            internal string <>m__1(int index) => 
                ("(" + Emit.Cast(MethodSignatureWriter.GetMethodPointerForVTable(this.methodReference), $"il2cpp_codegen_get_method_pointer((MethodInfo*)il2cpp_codegen_mono_class_rgctx(mono_unity_method_get_class((MonoMethod*)method), IL2CPP_RGCTX_DATA_METHOD, {index}, true))") + ")");

            internal string <>m__2(int index) => 
                ("(" + Emit.Cast(MethodSignatureWriter.GetMethodPointerForVTable(this.methodReference), $"il2cpp_codegen_get_method_pointer((MethodInfo*)il2cpp_codegen_mono_method_rgctx((MonoMethod*)method, IL2CPP_RGCTX_DATA_METHOD, {index}, true))") + ")");
        }

        [CompilerGenerated]
        private sealed class <MethodInfo>c__AnonStorey7
        {
            internal MonoSharedRuntimeMetadataAccess $this;
            internal MethodReference method;

            internal string <>m__0() => 
                ("(MethodInfo*)" + this.$this._defaultSharedRuntimeMetadataAccess.MethodInfo(this.method));
        }

        [CompilerGenerated]
        private sealed class <Newobj>c__AnonStorey4
        {
            internal MonoSharedRuntimeMetadataAccess $this;
            internal MethodReference ctor;

            internal string <>m__0() => 
                this.$this._defaultSharedRuntimeMetadataAccess.Newobj(this.ctor);
        }

        [CompilerGenerated]
        private sealed class <SizeOf>c__AnonStorey2
        {
            internal MonoSharedRuntimeMetadataAccess $this;
            internal TypeReference type;

            internal string <>m__0() => 
                this.$this._defaultSharedRuntimeMetadataAccess.SizeOf(this.type);
        }

        [CompilerGenerated]
        private sealed class <StaticData>c__AnonStorey0
        {
            internal MonoSharedRuntimeMetadataAccess $this;
            internal TypeReference type;

            internal string <>m__0() => 
                this.$this._defaultSharedRuntimeMetadataAccess.StaticData(this.type);
        }

        [CompilerGenerated]
        private sealed class <TypeInfoFor>c__AnonStorey1
        {
            internal MonoSharedRuntimeMetadataAccess $this;
            internal TypeReference type;

            internal string <>m__0() => 
                this.$this._defaultSharedRuntimeMetadataAccess.TypeInfoFor(this.type);
        }
    }
}

