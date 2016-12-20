namespace Unity.Serialization.Weaver
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;

    public class GenericFieldTypeResolver
    {
        private readonly Stack<Context> _resolutionContext = new Stack<Context>();

        public void AddGenericArgumentScope(TypeReference typeReference)
        {
            this._resolutionContext.Push(new Context(typeReference, (this._resolutionContext.Count != 0) ? this._resolutionContext.Peek() : null, this));
        }

        public bool HasGenericParameters(TypeReference typeReference)
        {
            if (typeReference.IsGenericInstance)
            {
                GenericInstanceType type = (GenericInstanceType) typeReference;
                return (this.HasGenericParameters(type.ElementType) || Enumerable.Any<TypeReference>(type.GenericArguments, new Func<TypeReference, bool>(this, (IntPtr) this.HasGenericParameters)));
            }
            if (typeReference.IsArray)
            {
                return this.HasGenericParameters(((ArrayType) typeReference).ElementType);
            }
            return typeReference.IsGenericParameter;
        }

        public void ResetGenericArgumentStack()
        {
            this._resolutionContext.Clear();
        }

        private TypeReference ResolveByName(GenericParameter genericParameter)
        {
            <ResolveByName>c__AnonStorey0 storey = new <ResolveByName>c__AnonStorey0 {
                genericParameter = genericParameter
            };
            foreach (Context context in Enumerable.Where<Context>(Enumerable.Reverse<Context>(this._resolutionContext), new Func<Context, bool>(storey, (IntPtr) this.<>m__0)))
            {
                return context.Resolve(storey.genericParameter);
            }
            throw new ResolutionException(storey.genericParameter);
        }

        public TypeReference ResolveGenericParameters(TypeReference typeReference)
        {
            if (typeReference.IsGenericParameter)
            {
                return this.ResolveByName((GenericParameter) typeReference);
            }
            if (typeReference.IsArray)
            {
                return new ArrayType(this.ResolveGenericParameters(typeReference.GetElementType()));
            }
            if (!typeReference.IsGenericInstance)
            {
                throw new ResolutionException(typeReference);
            }
            GenericInstanceType type = (GenericInstanceType) typeReference;
            GenericInstanceType type2 = new GenericInstanceType(type.ElementType);
            foreach (TypeReference reference2 in type.GenericArguments)
            {
                type2.GenericArguments.Add(this.ResolveGenericParameters(reference2));
            }
            return type2;
        }

        public TypeReference TypeOf(FieldReference fieldDefinition)
        {
            TypeReference fieldType = fieldDefinition.FieldType;
            if (this.HasGenericParameters(fieldType))
            {
                return this.ResolveGenericParameters(fieldType);
            }
            return fieldType;
        }

        [CompilerGenerated]
        private sealed class <ResolveByName>c__AnonStorey0
        {
            internal GenericParameter genericParameter;

            internal bool <>m__0(GenericFieldTypeResolver.Context context)
            {
                return context.CanResolve(this.genericParameter);
            }
        }

        private class Context
        {
            private readonly GenericFieldTypeResolver _fieldTypeResolver;
            private readonly Dictionary<string, TypeReference> _scope = new Dictionary<string, TypeReference>();
            private readonly TypeReference _typeReference;

            public Context(TypeReference typeReference, GenericFieldTypeResolver.Context parentContext, GenericFieldTypeResolver fieldTypeResolver)
            {
                this._typeReference = typeReference;
                this._fieldTypeResolver = fieldTypeResolver;
                this.BuildScope(parentContext);
            }

            private void BuildScope(GenericFieldTypeResolver.Context parentContext)
            {
                if (parentContext != null)
                {
                    foreach (KeyValuePair<string, TypeReference> pair in parentContext._scope)
                    {
                        this._scope[pair.Key] = pair.Value;
                    }
                }
                GenericInstanceType type = this._typeReference as GenericInstanceType;
                if (type != null)
                {
                    TypeReference elementType = type.ElementType;
                    foreach (GenericParameter parameter in elementType.GenericParameters)
                    {
                        TypeReference typeReference = Enumerable.ElementAt<TypeReference>(type.GenericArguments, parameter.Position);
                        if (this._fieldTypeResolver.HasGenericParameters(typeReference))
                        {
                            this._scope[parameter.Name] = this._fieldTypeResolver.ResolveGenericParameters(typeReference);
                        }
                        else
                        {
                            this._scope[parameter.Name] = typeReference;
                        }
                    }
                }
            }

            public bool CanResolve(GenericParameter genericParameter)
            {
                return (((TypeReference) genericParameter.Owner).FullName == this._typeReference.GetElementType().FullName);
            }

            public TypeReference Resolve(GenericParameter genericParameter)
            {
                return genericParameter.Module.ImportReference(this._scope[genericParameter.Name]);
            }
        }
    }
}

