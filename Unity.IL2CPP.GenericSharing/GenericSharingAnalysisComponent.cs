using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.IL2CPP.Common;
using Unity.IL2CPP.ILPreProcessor;
using Unity.IL2CPP.IoCServices;

namespace Unity.IL2CPP.GenericSharing
{
	public class GenericSharingAnalysisComponent : IGenericSharingAnalysisService, IDisposable
	{
		private readonly Dictionary<TypeDefinition, GenericSharingData> _genericTypeData = new Dictionary<TypeDefinition, GenericSharingData>();

		private readonly Dictionary<MethodDefinition, GenericSharingData> _genericMethodData = new Dictionary<MethodDefinition, GenericSharingData>();

		private readonly Dictionary<TypeDefinition, List<RuntimeGenericData>> _typeData = new Dictionary<TypeDefinition, List<RuntimeGenericData>>();

		private readonly Dictionary<MethodDefinition, List<RuntimeGenericData>> _methodData = new Dictionary<MethodDefinition, List<RuntimeGenericData>>();

		public bool IsGenericSharingForValueTypesEnabled
		{
			get
			{
				return CodeGenOptions.EnablePrimitiveValueTypeGenericSharing;
			}
		}

		public void Dispose()
		{
			this._genericTypeData.Clear();
			this._genericMethodData.Clear();
			this._typeData.Clear();
			this._methodData.Clear();
		}

		public void AddType(TypeDefinition typeDefinition, List<RuntimeGenericData> runtimeGenericDataList)
		{
			this._typeData.Add(typeDefinition, runtimeGenericDataList);
		}

		public void AddMethod(MethodDefinition methodDefinition, List<RuntimeGenericData> runtimeGenericDataList)
		{
			this._methodData.Add(methodDefinition, runtimeGenericDataList);
		}

		public bool NeedsTypeContextAsArgument(MethodReference method)
		{
			return method.Resolve().IsStatic || method.DeclaringType.IsValueType();
		}

		public bool IsSharedType(GenericInstanceType type)
		{
			return this.CanShareType(type) && TypeReferenceEqualityComparer.AreEqual(type, this.GetSharedType(type), TypeComparisonMode.Exact);
		}

		public bool IsSharedMethod(MethodReference method)
		{
			return this.CanShareMethod(method) && MethodReferenceComparer.AreEqual(method, this.GetSharedMethod(method));
		}

		public bool CanShareMethod(MethodReference method)
		{
			bool result;
			if (!method.IsGenericInstance && !method.DeclaringType.IsGenericInstance)
			{
				result = false;
			}
			else
			{
				GenericInstanceType genericInstanceType = method.DeclaringType as GenericInstanceType;
				result = (genericInstanceType == null || this.CanShareType(genericInstanceType));
			}
			return result;
		}

		public bool CanShareType(GenericInstanceType type)
		{
			return !GenericsUtilities.CheckForMaximumRecursion(type) && !type.IsComOrWindowsRuntimeInterface();
		}

		public GenericInstanceType GetFullySharedType(TypeDefinition typeDefinition)
		{
			GenericInstanceType genericInstanceType = new GenericInstanceType(typeDefinition);
			for (int i = 0; i < typeDefinition.GenericParameters.Count; i++)
			{
				genericInstanceType.GenericArguments.Add(typeDefinition.Module.TypeSystem.Object);
			}
			return genericInstanceType;
		}

		public GenericInstanceType GetSharedType(TypeReference type)
		{
			TypeDefinition typeDefinition = type.Resolve();
			TypeResolver typeResolver = TypeResolver.For(type);
			GenericInstanceType genericInstanceType = new GenericInstanceType(typeDefinition);
			foreach (GenericParameter current in typeDefinition.GenericParameters)
			{
				genericInstanceType.GenericArguments.Add(this.GetSharedTypeForGenericParameter(typeResolver, current));
			}
			return genericInstanceType;
		}

		public TypeReference GetFullySharedTypeForGenericParameter(GenericParameter genericParameter)
		{
			if (genericParameter.HasNotNullableValueTypeConstraint)
			{
				throw new InvalidOperationException(string.Format("Attempting to share generic parameter '{0}' which has a value type constraint.", genericParameter.FullName));
			}
			return genericParameter.Module.TypeSystem.Object;
		}

