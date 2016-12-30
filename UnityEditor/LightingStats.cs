namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct LightingStats
    {
        public uint dynamicLightsCount;
        public uint dynamicMeshesCount;
        public uint staticLightsCount;
        public uint staticMeshesCount;
        public uint staticMeshesRealtimeEmissive;
        public uint staticMeshesBakedEmissive;
        public uint lightProbeGroupsCount;
        public uint reflectionProbesCount;
        internal void Reset()
        {
            this.dynamicLightsCount = 0;
            this.dynamicMeshesCount = 0;
            this.staticLightsCount = 0;
            this.staticMeshesCount = 0;
            this.staticMeshesRealtimeEmissive = 0;
            this.staticMeshesBakedEmissive = 0;
            this.lightProbeGroupsCount = 0;
            this.reflectionProbesCount = 0;
        }
    }
}

