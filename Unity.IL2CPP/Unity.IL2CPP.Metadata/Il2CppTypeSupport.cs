using Mono.Cecil;
using System;

namespace Unity.IL2CPP.Metadata
{
	internal class Il2CppTypeSupport
	{
		internal static string For(TypeReference type)
		{
			MetadataType metadataType = type.MetadataType;
			switch (metadataType)
			{
			case MetadataType.Void:
			{
				string result = "IL2CPP_TYPE_VOID";
				return result;
			}
			case MetadataType.Boolean:
			{
				string result = "IL2CPP_TYPE_BOOLEAN";
				return result;
			}
			case MetadataType.Char:
			{
				string result = "IL2CPP_TYPE_CHAR";
				return result;
			}
			case MetadataType.SByte:
			{
				string result = "IL2CPP_TYPE_I1";
				return result;
			}
			case MetadataType.Byte:
			{
				string result = "IL2CPP_TYPE_U1";
				return result;
			}
			case MetadataType.Int16:
			{
				string result = "IL2CPP_TYPE_I2";
				return result;
			}
			case MetadataType.UInt16:
			{
				string result = "IL2CPP_TYPE_U2";
				return result;
			}
			case MetadataType.Int32:
			{
				string result = "IL2CPP_TYPE_I4";
				return result;
			}
			case MetadataType.UInt32:
			{
				string result = "IL2CPP_TYPE_U4";
				return result;
			}
			case MetadataType.Int64:
			{
				string result = "IL2CPP_TYPE_I8";
				return result;
			}
			case MetadataType.UInt64:
			{
				string result = "IL2CPP_TYPE_U8";
				return result;
			}
			case MetadataType.Single:
			{
				string result = "IL2CPP_TYPE_R4";
				return result;
			}
			case MetadataType.Double:
			{
				string result = "IL2CPP_TYPE_R8";
				return result;
			}
			case MetadataType.String:
			{
				string result = "IL2CPP_TYPE_STRING";
				return result;
			}
			case MetadataType.Pointer:
			{
				string result = "IL2CPP_TYPE_PTR";
				return result;
			}
			case MetadataType.ByReference:
			{
				string result = Il2CppTypeSupport.For(((ByReferenceType)type).ElementType);
				return result;
			}
			case MetadataType.ValueType:
			{
				string result = "IL2CPP_TYPE_VALUETYPE";
				return result;
			}
			case MetadataType.Class:
			{
				string result;
				if (type.Resolve().MetadataType == MetadataType.ValueType)
				{
					result = "IL2CPP_TYPE_VALUETYPE";
					return result;
				}
				result = "IL2CPP_TYPE_CLASS";
				return result;
			}
			case MetadataType.Var:
			{
				string result = "IL2CPP_TYPE_VAR";
				return result;
			}
			case MetadataType.Array:
			{
				ArrayType arrayType = (ArrayType)type;
				string result;
				if (arrayType.IsVector)
				{
					result = "IL2CPP_TYPE_SZARRAY";
					return result;
				}
				result = "IL2CPP_TYPE_ARRAY";
				return result;
			}
			case MetadataType.GenericInstance:
			{
				string result = "IL2CPP_TYPE_GENERICINST";
				return result;
			}
			case MetadataType.TypedByReference:
			{
				string result = "IL2CPP_TYPE_TYPEDBYREF";
				return result;
			}
			case (MetadataType)23:
			case (MetadataType)26:
			case (MetadataType)29:
				IL_90:
				if (metadataType == MetadataType.Sentinel)
				{
					throw new ArgumentOutOfRangeException();
				}
				if (metadataType != MetadataType.Pinned)
				{
					throw new ArgumentOutOfRangeException();
				}
				throw new ArgumentOutOfRangeException();
			case MetadataType.IntPtr:
			{
				string result = "IL2CPP_TYPE_I";
				return result;
			}
			case MetadataType.UIntPtr:
			{
				string result = "IL2CPP_TYPE_U";
				return result;
			}
			case MetadataType.FunctionPointer:
				throw new ArgumentOutOfRangeException();
			case MetadataType.Object:
			{
				string result = "IL2CPP_TYPE_OBJECT";
				return result;
			}
			case MetadataType.MVar:
			{
				string result = "IL2CPP_TYPE_MVAR";
				return result;
			}
			case MetadataType.RequiredModifier:
			{
				string result = Il2CppTypeSupport.For(((RequiredModifierType)type).ElementType);
				return result;
			}
			case MetadataType.OptionalModifier:
				throw new ArgumentOutOfRangeException();
			}
			goto IL_90;
		}
	}
}
