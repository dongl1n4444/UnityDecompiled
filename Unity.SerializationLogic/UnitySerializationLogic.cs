using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.CecilTools;
using Unity.CecilTools.Extensions;

namespace Unity.SerializationLogic
{
	public static class UnitySerializationLogic
	{
		public static bool WillUnitySerialize(FieldDefinition fieldDefinition)
		{
			return UnitySerializationLogic.WillUnitySerialize(fieldDefinition, new TypeResolver(null));
		}

		public static bool WillUnitySerialize(FieldDefinition fieldDefinition, TypeResolver typeResolver)
		{
			bool result;
			if (fieldDefinition == null)
			{
				result = false;
			}
			else if (fieldDefinition.IsStatic || UnitySerializationLogic.IsConst(fieldDefinition) || fieldDefinition.IsNotSerialized || fieldDefinition.IsInitOnly)
			{
				result = false;
			}
			else if (UnitySerializationLogic.ShouldNotTryToResolve(fieldDefinition.FieldType))
			{
				result = false;
			}
			else
			{
				bool flag = UnitySerializationLogic.HasSerializeFieldAttribute(fieldDefinition);
				result = ((fieldDefinition.IsPublic || flag || UnitySerializationLogic.ShouldHaveHadAllFieldsPublic(fieldDefinition)) && !(fieldDefinition.FullName == "UnityScript.Lang.Array") && UnitySerializationLogic.IsFieldTypeSerializable(typeResolver.Resolve(fieldDefinition.FieldType)) && !UnitySerializationLogic.IsDelegate(typeResolver.Resolve(fieldDefinition.FieldType)));
			}
			return result;
		}

		private static bool IsDelegate(TypeReference typeReference)
		{
			return typeReference.IsAssignableTo("System.Delegate");
		}

		public static bool ShouldFieldBePPtrRemapped(FieldDefinition fieldDefinition)
		{
			return UnitySerializationLogic.ShouldFieldBePPtrRemapped(fieldDefinition, new TypeResolver(null));
		}

		public static bool ShouldFieldBePPtrRemapped(FieldDefinition fieldDefinition, TypeResolver typeResolver)
		{
			return UnitySerializationLogic.WillUnitySerialize(fieldDefinition, typeResolver) && UnitySerializationLogic.CanTypeContainUnityEngineObjectReference(typeResolver.Resolve(fieldDefinition.FieldType));
		}

		private static bool CanTypeContainUnityEngineObjectReference(TypeReference typeReference)
		{
			bool result;
			if (UnitySerializationLogic.IsUnityEngineObject(typeReference))
			{
				result = true;
			}
			else if (typeReference.IsEnum())
			{
				result = false;
			}
			else if (UnitySerializationLogic.IsSerializablePrimitive(typeReference))
			{
				result = false;
			}
			else if (UnitySerializationLogic.IsSupportedCollection(typeReference))
			{
				result = UnitySerializationLogic.CanTypeContainUnityEngineObjectReference(CecilUtils.ElementTypeOfCollection(typeReference));
			}
			else
			{
				TypeDefinition typeDefinition = typeReference.Resolve();
				result = (typeDefinition != null && UnitySerializationLogic.HasFieldsThatCanContainUnityEngineObjectReferences(typeDefinition, new TypeResolver(typeReference as GenericInstanceType)));
			}
			return result;
		}

		private static bool HasFieldsThatCanContainUnityEngineObjectReferences(TypeDefinition definition, TypeResolver typeResolver)
		{
			return (from kv in UnitySerializationLogic.AllFieldsFor(definition, typeResolver)
			where kv.Value.Resolve(kv.Key.FieldType).Resolve() != definition
			select kv).Any((KeyValuePair<FieldDefinition, TypeResolver> kv) => UnitySerializationLogic.CanFieldContainUnityEngineObjectReference(definition, kv.Key, kv.Value));
		}

		[DebuggerHidden]
		private static IEnumerable<KeyValuePair<FieldDefinition, TypeResolver>> AllFieldsFor(TypeDefinition definition, TypeResolver typeResolver)
		{
			UnitySerializationLogic.<AllFieldsFor>c__Iterator0 <AllFieldsFor>c__Iterator = new UnitySerializationLogic.<AllFieldsFor>c__Iterator0();
			<AllFieldsFor>c__Iterator.definition = definition;
			<AllFieldsFor>c__Iterator.typeResolver = typeResolver;
			UnitySerializationLogic.<AllFieldsFor>c__Iterator0 expr_15 = <AllFieldsFor>c__Iterator;
			expr_15.$PC = -2;
			return expr_15;
		}

		private static bool CanFieldContainUnityEngineObjectReference(TypeReference typeReference, FieldDefinition t, TypeResolver typeResolver)
		{
			return typeResolver.Resolve(t.FieldType) != typeReference && UnitySerializationLogic.WillUnitySerialize(t, typeResolver) && !UnityEngineTypePredicates.IsUnityEngineValueType(typeReference);
		}

		private static bool IsConst(FieldDefinition fieldDefinition)
		{
			return fieldDefinition.IsLiteral && !fieldDefinition.IsInitOnly;
		}

