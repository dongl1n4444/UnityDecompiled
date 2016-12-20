namespace Unity.IL2CPP.Portability
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    [Extension]
    public static class TypeExtensions
    {
        [Extension]
        public static bool ContainsGenericParametersPortable(Type type)
        {
            return type.ContainsGenericParameters;
        }

        [Extension]
        public static Assembly GetAssemblyPortable(Type type)
        {
            return type.Assembly;
        }

        [Extension]
        public static Type GetBaseTypePortable(Type type)
        {
            return type.BaseType;
        }

        [Extension]
        public static IEnumerable<object> GetCustomAttributesPortable(Type type, Type attributeType, bool inherit)
        {
            return type.GetCustomAttributes(attributeType, inherit);
        }

        [Extension]
        public static Type GetGenericTypeDefinitionPortable(Type type)
        {
            return type.GetGenericTypeDefinition();
        }

        [Extension]
        public static MethodInfo GetMethodPortable(Type type, string name, BindingFlags flags, Type[] parameters)
        {
            return type.GetMethod(name, flags, null, parameters, new ParameterModifier[0]);
        }

        [Extension]
        public static Module GetModulePortable(Type type)
        {
            return type.Module;
        }

        [Extension]
        public static Type GetNestedTypePortable(Type type, string name)
        {
            return type.GetNestedType(name);
        }

        [Extension]
        public static Type[] GetNestedTypesPortable(Type type)
        {
            return type.GetNestedTypes();
        }

        [Extension]
        public static Type GetTypePortable(Exception type)
        {
            return type.GetType();
        }

        [Extension]
        public static bool IsAbstractPortable(Type type)
        {
            return type.IsAbstract;
        }

        [Extension]
        public static bool IsEnumPortable(Type type)
        {
            return type.IsEnum;
        }

        [Extension]
        public static bool IsGenericTypeDefinitionPortable(Type type)
        {
            return type.IsGenericTypeDefinition;
        }

        [Extension]
        public static bool IsGenericTypePortable(Type type)
        {
            return type.IsGenericType;
        }

        [Extension]
        public static bool IsInterfacePortable(Type type)
        {
            return type.IsInterface;
        }

        [Extension]
        public static bool IsSubclassOfPortable(Type type, Type c)
        {
            return type.IsSubclassOf(c);
        }
    }
}

