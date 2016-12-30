namespace UnityEditor.Rendering
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Editor-specific script interface for.</para>
    /// </summary>
    public sealed class EditorGraphicsSettings
    {
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern bool AreTierSettingsAutomatic(BuildTargetGroup target, GraphicsTier tier);
        internal static TierSettings GetCurrentTierSettings() => 
            GetCurrentTierSettingsImpl();

        internal static TierSettings GetCurrentTierSettingsImpl()
        {
            TierSettings settings;
            INTERNAL_CALL_GetCurrentTierSettingsImpl(out settings);
            return settings;
        }

        /// <summary>
        /// <para>Will return PlatformShaderSettings for given platform and shader hardware tier.</para>
        /// </summary>
        /// <param name="target"></param>
        /// <param name="tier"></param>
        [Obsolete("Use GetTierSettings() instead (UnityUpgradable) -> GetTierSettings(*)", false)]
        public static PlatformShaderSettings GetShaderSettingsForPlatform(BuildTargetGroup target, ShaderHardwareTier tier)
        {
            TierSettings tierSettings = GetTierSettings(target, (GraphicsTier) tier);
            return new PlatformShaderSettings { 
                cascadedShadowMaps = tierSettings.cascadedShadowMaps,
                standardShaderQuality = tierSettings.standardShaderQuality,
                reflectionProbeBoxProjection = tierSettings.reflectionProbeBoxProjection,
                reflectionProbeBlending = tierSettings.reflectionProbeBlending
            };
        }

        public static TierSettings GetTierSettings(BuildTargetGroup target, GraphicsTier tier) => 
            GetTierSettingsImpl(target, tier);

        /// <summary>
        /// <para>Will return TierSettings for given platform and shader hardware tier.</para>
        /// </summary>
        /// <param name="target"></param>
        /// <param name="tier"></param>
        [Obsolete("Use GraphicsTier instead of ShaderHardwareTier enum", false)]
        public static TierSettings GetTierSettings(BuildTargetGroup target, ShaderHardwareTier tier) => 
            GetTierSettings(target, (GraphicsTier) tier);

        internal static TierSettings GetTierSettingsImpl(BuildTargetGroup target, GraphicsTier tier)
        {
            TierSettings settings;
            INTERNAL_CALL_GetTierSettingsImpl(target, tier, out settings);
            return settings;
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetCurrentTierSettingsImpl(out TierSettings value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetTierSettingsImpl(BuildTargetGroup target, GraphicsTier tier, out TierSettings value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_SetTierSettingsImpl(BuildTargetGroup target, GraphicsTier tier, ref TierSettings settings);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void MakeTierSettingsAutomatic(BuildTargetGroup target, GraphicsTier tier, bool automatic);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void OnUpdateTierSettingsImpl(BuildTargetGroup target, bool shouldReloadShaders);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void RegisterUndoForGraphicsSettings();
        /// <summary>
        /// <para>Allows you to set the PlatformShaderSettings for the specified platform and shader hardware tier.</para>
        /// </summary>
        /// <param name="target"></param>
        /// <param name="tier"></param>
        /// <param name="settings"></param>
        [Obsolete("Use SetTierSettings() instead (UnityUpgradable) -> SetTierSettings(*)", false)]
        public static void SetShaderSettingsForPlatform(BuildTargetGroup target, ShaderHardwareTier tier, PlatformShaderSettings settings)
        {
            TierSettings tierSettings = GetTierSettings(target, (GraphicsTier) tier);
            tierSettings.standardShaderQuality = settings.standardShaderQuality;
            tierSettings.cascadedShadowMaps = settings.cascadedShadowMaps;
            tierSettings.reflectionProbeBoxProjection = settings.reflectionProbeBoxProjection;
            tierSettings.reflectionProbeBlending = settings.reflectionProbeBlending;
            SetTierSettings(target, (GraphicsTier) tier, tierSettings);
        }

        public static void SetTierSettings(BuildTargetGroup target, GraphicsTier tier, TierSettings settings)
        {
            if (settings.renderingPath == RenderingPath.UsePlayerSettings)
            {
                throw new ArgumentException("TierSettings.renderingPath must be actual rendering path (not UsePlayerSettings)", "settings");
            }
            SetTierSettingsImpl(target, tier, settings);
            MakeTierSettingsAutomatic(target, tier, false);
            OnUpdateTierSettingsImpl(target, true);
        }

        /// <summary>
        /// <para>Allows you to set the PlatformShaderSettings for the specified platform and shader hardware tier.</para>
        /// </summary>
        /// <param name="target"></param>
        /// <param name="tier"></param>
        /// <param name="settings"></param>
        [Obsolete("Use GraphicsTier instead of ShaderHardwareTier enum", false)]
        public static void SetTierSettings(BuildTargetGroup target, ShaderHardwareTier tier, TierSettings settings)
        {
            SetTierSettings(target, (GraphicsTier) tier, settings);
        }

        internal static void SetTierSettingsImpl(BuildTargetGroup target, GraphicsTier tier, TierSettings settings)
        {
            INTERNAL_CALL_SetTierSettingsImpl(target, tier, ref settings);
        }

        /// <summary>
        /// <para>Will return an array of Rendering.AlbedoSwatchInfo.</para>
        /// </summary>
        public static AlbedoSwatchInfo[] albedoSwatches { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}

