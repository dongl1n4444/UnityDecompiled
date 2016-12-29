namespace Unity.IL2CPP.Portability
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public static class CustomAttributeDataExtensions
    {
        public static ConstructorInfo GetConstructorInfoPortable(this CustomAttributeData target) => 
            target.Constructor;
    }
}