		public static bool HasSerializeFieldAttribute(FieldDefinition field)
		{
			bool result;
			foreach (TypeReference current in UnitySerializationLogic.FieldAttributes(field))
			{
				if (UnityEngineTypePredicates.IsSerializeFieldAttribute(current))
				{
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}

		private static IEnumerable<TypeReference> FieldAttributes(FieldDefinition field)
		{
			return from _ in field.CustomAttributes
			select _.AttributeType;
		}

		public static bool ShouldNotTryToResolve(TypeReference typeReference)
		{
			bool result;
			if (typeReference.Scope.Name == "Windows")
			{
				result = true;
			}
			else if (typeReference.Scope.Name == "mscorlib")
			{
				TypeDefinition typeDefinition = typeReference.Resolve();
				result = (typeDefinition == null);
			}
			else
			{
				try
				{
					typeReference.Resolve();
				}
				catch
				{
					result = true;
					return result;
				}
				result = false;
			}
			return result;
		}

		private static bool IsFieldTypeSerializable(TypeReference typeReference)
		{
			return UnitySerializationLogic.IsTypeSerializable(typeReference) || UnitySerializationLogic.IsSupportedCollection(typeReference);
		}

		private static bool IsTypeSerializable(TypeReference typeReference)
		{
			return !typeReference.IsAssignableTo("UnityScript.Lang.Array") && !UnitySerializationLogic.IsOrExtendsGenericDictionary(typeReference) && (UnitySerializationLogic.IsSerializablePrimitive(typeReference) || typeReference.IsEnum() || UnitySerializationLogic.IsUnityEngineObject(typeReference) || UnityEngineTypePredicates.IsSerializableUnityStruct(typeReference) || UnitySerializationLogic.ShouldImplementIDeserializable(typeReference));
		}

		private static bool IsOrExtendsGenericDictionary(TypeReference typeReference)
		{
			bool result;
			TypeDefinition typeDefinition;
			for (TypeReference type = typeReference; type != null; type = typeDefinition.BaseType)
			{
				if (CecilUtils.IsGenericDictionary(type))
				{
					result = true;
					return result;
				}
				typeDefinition = type.CheckedResolve();
				if (typeDefinition == null)
				{
					break;
				}
			}
			result = false;
			return result;
		}

		private static bool IsSerializablePrimitive(TypeReference typeReference)
		{
			bool result;
			switch (typeReference.MetadataType)
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
			case MetadataType.String:
				result = true;
				break;
			default:
				result = false;
				break;
			}
			return result;
		}

		public static bool IsSupportedCollection(TypeReference typeReference)
		{
			return (typeReference is ArrayType || CecilUtils.IsGenericList(typeReference)) && (!typeReference.IsArray || ((ArrayType)typeReference).Rank <= 1) && UnitySerializationLogic.IsTypeSerializable(CecilUtils.ElementTypeOfCollection(typeReference));
		}

		private static bool ShouldHaveHadAllFieldsPublic(FieldDefinition field)
		{
			return UnityEngineTypePredicates.IsUnityEngineValueType(field.DeclaringType);
		}

		private static bool IsUnityEngineObject(TypeReference typeReference)
		{
			return UnityEngineTypePredicates.IsUnityEngineObject(typeReference);
		}

		public static bool IsNonSerialized(TypeReference typeDeclaration)
		{
			return typeDeclaration == null || typeDeclaration.IsEnum() || typeDeclaration.HasGenericParameters || typeDeclaration.MetadataType == MetadataType.Object || typeDeclaration.FullName.StartsWith("System.") || typeDeclaration.IsArray || typeDeclaration.FullName == "UnityEngine.MonoBehaviour" || typeDeclaration.FullName == "UnityEngine.ScriptableObject";
		}

		public static bool ShouldImplementIDeserializable(TypeReference typeDeclaration)
		{
			bool result;
			if (UnitySerializationLogic.IsNonSerialized(typeDeclaration))
			{
				result = false;
			}
			else if (typeDeclaration is GenericInstanceType)
			{
				result = false;
			}
			else
			{
				try
				{
					bool arg_97_0;
					if (!UnityEngineTypePredicates.IsMonoBehaviour(typeDeclaration) && !UnityEngineTypePredicates.IsScriptableObject(typeDeclaration))
					{
						if (typeDeclaration.CheckedResolve().IsSerializable && !typeDeclaration.CheckedResolve().IsAbstract)
						{
							if (!typeDeclaration.CheckedResolve().CustomAttributes.Any((CustomAttribute a) => a.AttributeType.FullName.Contains("System.Runtime.CompilerServices.CompilerGenerated")))
							{
								goto IL_96;
							}
						}
						arg_97_0 = UnityEngineTypePredicates.ShouldHaveHadSerializableAttribute(typeDeclaration);
						goto IL_97;
					}
					IL_96:
					arg_97_0 = true;
					IL_97:
					result = arg_97_0;
				}
				catch (Exception)
				{
					result = false;
				}
			}
			return result;
		}
	}
}
