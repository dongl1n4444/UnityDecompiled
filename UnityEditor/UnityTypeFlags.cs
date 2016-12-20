namespace UnityEditor
{
    using System;

    [Flags]
    internal enum UnityTypeFlags
    {
        Abstract = 1,
        Deprecated = 0x10,
        EditorOnly = 4,
        Sealed = 2
    }
}

