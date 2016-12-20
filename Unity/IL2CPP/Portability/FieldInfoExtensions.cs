namespace Unity.IL2CPP.Portability
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    [Extension]
    public static class FieldInfoExtensions
    {
        [Extension]
        public static RuntimeFieldHandle GetFieldHandlePortable(FieldInfo fieldInfo)
        {
            return fieldInfo.FieldHandle;
        }
    }
}

