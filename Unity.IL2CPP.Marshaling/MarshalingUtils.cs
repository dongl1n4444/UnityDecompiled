using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;
using Unity.IL2CPP.Marshaling.MarshalInfoWriters;

namespace Unity.IL2CPP.Marshaling
{
	public static class MarshalingUtils
	{
		[Inject]
		public static INamingService Naming;

		[Inject]
		public static IWindowsRuntimeProjections WindowsRuntimeProjections;

		internal static bool IsBlittable(TypeReference type, NativeType? nativeType, MarshalType marshalType)
		{
			return !type.IsGenericInstance && !(type is TypeSpecification) && !(type is ArrayType) && MarshalingUtils.IsBlittable(type.Resolve(), nativeType, marshalType);
		}

		internal static bool IsBlittable(TypeDefinition type, NativeType? nativeType, MarshalType marshalType)
		{
			bool result;
			if (type.HasGenericParameters)
			{
				result = false;
			}
			else if (type.IsEnum)
			{
				result = MarshalingUtils.IsPrimitiveBlittable(type.GetUnderlyingEnumType().Resolve(), nativeType, marshalType);
			}
			else
			{
				result = ((type.IsSequentialLayout || type.IsExplicitLayout) && MarshalingUtils.AreFieldsBlittable(type, nativeType, marshalType));
			}
			return result;
		}

		private static bool AreFieldsBlittable(TypeDefinition typeDef, NativeType? nativeType, MarshalType marshalType)
		{
			bool result;
			if (typeDef.IsPrimitive)
			{
				result = MarshalingUtils.IsPrimitiveBlittable(typeDef, nativeType, marshalType);
			}
			else
			{
				result = typeDef.Fields.All((FieldDefinition field) => field.IsStatic || (field.FieldType.IsValueType() && MarshalingUtils.IsBlittable(field.FieldType, MarshalingUtils.GetFieldNativeType(field), marshalType)));
			}
			return result;
		}

		private static NativeType? GetFieldNativeType(FieldDefinition field)
		{
			NativeType? result;
			if (field.MarshalInfo == null)
			{
				result = null;
			}
			else
			{
				result = new NativeType?(field.MarshalInfo.NativeType);
			}
			return result;
		}

		private static bool IsPrimitiveBlittable(TypeDefinition type, NativeType? nativeType, MarshalType marshalType)
		{
			bool result;
			if (nativeType.HasValue)
			{
				switch (type.MetadataType)
				{
				case MetadataType.Boolean:
				case MetadataType.SByte:
				case MetadataType.Byte:
					result = (nativeType == NativeType.U1 || nativeType == NativeType.I1);
					return result;
				case MetadataType.Char:
				case MetadataType.Int16:
				case MetadataType.UInt16:
					result = (nativeType == NativeType.U2 || nativeType == NativeType.I2);
					return result;
				case MetadataType.Int32:
				case MetadataType.UInt32:
					result = (nativeType == NativeType.U4 || nativeType == NativeType.I4);
					return result;
				case MetadataType.Int64:
				case MetadataType.UInt64:
					result = (nativeType == NativeType.U8 || nativeType == NativeType.I8);
					return result;
				case MetadataType.Single:
					result = (nativeType == NativeType.R4);
					return result;
				case MetadataType.Double:
					result = (nativeType == NativeType.R8);
					return result;
				case MetadataType.IntPtr:
				case MetadataType.UIntPtr:
					result = (nativeType == NativeType.UInt || nativeType == NativeType.Int);
					return result;
				}
				throw new ArgumentException(string.Format("{0} is not a primitive!", type.FullName));
			}
			result = (marshalType == MarshalType.WindowsRuntime || (type.MetadataType != MetadataType.Boolean && type.MetadataType != MetadataType.Char));
			return result;
		}

		internal static bool IsStringBuilder(TypeReference type)
		{
			return type.MetadataType == MetadataType.Class && type.FullName == "System.Text.StringBuilder";
		}

		internal static IEnumerable<FieldDefinition> NonStaticFieldsOf(TypeDefinition typeDefinition)
		{
			return from field in typeDefinition.Fields
			where !field.IsStatic
			select field;
		}

		internal static bool UseUnicodeAsDefaultMarshalingForStringParameters(MethodReference method)
		{
			MethodDefinition methodDefinition = method.Resolve();
			return methodDefinition.HasPInvokeInfo && methodDefinition.PInvokeInfo.IsCharSetUnicode;
		}

		public static string MarshalTypeToString(MarshalType marshalType)
		{
			string result;
			switch (marshalType)
			{
			case MarshalType.PInvoke:
				result = "pinvoke";
				break;
			case MarshalType.COM:
				result = "com";
				break;
			case MarshalType.WindowsRuntime:
				result = "windows_runtime";
				break;
			default:
				throw new ArgumentException(string.Format("Unexpected MarshalType value '{0}'.", marshalType), "marshalType");
			}
			return result;
		}

		public static string MarshalTypeToNiceString(MarshalType marshalType)
		{
			string result;
			switch (marshalType)
			{
			case MarshalType.PInvoke:
				result = "P/Invoke";
				break;
			case MarshalType.COM:
				result = "COM";
				break;
			case MarshalType.WindowsRuntime:
				result = "Windows Runtime";
				break;
			default:
				throw new ArgumentException(string.Format("Unexpected MarshalType value '{0}'.", marshalType), "marshalType");
			}
			return result;
		}

		public static IEnumerable<FieldDefinition> GetMarshaledFields(TypeDefinition type, MarshalType marshalType)
		{
			return (from t in type.GetTypeHierarchy()
			where t == type || MarshalDataCollector.MarshalInfoWriterFor(t, marshalType, null, false, false, false, null).HasNativeStructDefinition
			select t).SelectMany((TypeDefinition t) => MarshalingUtils.NonStaticFieldsOf(t));
		}

		public static IEnumerable<DefaultMarshalInfoWriter> GetFieldMarshalInfoWriters(TypeDefinition type, MarshalType marshalType)
		{
			return from f in MarshalingUtils.GetMarshaledFields(type, marshalType)
			select MarshalDataCollector.MarshalInfoWriterFor(f.FieldType, marshalType, f.MarshalInfo, type.IsUnicodeClass, false, true, null);
		}

		public static MarshalType[] GetMarshalTypesForMarshaledType(TypeDefinition type)
		{
			MarshalType[] result;
			if (type.IsWindowsRuntime || MarshalingUtils.WindowsRuntimeProjections.ProjectToWindowsRuntime(type) != type)
			{
				if (type.MetadataType == MetadataType.ValueType || type.IsDelegate())
				{
					result = new MarshalType[]
					{
						MarshalType.PInvoke,
						MarshalType.COM,
						MarshalType.WindowsRuntime
					};
					return result;
				}
			}
			result = new MarshalType[]
			{
				MarshalType.PInvoke,
				MarshalType.COM
			};
			return result;
		}
	}
}
