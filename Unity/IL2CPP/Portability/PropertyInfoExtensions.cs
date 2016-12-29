namespace Unity.IL2CPP.Portability
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public static class PropertyInfoExtensions
    {
        public static MethodInfo GetGetMethodPortable(this PropertyInfo propertyInfo) => 
            propertyInfo.GetGetMethod();
    }
}

