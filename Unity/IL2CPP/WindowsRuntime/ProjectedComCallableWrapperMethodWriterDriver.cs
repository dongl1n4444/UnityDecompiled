namespace Unity.IL2CPP.WindowsRuntime
{
    using Mono.Cecil;
    using System;
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP;
    using Unity.IL2CPP.ILPreProcessor;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;
    using Unity.IL2CPP.Marshaling;
    using Unity.IL2CPP.Marshaling.BodyWriters.NativeToManaged;

    internal static class ProjectedComCallableWrapperMethodWriterDriver
    {
        [Inject]
        public static INamingService Naming;
        [Inject]
        public static ITypeProviderService TypeProvider;
        [Inject]
        public static IWindowsRuntimeProjections WindowsRuntimeProjections;

        public static void WriteFor(CppCodeWriter writer, TypeReference interfaceType)
        {
            Unity.IL2CPP.ILPreProcessor.TypeResolver typeResolver = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(interfaceType);
            TypeDefinition type = interfaceType.Resolve();
            IProjectedComCallableWrapperMethodWriter projectedComCallableWrapperMethodWriterFor = WindowsRuntimeProjections.GetProjectedComCallableWrapperMethodWriterFor(type);
            if (projectedComCallableWrapperMethodWriterFor != null)
            {
                projectedComCallableWrapperMethodWriterFor.WriteDependenciesFor(writer, interfaceType);
            }
            writer.AddIncludeForTypeDefinition(interfaceType);
            foreach (MethodDefinition definition2 in type.Methods)
            {
                <WriteFor>c__AnonStorey0 storey = new <WriteFor>c__AnonStorey0 {
                    method = typeResolver.Resolve(definition2)
                };
                ComCallableWrapperMethodBodyWriter bodyWriter = projectedComCallableWrapperMethodWriterFor?.GetBodyWriter(storey.method);
                if (bodyWriter == null)
                {
                }
                storey.methodBodyWriter = new NotSupportedMethodBodyWriter(storey.method);
                writer.WriteCommentedLine($"Projected COM callable wrapper method for {storey.method.FullName}");
                string methodSignature = MethodSignatureWriter.FormatProjectedComCallableWrapperMethodDeclaration(storey.method, typeResolver, MarshalType.WindowsRuntime);
                string methodFullName = Naming.ForComCallableWrapperProjectedMethod(storey.method);
                MethodWriter.WriteMethodWithMetadataInitialization(writer, methodSignature, methodFullName, new Action<CppCodeWriter, MetadataUsage, MethodUsage>(storey.<>m__0), methodFullName);
            }
        }

        [CompilerGenerated]
        private sealed class <WriteFor>c__AnonStorey0
        {
            internal MethodReference method;
            internal ComCallableWrapperMethodBodyWriter methodBodyWriter;

            internal void <>m__0(CppCodeWriter bodyWriter, MetadataUsage metadataUsage, MethodUsage methodUsage)
            {
                IRuntimeMetadataAccess metadataAccess = MethodWriter.GetDefaultRuntimeMetadataAccess(this.method, metadataUsage, methodUsage, null);
                this.methodBodyWriter.WriteMethodBody(bodyWriter, metadataAccess);
            }
        }

        private sealed class NotSupportedMethodBodyWriter : ComCallableWrapperMethodBodyWriter
        {
            public NotSupportedMethodBodyWriter(MethodReference method) : base(method, method, MarshalType.WindowsRuntime)
            {
            }

            protected override void WriteInteropCallStatementWithinTryBlock(CppCodeWriter writer, string[] localVariableNames, IRuntimeMetadataAccess metadataAccess)
            {
                string str = $"Cannot call method '{this.InteropMethod.FullName}' from native code. IL2CPP does not yet support calling this projected method.";
                writer.WriteStatement(Emit.RaiseManagedException($"il2cpp_codegen_get_not_supported_exception("{str}")"));
            }
        }
    }
}

