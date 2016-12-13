using Mono.Cecil;
using System;
using System.Collections.Generic;
using Unity.IL2CPP.GenericSharing;
using Unity.IL2CPP.ILPreProcessor;

namespace Unity.IL2CPP.IoCServices
{
	public interface IGenericSharingAnalysisService
	{
		bool IsGenericSharingForValueTypesEnabled
		{
			get;
		}

		void AddType(TypeDefinition typeDefinition, List<RuntimeGenericData> runtimeGenericDataList);

		void AddMethod(MethodDefinition methodDefinition, List<RuntimeGenericData> runtimeGenericDataList);

		bool NeedsTypeContextAsArgument(MethodReference method);

		bool IsSharedType(GenericInstanceType type);

		bool IsSharedMethod(MethodReference method);

		bool CanShareMethod(MethodReference method);

		bool CanShareType(GenericInstanceType type);

		GenericInstanceType GetFullySharedType(TypeDefinition typeDefinition);

		GenericInstanceType GetSharedType(TypeReference type);

		TypeReference GetFullySharedTypeForGenericParameter(GenericParameter genericParameter);

		TypeReference GetSharedTypeForGenericParameter(TypeResolver typeResolver, GenericParameter genericParameter);

		TypeReference GetUnderlyingSharedType(TypeReference inflatedType);

		bool AreFullySharableGenericParameters(IEnumerable<GenericParameter> genericParameters);

		MethodReference GetFullySharedMethod(MethodDefinition method);

		MethodReference GetSharedMethod(MethodReference method);

		GenericSharingData RuntimeGenericContextFor(TypeDefinition type);

		GenericSharingData GenerateRuntimeGenericContext(TypeDefinition type);

		GenericSharingData RuntimeGenericContextFor(MethodDefinition method);

		GenericSharingData GenerateRuntimeGenericContext(MethodDefinition method);

		bool ShouldTryToCallStaticConstructorBeforeMethodCall(MethodReference targetMethod, MethodReference invokingMethod);
	}
}
