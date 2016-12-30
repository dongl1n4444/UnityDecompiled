namespace UnityEngine
{
    using System;

    [Obsolete("LightmappingMode has been deprecated. Use LightmapBakeType instead (UnityUpgradable) -> LightmapBakeType", true)]
    public enum LightmappingMode
    {
        [Obsolete("LightmappingMode.Baked has been deprecated. Use LightmapBakeType.Static instead (UnityUpgradable) -> LightmapBakeType.Static", true)]
        Baked = 2,
        [Obsolete("LightmappingMode.Mixed has been deprecated. Use LightmapBakeType.Stationary instead (UnityUpgradable) -> LightmapBakeType.Stationary", true)]
        Mixed = 1,
        [Obsolete("LightmappingMode.Realtime has been deprecated. Use LightmapBakeType.Dynamic instead (UnityUpgradable) -> LightmapBakeType.Dynamic", true)]
        Realtime = 4
    }
}