		public TypeReference GetSharedTypeForGenericParameter(TypeResolver typeResolver, GenericParameter genericParameter)
		{
			return this.GetUnderlyingSharedType(typeResolver.Resolve(genericParameter));
		}

		public TypeReference GetUnderlyingSharedType(TypeReference inflatedType)
		{
			if (this.IsGenericSharingForValueTypesEnabled)
			{
				if (inflatedType.IsEnum())
				{
					inflatedType = inflatedType.GetUnderlyingEnumType();
				}
				if (inflatedType.MetadataType == MetadataType.Boolean)
				{
					inflatedType = inflatedType.Module.TypeSystem.Byte;
				}
				else if (inflatedType.MetadataType == MetadataType.Char)
				{
					inflatedType = inflatedType.Module.TypeSystem.UInt16;
				}
			}
			TypeReference result;
			if (inflatedType.IsValueType())
			{
				if (inflatedType.IsGenericInstance)
				{
					result = this.GetSharedType(inflatedType);
				}
				else
				{
					result = inflatedType;
				}
			}
			else
			{
				result = inflatedType.Module.TypeSystem.Object;
			}
			return result;
		}

		public bool AreFullySharableGenericParameters(IEnumerable<GenericParameter> genericParameters)
		{
			return genericParameters.All((GenericParameter gp) => !gp.HasNotNullableValueTypeConstraint);
		}

		public MethodReference GetFullySharedMethod(MethodDefinition method)
		{
			if (!method.HasGenericParameters && !method.DeclaringType.HasGenericParameters)
			{
				throw new ArgumentException(string.Format("Attempting to get a fully shared method for method '{0}' which does not have any generic parameters", method.FullName));
			}
			TypeReference declaringType = (!method.DeclaringType.HasGenericParameters) ? method.DeclaringType : this.GetFullySharedType(method.DeclaringType);
			MethodReference methodReference = new MethodReference(method.Name, method.ReturnType, declaringType);
			foreach (GenericParameter current in method.Resolve().GenericParameters)
			{
				methodReference.GenericParameters.Add(new GenericParameter(current.Name, methodReference));
			}
			methodReference.CallingConvention = method.CallingConvention;
			methodReference.ExplicitThis = method.ExplicitThis;
			methodReference.HasThis = method.HasThis;
			foreach (ParameterDefinition current2 in method.Parameters)
			{
				methodReference.Parameters.Add(new ParameterDefinition(current2.Name, current2.Attributes, current2.ParameterType));
			}
			if (method.IsGenericInstance || method.HasGenericParameters)
			{
				GenericInstanceMethod genericInstanceMethod = new GenericInstanceMethod(methodReference);
				for (int i = 0; i < method.GenericParameters.Count; i++)
				{
					genericInstanceMethod.GenericArguments.Add(method.DeclaringType.Module.TypeSystem.Object);
				}
				methodReference = genericInstanceMethod;
			}
			if (methodReference.Resolve() == null)
			{
				throw new Exception(string.Format("Failed to resolve shared generic instance method '{0}' constructed from method definition '{1}'", methodReference.FullName, method.FullName));
			}
			return methodReference;
		}

		public MethodReference GetSharedMethod(MethodReference method)
		{
			TypeReference typeReference = method.DeclaringType;
			if (typeReference.IsGenericInstance || typeReference.HasGenericParameters)
			{
				typeReference = this.GetSharedType(method.DeclaringType);
			}
			MethodReference methodReference = new MethodReference(method.Name, method.ReturnType, typeReference);
			foreach (GenericParameter current in method.Resolve().GenericParameters)
			{
				methodReference.GenericParameters.Add(new GenericParameter(current.Name, methodReference));
			}
			methodReference.CallingConvention = method.CallingConvention;
			methodReference.ExplicitThis = method.ExplicitThis;
			methodReference.HasThis = method.HasThis;
			foreach (ParameterDefinition current2 in method.Parameters)
			{
				methodReference.Parameters.Add(new ParameterDefinition(current2.Name, current2.Attributes, current2.ParameterType));
			}
			if (method.IsGenericInstance || method.HasGenericParameters)
			{
				TypeResolver typeResolver = TypeResolver.For(method.DeclaringType, method);
				GenericInstanceMethod genericInstanceMethod = new GenericInstanceMethod(methodReference);
				foreach (GenericParameter current3 in method.Resolve().GenericParameters)
				{
					genericInstanceMethod.GenericArguments.Add(this.GetSharedTypeForGenericParameter(typeResolver, current3));
				}
				methodReference = genericInstanceMethod;
			}
			if (methodReference.Resolve() == null)
			{
				throw new Exception("Failed to resolve shared generic method");
			}
			return methodReference;
		}

