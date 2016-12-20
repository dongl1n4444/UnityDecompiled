namespace Unity.IL2CPP.Metadata
{
    using Mono.Cecil;
    using System;

    internal class Il2CppTypeSupport
    {
        internal static string For(TypeReference type)
        {
            switch (type.MetadataType)
            {
                case MetadataType.Void:
                    return "IL2CPP_TYPE_VOID";

                case MetadataType.Boolean:
                    return "IL2CPP_TYPE_BOOLEAN";

                case MetadataType.Char:
                    return "IL2CPP_TYPE_CHAR";

                case MetadataType.SByte:
                    return "IL2CPP_TYPE_I1";

                case MetadataType.Byte:
                    return "IL2CPP_TYPE_U1";

                case MetadataType.Int16:
                    return "IL2CPP_TYPE_I2";

                case MetadataType.UInt16:
                    return "IL2CPP_TYPE_U2";

                case MetadataType.Int32:
                    return "IL2CPP_TYPE_I4";

                case MetadataType.UInt32:
                    return "IL2CPP_TYPE_U4";

                case MetadataType.Int64:
                    return "IL2CPP_TYPE_I8";

                case MetadataType.UInt64:
                    return "IL2CPP_TYPE_U8";

                case MetadataType.Single:
                    return "IL2CPP_TYPE_R4";

                case MetadataType.Double:
                    return "IL2CPP_TYPE_R8";

                case MetadataType.String:
                    return "IL2CPP_TYPE_STRING";

                case MetadataType.Pointer:
                    return "IL2CPP_TYPE_PTR";

                case MetadataType.ByReference:
                    return For(((ByReferenceType) type).ElementType);

                case MetadataType.ValueType:
                    return "IL2CPP_TYPE_VALUETYPE";

                case MetadataType.Class:
                    if (type.Resolve().MetadataType != MetadataType.ValueType)
                    {
                        return "IL2CPP_TYPE_CLASS";
                    }
                    return "IL2CPP_TYPE_VALUETYPE";

                case MetadataType.Var:
                    return "IL2CPP_TYPE_VAR";

                case MetadataType.Array:
                {
                    ArrayType type3 = (ArrayType) type;
                    if (!type3.IsVector)
                    {
                        return "IL2CPP_TYPE_ARRAY";
                    }
                    return "IL2CPP_TYPE_SZARRAY";
                }
                case MetadataType.GenericInstance:
                    return "IL2CPP_TYPE_GENERICINST";

                case MetadataType.TypedByReference:
                    return "IL2CPP_TYPE_TYPEDBYREF";

                case MetadataType.IntPtr:
                    return "IL2CPP_TYPE_I";

                case MetadataType.UIntPtr:
                    return "IL2CPP_TYPE_U";

                case MetadataType.FunctionPointer:
                    throw new ArgumentOutOfRangeException();

                case MetadataType.Object:
                    return "IL2CPP_TYPE_OBJECT";

                case MetadataType.MVar:
                    return "IL2CPP_TYPE_MVAR";

                case MetadataType.RequiredModifier:
                    return For(((RequiredModifierType) type).ElementType);

                case MetadataType.OptionalModifier:
                    throw new ArgumentOutOfRangeException();

                case MetadataType.Sentinel:
                    throw new ArgumentOutOfRangeException();

                case MetadataType.Pinned:
                    throw new ArgumentOutOfRangeException();
            }
            throw new ArgumentOutOfRangeException();
        }
    }
}

