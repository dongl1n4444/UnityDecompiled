namespace Unity.IL2CPP.Building.Statistics
{
    using System;

    public interface IBuildStatistics
    {
        int CacheHitPercentage { get; }

        int CacheHits { get; }

        int FilesCompiled { get; }

        int TotalFiles { get; }
    }
}

