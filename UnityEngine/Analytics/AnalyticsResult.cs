namespace UnityEngine.Analytics
{
    using System;

    /// <summary>
    /// <para>Analytics API result.</para>
    /// </summary>
    public enum AnalyticsResult
    {
        Ok,
        NotInitialized,
        AnalyticsDisabled,
        TooManyItems,
        SizeLimitReached,
        TooManyRequests,
        InvalidData,
        UnsupportedPlatform
    }
}

