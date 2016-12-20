namespace Unity.IL2CPP.Common
{
    using NiceIO;
    using System;
    using System.IO;

    public sealed class TempFile : IDisposable
    {
        public readonly NPath Path;

        private TempFile(NPath path)
        {
            this.Path = path;
        }

        public static TempFile CreateRandom()
        {
            string[] append = new string[] { System.IO.Path.GetRandomFileName() };
            NPath path = TempDir.Il2CppTemporaryDirectoryRoot.Combine(append);
            while (path.Exists(""))
            {
                string[] textArray2 = new string[] { System.IO.Path.GetRandomFileName() };
                path = TempDir.Il2CppTemporaryDirectoryRoot.Combine(textArray2);
            }
            return new TempFile(path);
        }

        public void Dispose()
        {
            if (this.Path.Exists(""))
            {
                this.Path.Delete(DeleteMode.Normal);
            }
        }
    }
}

