using Mono.Cecil;
using System;
using System.Globalization;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;

namespace Unity.IL2CPP.FileNaming
{
	public class ShortNameGenerator
	{
		[Inject]
		public static INamingService Naming;

		internal static string NonUniqueShortNameFor(MethodReference method)
		{
			return ShortNameGenerator.Naming.ForMethodNameOnly(method);
		}

		internal static string NonUniqueShortNameFor(TypeReference type)
		{
			GenericInstanceType genericInstanceType = type as GenericInstanceType;
			string result;
			if (genericInstanceType != null)
			{
				result = ShortNameGenerator.NonUniqueShortNameFor(genericInstanceType);
			}
			else
			{
				ArrayType arrayType = type as ArrayType;
				if (arrayType != null)
				{
					result = ShortNameGenerator.NonUniqueShortNameFor(arrayType);
				}
				else
				{
					TypeSpecification typeSpecification = type as TypeSpecification;
					if (typeSpecification != null)
					{
						throw new Exception();
					}
					result = ShortNameGenerator.NonUniqueShortNameFor(type.Resolve());
				}
			}
			return result;
		}

		private static string NonUniqueShortNameFor(TypeDefinition type)
		{
			return ShortNameGenerator.Naming.ForFile(type);
		}

		private static string NonUniqueShortNameFor(GenericInstanceType type)
		{
			return ShortNameGenerator.NonUniqueShortNameFor(type.ElementType) + "_gen";
		}

		private static string NonUniqueShortNameFor(ArrayType type)
		{
			return ShortNameGenerator.NonUniqueShortNameFor(type.ElementType) + "_arr" + ((type.Rank <= 1) ? "" : type.Rank.ToString(CultureInfo.InvariantCulture));
		}
	}
}
