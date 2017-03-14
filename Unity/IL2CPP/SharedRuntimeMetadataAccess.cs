namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP.Common;
    using Unity.IL2CPP.GenericSharing;
    using Unity.IL2CPP.ILPreProcessor;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;

    public sealed class SharedRuntimeMetadataAccess : IRuntimeMetadataAccess
    {
        private readonly DefaultRuntimeMetadataAccess _default;
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
        private static Func<bool> <>f__am$cache7;
        [CompilerGenerated]
        private static Func<int, bool> <>f__am$cache8;
        [CompilerGenerated]
        private static Func<int, bool> <>f__am$cache9;
        [CompilerGenerated]
        private static Func<int, string> <>f__am$cacheA;
        [CompilerGenerated]
        private static Func<int, string> <>f__am$cacheB;
        [Inject]
        public static IGenericSharingAnalysisService GenericSharingAnalysis;
        [Inject]
        public static INamingService Naming;

        public SharedRuntimeMetadataAccess(MethodReference enclosingMethod, DefaultRuntimeMetadataAccess defaultRuntimeMetadataAccess)
        {
            this._enclosingMethod = enclosingMethod;
            this._typeResolver = new Unity.IL2CPP.ILPreProcessor.TypeResolver(enclosingMethod.DeclaringType as GenericInstanceType, enclosingMethod as GenericInstanceMethod);
            this._default = defaultRuntimeMetadataAccess;
        }

        public string ArrayInfo(TypeReference elementType)
        {
            <ArrayInfo>c__AnonStorey5 storey = new <ArrayInfo>c__AnonStorey5 {
                elementType = elementType,
                $this = this
            };
            if (<>f__am$cache5 == null)
            {
                <>f__am$cache5 = index => Emit.Call("IL2CPP_RGCTX_DATA", "method->rgctx_data", index.ToString(CultureInfo.InvariantCulture));
            }
            return RetrieveType(storey.elementType, this._enclosingMethod, new Func<string>(storey.<>m__0), new Func<int, string>(storey.<>m__1), <>f__am$cache5, RuntimeGenericContextInfo.Array);
        }

        public string FieldInfo(FieldReference field)
        {
            if (GetRGCTXAccess(field.DeclaringType, this._enclosingMethod) == RuntimeGenericAccess.None)
            {
                return this._default.FieldInfo(field);
            }
            string str2 = this.TypeInfoFor(field.DeclaringType);
            return $"IL2CPP_RGCTX_FIELD_INFO({str2},{Naming.GetFieldIndex(field, false)})";
        }

        private static string FormatGenericContextErrorMessage(string name) => 
            $"Unable to retrieve the runtime generic context for '{name}'.";

        public static RuntimeGenericAccess GetRGCTXAccess(MethodReference method, MethodReference enclosingMethod)
        {
            switch (GenericSharingVisitor.GenericUsageFor(method))
            {
                case GenericContextUsage.None:
                    return RuntimeGenericAccess.None;

                case GenericContextUsage.Type:
                    if (!GenericSharingAnalysis.NeedsTypeContextAsArgument(enclosingMethod))
                    {
                        return RuntimeGenericAccess.This;
                    }
                    return RuntimeGenericAccess.Type;

                case GenericContextUsage.Method:
                case GenericContextUsage.Both:
                    return RuntimeGenericAccess.Method;
            }
            throw new ArgumentOutOfRangeException("method");
        }

        public static RuntimeGenericAccess GetRGCTXAccess(TypeReference type, MethodReference enclosingMethod)
        {
            switch (GenericSharingVisitor.GenericUsageFor(type))
            {
                case GenericContextUsage.None:
                    return RuntimeGenericAccess.None;

                case GenericContextUsage.Type:
                    if (!GenericSharingAnalysis.NeedsTypeContextAsArgument(enclosingMethod))
                    {
                        return RuntimeGenericAccess.This;
                    }
                    return RuntimeGenericAccess.Type;

                case GenericContextUsage.Method:
                case GenericContextUsage.Both:
                    return RuntimeGenericAccess.Method;
            }
            throw new ArgumentOutOfRangeException("type");
        }

        private string GetTypeRgctxDataExpression()
        {
            string argument = "method->declaring_type";
            if (!this._enclosingMethod.HasThis || this._enclosingMethod.DeclaringType.IsValueType())
            {
                argument = Naming.ForInitializedTypeInfo(argument);
            }
            return $"{argument}->rgctx_data";
        }

        public string HiddenMethodInfo(MethodReference method)
        {
            <HiddenMethodInfo>c__AnonStorey9 storey = new <HiddenMethodInfo>c__AnonStorey9 {
                method = method,
                $this = this
            };
            if (<>f__am$cacheB == null)
            {
                <>f__am$cacheB = index => Emit.Call("IL2CPP_RGCTX_METHOD_INFO", "method->rgctx_data", index.ToString(CultureInfo.InvariantCulture));
            }
            return RetrieveMethod<string>(storey.method, this._enclosingMethod, new Func<string>(storey.<>m__0), new Func<int, string>(storey.<>m__1), <>f__am$cacheB, RuntimeGenericContextInfo.Method);
        }

        public string Il2CppTypeFor(TypeReference type)
        {
            <Il2CppTypeFor>c__AnonStorey4 storey = new <Il2CppTypeFor>c__AnonStorey4 {
                type = type,
                $this = this
            };
            if (<>f__am$cache4 == null)
            {
                <>f__am$cache4 = index => Emit.Call("IL2CPP_RGCTX_TYPE", "method->rgctx_data", index.ToString(CultureInfo.InvariantCulture));
            }
            return RetrieveType(storey.type, this._enclosingMethod, new Func<string>(storey.<>m__0), new Func<int, string>(storey.<>m__1), <>f__am$cache4, RuntimeGenericContextInfo.Type);
        }

        public string Method(MethodReference method)
        {
            <Method>c__AnonStorey7 storey = new <Method>c__AnonStorey7 {
                method = method,
                $this = this
            };
            storey.methodReference = this._typeResolver.Resolve(storey.method);
            return RetrieveMethod<string>(storey.method, this._enclosingMethod, new Func<string>(storey.<>m__0), new Func<int, string>(storey.<>m__1), new Func<int, string>(storey.<>m__2), RuntimeGenericContextInfo.Method);
        }

        public string MethodInfo(MethodReference method)
        {
            <MethodInfo>c__AnonStorey8 storey = new <MethodInfo>c__AnonStorey8 {
                method = method,
                $this = this
            };
            if (<>f__am$cacheA == null)
            {
                <>f__am$cacheA = index => Emit.Call("IL2CPP_RGCTX_METHOD_INFO", "method->rgctx_data", index.ToString(CultureInfo.InvariantCulture));
            }
            return RetrieveMethod<string>(storey.method, this._enclosingMethod, new Func<string>(storey.<>m__0), new Func<int, string>(storey.<>m__1), <>f__am$cacheA, RuntimeGenericContextInfo.Method);
        }

        public bool NeedsBoxingForValueTypeThis(MethodReference method)
        {
            if (<>f__am$cache7 == null)
            {
                <>f__am$cache7 = () => false;
            }
            if (<>f__am$cache8 == null)
            {
                <>f__am$cache8 = index => true;
            }
            if (<>f__am$cache9 == null)
            {
                <>f__am$cache9 = index => true;
            }
            return RetrieveMethod<bool>(method, this._enclosingMethod, <>f__am$cache7, <>f__am$cache8, <>f__am$cache9, RuntimeGenericContextInfo.Method);
        }

        public string Newobj(MethodReference ctor)
        {
            <Newobj>c__AnonStorey6 storey = new <Newobj>c__AnonStorey6 {
                ctor = ctor,
                $this = this
            };
            if (<>f__am$cache6 == null)
            {
                <>f__am$cache6 = index => Emit.Call("IL2CPP_RGCTX_DATA", "method->rgctx_data", index.ToString(CultureInfo.InvariantCulture));
            }
            return RetrieveType(storey.ctor.DeclaringType, this._enclosingMethod, new Func<string>(storey.<>m__0), new Func<int, string>(storey.<>m__1), <>f__am$cache6, RuntimeGenericContextInfo.Class);
        }

        public static T RetrieveMethod<T>(MethodReference method, MethodReference enclosingMethod, Func<T> defaultFunc, Func<int, T> retrieveTypeSharedAccess, Func<int, T> retrieveMethodSharedAccess, RuntimeGenericContextInfo info)
        {
            RuntimeGenericAccess rGCTXAccess = GetRGCTXAccess(method, enclosingMethod);
            switch (rGCTXAccess)
            {
                case RuntimeGenericAccess.None:
                    return defaultFunc();

                case RuntimeGenericAccess.Method:
                {
                    GenericSharingData data = GenericSharingAnalysis.RuntimeGenericContextFor(enclosingMethod.Resolve());
                    int num = RetrieveMethodIndex(method, info, data);
                    if (num == -1)
                    {
                        throw new InvalidOperationException(FormatGenericContextErrorMessage(method.FullName));
                    }
                    return retrieveMethodSharedAccess(num);
                }
            }
            if ((rGCTXAccess != RuntimeGenericAccess.This) && (rGCTXAccess != RuntimeGenericAccess.Type))
            {
                throw new ArgumentOutOfRangeException("method");
            }
            GenericSharingData rgctx = GenericSharingAnalysis.RuntimeGenericContextFor(enclosingMethod.DeclaringType.Resolve());
            int arg = RetrieveMethodIndex(method, info, rgctx);
            if (arg == -1)
            {
                throw new InvalidOperationException(FormatGenericContextErrorMessage(method.FullName));
            }
            return retrieveTypeSharedAccess(arg);
        }

        public static int RetrieveMethodIndex(MethodReference method, RuntimeGenericContextInfo info, GenericSharingData rgctx)
        {
            for (int i = 0; i < rgctx.RuntimeGenericDatas.Count; i++)
            {
                RuntimeGenericData data = rgctx.RuntimeGenericDatas[i];
                if (data.InfoType == info)
                {
                    RuntimeGenericMethodData data2 = (RuntimeGenericMethodData) data;
                    if ((data2.GenericMethod != null) && new Unity.IL2CPP.Common.MethodReferenceComparer().Equals(data2.GenericMethod, method))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public static string RetrieveType(TypeReference type, MethodReference enclosingMethod, Func<string> defaultFunc, Func<int, string> retrieveTypeSharedAccess, Func<int, string> retrieveMethodSharedAccess, RuntimeGenericContextInfo info)
        {
            RuntimeGenericAccess rGCTXAccess = GetRGCTXAccess(type, enclosingMethod);
            switch (rGCTXAccess)
            {
                case RuntimeGenericAccess.None:
                    return defaultFunc();

                case RuntimeGenericAccess.Method:
                {
                    GenericSharingData data = GenericSharingAnalysis.RuntimeGenericContextFor(enclosingMethod.Resolve());
                    int num = RetrieveTypeIndex(type, info, data);
                    if (num == -1)
                    {
                        throw new InvalidOperationException(FormatGenericContextErrorMessage(type.FullName));
                    }
                    return retrieveMethodSharedAccess(num);
                }
            }
            if ((rGCTXAccess != RuntimeGenericAccess.This) && (rGCTXAccess != RuntimeGenericAccess.Type))
            {
                throw new ArgumentOutOfRangeException("type");
            }
            GenericSharingData rgctx = GenericSharingAnalysis.RuntimeGenericContextFor(enclosingMethod.DeclaringType.Resolve());
            int arg = RetrieveTypeIndex(type, info, rgctx);
            if (arg == -1)
            {
                throw new InvalidOperationException(FormatGenericContextErrorMessage(type.FullName));
            }
            return retrieveTypeSharedAccess(arg);
        }

        public static int RetrieveTypeIndex(TypeReference type, RuntimeGenericContextInfo info, GenericSharingData rgctx)
        {
            for (int i = 0; i < rgctx.RuntimeGenericDatas.Count; i++)
            {
                RuntimeGenericData data = rgctx.RuntimeGenericDatas[i];
                if (data.InfoType == info)
                {
                    RuntimeGenericTypeData data2 = (RuntimeGenericTypeData) data;
                    if ((data2.GenericType != null) && Unity.IL2CPP.Common.TypeReferenceEqualityComparer.AreEqual(data2.GenericType, type, TypeComparisonMode.Exact))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public string SizeOf(TypeReference type)
        {
            <SizeOf>c__AnonStorey3 storey = new <SizeOf>c__AnonStorey3 {
                type = type,
                $this = this
            };
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = index => Emit.Call("IL2CPP_RGCTX_SIZEOF", "method->rgctx_data", index.ToString(CultureInfo.InvariantCulture));
            }
            return RetrieveType(storey.type, this._enclosingMethod, new Func<string>(storey.<>m__0), new Func<int, string>(storey.<>m__1), <>f__am$cache3, RuntimeGenericContextInfo.Class);
        }

        public string StaticData(TypeReference type)
        {
            <StaticData>c__AnonStorey0 storey = new <StaticData>c__AnonStorey0 {
                type = type,
                $this = this
            };
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = index => Emit.Call("IL2CPP_RGCTX_DATA", "method->rgctx_data", index.ToString(CultureInfo.InvariantCulture));
            }
            return RetrieveType(storey.type, this._enclosingMethod, new Func<string>(storey.<>m__0), new Func<int, string>(storey.<>m__1), <>f__am$cache0, RuntimeGenericContextInfo.Static);
        }

        public string StringLiteral(string literal, MetadataToken token, AssemblyDefinition assemblyDefinition) => 
            this._default.StringLiteral(literal, token, assemblyDefinition);

        public string TypeInfoFor(TypeReference type)
        {
            <TypeInfoFor>c__AnonStorey1 storey = new <TypeInfoFor>c__AnonStorey1 {
                type = type,
                $this = this
            };
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = index => Emit.Call("IL2CPP_RGCTX_DATA", "method->rgctx_data", index.ToString(CultureInfo.InvariantCulture));
            }
            return RetrieveType(storey.type, this._enclosingMethod, new Func<string>(storey.<>m__0), new Func<int, string>(storey.<>m__1), <>f__am$cache1, RuntimeGenericContextInfo.Class);
        }

        public string UnresolvedTypeInfoFor(TypeReference type)
        {
            <UnresolvedTypeInfoFor>c__AnonStorey2 storey = new <UnresolvedTypeInfoFor>c__AnonStorey2 {
                type = type,
                $this = this
            };
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = index => Emit.Call("IL2CPP_RGCTX_DATA", "method->rgctx_data", index.ToString(CultureInfo.InvariantCulture));
            }
            return RetrieveType(storey.type, this._enclosingMethod, new Func<string>(storey.<>m__0), new Func<int, string>(storey.<>m__1), <>f__am$cache2, RuntimeGenericContextInfo.Class);
        }

        [CompilerGenerated]
        private sealed class <ArrayInfo>c__AnonStorey5
        {
            internal SharedRuntimeMetadataAccess $this;
            internal TypeReference elementType;

            internal string <>m__0() => 
                this.$this._default.ArrayInfo(this.elementType);

            internal string <>m__1(int index) => 
                Emit.Call("IL2CPP_RGCTX_DATA", this.$this.GetTypeRgctxDataExpression(), index.ToString(CultureInfo.InvariantCulture));
        }

        [CompilerGenerated]
        private sealed class <HiddenMethodInfo>c__AnonStorey9
        {
            internal SharedRuntimeMetadataAccess $this;
            internal MethodReference method;

            internal string <>m__0() => 
                this.$this._default.HiddenMethodInfo(this.method);

            internal string <>m__1(int index) => 
                Emit.Call("IL2CPP_RGCTX_METHOD_INFO", this.$this.GetTypeRgctxDataExpression(), index.ToString(CultureInfo.InvariantCulture));
        }

        [CompilerGenerated]
        private sealed class <Il2CppTypeFor>c__AnonStorey4
        {
            internal SharedRuntimeMetadataAccess $this;
            internal TypeReference type;

            internal string <>m__0() => 
                this.$this._default.Il2CppTypeFor(this.type);

            internal string <>m__1(int index) => 
                Emit.Call("IL2CPP_RGCTX_TYPE", this.$this.GetTypeRgctxDataExpression(), index.ToString(CultureInfo.InvariantCulture));
        }

        [CompilerGenerated]
        private sealed class <Method>c__AnonStorey7
        {
            internal SharedRuntimeMetadataAccess $this;
            internal MethodReference method;
            internal MethodReference methodReference;

            internal string <>m__0() => 
                this.$this._default.Method(this.method);

            internal string <>m__1(int index) => 
                ("(" + Emit.Cast(MethodSignatureWriter.GetMethodPointerForVTable(this.methodReference), Emit.Call("IL2CPP_RGCTX_METHOD_INFO", this.$this.GetTypeRgctxDataExpression(), index.ToString(CultureInfo.InvariantCulture)) + "->methodPointer") + ")");

            internal string <>m__2(int index) => 
                ("(" + Emit.Cast(MethodSignatureWriter.GetMethodPointerForVTable(this.methodReference), Emit.Call("IL2CPP_RGCTX_METHOD_INFO", "method->rgctx_data", index.ToString(CultureInfo.InvariantCulture)) + "->methodPointer") + ")");
        }

        [CompilerGenerated]
        private sealed class <MethodInfo>c__AnonStorey8
        {
            internal SharedRuntimeMetadataAccess $this;
            internal MethodReference method;

            internal string <>m__0() => 
                this.$this._default.MethodInfo(this.method);

            internal string <>m__1(int index) => 
                Emit.Call("IL2CPP_RGCTX_METHOD_INFO", this.$this.GetTypeRgctxDataExpression(), index.ToString(CultureInfo.InvariantCulture));
        }

        [CompilerGenerated]
        private sealed class <Newobj>c__AnonStorey6
        {
            internal SharedRuntimeMetadataAccess $this;
            internal MethodReference ctor;

            internal string <>m__0() => 
                this.$this._default.Newobj(this.ctor);

            internal string <>m__1(int index) => 
                Emit.Call("IL2CPP_RGCTX_DATA", this.$this.GetTypeRgctxDataExpression(), index.ToString(CultureInfo.InvariantCulture));
        }

        [CompilerGenerated]
        private sealed class <SizeOf>c__AnonStorey3
        {
            internal SharedRuntimeMetadataAccess $this;
            internal TypeReference type;

            internal string <>m__0() => 
                this.$this._default.SizeOf(this.type);

            internal string <>m__1(int index) => 
                Emit.Call("IL2CPP_RGCTX_SIZEOF", this.$this.GetTypeRgctxDataExpression(), index.ToString(CultureInfo.InvariantCulture));
        }

        [CompilerGenerated]
        private sealed class <StaticData>c__AnonStorey0
        {
            internal SharedRuntimeMetadataAccess $this;
            internal TypeReference type;

            internal string <>m__0() => 
                this.$this._default.StaticData(this.type);

            internal string <>m__1(int index) => 
                Emit.Call("IL2CPP_RGCTX_DATA", this.$this.GetTypeRgctxDataExpression(), index.ToString(CultureInfo.InvariantCulture));
        }

        [CompilerGenerated]
        private sealed class <TypeInfoFor>c__AnonStorey1
        {
            internal SharedRuntimeMetadataAccess $this;
            internal TypeReference type;

            internal string <>m__0() => 
                this.$this._default.TypeInfoFor(this.type);

            internal string <>m__1(int index) => 
                Emit.Call("IL2CPP_RGCTX_DATA", this.$this.GetTypeRgctxDataExpression(), index.ToString(CultureInfo.InvariantCulture));
        }

        [CompilerGenerated]
        private sealed class <UnresolvedTypeInfoFor>c__AnonStorey2
        {
            internal SharedRuntimeMetadataAccess $this;
            internal TypeReference type;

            internal string <>m__0() => 
                this.$this._default.UnresolvedTypeInfoFor(this.type);

            internal string <>m__1(int index) => 
                Emit.Call("IL2CPP_RGCTX_DATA", this.$this.GetTypeRgctxDataExpression(), index.ToString(CultureInfo.InvariantCulture));
        }
    }
}

