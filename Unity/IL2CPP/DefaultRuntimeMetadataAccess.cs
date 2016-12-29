namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;
    using Unity.IL2CPP.ILPreProcessor;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;
    using Unity.IL2CPP.Metadata;

    public sealed class DefaultRuntimeMetadataAccess : IRuntimeMetadataAccess
    {
        private readonly MetadataUsage _metadataUsage;
        private readonly MethodUsage _methodUsage;
        private readonly Unity.IL2CPP.ILPreProcessor.TypeResolver _typeResolver;
        [Inject]
        public static IIl2CppGenericMethodCollectorWriterService Il2CppGenericMethodCollector;
        [Inject]
        public static INamingService Naming;

        public DefaultRuntimeMetadataAccess(MethodReference methodReference, MetadataUsage metadataUsage, MethodUsage methodUsage)
        {
            this._metadataUsage = metadataUsage;
            this._methodUsage = methodUsage;
            if (methodReference != null)
            {
                this._typeResolver = new Unity.IL2CPP.ILPreProcessor.TypeResolver(methodReference.DeclaringType as GenericInstanceType, methodReference as GenericInstanceMethod);
            }
            else
            {
                this._typeResolver = new Unity.IL2CPP.ILPreProcessor.TypeResolver();
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

        public string Method(MethodReference genericMethod)
        {
            this._methodUsage.AddMethod(genericMethod);
            return Naming.ForMethod(this._typeResolver.Resolve(genericMethod));
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
            TypeReference variableType = this._typeResolver.Resolve(type);
            return $"sizeof({Naming.ForVariable(variableType)})";
        }

        public string StaticData(TypeReference type)
        {
            TypeReference reference = this._typeResolver.Resolve(type);
            this._metadataUsage.AddTypeInfo(reference);
            return Naming.ForRuntimeTypeInfo(reference);
        }

        public string StringLiteral(string literal)
        {
            if (literal == null)
            {
                return Naming.Null;
            }
            this._metadataUsage.AddStringLiteral(literal);
            return Naming.ForStringLiteralIdentifier(literal);
        }

        public string TypeInfoFor(TypeReference type)
        {
            TypeReference reference = this._typeResolver.Resolve(type);
            this._metadataUsage.AddTypeInfo(reference);
            return Naming.ForRuntimeTypeInfo(reference);
        }
    }
}

