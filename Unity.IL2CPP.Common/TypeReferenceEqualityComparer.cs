using Mono.Cecil;
using System;
using System.Collections.Generic;

namespace Unity.IL2CPP.Common
{
	public class TypeReferenceEqualityComparer : EqualityComparer<TypeReference>
	{
		public override bool Equals(TypeReference x, TypeReference y)
		{
			return TypeReferenceEqualityComparer.AreEqual(x, y, TypeComparisonMode.Exact);
		}

		public override int GetHashCode(TypeReference obj)
		{
			return TypeReferenceEqualityComparer.GetHashCodeFor(obj);
		}

		public static bool AreEqual(TypeReference a, TypeReference b, TypeComparisonMode comparisonMode = TypeComparisonMode.Exact)
		{
			bool result;
			if (object.ReferenceEquals(a, b))
			{
				result = true;
			}
			else if (a == null || b == null)
			{
				result = false;
			}
			else
			{
				MetadataType metadataType = a.MetadataType;
				MetadataType metadataType2 = b.MetadataType;
				if (metadataType == MetadataType.GenericInstance || metadataType2 == MetadataType.GenericInstance)
				{
					result = (metadataType == metadataType2 && TypeReferenceEqualityComparer.AreEqual((GenericInstanceType)a, (GenericInstanceType)b, comparisonMode));
				}
				else if (metadataType == MetadataType.Array || metadataType2 == MetadataType.Array)
				{
					if (metadataType != metadataType2)
					{
						result = false;
					}
					else
					{
						ArrayType arrayType = (ArrayType)a;
						ArrayType arrayType2 = (ArrayType)b;
						result = (arrayType.Rank == arrayType2.Rank && TypeReferenceEqualityComparer.AreEqual(arrayType.ElementType, arrayType2.ElementType, comparisonMode));
					}
				}
				else if (metadataType == MetadataType.Var || metadataType2 == MetadataType.Var)
				{
					result = (metadataType == metadataType2 && TypeReferenceEqualityComparer.AreEqual((GenericParameter)a, (GenericParameter)b, TypeComparisonMode.Exact));
				}
				else if (metadataType == MetadataType.MVar || metadataType2 == MetadataType.MVar)
				{
					result = (metadataType == metadataType2 && TypeReferenceEqualityComparer.AreEqual((GenericParameter)a, (GenericParameter)b, comparisonMode));
				}
				else if (metadataType == MetadataType.ByReference || metadataType2 == MetadataType.ByReference)
				{
					result = (metadataType == metadataType2 && TypeReferenceEqualityComparer.AreEqual(((ByReferenceType)a).ElementType, ((ByReferenceType)b).ElementType, comparisonMode));
				}
				else if (metadataType == MetadataType.Pointer || metadataType2 == MetadataType.Pointer)
				{
					result = (metadataType == metadataType2 && TypeReferenceEqualityComparer.AreEqual(((PointerType)a).ElementType, ((PointerType)b).ElementType, comparisonMode));
				}
				else if (metadataType == MetadataType.RequiredModifier || metadataType2 == MetadataType.RequiredModifier)
				{
					if (metadataType != metadataType2)
					{
						result = false;
					}
					else
					{
						RequiredModifierType requiredModifierType = (RequiredModifierType)a;
						RequiredModifierType requiredModifierType2 = (RequiredModifierType)b;
						result = (TypeReferenceEqualityComparer.AreEqual(requiredModifierType.ModifierType, requiredModifierType2.ModifierType, TypeComparisonMode.Exact) && TypeReferenceEqualityComparer.AreEqual(requiredModifierType.ElementType, requiredModifierType2.ElementType, comparisonMode));
					}
				}
				else if (metadataType == MetadataType.OptionalModifier || metadataType2 == MetadataType.OptionalModifier)
				{
					if (metadataType != metadataType2)
					{
						result = false;
					}
					else
					{
						OptionalModifierType optionalModifierType = (OptionalModifierType)a;
						OptionalModifierType optionalModifierType2 = (OptionalModifierType)b;
						result = (TypeReferenceEqualityComparer.AreEqual(optionalModifierType.ModifierType, optionalModifierType2.ModifierType, TypeComparisonMode.Exact) && TypeReferenceEqualityComparer.AreEqual(optionalModifierType.ElementType, optionalModifierType2.ElementType, comparisonMode));
					}
				}
				else if (metadataType == MetadataType.Pinned || metadataType2 == MetadataType.Pinned)
				{
					result = (metadataType == metadataType2 && TypeReferenceEqualityComparer.AreEqual(((PinnedType)a).ElementType, ((PinnedType)b).ElementType, comparisonMode));
				}
				else if (metadataType == MetadataType.Sentinel || metadataType2 == MetadataType.Sentinel)
				{
					result = (metadataType == metadataType2 && TypeReferenceEqualityComparer.AreEqual(((SentinelType)a).ElementType, ((SentinelType)b).ElementType, TypeComparisonMode.Exact));
				}
				else if (!a.Name.Equals(b.Name) || !a.Namespace.Equals(b.Namespace))
				{
					result = false;
				}
				else
				{
					TypeDefinition typeDefinition = a.Resolve();
					TypeDefinition typeDefinition2 = b.Resolve();
					result = (typeDefinition == typeDefinition2);
				}
			}
			return result;
		}

