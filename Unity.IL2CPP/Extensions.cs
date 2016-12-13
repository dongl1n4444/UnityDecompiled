using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.Common;
using Unity.IL2CPP.ILPreProcessor;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;
using Unity.IL2CPP.Metadata;

namespace Unity.IL2CPP
{
	public static class Extensions
	{
		private struct ChunkItem<T>
		{
			public int Index;

			public T Value;
		}

		[Inject]
		public static INamingService Naming;

		[Inject]
		public static ITypeProviderService TypeProvider;

		[Inject]
		public static IAssemblyDependencies AssemblyDependencies;

		[Inject]
		public static IGuidProvider GuidProvider;

		[CompilerGenerated]
		private static Func<MethodDefinition, bool> <>f__mg$cache0;

		[CompilerGenerated]
		private static Func<TypeReference, bool> <>f__mg$cache1;

		[CompilerGenerated]
		private static Func<MethodDefinition, bool> <>f__mg$cache2;

		public static bool HasFinalizer(this TypeDefinition type)
		{
			bool result;
			if (type.IsInterface)
			{
				result = false;
			}
			else if (type.MetadataType == MetadataType.Object)
			{
				result = false;
			}
			else if (type.BaseType == null)
			{
				result = false;
			}
			else
			{
				bool arg_7F_0;
				if (!type.BaseType.Resolve().HasFinalizer())
				{
					IEnumerable<MethodDefinition> arg_71_0 = type.Methods;
					if (Extensions.<>f__mg$cache0 == null)
					{
						Extensions.<>f__mg$cache0 = new Func<MethodDefinition, bool>(Extensions.IsFinalizerMethod);
					}
					arg_7F_0 = (arg_71_0.SingleOrDefault(Extensions.<>f__mg$cache0) != null);
				}
				else
				{
					arg_7F_0 = true;
				}
				result = arg_7F_0;
			}
			return result;
		}

		public static bool ContainsGenericParameters(this MethodReference method)
		{
			bool result;
			if (method.DeclaringType.ContainsGenericParameters())
			{
				result = true;
			}
			else
			{
				GenericInstanceMethod genericInstanceMethod = method as GenericInstanceMethod;
				if (genericInstanceMethod != null)
				{
					foreach (TypeReference current in genericInstanceMethod.GenericArguments)
					{
						if (current.ContainsGenericParameters())
						{
							result = true;
							return result;
						}
					}
				}
				result = false;
			}
			return result;
		}

