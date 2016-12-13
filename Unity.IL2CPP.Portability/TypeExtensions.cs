using System;
using System.Collections.Generic;
using System.Reflection;

namespace Unity.IL2CPP.Portability
{
	public static class TypeExtensions
	{
		public static Type[] GetNestedTypesPortable(this Type type)
		{
			return type.GetNestedTypes();
		}

		public static Type GetNestedTypePortable(this Type type, string name)
		{
			return type.GetNestedType(name);
		}

		public static Assembly GetAssemblyPortable(this Type type)
		{
			return type.Assembly;
		}

		public static Type GetBaseTypePortable(this Type type)
		{
			return type.BaseType;
		}

		public static bool IsEnumPortable(this Type type)
		{
			return type.IsEnum;
		}

		public static bool IsGenericTypePortable(this Type type)
		{
			return type.IsGenericType;
		}

		public static bool IsGenericTypeDefinitionPortable(this Type type)
		{
			return type.IsGenericTypeDefinition;
		}

		public static bool IsAbstractPortable(this Type type)
		{
			return type.IsAbstract;
		}

		public static bool IsInterfacePortable(this Type type)
		{
			return type.IsInterface;
		}

		public static bool IsSubclassOfPortable(this Type type, Type c)
		{
			return type.IsSubclassOf(c);
		}

		public static bool ContainsGenericParametersPortable(this Type type)
		{
			return type.ContainsGenericParameters;
		}

		public static Type GetGenericTypeDefinitionPortable(this Type type)
		{
			return type.GetGenericTypeDefinition();
		}

		public static MethodInfo GetMethodPortable(this Type type, string name, BindingFlags flags, Type[] parameters)
		{
			return type.GetMethod(name, flags, null, parameters, new ParameterModifier[0]);
		}

		public static IEnumerable<object> GetCustomAttributesPortable(this Type type, Type attributeType, bool inherit)
		{
			return type.GetCustomAttributes(attributeType, inherit);
		}

		public static Module GetModulePortable(this Type type)
		{
			return type.Module;
		}

		public static Type GetTypePortable(this Exception type)
		{
			return type.GetType();
		}
	}
}
