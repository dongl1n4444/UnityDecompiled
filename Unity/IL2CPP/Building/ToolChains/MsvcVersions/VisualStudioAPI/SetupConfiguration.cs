namespace Unity.IL2CPP.Building.ToolChains.MsvcVersions.VisualStudioAPI
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [ComImport, Guid("177F0C4A-1CD3-4DE7-A32C-71DBBB9FA36D")]
    public class SetupConfiguration : ISetupConfiguration
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern IEnumSetupInstances EnumInstances();
        [return: MarshalAs(UnmanagedType.IUnknown)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern object GetInstanceForCurrentProcess();
        [return: MarshalAs(UnmanagedType.IUnknown)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern object GetInstanceForPath([In, MarshalAs(UnmanagedType.LPWStr)] string path);
    }
}

