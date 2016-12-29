namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using System;

    internal class GenericParameterResolver
    {
        internal static TypeReference ResolveFieldTypeIfNeeded(FieldReference fieldReference) => 
            ResolveIfNeeded(null, fieldReference.DeclaringType as GenericInstanceType, fieldReference.FieldType);

        private static ArrayType ResolveIfNeeded(IGenericInstance genericInstanceMethod, IGenericInstance genericInstanceType, ArrayType arrayType) => 
            new ArrayType(ResolveIfNeeded(genericInstanceMethod, genericInstanceType, arrayType.ElementType), arrayType.Rank);

        private static ByReferenceType ResolveIfNeeded(IGenericInstance genericInstanceMethod, IGenericInstance genericInstanceType, ByReferenceType byReferenceType) => 
            new ByReferenceType(ResolveIfNeeded(genericInstanceMethod, genericInstanceType, byReferenceType.ElementType));

        private static GenericInstanceType ResolveIfNeeded(IGenericInstance genericInstanceMethod, IGenericInstance genericInstanceType, GenericInstanceType genericInstanceType1)
        {
            if (!genericInstanceType1.ContainsGenericParameters())
            {
                return genericInstanceType1;
            }
            GenericInstanceType type2 = new GenericInstanceType(genericInstanceType1.ElementType);
            foreach (TypeReference reference in genericInstanceType1.GenericArguments)
            {
                if (!reference.IsGenericParameter)
                {
                    type2.GenericArguments.Add(ResolveIfNeeded(genericInstanceMethod, genericInstanceType, reference));
                }
                else
                {
                    GenericParameter item = (GenericParameter) reference;
                    switch (item.Type)
                    {
                        case GenericParameterType.Type:
                            if (genericInstanceType == null)
                            {
                                throw new NotSupportedException();
                            }
                            type2.GenericArguments.Add(genericInstanceType.GenericArguments[item.Position]);
                            break;

                        case GenericParameterType.Method:
                            if (genericInstanceMethod == null)
                            {
                                type2.GenericArguments.Add(item);
                            }
                            else
                            {
                                type2.GenericArguments.Add(genericInstanceMethod.GenericArguments[item.Position]);
                            }
                            break;
                    }
                }
            }
            return type2;
        }

        private static TypeReference ResolveIfNeeded(IGenericInstance genericInstanceMethod, IGenericInstance genericInstanceType, GenericParameter genericParameterElement) => 
            ((genericParameterElement.MetadataType != MetadataType.MVar) ? genericInstanceType.GenericArguments[genericParameterElement.Position] : ((genericInstanceMethod == null) ? genericParameterElement : genericInstanceMethod.GenericArguments[genericParameterElement.Position]));

        private static TypeReference ResolveIfNeeded(IGenericInstance genericInstanceMethod, IGenericInstance declaringGenericInstanceType, TypeReference parameterType)
        {
            ByReferenceType byReferenceType = parameterType as ByReferenceType;
            if (byReferenceType != null)
            {
                return ResolveIfNeeded(genericInstanceMethod, declaringGenericInstanceType, byReferenceType);
            }
            ArrayType arrayType = parameterType as ArrayType;
            if (arrayType != null)
            {
                return ResolveIfNeeded(genericInstanceMethod, declaringGenericInstanceType, arrayType);
            }
            GenericInstanceType type3 = parameterType as GenericInstanceType;
            if (type3 != null)
            {
                return ResolveIfNeeded(genericInstanceMethod, declaringGenericInstanceType, type3);
            }
            GenericParameter genericParameterElement = parameterType as GenericParameter;
            if (genericParameterElement != null)
            {
                return ResolveIfNeeded(genericInstanceMethod, declaringGenericInstanceType, genericParameterElement);
            }
            RequiredModifierType typeReference = parameterType as RequiredModifierType;
            if ((typeReference != null) && typeReference.ContainsGenericParameters())
            {
                return ResolveIfNeeded(genericInstanceMethod, declaringGenericInstanceType, typeReference.ElementType);
            }
            if (parameterType.ContainsGenericParameters())
            {
                throw new Exception("Unexpected generic parameter.");
            }
            return parameterType;
        }

        internal static TypeReference ResolveParameterTypeIfNeeded(MethodReference method, ParameterReference parameter)
        {
            GenericInstanceMethod genericInstanceMethod = method as GenericInstanceMethod;
            GenericInstanceType declaringType = method.DeclaringType as GenericInstanceType;
            if ((genericInstanceMethod == null) && (declaringType == null))
            {
                return parameter.ParameterType;
            }
            return ResolveIfNeeded(genericInstanceMethod, declaringType, parameter.ParameterType);
        }

        internal static TypeReference ResolveReturnTypeIfNeeded(MethodReference methodReference)
        {
            if (methodReference.DeclaringType.IsArray && (methodReference.Name == "Get"))
            {
                return methodReference.ReturnType;
            }
            GenericInstanceMethod genericInstanceMethod = methodReference as GenericInstanceMethod;
            GenericInstanceType declaringType = methodReference.DeclaringType as GenericInstanceType;
            if ((genericInstanceMethod == null) && (declaringType == null))
            {
                return methodReference.ReturnType;
            }
            return ResolveIfNeeded(genericInstanceMethod, declaringType, methodReference.ReturnType);
        }

        internal static TypeReference ResolveVariableTypeIfNeeded(MethodReference method, VariableReference variable)
        {
            GenericInstanceMethod genericInstanceMethod = method as GenericInstanceMethod;
            GenericInstanceType declaringType = method.DeclaringType as GenericInstanceType;
            if ((genericInstanceMethod == null) && (declaringType == null))
            {
                return variable.VariableType;
            }
            return ResolveIfNeeded(genericInstanceMethod, declaringType, variable.VariableType);
        }
    }
}

