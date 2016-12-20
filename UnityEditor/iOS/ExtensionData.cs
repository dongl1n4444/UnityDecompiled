namespace UnityEditor.iOS
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    internal class ExtensionData : ScriptableSingleton<ExtensionData>
    {
        [SerializeField]
        private List<iOSDevice> devices = new List<iOSDevice>();

        internal static List<iOSDevice> Devices
        {
            get
            {
                return ScriptableSingleton<ExtensionData>.instance.devices;
            }
        }
    }
}

