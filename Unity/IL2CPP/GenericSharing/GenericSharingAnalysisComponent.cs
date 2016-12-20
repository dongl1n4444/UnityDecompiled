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
        private readonly Dictionary<TypeDefinition, List<RuntimeGenericData>> _typeData = new Dictionary<TypeDefinition, List<RuntimeGenericData>>();
        [CompilerGenerated]
        private static Func<GenericParameter, bool> <>f__am$cache0;

        public void AddMethod(MethodDefinition methodDefinition, List<RuntimeGenericData> runtimeGenericDataList)
        {
            this._methodData.Add(methodDefinition, runtimeGenericDataList);
        }

        public void AddType(TypeDefinition typeDefinition, List<RuntimeGenericData> runtimeGenericDataList)
        {
            this._typeData.Add(typeDefinition, runtimeGenericDataList);
        }

        public bool AreFullySharableGenericParameters(IEnumerable<GenericParameter> genericParameters)
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<GenericParameter, bool>(null, (IntPtr) <AreFullySharableGenericParameters>m__0);
            }
            return Enumerable.All<GenericParameter>(genericParameters, <>f__am$cache0);
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
            if (Extensions.IsComOrWindowsRuntimeInterface(type))
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
                throw new ArgumentException(string.Format("Attempting to get a fully shared method for method '{0}' which does not have any generic parameters", method.FullName));
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
                throw new Exception(string.Format("Failed to resolve shared generic instance method '{0}' constructed from method definition '{1}'", owner.FullName, method.FullName));
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
                throw new InvalidOperationException(string.Format("Attempting to share generic parameter '{0}' which has a value type constraint.", genericParameter.FullName));
            }
            return genericParameter.Module.TypeSystem.Object;
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

        public TypeReference GetSharedTypeForGenericParameter(Unity.IL2CPP.ILPreProcessor.TypeResolver typeResolver, GenericParameter genericParameter)
        {
            return this.GetUnderlyingSharedType(typeResolver.Resolve(genericParameter));
        }

        public TypeReference GetUnderlyingSharedType(TypeReference inflatedType)
        {
            if (this.IsGenericSharingForValueTypesEnabled)
            {
                if (Extensions.IsEnum(inflatedType))
                {
                    inflatedType = Extensions.GetUnderlyingEnumType(inflatedType);
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
            if (Extensions.IsValueType(inflatedType))
            {
                if (inflatedType.IsGenericInstance)
                {
                    return this.GetSharedType(inflatedType);
                }
                return inflatedType;
            }
            return inflatedType.Module.TypeSystem.Object;
        }

        public bool IsSharedMethod(MethodReference method)
        {
            return (this.CanShareMethod(method) && Unity.IL2CPP.Common.MethodReferenceComparer.AreEqual(method, this.GetSharedMethod(method)));
        }

        public bool IsSharedType(GenericInstanceType type)
        {
            return (this.CanShareType(type) && Unity.IL2CPP.Common.TypeReferenceEqualityComparer.AreEqual((TypeReference) type, (TypeReference) this.GetSharedType(type), TypeComparisonMode.Exact));
        }

        public bool NeedsTypeContextAsArgument(MethodReference method)
        {
            return (method.Resolve().IsStatic || Extensions.IsValueType(method.DeclaringType));
        }

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
            return Unity.IL2CPP.Common.TypeReferenceEqualityComparer.AreEqual(targetMethod.DeclaringType, Extensions.GetBaseType(invokingMethod.DeclaringType), TypeComparisonMode.Exact);
        }

        public bool IsGenericSharingForValueTypesEnabled
        {
            get
            {
                return CodeGenOptions.EnablePrimitiveValueTypeGenericSharing;
            }
        }
    }
}

