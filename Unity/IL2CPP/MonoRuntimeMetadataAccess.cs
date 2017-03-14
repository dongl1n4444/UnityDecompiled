namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;
    using Unity.IL2CPP.ILPreProcessor;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;

    public sealed class MonoRuntimeMetadataAccess : IRuntimeMetadataAccess
    {
        private readonly DefaultRuntimeMetadataAccess _defaultRuntimeMetadataAccess;
        private readonly MetadataUsage _metadataUsage;
        private readonly Unity.IL2CPP.ILPreProcessor.TypeResolver _typeResolver;
        [Inject]
        public static IIl2CppGenericMethodCollectorWriterService Il2CppGenericMethodCollector;
        [Inject]
        public static IIl2CppTypeCollectorWriterService Il2CppTypeCollector;
        [Inject]
        public static INamingService Naming;
        [Inject]
        public static ITypeProviderService TypeProvider;

        public MonoRuntimeMetadataAccess(DefaultRuntimeMetadataAccess defaultRuntimeMetadataAccess, MethodReference methodReference, MetadataUsage metadataUsage, MethodUsage methodUsage)
        {
            this._defaultRuntimeMetadataAccess = defaultRuntimeMetadataAccess;
            this._metadataUsage = metadataUsage;
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
            return this._defaultRuntimeMetadataAccess.ArrayInfo(elementType);
        }

        public string FieldInfo(FieldReference field)
        {
            FieldReference reference = this._typeResolver.Resolve(field);
            this._metadataUsage.AddFieldInfo(reference);
            return this._defaultRuntimeMetadataAccess.FieldInfo(field);
        }

        public string HiddenMethodInfo(MethodReference method)
        {
            MethodReference reference = this._typeResolver.Resolve(method);
            if (method.IsGenericInstance || method.DeclaringType.IsGenericInstance)
            {
                Il2CppGenericMethodCollector.Add(reference);
                this._metadataUsage.AddInflatedMethod(reference);
            }
            return ("(const MethodInfo*)" + this._defaultRuntimeMetadataAccess.HiddenMethodInfo(reference));
        }

        public string Il2CppTypeFor(TypeReference type)
        {
            TypeReference reference = this._typeResolver.Resolve(type, false);
            this._metadataUsage.AddIl2CppType(reference);
            return this._defaultRuntimeMetadataAccess.Il2CppTypeFor(type);
        }

        public string Method(MethodReference genericMethod) => 
            this._defaultRuntimeMetadataAccess.Method(genericMethod);

        public string MethodInfo(MethodReference method) => 
            this._defaultRuntimeMetadataAccess.MethodInfo(method);

        public bool NeedsBoxingForValueTypeThis(MethodReference method) => 
            this._defaultRuntimeMetadataAccess.NeedsBoxingForValueTypeThis(method);

        public string Newobj(MethodReference ctor)
        {
            TypeReference type = this._typeResolver.Resolve(ctor.DeclaringType);
            this._metadataUsage.AddTypeInfo(type);
            Il2CppTypeCollector.Add(type, 0);
            return this._defaultRuntimeMetadataAccess.Newobj(ctor);
        }

        public string SizeOf(TypeReference type) => 
            this._defaultRuntimeMetadataAccess.SizeOf(type);

        public string StaticData(TypeReference type)
        {
            TypeReference reference = this._typeResolver.Resolve(type);
            this._metadataUsage.AddTypeInfo(reference);
            Il2CppTypeCollector.Add(reference, 0);
            return this._defaultRuntimeMetadataAccess.StaticData(type);
        }

        public string StringLiteral(string literal, MetadataToken token, AssemblyDefinition assemblyDefinition)
        {
            if (literal == null)
            {
                return Naming.Null;
            }
            this._metadataUsage.AddStringLiteral(literal, assemblyDefinition, token);
            return this._defaultRuntimeMetadataAccess.StringLiteral(literal, token, assemblyDefinition);
        }

        public string TypeInfoFor(TypeReference type)
        {
            TypeReference reference = this._typeResolver.Resolve(type);
            this._metadataUsage.AddTypeInfo(reference);
            Il2CppTypeCollector.Add(reference, 0);
            return this._defaultRuntimeMetadataAccess.TypeInfoFor(type);
        }

        public string UnresolvedTypeInfoFor(TypeReference type) => 
            this._defaultRuntimeMetadataAccess.UnresolvedTypeInfoFor(type);
    }
}

