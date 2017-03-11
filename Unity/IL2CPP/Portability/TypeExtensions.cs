namespace Unity.IL2CPP.Portability
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public static class TypeExtensions
    {
        public static bool ContainsGenericParametersPortable(this Type type) => 
            type.ContainsGenericParameters;

        public static Assembly GetAssemblyPortable(this Type type) => 
            type.Assembly;

        public static Type GetBaseTypePortable(this Type type) => 
            type.BaseType;

        public static IEnumerable<object> GetCustomAttributesPortable(this Type type, Type attributeType, bool inherit) => 
            type.GetCustomAttributes(attributeType, inherit);

        public static Type GetGenericTypeDefinitionPortable(this Type type) => 
            type.GetGenericTypeDefinition();

        public static MethodInfo GetMethodPortable(this Type type, string name, BindingFlags flags, Type[] parameters) => 
            type.GetMethod(name, flags, null, parameters, new ParameterModifier[0]);

        public static Module GetModulePortable(this Type type) => 
            type.Module;

        public static Type GetNestedTypePortable(this Type type, string name) => 
            type.GetNestedType(name);

        public static Type[] GetNestedTypesPortable(this Type type) => 
            type.GetNestedTypes();

        public static Type GetTypePortable(this Exception type) => 
            type.GetType();

        public static bool IsAbstractPortable(this Type type) => 
            type.IsAbstract;

        public static bool IsAssignableFromPortable(this Type type, Type other) => 
            type.IsAssignableFrom(other);

        public static bool IsEnumPortable(this Type type) => 
            type.IsEnum;

        public static bool IsGenericTypeDefinitionPortable(this Type type) => 
            type.IsGenericTypeDefinition;

        public static bool IsGenericTypePortable(this Type type) => 
            type.IsGenericType;

        public static bool IsInterfacePortable(this Type type) => 
            type.IsInterface;

        public static bool IsSubclassOfPortable(this Type type, Type c) => 
            type.IsSubclassOf(c);
    }
}

