namespace UnityEditor
{
    using System;
    using UnityEngine;
    using UnityEngine.Rendering;

    [Serializable]
    internal class CubemapInfo
    {
        [NonSerialized]
        public bool alreadyComputed;
        public SphericalHarmonicsL2 ambientProbe;
        public float angleOffset = 0f;
        public Cubemap cubemap;
        public CubemapInfo cubemapShadowInfo;
        private const float kDefaultShadowIntensity = 0.3f;
        public int serialIndexMain;
        public int serialIndexShadow;
        public ShadowInfo shadowInfo = new ShadowInfo();

        public void ResetEnvInfos()
        {
            this.angleOffset = 0f;
        }

        public void SetCubemapShadowInfo(CubemapInfo newCubemapShadowInfo)
        {
            this.cubemapShadowInfo = newCubemapShadowInfo;
            this.shadowInfo.shadowIntensity = (newCubemapShadowInfo != this) ? 1f : 0.3f;
            this.shadowInfo.shadowColor = Color.white;
        }
    }
}

