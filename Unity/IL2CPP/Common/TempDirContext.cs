namespace Unity.IL2CPP.Common
{
    using NiceIO;
    using System;

    public sealed class TempDirContext : IDisposable
    {
        private readonly bool _noClean;
        public readonly NPath Directory;

        internal TempDirContext(NPath directory, bool noClean)
        {
            this.Directory = directory;
            this._noClean = noClean;
        }

        public void Dispose()
        {
            if (!this._noClean && this.Directory.Exists(""))
            {
                TempDir.ForgivingCleanDirectory(this.Directory);
            }
        }
    }
}