		private static bool AreEqual(GenericParameter a, GenericParameter b, TypeComparisonMode comparisonMode = TypeComparisonMode.Exact)
		{
			bool result;
			if (object.ReferenceEquals(a, b))
			{
				result = true;
			}
			else if (a.Position != b.Position)
			{
				result = false;
			}
			else if (a.Type != b.Type)
			{
				result = false;
			}
			else
			{
				TypeReference typeReference = a.Owner as TypeReference;
				if (typeReference != null && TypeReferenceEqualityComparer.AreEqual(typeReference, b.Owner as TypeReference, comparisonMode))
				{
					result = true;
				}
				else
				{
					MethodReference methodReference = a.Owner as MethodReference;
					result = ((methodReference != null && MethodReferenceComparer.AreEqual(methodReference, b.Owner as MethodReference)) || comparisonMode == TypeComparisonMode.SignatureOnly);
				}
			}
			return result;
		}

		private static bool AreEqual(GenericInstanceType a, GenericInstanceType b, TypeComparisonMode comparisonMode = TypeComparisonMode.Exact)
		{
			bool result;
			if (object.ReferenceEquals(a, b))
			{
				result = true;
			}
			else
			{
				int count = a.GenericArguments.Count;
				if (count != b.GenericArguments.Count)
				{
					result = false;
				}
				else if (!TypeReferenceEqualityComparer.AreEqual(a.ElementType, b.ElementType, comparisonMode))
				{
					result = false;
				}
				else
				{
					for (int i = 0; i < count; i++)
					{
						if (!TypeReferenceEqualityComparer.AreEqual(a.GenericArguments[i], b.GenericArguments[i], comparisonMode))
						{
							result = false;
							return result;
						}
					}
					result = true;
				}
			}
			return result;
		}

		public static int GetHashCodeFor(TypeReference obj)
		{
			MetadataType metadataType = obj.MetadataType;
			int result;
			if (metadataType == MetadataType.GenericInstance)
			{
				GenericInstanceType genericInstanceType = (GenericInstanceType)obj;
				int num = TypeReferenceEqualityComparer.GetHashCodeFor(genericInstanceType.ElementType) * 486187739 + 31;
				for (int i = 0; i < genericInstanceType.GenericArguments.Count; i++)
				{
					num = num * 486187739 + TypeReferenceEqualityComparer.GetHashCodeFor(genericInstanceType.GenericArguments[i]);
				}
				result = num;
			}
			else if (metadataType == MetadataType.Array)
			{
				ArrayType arrayType = (ArrayType)obj;
				result = TypeReferenceEqualityComparer.GetHashCodeFor(arrayType.ElementType) * 486187739 + arrayType.Rank.GetHashCode();
			}
			else if (metadataType == MetadataType.Var || metadataType == MetadataType.MVar)
			{
				GenericParameter genericParameter = (GenericParameter)obj;
				int arg_F1_0 = genericParameter.Position.GetHashCode() * 486187739;
				int num2 = (int)metadataType;
				int num3 = arg_F1_0 + num2.GetHashCode();
				TypeReference typeReference = genericParameter.Owner as TypeReference;
				if (typeReference != null)
				{
					result = num3 * 486187739 + TypeReferenceEqualityComparer.GetHashCodeFor(typeReference);
				}
				else
				{
					MethodReference methodReference = genericParameter.Owner as MethodReference;
					if (methodReference == null)
					{
						throw new InvalidOperationException("Generic parameter encountered with invalid owner");
					}
					result = num3 * 486187739 + MethodReferenceComparer.GetHashCodeFor(methodReference);
				}
			}
			else if (metadataType == MetadataType.ByReference)
			{
				ByReferenceType byReferenceType = (ByReferenceType)obj;
				result = TypeReferenceEqualityComparer.GetHashCodeFor(byReferenceType.ElementType) * 486187739 * 37;
			}
			else if (metadataType == MetadataType.Pointer)
			{
				PointerType pointerType = (PointerType)obj;
				result = TypeReferenceEqualityComparer.GetHashCodeFor(pointerType.ElementType) * 486187739 * 41;
			}
			else if (metadataType == MetadataType.RequiredModifier)
			{
				RequiredModifierType requiredModifierType = (RequiredModifierType)obj;
				int num4 = TypeReferenceEqualityComparer.GetHashCodeFor(requiredModifierType.ElementType) * 43;
				num4 = num4 * 486187739 + TypeReferenceEqualityComparer.GetHashCodeFor(requiredModifierType.ModifierType);
				result = num4;
			}
			else if (metadataType == MetadataType.OptionalModifier)
			{
				OptionalModifierType optionalModifierType = (OptionalModifierType)obj;
				int num5 = TypeReferenceEqualityComparer.GetHashCodeFor(optionalModifierType.ElementType) * 47;
				num5 = num5 * 486187739 + TypeReferenceEqualityComparer.GetHashCodeFor(optionalModifierType.ModifierType);
				result = num5;
			}
			else if (metadataType == MetadataType.Pinned)
			{
				PinnedType pinnedType = (PinnedType)obj;
				result = TypeReferenceEqualityComparer.GetHashCodeFor(pinnedType.ElementType) * 486187739 * 53;
			}
			else if (metadataType == MetadataType.Sentinel)
			{
				SentinelType sentinelType = (SentinelType)obj;
				result = TypeReferenceEqualityComparer.GetHashCodeFor(sentinelType.ElementType) * 486187739 * 59;
			}
			else
			{
				if (metadataType == MetadataType.FunctionPointer)
				{
					throw new NotImplementedException("We currently don't handle function pointer types.");
				}
				result = obj.Namespace.GetHashCode() * 486187739 + obj.FullName.GetHashCode();
			}
			return result;
		}
	}
}
