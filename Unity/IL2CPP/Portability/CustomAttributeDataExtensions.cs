namespace Unity.IL2CPP.Portability
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    [Extension]
    public static class CustomAttributeDataExtensions
    {
        [Extension]
        public static ConstructorInfo GetConstructorInfoPortable(CustomAttributeData target)
        {
            return target.Constructor;
        }
    }
}

