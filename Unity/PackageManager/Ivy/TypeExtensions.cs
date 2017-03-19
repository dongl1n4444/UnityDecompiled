namespace Unity.PackageManager.Ivy
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    internal static class TypeExtensions
    {
        public static bool CanWrite(this MemberInfo member)
        {
            if (member is PropertyInfo)
            {
                return ((PropertyInfo) member).CanWrite;
            }
            return true;
        }

        public static object GetValue(this MemberInfo member, object target)
        {
            if (member is PropertyInfo)
            {
                return ((PropertyInfo) member).GetGetMethod().Invoke(target, null);
            }
            return ((FieldInfo) member).GetValue(target);
        }

        public static void SetValue(this MemberInfo member, object target, object value)
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

