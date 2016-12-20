namespace Unity.IL2CPP.IoCServices
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using Unity.IL2CPP.GenericSharing;
    using Unity.IL2CPP.ILPreProcessor;

    public interface IGenericSharingAnalysisService
    {
        void AddMethod(MethodDefinition methodDefinition, List<RuntimeGenericData> runtimeGenericDataList);
        void AddType(TypeDefinition typeDefinition, List<RuntimeGenericData> runtimeGenericDataList);
        bool AreFullySharableGenericParameters(IEnumerable<GenericParameter> genericParameters);
        bool CanShareMethod(MethodReference method);
        bool CanShareType(GenericInstanceType type);
        GenericSharingData GenerateRuntimeGenericContext(MethodDefinition method);
        GenericSharingData GenerateRuntimeGenericContext(TypeDefinition type);
        MethodReference GetFullySharedMethod(MethodDefinition method);
        GenericInstanceType GetFullySharedType(TypeDefinition typeDefinition);
        TypeReference GetFullySharedTypeForGenericParameter(GenericParameter genericParameter);
        MethodReference GetSharedMethod(MethodReference method);
        GenericInstanceType GetSharedType(TypeReference type);
        TypeReference GetSharedTypeForGenericParameter(Unity.IL2CPP.ILPreProcessor.TypeResolver typeResolver, GenericParameter genericParameter);
        TypeReference GetUnderlyingSharedType(TypeReference inflatedType);
        bool IsSharedMethod(MethodReference method);
        bool IsSharedType(GenericInstanceType type);
        bool NeedsTypeContextAsArgument(MethodReference method);
        GenericSharingData RuntimeGenericContextFor(MethodDefinition method);
        GenericSharingData RuntimeGenericContextFor(TypeDefinition type);
        bool ShouldTryToCallStaticConstructorBeforeMethodCall(MethodReference targetMethod, MethodReference invokingMethod);

        bool IsGenericSharingForValueTypesEnabled { get; }
    }
}

