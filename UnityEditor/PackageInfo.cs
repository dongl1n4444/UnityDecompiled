namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct PackageInfo
    {
        public string packagePath;
        public string jsonInfo;
        public string iconURL;
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern PackageInfo[] GetPackageList();
    }
}

