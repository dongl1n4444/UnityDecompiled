using Mono.Cecil;
using System;
using System.Text;
using Unity.IL2CPP.Common;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;

namespace Unity.IL2CPP.Metadata
{
	public class MetadataUtils
	{
		[Inject]
		public static ITypeProviderService TypeProvider;

		public static TypeReference GetUnderlyingType(TypeReference type)
		{
			TypeReference result;
			if (type.IsEnum())
			{
				result = type.GetUnderlyingEnumType();
			}
			else
			{
				result = type;
			}
			return result;
		}

		internal static byte[] ConstantDataFor(IConstantProvider constantProvider, TypeReference declaredParameterOrFieldType, string name)
		{
			GenericInstanceType genericInstanceType = declaredParameterOrFieldType as GenericInstanceType;
			byte[] result;
			if (genericInstanceType == null || !TypeReferenceEqualityComparer.AreEqual(declaredParameterOrFieldType.Resolve(), MetadataUtils.TypeProvider.SystemNullable, TypeComparisonMode.Exact))
			{
				if (declaredParameterOrFieldType.IsEnum())
				{
					declaredParameterOrFieldType = declaredParameterOrFieldType.GetUnderlyingEnumType();
				}
				object obj = constantProvider.Constant;
				MetadataType metadataType = MetadataUtils.DetermineMetadataTypeForDefaultValueBasedOnTypeOfConstant(declaredParameterOrFieldType.MetadataType, obj);
				if (metadataType != declaredParameterOrFieldType.MetadataType)
				{
					obj = Convert.ChangeType(obj, MetadataUtils.DetermineTypeForDefaultValueBasedOnDeclaredType(declaredParameterOrFieldType, obj));
				}
				switch (declaredParameterOrFieldType.MetadataType)
				{
				case MetadataType.Boolean:
					result = new byte[]
					{
						(!(bool)obj) ? 0 : 1
					};
					return result;
				case MetadataType.Char:
					result = BitConverter.GetBytes((ushort)((char)obj));
					return result;
				case MetadataType.SByte:
					result = new byte[]
					{
						(byte)((sbyte)obj)
					};
					return result;
				case MetadataType.Byte:
					result = new byte[]
					{
						(byte)obj
					};
					return result;
				case MetadataType.Int16:
					result = BitConverter.GetBytes((short)obj);
					return result;
				case MetadataType.UInt16:
					result = BitConverter.GetBytes((ushort)obj);
					return result;
				case MetadataType.Int32:
					result = BitConverter.GetBytes((int)obj);
					return result;
				case MetadataType.UInt32:
					result = BitConverter.GetBytes((uint)obj);
					return result;
				case MetadataType.Int64:
					result = BitConverter.GetBytes((long)obj);
					return result;
				case MetadataType.UInt64:
					result = BitConverter.GetBytes((ulong)obj);
					return result;
				case MetadataType.Single:
					result = BitConverter.GetBytes((float)obj);
					return result;
				case MetadataType.Double:
					result = BitConverter.GetBytes((double)obj);
					return result;
				case MetadataType.String:
				{
					string text = (string)obj;
					int byteCount = Encoding.UTF8.GetByteCount(text);
					byte[] array = new byte[4 + byteCount];
					Array.Copy(BitConverter.GetBytes(text.Length), array, 4);
					Array.Copy(Encoding.UTF8.GetBytes(text), 0, array, 4, byteCount);
					result = array;
					return result;
				}
				case MetadataType.Array:
				case MetadataType.Object:
					if (obj != null)
					{
						throw new InvalidOperationException(string.Format("Default value for field {0} must be null.", name));
					}
					result = null;
					return result;
				}
				throw new ArgumentOutOfRangeException();
			}
			result = MetadataUtils.ConstantDataFor(constantProvider, genericInstanceType.GenericArguments[0], name);
			return result;
		}

		private static MetadataType DetermineMetadataTypeForDefaultValueBasedOnTypeOfConstant(MetadataType metadataType, object constant)
		{
			MetadataType result;
			if (constant is byte)
			{
				result = MetadataType.Byte;
			}
			else if (constant is sbyte)
			{
				result = MetadataType.SByte;
			}
			else if (constant is ushort)
			{
				result = MetadataType.UInt16;
			}
			else if (constant is short)
			{
				result = MetadataType.Int16;
			}
			else if (constant is uint)
			{
				result = MetadataType.UInt32;
			}
			else if (constant is int)
			{
				result = MetadataType.Int32;
			}
			else if (constant is ulong)
			{
				result = MetadataType.UInt64;
			}
			else if (constant is long)
			{
				result = MetadataType.Int64;
			}
			else if (constant is float)
			{
				result = MetadataType.Single;
			}
			else if (constant is double)
			{
				result = MetadataType.Double;
			}
			else if (constant is char)
			{
				result = MetadataType.Char;
			}
			else if (constant is bool)
			{
				result = MetadataType.Boolean;
			}
			else
			{
				result = metadataType;
			}
			return result;
		}

		private static Type DetermineTypeForDefaultValueBasedOnDeclaredType(TypeReference type, object constant)
		{
			Type result;
			switch (type.MetadataType)
			{
			case MetadataType.SByte:
				result = typeof(sbyte);
				break;
			case MetadataType.Byte:
				result = typeof(byte);
				break;
			case MetadataType.Int16:
				result = typeof(short);
				break;
			case MetadataType.UInt16:
				result = typeof(ushort);
				break;
			case MetadataType.Int32:
				result = typeof(int);
				break;
			case MetadataType.UInt32:
				result = typeof(uint);
				break;
			case MetadataType.Int64:
				result = typeof(long);
				break;
			case MetadataType.UInt64:
				result = typeof(ulong);
				break;
			case MetadataType.Single:
				result = typeof(float);
				break;
			case MetadataType.Double:
				result = typeof(double);
				break;
			default:
				result = constant.GetType();
				break;
			}
			return result;
		}
	}
}
