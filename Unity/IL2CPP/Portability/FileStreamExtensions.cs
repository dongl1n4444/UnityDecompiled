namespace Unity.IL2CPP.Portability
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;

    [Extension]
    public static class FileStreamExtensions
    {
        [Extension]
        public static void LockPortable(FileStream stream, long position, long length)
        {
            stream.Lock(position, length);
        }

        [Extension]
        public static void UnlockPortable(FileStream stream, long position, long length)
        {
            stream.Unlock(position, length);
        }
    }
}

