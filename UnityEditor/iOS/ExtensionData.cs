namespace UnityEditor.iOS
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    internal class ExtensionData : ScriptableSingleton<ExtensionData>
    {
        [SerializeField]
        private List<iOSDevice> devices = new List<iOSDevice>();

        internal static List<iOSDevice> Devices =>
            ScriptableSingleton<ExtensionData>.instance.devices;
    }
}

