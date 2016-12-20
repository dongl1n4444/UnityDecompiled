namespace Unity.IL2CPP.Metadata
{
    using Mono.Cecil;
    using System;
    using System.Text;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Common;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;

    public class MetadataUtils
    {
        [Inject]
        public static ITypeProviderService TypeProvider;

        public static object ChangePrimitiveType(object o, TypeReference type)
        {
            if ((o is uint) && (type.MetadataType == MetadataType.Int32))
            {
                return (int) ((uint) o);
            }
            if ((o is int) && (type.MetadataType == MetadataType.UInt32))
            {
                return (uint) ((int) o);
            }
            return Convert.ChangeType(o, DetermineTypeForDefaultValueBasedOnDeclaredType(type, o));
        }

        internal static byte[] ConstantDataFor(IConstantProvider constantProvider, TypeReference declaredParameterOrFieldType, string name)
        {
            GenericInstanceType type = declaredParameterOrFieldType as GenericInstanceType;
            if ((type != null) && Unity.IL2CPP.Common.TypeReferenceEqualityComparer.AreEqual(declaredParameterOrFieldType.Resolve(), TypeProvider.SystemNullable, TypeComparisonMode.Exact))
            {
                return ConstantDataFor(constantProvider, type.GenericArguments[0], name);
            }
            if (Extensions.IsEnum(declaredParameterOrFieldType))
            {
                declaredParameterOrFieldType = Extensions.GetUnderlyingEnumType(declaredParameterOrFieldType);
            }
            object constant = constantProvider.Constant;
            if (DetermineMetadataTypeForDefaultValueBasedOnTypeOfConstant(declaredParameterOrFieldType.MetadataType, constant) != declaredParameterOrFieldType.MetadataType)
            {
                constant = ChangePrimitiveType(constant, declaredParameterOrFieldType);
            }
            switch (declaredParameterOrFieldType.MetadataType)
            {
                case MetadataType.Boolean:
                    return new byte[] { (!((bool) constant) ? ((byte) 0) : ((byte) 1)) };

                case MetadataType.Char:
                    return BitConverter.GetBytes((ushort) ((char) constant));

                case MetadataType.SByte:
                    return new byte[] { ((byte) ((sbyte) constant)) };

                case MetadataType.Byte:
                    return new byte[] { ((byte) constant) };

                case MetadataType.Int16:
                    return BitConverter.GetBytes((short) constant);

                case MetadataType.UInt16:
                    return BitConverter.GetBytes((ushort) constant);

                case MetadataType.Int32:
                    return BitConverter.GetBytes((int) constant);

                case MetadataType.UInt32:
                    return BitConverter.GetBytes((uint) constant);

                case MetadataType.Int64:
                    return BitConverter.GetBytes((long) constant);

                case MetadataType.UInt64:
                    return BitConverter.GetBytes((ulong) constant);

                case MetadataType.Single:
                    return BitConverter.GetBytes((float) constant);

                case MetadataType.Double:
                    return BitConverter.GetBytes((double) constant);

                case MetadataType.String:
                {
                    string s = (string) constant;
                    int byteCount = Encoding.UTF8.GetByteCount(s);
                    byte[] destinationArray = new byte[4 + byteCount];
                    Array.Copy(BitConverter.GetBytes(s.Length), destinationArray, 4);
                    Array.Copy(Encoding.UTF8.GetBytes(s), 0, destinationArray, 4, byteCount);
                    return destinationArray;
                }
                case MetadataType.Array:
                case MetadataType.Object:
                    if (constant != null)
                    {
                        throw new InvalidOperationException(string.Format("Default value for field {0} must be null.", name));
                    }
                    return null;
            }
            throw new ArgumentOutOfRangeException();
        }

        private static MetadataType DetermineMetadataTypeForDefaultValueBasedOnTypeOfConstant(MetadataType metadataType, object constant)
        {
            if (constant is byte)
            {
                return MetadataType.Byte;
            }
            if (constant is sbyte)
            {
                return MetadataType.SByte;
            }
            if (constant is ushort)
            {
                return MetadataType.UInt16;
            }
            if (constant is short)
            {
                return MetadataType.Int16;
            }
            if (constant is uint)
            {
                return MetadataType.UInt32;
            }
            if (constant is int)
            {
                return MetadataType.Int32;
            }
            if (constant is ulong)
            {
                return MetadataType.UInt64;
            }
            if (constant is long)
            {
                return MetadataType.Int64;
            }
            if (constant is float)
            {
                return MetadataType.Single;
            }
            if (constant is double)
            {
                return MetadataType.Double;
            }
            if (constant is char)
            {
                return MetadataType.Char;
            }
            if (constant is bool)
            {
                return MetadataType.Boolean;
            }
            return metadataType;
        }

        private static System.Type DetermineTypeForDefaultValueBasedOnDeclaredType(TypeReference type, object constant)
        {
            switch (type.MetadataType)
            {
                case MetadataType.SByte:
                    return typeof(sbyte);

                case MetadataType.Byte:
                    return typeof(byte);

                case MetadataType.Int16:
                    return typeof(short);

                case MetadataType.UInt16:
                    return typeof(ushort);

                case MetadataType.Int32:
                    return typeof(int);

                case MetadataType.UInt32:
                    return typeof(uint);

                case MetadataType.Int64:
                    return typeof(long);

                case MetadataType.UInt64:
                    return typeof(ulong);

                case MetadataType.Single:
                    return typeof(float);

                case MetadataType.Double:
                    return typeof(double);
            }
            return constant.GetType();
        }

        public static TypeReference GetUnderlyingType(TypeReference type)
        {
            if (Extensions.IsEnum(type))
            {
                return Extensions.GetUnderlyingEnumType(type);
            }
            return type;
        }
    }
}

