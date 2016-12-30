namespace Unity.IL2CPP.WindowsRuntime
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
    using Unity.IL2CPP.Marshaling.BodyWriters;
    using Unity.IL2CPP.Marshaling.BodyWriters.NativeToManaged;

    internal sealed class GenericEnumeratorToIteratorAdapterWriter : CCWWriterBase
    {
        private readonly string _hresult;
        private readonly MethodReference _iEnumeratorGetCurrentMethod;
        private readonly MethodReference _iEnumeratorMoveNextMethod;
        private readonly MethodReference _iIteratorGetCurrentMethod;
        private readonly InteropMethodInfo _iIteratorGetCurrentMethodInfo;
        private readonly IEnumerable<MethodReference> _iIteratorMethods;
        private readonly GenericInstanceType _iIteratorType;
        private readonly Unity.IL2CPP.ILPreProcessor.TypeResolver _iIteratorTypeResolver;
        private readonly string _returnValue;
        private readonly string _typeName;
        [CompilerGenerated]
        private static Func<MethodReference, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<MethodReference, bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<MethodReference, bool> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<MethodReference, bool> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<TypeReference, bool> <>f__am$cache4;
        [CompilerGenerated]
        private static Func<MethodReference, bool> <>f__am$cache5;
        [CompilerGenerated]
        private static Func<MethodReference, bool> <>f__am$cache6;
        [CompilerGenerated]
        private static Func<MethodReference, bool> <>f__am$cache7;
        private const string GetCurrentMethodName = "GetCurrent";
        private const string HasCurrentFieldName = "_hasCurrent";
        private const string InitializedFieldName = "_initialized";
        private const string InitializeMethodName = "Initialize";
        private const string MoveNextMethodName = "MoveNext";
        [Inject]
        public static IWindowsRuntimeProjections WindowsRuntimeProjections;

        private GenericEnumeratorToIteratorAdapterWriter(GenericInstanceType iIterableType) : base(Unity.IL2CPP.ILPreProcessor.TypeResolver.For(iIterableType).Resolve(iIterableType.GetMethods().First<MethodReference>(<>f__am$cache0).ReturnType))
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<MethodReference, bool>(GenericEnumeratorToIteratorAdapterWriter.<GenericEnumeratorToIteratorAdapterWriter>m__0);
            }
            this._iIteratorType = (GenericInstanceType) base._type;
            this._iIteratorTypeResolver = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(this._iIteratorType);
            this._iIteratorMethods = this._iIteratorType.GetMethods();
            this._typeName = CCWWriterBase.Naming.ForWindowsRuntimeAdapterClass(this._iIteratorType);
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = new Func<MethodReference, bool>(GenericEnumeratorToIteratorAdapterWriter.<GenericEnumeratorToIteratorAdapterWriter>m__1);
            }
            this._iIteratorGetCurrentMethod = this._iIteratorMethods.First<MethodReference>(<>f__am$cache1);
            GenericInstanceType type = (GenericInstanceType) WindowsRuntimeProjections.ProjectToCLR(iIterableType);
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = new Func<MethodReference, bool>(GenericEnumeratorToIteratorAdapterWriter.<GenericEnumeratorToIteratorAdapterWriter>m__2);
            }
            MethodReference reference = type.GetMethods().First<MethodReference>(<>f__am$cache2);
            GenericInstanceType type2 = (GenericInstanceType) Unity.IL2CPP.ILPreProcessor.TypeResolver.For(type).Resolve(reference.ReturnType);
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = new Func<MethodReference, bool>(GenericEnumeratorToIteratorAdapterWriter.<GenericEnumeratorToIteratorAdapterWriter>m__3);
            }
            this._iEnumeratorGetCurrentMethod = type2.GetMethods().First<MethodReference>(<>f__am$cache3);
            if (<>f__am$cache4 == null)
            {
                <>f__am$cache4 = new Func<TypeReference, bool>(GenericEnumeratorToIteratorAdapterWriter.<GenericEnumeratorToIteratorAdapterWriter>m__4);
            }
            TypeReference reference2 = type2.GetInterfaces().First<TypeReference>(<>f__am$cache4);
            if (<>f__am$cache5 == null)
            {
                <>f__am$cache5 = new Func<MethodReference, bool>(GenericEnumeratorToIteratorAdapterWriter.<GenericEnumeratorToIteratorAdapterWriter>m__5);
            }
            this._iEnumeratorMoveNextMethod = reference2.GetMethods().First<MethodReference>(<>f__am$cache5);
            this._iIteratorGetCurrentMethodInfo = InteropMethodInfo.ForComCallableWrapper(this._iEnumeratorGetCurrentMethod, this._iIteratorGetCurrentMethod, MarshalType.WindowsRuntime);
            this._returnValue = CCWWriterBase.Naming.ForComInterfaceReturnParameterName();
            this._hresult = CCWWriterBase.Naming.ForInteropHResultVariable();
        }

        [CompilerGenerated]
        private static bool <GenericEnumeratorToIteratorAdapterWriter>m__0(MethodReference m) => 
            (m.Name == "First");

        [CompilerGenerated]
        private static bool <GenericEnumeratorToIteratorAdapterWriter>m__1(MethodReference m) => 
            (m.Name == "get_Current");

        [CompilerGenerated]
        private static bool <GenericEnumeratorToIteratorAdapterWriter>m__2(MethodReference m) => 
            (m.Name == "GetEnumerator");

        [CompilerGenerated]
        private static bool <GenericEnumeratorToIteratorAdapterWriter>m__3(MethodReference m) => 
            (m.Name == "get_Current");

        [CompilerGenerated]
        private static bool <GenericEnumeratorToIteratorAdapterWriter>m__4(TypeReference i) => 
            (i.FullName == "System.Collections.IEnumerator");

        [CompilerGenerated]
        private static bool <GenericEnumeratorToIteratorAdapterWriter>m__5(MethodReference m) => 
            (m.Name == "MoveNext");

        private void WriteConstructor(CppCodeWriter writer)
        {
            writer.WriteLine($"inline {this._typeName}(Il2CppObject* obj) : il2cpp::vm::NonCachedCCWBase<{this._typeName}>(obj), {"_initialized"}(false), {"_hasCurrent"}(false) {{}}");
        }

        public static void WriteDefinitions(CppCodeWriter writer, GenericInstanceType type)
        {
            GenericEnumeratorToIteratorAdapterWriter writer2 = new GenericEnumeratorToIteratorAdapterWriter(type);
            writer2.WriteTypeDefinition(writer);
            writer2.WriteMethodDefinitions(writer);
        }

        private void WriteInitializationCode(CppCodeWriter writer, string defaultValue)
        {
            writer.WriteStatement("il2cpp_native_wrapper_vm_thread_attacher _vmThreadHelper");
            writer.WriteStatement(Emit.Assign("il2cpp_hresult_t " + this._hresult, Emit.Call("Initialize")));
            writer.WriteLine($"if (IL2CPP_HR_FAILED({this._hresult}))");
            using (new BlockWriter(writer, false))
            {
                writer.WriteStatement(Emit.Assign('*' + this._returnValue, defaultValue));
                writer.WriteStatement("return " + this._hresult);
            }
        }

        private void WriteMethodDefinitions(CppCodeWriter writer)
        {
            if (<>f__am$cache6 == null)
            {
                <>f__am$cache6 = m => m.Name == "get_Current";
            }
            TypeReference typeReference = this._iIteratorTypeResolver.Resolve(this._iIteratorMethods.First<MethodReference>(<>f__am$cache6).ReturnType);
            writer.AddIncludeForTypeDefinition(typeReference);
            using (IEnumerator<MethodReference> enumerator = this._iIteratorMethods.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    <WriteMethodDefinitions>c__AnonStorey0 storey = new <WriteMethodDefinitions>c__AnonStorey0 {
                        method = enumerator.Current,
                        $this = this
                    };
                    string methodSignature = ComInterfaceWriter.GetSignature(storey.method, storey.method, this._iIteratorTypeResolver, this._typeName, true);
                    MethodWriter.WriteMethodWithMetadataInitialization(writer, methodSignature, storey.method.FullName, new Action<CppCodeWriter, MetadataUsage, MethodUsage>(storey.<>m__0), this._typeName + '_' + CCWWriterBase.Naming.ForMethodNameOnly(storey.method));
                }
            }
            this.WriteMethodInitialize(writer);
            this.WriteMethodGetCurrent(writer);
            this.WriteMethodMoveNext(writer);
        }

        private void WriteMethodGetCurrent(CppCodeWriter writer)
        {
            string methodSignature = $"il2cpp_hresult_t {this._typeName}::{"GetCurrent"}({this._iIteratorGetCurrentMethodInfo.MarshaledReturnType.DecoratedName}* {this._returnValue})";
            MethodWriter.WriteMethodWithMetadataInitialization(writer, methodSignature, "GetCurrent", delegate (CppCodeWriter bodyWriter, MetadataUsage metadataUsage, MethodUsage methodUsage) {
                IRuntimeMetadataAccess metadataAccess = MethodWriter.GetDefaultRuntimeMetadataAccess(this._iEnumeratorGetCurrentMethod, metadataUsage, methodUsage);
                new GetCurrentMethodBodyWriter(this._iEnumeratorGetCurrentMethod, this._iIteratorGetCurrentMethod).WriteMethodBody(bodyWriter, metadataAccess);
            }, this._typeName + '_' + "GetCurrent");
        }

        private void WriteMethodGetCurrent(MethodReference method, CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
        {
            string defaultReturnCppValue = this._iIteratorGetCurrentMethodInfo.GetDefaultReturnCppValue();
            this.WriteInitializationCode(writer, defaultReturnCppValue);
            writer.WriteStatement("return " + Emit.Call("GetCurrent", this._returnValue));
        }

        private void WriteMethodGetHasCurrentCurrent(MethodReference method, CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
        {
            this.WriteInitializationCode(writer, "false");
            writer.WriteStatement(Emit.Assign('*' + this._returnValue, "_hasCurrent"));
            writer.WriteStatement("return IL2CPP_S_OK");
        }

        private void WriteMethodGetMany(MethodReference method, CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
        {
            this.WriteInitializationCode(writer, "0");
            writer.WriteVariable(CCWWriterBase.TypeProvider.UInt32TypeReference, "count");
            InteropMethodInfo info = InteropMethodInfo.ForComCallableWrapper(method, method, MarshalType.WindowsRuntime);
            string variableName = info.MarshaledParameterTypes[0].VariableName;
            string str2 = info.MarshaledParameterTypes[1].VariableName;
            writer.WriteLine($"for (; {"_hasCurrent"} && (count < {variableName}); ++count)");
            using (new BlockWriter(writer, false))
            {
                writer.WriteStatement(Emit.Assign(this._hresult, Emit.Call("GetCurrent", str2 + " + count")));
                writer.WriteLine($"if (IL2CPP_HR_FAILED({this._hresult}))");
                using (new BlockWriter(writer, false))
                {
                    writer.WriteStatement(Emit.Assign('*' + this._returnValue, "count"));
                    writer.WriteStatement("return " + this._hresult);
                }
                writer.WriteStatement(Emit.Assign(this._hresult, Emit.Call("MoveNext", '&' + "_hasCurrent")));
                writer.WriteLine($"if (IL2CPP_HR_FAILED({this._hresult}))");
                using (new BlockWriter(writer, false))
                {
                    writer.WriteStatement(Emit.Assign('*' + this._returnValue, "count + 1"));
                    writer.WriteStatement("return " + this._hresult);
                }
            }
            writer.WriteStatement(Emit.Assign('*' + this._returnValue, "count"));
            writer.WriteStatement("return IL2CPP_S_OK");
        }

        private void WriteMethodInitialize(CppCodeWriter writer)
        {
            string methodSignature = $"il2cpp_hresult_t {this._typeName}::{"Initialize"}()";
            MethodWriter.WriteMethodWithMetadataInitialization(writer, methodSignature, "Initialize", delegate (CppCodeWriter bodyWriter, MetadataUsage metadataUsage, MethodUsage methodUsage) {
                bodyWriter.WriteLine($"if ({"_initialized"})");
                using (new BlockWriter(bodyWriter, false))
                {
                    bodyWriter.WriteStatement("return IL2CPP_S_OK");
                }
                bodyWriter.WriteStatement(Emit.Assign("il2cpp_hresult_t " + this._hresult, Emit.Call("MoveNext", '&' + "_hasCurrent")));
                bodyWriter.WriteStatement(Emit.Assign("_initialized", $"IL2CPP_HR_SUCCEEDED({this._hresult})"));
                bodyWriter.WriteStatement("return " + this._hresult);
            }, this._typeName + '_' + "Initialize");
        }

        private void WriteMethodMoveNext(CppCodeWriter writer)
        {
            string methodSignature = $"il2cpp_hresult_t {this._typeName}::{"MoveNext"}(bool* {this._returnValue})";
            MethodWriter.WriteMethodWithMetadataInitialization(writer, methodSignature, "MoveNext", delegate (CppCodeWriter bodyWriter, MetadataUsage metadataUsage, MethodUsage methodUsage) {
                if (<>f__am$cache7 == null)
                {
                    <>f__am$cache7 = m => m.Name == "MoveNext";
                }
                MethodReference interfaceMethod = this._iIteratorType.GetMethods().First<MethodReference>(<>f__am$cache7);
                IRuntimeMetadataAccess metadataAccess = MethodWriter.GetDefaultRuntimeMetadataAccess(this._iEnumeratorMoveNextMethod, metadataUsage, methodUsage);
                new MoveNextMethodBodyWriter(this._iEnumeratorMoveNextMethod, interfaceMethod).WriteMethodBody(bodyWriter, metadataAccess);
            }, this._typeName + '_' + "MoveNext");
        }

        private void WriteMethodMoveNext(MethodReference method, CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
        {
            this.WriteInitializationCode(writer, "false");
            writer.WriteStatement(Emit.Assign(this._hresult, Emit.Call("MoveNext", '&' + "_hasCurrent")));
            writer.WriteStatement(Emit.Assign('*' + this._returnValue, "_hasCurrent"));
            writer.WriteStatement("return " + this._hresult);
        }

        private void WriteTypeDefinition(CppCodeWriter writer)
        {
            writer.AddCodeGenIncludes();
            writer.AddInclude("vm/NonCachedCCWBase.h");
            base.AddIncludes(writer);
            writer.WriteCommentedLine(this._iIteratorType.FullName + " adapter");
            writer.WriteLine($"struct {this._typeName} IL2CPP_FINAL : il2cpp::vm::NonCachedCCWBase<{this._typeName}>, {CCWWriterBase.Naming.ForTypeNameOnly(this._iIteratorType)}");
            using (new BlockWriter(writer, true))
            {
                this.WriteConstructor(writer);
                base.WriteCommonInterfaceMethods(writer);
                foreach (MethodReference reference in this._iIteratorMethods)
                {
                    writer.WriteLine(ComInterfaceWriter.GetSignature(reference, reference, this._iIteratorTypeResolver, null, true) + ';');
                }
                using (new DedentWriter(writer))
                {
                    writer.WriteLine("private:");
                }
                writer.WriteLine($"bool {"_initialized"};");
                writer.WriteLine($"bool {"_hasCurrent"};");
                writer.WriteLine($"il2cpp_hresult_t {"Initialize"}();");
                writer.WriteLine($"il2cpp_hresult_t {"GetCurrent"}({this._iIteratorGetCurrentMethodInfo.MarshaledReturnType.DecoratedName}* {this._returnValue});");
                writer.WriteLine($"il2cpp_hresult_t {"MoveNext"}(bool* {this._returnValue});");
            }
        }

        protected override IEnumerable<TypeReference> AllImplementedInterfaces =>
            new TypeReference[] { base._type };

        protected override bool ImplementsAnyIInspectableInterfaces =>
            true;

        [CompilerGenerated]
        private sealed class <WriteMethodDefinitions>c__AnonStorey0
        {
            internal GenericEnumeratorToIteratorAdapterWriter $this;
            internal MethodReference method;

            internal void <>m__0(CppCodeWriter bodyWriter, MetadataUsage metadataUsage, MethodUsage methodUsage)
            {
                DefaultRuntimeMetadataAccess metadataAccess = new DefaultRuntimeMetadataAccess(this.method, metadataUsage, methodUsage);
                switch (this.method.Name)
                {
                    case "get_Current":
                        this.$this.WriteMethodGetCurrent(this.method, bodyWriter, metadataAccess);
                        return;

                    case "get_HasCurrent":
                        this.$this.WriteMethodGetHasCurrentCurrent(this.method, bodyWriter, metadataAccess);
                        return;

                    case "MoveNext":
                        this.$this.WriteMethodMoveNext(this.method, bodyWriter, metadataAccess);
                        return;

                    case "GetMany":
                        this.$this.WriteMethodGetMany(this.method, bodyWriter, metadataAccess);
                        return;
                }
                throw new NotSupportedException($"Interface '{this.$this._iIteratorType.FullName}' contains unsupported method '{this.method.Name}'.");
            }
        }

        private sealed class GetCurrentMethodBodyWriter : ComCallableWrapperMethodBodyWriter
        {
            public GetCurrentMethodBodyWriter(MethodReference managedMethod, MethodReference interfaceMethod) : base(managedMethod, interfaceMethod, MarshalType.WindowsRuntime)
            {
            }

            protected override void WriteExceptionReturnStatement(CppCodeWriter writer)
            {
                writer.WriteStatement("return (ex.ex->hresult == IL2CPP_COR_E_INVALIDOPERATION) ? IL2CPP_E_BOUNDS : ex.ex->hresult");
            }

            protected override void WriteMethodPrologue(CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
            {
            }
        }

        private sealed class MoveNextMethodBodyWriter : ComCallableWrapperMethodBodyWriter
        {
            public MoveNextMethodBodyWriter(MethodReference managedMethod, MethodReference interfaceMethod) : base(managedMethod, interfaceMethod, MarshalType.WindowsRuntime)
            {
            }

            protected override void WriteMethodPrologue(CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
            {
            }
        }
    }
}

