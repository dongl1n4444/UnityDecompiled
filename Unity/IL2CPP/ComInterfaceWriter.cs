namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using Unity.IL2CPP.GenericsCollection;
    using Unity.IL2CPP.ILPreProcessor;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;
    using Unity.IL2CPP.Marshaling;

    public class ComInterfaceWriter
    {
        private readonly SourceCodeWriter _writer;
        [Inject]
        public static INamingService Naming;
        [Inject]
        public static IWindowsRuntimeProjections WindowsRuntimeProjections;

        public ComInterfaceWriter(SourceCodeWriter writer)
        {
            this._writer = writer;
        }

        public static string GetSignature(MethodReference method, MethodReference interfaceMethod, Unity.IL2CPP.ILPreProcessor.TypeResolver typeResolver, string typeName = null, bool isImplementation = true)
        {
            StringBuilder builder = new StringBuilder();
            MarshalType marshalType = !interfaceMethod.DeclaringType.Resolve().IsWindowsRuntime ? MarshalType.COM : MarshalType.WindowsRuntime;
            if (string.IsNullOrEmpty(typeName))
            {
                builder.Append("virtual il2cpp_hresult_t STDCALL ");
            }
            else
            {
                builder.Append("il2cpp_hresult_t ");
                builder.Append(typeName);
                builder.Append("::");
            }
            builder.Append(Naming.ForMethodNameOnly(interfaceMethod));
            builder.Append('(');
            builder.Append(MethodSignatureWriter.FormatComMethodParameterList(method, interfaceMethod, typeResolver, marshalType, true));
            builder.Append(')');
            if (string.IsNullOrEmpty(typeName) && isImplementation)
            {
                builder.Append(" IL2CPP_OVERRIDE");
            }
            return builder.ToString();
        }

        public void WriteComInterfaceFor(TypeReference type, ReadOnlyInflatedCollectionCollector genericsCollectionCollector, IInteropDataCollector interopDataCollector)
        {
            this._writer.WriteCommentedLine(type.FullName);
            this.WriteForwardDeclarations(type);
            string str = !type.Resolve().IsWindowsRuntime ? "Il2CppIUnknown" : "Il2CppIInspectable";
            object[] args = new object[] { Naming.ForTypeNameOnly(type), str };
            this._writer.WriteLine("struct NOVTABLE {0} : {1}", args);
            using (new BlockWriter(this._writer, true))
            {
                this._writer.WriteStatement("static const Il2CppGuid IID");
                interopDataCollector.AddGuid(type);
                TypeReference reference = WindowsRuntimeProjections.ProjectToCLR(type);
                if ((reference != type) && reference.IsInterface())
                {
                    interopDataCollector.AddGuid(reference);
                }
                Unity.IL2CPP.ILPreProcessor.TypeResolver typeResolver = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(type);
                foreach (MethodDefinition definition in type.Resolve().Methods)
                {
                    MethodReference method = typeResolver.Resolve(definition);
                    this._writer.Write(GetSignature(method, method, typeResolver, null, false));
                    this._writer.WriteLine(" = 0;");
                }
            }
        }

        private void WriteForwardDeclarations(TypeReference type)
        {
            Unity.IL2CPP.ILPreProcessor.TypeResolver resolver = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(type);
            MarshalType marshalType = !type.Resolve().IsWindowsRuntime ? MarshalType.COM : MarshalType.WindowsRuntime;
            foreach (MethodDefinition definition in type.Resolve().Methods)
            {
                foreach (ParameterDefinition definition2 in definition.Parameters)
                {
                    MarshalDataCollector.MarshalInfoWriterFor(resolver.Resolve(definition2.ParameterType), marshalType, definition2.MarshalInfo, true, false, false, null).WriteIncludesForFieldDeclaration(this._writer);
                }
                if (definition.ReturnType.MetadataType != MetadataType.Void)
                {
                    MarshalDataCollector.MarshalInfoWriterFor(resolver.Resolve(definition.ReturnType), marshalType, definition.MethodReturnType.MarshalInfo, true, false, false, null).WriteIncludesForFieldDeclaration(this._writer);
                }
            }
        }
    }
}

