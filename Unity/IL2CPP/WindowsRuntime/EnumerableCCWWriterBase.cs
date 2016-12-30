namespace Unity.IL2CPP.WindowsRuntime
{
    using Mono.Cecil;
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP;
    using Unity.IL2CPP.ILPreProcessor;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;
    using Unity.IL2CPP.Marshaling.BodyWriters;
    using Unity.IL2CPP.Marshaling.BodyWriters.NativeToManaged;

    internal abstract class EnumerableCCWWriterBase
    {
        private readonly MethodReference _firstMethod;
        private readonly string _firstMethodName;
        private readonly TypeReference _iEnumerableType;
        private readonly TypeReference _returnType;
        private readonly string _returnValue;
        private readonly TypeReference _type;
        private readonly Unity.IL2CPP.ILPreProcessor.TypeResolver _typeResolver;
        [CompilerGenerated]
        private static Func<MethodReference, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<MethodReference, bool> <>f__am$cache1;
        [Inject]
        public static INamingService Naming;
        [Inject]
        public static ITypeProviderService TypeProvider;
        [Inject]
        public static IWindowsRuntimeProjections WindowsRuntimeProjections;

        protected EnumerableCCWWriterBase(TypeReference type)
        {
            this._type = type;
            this._iEnumerableType = WindowsRuntimeProjections.ProjectToCLR(this._type);
            this._firstMethodName = Naming.ForComCallableWrapperClass(this._type) + "_First";
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<MethodReference, bool>(EnumerableCCWWriterBase.<EnumerableCCWWriterBase>m__0);
            }
            this._firstMethod = this._type.GetMethods().First<MethodReference>(<>f__am$cache0);
            this._typeResolver = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(this._type);
            this._returnType = this._typeResolver.Resolve(this._firstMethod.ReturnType);
            this._returnValue = Naming.ForComInterfaceReturnParameterName();
        }

        [CompilerGenerated]
        private static bool <EnumerableCCWWriterBase>m__0(MethodReference m) => 
            (m.Name == "First");

        protected bool HasIEnumerable() => 
            (this._iEnumerableType != null);

        protected void WriteMethodDefinitions(CppCodeWriter writer)
        {
            writer.AddIncludeForTypeDefinition(this._type);
            writer.WriteCommentedLine(this._type.FullName + " COM callable wrapper");
            string methodSignature = $"il2cpp_hresult_t {this._firstMethodName}(Il2CppObject* obj, {Naming.ForTypeNameOnly(this._returnType)}** {this._returnValue})";
            MethodWriter.WriteMethodWithMetadataInitialization(writer, methodSignature, this._firstMethodName, delegate (CppCodeWriter bodyWriter, MetadataUsage metadataUsage, MethodUsage methodUsage) {
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = m => m.Name == "GetEnumerator";
                }
                MethodReference method = this._iEnumerableType.GetMethods().First<MethodReference>(<>f__am$cache1);
                IRuntimeMetadataAccess metadataAccess = MethodWriter.GetDefaultRuntimeMetadataAccess(method, metadataUsage, methodUsage);
                new FirstMethodBodyWriter(method, this._firstMethod, Naming.ForWindowsRuntimeAdapterClass(this._returnType)).WriteMethodBody(bodyWriter, metadataAccess);
            }, this._firstMethodName);
        }

        private void WriteMethodFirst(CppCodeWriter writer)
        {
            string str = Emit.Call("static_cast<CCW*>(this)->GetManagedObjectInline");
            writer.WriteStatement("return " + Emit.Call(this._firstMethodName, str, this._returnValue));
        }

        protected void WriteTypeDefinition(CppCodeWriter writer)
        {
            writer.AddForwardDeclaration(this._returnType);
            writer.WriteCommentedLine("COM callable wrapper");
            writer.WriteLine($"il2cpp_hresult_t {this._firstMethodName}(Il2CppObject* obj, {Naming.ForTypeNameOnly(this._returnType)}** {this._returnValue});");
            writer.WriteLine("template <typename CCW>");
            writer.WriteLine($"struct NOVTABLE {Naming.ForComCallableWrapperClass(this._type)} : {Naming.ForTypeNameOnly(this._type)}");
            using (new BlockWriter(writer, true))
            {
                foreach (MethodReference reference in this._type.GetMethods())
                {
                    string block = ComInterfaceWriter.GetSignature(reference, reference, this._typeResolver, null, true);
                    writer.WriteLine(block);
                    using (new BlockWriter(writer, false))
                    {
                        string name = reference.Name;
                        if ((name == null) || (name != "First"))
                        {
                            throw new NotSupportedException($"Interface '{this._type.FullName}' contains unsupported method '_{reference.Name}'.");
                        }
                        this.WriteMethodFirst(writer);
                    }
                }
            }
        }

        private sealed class FirstMethodBodyWriter : ComCallableWrapperMethodBodyWriter
        {
            private readonly string _adapterTypeName;

            public FirstMethodBodyWriter(MethodReference getEnumeratorMethod, MethodReference firstMethod, string adapterTypeName) : base(getEnumeratorMethod, firstMethod, MarshalType.WindowsRuntime)
            {
                this._adapterTypeName = adapterTypeName;
            }

            protected override void WriteReturnStatementEpilogue(CppCodeWriter writer, string unmarshaledReturnValueVariableName)
            {
                string str = Emit.Call(this._adapterTypeName + "::__CreateInstance", unmarshaledReturnValueVariableName);
                writer.WriteStatement(Emit.Assign('*' + InteropMethodInfo.Naming.ForComInterfaceReturnParameterName(), $"({unmarshaledReturnValueVariableName} != NULL) ? {str} : {InteropMethodInfo.Naming.Null}"));
                writer.WriteStatement("return IL2CPP_S_OK");
            }

            protected override bool IsReturnValueMarshaled =>
                false;

            protected override string ManagedObjectExpression =>
                "obj";
        }
    }
}

