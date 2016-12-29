namespace Unity.IL2CPP.Portability
{
    using System;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;

    public static class TypeBuilderExtensions
    {
        public static Type CreateTypePortable(this TypeBuilder typeBuilder) => 
            typeBuilder.CreateType();
    }
}

