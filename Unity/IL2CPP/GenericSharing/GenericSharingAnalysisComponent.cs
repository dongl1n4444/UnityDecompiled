namespace Unity.IL2CPP.GenericSharing
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Common;
    using Unity.IL2CPP.ILPreProcessor;
    using Unity.IL2CPP.IoCServices;

    public class GenericSharingAnalysisComponent : IGenericSharingAnalysisService, IDisposable
    {
        private readonly Dictionary<MethodDefinition, GenericSharingData> _genericMethodData = new Dictionary<MethodDefinition, GenericSharingData>();
        private readonly Dictionary<TypeDefinition, GenericSharingData> _genericTypeData = new Dictionary<TypeDefinition, GenericSharingData>();
        private readonly Dictionary<MethodDefinition, List<RuntimeGenericData>> _methodData = new Dictionary<MethodDefinition, List<RuntimeGenericData>>();
        private readonly Dictionary<MethodReference, List<MethodReference>> _methodsSharedFrom = new Dictionary<MethodReference, List<MethodReference>>(new Unity.IL2CPP.Common.MethodReferenceComparer());
        private readonly Dictionary<TypeDefinition, List<RuntimeGenericData>> _typeData = new Dictionary<TypeDefinition, List<RuntimeGenericData>>();
        [CompilerGenerated]
        private static Func<GenericParameter, bool> <>f__am$cache0;

        public void AddMethod(MethodDefinition methodDefinition, List<RuntimeGenericData> runtimeGenericDataList)
        {
            this._methodData.Add(methodDefinition, runtimeGenericDataList);
        }

        public void AddSharedMethod(MethodReference sharedMethod, MethodReference actualMethod)
        {
            if (!this._methodsSharedFrom.ContainsKey(sharedMethod))
            {
                this._methodsSharedFrom[sharedMethod] = new List<MethodReference>();
            }
            this._methodsSharedFrom[sharedMethod].Add(actualMethod);
        }

        public void AddType(TypeDefinition typeDefinition, List<RuntimeGenericData> runtimeGenericDataList)
        {
            this._typeData.Add(typeDefinition, runtimeGenericDataList);
        }

        public bool AreFullySharableGenericParameters(IEnumerable<GenericParameter> genericParameters)
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = gp => !gp.HasNotNullableValueTypeConstraint;
            }
            return genericParameters.All<GenericParameter>(<>f__am$cache0);
        }

        public bool CanShareMethod(MethodReference method)
        {
            if (!method.IsGenericInstance && !method.DeclaringType.IsGenericInstance)
            {
                return false;
            }
            GenericInstanceType declaringType = method.DeclaringType as GenericInstanceType;
            if (declaringType != null)
            {
                return this.CanShareType(declaringType);
            }
            return true;
        }

        public bool CanShareType(GenericInstanceType type)
        {
            if (GenericsUtilities.CheckForMaximumRecursion(type))
            {
                return false;
            }
            if (type.IsComOrWindowsRuntimeInterface())
            {
                return false;
            }
            return true;
        }

        public void Dispose()
        {
            this._genericTypeData.Clear();
            this._genericMethodData.Clear();
            this._typeData.Clear();
            this._methodData.Clear();
        }

        public GenericSharingData GenerateRuntimeGenericContext(MethodDefinition method)
        {
            List<RuntimeGenericData> list;
            GenericSharingData data = new GenericSharingData();
            if (this._methodData.TryGetValue(method, out list))
            {
                if (!method.HasGenericParameters)
                {
                    return data;
                }
                foreach (RuntimeGenericData data3 in list)
                {
                    RuntimeGenericTypeData data4 = data3 as RuntimeGenericTypeData;
                    RuntimeGenericMethodData data5 = data3 as RuntimeGenericMethodData;
                    if (data4 != null)
                    {
                        data.AddData(new RuntimeGenericInflatedTypeData(data4.InfoType, data4.GenericType, data4.GenericType));
                    }
                    if (data5 != null)
                    {
                        data.AddMethodUsage(data5.GenericMethod, data5.GenericMethod);
                    }
                }
            }
            return data;
        }

        public GenericSharingData GenerateRuntimeGenericContext(TypeDefinition type)
        {
            List<RuntimeGenericData> list;
            GenericSharingData data = new GenericSharingData();
            if (this._typeData.TryGetValue(type, out list))
            {
                if (!type.HasGenericParameters)
                {
                    return data;
                }
                foreach (RuntimeGenericData data3 in list)
                {
                    RuntimeGenericTypeData data4 = data3 as RuntimeGenericTypeData;
                    RuntimeGenericMethodData data5 = data3 as RuntimeGenericMethodData;
                    if (data4 != null)
                    {
                        data.AddData(new RuntimeGenericInflatedTypeData(data4.InfoType, data4.GenericType, data4.GenericType));
                    }
                    if (data5 != null)
                    {
                        data.AddMethodUsage(data5.GenericMethod, data5.GenericMethod);
                    }
                }
            }
            return data;
        }

        public MethodReference GetFullySharedMethod(MethodDefinition method)
        {
            if (!method.HasGenericParameters && !method.DeclaringType.HasGenericParameters)
            {
                throw new ArgumentException($"Attempting to get a fully shared method for method '{method.FullName}' which does not have any generic parameters");
            }
            TypeReference declaringType = !method.DeclaringType.HasGenericParameters ? ((TypeReference) method.DeclaringType) : ((TypeReference) this.GetFullySharedType(method.DeclaringType));
            MethodReference owner = new MethodReference(method.Name, method.ReturnType, declaringType);
            foreach (GenericParameter parameter in method.Resolve().GenericParameters)
            {
                owner.GenericParameters.Add(new GenericParameter(parameter.Name, owner));
            }
            owner.CallingConvention = method.CallingConvention;
            owner.ExplicitThis = method.ExplicitThis;
            owner.HasThis = method.HasThis;
            foreach (ParameterDefinition definition in method.Parameters)
            {
                owner.Parameters.Add(new ParameterDefinition(definition.Name, definition.Attributes, definition.ParameterType));
            }
            if (method.IsGenericInstance || method.HasGenericParameters)
            {
                GenericInstanceMethod method2 = new GenericInstanceMethod(owner);
                for (int i = 0; i < method.GenericParameters.Count; i++)
                {
                    method2.GenericArguments.Add(method.DeclaringType.Module.TypeSystem.Object);
                }
                owner = method2;
            }
            if (owner.Resolve() == null)
            {
                throw new Exception($"Failed to resolve shared generic instance method '{owner.FullName}' constructed from method definition '{method.FullName}'");
            }
            return owner;
        }

        public GenericInstanceType GetFullySharedType(TypeDefinition typeDefinition)
        {
            GenericInstanceType type = new GenericInstanceType(typeDefinition);
            for (int i = 0; i < typeDefinition.GenericParameters.Count; i++)
            {
                type.GenericArguments.Add(typeDefinition.Module.TypeSystem.Object);
            }
            return type;
        }

        public TypeReference GetFullySharedTypeForGenericParameter(GenericParameter genericParameter)
        {
            if (genericParameter.HasNotNullableValueTypeConstraint)
            {
                throw new InvalidOperationException($"Attempting to share generic parameter '{genericParameter.FullName}' which has a value type constraint.");
            }
            return genericParameter.Module.TypeSystem.Object;
        }

        public IEnumerable<MethodReference> GetMethodsSharedFrom(MethodReference sharedMethod)
        {
            if (this._methodsSharedFrom.ContainsKey(sharedMethod))
            {
                return this._methodsSharedFrom[sharedMethod];
            }
            return Enumerable.Empty<MethodReference>();
        }

        public MethodReference GetSharedMethod(MethodReference method)
        {
            TypeReference declaringType = method.DeclaringType;
            if (declaringType.IsGenericInstance || declaringType.HasGenericParameters)
            {
                declaringType = this.GetSharedType(method.DeclaringType);
            }
            MethodReference owner = new MethodReference(method.Name, method.ReturnType, declaringType);
            foreach (GenericParameter parameter in method.Resolve().GenericParameters)
            {
                owner.GenericParameters.Add(new GenericParameter(parameter.Name, owner));
            }
            owner.CallingConvention = method.CallingConvention;
            owner.ExplicitThis = method.ExplicitThis;
            owner.HasThis = method.HasThis;
            foreach (ParameterDefinition definition in method.Parameters)
            {
                owner.Parameters.Add(new ParameterDefinition(definition.Name, definition.Attributes, definition.ParameterType));
            }
            if (method.IsGenericInstance || method.HasGenericParameters)
            {
                Unity.IL2CPP.ILPreProcessor.TypeResolver typeResolver = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(method.DeclaringType, method);
                GenericInstanceMethod method2 = new GenericInstanceMethod(owner);
                foreach (GenericParameter parameter2 in method.Resolve().GenericParameters)
                {
                    method2.GenericArguments.Add(this.GetSharedTypeForGenericParameter(typeResolver, parameter2));
                }
                owner = method2;
            }
            if (owner.Resolve() == null)
            {
                throw new Exception("Failed to resolve shared generic method");
            }
            return owner;
        }

        public GenericInstanceType GetSharedType(TypeReference type)
        {
            TypeDefinition definition = type.Resolve();
            Unity.IL2CPP.ILPreProcessor.TypeResolver typeResolver = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(type);
            GenericInstanceType type2 = new GenericInstanceType(definition);
            foreach (GenericParameter parameter in definition.GenericParameters)
            {
                type2.GenericArguments.Add(this.GetSharedTypeForGenericParameter(typeResolver, parameter));
            }
            return type2;
        }

        public TypeReference GetSharedTypeForGenericParameter(Unity.IL2CPP.ILPreProcessor.TypeResolver typeResolver, GenericParameter genericParameter) => 
            this.GetUnderlyingSharedType(typeResolver.Resolve(genericParameter));

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
            if (inflatedType.IsValueType())
            {
                if (inflatedType.IsGenericInstance)
                {
                    return this.GetSharedType(inflatedType);
                }
                return inflatedType;
            }
            return inflatedType.Module.TypeSystem.Object;
        }

        public bool IsSharedMethod(MethodReference method) => 
            (this.CanShareMethod(method) && Unity.IL2CPP.Common.MethodReferenceComparer.AreEqual(method, this.GetSharedMethod(method)));

        public bool IsSharedType(GenericInstanceType type) => 
            (this.CanShareType(type) && Unity.IL2CPP.Common.TypeReferenceEqualityComparer.AreEqual((TypeReference) type, (TypeReference) this.GetSharedType(type), TypeComparisonMode.Exact));

        public bool NeedsTypeContextAsArgument(MethodReference method) => 
            (method.Resolve().IsStatic || method.DeclaringType.IsValueType());

        public GenericSharingData RuntimeGenericContextFor(MethodDefinition method)
        {
            GenericSharingData data;
            if (!this._genericMethodData.TryGetValue(method, out data))
            {
                this._genericMethodData[method] = data = this.GenerateRuntimeGenericContext(method);
            }
            return data;
        }

        public GenericSharingData RuntimeGenericContextFor(TypeDefinition type)
        {
            GenericSharingData data;
            if (!this._genericTypeData.TryGetValue(type, out data))
            {
                this._genericTypeData[type] = data = this.GenerateRuntimeGenericContext(type);
            }
            return data;
        }

        public bool ShouldTryToCallStaticConstructorBeforeMethodCall(MethodReference targetMethod, MethodReference invokingMethod)
        {
            if (!targetMethod.HasThis)
            {
                return true;
            }
            if (!invokingMethod.Resolve().IsConstructor)
            {
                return false;
            }
            return Unity.IL2CPP.Common.TypeReferenceEqualityComparer.AreEqual(targetMethod.DeclaringType, invokingMethod.DeclaringType.GetBaseType(), TypeComparisonMode.Exact);
        }

        public bool IsGenericSharingForValueTypesEnabled =>
            CodeGenOptions.EnablePrimitiveValueTypeGenericSharing;
    }
}

