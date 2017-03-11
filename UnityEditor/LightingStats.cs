namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct LightingStats
    {
        public uint realtimeLightsCount;
        public uint dynamicMeshesCount;
        public uint mixedLightsCount;
        public uint bakedLightsCount;
        public uint staticMeshesCount;
        public uint staticMeshesRealtimeEmissive;
        public uint staticMeshesBakedEmissive;
        public uint lightProbeGroupsCount;
        public uint reflectionProbesCount;
        internal void Reset()
        {
            this.realtimeLightsCount = 0;
            this.dynamicMeshesCount = 0;
            this.mixedLightsCount = 0;
            this.bakedLightsCount = 0;
            this.staticMeshesCount = 0;
            this.staticMeshesRealtimeEmissive = 0;
            this.staticMeshesBakedEmissive = 0;
            this.lightProbeGroupsCount = 0;
            this.reflectionProbesCount = 0;
        }
    }
}

