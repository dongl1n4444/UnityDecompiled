namespace Unity.IL2CPP.WindowsRuntime
{
    using Mono.Cecil;
    using Mono.Collections.Generic;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP;
    using Unity.IL2CPP.ILPreProcessor;
    using Unity.IL2CPP.Marshaling;
    using Unity.IL2CPP.Marshaling.BodyWriters;
    using Unity.IL2CPP.Marshaling.BodyWriters.NativeToManaged;

    internal sealed class EnumeratorToBindableIteratorAdapterWriter : CCWWriterBase
    {
        private readonly string _hresult;
        private readonly TypeDefinition _iEnumeratorType;
        private readonly string _returnValue;
        private string _typeName;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache2;
        private const string HasCurrentFieldName = "_hasCurrent";
        private const string InitializedFieldName = "_initialized";
        private const string InitializeMethodName = "Initialize";
        private const string MoveNextMethodName = "MoveNext";

        private EnumeratorToBindableIteratorAdapterWriter() : base(CCWWriterBase.TypeProvider.IBindableIteratorTypeReference)
        {
            this._iEnumeratorType = CCWWriterBase.TypeProvider.Corlib.MainModule.GetType("System.Collections", "IEnumerator");
            this._returnValue = CCWWriterBase.Naming.ForComInterfaceReturnParameterName();
            this._hresult = CCWWriterBase.Naming.ForInteropHResultVariable();
            if (base._type == null)
            {
                throw new InvalidOperationException("It is not valid to use EnumeratorToBindableIteratorAdapterWriter without IBindableIterator type available.");
            }
            if (this._iEnumeratorType == null)
            {
                throw new InvalidOperationException("It is not valid to use EnumeratorToBindableIteratorAdapterWriter without IEnumerator type available.");
            }
            this._typeName = CCWWriterBase.Naming.ForWindowsRuntimeAdapterClass(this.IBindableIteratorType);
        }

        private void WriteConstructor(CppCodeWriter writer)
        {
            writer.WriteLine($"inline {this._typeName}(Il2CppObject* obj) : il2cpp::vm::NonCachedCCWBase<{this._typeName}>(obj), {"_initialized"}(false), {"_hasCurrent"}(false) {{}}");
        }

        public static void WriteDefinitions(CppCodeWriter writer)
        {
            EnumeratorToBindableIteratorAdapterWriter writer2 = new EnumeratorToBindableIteratorAdapterWriter();
            writer2.WriteTypeDefinition(writer);
            writer2.WriteMethodDefinitions(writer);
        }

        private void WriteInitializationCode(CppCodeWriter writer)
        {
            writer.WriteStatement(Emit.Assign($"il2cpp_hresult_t {this._hresult}", Emit.Call("Initialize")));
            writer.WriteLine($"if (IL2CPP_HR_FAILED({this._hresult}))");
            using (new BlockWriter(writer, false))
            {
                writer.WriteStatement(Emit.Assign('*' + this._returnValue, "false"));
                writer.WriteStatement("return " + this._hresult);
            }
        }

        private void WriteMethodDefinitions(CppCodeWriter writer)
        {
            Unity.IL2CPP.ILPreProcessor.TypeResolver typeResolver = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(this.IBindableIteratorType);
            using (Collection<MethodDefinition>.Enumerator enumerator = this.IBindableIteratorType.Resolve().Methods.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    <WriteMethodDefinitions>c__AnonStorey0 storey = new <WriteMethodDefinitions>c__AnonStorey0 {
                        method = enumerator.Current,
                        $this = this
                    };
                    string methodSignature = ComInterfaceWriter.GetSignature(storey.method, storey.method, typeResolver, this._typeName, true);
                    MethodWriter.WriteMethodWithMetadataInitialization(writer, methodSignature, storey.method.FullName, new Action<CppCodeWriter, MetadataUsage, MethodUsage>(storey.<>m__0), this._typeName + '_' + CCWWriterBase.Naming.ForMethodNameOnly(storey.method));
                }
            }
            this.WriteMethodInitialize(writer);
            this.WriteMethodMoveNext(writer);
        }

        private void WriteMethodGetCurrent(MethodReference method, CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = m => m.Name == "get_Current";
            }
            new GetCurrentMethodBodyWriter(this._iEnumeratorType.Methods.First<MethodDefinition>(<>f__am$cache0), method).WriteMethodBody(writer, metadataAccess);
        }

        private void WriteMethodGetHasCurrentCurrent(MethodReference method, CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
        {
            this.WriteInitializationCode(writer);
            writer.WriteStatement(Emit.Assign('*' + this._returnValue, "_hasCurrent"));
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
                bodyWriter.WriteStatement(Emit.Assign($"const il2cpp_hresult_t {this._hresult}", Emit.Call("MoveNext", '&' + "_hasCurrent")));
                bodyWriter.WriteStatement(Emit.Assign("_initialized", $"IL2CPP_HR_SUCCEEDED({this._hresult})"));
                bodyWriter.WriteStatement("return " + this._hresult);
            }, this._typeName + '_' + "Initialize");
        }

        private void WriteMethodMoveNext(CppCodeWriter writer)
        {
            string methodSignature = $"il2cpp_hresult_t {this._typeName}::{"MoveNext"}(bool* {this._returnValue})";
            MethodWriter.WriteMethodWithMetadataInitialization(writer, methodSignature, "MoveNext", delegate (CppCodeWriter bodyWriter, MetadataUsage metadataUsage, MethodUsage methodUsage) {
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = m => m.Name == "MoveNext";
                }
                MethodDefinition method = this._iEnumeratorType.Methods.First<MethodDefinition>(<>f__am$cache1);
                if (<>f__am$cache2 == null)
                {
                    <>f__am$cache2 = m => m.Name == "MoveNext";
                }
                MethodDefinition interfaceMethod = this.IBindableIteratorType.Resolve().Methods.First<MethodDefinition>(<>f__am$cache2);
                IRuntimeMetadataAccess metadataAccess = MethodWriter.GetDefaultRuntimeMetadataAccess(method, metadataUsage, methodUsage, null);
                new ComCallableWrapperMethodBodyWriter(method, interfaceMethod, MarshalType.WindowsRuntime).WriteMethodBody(bodyWriter, metadataAccess);
            }, this._typeName + '_' + "MoveNext");
        }

        private void WriteMethodMoveNext(MethodReference method, CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
        {
            this.WriteInitializationCode(writer);
            writer.WriteStatement(Emit.Assign(this._hresult, Emit.Call("MoveNext", $"&{"_hasCurrent"}")));
            writer.WriteStatement(Emit.Assign('*' + this._returnValue, "_hasCurrent"));
            writer.WriteStatement("return " + this._hresult);
        }

        private void WriteTypeDefinition(CppCodeWriter writer)
        {
            writer.AddCodeGenIncludes();
            writer.AddInclude("vm/NonCachedCCWBase.h");
            base.AddIncludes(writer);
            writer.WriteCommentedLine(this.IBindableIteratorType.FullName + " adapter");
            writer.WriteLine($"struct {this._typeName} IL2CPP_FINAL : il2cpp::vm::NonCachedCCWBase<{this._typeName}>, {CCWWriterBase.Naming.ForTypeNameOnly(this.IBindableIteratorType)}");
            using (new BlockWriter(writer, true))
            {
                this.WriteConstructor(writer);
                base.WriteCommonInterfaceMethods(writer);
                Unity.IL2CPP.ILPreProcessor.TypeResolver typeResolver = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(this.IBindableIteratorType);
                foreach (MethodDefinition definition in this.IBindableIteratorType.Resolve().Methods)
                {
                    writer.WriteLine(ComInterfaceWriter.GetSignature(definition, definition, typeResolver, null, true) + ';');
                }
                using (new DedentWriter(writer))
                {
                    writer.WriteLine("private:");
                }
                writer.WriteLine($"bool {"_initialized"};");
                writer.WriteLine($"bool {"_hasCurrent"};");
                writer.WriteLine($"il2cpp_hresult_t {"Initialize"}();");
                writer.WriteLine($"il2cpp_hresult_t {"MoveNext"}(bool* {this._returnValue});");
            }
        }

        protected override IEnumerable<TypeReference> AllImplementedInterfaces =>
            new TypeReference[] { base._type };

        private TypeReference IBindableIteratorType =>
            base._type;

        protected override bool ImplementsAnyIInspectableInterfaces =>
            true;

        [CompilerGenerated]
        private sealed class <WriteMethodDefinitions>c__AnonStorey0
        {
            internal EnumeratorToBindableIteratorAdapterWriter $this;
            internal MethodDefinition method;

            internal void <>m__0(CppCodeWriter bodyWriter, MetadataUsage metadataUsage, MethodUsage methodUsage)
            {
                DefaultRuntimeMetadataAccess metadataAccess = new DefaultRuntimeMetadataAccess(this.method, metadataUsage, methodUsage, null);
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
                }
                throw new NotSupportedException($"Interface '{this.$this.IBindableIteratorType.FullName}' contains unsupported method '{this.method.Name}'.");
            }
        }

        private sealed class GetCurrentMethodBodyWriter : ComCallableWrapperMethodBodyWriter
        {
            private readonly string _hresult;

            public GetCurrentMethodBodyWriter(MethodReference managedMethod, MethodReference interfaceMethod) : base(managedMethod, interfaceMethod, MarshalType.WindowsRuntime)
            {
                this._hresult = InteropMethodInfo.Naming.ForInteropHResultVariable();
            }

            protected override void WriteExceptionReturnStatement(CppCodeWriter writer)
            {
                writer.WriteStatement(Emit.Assign(this._hresult, "ex.ex->hresult"));
                writer.WriteStatement($"return ({this._hresult} == IL2CPP_COR_E_INVALIDOPERATION) ? IL2CPP_E_BOUNDS : {this._hresult}");
            }

            protected override void WriteMethodPrologue(CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
            {
                base.WriteMethodPrologue(writer, metadataAccess);
                writer.WriteStatement(Emit.Assign("il2cpp_hresult_t " + this._hresult, Emit.Call("Initialize")));
                writer.WriteLine($"if (IL2CPP_HR_FAILED({this._hresult}))");
                using (new BlockWriter(writer, false))
                {
                    writer.WriteStatement(Emit.Assign('*' + InteropMethodInfo.Naming.ForComInterfaceReturnParameterName(), InteropMethodInfo.Naming.Null));
                    writer.WriteStatement("return " + this._hresult);
                }
            }
        }
    }
}

