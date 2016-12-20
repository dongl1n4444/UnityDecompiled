namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using mscorlib;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    using Unity.IL2CPP.Common;
    using Unity.IL2CPP.ILPreProcessor;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;
    using Unity.IL2CPP.Metadata;

    [Extension]
    public static class Extensions
    {
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<FieldDefinition, bool> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<FieldDefinition, bool> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<CustomAttribute, bool> <>f__am$cache4;
        [CompilerGenerated]
        private static Func<CustomAttribute, TypeReference> <>f__am$cache5;
        [CompilerGenerated]
        private static Func<CustomAttribute, TypeReference> <>f__am$cache6;
        [CompilerGenerated]
        private static Func<CustomAttribute, TypeReference> <>f__am$cache7;
        [CompilerGenerated]
        private static Func<TypeReference, IEnumerable<MethodReference>> <>f__am$cache8;
        [CompilerGenerated]
        private static Func<CustomAttribute, bool> <>f__am$cache9;
        [CompilerGenerated]
        private static Func<CustomAttribute, bool> <>f__am$cacheA;
        [CompilerGenerated]
        private static Func<FieldDefinition, bool> <>f__am$cacheB;
        [CompilerGenerated]
        private static Func<AssemblyDefinition, string> <>f__am$cacheC;
        [CompilerGenerated]
        private static Func<CustomAttribute, bool> <>f__am$cacheD;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__mg$cache0;
        [CompilerGenerated]
        private static Func<TypeReference, bool> <>f__mg$cache1;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__mg$cache2;
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$map1;
        [Inject]
        public static IAssemblyDependencies AssemblyDependencies;
        [Inject]
        public static IGuidProvider GuidProvider;
        [Inject]
        public static INamingService Naming;
        [Inject]
        public static ITypeProviderService TypeProvider;

        [CompilerGenerated]
        private static T <Chunk`1>m__10<T>(ChunkItem<T> x)
        {
            return x.Value;
        }

        [CompilerGenerated]
        private static ChunkItem<T> <Chunk`1>m__9<T>(T value, int index)
        {
            return new ChunkItem<T> { 
                Index = index,
                Value = value
            };
        }

        [CompilerGenerated]
        private static List<T> <Chunk`1>m__A<T>(IGrouping<int, ChunkItem<T>> g)
        {
            return Enumerable.ToList<T>(Enumerable.Select<ChunkItem<T>, T>(g, new Func<ChunkItem<T>, T>(null, <Chunk`1>m__10<T>)));
        }

        private static void AddInterfacesRecursive(TypeReference type, HashSet<TypeReference> interfaces)
        {
            if (!type.IsArray)
            {
                Unity.IL2CPP.ILPreProcessor.TypeResolver resolver = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(type);
                foreach (InterfaceImplementation implementation in type.Resolve().Interfaces)
                {
                    TypeReference item = resolver.Resolve(implementation.InterfaceType);
                    if (interfaces.Add(item))
                    {
                        AddInterfacesRecursive(item, interfaces);
                    }
                }
            }
        }

        [Extension]
        public static List<List<T>> Chunk<T>(IEnumerable<T> foo, int size)
        {
            <Chunk>c__AnonStorey3<T> storey = new <Chunk>c__AnonStorey3<T> {
                size = size
            };
            return Enumerable.ToList<List<T>>(Enumerable.Select<IGrouping<int, ChunkItem<T>>, List<T>>(Enumerable.GroupBy<ChunkItem<T>, int>(Enumerable.Select<T, ChunkItem<T>>(foo, new Func<T, int, ChunkItem<T>>(null, (IntPtr) <Chunk`1>m__9<T>)), new Func<ChunkItem<T>, int>(storey, (IntPtr) this.<>m__0)), new Func<IGrouping<int, ChunkItem<T>>, List<T>>(null, (IntPtr) <Chunk`1>m__A<T>)));
        }

        [Extension]
        public static bool ContainsGenericParameters(MethodReference method)
        {
            if (ContainsGenericParameters(method.DeclaringType))
            {
                return true;
            }
            GenericInstanceMethod method2 = method as GenericInstanceMethod;
            if (method2 != null)
            {
                foreach (TypeReference reference in method2.GenericArguments)
                {
                    if (ContainsGenericParameters(reference))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        [Extension]
        public static bool ContainsGenericParameters(TypeReference typeReference)
        {
            if (typeReference is GenericParameter)
            {
                return true;
            }
            ArrayType type = typeReference as ArrayType;
            if (type != null)
            {
                return ContainsGenericParameters(type.ElementType);
            }
            PointerType type2 = typeReference as PointerType;
            if (type2 != null)
            {
                return ContainsGenericParameters(type2.ElementType);
            }
            ByReferenceType type3 = typeReference as ByReferenceType;
            if (type3 != null)
            {
                return ContainsGenericParameters(type3.ElementType);
            }
            SentinelType type4 = typeReference as SentinelType;
            if (type4 != null)
            {
                return ContainsGenericParameters(type4.ElementType);
            }
            PinnedType type5 = typeReference as PinnedType;
            if (type5 != null)
            {
                return ContainsGenericParameters(type5.ElementType);
            }
            RequiredModifierType type6 = typeReference as RequiredModifierType;
            if (type6 != null)
            {
                return ContainsGenericParameters(type6.ElementType);
            }
            GenericInstanceType type7 = typeReference as GenericInstanceType;
            if (type7 != null)
            {
                if (<>f__mg$cache1 == null)
                {
                    <>f__mg$cache1 = new Func<TypeReference, bool>(null, (IntPtr) ContainsGenericParameters);
                }
                return Enumerable.Any<TypeReference>(type7.GenericArguments, <>f__mg$cache1);
            }
            if (typeReference is TypeSpecification)
            {
                throw new NotSupportedException();
            }
            return false;
        }

        [Extension]
        public static bool DerivesFrom(TypeReference type, TypeReference potentialBaseType, [Optional, DefaultParameterValue(true)] bool checkInterfaces)
        {
            while (type != null)
            {
                if (Unity.IL2CPP.Common.TypeReferenceEqualityComparer.AreEqual(type, potentialBaseType, TypeComparisonMode.Exact))
                {
                    return true;
                }
                if (checkInterfaces)
                {
                    foreach (TypeReference reference in GetInterfaces(type))
                    {
                        if (Unity.IL2CPP.Common.TypeReferenceEqualityComparer.AreEqual(reference, potentialBaseType, TypeComparisonMode.Exact))
                        {
                            return true;
                        }
                    }
                }
                type = GetBaseType(type);
            }
            return false;
        }

        [Extension]
        public static bool DerivesFromObject(TypeReference typeReference)
        {
            TypeReference baseType = GetBaseType(typeReference);
            if (baseType == null)
            {
                return false;
            }
            return (baseType.MetadataType == MetadataType.Object);
        }

        [Extension]
        public static TypeReference ExtractDefaultInterface(TypeDefinition type)
        {
            if (!type.IsWindowsRuntime)
            {
                throw new ArgumentException(string.Format("Extracting default interface is only valid for Windows Runtime types. {0} is not a Windows Runtime type.", type.FullName));
            }
            foreach (InterfaceImplementation implementation in type.Interfaces)
            {
                foreach (CustomAttribute attribute in implementation.CustomAttributes)
                {
                    if (attribute.AttributeType.FullName == "Windows.Foundation.Metadata.DefaultAttribute")
                    {
                        return implementation.InterfaceType;
                    }
                }
            }
            throw new InvalidProgramException(string.Format("Windows Runtime class {0} has no default interface!", type));
        }

        [Extension]
        public static IEnumerable<TypeReference> GetActivationFactoryTypes(TypeReference type)
        {
            TypeDefinition definition = type.Resolve();
            if (!definition.IsWindowsRuntime || definition.IsValueType)
            {
                return Enumerable.Empty<TypeReference>();
            }
            if (<>f__am$cache6 == null)
            {
                <>f__am$cache6 = new Func<CustomAttribute, TypeReference>(null, (IntPtr) <GetActivationFactoryTypes>m__6);
            }
            return GetTypesFromSpecificAttribute(definition, "Windows.Foundation.Metadata.ActivatableAttribute", <>f__am$cache6);
        }

        [Extension]
        public static IEnumerable<TypeReference> GetAllFactoryTypes(TypeReference type)
        {
            TypeDefinition definition = type.Resolve();
            if (!definition.IsWindowsRuntime || definition.IsValueType)
            {
                return Enumerable.Empty<TypeReference>();
            }
            return Enumerable.Distinct<TypeReference>(Enumerable.Concat<TypeReference>(Enumerable.Concat<TypeReference>(GetActivationFactoryTypes(definition), GetComposableFactoryTypes(definition)), GetStaticFactoryTypes(definition)), new Unity.IL2CPP.Common.TypeReferenceEqualityComparer());
        }

        [Extension]
        public static TypeReference GetBaseType(TypeReference typeReference)
        {
            if (typeReference is TypeSpecification)
            {
                if (typeReference.IsArray)
                {
                    return TypeProvider.SystemArray;
                }
                if ((typeReference.IsGenericParameter || typeReference.IsByReference) || typeReference.IsPointer)
                {
                    return null;
                }
                SentinelType type = typeReference as SentinelType;
                if (type != null)
                {
                    return GetBaseType(type.ElementType);
                }
                PinnedType type2 = typeReference as PinnedType;
                if (type2 != null)
                {
                    return GetBaseType(type2.ElementType);
                }
                RequiredModifierType type3 = typeReference as RequiredModifierType;
                if (type3 != null)
                {
                    return GetBaseType(type3.ElementType);
                }
            }
            return Unity.IL2CPP.ILPreProcessor.TypeResolver.For(typeReference).Resolve(typeReference.Resolve().BaseType);
        }

        [Extension]
        public static IEnumerable<TypeReference> GetComposableFactoryTypes(TypeReference type)
        {
            TypeDefinition definition = type.Resolve();
            if (!definition.IsWindowsRuntime || definition.IsValueType)
            {
                return Enumerable.Empty<TypeReference>();
            }
            if (<>f__am$cache7 == null)
            {
                <>f__am$cache7 = new Func<CustomAttribute, TypeReference>(null, (IntPtr) <GetComposableFactoryTypes>m__7);
            }
            return GetTypesFromSpecificAttribute(definition, "Windows.Foundation.Metadata.ComposableAttribute", <>f__am$cache7);
        }

        [Extension]
        public static IEnumerable<CustomAttribute> GetConstructibleCustomAttributes(ICustomAttributeProvider customAttributeProvider)
        {
            if (<>f__am$cacheD == null)
            {
                <>f__am$cacheD = new Func<CustomAttribute, bool>(null, (IntPtr) <GetConstructibleCustomAttributes>m__F);
            }
            return Enumerable.Where<CustomAttribute>(customAttributeProvider.CustomAttributes, <>f__am$cacheD);
        }

        [Extension]
        public static Guid GetGuid(TypeReference type)
        {
            return GuidProvider.GuidFor(type);
        }

        [Extension]
        public static ReadOnlyCollection<TypeReference> GetInterfaces(TypeReference type)
        {
            HashSet<TypeReference> interfaces = new HashSet<TypeReference>(new Unity.IL2CPP.Common.TypeReferenceEqualityComparer());
            AddInterfacesRecursive(type, interfaces);
            return Enumerable.ToList<TypeReference>(interfaces).AsReadOnly();
        }

        [Extension]
        public static ReadOnlyCollection<MethodReference> GetMethods(TypeReference type)
        {
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = new Func<MethodDefinition, bool>(null, (IntPtr) <GetMethods>m__1);
            }
            return GetMethods(type, <>f__am$cache1);
        }

        [Extension]
        private static ReadOnlyCollection<MethodReference> GetMethods(TypeReference type, Func<MethodDefinition, bool> filter)
        {
            Unity.IL2CPP.ILPreProcessor.TypeResolver resolver = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(type);
            List<MethodReference> list = new List<MethodReference>();
            foreach (MethodDefinition definition in Enumerable.Where<MethodDefinition>(type.Resolve().Methods, filter))
            {
                list.Add(resolver.Resolve(definition));
            }
            return list.AsReadOnly();
        }

        [Extension]
        public static TypeReference GetNonPinnedAndNonByReferenceType(TypeReference type)
        {
            type = Naming.RemoveModifiers(type);
            TypeReference elementType = type;
            ByReferenceType type2 = type as ByReferenceType;
            if (type2 != null)
            {
                elementType = type2.ElementType;
            }
            PinnedType type3 = type as PinnedType;
            if (type3 != null)
            {
                elementType = type3.ElementType;
            }
            return elementType;
        }

        [Extension]
        public static MethodReference GetOverridenInterfaceMethod(MethodReference overridingMethod, IEnumerable<TypeReference> candidateInterfaces)
        {
            <GetOverridenInterfaceMethod>c__AnonStorey2 storey = new <GetOverridenInterfaceMethod>c__AnonStorey2 {
                overridingMethod = overridingMethod
            };
            MethodDefinition definition = storey.overridingMethod.Resolve();
            if (definition.Overrides.Count > 0)
            {
                if (definition.Overrides.Count != 1)
                {
                    throw new InvalidOperationException(string.Format("Cannot choose overriden method for '{0}'", storey.overridingMethod.FullName));
                }
                return Unity.IL2CPP.ILPreProcessor.TypeResolver.For(storey.overridingMethod.DeclaringType, storey.overridingMethod).Resolve(definition.Overrides[0]);
            }
            if (<>f__am$cache8 == null)
            {
                <>f__am$cache8 = new Func<TypeReference, IEnumerable<MethodReference>>(null, (IntPtr) <GetOverridenInterfaceMethod>m__8);
            }
            return Enumerable.FirstOrDefault<MethodReference>(Enumerable.SelectMany<TypeReference, MethodReference>(candidateInterfaces, <>f__am$cache8), new Func<MethodReference, bool>(storey, (IntPtr) this.<>m__0));
        }

        [Extension]
        public static IEnumerable<TypeReference> GetStaticFactoryTypes(TypeReference type)
        {
            TypeDefinition definition = type.Resolve();
            if (!definition.IsWindowsRuntime || definition.IsValueType)
            {
                return Enumerable.Empty<TypeReference>();
            }
            if (<>f__am$cache5 == null)
            {
                <>f__am$cache5 = new Func<CustomAttribute, TypeReference>(null, (IntPtr) <GetStaticFactoryTypes>m__5);
            }
            return GetTypesFromSpecificAttribute(definition, "Windows.Foundation.Metadata.StaticAttribute", <>f__am$cache5);
        }

        [Extension, DebuggerHidden]
        public static IEnumerable<TypeDefinition> GetTypeHierarchy(TypeDefinition type)
        {
            return new <GetTypeHierarchy>c__Iterator0 { 
                type = type,
                <$>type = type,
                $PC = -2
            };
        }

        [Extension]
        private static IEnumerable<TypeReference> GetTypesFromSpecificAttribute(TypeDefinition type, string attributeName, Func<CustomAttribute, TypeReference> customAttributeSelector)
        {
            <GetTypesFromSpecificAttribute>c__AnonStorey1 storey = new <GetTypesFromSpecificAttribute>c__AnonStorey1 {
                attributeName = attributeName
            };
            return Enumerable.Select<CustomAttribute, TypeReference>(Enumerable.Where<CustomAttribute>(type.CustomAttributes, new Func<CustomAttribute, bool>(storey, (IntPtr) this.<>m__0)), customAttributeSelector);
        }

        [Extension]
        public static TypeReference GetUnderlyingEnumType(TypeReference type)
        {
            TypeDefinition definition = type.Resolve();
            if (definition == null)
            {
                throw new Exception("Failed to resolve type reference");
            }
            if (!definition.IsEnum)
            {
                throw new ArgumentException("Attempting to retrieve underlying enum type for non-enum type.", "type");
            }
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = new Func<FieldDefinition, bool>(null, (IntPtr) <GetUnderlyingEnumType>m__2);
            }
            return Enumerable.Single<FieldDefinition>(definition.Fields, <>f__am$cache2).FieldType;
        }

        [Extension]
        public static ReadOnlyCollection<MethodReference> GetVirtualMethods(TypeReference type)
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<MethodDefinition, bool>(null, (IntPtr) <GetVirtualMethods>m__0);
            }
            return GetMethods(type, <>f__am$cache0);
        }

        [Extension]
        public static bool HasActivationFactories(TypeReference type)
        {
            TypeDefinition definition = type.Resolve();
            if (!definition.IsWindowsRuntime || definition.IsValueType)
            {
                return false;
            }
            if (<>f__am$cache4 == null)
            {
                <>f__am$cache4 = new Func<CustomAttribute, bool>(null, (IntPtr) <HasActivationFactories>m__4);
            }
            return Enumerable.Any<CustomAttribute>(definition.CustomAttributes, <>f__am$cache4);
        }

        [Extension]
        public static bool HasFinalizer(TypeDefinition type)
        {
            if (type.IsInterface)
            {
                return false;
            }
            if (type.MetadataType == MetadataType.Object)
            {
                return false;
            }
            if (type.BaseType == null)
            {
                return false;
            }
            if (!HasFinalizer(type.BaseType.Resolve()))
            {
            }
            return ((<>f__mg$cache0 != null) || (Enumerable.SingleOrDefault<MethodDefinition>(type.Methods, <>f__mg$cache0) != null));
        }

        [Extension]
        public static bool HasStaticConstructor(TypeReference typeReference)
        {
            TypeDefinition definition = typeReference.Resolve();
            if (definition == null)
            {
            }
            return ((<>f__mg$cache2 == null) && (Enumerable.SingleOrDefault<MethodDefinition>(definition.Methods, <>f__mg$cache2) != null));
        }

        [Extension]
        public static bool HasStaticFields(TypeReference typeReference)
        {
            if (typeReference.IsArray)
            {
                return false;
            }
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = new Func<FieldDefinition, bool>(null, (IntPtr) <HasStaticFields>m__3);
            }
            return Enumerable.Any<FieldDefinition>(typeReference.Resolve().Fields, <>f__am$cache3);
        }

        [Extension]
        public static IEnumerable<TypeReference> ImplementedComOrWindowsRuntimeInterfaces(TypeDefinition typeDefinition)
        {
            List<TypeReference> list = new List<TypeReference>();
            foreach (InterfaceImplementation implementation in typeDefinition.Interfaces)
            {
                if (IsComOrWindowsRuntimeInterface(implementation.InterfaceType))
                {
                    list.Add(implementation.InterfaceType);
                }
            }
            return list;
        }

        [Extension]
        public static bool IsAttribute(TypeReference type)
        {
            if (type.FullName == "System.Attribute")
            {
                return true;
            }
            TypeDefinition definition = type.Resolve();
            return (((definition != null) && (definition.BaseType != null)) && IsAttribute(definition.BaseType));
        }

        [Extension]
        public static bool IsComInterface(TypeReference type)
        {
            if (type.IsArray)
            {
                return false;
            }
            if (type.IsGenericParameter)
            {
                return false;
            }
            TypeDefinition definition = type.Resolve();
            return ((((definition != null) && definition.IsInterface) && definition.IsImport) && !IsWindowsRuntimeProjection(definition));
        }

        [Extension]
        public static bool IsComOrWindowsRuntimeInterface(MethodDefinition method)
        {
            if (!IsComOrWindowsRuntimeMethod(method))
            {
                return false;
            }
            return method.DeclaringType.IsInterface;
        }

        [Extension]
        public static bool IsComOrWindowsRuntimeInterface(MethodReference method)
        {
            return IsComOrWindowsRuntimeInterface(method.Resolve());
        }

        [Extension]
        public static bool IsComOrWindowsRuntimeInterface(TypeReference type)
        {
            if (type.IsArray)
            {
                return false;
            }
            if (type.IsGenericParameter)
            {
                return false;
            }
            TypeDefinition definition = type.Resolve();
            if (definition == null)
            {
                return false;
            }
            if (!definition.IsInterface)
            {
                return false;
            }
            return IsComOrWindowsRuntimeType(definition);
        }

        [Extension]
        public static bool IsComOrWindowsRuntimeMethod(MethodDefinition method)
        {
            TypeDefinition declaringType = method.DeclaringType;
            if (declaringType.IsWindowsRuntime)
            {
                return true;
            }
            if (IsIl2CppComObject(declaringType))
            {
                return true;
            }
            if (!declaringType.IsImport)
            {
                return false;
            }
            return ((method.IsInternalCall || IsFinalizerMethod(method)) || declaringType.IsInterface);
        }

        [Extension]
        public static bool IsComOrWindowsRuntimeType(TypeDefinition type)
        {
            if (type.IsValueType)
            {
                return false;
            }
            if (IsDelegate(type))
            {
                return false;
            }
            return (IsIl2CppComObject(type) || (type.IsImport || type.IsWindowsRuntime));
        }

        [Extension]
        public static bool IsDefinedInMscorlib(MemberReference memberReference)
        {
            return (memberReference.Module.Assembly.Name.Name == "mscorlib");
        }

        [Extension]
        public static bool IsDefinedInUnityEngine(MemberReference memberReference)
        {
            return memberReference.Module.Assembly.Name.Name.Contains("UnityEngine");
        }

        [Extension]
        public static bool IsDelegate(TypeDefinition type)
        {
            return ((type.BaseType != null) && (type.BaseType.FullName == "System.MulticastDelegate"));
        }

        [Extension]
        public static bool IsEnum(TypeReference type)
        {
            if (type.IsArray)
            {
                return false;
            }
            if (type.IsGenericParameter)
            {
                return false;
            }
            TypeDefinition definition = type.Resolve();
            if (definition == null)
            {
                throw new Exception("Failed to resolve type reference");
            }
            return definition.IsEnum;
        }

        [Extension]
        public static bool IsFinalizerMethod(MethodDefinition method)
        {
            return ((((method.Name == "Finalize") && (method.ReturnType.MetadataType == MetadataType.Void)) && !method.HasParameters) && (((ushort) (method.Attributes & (MethodAttributes.CompilerControlled | MethodAttributes.Family))) != 0));
        }

        [Extension]
        public static bool IsGenericParameter(TypeReference typeReference)
        {
            if (typeReference is ArrayType)
            {
                return false;
            }
            if (typeReference is PointerType)
            {
                return false;
            }
            if (typeReference is ByReferenceType)
            {
                return false;
            }
            return typeReference.GetElementType().IsGenericParameter;
        }

        [Extension]
        public static bool IsIActivationFactory(TypeReference typeReference)
        {
            return Unity.IL2CPP.Common.TypeReferenceEqualityComparer.AreEqual(typeReference, TypeProvider.IActivationFactoryTypeReference, TypeComparisonMode.Exact);
        }

        [Extension]
        public static bool IsIl2CppComObject(TypeReference typeReference)
        {
            return Unity.IL2CPP.Common.TypeReferenceEqualityComparer.AreEqual(typeReference, TypeProvider.Il2CppComObjectTypeReference, TypeComparisonMode.Exact);
        }

        [Extension]
        public static bool IsIntegralPointerType(TypeReference typeReference)
        {
            return ((typeReference.MetadataType == MetadataType.IntPtr) || (typeReference.MetadataType == MetadataType.UIntPtr));
        }

        [Extension]
        public static bool IsIntegralType(TypeReference type)
        {
            return (IsSignedIntegralType(type) || IsUnsignedIntegralType(type));
        }

        [Extension]
        public static bool IsInterface(TypeReference type)
        {
            if (type.IsArray)
            {
                return false;
            }
            if (type.IsGenericParameter)
            {
                return false;
            }
            TypeDefinition definition = type.Resolve();
            return ((definition != null) && definition.IsInterface);
        }

        [Extension]
        public static bool IsNativeIntegralType(TypeReference typeReference)
        {
            return (Unity.IL2CPP.Common.TypeReferenceEqualityComparer.AreEqual(typeReference, TypeProvider.NativeIntTypeReference, TypeComparisonMode.Exact) || Unity.IL2CPP.Common.TypeReferenceEqualityComparer.AreEqual(typeReference, TypeProvider.NativeUIntTypeReference, TypeComparisonMode.Exact));
        }

        [Extension]
        public static bool IsNormalStatic(FieldReference field)
        {
            FieldDefinition definition = field.Resolve();
            if (definition.IsLiteral)
            {
                return false;
            }
            if (!definition.IsStatic)
            {
                return false;
            }
            if (!definition.HasCustomAttributes)
            {
                return true;
            }
            if (<>f__am$cache9 == null)
            {
                <>f__am$cache9 = new Func<CustomAttribute, bool>(null, (IntPtr) <IsNormalStatic>m__B);
            }
            return Enumerable.All<CustomAttribute>(definition.CustomAttributes, <>f__am$cache9);
        }

        [Extension]
        public static bool IsNullable(TypeReference type)
        {
            if (type.IsArray)
            {
                return false;
            }
            if (type.IsGenericParameter)
            {
                return false;
            }
            GenericInstanceType type2 = type as GenericInstanceType;
            if (type2 == null)
            {
                return false;
            }
            return (type2.ElementType.FullName == "System.Nullable`1");
        }

        [Extension]
        public static bool IsPrimitiveCppType(string typeName)
        {
            if (typeName != null)
            {
                int num;
                if (<>f__switch$map1 == null)
                {
                    Dictionary<string, int> dictionary = new Dictionary<string, int>(14) {
                        { 
                            "bool",
                            0
                        },
                        { 
                            "char",
                            0
                        },
                        { 
                            "wchar_t",
                            0
                        },
                        { 
                            "size_t",
                            0
                        },
                        { 
                            "int8_t",
                            0
                        },
                        { 
                            "int16_t",
                            0
                        },
                        { 
                            "int32_t",
                            0
                        },
                        { 
                            "int64_t",
                            0
                        },
                        { 
                            "uint8_t",
                            0
                        },
                        { 
                            "uint16_t",
                            0
                        },
                        { 
                            "uint32_t",
                            0
                        },
                        { 
                            "uint64_t",
                            0
                        },
                        { 
                            "double",
                            0
                        },
                        { 
                            "float",
                            0
                        }
                    };
                    <>f__switch$map1 = dictionary;
                }
                if (<>f__switch$map1.TryGetValue(typeName, out num) && (num == 0))
                {
                    return true;
                }
            }
            return false;
        }

        [Extension]
        public static bool IsPrimitiveType(MetadataType type)
        {
            switch (type)
            {
                case MetadataType.Boolean:
                case MetadataType.Char:
                case MetadataType.SByte:
                case MetadataType.Byte:
                case MetadataType.Int16:
                case MetadataType.UInt16:
                case MetadataType.Int32:
                case MetadataType.UInt32:
                case MetadataType.Int64:
                case MetadataType.UInt64:
                case MetadataType.Single:
                case MetadataType.Double:
                    return true;
            }
            return false;
        }

        [Extension]
        public static bool IsSameType(TypeReference a, TypeReference b)
        {
            return Unity.IL2CPP.Common.TypeReferenceEqualityComparer.AreEqual(a, b, TypeComparisonMode.Exact);
        }

        [Extension]
        public static bool IsSignedIntegralType(TypeReference type)
        {
            return ((((type.MetadataType == MetadataType.SByte) || (type.MetadataType == MetadataType.Int16)) || (type.MetadataType == MetadataType.Int32)) || (type.MetadataType == MetadataType.Int64));
        }

        [Extension]
        public static bool IsSpecialSystemBaseType(TypeReference typeReference)
        {
            return (((typeReference.FullName == "System.Object") || (typeReference.FullName == "System.ValueType")) || (typeReference.FullName == "System.Enum"));
        }

        [Extension]
        public static bool IsStaticConstructor(MethodReference methodReference)
        {
            MethodDefinition definition = methodReference.Resolve();
            if (definition == null)
            {
                return false;
            }
            return ((definition.IsConstructor && definition.IsStatic) && (definition.Parameters.Count == 0));
        }

        [Extension]
        public static bool IsStripped(MethodReference method)
        {
            return method.Name.StartsWith("$__Stripped");
        }

        [Extension]
        public static bool IsStructWithNoInstanceFields(TypeReference typeReference)
        {
            System.Boolean ReflectorVariable0;
            if (!IsValueType(typeReference) || IsEnum(typeReference))
            {
                return false;
            }
            TypeDefinition definition = typeReference.Resolve();
            if (definition != null)
            {
                if (definition.HasFields)
                {
                }
                ReflectorVariable0 = true;
            }
            else
            {
                ReflectorVariable0 = false;
            }
            return (ReflectorVariable0 ? ((<>f__am$cacheB != null) || Enumerable.All<FieldDefinition>(definition.Fields, <>f__am$cacheB)) : false);
        }

        [Extension]
        public static bool IsSystemArray(TypeReference typeReference)
        {
            return ((typeReference.FullName == "System.Array") && (typeReference.Resolve().Module.Name == "mscorlib.dll"));
        }

        [Extension]
        public static bool IsSystemObject(TypeReference typeReference)
        {
            return (typeReference.MetadataType == MetadataType.Object);
        }

        [Extension]
        public static bool IsSystemType(TypeReference typeReference)
        {
            return ((typeReference.FullName == "System.Type") && (typeReference.Resolve().Module.Name == "mscorlib.dll"));
        }

        [Extension]
        public static bool IsThreadStatic(FieldReference field)
        {
            FieldDefinition definition = field.Resolve();
            if (definition.IsStatic && definition.HasCustomAttributes)
            {
            }
            return ((<>f__am$cacheA == null) && Enumerable.Any<CustomAttribute>(definition.CustomAttributes, <>f__am$cacheA));
        }

        [Extension]
        public static bool IsUnsignedIntegralType(TypeReference type)
        {
            return ((((type.MetadataType == MetadataType.Byte) || (type.MetadataType == MetadataType.UInt16)) || (type.MetadataType == MetadataType.UInt32)) || (type.MetadataType == MetadataType.UInt64));
        }

        [Extension]
        public static bool IsValueType(TypeReference typeReference)
        {
            if (typeReference.IsValueType)
            {
                return true;
            }
            if (typeReference is ArrayType)
            {
                return false;
            }
            if (typeReference is PointerType)
            {
                return false;
            }
            if (typeReference is ByReferenceType)
            {
                return false;
            }
            if (typeReference is GenericParameter)
            {
                return false;
            }
            PinnedType type = typeReference as PinnedType;
            if (type != null)
            {
                return IsValueType(type.ElementType);
            }
            return typeReference.Resolve().IsValueType;
        }

        [Extension]
        public static bool IsVoid(TypeReference type)
        {
            return (type.MetadataType == MetadataType.Void);
        }

        [Extension]
        public static bool IsVolatile(FieldReference fieldReference)
        {
            return (((fieldReference != null) && fieldReference.FieldType.IsRequiredModifier) && ((RequiredModifierType) fieldReference.FieldType).ModifierType.Name.Contains("IsVolatile"));
        }

        [Extension]
        public static bool IsWindowsRuntimeProjection(TypeDefinition type)
        {
            return type.IsWindowsRuntimeProjection;
        }

        [Extension]
        public static bool IsWindowsRuntimeProjection(TypeReference type)
        {
            return type.GetElementType().IsWindowsRuntimeProjection;
        }

        [Extension]
        public static bool NeedsComCallableWrapper(TypeDefinition type)
        {
            if (!IsComOrWindowsRuntimeType(type))
            {
                if (Enumerable.Any<TypeReference>(ImplementedComOrWindowsRuntimeInterfaces(type)))
                {
                    return true;
                }
                while (type.BaseType != null)
                {
                    type = type.BaseType.Resolve();
                    if (Enumerable.Any<TypeReference>(ImplementedComOrWindowsRuntimeInterfaces(type)) || IsComOrWindowsRuntimeType(type))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        [Extension]
        public static bool References(AssemblyDefinition assemblyDoingTheReferencing, AssemblyDefinition assemblyBeingReference)
        {
            if (<>f__am$cacheC == null)
            {
                <>f__am$cacheC = new Func<AssemblyDefinition, string>(null, (IntPtr) <References>m__E);
            }
            return Enumerable.Contains<string>(Enumerable.Select<AssemblyDefinition, string>(AssemblyDependencies.GetReferencedAssembliesFor(assemblyDoingTheReferencing), <>f__am$cacheC), assemblyBeingReference.Name.Name);
        }

        [Extension]
        public static bool ShouldProcessAsInternalCall(MethodReference methodReference)
        {
            MethodDefinition definition = methodReference.Resolve();
            return ((definition != null) && (definition.IsInternalCall && !definition.HasGenericParameters));
        }

        [Extension]
        public static bool StoresNonFieldsInStaticFields(TypeReference type)
        {
            return HasActivationFactories(type);
        }

        [Extension]
        public static string ToInitializer(Guid guid)
        {
            byte[] buffer = guid.ToByteArray();
            uint num = BitConverter.ToUInt32(buffer, 0);
            ushort num2 = BitConverter.ToUInt16(buffer, 4);
            ushort num3 = BitConverter.ToUInt16(buffer, 6);
            return ('{' + string.Format(" 0x{0:x}, 0x{1:x}, 0x{2:x}, 0x{3:x}, 0x{4:x}, 0x{5:x}, 0x{6:x}, 0x{7:x}, 0x{8:x}, 0x{9:x}, 0x{10:x} ", new object[] { num, num2, num3, buffer[8], buffer[9], buffer[10], buffer[11], buffer[12], buffer[13], buffer[14], buffer[15] }) + '}');
        }

        [CompilerGenerated]
        private sealed class <Chunk>c__AnonStorey3<T>
        {
            internal int size;

            internal int <>m__0(Extensions.ChunkItem<T> x)
            {
                return (x.Index / this.size);
            }
        }

        [CompilerGenerated]
        private sealed class <GetOverridenInterfaceMethod>c__AnonStorey2
        {
            internal MethodReference overridingMethod;

            internal bool <>m__0(MethodReference interfaceMethod)
            {
                return ((this.overridingMethod.Name == interfaceMethod.Name) && VirtualMethodResolution.MethodSignaturesMatchIgnoreStaticness(interfaceMethod, this.overridingMethod));
            }
        }

        [CompilerGenerated]
        private sealed class <GetTypeHierarchy>c__Iterator0 : IEnumerable, IEnumerable<TypeDefinition>, IEnumerator, IDisposable, IEnumerator<TypeDefinition>
        {
            internal TypeDefinition $current;
            internal bool $disposing;
            internal int $PC;
            internal TypeDefinition <$>type;
            internal TypeDefinition type;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$disposing = true;
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        break;

                    case 1:
                        this.type = (this.type.BaseType == null) ? null : this.type.BaseType.Resolve();
                        break;

                    default:
                        goto Label_0087;
                }
                if (this.type != null)
                {
                    this.$current = this.type;
                    if (!this.$disposing)
                    {
                        this.$PC = 1;
                    }
                    return true;
                }
                this.$PC = -1;
            Label_0087:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<TypeDefinition> IEnumerable<TypeDefinition>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new Extensions.<GetTypeHierarchy>c__Iterator0 { type = this.<$>type };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.System.Collections.Generic.IEnumerable<Mono.Cecil.TypeDefinition>.GetEnumerator();
            }

            TypeDefinition IEnumerator<TypeDefinition>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <GetTypesFromSpecificAttribute>c__AnonStorey1
        {
            internal string attributeName;

            internal bool <>m__0(CustomAttribute ca)
            {
                return (ca.AttributeType.FullName == this.attributeName);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ChunkItem<T>
        {
            public int Index;
            public T Value;
        }
    }
}

