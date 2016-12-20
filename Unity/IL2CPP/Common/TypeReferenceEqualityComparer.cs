namespace Unity.IL2CPP.Common
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public class TypeReferenceEqualityComparer : EqualityComparer<TypeReference>
    {
        private static bool AreEqual(GenericInstanceType a, GenericInstanceType b, [Optional, DefaultParameterValue(0)] TypeComparisonMode comparisonMode)
        {
            if (!object.ReferenceEquals(a, b))
            {
                int count = a.GenericArguments.Count;
                if (count != b.GenericArguments.Count)
                {
                    return false;
                }
                if (!AreEqual(a.ElementType, b.ElementType, comparisonMode))
                {
                    return false;
                }
                for (int i = 0; i < count; i++)
                {
                    if (!AreEqual(a.GenericArguments[i], b.GenericArguments[i], comparisonMode))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static bool AreEqual(GenericParameter a, GenericParameter b, [Optional, DefaultParameterValue(0)] TypeComparisonMode comparisonMode)
        {
            if (object.ReferenceEquals(a, b))
            {
                return true;
            }
            if (a.Position != b.Position)
            {
                return false;
            }
            if (a.Type != b.Type)
            {
                return false;
            }
            TypeReference owner = a.Owner as TypeReference;
            if ((owner != null) && AreEqual(owner, b.Owner as TypeReference, comparisonMode))
            {
                return true;
            }
            MethodReference x = a.Owner as MethodReference;
            return (((x != null) && Unity.IL2CPP.Common.MethodReferenceComparer.AreEqual(x, b.Owner as MethodReference)) || (comparisonMode == TypeComparisonMode.SignatureOnly));
        }

        public static bool AreEqual(TypeReference a, TypeReference b, [Optional, DefaultParameterValue(0)] TypeComparisonMode comparisonMode)
        {
            if (object.ReferenceEquals(a, b))
            {
                return true;
            }
            if ((a == null) || (b == null))
            {
                return false;
            }
            MetadataType metadataType = a.MetadataType;
            MetadataType type2 = b.MetadataType;
            if ((metadataType == MetadataType.GenericInstance) || (type2 == MetadataType.GenericInstance))
            {
                if (metadataType != type2)
                {
                    return false;
                }
                return AreEqual((GenericInstanceType) a, (GenericInstanceType) b, comparisonMode);
            }
            if ((metadataType == MetadataType.Array) || (type2 == MetadataType.Array))
            {
                if (metadataType != type2)
                {
                    return false;
                }
                ArrayType type3 = (ArrayType) a;
                ArrayType type4 = (ArrayType) b;
                if (type3.Rank != type4.Rank)
                {
                    return false;
                }
                return AreEqual(type3.ElementType, type4.ElementType, comparisonMode);
            }
            if ((metadataType == MetadataType.Var) || (type2 == MetadataType.Var))
            {
                if (metadataType != type2)
                {
                    return false;
                }
                return AreEqual((GenericParameter) a, (GenericParameter) b, TypeComparisonMode.Exact);
            }
            if ((metadataType == MetadataType.MVar) || (type2 == MetadataType.MVar))
            {
                if (metadataType != type2)
                {
                    return false;
                }
                return AreEqual((GenericParameter) a, (GenericParameter) b, comparisonMode);
            }
            if ((metadataType == MetadataType.ByReference) || (type2 == MetadataType.ByReference))
            {
                if (metadataType != type2)
                {
                    return false;
                }
                return AreEqual(((ByReferenceType) a).ElementType, ((ByReferenceType) b).ElementType, comparisonMode);
            }
            if ((metadataType == MetadataType.Pointer) || (type2 == MetadataType.Pointer))
            {
                if (metadataType != type2)
                {
                    return false;
                }
                return AreEqual(((PointerType) a).ElementType, ((PointerType) b).ElementType, comparisonMode);
            }
            if ((metadataType == MetadataType.RequiredModifier) || (type2 == MetadataType.RequiredModifier))
            {
                if (metadataType != type2)
                {
                    return false;
                }
                RequiredModifierType type5 = (RequiredModifierType) a;
                RequiredModifierType type6 = (RequiredModifierType) b;
                return (AreEqual(type5.ModifierType, type6.ModifierType, TypeComparisonMode.Exact) && AreEqual(type5.ElementType, type6.ElementType, comparisonMode));
            }
            if ((metadataType == MetadataType.OptionalModifier) || (type2 == MetadataType.OptionalModifier))
            {
                if (metadataType != type2)
                {
                    return false;
                }
                OptionalModifierType type7 = (OptionalModifierType) a;
                OptionalModifierType type8 = (OptionalModifierType) b;
                return (AreEqual(type7.ModifierType, type8.ModifierType, TypeComparisonMode.Exact) && AreEqual(type7.ElementType, type8.ElementType, comparisonMode));
            }
            if ((metadataType == MetadataType.Pinned) || (type2 == MetadataType.Pinned))
            {
                if (metadataType != type2)
                {
                    return false;
                }
                return AreEqual(((PinnedType) a).ElementType, ((PinnedType) b).ElementType, comparisonMode);
            }
            if ((metadataType == MetadataType.Sentinel) || (type2 == MetadataType.Sentinel))
            {
                if (metadataType != type2)
                {
                    return false;
                }
                return AreEqual(((SentinelType) a).ElementType, ((SentinelType) b).ElementType, TypeComparisonMode.Exact);
            }
            if (!a.Name.Equals(b.Name) || !a.Namespace.Equals(b.Namespace))
            {
                return false;
            }
            TypeDefinition definition = a.Resolve();
            TypeDefinition definition2 = b.Resolve();
            return (definition == definition2);
        }

        public override bool Equals(TypeReference x, TypeReference y)
        {
            return AreEqual(x, y, TypeComparisonMode.Exact);
        }

        public override int GetHashCode(TypeReference obj)
        {
            return GetHashCodeFor(obj);
        }

        public static int GetHashCodeFor(TypeReference obj)
        {
            MetadataType metadataType = obj.MetadataType;
            switch (metadataType)
            {
                case MetadataType.GenericInstance:
                {
                    GenericInstanceType type2 = (GenericInstanceType) obj;
                    int num = (GetHashCodeFor(type2.ElementType) * 0x1cfaa2db) + 0x1f;
                    for (int i = 0; i < type2.GenericArguments.Count; i++)
                    {
                        num = (num * 0x1cfaa2db) + GetHashCodeFor(type2.GenericArguments[i]);
                    }
                    return num;
                }
                case MetadataType.Array:
                {
                    ArrayType type3 = (ArrayType) obj;
                    return ((GetHashCodeFor(type3.ElementType) * 0x1cfaa2db) + type3.Rank.GetHashCode());
                }
                case MetadataType.Var:
                case MetadataType.MVar:
                {
                    GenericParameter parameter = (GenericParameter) obj;
                    int num7 = (int) metadataType;
                    int num5 = (parameter.Position.GetHashCode() * 0x1cfaa2db) + num7.GetHashCode();
                    TypeReference owner = parameter.Owner as TypeReference;
                    if (owner != null)
                    {
                        return ((num5 * 0x1cfaa2db) + GetHashCodeFor(owner));
                    }
                    MethodReference reference2 = parameter.Owner as MethodReference;
                    if (reference2 == null)
                    {
                        throw new InvalidOperationException("Generic parameter encountered with invalid owner");
                    }
                    return ((num5 * 0x1cfaa2db) + Unity.IL2CPP.Common.MethodReferenceComparer.GetHashCodeFor(reference2));
                }
                case MetadataType.ByReference:
                {
                    ByReferenceType type4 = (ByReferenceType) obj;
                    return ((GetHashCodeFor(type4.ElementType) * 0x1cfaa2db) * 0x25);
                }
                case MetadataType.Pointer:
                {
                    PointerType type5 = (PointerType) obj;
                    return ((GetHashCodeFor(type5.ElementType) * 0x1cfaa2db) * 0x29);
                }
                case MetadataType.RequiredModifier:
                {
                    RequiredModifierType type6 = (RequiredModifierType) obj;
                    int num8 = GetHashCodeFor(type6.ElementType) * 0x2b;
                    return ((num8 * 0x1cfaa2db) + GetHashCodeFor(type6.ModifierType));
                }
                case MetadataType.OptionalModifier:
                {
                    OptionalModifierType type7 = (OptionalModifierType) obj;
                    int num9 = GetHashCodeFor(type7.ElementType) * 0x2f;
                    return ((num9 * 0x1cfaa2db) + GetHashCodeFor(type7.ModifierType));
                }
                case MetadataType.Pinned:
                {
                    PinnedType type8 = (PinnedType) obj;
                    return ((GetHashCodeFor(type8.ElementType) * 0x1cfaa2db) * 0x35);
                }
                case MetadataType.Sentinel:
                {
                    SentinelType type9 = (SentinelType) obj;
                    return ((GetHashCodeFor(type9.ElementType) * 0x1cfaa2db) * 0x3b);
                }
                case MetadataType.FunctionPointer:
                    throw new NotImplementedException("We currently don't handle function pointer types.");
            }
            return ((obj.Namespace.GetHashCode() * 0x1cfaa2db) + obj.FullName.GetHashCode());
        }
    }
}

