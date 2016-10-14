using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Unity.IL2CPP.FileNaming
{
	internal class UniqueLongNameGenerator
	{
		[CompilerGenerated]
		private static Func<TypeReference, string> <>f__mg$cache0;

		[CompilerGenerated]
		private static Func<TypeReference, string> <>f__mg$cache1;

		internal static string NameFor(MethodReference method)
		{
			string text = string.Format("{0} - {1}", UniqueLongNameGenerator.NameFor(method.DeclaringType), method.Resolve().FullName);
			GenericInstanceMethod genericInstanceMethod = method as GenericInstanceMethod;
			if (genericInstanceMethod != null)
			{
				string arg_67_0 = text;
				string arg_67_1 = "[";
				IEnumerable<TypeReference> arg_58_0 = genericInstanceMethod.GenericArguments;
				if (UniqueLongNameGenerator.<>f__mg$cache0 == null)
				{
					UniqueLongNameGenerator.<>f__mg$cache0 = new Func<TypeReference, string>(UniqueLongNameGenerator.NameFor);
				}
				text = arg_67_0 + arg_67_1 + arg_58_0.Select(UniqueLongNameGenerator.<>f__mg$cache0).AggregateWithComma() + "]";
			}
			return text;
		}

		internal static string NameFor(TypeReference type)
		{
			GenericInstanceType genericInstanceType = type as GenericInstanceType;
			string result;
			if (genericInstanceType != null)
			{
				result = UniqueLongNameGenerator.NameFor(genericInstanceType);
			}
			else
			{
				ArrayType arrayType = type as ArrayType;
				if (arrayType != null)
				{
					result = UniqueLongNameGenerator.NameFor(arrayType);
				}
				else
				{
					PointerType pointerType = type as PointerType;
					if (pointerType != null)
					{
						result = UniqueLongNameGenerator.NameFor(pointerType);
					}
					else
					{
						TypeSpecification typeSpecification = type as TypeSpecification;
						if (typeSpecification != null)
						{
							throw new Exception();
						}
						result = UniqueLongNameGenerator.NameFor(type.Resolve());
					}
				}
			}
			return result;
		}

		private static string NameFor(TypeDefinition type)
		{
			return string.Format("{0} {1}", type.Resolve().Module.Assembly.Name.Name, type.FullName);
		}

		private static string NameFor(GenericInstanceType type)
		{
			string arg_4D_0 = "{0} {1}";
			object arg_4D_1 = UniqueLongNameGenerator.NameFor(type.ElementType);
			string arg_48_0 = "[";
			IEnumerable<TypeReference> arg_39_0 = type.GenericArguments;
			if (UniqueLongNameGenerator.<>f__mg$cache1 == null)
			{
				UniqueLongNameGenerator.<>f__mg$cache1 = new Func<TypeReference, string>(UniqueLongNameGenerator.NameFor);
			}
			return string.Format(arg_4D_0, arg_4D_1, arg_48_0 + arg_39_0.Select(UniqueLongNameGenerator.<>f__mg$cache1).AggregateWithComma() + "]");
		}

		private static string NameFor(ArrayType type)
		{
			return string.Format("{0} {1}", type.Resolve().Module.Assembly.Name.Name, type.FullName);
		}

		private static string NameFor(PointerType type)
		{
			return string.Format("{0} {1}", type.Resolve().Module.Assembly.Name.Name, type.FullName);
		}
	}
}
