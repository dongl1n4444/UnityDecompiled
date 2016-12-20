namespace Unity.IL2CPP.Portability
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    [Extension]
    public static class PropertyInfoExtensions
    {
        [Extension]
        public static MethodInfo GetGetMethodPortable(PropertyInfo propertyInfo)
        {
            return propertyInfo.GetGetMethod();
        }
    }
}

