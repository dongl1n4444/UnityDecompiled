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

    internal sealed class EnumerableCCWWriter : IProjectedComCallableWrapperMethodWriter
    {
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache0;
        [Inject]
        public static IWindowsRuntimeProjections WindowsRuntimeProjections;

        public ComCallableWrapperMethodBodyWriter GetBodyWriter(MethodReference method)
        {
            TypeReference declaringType = method.DeclaringType;
            Unity.IL2CPP.ILPreProcessor.TypeResolver resolver = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(declaringType);
            TypeReference typeReference = WindowsRuntimeProjections.ProjectToCLR(declaringType);
            Unity.IL2CPP.ILPreProcessor.TypeResolver resolver2 = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(typeReference);
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = m => m.Name == "GetEnumerator";
            }
            MethodDefinition definition = typeReference.Resolve().Methods.First<MethodDefinition>(<>f__am$cache0);
            MethodReference getEnumeratorMethod = resolver2.Resolve(definition);
            return new FirstMethodBodyWriter(getEnumeratorMethod, method, resolver.Resolve(method.ReturnType));
        }

        public void WriteDependenciesFor(CppCodeWriter writer, TypeReference interfaceType)
        {
            if (interfaceType.Resolve().HasGenericParameters)
            {
                GenericEnumeratorToIteratorAdapterWriter.WriteDefinitions(writer, (GenericInstanceType) interfaceType);
            }
            else
            {
                EnumeratorToBindableIteratorAdapterWriter.WriteDefinitions(writer);
            }
        }

        private sealed class FirstMethodBodyWriter : ProjectedMethodBodyWriter
        {
            private readonly string _adapterTypeName;

            public FirstMethodBodyWriter(MethodReference getEnumeratorMethod, MethodReference firstMethod, TypeReference iteratorType) : base(getEnumeratorMethod, firstMethod)
            {
                this._adapterTypeName = InteropMethodInfo.Naming.ForWindowsRuntimeAdapterClass(iteratorType);
            }

            protected override void WriteReturnStatementEpilogue(CppCodeWriter writer, string unmarshaledReturnValueVariableName)
            {
                string str = Emit.Call(this._adapterTypeName + "::__CreateInstance", unmarshaledReturnValueVariableName);
                writer.WriteStatement(Emit.Assign('*' + InteropMethodInfo.Naming.ForComInterfaceReturnParameterName(), $"({unmarshaledReturnValueVariableName} != NULL) ? {str} : {InteropMethodInfo.Naming.Null}"));
                writer.WriteStatement("return IL2CPP_S_OK");
            }

            protected override bool IsReturnValueMarshaled =>
                false;
        }
    }
}

