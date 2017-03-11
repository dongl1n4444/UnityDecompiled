namespace UnityEngine
{
    using System;

    [Flags]
    internal enum AtomicSafetyHandleVersionMask
    {
        Read = 1,
        ReadAndWrite = 3,
        ReadAndWriteInv = -4,
        ReadInv = -2,
        Write = 2,
        WriteInv = -3
    }
}

