namespace UnityEditor
{
    using System;
    using UnityEngine.Scripting;

    internal class RetainedMode : AssetPostprocessor
    {
        [RequiredByNativeCode]
        private static void UpdateSchedulers()
        {
        }
    }
}

