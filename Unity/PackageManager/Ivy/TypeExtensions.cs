namespace Unity.PackageManager.Ivy
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    [Extension]
    internal static class TypeExtensions
    {
        [Extension]
        public static bool CanWrite(MemberInfo member)
        {
            if (member is PropertyInfo)
            {
                return ((PropertyInfo) member).CanWrite;
            }
            return true;
        }

        [Extension]
        public static object GetValue(MemberInfo member, object target)
        {
            if (member is PropertyInfo)
            {
                return ((PropertyInfo) member).GetGetMethod().Invoke(target, null);
            }
            return ((FieldInfo) member).GetValue(target);
        }

        [Extension]
        public static void SetValue(MemberInfo member, object target, object value)
        {
            if (member is PropertyInfo)
            {
                object[] parameters = new object[] { value };
                ((PropertyInfo) member).GetSetMethod().Invoke(target, parameters);
            }
            else
            {
                ((FieldInfo) member).SetValue(target, value);
            }
        }
    }
}

