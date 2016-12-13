using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;

namespace Unity.IL2CPP
{
	internal class GuidProviderComponent : IGuidProvider
	{
		[Inject]
		public static IWindowsRuntimeProjections WindowsRuntimeProjections;

		private static readonly byte[] kParameterizedNamespaceGuid = new byte[]
		{
			17,
			244,
			122,
			213,
			123,
			115,
			66,
			192,
			171,
			174,
			135,
			139,
			30,
			22,
			173,
			238
		};

		public Guid GuidFor(TypeReference type)
		{
			GenericInstanceType genericInstanceType = type as GenericInstanceType;
			Guid result;
			if (genericInstanceType != null)
			{
				result = GuidProviderComponent.ParameterizedGuidFromTypeIdentifier(GuidProviderComponent.IdentifierFor(genericInstanceType));
			}
			else
			{
				if (type is TypeSpecification || type is GenericParameter)
				{
					throw new InvalidOperationException(string.Format("Cannot retrieve GUID for {0}", type.FullName));
				}
				TypeDefinition typeDefinition = type.Resolve();
				CustomAttribute customAttribute = typeDefinition.CustomAttributes.SingleOrDefault((CustomAttribute a) => a.AttributeType.FullName == "System.Runtime.InteropServices.GuidAttribute");
				if (customAttribute != null)
				{
					result = new Guid((string)customAttribute.ConstructorArguments[0].Value);
				}
				else
				{
					customAttribute = typeDefinition.CustomAttributes.SingleOrDefault((CustomAttribute a) => a.AttributeType.FullName == "Windows.Foundation.Metadata.GuidAttribute");
					if (customAttribute == null)
					{
						throw new InvalidOperationException(string.Format("'{0}' doesn't have a GUID.", type.FullName));
					}
					result = new Guid((uint)customAttribute.ConstructorArguments[0].Value, (ushort)customAttribute.ConstructorArguments[1].Value, (ushort)customAttribute.ConstructorArguments[2].Value, (byte)customAttribute.ConstructorArguments[3].Value, (byte)customAttribute.ConstructorArguments[4].Value, (byte)customAttribute.ConstructorArguments[5].Value, (byte)customAttribute.ConstructorArguments[6].Value, (byte)customAttribute.ConstructorArguments[7].Value, (byte)customAttribute.ConstructorArguments[8].Value, (byte)customAttribute.ConstructorArguments[9].Value, (byte)customAttribute.ConstructorArguments[10].Value);
				}
			}
			return result;
		}

		private static Guid ParameterizedGuidFromTypeIdentifier(string typeIdentifier)
		{
			List<byte> list = new List<byte>();
			list.AddRange(GuidProviderComponent.kParameterizedNamespaceGuid);
			list.AddRange(Encoding.UTF8.GetBytes(typeIdentifier));
			byte[] array;
			using (SHA1Managed sHA1Managed = new SHA1Managed())
			{
				array = sHA1Managed.ComputeHash(list.ToArray());
			}
			int a = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(array, 0));
			short b = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(array, 4));
			short num = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(array, 6));
			byte[] array2 = array.Skip(8).Take(8).ToArray<byte>();
			num &= 4095;
			num |= 20480;
			byte[] expr_A3_cp_0 = array2;
			int expr_A3_cp_1 = 0;
			expr_A3_cp_0[expr_A3_cp_1] &= 63;
			byte[] expr_B2_cp_0 = array2;
			int expr_B2_cp_1 = 0;
			expr_B2_cp_0[expr_B2_cp_1] |= 128;
			return new Guid(a, b, num, array2);
		}

		public static string IdentifierFor(IEnumerable<TypeReference> nameElements)
		{
			return (from element in nameElements
			select GuidProviderComponent.IdentifierFor(element)).AggregateWith(";");
		}

		private static string IdentifierFor(TypeReference type)
		{
			string result;
			switch (type.MetadataType)
			{
			case MetadataType.Boolean:
				result = "b1";
				return result;
			case MetadataType.Char:
				result = "c2";
				return result;
			case MetadataType.Byte:
				result = "u1";
				return result;
			case MetadataType.Int16:
				result = "i2";
				return result;
			case MetadataType.UInt16:
				result = "u2";
				return result;
			case MetadataType.Int32:
				result = "i4";
				return result;
			case MetadataType.UInt32:
				result = "u4";
				return result;
			case MetadataType.Int64:
				result = "i8";
				return result;
			case MetadataType.UInt64:
				result = "u8";
				return result;
			case MetadataType.Single:
				result = "f4";
				return result;
			case MetadataType.Double:
				result = "f8";
				return result;
			case MetadataType.String:
				result = "string";
				return result;
			case MetadataType.ValueType:
				if (type.FullName == "System.Guid")
				{
					result = "g16";
					return result;
				}
				break;
			case MetadataType.Object:
				result = "cinterface(IInspectable)";
				return result;
			}
			TypeDefinition typeDefinition = GuidProviderComponent.WindowsRuntimeProjections.ProjectToWindowsRuntime(type.Resolve()).Resolve();
			if (type.MetadataType != MetadataType.Class && type.MetadataType != MetadataType.ValueType && type.MetadataType != MetadataType.GenericInstance)
			{
				throw new InvalidOperationException(string.Format("Cannot compute type identifier for {0}, as its metadata type is not supported: {1}.", type.FullName, type.MetadataType));
			}
			if (!typeDefinition.IsWindowsRuntime)
			{
				throw new InvalidOperationException(string.Format("Cannot compute type identifier for {0}, as it is not a Windows Runtime type.", type.FullName));
			}
			GenericInstanceType genericInstanceType = type as GenericInstanceType;
			if (genericInstanceType != null)
			{
				result = string.Format("pinterface({{{0}}};{1})", typeDefinition.GetGuid().ToString(), GuidProviderComponent.IdentifierFor(genericInstanceType.GenericArguments));
			}
			else if (typeDefinition.MetadataType == MetadataType.ValueType)
			{
				if (typeDefinition.IsEnum())
				{
					result = string.Format("enum({0};{1})", typeDefinition.FullName, GuidProviderComponent.IdentifierFor(typeDefinition.GetUnderlyingEnumType()));
				}
				else
				{
					IEnumerable<TypeReference> nameElements = from f in typeDefinition.Fields
					where !f.IsStatic
					select f.FieldType;
					result = string.Format("struct({0};{1})", typeDefinition.FullName, GuidProviderComponent.IdentifierFor(nameElements));
				}
			}
			else if (typeDefinition.IsInterface)
			{
				result = string.Format("{{{0}}}", typeDefinition.GetGuid().ToString());
			}
			else if (typeDefinition.IsDelegate())
			{
				result = string.Format("delegate({{{0}}})", typeDefinition.GetGuid().ToString());
			}
			else
			{
				TypeReference typeReference = typeDefinition.ExtractDefaultInterface();
				GenericInstanceType genericInstanceType2 = typeReference as GenericInstanceType;
				if (genericInstanceType2 != null)
				{
					result = string.Format("rc({0};{1})", typeDefinition.FullName, GuidProviderComponent.IdentifierFor(genericInstanceType2));
				}
				else
				{
					result = string.Format("rc({0};{{{1}}})", typeDefinition.FullName, typeReference.GetGuid().ToString());
				}
			}
			return result;
		}
	}
}
