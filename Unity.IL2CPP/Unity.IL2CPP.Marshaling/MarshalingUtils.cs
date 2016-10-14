using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.IL2CPP.ILPreProcessor;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;

namespace Unity.IL2CPP.Marshaling
{
	public static class MarshalingUtils
	{
		[Inject]
		public static INamingService Naming;

		internal static bool IsBlittable(TypeReference type, MarshalInfo marshalInfo)
		{
			return !type.IsGenericInstance && !(type is TypeSpecification) && MarshalingUtils.IsBlittable(type.Resolve(), marshalInfo);
		}

		internal static bool IsBlittable(TypeDefinition type, MarshalInfo marshalInfo)
		{
			return !type.HasGenericParameters && (type.IsSequentialLayout || type.IsExplicitLayout) && MarshalingUtils.AreFieldsBlittable(type, marshalInfo);
		}

		private static bool AreFieldsBlittable(TypeDefinition typeDef, MarshalInfo marshalInfo)
		{
			bool result;
			if (typeDef.IsPrimitive)
			{
				result = MarshalingUtils.IsPrimitiveBlittable(typeDef, marshalInfo);
			}
			else
			{
				result = typeDef.Fields.All((FieldDefinition field) => field.IsStatic || (field.FieldType.IsValueType() && MarshalingUtils.IsBlittable(field.FieldType, field.MarshalInfo)));
			}
			return result;
		}

		private static bool IsPrimitiveBlittable(TypeDefinition type, MarshalInfo marshalInfo)
		{
			bool result;
			if (marshalInfo != null)
			{
				switch (type.MetadataType)
				{
				case MetadataType.Boolean:
				case MetadataType.SByte:
				case MetadataType.Byte:
					result = (marshalInfo.NativeType == NativeType.U1 || marshalInfo.NativeType == NativeType.I1);
					return result;
				case MetadataType.Char:
				case MetadataType.Int16:
				case MetadataType.UInt16:
					result = (marshalInfo.NativeType == NativeType.U2 || marshalInfo.NativeType == NativeType.I2);
					return result;
				case MetadataType.Int32:
				case MetadataType.UInt32:
					result = (marshalInfo.NativeType == NativeType.U4 || marshalInfo.NativeType == NativeType.I4);
					return result;
				case MetadataType.Int64:
				case MetadataType.UInt64:
					result = (marshalInfo.NativeType == NativeType.U8 || marshalInfo.NativeType == NativeType.I8);
					return result;
				case MetadataType.Single:
					result = (marshalInfo.NativeType == NativeType.R4);
					return result;
				case MetadataType.Double:
					result = (marshalInfo.NativeType == NativeType.R8);
					return result;
				case MetadataType.IntPtr:
				case MetadataType.UIntPtr:
					result = (marshalInfo.NativeType == NativeType.UInt || marshalInfo.NativeType == NativeType.Int);
					return result;
				}
				throw new ArgumentException(string.Format("{0} is not a primitive!", type.FullName));
			}
			result = (type.MetadataType != MetadataType.Boolean && type.MetadataType != MetadataType.Char);
			return result;
		}

		internal static void EmitNullCheckFor(CppCodeWriter writer, string variableName, string line)
		{
			writer.WriteLine("if ({0} != NULL)", new object[]
			{
				variableName
			});
			writer.BeginBlock();
			writer.WriteLine(line);
			writer.EndBlock(false);
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

		public static string NativeParameterTypeFor(ParameterDefinition parameter, MarshalType marshalType, TypeResolver typeResolver, bool defaultToUnicodeStringMarshalMarshaling)
		{
			return MarshalDataCollector.MarshalInfoWriterFor(typeResolver.Resolve(parameter.ParameterType), marshalType, parameter.MarshalInfo, defaultToUnicodeStringMarshalMarshaling, false).MarshaledDecoratedTypeName;
		}

		public static string NativeReturnTypeFor(MethodReturnType methodReturnType, MarshalType marshalType, TypeResolver typeResolver, bool defaultToUnicodeStringMarshalMarshaling)
		{
			return MarshalDataCollector.MarshalInfoWriterFor(typeResolver.Resolve(methodReturnType.ReturnType), marshalType, methodReturnType.MarshalInfo, defaultToUnicodeStringMarshalMarshaling, false).MarshaledDecoratedTypeName;
		}

		public static string MarshalTypeToString(MarshalType marshalType)
		{
			string result;
			if (marshalType != MarshalType.PInvoke)
			{
				if (marshalType != MarshalType.COM)
				{
					throw new ArgumentException(string.Format("Unexpected MarshalType value '{0}'.", marshalType), "marshalType");
				}
				result = "com";
			}
			else
			{
				result = "pinvoke";
			}
			return result;
		}
	}
}
