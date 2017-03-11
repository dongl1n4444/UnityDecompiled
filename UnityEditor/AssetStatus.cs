namespace UnityEditor
{
    using System;

    [Obsolete("AssetStatus enum is not used anymore (Asset Server has been removed)")]
    public enum AssetStatus
    {
        BadState = 9,
        Calculating = -1,
        ClientOnly = 0,
        Conflict = 3,
        Ignored = 8,
        NewLocalVersion = 6,
        NewVersionAvailable = 5,
        RestoredFromTrash = 7,
        Same = 4,
        ServerOnly = 1,
        Unchanged = 2
    }
}

