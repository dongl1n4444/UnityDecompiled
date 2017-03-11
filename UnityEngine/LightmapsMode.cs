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
        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Enum member LightmapsMode.Directional has been deprecated. Use CombinedDirectional instead (UnityUpgradable) -> CombinedDirectional", true)]
        Directional = 2,
        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Enum member LightmapsMode.Dual has been deprecated. Use CombinedDirectional instead (UnityUpgradable) -> CombinedDirectional", true)]
        Dual = 1,
        /// <summary>
        /// <para>Light intensity (no directional information), encoded as 1 lightmap.</para>
        /// </summary>
        NonDirectional = 0,
        /// <summary>
        /// <para>Deprecated in Unity 5.5, please use CombinedDirectional instead. Directional information for direct light is stored separately from directional information for indirect light, encoded as 4 lightmaps. </para>
        /// </summary>
        [Obsolete("Enum member LightmapsMode.SeparateDirectional has been deprecated and will be removed in a future version. Use CombinedDirectional instead (UnityUpgradable) -> CombinedDirectional", false), EditorBrowsable(EditorBrowsableState.Never)]
        SeparateDirectional = 2,
        [Obsolete("Enum member LightmapsMode.Single has been deprecated. Use NonDirectional instead (UnityUpgradable) -> NonDirectional", true), EditorBrowsable(EditorBrowsableState.Never)]
        Single = 0
    }
}

