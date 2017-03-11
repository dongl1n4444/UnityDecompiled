namespace UnityEngine
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// <para>Lightmap (and lighting) configuration mode, controls how lightmaps interact with lighting and what kind of information they store.</para>
    /// </summary>
    public enum LightmapsMode
    {
        /// <summary>
        /// <para>Directional information for direct light is combined with directional information for indirect light, encoded as 2 lightmaps.</para>
        /// </summary>
        CombinedDirectional = 1,
        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Enum member LightmapsMode.Directional has been removed. Use CombinedDirectional instead (UnityUpgradable) -> CombinedDirectional", true)]
        Directional = 2,
        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Enum member LightmapsMode.Dual has been removed. Use CombinedDirectional instead (UnityUpgradable) -> CombinedDirectional", true)]
        Dual = 1,
        /// <summary>
        /// <para>Light intensity (no directional information), encoded as 1 lightmap.</para>
        /// </summary>
        NonDirectional = 0,
        /// <summary>
        /// <para>Directional information for direct light is stored separately from directional information for indirect light, encoded as 4 lightmaps.</para>
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Enum member LightmapsMode.SeparateDirectional has been removed. Use CombinedDirectional instead (UnityUpgradable) -> CombinedDirectional", true)]
        SeparateDirectional = 2,
        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Enum member LightmapsMode.Single has been removed. Use NonDirectional instead (UnityUpgradable) -> NonDirectional", true)]
        Single = 0
    }
}

