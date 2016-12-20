namespace Unity.IL2CPP.Portability
{
    using System;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;

    [Extension]
    public static class TypeBuilderExtensions
    {
        [Extension]
        public static Type CreateTypePortable(TypeBuilder typeBuilder)
        {
            return typeBuilder.CreateType();
        }
    }
}

