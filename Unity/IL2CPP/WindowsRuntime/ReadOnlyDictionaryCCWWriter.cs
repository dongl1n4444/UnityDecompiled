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

    internal sealed class ReadOnlyDictionaryCCWWriter : IProjectedComCallableWrapperMethodWriter
    {
        private readonly MethodDefinition _containsKeyMethodDef;
        private readonly MethodDefinition _getCountMethodDef;
        private readonly TypeReference _iReadOnlyCollectionTypeRef;
        private readonly MethodDefinition _tryGetValueMethodDef;
        [CompilerGenerated]
        private static Func<InterfaceImplementation, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache3;
        [Inject]
        public static IWindowsRuntimeProjections WindowsRuntimeProjections;

        public ReadOnlyDictionaryCCWWriter(TypeDefinition iReadOnlyDictionary)
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<InterfaceImplementation, bool>(ReadOnlyDictionaryCCWWriter.<ReadOnlyDictionaryCCWWriter>m__0);
            }
            this._iReadOnlyCollectionTypeRef = iReadOnlyDictionary.Interfaces.Single<InterfaceImplementation>(<>f__am$cache0).InterfaceType;
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = new Func<MethodDefinition, bool>(ReadOnlyDictionaryCCWWriter.<ReadOnlyDictionaryCCWWriter>m__1);
            }
            this._getCountMethodDef = this._iReadOnlyCollectionTypeRef.Resolve().Methods.Single<MethodDefinition>(<>f__am$cache1);
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = new Func<MethodDefinition, bool>(ReadOnlyDictionaryCCWWriter.<ReadOnlyDictionaryCCWWriter>m__2);
            }
            this._tryGetValueMethodDef = iReadOnlyDictionary.Methods.Single<MethodDefinition>(<>f__am$cache2);
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = new Func<MethodDefinition, bool>(ReadOnlyDictionaryCCWWriter.<ReadOnlyDictionaryCCWWriter>m__3);
            }
            this._containsKeyMethodDef = iReadOnlyDictionary.Methods.Single<MethodDefinition>(<>f__am$cache3);
        }

        [CompilerGenerated]
        private static bool <ReadOnlyDictionaryCCWWriter>m__0(InterfaceImplementation t) => 
            (t.InterfaceType.Name == "IReadOnlyCollection`1");

        [CompilerGenerated]
        private static bool <ReadOnlyDictionaryCCWWriter>m__1(MethodDefinition m) => 
            (m.Name == "get_Count");

        [CompilerGenerated]
        private static bool <ReadOnlyDictionaryCCWWriter>m__2(MethodDefinition m) => 
            (m.Name == "TryGetValue");

        [CompilerGenerated]
        private static bool <ReadOnlyDictionaryCCWWriter>m__3(MethodDefinition m) => 
            (m.Name == "ContainsKey");

        public ComCallableWrapperMethodBodyWriter GetBodyWriter(MethodReference method)
        {
            TypeReference declaringType = method.DeclaringType;
            Unity.IL2CPP.ILPreProcessor.TypeResolver iReadOnlyDictionaryTypeResolver = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(WindowsRuntimeProjections.ProjectToCLR(declaringType));
            Unity.IL2CPP.ILPreProcessor.TypeResolver resolver2 = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(iReadOnlyDictionaryTypeResolver.Resolve(this._iReadOnlyCollectionTypeRef));
            switch (method.Name)
            {
                case "get_Size":
                    return new ProjectedMethodBodyWriter(resolver2.Resolve(this._getCountMethodDef), method);

                case "Lookup":
                    return new LookupMethodBodyWriter(iReadOnlyDictionaryTypeResolver.Resolve(this._tryGetValueMethodDef), method);

                case "HasKey":
                    return new ProjectedMethodBodyWriter(iReadOnlyDictionaryTypeResolver.Resolve(this._containsKeyMethodDef), method);

                case "Split":
                    return new SplitMethodBodyWriter(this, iReadOnlyDictionaryTypeResolver, method);
            }
            throw new NotSupportedException($"ReadOnlyDictionaryCCWWriter does not support writing method body for {method.FullName}.");
        }

        public void WriteDependenciesFor(CppCodeWriter writer, TypeReference interfaceType)
        {
        }

        private class LookupMethodBodyWriter : ProjectedMethodBodyWriter
        {
            private readonly MethodReference _tryGetValueMethod;
            [CompilerGenerated]
            private static Func<MethodDefinition, bool> <>f__am$cache0;
            [CompilerGenerated]
            private static Func<PropertyDefinition, bool> <>f__am$cache1;

            public LookupMethodBodyWriter(MethodReference tryGetValueMethod, MethodReference lookupMethod) : base(lookupMethod, lookupMethod)
            {
                this._tryGetValueMethod = tryGetValueMethod;
            }

            protected override void WriteInteropCallStatementWithinTryBlock(CppCodeWriter writer, string[] localVariableNames, IRuntimeMetadataAccess metadataAccess)
            {
                TypeDefinition typeReference = InteropMethodBodyWriter.TypeProvider.OptionalResolve("System.Collections.Generic", "KeyNotFoundException", InteropMethodBodyWriter.TypeProvider.Corlib.Name);
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = m => ((m.HasThis && m.IsConstructor) && (m.Parameters.Count == 1)) && (m.Parameters[0].ParameterType.MetadataType == MetadataType.String);
                }
                MethodDefinition method = typeReference.Methods.Single<MethodDefinition>(<>f__am$cache0);
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = p => p.Name == "HResult";
                }
                PropertyDefinition definition3 = InteropMethodBodyWriter.TypeProvider.SystemException.Properties.Single<PropertyDefinition>(<>f__am$cache1);
                writer.AddIncludeForTypeDefinition(typeReference);
                writer.AddIncludeForMethodDeclaration(method);
                writer.AddIncludeForMethodDeclaration(definition3.SetMethod);
                string[] args = new string[] { localVariableNames[0], InteropMethodInfo.Naming.AddressOf(InteropMethodInfo.Naming.ForInteropReturnValue()) };
                string str = base.GetMethodCallExpression(metadataAccess, this.ManagedObjectExpression, this._tryGetValueMethod, MethodCallType.Virtual, args);
                writer.WriteLine($"bool keyFound = {str};");
                writer.WriteLine();
                writer.WriteLine("if (!keyFound)");
                using (new BlockWriter(writer, false))
                {
                    string str2 = InteropMethodInfo.Naming.ForVariable(typeReference);
                    writer.WriteLine($"{str2} e = {Emit.NewObj(typeReference, metadataAccess)};");
                    string[] textArray2 = new string[] { metadataAccess.StringLiteral("The given key was not present in the dictionary.") };
                    writer.WriteStatement(base.GetMethodCallExpression(metadataAccess, "e", method, MethodCallType.Normal, textArray2));
                    string[] textArray3 = new string[] { -2147483637.ToString() };
                    writer.WriteLine($"{base.GetMethodCallExpression(metadataAccess, "e", definition3.SetMethod, MethodCallType.Normal, textArray3)}; // E_BOUNDS");
                    writer.WriteStatement(Emit.RaiseManagedException("e"));
                }
            }
        }

        private class SplitMethodBodyWriter : ProjectedMethodBodyWriter
        {
            private readonly Unity.IL2CPP.ILPreProcessor.TypeResolver _iReadOnlyDictionaryTypeResolver;
            private readonly ReadOnlyDictionaryCCWWriter _parent;
            [CompilerGenerated]
            private static Func<MethodDefinition, bool> <>f__am$cache0;
            [CompilerGenerated]
            private static Func<MethodDefinition, bool> <>f__am$cache1;

            public SplitMethodBodyWriter(ReadOnlyDictionaryCCWWriter parent, Unity.IL2CPP.ILPreProcessor.TypeResolver iReadOnlyDictionaryTypeResolver, MethodReference method) : base(method, method)
            {
                this._parent = parent;
                this._iReadOnlyDictionaryTypeResolver = iReadOnlyDictionaryTypeResolver;
            }

            protected override void WriteInteropCallStatementWithinTryBlock(CppCodeWriter writer, string[] localVariableNames, IRuntimeMetadataAccess metadataAccess)
            {
                if (InteropMethodBodyWriter.TypeProvider.ConstantSplittableMapType == null)
                {
                    string str = $"Cannot call method '{this.InteropMethod.FullName}' from native code. It requires type System.Runtime.InteropServices.WindowsRuntime.ConstantSplittableMap`2<K, V> to be present. Was it incorrectly stripped?";
                    writer.WriteStatement(Emit.RaiseManagedException($"il2cpp_codegen_get_not_supported_exception("{str}")"));
                }
                else
                {
                    if (!InteropMethodBodyWriter.TypeProvider.ConstantSplittableMapType.IsSealed)
                    {
                        throw new InvalidProgramException("System.Runtime.InteropServices.WindowsRuntime.ConstantSplittableMap`2 was not sealed. Was System.Runtime.WindowsRuntime.dll modified unexpectedly?");
                    }
                    TypeReference typeReference = this._iReadOnlyDictionaryTypeResolver.Resolve(InteropMethodBodyWriter.TypeProvider.ConstantSplittableMapType);
                    Unity.IL2CPP.ILPreProcessor.TypeResolver resolver = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(typeReference);
                    if (<>f__am$cache0 == null)
                    {
                        <>f__am$cache0 = m => (m.HasThis && m.IsConstructor) && (m.Parameters.Count == 1);
                    }
                    MethodDefinition method = InteropMethodBodyWriter.TypeProvider.ConstantSplittableMapType.Methods.Single<MethodDefinition>(<>f__am$cache0);
                    MethodReference reference2 = resolver.Resolve(method);
                    if (<>f__am$cache1 == null)
                    {
                        <>f__am$cache1 = m => m.HasThis && (m.Name == "Split");
                    }
                    MethodDefinition definition2 = InteropMethodBodyWriter.TypeProvider.ConstantSplittableMapType.Methods.Single<MethodDefinition>(<>f__am$cache1);
                    MethodReference reference3 = resolver.Resolve(definition2);
                    MethodReference reference4 = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(this._iReadOnlyDictionaryTypeResolver.Resolve(this._parent._iReadOnlyCollectionTypeRef)).Resolve(this._parent._getCountMethodDef);
                    writer.AddIncludeForTypeDefinition(typeReference);
                    writer.AddIncludeForMethodDeclaration(reference2);
                    writer.AddIncludeForMethodDeclaration(reference3);
                    writer.WriteLine($"int32_t itemsInCollection = {base.GetMethodCallExpression(metadataAccess, this.ManagedObjectExpression, reference4, MethodCallType.Virtual, new string[0])};");
                    writer.WriteLine("if (itemsInCollection > 1)");
                    using (new BlockWriter(writer, false))
                    {
                        string str2 = InteropMethodInfo.Naming.ForVariable(typeReference);
                        string str3 = $"IsInstSealed({this.ManagedObjectExpression}, {metadataAccess.TypeInfoFor(typeReference)})";
                        writer.WriteLine($"{str2} splittableMap = {Emit.Cast(str2, str3)};");
                        writer.WriteLine();
                        writer.WriteLine("if (splittableMap == NULL)");
                        using (new BlockWriter(writer, false))
                        {
                            writer.WriteLine($"splittableMap = {Emit.NewObj(typeReference, metadataAccess)};");
                            string[] textArray1 = new string[] { this.ManagedObjectExpression };
                            writer.WriteStatement(base.GetMethodCallExpression(metadataAccess, "splittableMap", reference2, MethodCallType.Normal, textArray1));
                        }
                        writer.WriteLine();
                        string[] args = new string[] { localVariableNames[0], localVariableNames[1] };
                        writer.WriteStatement(base.GetMethodCallExpression(metadataAccess, "splittableMap", reference3, MethodCallType.Normal, args));
                    }
                }
            }
        }
    }
}

