namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    [StructLayout(LayoutKind.Sequential), UsedByNativeCode]
    internal struct LightmapConvergence
    {
        public Hash128 cullingHash;
        public int visibleConvergedDirectTexelCount;
        public int visibleConvergedGITexelCount;
        public int visibleTexelCount;
        public int convergedDirectTexelCount;
        public int convergedGITexelCount;
        public int occupiedTexelCount;
        public int minDirectSamples;
        public int minGISamples;
        public int maxDirectSamples;
        public int maxGISamples;
        public int avgDirectSamples;
        public int avgGISamples;
        public float progress;
        public bool IsConverged() => 
            ((this.convergedDirectTexelCount == this.occupiedTexelCount) && (this.convergedGITexelCount == this.occupiedTexelCount));

        public bool IsValid() => 
            (-1 != this.visibleConvergedDirectTexelCount);
    }
}

