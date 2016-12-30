namespace Unity.IL2CPP.Marshaling.BodyWriters.ManagedToNative.WindowsRuntimeProjection
{
    using Mono.Cecil;
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;

    internal static class IEnumerableMethodBodyWriter
    {
        private const string _adapterVariableName = "adapter";
        private const string _iteratorVariableName = "iterator";
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache1;
        [Inject]
        public static INamingService Naming;
        [Inject]
        public static ITypeProviderService TypeProvider;
        [Inject]
        public static IWindowsRuntimeProjections WindowsRuntimeProjections;

        public static void WriteGetEnumerator(MethodDefinition method, CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
        {
            TypeDefinition typeReference = WindowsRuntimeProjections.ProjectToWindowsRuntime(method.Overrides.First<MethodReference>().DeclaringType.Resolve());
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = m => m.Name == "First";
            }
            MethodDefinition definition2 = typeReference.Methods.First<MethodDefinition>(<>f__am$cache0);
            writer.AddIncludesForTypeReference(typeReference, false);
            writer.AddIncludeForMethodDeclarations(typeReference);
            TypeDefinition definition3 = TypeProvider.BindableIteratorToEnumeratorAdapterTypeReference.Resolve();
            writer.AddIncludeForTypeDefinition(definition3);
            writer.AddIncludeForMethodDeclarations(definition3);
            string right = Emit.Call(Naming.ForMethodNameOnly(definition2), Naming.ThisParameterName, (!CodeGenOptions.EmitComments ? string.Empty : "/*hidden argument*/") + metadataAccess.HiddenMethodInfo(definition2));
            writer.WriteStatement(Emit.Assign(Naming.ForVariable(typeReference) + ' ' + "iterator", right));
            writer.WriteLine($"if ({"iterator"} == {Naming.Null})");
            using (new BlockWriter(writer, false))
            {
                writer.WriteStatement($"return {Naming.Null}");
            }
            writer.WriteStatement(Emit.Assign(Naming.ForVariable(definition3) + ' ' + "adapter", Emit.NewObj(definition3, metadataAccess)));
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = m => m.IsConstructor;
            }
            MethodDefinition definition4 = definition3.Methods.First<MethodDefinition>(<>f__am$cache1);
            writer.WriteStatement(Emit.Call(Naming.ForMethodNameOnly(definition4), "adapter", "iterator", (!CodeGenOptions.EmitComments ? string.Empty : "/*hidden argument*/") + metadataAccess.HiddenMethodInfo(definition4)));
            writer.WriteStatement($"return {"adapter"}");
        }
    }
}

