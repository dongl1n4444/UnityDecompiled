namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;
    using System.Runtime.InteropServices;
    using Unity.IL2CPP.ILPreProcessor;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;
    using Unity.IL2CPP.Metadata;

    public sealed class DefaultRuntimeMetadataAccess : IRuntimeMetadataAccess
    {
        private readonly MetadataUsage _metadataUsage;
        private readonly MethodUsage _methodUsage;
        private readonly IMethodVerifier _methodVerifier;
        private readonly Unity.IL2CPP.ILPreProcessor.TypeResolver _typeResolver;
        [Inject]
        public static IIl2CppGenericMethodCollectorWriterService Il2CppGenericMethodCollector;
        [Inject]
        public static INamingService Naming;

        public DefaultRuntimeMetadataAccess(MethodReference methodReference, MetadataUsage metadataUsage, MethodUsage methodUsage, IMethodVerifier methodVerifier = null)
        {
            this._metadataUsage = metadataUsage;
            this._methodUsage = methodUsage;
            this._methodVerifier = methodVerifier;
            if (methodReference != null)
            {
                this._typeResolver = new Unity.IL2CPP.ILPreProcessor.TypeResolver(methodReference.DeclaringType as GenericInstanceType, methodReference as GenericInstanceMethod);
            }
            else
            {
                this._typeResolver = Unity.IL2CPP.ILPreProcessor.TypeResolver.Empty;
            }
        }

        public string ArrayInfo(TypeReference elementType)
        {
            ArrayType type = new ArrayType(this._typeResolver.Resolve(elementType));
            this._metadataUsage.AddTypeInfo(type);
            return Naming.ForRuntimeTypeInfo(type);
        }

        public string FieldInfo(FieldReference field)
        {
            FieldReference reference = this._typeResolver.Resolve(field);
            this._metadataUsage.AddFieldInfo(reference);
            return Naming.ForRuntimeFieldInfo(reference);
        }

        public string HiddenMethodInfo(MethodReference method)
        {
            MethodReference reference = this._typeResolver.Resolve(method);
            if (method.IsGenericInstance || method.DeclaringType.IsGenericInstance)
            {
                Il2CppGenericMethodCollector.Add(reference);
                this._metadataUsage.AddInflatedMethod(reference);
                return Naming.ForRuntimeMethodInfo(reference);
            }
            return Naming.Null;
        }

        public string Il2CppTypeFor(TypeReference type)
        {
            TypeReference reference = this._typeResolver.Resolve(type, false);
            this._metadataUsage.AddIl2CppType(reference);
            MetadataWriter.TypeRepositoryTypeFor(reference, 0);
            return Naming.ForRuntimeIl2CppType(reference);
        }

        public string Method(MethodReference method)
        {
            if ((!method.IsGenericInstance && !method.DeclaringType.IsGenericInstance) && ((this._methodVerifier != null) && !this._methodVerifier.MethodExists(method)))
            {
                MethodReference reference = method;
                method = method.Resolve();
                if (!this._methodVerifier.MethodExists(method))
                {
                    throw new InvalidOperationException($"attempting to call method '{reference.FullName}' that does not exist");
                }
            }
            MethodReference reference2 = this._typeResolver.Resolve(method);
            this._methodUsage.AddMethod(reference2);
            return Naming.ForMethod(reference2);
        }

        public string MethodInfo(MethodReference method)
        {
            MethodReference reference = this._typeResolver.Resolve(method);
            if (method.IsGenericInstance || method.DeclaringType.IsGenericInstance)
            {
                Il2CppGenericMethodCollector.Add(reference);
            }
            this._metadataUsage.AddInflatedMethod(reference);
            return Naming.ForRuntimeMethodInfo(reference);
        }

        public bool NeedsBoxingForValueTypeThis(MethodReference method) => 
            false;

        public string Newobj(MethodReference ctor)
        {
            TypeReference type = this._typeResolver.Resolve(ctor.DeclaringType);
            this._metadataUsage.AddTypeInfo(type);
            return Naming.ForRuntimeTypeInfo(type);
        }

        public string SizeOf(TypeReference type)
        {
            TypeReference reference = this._typeResolver.Resolve(type);
            this._metadataUsage.AddTypeInfo(reference);
            return Emit.Call("il2cpp_codegen_sizeof", Naming.ForRuntimeTypeInfo(reference));
        }

        public string StaticData(TypeReference type)
        {
            TypeReference reference = this._typeResolver.Resolve(type);
            this._metadataUsage.AddTypeInfo(reference);
            return Naming.ForRuntimeTypeInfo(reference);
        }

        public string StringLiteral(string literal, MetadataToken token, AssemblyDefinition assemblyDefinition)
        {
            if (literal == null)
            {
                return Naming.Null;
            }
            this._metadataUsage.AddStringLiteral(literal, assemblyDefinition, token);
            return Naming.ForStringLiteralIdentifier(literal);
        }

        public string TypeInfoFor(TypeReference type) => 
            this.UnresolvedTypeInfoFor(this._typeResolver.Resolve(type));

        public string UnresolvedTypeInfoFor(TypeReference type)
        {
            this._metadataUsage.AddTypeInfo(type);
            return Naming.ForRuntimeTypeInfo(type);
        }
    }
}

