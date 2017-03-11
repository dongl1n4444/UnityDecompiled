namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;

    internal class LightModeValidator
    {
        internal static void AnalyzeScene(int realtimeMode, int mixedMode, int bakedMode, int ambientMode, ref Stats stats)
        {
            stats.Reset();
            stats.realtimeMode = realtimeMode;
            stats.mixedMode = mixedMode;
            stats.bakedMode = bakedMode;
            stats.ambientMode = ambientMode;
            LightmapEditorSettings.AnalyzeLighting(out stats.enabled, out stats.active, out stats.inactive);
            stats.emitterMask = Emitters.None;
            stats.emitterMask |= (stats.enabled.realtimeLightsCount <= 0) ? Emitters.None : Emitters.RealtimeLight;
            stats.emitterMask |= (stats.enabled.staticMeshesRealtimeEmissive <= 0) ? Emitters.None : Emitters.RealtimeEmissive;
            stats.emitterMask |= !IsAmbientRealtime(ref stats) ? Emitters.None : Emitters.RealtimeAmbient;
            stats.emitterMask |= (stats.enabled.bakedLightsCount <= 0) ? Emitters.None : Emitters.BakedLight;
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

        private static bool IsAmbientBaked(ref Stats stats) => 
            (stats.ambientMode == 1);

        private static bool IsAmbientRealtime(ref Stats stats) => 
            (stats.ambientMode == 0);

        private static bool IsRealtimeGI(ref Stats stats) => 
            (stats.realtimeMode == 0);

        [Flags]
        internal enum Emitters
        {
            Baked = 0x70,
            BakedAmbient = 0x20,
            BakedEmissive = 0x40,
            BakedLight = 0x10,
            None = 0,
            Realtime = 14,
            RealtimeAmbient = 4,
            RealtimeEmissive = 8,
            RealtimeLight = 2
        }

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
            public int realtimeMode;
            public int mixedMode;
            public int bakedMode;
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
                this.realtimeMode = 0;
                this.mixedMode = 0;
                this.bakedMode = 0;
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

