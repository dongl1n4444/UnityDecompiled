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
        private static T <Chunk`1>m__10<T>(ChunkItem<T> x) => 
            x.Value;

        [CompilerGenerated]
        private static ChunkItem<T> <Chunk`1>m__9<T>(T value, int index) => 
            new ChunkItem<T> { 
                Index = index,
                Value = value
            };

        [CompilerGenerated]
        private static List<T> <Chunk`1>m__A<T>(IGrouping<int, ChunkItem<T>> g) => 
            (from x in g select x.Value).ToList<T>();

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

        public static List<List<T>> Chunk<T>(this IEnumerable<T> foo, int size)
        {
            <Chunk>c__AnonStorey3<T> storey = new <Chunk>c__AnonStorey3<T> {
                size = size
            };
            return foo.Select<T, ChunkItem<T>>(new Func<T, int, ChunkItem<T>>(Extensions.<Chunk`1>m__9<T>)).GroupBy<ChunkItem<T>, int>(new Func<ChunkItem<T>, int>(storey.<>m__0)).Select<IGrouping<int, ChunkItem<T>>, List<T>>(new Func<IGrouping<int, ChunkItem<T>>, List<T>>(Extensions.<Chunk`1>m__A<T>)).ToList<List<T>>();
        }

        public static bool ContainsGenericParameters(this MethodReference method)
        {
            if (method.DeclaringType.ContainsGenericParameters())
            {
                return true;
            }
            GenericInstanceMethod method2 = method as GenericInstanceMethod;
            if (method2 != null)
            {
                foreach (TypeReference reference in method2.GenericArguments)
                {
                    if (reference.ContainsGenericParameters())
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool ContainsGenericParameters(this TypeReference typeReference)
        {
            if (typeReference is GenericParameter)
            {
                return true;
            }
            ArrayType type = typeReference as ArrayType;
            if (type != null)
            {
                return type.ElementType.ContainsGenericParameters();
            }
            PointerType type2 = typeReference as PointerType;
            if (type2 != null)
            {
                return type2.ElementType.ContainsGenericParameters();
            }
            ByReferenceType type3 = typeReference as ByReferenceType;
            if (type3 != null)
            {
                return type3.ElementType.ContainsGenericParameters();
            }
            SentinelType type4 = typeReference as SentinelType;
            if (type4 != null)
            {
                return type4.ElementType.ContainsGenericParameters();
            }
            PinnedType type5 = typeReference as PinnedType;
            if (type5 != null)
            {
                return type5.ElementType.ContainsGenericParameters();
            }
            RequiredModifierType type6 = typeReference as RequiredModifierType;
            if (type6 != null)
            {
                return type6.ElementType.ContainsGenericParameters();
            }
            GenericInstanceType type7 = typeReference as GenericInstanceType;
            if (type7 != null)
            {
                if (<>f__mg$cache1 == null)
                {
                    <>f__mg$cache1 = new Func<TypeReference, bool>(Extensions.ContainsGenericParameters);
                }
                return type7.GenericArguments.Any<TypeReference>(<>f__mg$cache1);
            }
            if (typeReference is TypeSpecification)
            {
                throw new NotSupportedException();
            }
            return false;
        }

        public static bool DerivesFrom(this TypeReference type, TypeReference potentialBaseType, bool checkInterfaces = true)
        {
            while (type != null)
            {
                if (Unity.IL2CPP.Common.TypeReferenceEqualityComparer.AreEqual(type, potentialBaseType, TypeComparisonMode.Exact))
                {
                    return true;
                }
                if (checkInterfaces)
                {
                    foreach (TypeReference reference in type.GetInterfaces())
                    {
                        if (Unity.IL2CPP.Common.TypeReferenceEqualityComparer.AreEqual(reference, potentialBaseType, TypeComparisonMode.Exact))
                        {
                            return true;
                        }
                    }
                }
                type = type.GetBaseType();
            }
            return false;
        }

        public static bool DerivesFromObject(this TypeReference typeReference)
        {
            TypeReference baseType = typeReference.GetBaseType();
            return (baseType?.MetadataType == MetadataType.Object);
        }

        public static TypeReference ExtractDefaultInterface(this TypeDefinition type)
        {
            if (!type.IsWindowsRuntime)
            {
                throw new ArgumentException($"Extracting default interface is only valid for Windows Runtime types. {type.FullName} is not a Windows Runtime type.");
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
            throw new InvalidProgramException($"Windows Runtime class {type} has no default interface!");
        }

        public static IEnumerable<TypeReference> GetActivationFactoryTypes(this TypeReference type)
        {
            TypeDefinition definition = type.Resolve();
            if (!definition.IsWindowsRuntime || definition.IsValueType)
            {
                return Enumerable.Empty<TypeReference>();
            }
            if (<>f__am$cache6 == null)
            {
                <>f__am$cache6 = delegate (CustomAttribute attribute) {
                    CustomAttributeArgument argument = attribute.ConstructorArguments[0];
                    if (argument.Type.IsSystemType())
                    {
                        return (TypeReference) argument.Value;
                    }
                    return TypeProvider.IActivationFactoryTypeReference;
                };
            }
            return definition.GetTypesFromSpecificAttribute("Windows.Foundation.Metadata.ActivatableAttribute", <>f__am$cache6);
        }

        public static IEnumerable<TypeReference> GetAllFactoryTypes(this TypeReference type)
        {
            TypeDefinition definition = type.Resolve();
            if (!definition.IsWindowsRuntime || definition.IsValueType)
            {
                return Enumerable.Empty<TypeReference>();
            }
            return definition.GetActivationFactoryTypes().Concat<TypeReference>(definition.GetComposableFactoryTypes()).Concat<TypeReference>(definition.GetStaticFactoryTypes()).Distinct<TypeReference>(new Unity.IL2CPP.Common.TypeReferenceEqualityComparer());
        }

        public static TypeReference GetBaseType(this TypeReference typeReference)
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
                    return type.ElementType.GetBaseType();
                }
                PinnedType type2 = typeReference as PinnedType;
                if (type2 != null)
                {
                    return type2.ElementType.GetBaseType();
                }
                RequiredModifierType type3 = typeReference as RequiredModifierType;
                if (type3 != null)
                {
                    return type3.ElementType.GetBaseType();
                }
            }
            return Unity.IL2CPP.ILPreProcessor.TypeResolver.For(typeReference).Resolve(typeReference.Resolve().BaseType);
        }

        public static IEnumerable<TypeReference> GetComposableFactoryTypes(this TypeReference type)
        {
            TypeDefinition definition = type.Resolve();
            if (!definition.IsWindowsRuntime || definition.IsValueType)
            {
                return Enumerable.Empty<TypeReference>();
            }
            if (<>f__am$cache7 == null)
            {
                <>f__am$cache7 = attribute => (TypeReference) attribute.ConstructorArguments[0].Value;
            }
            return definition.GetTypesFromSpecificAttribute("Windows.Foundation.Metadata.ComposableAttribute", <>f__am$cache7);
        }

        public static IEnumerable<CustomAttribute> GetConstructibleCustomAttributes(this ICustomAttributeProvider customAttributeProvider)
        {
            if (<>f__am$cacheD == null)
            {
                <>f__am$cacheD = delegate (CustomAttribute ca) {
                    TypeDefinition definition = ca.AttributeType.Resolve();
                    return (definition != null) && !definition.IsWindowsRuntime;
                };
            }
            return customAttributeProvider.CustomAttributes.Where<CustomAttribute>(<>f__am$cacheD);
        }

        public static Guid GetGuid(this TypeReference type) => 
            GuidProvider.GuidFor(type);

        public static ReadOnlyCollection<TypeReference> GetInterfaces(this TypeReference type)
        {
            HashSet<TypeReference> interfaces = new HashSet<TypeReference>(new Unity.IL2CPP.Common.TypeReferenceEqualityComparer());
            AddInterfacesRecursive(type, interfaces);
            return interfaces.ToList<TypeReference>().AsReadOnly();
        }

        public static ReadOnlyCollection<MethodReference> GetMethods(this TypeReference type)
        {
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = m => true;
            }
            return type.GetMethods(<>f__am$cache1);
        }

        private static ReadOnlyCollection<MethodReference> GetMethods(this TypeReference type, Func<MethodDefinition, bool> filter)
        {
            Unity.IL2CPP.ILPreProcessor.TypeResolver resolver = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(type);
            List<MethodReference> list = new List<MethodReference>();
            foreach (MethodDefinition definition in type.Resolve().Methods.Where<MethodDefinition>(filter))
            {
                list.Add(resolver.Resolve(definition));
            }
            return list.AsReadOnly();
        }

        public static TypeReference GetNonPinnedAndNonByReferenceType(this TypeReference type)
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

        public static MethodReference GetOverridenInterfaceMethod(this MethodReference overridingMethod, IEnumerable<TypeReference> candidateInterfaces)
        {
            <GetOverridenInterfaceMethod>c__AnonStorey2 storey = new <GetOverridenInterfaceMethod>c__AnonStorey2 {
                overridingMethod = overridingMethod
            };
            MethodDefinition definition = storey.overridingMethod.Resolve();
            if (definition.Overrides.Count > 0)
            {
                if (definition.Overrides.Count != 1)
                {
                    throw new InvalidOperationException($"Cannot choose overriden method for '{storey.overridingMethod.FullName}'");
                }
                return Unity.IL2CPP.ILPreProcessor.TypeResolver.For(storey.overridingMethod.DeclaringType, storey.overridingMethod).Resolve(definition.Overrides[0]);
            }
            if (<>f__am$cache8 == null)
            {
                <>f__am$cache8 = iface => iface.GetMethods();
            }
            return candidateInterfaces.SelectMany<TypeReference, MethodReference>(<>f__am$cache8).FirstOrDefault<MethodReference>(new Func<MethodReference, bool>(storey.<>m__0));
        }

        public static IEnumerable<TypeReference> GetStaticFactoryTypes(this TypeReference type)
        {
            TypeDefinition definition = type.Resolve();
            if (!definition.IsWindowsRuntime || definition.IsValueType)
            {
                return Enumerable.Empty<TypeReference>();
            }
            if (<>f__am$cache5 == null)
            {
                <>f__am$cache5 = attribute => (TypeReference) attribute.ConstructorArguments[0].Value;
            }
            return definition.GetTypesFromSpecificAttribute("Windows.Foundation.Metadata.StaticAttribute", <>f__am$cache5);
        }

        [DebuggerHidden]
        public static IEnumerable<TypeDefinition> GetTypeHierarchy(this TypeDefinition type) => 
            new <GetTypeHierarchy>c__Iterator0 { 
                type = type,
                <$>type = type,
                $PC = -2
            };

        private static IEnumerable<TypeReference> GetTypesFromSpecificAttribute(this TypeDefinition type, string attributeName, Func<CustomAttribute, TypeReference> customAttributeSelector)
        {
            <GetTypesFromSpecificAttribute>c__AnonStorey1 storey = new <GetTypesFromSpecificAttribute>c__AnonStorey1 {
                attributeName = attributeName
            };
            return type.CustomAttributes.Where<CustomAttribute>(new Func<CustomAttribute, bool>(storey.<>m__0)).Select<CustomAttribute, TypeReference>(customAttributeSelector);
        }

        public static TypeReference GetUnderlyingEnumType(this TypeReference type)
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
                <>f__am$cache2 = f => !f.IsStatic && (f.Name == "value__");
            }
            return definition.Fields.Single<FieldDefinition>(<>f__am$cache2).FieldType;
        }

        public static ReadOnlyCollection<MethodReference> GetVirtualMethods(this TypeReference type)
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = m => m.IsVirtual && !m.IsStatic;
            }
            return type.GetMethods(<>f__am$cache0);
        }

        public static bool HasActivationFactories(this TypeReference type)
        {
            TypeDefinition definition = type.Resolve();
            if (!definition.IsWindowsRuntime || definition.IsValueType)
            {
                return false;
            }
            if (<>f__am$cache4 == null)
            {
                <>f__am$cache4 = ca => ((ca.AttributeType.FullName == "Windows.Foundation.Metadata.ActivatableAttribute") || (ca.AttributeType.FullName == "Windows.Foundation.Metadata.StaticAttribute")) || (ca.AttributeType.FullName == "Windows.Foundation.Metadata.ComposableAttribute");
            }
            return definition.CustomAttributes.Any<CustomAttribute>(<>f__am$cache4);
        }

        public static bool HasFinalizer(this TypeDefinition type)
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
            if (!type.BaseType.Resolve().HasFinalizer())
            {
            }
            return ((<>f__mg$cache0 != null) || (type.Methods.SingleOrDefault<MethodDefinition>(<>f__mg$cache0) != null));
        }

        public static bool HasStaticConstructor(this TypeReference typeReference)
        {
            TypeDefinition definition = typeReference.Resolve();
            if (definition == null)
            {
            }
            return ((<>f__mg$cache2 == null) && (definition.Methods.SingleOrDefault<MethodDefinition>(<>f__mg$cache2) != null));
        }

        public static bool HasStaticFields(this TypeReference typeReference)
        {
            if (typeReference.IsArray)
            {
                return false;
            }
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = f => f.IsStatic;
            }
            return typeReference.Resolve().Fields.Any<FieldDefinition>(<>f__am$cache3);
        }

        public static IEnumerable<TypeReference> ImplementedComOrWindowsRuntimeInterfaces(this TypeDefinition typeDefinition)
        {
            List<TypeReference> list = new List<TypeReference>();
            foreach (InterfaceImplementation implementation in typeDefinition.Interfaces)
            {
                if (implementation.InterfaceType.IsComOrWindowsRuntimeInterface())
                {
                    list.Add(implementation.InterfaceType);
                }
            }
            return list;
        }

        public static bool IsAttribute(this TypeReference type)
        {
            if (type.FullName == "System.Attribute")
            {
                return true;
            }
            TypeDefinition definition = type.Resolve();
            return (((definition != null) && (definition.BaseType != null)) && definition.BaseType.IsAttribute());
        }

        public static bool IsComInterface(this TypeReference type)
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
            return ((((definition != null) && definition.IsInterface) && definition.IsImport) && !definition.IsWindowsRuntimeProjection());
        }

        public static bool IsComOrWindowsRuntimeInterface(this MethodDefinition method)
        {
            if (!method.IsComOrWindowsRuntimeMethod())
            {
                return false;
            }
            return method.DeclaringType.IsInterface;
        }

        public static bool IsComOrWindowsRuntimeInterface(this MethodReference method) => 
            method.Resolve().IsComOrWindowsRuntimeInterface();

        public static bool IsComOrWindowsRuntimeInterface(this TypeReference type)
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
            return definition.IsComOrWindowsRuntimeType();
        }

        public static bool IsComOrWindowsRuntimeMethod(this MethodDefinition method)
        {
            TypeDefinition declaringType = method.DeclaringType;
            if (declaringType.IsWindowsRuntime)
            {
                return true;
            }
            if (declaringType.IsIl2CppComObject())
            {
                return true;
            }
            if (!declaringType.IsImport)
            {
                return false;
            }
            return ((method.IsInternalCall || method.IsFinalizerMethod()) || declaringType.IsInterface);
        }

        public static bool IsComOrWindowsRuntimeType(this TypeDefinition type)
        {
            if (type.IsValueType)
            {
                return false;
            }
            if (type.IsDelegate())
            {
                return false;
            }
            return (type.IsIl2CppComObject() || (type.IsImport || type.IsWindowsRuntime));
        }

        public static bool IsDefinedInMscorlib(this MemberReference memberReference) => 
            (memberReference.Module.Assembly.Name.Name == "mscorlib");

        public static bool IsDefinedInUnityEngine(this MemberReference memberReference) => 
            memberReference.Module.Assembly.Name.Name.Contains("UnityEngine");

        public static bool IsDelegate(this TypeDefinition type) => 
            ((type.BaseType != null) && (type.BaseType.FullName == "System.MulticastDelegate"));

        public static bool IsEnum(this TypeReference type)
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
            return definition?.IsEnum;
        }

        public static bool IsFinalizerMethod(this MethodDefinition method) => 
            ((((method.Name == "Finalize") && (method.ReturnType.MetadataType == MetadataType.Void)) && !method.HasParameters) && (((ushort) (method.Attributes & (MethodAttributes.CompilerControlled | MethodAttributes.Family))) != 0));

        public static bool IsGenericParameter(this TypeReference typeReference)
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

        public static bool IsIActivationFactory(this TypeReference typeReference) => 
            Unity.IL2CPP.Common.TypeReferenceEqualityComparer.AreEqual(typeReference, TypeProvider.IActivationFactoryTypeReference, TypeComparisonMode.Exact);

        public static bool IsIl2CppComObject(this TypeReference typeReference) => 
            Unity.IL2CPP.Common.TypeReferenceEqualityComparer.AreEqual(typeReference, TypeProvider.Il2CppComObjectTypeReference, TypeComparisonMode.Exact);

        public static bool IsIntegralPointerType(this TypeReference typeReference) => 
            ((typeReference.MetadataType == MetadataType.IntPtr) || (typeReference.MetadataType == MetadataType.UIntPtr));

        public static bool IsIntegralType(this TypeReference type) => 
            (type.IsSignedIntegralType() || type.IsUnsignedIntegralType());

        public static bool IsInterface(this TypeReference type)
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

        public static bool IsNativeIntegralType(this TypeReference typeReference) => 
            (Unity.IL2CPP.Common.TypeReferenceEqualityComparer.AreEqual(typeReference, TypeProvider.NativeIntTypeReference, TypeComparisonMode.Exact) || Unity.IL2CPP.Common.TypeReferenceEqualityComparer.AreEqual(typeReference, TypeProvider.NativeUIntTypeReference, TypeComparisonMode.Exact));

        public static bool IsNormalStatic(this FieldReference field)
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
                <>f__am$cache9 = ca => ca.AttributeType.Name != "ThreadStaticAttribute";
            }
            return definition.CustomAttributes.All<CustomAttribute>(<>f__am$cache9);
        }

        public static bool IsNullable(this TypeReference type)
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
            return (type2?.ElementType.FullName == "System.Nullable`1");
        }

        public static bool IsPrimitiveCppType(this string typeName)
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

        public static bool IsPrimitiveType(this MetadataType type)
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

        public static bool IsSameType(this TypeReference a, TypeReference b) => 
            Unity.IL2CPP.Common.TypeReferenceEqualityComparer.AreEqual(a, b, TypeComparisonMode.Exact);

        public static bool IsSignedIntegralType(this TypeReference type) => 
            ((((type.MetadataType == MetadataType.SByte) || (type.MetadataType == MetadataType.Int16)) || (type.MetadataType == MetadataType.Int32)) || (type.MetadataType == MetadataType.Int64));

        public static bool IsSpecialSystemBaseType(this TypeReference typeReference) => 
            (((typeReference.FullName == "System.Object") || (typeReference.FullName == "System.ValueType")) || (typeReference.FullName == "System.Enum"));

        public static bool IsStaticConstructor(this MethodReference methodReference)
        {
            MethodDefinition definition = methodReference.Resolve();
            return ((definition?.IsConstructor && definition.IsStatic) && (definition.Parameters.Count == 0));
        }

        public static bool IsStripped(this MethodReference method) => 
            method.Name.StartsWith("$__Stripped");

        public static bool IsStructWithNoInstanceFields(this TypeReference typeReference)
        {
            System.Boolean ReflectorVariable0;
            if (!typeReference.IsValueType() || typeReference.IsEnum())
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
            return (ReflectorVariable0 ? ((<>f__am$cacheB != null) || definition.Fields.All<FieldDefinition>(<>f__am$cacheB)) : false);
        }

        public static bool IsSystemArray(this TypeReference typeReference) => 
            ((typeReference.FullName == "System.Array") && (typeReference.Resolve().Module.Name == "mscorlib.dll"));

        public static bool IsSystemObject(this TypeReference typeReference) => 
            (typeReference.MetadataType == MetadataType.Object);

        public static bool IsSystemType(this TypeReference typeReference) => 
            ((typeReference.FullName == "System.Type") && (typeReference.Resolve().Module.Name == "mscorlib.dll"));

        public static bool IsThreadStatic(this FieldReference field)
        {
            FieldDefinition definition = field.Resolve();
            if (definition.IsStatic && definition.HasCustomAttributes)
            {
            }
            return ((<>f__am$cacheA == null) && definition.CustomAttributes.Any<CustomAttribute>(<>f__am$cacheA));
        }

        public static bool IsUnsignedIntegralType(this TypeReference type) => 
            ((((type.MetadataType == MetadataType.Byte) || (type.MetadataType == MetadataType.UInt16)) || (type.MetadataType == MetadataType.UInt32)) || (type.MetadataType == MetadataType.UInt64));

        public static bool IsValueType(this TypeReference typeReference)
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
                return type.ElementType.IsValueType();
            }
            return typeReference.Resolve().IsValueType;
        }

        public static bool IsVoid(this TypeReference type) => 
            (type.MetadataType == MetadataType.Void);

        public static bool IsVolatile(this FieldReference fieldReference) => 
            (((fieldReference != null) && fieldReference.FieldType.IsRequiredModifier) && ((RequiredModifierType) fieldReference.FieldType).ModifierType.Name.Contains("IsVolatile"));

        public static bool IsWindowsRuntimeProjection(this TypeDefinition type) => 
            type.IsWindowsRuntimeProjection;

        public static bool IsWindowsRuntimeProjection(this TypeReference type) => 
            type.GetElementType().IsWindowsRuntimeProjection;

        public static bool NeedsComCallableWrapper(this TypeDefinition type)
        {
            if (!type.IsComOrWindowsRuntimeType())
            {
                if (type.ImplementedComOrWindowsRuntimeInterfaces().Any<TypeReference>())
                {
                    return true;
                }
                while (type.BaseType != null)
                {
                    type = type.BaseType.Resolve();
                    if (type.ImplementedComOrWindowsRuntimeInterfaces().Any<TypeReference>() || type.IsComOrWindowsRuntimeType())
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool References(this AssemblyDefinition assemblyDoingTheReferencing, AssemblyDefinition assemblyBeingReference)
        {
            if (<>f__am$cacheC == null)
            {
                <>f__am$cacheC = eachAssemblyReference => eachAssemblyReference.Name.Name;
            }
            return AssemblyDependencies.GetReferencedAssembliesFor(assemblyDoingTheReferencing).Select<AssemblyDefinition, string>(<>f__am$cacheC).Contains<string>(assemblyBeingReference.Name.Name);
        }

        public static bool ShouldProcessAsInternalCall(this MethodReference methodReference)
        {
            MethodDefinition definition = methodReference.Resolve();
            return ((definition != null) && (definition.IsInternalCall && !definition.HasGenericParameters));
        }

        public static bool StoresNonFieldsInStaticFields(this TypeReference type) => 
            type.HasActivationFactories();

        public static string ToInitializer(this Guid guid)
        {
            byte[] buffer = guid.ToByteArray();
            uint num = BitConverter.ToUInt32(buffer, 0);
            ushort num2 = BitConverter.ToUInt16(buffer, 4);
            ushort num3 = BitConverter.ToUInt16(buffer, 6);
            return ('{' + $" 0x{num:x}, 0x{num2:x}, 0x{num3:x}, 0x{buffer[8]:x}, 0x{buffer[9]:x}, 0x{buffer[10]:x}, 0x{buffer[11]:x}, 0x{buffer[12]:x}, 0x{buffer[13]:x}, 0x{buffer[14]:x}, 0x{buffer[15]:x} " + '}');
        }

        [CompilerGenerated]
        private sealed class <Chunk>c__AnonStorey3<T>
        {
            internal int size;

            internal int <>m__0(Extensions.ChunkItem<T> x) => 
                (x.Index / this.size);
        }

        [CompilerGenerated]
        private sealed class <GetOverridenInterfaceMethod>c__AnonStorey2
        {
            internal MethodReference overridingMethod;

            internal bool <>m__0(MethodReference interfaceMethod) => 
                ((this.overridingMethod.Name == interfaceMethod.Name) && VirtualMethodResolution.MethodSignaturesMatchIgnoreStaticness(interfaceMethod, this.overridingMethod));
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
                        this.type = this.type.BaseType?.Resolve();
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
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<Mono.Cecil.TypeDefinition>.GetEnumerator();

            TypeDefinition IEnumerator<TypeDefinition>.Current =>
                this.$current;

            object IEnumerator.Current =>
                this.$current;
        }

        [CompilerGenerated]
        private sealed class <GetTypesFromSpecificAttribute>c__AnonStorey1
        {
            internal string attributeName;

            internal bool <>m__0(CustomAttribute ca) => 
                (ca.AttributeType.FullName == this.attributeName);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ChunkItem<T>
        {
            public int Index;
            public T Value;
        }
    }
}

