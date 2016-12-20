namespace Unity.PackageManager
{
    using System;
    using UnityEngine;

    internal class EntryPoint : ScriptableObject
    {
        private void Awake()
        {
            base.hideFlags = HideFlags.HideAndDontSave;
            Unity.PackageManager.PackageManager.Start();
        }
    }
}

