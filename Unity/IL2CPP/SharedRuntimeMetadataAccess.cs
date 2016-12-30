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
        private readonly MethodReference _methodReference;
        private readonly Unity.IL2CPP.ILPreProcessor.TypeResolver _typeResolver;
        [CompilerGenerated]
        private static Func<bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<int, bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<int, bool> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<int, string> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<int, string> <>f__am$cache4;
        [Inject]
        public static IGenericSharingAnalysisService GenericSharingAnalysis;
        [Inject]
        public static INamingService Naming;

        public SharedRuntimeMetadataAccess(MethodReference methodReference, DefaultRuntimeMetadataAccess defaultRuntimeMetadataAccess)
        {
            this._methodReference = methodReference;
            this._typeResolver = new Unity.IL2CPP.ILPreProcessor.TypeResolver(methodReference.DeclaringType as GenericInstanceType, methodReference as GenericInstanceMethod);
            this._default = defaultRuntimeMetadataAccess;
        }

        public string ArrayInfo(TypeReference elementType)
        {
            <ArrayInfo>c__AnonStorey4 storey = new <ArrayInfo>c__AnonStorey4 {
                elementType = elementType,
                $this = this
            };
            return this.RetreiveType(storey.elementType, new Func<string>(storey.<>m__0), "IL2CPP_RGCTX_DATA", "IL2CPP_RGCTX_DATA", RuntimeGenericContextInfo.Array);
        }

        public string FieldInfo(FieldReference field)
        {
            if (GetRGCTXAccess(field.DeclaringType, this._methodReference) == RuntimeGenericAccess.None)
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
            if (!this._methodReference.HasThis || this._methodReference.DeclaringType.IsValueType())
            {
                argument = Naming.ForInitializedTypeInfo(argument);
            }
            return $"{argument}->rgctx_data";
        }

        public string HiddenMethodInfo(MethodReference method)
        {
            <HiddenMethodInfo>c__AnonStorey8 storey = new <HiddenMethodInfo>c__AnonStorey8 {
                method = method,
                $this = this
            };
            if (<>f__am$cache4 == null)
            {
                <>f__am$cache4 = index => Emit.Call("IL2CPP_RGCTX_METHOD_INFO", "method->rgctx_data", index.ToString(CultureInfo.InvariantCulture));
            }
            return this.RetreiveMethod<string>(storey.method, new Func<string>(storey.<>m__0), new Func<int, string>(storey.<>m__1), <>f__am$cache4, RuntimeGenericContextInfo.Method);
        }

        public string Il2CppTypeFor(TypeReference type)
        {
            <Il2CppTypeFor>c__AnonStorey3 storey = new <Il2CppTypeFor>c__AnonStorey3 {
                type = type,
                $this = this
            };
            return this.RetreiveType(storey.type, new Func<string>(storey.<>m__0), "IL2CPP_RGCTX_TYPE", "IL2CPP_RGCTX_TYPE", RuntimeGenericContextInfo.Type);
        }

        public string Method(MethodReference method)
        {
            <Method>c__AnonStorey6 storey = new <Method>c__AnonStorey6 {
                method = method,
                $this = this
            };
            storey.methodReference = this._typeResolver.Resolve(storey.method);
            return this.RetreiveMethod<string>(storey.method, new Func<string>(storey.<>m__0), new Func<int, string>(storey.<>m__1), new Func<int, string>(storey.<>m__2), RuntimeGenericContextInfo.Method);
        }

        public string MethodInfo(MethodReference method)
        {
            <MethodInfo>c__AnonStorey7 storey = new <MethodInfo>c__AnonStorey7 {
                method = method,
                $this = this
            };
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = index => Emit.Call("IL2CPP_RGCTX_METHOD_INFO", "method->rgctx_data", index.ToString(CultureInfo.InvariantCulture));
            }
            return this.RetreiveMethod<string>(storey.method, new Func<string>(storey.<>m__0), new Func<int, string>(storey.<>m__1), <>f__am$cache3, RuntimeGenericContextInfo.Method);
        }

        public bool NeedsBoxingForValueTypeThis(MethodReference method)
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = () => false;
            }
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = index => true;
            }
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = index => true;
            }
            return this.RetreiveMethod<bool>(method, <>f__am$cache0, <>f__am$cache1, <>f__am$cache2, RuntimeGenericContextInfo.Method);
        }

        public string Newobj(MethodReference ctor)
        {
            <Newobj>c__AnonStorey5 storey = new <Newobj>c__AnonStorey5 {
                ctor = ctor,
                $this = this
            };
            return this.RetreiveType(storey.ctor.DeclaringType, new Func<string>(storey.<>m__0), "IL2CPP_RGCTX_DATA", "IL2CPP_RGCTX_DATA", RuntimeGenericContextInfo.Class);
        }

        private T RetreiveMethod<T>(MethodReference method, Func<T> defaultFunc, Func<int, T> retrieveTypeSharedAccess, Func<int, T> retrieveMethodSharedAccess, RuntimeGenericContextInfo info)
        {
            RuntimeGenericAccess rGCTXAccess = GetRGCTXAccess(method, this._methodReference);
            switch (rGCTXAccess)
            {
                case RuntimeGenericAccess.None:
                    return defaultFunc();

                case RuntimeGenericAccess.Method:
                {
                    GenericSharingData data = GenericSharingAnalysis.RuntimeGenericContextFor(this._methodReference.Resolve());
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
            GenericSharingData rgctx = GenericSharingAnalysis.RuntimeGenericContextFor(this._methodReference.DeclaringType.Resolve());
            int arg = RetrieveMethodIndex(method, info, rgctx);
            if (arg == -1)
            {
                throw new InvalidOperationException(FormatGenericContextErrorMessage(method.FullName));
            }
            return retrieveTypeSharedAccess(arg);
        }

        private string RetreiveType(TypeReference type, Func<string> defaultFunc, string typeSharedAccessName, string methodSharedAccessName, RuntimeGenericContextInfo info)
        {
            RuntimeGenericAccess rGCTXAccess = GetRGCTXAccess(type, this._methodReference);
            switch (rGCTXAccess)
            {
                case RuntimeGenericAccess.None:
                    return defaultFunc();

                case RuntimeGenericAccess.Method:
                {
                    GenericSharingData data = GenericSharingAnalysis.RuntimeGenericContextFor(this._methodReference.Resolve());
                    int num = RetrieveTypeIndex(type, info, data);
                    if (num == -1)
                    {
                        throw new InvalidOperationException(FormatGenericContextErrorMessage(type.FullName));
                    }
                    return Emit.Call(methodSharedAccessName, "method->rgctx_data", num.ToString(CultureInfo.InvariantCulture));
                }
            }
            if ((rGCTXAccess != RuntimeGenericAccess.This) && (rGCTXAccess != RuntimeGenericAccess.Type))
            {
                throw new ArgumentOutOfRangeException("type");
            }
            GenericSharingData rgctx = GenericSharingAnalysis.RuntimeGenericContextFor(this._methodReference.DeclaringType.Resolve());
            int num2 = RetrieveTypeIndex(type, info, rgctx);
            if (num2 == -1)
            {
                throw new InvalidOperationException(FormatGenericContextErrorMessage(type.FullName));
            }
            return Emit.Call(typeSharedAccessName, this.GetTypeRgctxDataExpression(), num2.ToString(CultureInfo.InvariantCulture));
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
            <SizeOf>c__AnonStorey2 storey = new <SizeOf>c__AnonStorey2 {
                type = type,
                $this = this
            };
            return this.RetreiveType(storey.type, new Func<string>(storey.<>m__0), "IL2CPP_RGCTX_SIZEOF", "IL2CPP_RGCTX_SIZEOF", RuntimeGenericContextInfo.Class);
        }

        public string StaticData(TypeReference type)
        {
            <StaticData>c__AnonStorey0 storey = new <StaticData>c__AnonStorey0 {
                type = type,
                $this = this
            };
            return this.RetreiveType(storey.type, new Func<string>(storey.<>m__0), "IL2CPP_RGCTX_DATA", "IL2CPP_RGCTX_DATA", RuntimeGenericContextInfo.Static);
        }

        public string StringLiteral(string literal) => 
            this._default.StringLiteral(literal);

        public string TypeInfoFor(TypeReference type)
        {
            <TypeInfoFor>c__AnonStorey1 storey = new <TypeInfoFor>c__AnonStorey1 {
                type = type,
                $this = this
            };
            return this.RetreiveType(storey.type, new Func<string>(storey.<>m__0), "IL2CPP_RGCTX_DATA", "IL2CPP_RGCTX_DATA", RuntimeGenericContextInfo.Class);
        }

        [CompilerGenerated]
        private sealed class <ArrayInfo>c__AnonStorey4
        {
            internal SharedRuntimeMetadataAccess $this;
            internal TypeReference elementType;

            internal string <>m__0() => 
                this.$this._default.ArrayInfo(this.elementType);
        }

        [CompilerGenerated]
        private sealed class <HiddenMethodInfo>c__AnonStorey8
        {
            internal SharedRuntimeMetadataAccess $this;
            internal MethodReference method;

            internal string <>m__0() => 
                this.$this._default.HiddenMethodInfo(this.method);

            internal string <>m__1(int index) => 
                Emit.Call("IL2CPP_RGCTX_METHOD_INFO", this.$this.GetTypeRgctxDataExpression(), index.ToString(CultureInfo.InvariantCulture));
        }

        [CompilerGenerated]
        private sealed class <Il2CppTypeFor>c__AnonStorey3
        {
            internal SharedRuntimeMetadataAccess $this;
            internal TypeReference type;

            internal string <>m__0() => 
                this.$this._default.Il2CppTypeFor(this.type);
        }

        [CompilerGenerated]
        private sealed class <Method>c__AnonStorey6
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
        private sealed class <MethodInfo>c__AnonStorey7
        {
            internal SharedRuntimeMetadataAccess $this;
            internal MethodReference method;

            internal string <>m__0() => 
                this.$this._default.MethodInfo(this.method);

            internal string <>m__1(int index) => 
                Emit.Call("IL2CPP_RGCTX_METHOD_INFO", this.$this.GetTypeRgctxDataExpression(), index.ToString(CultureInfo.InvariantCulture));
        }

        [CompilerGenerated]
        private sealed class <Newobj>c__AnonStorey5
        {
            internal SharedRuntimeMetadataAccess $this;
            internal MethodReference ctor;

            internal string <>m__0() => 
                this.$this._default.Newobj(this.ctor);
        }

        [CompilerGenerated]
        private sealed class <SizeOf>c__AnonStorey2
        {
            internal SharedRuntimeMetadataAccess $this;
            internal TypeReference type;

            internal string <>m__0() => 
                this.$this._default.SizeOf(this.type);
        }

        [CompilerGenerated]
        private sealed class <StaticData>c__AnonStorey0
        {
            internal SharedRuntimeMetadataAccess $this;
            internal TypeReference type;

            internal string <>m__0() => 
                this.$this._default.StaticData(this.type);
        }

        [CompilerGenerated]
        private sealed class <TypeInfoFor>c__AnonStorey1
        {
            internal SharedRuntimeMetadataAccess $this;
            internal TypeReference type;

            internal string <>m__0() => 
                this.$this._default.TypeInfoFor(this.type);
        }
    }
}

