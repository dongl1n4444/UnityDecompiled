namespace UnityEditor
{
    using System;

    [Obsolete("Use Screen.SetResolution APIs", true)]
    public enum iOSTargetResolution
    {
        Native = 0,
        Resolution320p = 5,
        Resolution640p = 6,
        Resolution768p = 7,
        ResolutionAutoPerformance = 3,
        ResolutionAutoQuality = 4
    }
}

