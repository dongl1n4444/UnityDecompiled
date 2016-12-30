namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    internal class LightModeValidator
    {
        [CompilerGenerated]
        private static Func<Light, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<Light, bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<Material, bool> <>f__am$cache10;
        [CompilerGenerated]
        private static Func<Material, bool> <>f__am$cache11;
        [CompilerGenerated]
        private static Func<Material, bool> <>f__am$cache12;
        [CompilerGenerated]
        private static Func<Material, bool> <>f__am$cache13;
        [CompilerGenerated]
        private static Func<Light, bool> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<Light, bool> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<MeshRenderer, bool> <>f__am$cache4;
        [CompilerGenerated]
        private static Func<MeshRenderer, bool> <>f__am$cache5;
        [CompilerGenerated]
        private static Func<MeshRenderer, bool> <>f__am$cache6;
        [CompilerGenerated]
        private static Func<MeshRenderer, bool> <>f__am$cache7;
        [CompilerGenerated]
        private static Func<MeshRenderer, bool> <>f__am$cache8;
        [CompilerGenerated]
        private static Func<MeshRenderer, bool> <>f__am$cache9;
        [CompilerGenerated]
        private static Func<MeshRenderer, bool> <>f__am$cacheA;
        [CompilerGenerated]
        private static Func<MeshRenderer, bool> <>f__am$cacheB;
        [CompilerGenerated]
        private static Func<LightProbeGroup, bool> <>f__am$cacheC;
        [CompilerGenerated]
        private static Func<LightProbeGroup, bool> <>f__am$cacheD;
        [CompilerGenerated]
        private static Func<ReflectionProbe, bool> <>f__am$cacheE;
        [CompilerGenerated]
        private static Func<ReflectionProbe, bool> <>f__am$cacheF;

        internal static void AnalyzeScene(int dynamicMode, int stationaryMode, int staticMode, int ambientMode, ref Stats stats)
        {
            stats.Reset();
            stats.dynamicMode = dynamicMode;
            stats.stationaryMode = stationaryMode;
            stats.staticMode = staticMode;
            stats.ambientMode = ambientMode;
            LightmapEditorSettings.AnalyzeLighting(out stats.enabled, out stats.active, out stats.inactive);
            stats.emitterMask = Emitters.None;
            stats.emitterMask |= (stats.enabled.dynamicLightsCount <= 0) ? Emitters.None : Emitters.DynamicLight;
            stats.emitterMask |= (stats.enabled.staticMeshesRealtimeEmissive <= 0) ? Emitters.None : Emitters.DynamicEmissive;
            stats.emitterMask |= !IsAmbientDynamic(ref stats) ? Emitters.None : Emitters.DynamicAmbient;
            stats.emitterMask |= (stats.enabled.staticLightsCount <= 0) ? Emitters.None : Emitters.BakedLight;
            stats.emitterMask |= (stats.enabled.staticMeshesBakedEmissive <= 0) ? Emitters.None : Emitters.BakedEmissive;
            stats.emitterMask |= !IsAmbientBaked(ref stats) ? Emitters.None : Emitters.BakedAmbient;
            stats.receiverMask = Receivers.None;
            stats.receiverMask |= (stats.enabled.lightProbeGroupsCount <= 0) ? Receivers.None : Receivers.LightProbe;
            stats.receiverMask |= (stats.enabled.staticMeshesCount <= 0) ? Receivers.None : Receivers.StaticMesh;
            if (stats.receiverMask == Receivers.None)
            {
                stats.requiresRealtimeGI = false;
                stats.requiresLightmaps = false;
            }
            else
            {
                stats.requiresRealtimeGI = IsRealtimeGI(ref stats);
                stats.requiresLightmaps = (stats.emitterMask & Emitters.Baked) != Emitters.None;
            }
        }

        internal static IEnumerable<Component> GetBakedEmissive(bool bEnabled)
        {
            if (bEnabled)
            {
                if (<>f__am$cacheA == null)
                {
                    <>f__am$cacheA = delegate (MeshRenderer mr) {
                        if (!mr.enabled || !GameObjectUtility.AreStaticEditorFlagsSet(mr.gameObject, StaticEditorFlags.LightmapStatic))
                        {
                            return false;
                        }
                        if (<>f__am$cache12 == null)
                        {
                            <>f__am$cache12 = m => (m != null) && (m.globalIlluminationFlags == MaterialGlobalIlluminationFlags.BakedEmissive);
                        }
                        return Enumerable.Any<Material>(mr.sharedMaterials, <>f__am$cache12);
                    };
                }
                return Enumerable.Where<MeshRenderer>(Object.FindObjectsOfType<MeshRenderer>(), <>f__am$cacheA).Cast<Component>();
            }
            if (<>f__am$cacheB == null)
            {
                <>f__am$cacheB = delegate (MeshRenderer mr) {
                    if (mr.enabled || !GameObjectUtility.AreStaticEditorFlagsSet(mr.gameObject, StaticEditorFlags.LightmapStatic))
                    {
                        return false;
                    }
                    if (<>f__am$cache13 == null)
                    {
                        <>f__am$cache13 = m => (m != null) && (m.globalIlluminationFlags == MaterialGlobalIlluminationFlags.BakedEmissive);
                    }
                    return Enumerable.Any<Material>(mr.sharedMaterials, <>f__am$cache13);
                };
            }
            return Enumerable.Where<MeshRenderer>(Object.FindObjectsOfType<MeshRenderer>(), <>f__am$cacheB).Cast<Component>();
        }

        internal static IEnumerable<Component> GetDynamicLights(bool bEnabled)
        {
            if (bEnabled)
            {
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = l => l.isActiveAndEnabled && !l.isStatic;
                }
                return Enumerable.Where<Light>(Object.FindObjectsOfType<Light>(), <>f__am$cache0).Cast<Component>();
            }
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = l => !l.isActiveAndEnabled && !l.isStatic;
            }
            return Enumerable.Where<Light>(Object.FindObjectsOfType<Light>(), <>f__am$cache1).Cast<Component>();
        }

        internal static IEnumerable<Component> GetDynamicMeshes(bool bEnabled)
        {
            if (bEnabled)
            {
                if (<>f__am$cache4 == null)
                {
                    <>f__am$cache4 = mr => mr.enabled && !GameObjectUtility.AreStaticEditorFlagsSet(mr.gameObject, StaticEditorFlags.LightmapStatic);
                }
                return Enumerable.Where<MeshRenderer>(Object.FindObjectsOfType<MeshRenderer>(), <>f__am$cache4).Cast<Component>();
            }
            if (<>f__am$cache5 == null)
            {
                <>f__am$cache5 = mr => !mr.enabled && !GameObjectUtility.AreStaticEditorFlagsSet(mr.gameObject, StaticEditorFlags.LightmapStatic);
            }
            return Enumerable.Where<MeshRenderer>(Object.FindObjectsOfType<MeshRenderer>(), <>f__am$cache5).Cast<Component>();
        }

        internal static IEnumerable<Component> GetLightProbeGroups(bool bEnabled)
        {
            if (bEnabled)
            {
                if (<>f__am$cacheC == null)
                {
                    <>f__am$cacheC = lpg => lpg.enabled;
                }
                return Enumerable.Where<LightProbeGroup>(Object.FindObjectsOfType<LightProbeGroup>(), <>f__am$cacheC).Cast<Component>();
            }
            if (<>f__am$cacheD == null)
            {
                <>f__am$cacheD = lpg => !lpg.enabled;
            }
            return Enumerable.Where<LightProbeGroup>(Object.FindObjectsOfType<LightProbeGroup>(), <>f__am$cacheD).Cast<Component>();
        }

        internal static IEnumerable<Component> GetRealtimeEmissive(bool bEnabled)
        {
            if (bEnabled)
            {
                if (<>f__am$cache8 == null)
                {
                    <>f__am$cache8 = delegate (MeshRenderer mr) {
                        if (!mr.enabled || !GameObjectUtility.AreStaticEditorFlagsSet(mr.gameObject, StaticEditorFlags.LightmapStatic))
                        {
                            return false;
                        }
                        if (<>f__am$cache10 == null)
                        {
                            <>f__am$cache10 = m => (m != null) && (m.globalIlluminationFlags == MaterialGlobalIlluminationFlags.RealtimeEmissive);
                        }
                        return Enumerable.Any<Material>(mr.sharedMaterials, <>f__am$cache10);
                    };
                }
                return Enumerable.Where<MeshRenderer>(Object.FindObjectsOfType<MeshRenderer>(), <>f__am$cache8).Cast<Component>();
            }
            if (<>f__am$cache9 == null)
            {
                <>f__am$cache9 = delegate (MeshRenderer mr) {
                    if (mr.enabled || !GameObjectUtility.AreStaticEditorFlagsSet(mr.gameObject, StaticEditorFlags.LightmapStatic))
                    {
                        return false;
                    }
                    if (<>f__am$cache11 == null)
                    {
                        <>f__am$cache11 = m => (m != null) && (m.globalIlluminationFlags == MaterialGlobalIlluminationFlags.RealtimeEmissive);
                    }
                    return Enumerable.Any<Material>(mr.sharedMaterials, <>f__am$cache11);
                };
            }
            return Enumerable.Where<MeshRenderer>(Object.FindObjectsOfType<MeshRenderer>(), <>f__am$cache9).Cast<Component>();
        }

        internal static IEnumerable<Component> GetReflectionProbes(bool bEnabled)
        {
            if (bEnabled)
            {
                if (<>f__am$cacheE == null)
                {
                    <>f__am$cacheE = rp => rp.enabled;
                }
                return Enumerable.Where<ReflectionProbe>(Object.FindObjectsOfType<ReflectionProbe>(), <>f__am$cacheE).Cast<Component>();
            }
            if (<>f__am$cacheF == null)
            {
                <>f__am$cacheF = rp => !rp.enabled;
            }
            return Enumerable.Where<ReflectionProbe>(Object.FindObjectsOfType<ReflectionProbe>(), <>f__am$cacheF).Cast<Component>();
        }

        internal static IEnumerable<Component> GetStaticLights(bool bEnabled)
        {
            if (bEnabled)
            {
                if (<>f__am$cache2 == null)
                {
                    <>f__am$cache2 = l => l.isActiveAndEnabled && l.isStatic;
                }
                return Enumerable.Where<Light>(Object.FindObjectsOfType<Light>(), <>f__am$cache2).Cast<Component>();
            }
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = l => !l.isActiveAndEnabled && l.isStatic;
            }
            return Enumerable.Where<Light>(Object.FindObjectsOfType<Light>(), <>f__am$cache3).Cast<Component>();
        }

        internal static IEnumerable<Component> GetStaticMeshes(bool bEnabled)
        {
            if (bEnabled)
            {
                if (<>f__am$cache6 == null)
                {
                    <>f__am$cache6 = mr => mr.enabled && GameObjectUtility.AreStaticEditorFlagsSet(mr.gameObject, StaticEditorFlags.LightmapStatic);
                }
                return Enumerable.Where<MeshRenderer>(Object.FindObjectsOfType<MeshRenderer>(), <>f__am$cache6).Cast<Component>();
            }
            if (<>f__am$cache7 == null)
            {
                <>f__am$cache7 = mr => !mr.enabled && GameObjectUtility.AreStaticEditorFlagsSet(mr.gameObject, StaticEditorFlags.LightmapStatic);
            }
            return Enumerable.Where<MeshRenderer>(Object.FindObjectsOfType<MeshRenderer>(), <>f__am$cache7).Cast<Component>();
        }

        private static bool IsAmbientBaked(ref Stats stats) => 
            (stats.ambientMode == 1);

        private static bool IsAmbientDynamic(ref Stats stats) => 
            (stats.ambientMode == 0);

        private static bool IsRealtimeGI(ref Stats stats) => 
            (stats.dynamicMode == 0);

        [Flags]
        internal enum Emitters
        {
            Baked = 0x70,
            BakedAmbient = 0x20,
            BakedEmissive = 0x40,
            BakedLight = 0x10,
            Dynamic = 14,
            DynamicAmbient = 4,
            DynamicEmissive = 8,
            DynamicLight = 2,
            None = 0
        }

        internal delegate IEnumerable<Component> GetComponent(bool bEnabled);

        [Flags]
        internal enum Receivers
        {
            None,
            StaticMesh,
            LightProbe
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct Stats
        {
            public LightModeValidator.Receivers receiverMask;
            public LightModeValidator.Emitters emitterMask;
            public int dynamicMode;
            public int stationaryMode;
            public int staticMode;
            public int ambientMode;
            public bool requiresRealtimeGI;
            public bool requiresLightmaps;
            public LightingStats enabled;
            public LightingStats active;
            public LightingStats inactive;
            public void Reset()
            {
                this.receiverMask = LightModeValidator.Receivers.None;
                this.emitterMask = LightModeValidator.Emitters.None;
                this.dynamicMode = 0;
                this.stationaryMode = 0;
                this.staticMode = 0;
                this.ambientMode = 0;
                this.requiresRealtimeGI = false;
                this.requiresLightmaps = false;
                this.enabled.Reset();
                this.active.Reset();
                this.inactive.Reset();
            }
        }
    }
}