		public static bool ContainsGenericParameters(this TypeReference typeReference)
		{
			GenericParameter genericParameter = typeReference as GenericParameter;
			bool result;
			if (genericParameter != null)
			{
				result = true;
			}
			else
			{
				ArrayType arrayType = typeReference as ArrayType;
				if (arrayType != null)
				{
					result = arrayType.ElementType.ContainsGenericParameters();
				}
				else
				{
					PointerType pointerType = typeReference as PointerType;
					if (pointerType != null)
					{
						result = pointerType.ElementType.ContainsGenericParameters();
					}
					else
					{
						ByReferenceType byReferenceType = typeReference as ByReferenceType;
						if (byReferenceType != null)
						{
							result = byReferenceType.ElementType.ContainsGenericParameters();
						}
						else
						{
							SentinelType sentinelType = typeReference as SentinelType;
							if (sentinelType != null)
							{
								result = sentinelType.ElementType.ContainsGenericParameters();
							}
							else
							{
								PinnedType pinnedType = typeReference as PinnedType;
								if (pinnedType != null)
								{
									result = pinnedType.ElementType.ContainsGenericParameters();
								}
								else
								{
									RequiredModifierType requiredModifierType = typeReference as RequiredModifierType;
									if (requiredModifierType != null)
									{
										result = requiredModifierType.ElementType.ContainsGenericParameters();
									}
									else
									{
										GenericInstanceType genericInstanceType = typeReference as GenericInstanceType;
										if (genericInstanceType != null)
										{
											IEnumerable<TypeReference> arg_108_0 = genericInstanceType.GenericArguments;
											if (Extensions.<>f__mg$cache1 == null)
											{
												Extensions.<>f__mg$cache1 = new Func<TypeReference, bool>(Extensions.ContainsGenericParameters);
											}
											result = arg_108_0.Any(Extensions.<>f__mg$cache1);
										}
										else
										{
											if (typeReference is TypeSpecification)
											{
												throw new NotSupportedException();
											}
											result = false;
										}
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		public static TypeReference GetBaseType(this TypeReference typeReference)
		{
			TypeReference result;
			if (typeReference is TypeSpecification)
			{
				if (typeReference.IsArray)
				{
					result = Extensions.TypeProvider.SystemArray;
					return result;
				}
				if (typeReference.IsGenericParameter || typeReference.IsByReference || typeReference.IsPointer)
				{
					result = null;
					return result;
				}
				SentinelType sentinelType = typeReference as SentinelType;
				if (sentinelType != null)
				{
					result = sentinelType.ElementType.GetBaseType();
					return result;
				}
				PinnedType pinnedType = typeReference as PinnedType;
				if (pinnedType != null)
				{
					result = pinnedType.ElementType.GetBaseType();
					return result;
				}
				RequiredModifierType requiredModifierType = typeReference as RequiredModifierType;
				if (requiredModifierType != null)
				{
					result = requiredModifierType.ElementType.GetBaseType();
					return result;
				}
			}
			TypeResolver typeResolver = TypeResolver.For(typeReference);
			result = typeResolver.Resolve(typeReference.Resolve().BaseType);
			return result;
		}

		[DebuggerHidden]
		public static IEnumerable<TypeDefinition> GetTypeHierarchy(this TypeDefinition type)
		{
			Extensions.<GetTypeHierarchy>c__Iterator0 <GetTypeHierarchy>c__Iterator = new Extensions.<GetTypeHierarchy>c__Iterator0();
			<GetTypeHierarchy>c__Iterator.type = type;
			<GetTypeHierarchy>c__Iterator.<$>type = type;
			Extensions.<GetTypeHierarchy>c__Iterator0 expr_15 = <GetTypeHierarchy>c__Iterator;
			expr_15.$PC = -2;
			return expr_15;
		}

		public static ReadOnlyCollection<TypeReference> GetInterfaces(this TypeReference type)
		{
			HashSet<TypeReference> hashSet = new HashSet<TypeReference>(new TypeReferenceEqualityComparer());
			Extensions.AddInterfacesRecursive(type, hashSet);
			return hashSet.ToList<TypeReference>().AsReadOnly();
		}

		private static void AddInterfacesRecursive(TypeReference type, HashSet<TypeReference> interfaces)
		{
			if (!type.IsArray)
			{
				TypeResolver typeResolver = TypeResolver.For(type);
				foreach (InterfaceImplementation current in type.Resolve().Interfaces)
				{
					TypeReference typeReference = typeResolver.Resolve(current.InterfaceType);
					if (interfaces.Add(typeReference))
					{
						Extensions.AddInterfacesRecursive(typeReference, interfaces);
					}
				}
			}
		}

		public static TypeReference GetNonPinnedAndNonByReferenceType(this TypeReference type)
		{
			type = Extensions.Naming.RemoveModifiers(type);
			TypeReference result = type;
			ByReferenceType byReferenceType = type as ByReferenceType;
			if (byReferenceType != null)
			{
				result = byReferenceType.ElementType;
			}
			PinnedType pinnedType = type as PinnedType;
			if (pinnedType != null)
			{
				result = pinnedType.ElementType;
			}
			return result;
		}

		public static ReadOnlyCollection<MethodReference> GetVirtualMethods(this TypeReference type)
		{
			return type.GetMethods((MethodDefinition m) => m.IsVirtual && !m.IsStatic);
		}

		public static ReadOnlyCollection<MethodReference> GetMethods(this TypeReference type)
		{
			return type.GetMethods((MethodDefinition m) => true);
		}

		private static ReadOnlyCollection<MethodReference> GetMethods(this TypeReference type, Func<MethodDefinition, bool> filter)
		{
			TypeResolver typeResolver = TypeResolver.For(type);
			List<MethodReference> list = new List<MethodReference>();
			foreach (MethodDefinition current in type.Resolve().Methods.Where(filter))
			{
				list.Add(typeResolver.Resolve(current));
			}
			return list.AsReadOnly();
		}

		public static TypeReference GetUnderlyingEnumType(this TypeReference type)
		{
			TypeDefinition typeDefinition = type.Resolve();
			if (typeDefinition == null)
			{
				throw new Exception("Failed to resolve type reference");
			}
			if (!typeDefinition.IsEnum)
			{
				throw new ArgumentException("Attempting to retrieve underlying enum type for non-enum type.", "type");
			}
			return typeDefinition.Fields.Single((FieldDefinition f) => !f.IsStatic && f.Name == "value__").FieldType;
		}

		public static bool IsAttribute(this TypeReference type)
		{
			bool result;
			if (type.FullName == "System.Attribute")
			{
				result = true;
			}
			else
			{
				TypeDefinition typeDefinition = type.Resolve();
				result = (typeDefinition != null && typeDefinition.BaseType != null && typeDefinition.BaseType.IsAttribute());
			}
			return result;
		}

		public static bool IsEnum(this TypeReference type)
		{
			bool result;
			if (type.IsArray)
			{
				result = false;
			}
			else if (type.IsGenericParameter)
			{
				result = false;
			}
			else
			{
				TypeDefinition typeDefinition = type.Resolve();
				if (typeDefinition == null)
				{
					throw new Exception("Failed to resolve type reference");
				}
				result = typeDefinition.IsEnum;
			}
			return result;
		}

		public static bool IsInterface(this TypeReference type)
		{
			bool result;
			if (type.IsArray)
			{
				result = false;
			}
			else if (type.IsGenericParameter)
			{
				result = false;
			}
			else
			{
				TypeDefinition typeDefinition = type.Resolve();
				result = (typeDefinition != null && typeDefinition.IsInterface);
			}
			return result;
		}

		public static bool IsComInterface(this TypeReference type)
		{
			bool result;
			if (type.IsArray)
			{
				result = false;
			}
			else if (type.IsGenericParameter)
			{
				result = false;
			}
			else
			{
				TypeDefinition typeDefinition = type.Resolve();
				result = (typeDefinition != null && typeDefinition.IsInterface && typeDefinition.IsImport && !typeDefinition.IsWindowsRuntimeProjection());
			}
			return result;
		}

		public static bool IsComOrWindowsRuntimeInterface(this TypeReference type)
		{
			bool result;
			if (type.IsArray)
			{
				result = false;
			}
			else if (type.IsGenericParameter)
			{
				result = false;
			}
			else
			{
				TypeDefinition typeDefinition = type.Resolve();
				result = (typeDefinition != null && typeDefinition.IsInterface && typeDefinition.IsComOrWindowsRuntimeType());
			}
			return result;
		}

		public static bool IsNullable(this TypeReference type)
		{
			bool result;
			if (type.IsArray)
			{
				result = false;
			}
			else if (type.IsGenericParameter)
			{
				result = false;
			}
			else
			{
				GenericInstanceType genericInstanceType = type as GenericInstanceType;
				result = (genericInstanceType != null && genericInstanceType.ElementType.FullName == "System.Nullable`1");
			}
			return result;
		}

		public static bool HasStaticConstructor(this TypeReference typeReference)
		{
			TypeDefinition typeDefinition = typeReference.Resolve();
			bool arg_3F_0;
			if (typeDefinition != null)
			{
				IEnumerable<MethodDefinition> arg_31_0 = typeDefinition.Methods;
				if (Extensions.<>f__mg$cache2 == null)
				{
					Extensions.<>f__mg$cache2 = new Func<MethodDefinition, bool>(Extensions.IsStaticConstructor);
				}
				arg_3F_0 = (arg_31_0.SingleOrDefault(Extensions.<>f__mg$cache2) != null);
			}
			else
			{
				arg_3F_0 = false;
			}
			return arg_3F_0;
		}

		public static bool HasStaticFields(this TypeReference typeReference)
		{
			bool result;
			if (typeReference.IsArray)
			{
				result = false;
			}
			else
			{
				TypeDefinition typeDefinition = typeReference.Resolve();
				result = typeDefinition.Fields.Any((FieldDefinition f) => f.IsStatic);
			}
			return result;
		}

		public static bool IsGenericParameter(this TypeReference typeReference)
		{
			bool result;
			if (typeReference is ArrayType)
			{
				result = false;
			}
			else if (typeReference is PointerType)
			{
				result = false;
			}
			else if (typeReference is ByReferenceType)
			{
				result = false;
			}
			else
			{
				TypeReference elementType = typeReference.GetElementType();
				result = elementType.IsGenericParameter;
			}
			return result;
		}

		public static bool IsValueType(this TypeReference typeReference)
		{
			bool result;
			if (typeReference.IsValueType)
			{
				result = true;
			}
			else if (typeReference is ArrayType)
			{
				result = false;
			}
			else if (typeReference is PointerType)
			{
				result = false;
			}
			else if (typeReference is ByReferenceType)
			{
				result = false;
			}
			else if (typeReference is GenericParameter)
			{
				result = false;
			}
			else
			{
				PinnedType pinnedType = typeReference as PinnedType;
				if (pinnedType != null)
				{
					result = pinnedType.ElementType.IsValueType();
				}
				else
				{
					TypeDefinition typeDefinition = typeReference.Resolve();
					result = typeDefinition.IsValueType;
				}
			}
			return result;
		}

		public static bool IsSameType(this TypeReference a, TypeReference b)
		{
			return TypeReferenceEqualityComparer.AreEqual(a, b, TypeComparisonMode.Exact);
		}

		public static bool IsSystemArray(this TypeReference typeReference)
		{
			return typeReference.FullName == "System.Array" && typeReference.Resolve().Module.Name == "mscorlib.dll";
		}

		public static bool IsIl2CppComObject(this TypeReference typeReference)
		{
			return TypeReferenceEqualityComparer.AreEqual(typeReference, Extensions.TypeProvider.Il2CppComObjectTypeReference, TypeComparisonMode.Exact);
		}

		public static bool IsIActivationFactory(this TypeReference typeReference)
		{
			return TypeReferenceEqualityComparer.AreEqual(typeReference, Extensions.TypeProvider.IActivationFactoryTypeReference, TypeComparisonMode.Exact);
		}

		public static bool IsIntegralPointerType(this TypeReference typeReference)
		{
			return typeReference.MetadataType == MetadataType.IntPtr || typeReference.MetadataType == MetadataType.UIntPtr;
		}

		public static bool IsNativeIntegralType(this TypeReference typeReference)
		{
			return TypeReferenceEqualityComparer.AreEqual(typeReference, Extensions.TypeProvider.NativeIntTypeReference, TypeComparisonMode.Exact) || TypeReferenceEqualityComparer.AreEqual(typeReference, Extensions.TypeProvider.NativeUIntTypeReference, TypeComparisonMode.Exact);
		}

		public static bool IsSystemObject(this TypeReference typeReference)
		{
			return typeReference.MetadataType == MetadataType.Object;
		}

		public static bool IsSystemType(this TypeReference typeReference)
		{
			return typeReference.FullName == "System.Type" && typeReference.Resolve().Module.Name == "mscorlib.dll";
		}

		public static bool IsSpecialSystemBaseType(this TypeReference typeReference)
		{
			return typeReference.FullName == "System.Object" || typeReference.FullName == "System.ValueType" || typeReference.FullName == "System.Enum";
		}

		public static bool IsFinalizerMethod(this MethodDefinition method)
		{
			return method.Name == "Finalize" && method.ReturnType.MetadataType == MetadataType.Void && !method.HasParameters && (ushort)(method.Attributes & MethodAttributes.Family) != 0;
		}

		public static bool NeedsComCallableWrapper(this TypeDefinition type)
		{
			bool result;
			if (type.IsComOrWindowsRuntimeType())
			{
				result = false;
			}
			else if (type.ImplementedComOrWindowsRuntimeInterfaces().Any<TypeReference>())
			{
				result = true;
			}
			else
			{
				while (type.BaseType != null)
				{
					type = type.BaseType.Resolve();
					if (type.ImplementedComOrWindowsRuntimeInterfaces().Any<TypeReference>() || type.IsComOrWindowsRuntimeType())
					{
						result = true;
						return result;
					}
				}
				result = false;
			}
			return result;
		}

		public static IEnumerable<TypeReference> ImplementedComOrWindowsRuntimeInterfaces(this TypeDefinition typeDefinition)
		{
			List<TypeReference> list = new List<TypeReference>();
			foreach (InterfaceImplementation current in typeDefinition.Interfaces)
			{
				if (current.InterfaceType.IsComOrWindowsRuntimeInterface())
				{
					list.Add(current.InterfaceType);
				}
			}
			return list;
		}

		public static bool IsComOrWindowsRuntimeMethod(this MethodDefinition method)
		{
			TypeDefinition declaringType = method.DeclaringType;
			return declaringType.IsWindowsRuntime || declaringType.IsIl2CppComObject() || (declaringType.IsImport && (method.IsInternalCall || method.IsFinalizerMethod() || declaringType.IsInterface));
		}

		public static bool IsComOrWindowsRuntimeType(this TypeDefinition type)
		{
			return !type.IsValueType && !type.IsDelegate() && (type.IsIl2CppComObject() || type.IsImport || type.IsWindowsRuntime);
		}

		public static bool IsComOrWindowsRuntimeInterface(this MethodDefinition method)
		{
			return method.IsComOrWindowsRuntimeMethod() && method.DeclaringType.IsInterface;
		}

		public static bool IsComOrWindowsRuntimeInterface(this MethodReference method)
		{
			return method.Resolve().IsComOrWindowsRuntimeInterface();
		}

		public static bool IsStripped(this MethodReference method)
		{
			return method.Name.StartsWith("$__Stripped");
		}

		public static bool IsWindowsRuntimeProjection(this TypeReference type)
		{
			return type.GetElementType().IsWindowsRuntimeProjection;
		}

		public static bool IsWindowsRuntimeProjection(this TypeDefinition type)
		{
			return type.IsWindowsRuntimeProjection;
		}

		public static bool HasActivationFactories(this TypeReference type)
		{
			TypeDefinition typeDefinition = type.Resolve();
			bool result;
			if (!typeDefinition.IsWindowsRuntime || typeDefinition.IsValueType)
			{
				result = false;
			}
			else
			{
				result = typeDefinition.CustomAttributes.Any((CustomAttribute ca) => ca.AttributeType.FullName == "Windows.Foundation.Metadata.ActivatableAttribute" || ca.AttributeType.FullName == "Windows.Foundation.Metadata.StaticAttribute" || ca.AttributeType.FullName == "Windows.Foundation.Metadata.ComposableAttribute");
			}
			return result;
		}

		private static IEnumerable<TypeReference> GetTypesFromSpecificAttribute(this TypeDefinition type, string attributeName, Func<CustomAttribute, TypeReference> customAttributeSelector)
		{
			return (from ca in type.CustomAttributes
			where ca.AttributeType.FullName == attributeName
			select ca).Select(customAttributeSelector);
		}

		public static IEnumerable<TypeReference> GetStaticFactoryTypes(this TypeReference type)
		{
			TypeDefinition typeDefinition = type.Resolve();
			IEnumerable<TypeReference> result;
			if (!typeDefinition.IsWindowsRuntime || typeDefinition.IsValueType)
			{
				result = Enumerable.Empty<TypeReference>();
			}
			else
			{
				result = typeDefinition.GetTypesFromSpecificAttribute("Windows.Foundation.Metadata.StaticAttribute", (CustomAttribute attribute) => (TypeReference)attribute.ConstructorArguments[0].Value);
			}
			return result;
		}

		public static IEnumerable<TypeReference> GetActivationFactoryTypes(this TypeReference type)
		{
			TypeDefinition typeDefinition = type.Resolve();
			IEnumerable<TypeReference> result;
			if (!typeDefinition.IsWindowsRuntime || typeDefinition.IsValueType)
			{
				result = Enumerable.Empty<TypeReference>();
			}
			else
			{
				result = typeDefinition.GetTypesFromSpecificAttribute("Windows.Foundation.Metadata.ActivatableAttribute", delegate(CustomAttribute attribute)
				{
					CustomAttributeArgument customAttributeArgument = attribute.ConstructorArguments[0];
					TypeReference result2;
					if (customAttributeArgument.Type.IsSystemType())
					{
						result2 = (TypeReference)customAttributeArgument.Value;
					}
					else
					{
						result2 = Extensions.TypeProvider.IActivationFactoryTypeReference;
					}
					return result2;
				});
			}
			return result;
		}

		public static IEnumerable<TypeReference> GetComposableFactoryTypes(this TypeReference type)
		{
			TypeDefinition typeDefinition = type.Resolve();
			IEnumerable<TypeReference> result;
			if (!typeDefinition.IsWindowsRuntime || typeDefinition.IsValueType)
			{
				result = Enumerable.Empty<TypeReference>();
			}
			else
			{
				result = typeDefinition.GetTypesFromSpecificAttribute("Windows.Foundation.Metadata.ComposableAttribute", (CustomAttribute attribute) => (TypeReference)attribute.ConstructorArguments[0].Value);
			}
			return result;
		}

		public static IEnumerable<TypeReference> GetAllFactoryTypes(this TypeReference type)
		{
			TypeDefinition typeDefinition = type.Resolve();
			IEnumerable<TypeReference> result;
			if (!typeDefinition.IsWindowsRuntime || typeDefinition.IsValueType)
			{
				result = Enumerable.Empty<TypeReference>();
			}
			else
			{
				result = typeDefinition.GetActivationFactoryTypes().Concat(typeDefinition.GetComposableFactoryTypes()).Concat(typeDefinition.GetStaticFactoryTypes()).Distinct(new TypeReferenceEqualityComparer());
			}
			return result;
		}

		public static TypeReference ExtractDefaultInterface(this TypeDefinition type)
		{
			if (!type.IsWindowsRuntime)
			{
				throw new ArgumentException(string.Format("Extracting default interface is only valid for Windows Runtime types. {0} is not a Windows Runtime type.", type.FullName));
			}
			foreach (InterfaceImplementation current in type.Interfaces)
			{
				foreach (CustomAttribute current2 in current.CustomAttributes)
				{
					if (current2.AttributeType.FullName == "Windows.Foundation.Metadata.DefaultAttribute")
					{
						return current.InterfaceType;
					}
				}
			}
			throw new InvalidProgramException(string.Format("Windows Runtime class {0} has no default interface!", type));
		}

		public static bool StoresNonFieldsInStaticFields(this TypeReference type)
		{
			return type.HasActivationFactories();
		}

		public static bool IsStaticConstructor(this MethodReference methodReference)
		{
			MethodDefinition methodDefinition = methodReference.Resolve();
			return methodDefinition != null && (methodDefinition.IsConstructor && methodDefinition.IsStatic) && methodDefinition.Parameters.Count == 0;
		}

		public static bool ShouldProcessAsInternalCall(this MethodReference methodReference)
		{
			MethodDefinition methodDefinition = methodReference.Resolve();
			return methodDefinition != null && methodDefinition.IsInternalCall && !methodDefinition.HasGenericParameters;
		}

		public static MethodReference GetOverridenInterfaceMethod(this MethodReference overridingMethod, IEnumerable<TypeReference> candidateInterfaces)
		{
			MethodDefinition methodDefinition = overridingMethod.Resolve();
			MethodReference result;
			if (methodDefinition.Overrides.Count > 0)
			{
				if (methodDefinition.Overrides.Count != 1)
				{
					throw new InvalidOperationException(string.Format("Cannot choose overriden method for '{0}'", overridingMethod.FullName));
				}
				result = TypeResolver.For(overridingMethod.DeclaringType, overridingMethod).Resolve(methodDefinition.Overrides[0]);
			}
			else
			{
				IEnumerable<MethodReference> source = candidateInterfaces.SelectMany((TypeReference iface) => iface.GetMethods());
				result = source.FirstOrDefault((MethodReference interfaceMethod) => overridingMethod.Name == interfaceMethod.Name && VirtualMethodResolution.MethodSignaturesMatchIgnoreStaticness(interfaceMethod, overridingMethod));
			}
			return result;
		}

		public static bool IsDefinedInUnityEngine(this MemberReference memberReference)
		{
			return memberReference.Module.Assembly.Name.Name.Contains("UnityEngine");
		}

		public static bool IsDefinedInMscorlib(this MemberReference memberReference)
		{
			return memberReference.Module.Assembly.Name.Name == "mscorlib";
		}

		public static List<List<T>> Chunk<T>(this IEnumerable<T> foo, int size)
		{
			return (from x in foo.Select((T value, int index) => new Extensions.ChunkItem<T>
			{
				Index = index,
				Value = value
			})
			group x by x.Index / size into g
			select (from x in g
			select x.Value).ToList<T>()).ToList<List<T>>();
		}

		public static bool IsNormalStatic(this FieldReference field)
		{
			FieldDefinition fieldDefinition = field.Resolve();
			bool result;
			if (fieldDefinition.IsLiteral)
			{
				result = false;
			}
			else if (!fieldDefinition.IsStatic)
			{
				result = false;
			}
			else if (!fieldDefinition.HasCustomAttributes)
			{
				result = true;
			}
			else
			{
				result = fieldDefinition.CustomAttributes.All((CustomAttribute ca) => ca.AttributeType.Name != "ThreadStaticAttribute");
			}
			return result;
		}

		public static bool IsThreadStatic(this FieldReference field)
		{
			FieldDefinition fieldDefinition = field.Resolve();
			bool arg_49_0;
			if (fieldDefinition.IsStatic && fieldDefinition.HasCustomAttributes)
			{
				arg_49_0 = fieldDefinition.CustomAttributes.Any((CustomAttribute ca) => ca.AttributeType.Name == "ThreadStaticAttribute");
			}
			else
			{
				arg_49_0 = false;
			}
			return arg_49_0;
		}

		public static bool IsDelegate(this TypeDefinition type)
		{
			return type.BaseType != null && type.BaseType.FullName == "System.MulticastDelegate";
		}

		public static bool IsStructWithNoInstanceFields(this TypeReference typeReference)
		{
			bool result;
			if (typeReference.IsValueType() && !typeReference.IsEnum())
			{
				TypeDefinition typeDefinition = typeReference.Resolve();
				bool arg_5E_0;
				if (typeDefinition != null)
				{
					if (typeDefinition.HasFields)
					{
						arg_5E_0 = typeDefinition.Fields.All((FieldDefinition field) => field.IsStatic);
					}
					else
					{
						arg_5E_0 = true;
					}
				}
				else
				{
					arg_5E_0 = false;
				}
				result = arg_5E_0;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public static bool DerivesFromObject(this TypeReference typeReference)
		{
			TypeReference baseType = typeReference.GetBaseType();
			return baseType != null && baseType.MetadataType == MetadataType.Object;
		}

		public static bool DerivesFrom(this TypeReference type, TypeReference potentialBaseType, bool checkInterfaces = true)
		{
			bool result;
			while (type != null)
			{
				if (!TypeReferenceEqualityComparer.AreEqual(type, potentialBaseType, TypeComparisonMode.Exact))
				{
					if (checkInterfaces)
					{
						foreach (TypeReference current in type.GetInterfaces())
						{
							if (TypeReferenceEqualityComparer.AreEqual(current, potentialBaseType, TypeComparisonMode.Exact))
							{
								result = true;
								return result;
							}
						}
					}
					type = type.GetBaseType();
					continue;
				}
				result = true;
				return result;
			}
			result = false;
			return result;
		}

		public static bool IsVolatile(this FieldReference fieldReference)
		{
			bool result;
			if (fieldReference != null && fieldReference.FieldType.IsRequiredModifier)
			{
				if (((RequiredModifierType)fieldReference.FieldType).ModifierType.Name.Contains("IsVolatile"))
				{
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}

		public static bool IsVoid(this TypeReference type)
		{
			return type.MetadataType == MetadataType.Void;
		}

		public static bool References(this AssemblyDefinition assemblyDoingTheReferencing, AssemblyDefinition assemblyBeingReference)
		{
			return (from eachAssemblyReference in Extensions.AssemblyDependencies.GetReferencedAssembliesFor(assemblyDoingTheReferencing)
			select eachAssemblyReference.Name.Name).Contains(assemblyBeingReference.Name.Name);
		}

		public static bool IsIntegralType(this TypeReference type)
		{
			return type.IsSignedIntegralType() || type.IsUnsignedIntegralType();
		}

		public static bool IsSignedIntegralType(this TypeReference type)
		{
			return type.MetadataType == MetadataType.SByte || type.MetadataType == MetadataType.Int16 || type.MetadataType == MetadataType.Int32 || type.MetadataType == MetadataType.Int64;
		}

		public static bool IsUnsignedIntegralType(this TypeReference type)
		{
			return type.MetadataType == MetadataType.Byte || type.MetadataType == MetadataType.UInt16 || type.MetadataType == MetadataType.UInt32 || type.MetadataType == MetadataType.UInt64;
		}

		public static Guid GetGuid(this TypeReference type)
		{
			return Extensions.GuidProvider.GuidFor(type);
		}

		public static string ToInitializer(this Guid guid)
		{
			byte[] array = guid.ToByteArray();
			uint num = BitConverter.ToUInt32(array, 0);
			ushort num2 = BitConverter.ToUInt16(array, 4);
			ushort num3 = BitConverter.ToUInt16(array, 6);
			return '{' + string.Format(" 0x{0:x}, 0x{1:x}, 0x{2:x}, 0x{3:x}, 0x{4:x}, 0x{5:x}, 0x{6:x}, 0x{7:x}, 0x{8:x}, 0x{9:x}, 0x{10:x} ", new object[]
			{
				num,
				num2,
				num3,
				array[8],
				array[9],
				array[10],
				array[11],
				array[12],
				array[13],
				array[14],
				array[15]
			}) + '}';
		}

		public static IEnumerable<CustomAttribute> GetConstructibleCustomAttributes(this ICustomAttributeProvider customAttributeProvider)
		{
			return customAttributeProvider.CustomAttributes.Where(delegate(CustomAttribute ca)
			{
				TypeDefinition typeDefinition = ca.AttributeType.Resolve();
				return typeDefinition != null && !typeDefinition.IsWindowsRuntime;
			});
		}

		public static bool IsPrimitiveType(this MetadataType type)
		{
			bool result;
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
				result = true;
				break;
			default:
				result = false;
				break;
			}
			return result;
		}

		public static bool IsPrimitiveCppType(this string typeName)
		{
			bool result;
			if (typeName != null)
			{
				if (Extensions.<>f__switch$map1 == null)
				{
					Extensions.<>f__switch$map1 = new Dictionary<string, int>(14)
					{
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
				}
				int num;
				if (Extensions.<>f__switch$map1.TryGetValue(typeName, out num))
				{
					if (num == 0)
					{
						result = true;
						return result;
					}
				}
			}
			result = false;
			return result;
		}
	}
}
