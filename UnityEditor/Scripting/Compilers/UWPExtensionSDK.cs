namespace UnityEditor.Scripting.Compilers
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct UWPExtensionSDK
    {
        public readonly string Name;
        public readonly string Version;
        public readonly string ManifestPath;
        public UWPExtensionSDK(string name, string version, string manifestPath)
        {
            this.Name = name;
            this.Version = version;
            this.ManifestPath = manifestPath;
        }
    }
}

