namespace Unity.IL2CPP.GenericsCollection
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;

    internal static class Inflater
    {
        private static GenericInstanceMethod ConstructGenericMethod(GenericContext context, TypeReference declaringType, MethodDefinition method, IEnumerable<TypeReference> genericArguments)
        {
            MethodReference owner = new MethodReference(method.Name, method.ReturnType, declaringType) {
                HasThis = method.HasThis
            };
            foreach (GenericParameter parameter in method.GenericParameters)
            {
                owner.GenericParameters.Add(new GenericParameter(parameter.Name, owner));
            }
            foreach (ParameterDefinition definition in method.Parameters)
            {
                owner.Parameters.Add(new ParameterDefinition(definition.Name, definition.Attributes, definition.ParameterType));
            }
            if (owner.Resolve() == null)
            {
                throw new Exception();
            }
            GenericInstanceMethod method2 = new GenericInstanceMethod(owner);
            foreach (TypeReference reference3 in genericArguments)
            {
                method2.GenericArguments.Add(InflateType(context, reference3));
            }
            return method2;
        }

        private static GenericInstanceType ConstructGenericType(GenericContext context, TypeDefinition typeDefinition, IEnumerable<TypeReference> genericArguments)
        {
            GenericInstanceType type = new GenericInstanceType(typeDefinition);
            foreach (TypeReference reference in genericArguments)
            {
                type.GenericArguments.Add(InflateType(context, reference));
            }
            return type;
        }

        public static GenericInstanceMethod InflateMethod(GenericContext context, GenericInstanceMethod genericInstanceMethod)
        {
            GenericInstanceType declaringType = genericInstanceMethod.DeclaringType as GenericInstanceType;
            TypeReference reference = (declaringType == null) ? InflateType(context, genericInstanceMethod.DeclaringType) : InflateType(context, declaringType);
            return ConstructGenericMethod(context, reference, genericInstanceMethod.Resolve(), genericInstanceMethod.GenericArguments);
        }

        public static GenericInstanceMethod InflateMethod(GenericContext context, MethodDefinition methodDefinition)
        {
            TypeReference declaringType = methodDefinition.DeclaringType;
            if (declaringType.Resolve().HasGenericParameters)
            {
                declaringType = InflateType(context, methodDefinition.DeclaringType);
            }
            return ConstructGenericMethod(context, declaringType, methodDefinition, (IEnumerable<TypeReference>) methodDefinition.GenericParameters);
        }

        public static GenericInstanceType InflateType(GenericContext context, GenericInstanceType genericInstanceType)
        {
            return ConstructGenericType(context, genericInstanceType.Resolve(), genericInstanceType.GenericArguments);
        }

        public static GenericInstanceType InflateType(GenericContext context, TypeDefinition typeDefinition)
        {
            return ConstructGenericType(context, typeDefinition, (IEnumerable<TypeReference>) typeDefinition.GenericParameters);
        }

        public static TypeReference InflateType(GenericContext context, TypeReference typeReference)
        {
            TypeReference reference = InflateTypeWithoutException(context, typeReference);
            if (reference == null)
            {
                throw new Exception();
            }
            return reference;
        }

        public static TypeReference InflateTypeWithoutException(GenericContext context, TypeReference typeReference)
        {
            GenericParameter parameter = typeReference as GenericParameter;
            if (parameter != null)
            {
                return ((parameter.Type != GenericParameterType.Type) ? context.Method.GenericArguments[parameter.Position] : context.Type.GenericArguments[parameter.Position]);
            }
            GenericInstanceType genericInstanceType = typeReference as GenericInstanceType;
            if (genericInstanceType != null)
            {
                return InflateType(context, genericInstanceType);
            }
            ArrayType type2 = typeReference as ArrayType;
            if (type2 != null)
            {
                return new ArrayType(InflateType(context, type2.ElementType), type2.Rank);
            }
            PointerType type3 = typeReference as PointerType;
            if (type3 != null)
            {
                return new PointerType(InflateType(context, type3.ElementType));
            }
            return typeReference.Resolve();
        }
    }
}

