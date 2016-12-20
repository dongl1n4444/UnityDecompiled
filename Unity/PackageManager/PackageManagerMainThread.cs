namespace Unity.PackageManager
{
    using System;
    using UnityEditor;
    using UnityEngine;

    [InitializeOnLoad]
    internal class PackageManagerMainThread
    {
        static PackageManagerMainThread()
        {
            if (Unity.PackageManager.PackageManager.Ready)
            {
                ScriptableObject.CreateInstance<EntryPoint>();
            }
        }
    }
}