		public GenericSharingData RuntimeGenericContextFor(TypeDefinition type)
		{
			GenericSharingData result;
			if (!this._genericTypeData.TryGetValue(type, out result))
			{
				result = (this._genericTypeData[type] = this.GenerateRuntimeGenericContext(type));
			}
			return result;
		}

		public GenericSharingData GenerateRuntimeGenericContext(TypeDefinition type)
		{
			GenericSharingData genericSharingData = new GenericSharingData();
			List<RuntimeGenericData> list;
			GenericSharingData result;
			if (!this._typeData.TryGetValue(type, out list))
			{
				result = genericSharingData;
			}
			else if (!type.HasGenericParameters)
			{
				result = genericSharingData;
			}
			else
			{
				foreach (RuntimeGenericData current in list)
				{
					RuntimeGenericTypeData runtimeGenericTypeData = current as RuntimeGenericTypeData;
					RuntimeGenericMethodData runtimeGenericMethodData = current as RuntimeGenericMethodData;
					if (runtimeGenericTypeData != null)
					{
						genericSharingData.AddData(new RuntimeGenericInflatedTypeData(runtimeGenericTypeData.InfoType, runtimeGenericTypeData.GenericType, runtimeGenericTypeData.GenericType));
					}
					if (runtimeGenericMethodData != null)
					{
						genericSharingData.AddMethodUsage(runtimeGenericMethodData.GenericMethod, runtimeGenericMethodData.GenericMethod);
					}
				}
				result = genericSharingData;
			}
			return result;
		}

		public GenericSharingData RuntimeGenericContextFor(MethodDefinition method)
		{
			GenericSharingData result;
			if (!this._genericMethodData.TryGetValue(method, out result))
			{
				result = (this._genericMethodData[method] = this.GenerateRuntimeGenericContext(method));
			}
			return result;
		}

		public GenericSharingData GenerateRuntimeGenericContext(MethodDefinition method)
		{
			GenericSharingData genericSharingData = new GenericSharingData();
			List<RuntimeGenericData> list;
			GenericSharingData result;
			if (!this._methodData.TryGetValue(method, out list))
			{
				result = genericSharingData;
			}
			else if (!method.HasGenericParameters)
			{
				result = genericSharingData;
			}
			else
			{
				foreach (RuntimeGenericData current in list)
				{
					RuntimeGenericTypeData runtimeGenericTypeData = current as RuntimeGenericTypeData;
					RuntimeGenericMethodData runtimeGenericMethodData = current as RuntimeGenericMethodData;
					if (runtimeGenericTypeData != null)
					{
						genericSharingData.AddData(new RuntimeGenericInflatedTypeData(runtimeGenericTypeData.InfoType, runtimeGenericTypeData.GenericType, runtimeGenericTypeData.GenericType));
					}
					if (runtimeGenericMethodData != null)
					{
						genericSharingData.AddMethodUsage(runtimeGenericMethodData.GenericMethod, runtimeGenericMethodData.GenericMethod);
					}
				}
				result = genericSharingData;
			}
			return result;
		}

		public bool ShouldTryToCallStaticConstructorBeforeMethodCall(MethodReference targetMethod, MethodReference invokingMethod)
		{
			return !targetMethod.HasThis || (invokingMethod.Resolve().IsConstructor && TypeReferenceEqualityComparer.AreEqual(targetMethod.DeclaringType, invokingMethod.DeclaringType.GetBaseType(), TypeComparisonMode.Exact));
		}
	}
}
