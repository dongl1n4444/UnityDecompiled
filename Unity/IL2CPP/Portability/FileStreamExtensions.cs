namespace Unity.IL2CPP.Portability
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;

    public static class FileStreamExtensions
    {
        public static void LockPortable(this FileStream stream, long position, long length)
        {
            stream.Lock(position, length);
        }

        public static void UnlockPortable(this FileStream stream, long position, long length)
        {
            stream.Unlock(position, length);
        }
    }
}

