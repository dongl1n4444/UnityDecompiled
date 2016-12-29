namespace Unity.IL2CPP.Portability
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public static class FieldInfoExtensions
    {
        public static RuntimeFieldHandle GetFieldHandlePortable(this FieldInfo fieldInfo) => 
            fieldInfo.FieldHandle;
    }
}

