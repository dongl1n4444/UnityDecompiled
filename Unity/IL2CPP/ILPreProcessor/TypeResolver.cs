namespace Unity.IL2CPP.ILPreProcessor
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using System;
    using Unity.IL2CPP;

    public class TypeResolver
    {
        private static Unity.IL2CPP.ILPreProcessor.TypeResolver _empty;
        private readonly IGenericInstance _methodDefinitionContext;
        private readonly IGenericInstance _typeDefinitionContext;

        private TypeResolver()
        {
        }

        public TypeResolver(GenericInstanceMethod methodDefinitionContext)
        {
            this._methodDefinitionContext = methodDefinitionContext;
        }

        public TypeResolver(GenericInstanceType typeDefinitionContext)
        {
            this._typeDefinitionContext = typeDefinitionContext;
        }

        public TypeResolver(GenericInstanceType typeDefinitionContext, GenericInstanceMethod methodDefinitionContext)
        {
            this._typeDefinitionContext = typeDefinitionContext;
            this._methodDefinitionContext = methodDefinitionContext;
        }

        public static Unity.IL2CPP.ILPreProcessor.TypeResolver For(TypeReference typeReference) => 
            (!typeReference.IsGenericInstance ? Empty : new Unity.IL2CPP.ILPreProcessor.TypeResolver((GenericInstanceType) typeReference));

        public static Unity.IL2CPP.ILPreProcessor.TypeResolver For(TypeReference typeReference, MethodReference methodReference) => 
            new Unity.IL2CPP.ILPreProcessor.TypeResolver(typeReference as GenericInstanceType, methodReference as GenericInstanceMethod);

        private TypeReference HandleOwnerlessInvalidILCode(GenericParameter genericParameter)
        {
            if (((genericParameter.Type == GenericParameterType.Method) && (this._typeDefinitionContext != null)) && (genericParameter.Position < this._typeDefinitionContext.GenericArguments.Count))
            {
                return this._typeDefinitionContext.GenericArguments[genericParameter.Position];
            }
            return genericParameter.Module.TypeSystem.Object;
        }

        private bool IsDummy() => 
            ((this._typeDefinitionContext == null) && (this._methodDefinitionContext == null));

        internal Unity.IL2CPP.ILPreProcessor.TypeResolver Nested(GenericInstanceMethod genericInstanceMethod) => 
            new Unity.IL2CPP.ILPreProcessor.TypeResolver(this._typeDefinitionContext as GenericInstanceType, genericInstanceMethod);

        public FieldReference Resolve(FieldReference field)
        {
            TypeReference declaringType = this.Resolve(field.DeclaringType);
            if (declaringType == field.DeclaringType)
            {
                return field;
            }
            return new FieldReference(field.Name, field.FieldType, declaringType);
        }

        public MethodReference Resolve(MethodReference method)
        {
            MethodReference owner = method;
            if (!this.IsDummy())
            {
                TypeReference declaringType = this.Resolve(method.DeclaringType);
                GenericInstanceMethod method2 = method as GenericInstanceMethod;
                if (method2 != null)
                {
                    owner = new MethodReference(method.Name, method.ReturnType, declaringType);
                    foreach (ParameterDefinition definition in method.Parameters)
                    {
                        owner.Parameters.Add(new ParameterDefinition(definition.Name, definition.Attributes, definition.ParameterType));
                    }
                    foreach (GenericParameter parameter in method2.ElementMethod.GenericParameters)
                    {
                        owner.GenericParameters.Add(new GenericParameter(parameter.Name, owner));
                    }
                    owner.HasThis = method.HasThis;
                    GenericInstanceMethod method3 = new GenericInstanceMethod(owner);
                    foreach (TypeReference reference4 in method2.GenericArguments)
                    {
                        method3.GenericArguments.Add(this.Resolve(reference4));
                    }
                    return method3;
                }
                owner = new MethodReference(method.Name, method.ReturnType, declaringType);
                foreach (GenericParameter parameter2 in method.GenericParameters)
                {
                    owner.GenericParameters.Add(new GenericParameter(parameter2.Name, owner));
                }
                foreach (ParameterDefinition definition2 in method.Parameters)
                {
                    owner.Parameters.Add(new ParameterDefinition(definition2.Name, definition2.Attributes, definition2.ParameterType));
                }
                owner.HasThis = method.HasThis;
                owner.MetadataToken = method.MetadataToken;
            }
            return owner;
        }

        public TypeReference Resolve(TypeReference typeReference) => 
            this.Resolve(typeReference, true);

        public TypeReference Resolve(TypeReference typeReference, bool includeTypeDefinitions)
        {
            if (!this.IsDummy())
            {
                if ((this._typeDefinitionContext != null) && this._typeDefinitionContext.GenericArguments.Contains(typeReference))
                {
                    return typeReference;
                }
                if ((this._methodDefinitionContext != null) && this._methodDefinitionContext.GenericArguments.Contains(typeReference))
                {
                    return typeReference;
                }
                GenericParameter item = typeReference as GenericParameter;
                if (item != null)
                {
                    if ((this._typeDefinitionContext != null) && this._typeDefinitionContext.GenericArguments.Contains(item))
                    {
                        return item;
                    }
                    if ((this._methodDefinitionContext != null) && this._methodDefinitionContext.GenericArguments.Contains(item))
                    {
                        return item;
                    }
                    return this.ResolveGenericParameter(item);
                }
                ArrayType type = typeReference as ArrayType;
                if (type != null)
                {
                    return new ArrayType(this.Resolve(type.ElementType), type.Rank);
                }
                PointerType type2 = typeReference as PointerType;
                if (type2 != null)
                {
                    return new PointerType(this.Resolve(type2.ElementType));
                }
                ByReferenceType type3 = typeReference as ByReferenceType;
                if (type3 != null)
                {
                    return new ByReferenceType(this.Resolve(type3.ElementType));
                }
                PinnedType type4 = typeReference as PinnedType;
                if (type4 != null)
                {
                    return new PinnedType(this.Resolve(type4.ElementType));
                }
                GenericInstanceType type5 = typeReference as GenericInstanceType;
                if (type5 != null)
                {
                    GenericInstanceType type6 = new GenericInstanceType(type5.ElementType);
                    foreach (TypeReference reference2 in type5.GenericArguments)
                    {
                        type6.GenericArguments.Add(this.Resolve(reference2));
                    }
                    type6.MetadataToken = type5.MetadataToken;
                    return type6;
                }
                RequiredModifierType type7 = typeReference as RequiredModifierType;
                if (type7 != null)
                {
                    return this.Resolve(type7.ElementType, includeTypeDefinitions);
                }
                if (includeTypeDefinitions)
                {
                    TypeDefinition definition = typeReference as TypeDefinition;
                    if ((definition != null) && definition.HasGenericParameters)
                    {
                        GenericInstanceType type8 = new GenericInstanceType(definition);
                        foreach (GenericParameter parameter2 in definition.GenericParameters)
                        {
                            type8.GenericArguments.Add(this.Resolve(parameter2));
                        }
                        return type8;
                    }
                }
                if (typeReference is TypeSpecification)
                {
                    throw new NotSupportedException($"The type {typeReference.FullName} cannot be resolved correctly.");
                }
            }
            return typeReference;
        }

        public TypeReference ResolveFieldType(FieldReference field) => 
            this.Resolve(Unity.IL2CPP.GenericParameterResolver.ResolveFieldTypeIfNeeded(field));

        private TypeReference ResolveGenericParameter(GenericParameter genericParameter)
        {
            if (genericParameter.Owner == null)
            {
                return this.HandleOwnerlessInvalidILCode(genericParameter);
            }
            if (!(genericParameter.Owner is MemberReference))
            {
                throw new NotSupportedException();
            }
            return ((genericParameter.Type != GenericParameterType.Type) ? ((this._methodDefinitionContext == null) ? genericParameter : this._methodDefinitionContext.GenericArguments[genericParameter.Position]) : this._typeDefinitionContext.GenericArguments[genericParameter.Position]);
        }

        public TypeReference ResolveParameterType(MethodReference method, ParameterReference parameter) => 
            this.Resolve(Unity.IL2CPP.GenericParameterResolver.ResolveParameterTypeIfNeeded(method, parameter));

        public TypeReference ResolveReturnType(MethodReference method) => 
            this.Resolve(Unity.IL2CPP.GenericParameterResolver.ResolveReturnTypeIfNeeded(method));

        public TypeReference ResolveVariableType(MethodReference method, VariableReference variable) => 
            this.Resolve(Unity.IL2CPP.GenericParameterResolver.ResolveVariableTypeIfNeeded(method, variable));

        public static Unity.IL2CPP.ILPreProcessor.TypeResolver Empty
        {
            get
            {
                if (_empty == null)
                {
                    _empty = new Unity.IL2CPP.ILPreProcessor.TypeResolver();
                }
                return _empty;
            }
        }
    }
}

