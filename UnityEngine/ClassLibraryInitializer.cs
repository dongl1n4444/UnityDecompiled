namespace UnityEngine
{
    using System;
    using UnityEngine.Scripting;

    internal static class ClassLibraryInitializer
    {
        [RequiredByNativeCode]
        private static void Init()
        {
            UnityLogWriter.Init();
        }
    }
}

