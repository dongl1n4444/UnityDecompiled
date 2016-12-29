namespace Unity.SerializationLogic
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;

    public class TypeResolver
    {
        private readonly Dictionary<string, GenericInstanceHolder> _context;
        private readonly IGenericInstance _methodDefinitionContext;
        private readonly IGenericInstance _typeDefinitionContext;

        public TypeResolver()
        {
            this._context = new Dictionary<string, GenericInstanceHolder>();
        }

        public TypeResolver(GenericInstanceMethod methodDefinitionContext)
        {
            this._context = new Dictionary<string, GenericInstanceHolder>();
            this._methodDefinitionContext = methodDefinitionContext;
        }

        public TypeResolver(IGenericInstance typeDefinitionContext)
        {
            this._context = new Dictionary<string, GenericInstanceHolder>();
            this._typeDefinitionContext = typeDefinitionContext;
        }

        public TypeResolver(IGenericInstance typeDefinitionContext, IGenericInstance methodDefinitionContext)
        {
            this._context = new Dictionary<string, GenericInstanceHolder>();
            this._typeDefinitionContext = typeDefinitionContext;
            this._methodDefinitionContext = methodDefinitionContext;
        }

        public void Add(GenericInstanceMethod genericInstanceMethod)
        {
            this.Add(ElementTypeFor(genericInstanceMethod).FullName, genericInstanceMethod);
        }

        public void Add(GenericInstanceType genericInstanceType)
        {
            this.Add(ElementTypeFor(genericInstanceType).FullName, genericInstanceType);
        }

        private void Add(string key, IGenericInstance value)
        {
            GenericInstanceHolder holder;
            if (this._context.TryGetValue(key, out holder))
            {
                MemberReference reference = value as MemberReference;
                if (reference == null)
                {
                    throw new NotSupportedException();
                }
                MemberReference genericInstance = (MemberReference) holder.GenericInstance;
                if (genericInstance.FullName != reference.FullName)
                {
                    throw new ArgumentException("Duplicate key!", "key");
                }
                holder.Count++;
            }
            else
            {
                GenericInstanceHolder holder2 = new GenericInstanceHolder {
                    Count = 1,
                    GenericInstance = value
                };
                this._context.Add(key, holder2);
            }
        }

        private static MemberReference ElementTypeFor(MethodSpecification genericInstanceMethod) => 
            genericInstanceMethod.ElementMethod;

        private static MemberReference ElementTypeFor(TypeSpecification genericInstanceType) => 
            genericInstanceType.ElementType;

        private TypeReference GenericArgumentAt(string key, int position) => 
            this._context[key].GenericInstance.GenericArguments[position];

        public void Remove(GenericInstanceMethod genericInstanceMethod)
        {
            this.Remove(genericInstanceMethod.ElementMethod.FullName, genericInstanceMethod);
        }

        public void Remove(GenericInstanceType genericInstanceType)
        {
            this.Remove(genericInstanceType.ElementType.FullName, genericInstanceType);
        }

        private void Remove(string key, IGenericInstance value)
        {
            GenericInstanceHolder holder;
            if (!this._context.TryGetValue(key, out holder))
            {
                throw new ArgumentException("Invalid key!", "key");
            }
            MemberReference reference = value as MemberReference;
            if (reference == null)
            {
                throw new NotSupportedException();
            }
            MemberReference genericInstance = (MemberReference) holder.GenericInstance;
            if (genericInstance.FullName != reference.FullName)
            {
                throw new ArgumentException("Invalid value!", "value");
            }
            holder.Count--;
            if (holder.Count == 0)
            {
                this._context.Remove(key);
            }
        }

        public TypeReference Resolve(TypeReference typeReference)
        {
            GenericParameter genericParameter = typeReference as GenericParameter;
            if (genericParameter != null)
            {
                return this.Resolve(this.ResolveGenericParameter(genericParameter));
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
            GenericInstanceType type4 = typeReference as GenericInstanceType;
            if (type4 != null)
            {
                GenericInstanceType type5 = new GenericInstanceType(this.Resolve(type4.ElementType));
                foreach (TypeReference reference2 in type4.GenericArguments)
                {
                    type5.GenericArguments.Add(this.Resolve(reference2));
                }
                return type5;
            }
            PinnedType type6 = typeReference as PinnedType;
            if (type6 != null)
            {
                return new PinnedType(this.Resolve(type6.ElementType));
            }
            RequiredModifierType type7 = typeReference as RequiredModifierType;
            if (type7 != null)
            {
                return this.Resolve(type7.ElementType);
            }
            OptionalModifierType type8 = typeReference as OptionalModifierType;
            if (type8 != null)
            {
                return new OptionalModifierType(this.Resolve(type8.ModifierType), this.Resolve(type8.ElementType));
            }
            SentinelType type9 = typeReference as SentinelType;
            if (type9 != null)
            {
                return new SentinelType(this.Resolve(type9.ElementType));
            }
            if (typeReference is FunctionPointerType)
            {
                throw new NotSupportedException("Function pointer types are not supported by the SerializationWeaver");
            }
            if (typeReference is TypeSpecification)
            {
                throw new NotSupportedException();
            }
            return typeReference;
        }

        private TypeReference ResolveGenericParameter(GenericParameter genericParameter)
        {
            if (genericParameter.Owner == null)
            {
                throw new NotSupportedException();
            }
            MemberReference owner = genericParameter.Owner as MemberReference;
            if (owner == null)
            {
                throw new NotSupportedException();
            }
            string fullName = owner.FullName;
            if (!this._context.ContainsKey(fullName))
            {
                return ((genericParameter.Type != GenericParameterType.Type) ? this._methodDefinitionContext.GenericArguments[genericParameter.Position] : this._typeDefinitionContext.GenericArguments[genericParameter.Position]);
            }
            return this.GenericArgumentAt(fullName, genericParameter.Position);
        }
    }
}

