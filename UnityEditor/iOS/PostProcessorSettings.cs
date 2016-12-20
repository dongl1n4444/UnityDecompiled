namespace UnityEditor.iOS
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct PostProcessorSettings
    {
        public string OsName;
        public Version MinimumOsVersion;
    }
}

