namespace Unity.IL2CPP.Building.Statistics
{
    using System;

    internal interface IBuildStatisticsCollector
    {
        void IncrementCacheHitCountBy(int amount);
        void IncrementTotalFileCountBy(int amount);
    }
}

