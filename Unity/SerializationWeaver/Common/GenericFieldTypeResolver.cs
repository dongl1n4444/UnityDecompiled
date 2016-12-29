namespace Unity.SerializationWeaver.Common
{
    using Mono.Cecil;
    using Mono.Collections.Generic;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class GenericFieldTypeResolver : IFieldTypeResolver
    {
        private readonly Stack<Collection<TypeReference>> _genericArguments = new Stack<Collection<TypeReference>>();

        public void EnterGenericArgumentsProvider(Collection<TypeReference> genericArguments)
        {
            this._genericArguments.Push(genericArguments);
        }

        public void ExitGenericArgumentsProvider()
        {
            this._genericArguments.Pop();
        }

        public void ResetGenericArgumentStack()
        {
            while (this._genericArguments.Count > 0)
            {
                this._genericArguments.Pop();
            }
        }

        private TypeReference ResolveGenericParameters(TypeReference typeReference)
        {
            if (typeReference.IsGenericParameter)
            {
                GenericParameter parameter = (GenericParameter) typeReference;
                return this.GenericArguments.ElementAt<TypeReference>(parameter.Position);
            }
            if (typeReference.IsGenericInstance)
            {
                GenericInstanceType type = (GenericInstanceType) typeReference;
                GenericInstanceType type2 = new GenericInstanceType(this.ResolveGenericParameters(type.ElementType));
                foreach (TypeReference reference2 in type.GenericArguments)
                {
                    type2.GenericArguments.Add(this.ResolveGenericParameters(reference2));
                }
                return type2;
            }
            if (typeReference.IsArray)
            {
                ArrayType type3 = (ArrayType) typeReference;
                return new ArrayType(this.ResolveGenericParameters(type3.ElementType), type3.Rank);
            }
            if (typeReference.IsPointer)
            {
                return new PointerType(this.ResolveGenericParameters(((PointerType) typeReference).ElementType));
            }
            if (typeReference.IsByReference)
            {
                return new ByReferenceType(this.ResolveGenericParameters(((ByReferenceType) typeReference).ElementType));
            }
            return typeReference;
        }

        public TypeReference TypeOf(FieldReference fieldDefinition) => 
            this.ResolveGenericParameters(fieldDefinition.FieldType);

        public IEnumerable<TypeReference> GenericArguments =>
            this._genericArguments.Peek();
    }
}

