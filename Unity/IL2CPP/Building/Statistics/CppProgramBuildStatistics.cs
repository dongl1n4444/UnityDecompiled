namespace Unity.IL2CPP.Building.Statistics
{
    using System;

    public class CppProgramBuildStatistics : IBuildStatistics, IBuildStatisticsCollector
    {
        private int _cacheHits;
        private int _totalFiles;

        public CppProgramBuildStatistics()
        {
        }

        public CppProgramBuildStatistics(int totalFiles, int cacheHits)
        {
            this._totalFiles = totalFiles;
            this._cacheHits = cacheHits;
        }

        void IBuildStatisticsCollector.IncrementCacheHitCountBy(int amount)
        {
            this._cacheHits += amount;
        }

        void IBuildStatisticsCollector.IncrementTotalFileCountBy(int amount)
        {
            this._totalFiles += amount;
        }

        public int CacheHitPercentage =>
            ((this.TotalFiles != 0) ? ((int) ((((double) this.CacheHits) / ((double) this.TotalFiles)) * 100.0)) : 0);

        public int CacheHits =>
            this._cacheHits;

        public int FilesCompiled =>
            (this.TotalFiles - this.CacheHits);

        public int TotalFiles =>
            this._totalFiles;
    }
}

