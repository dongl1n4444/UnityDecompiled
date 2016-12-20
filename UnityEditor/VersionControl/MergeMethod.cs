namespace UnityEditor.VersionControl
{
    using System;

    /// <summary>
    /// <para>Which method to use when merging.</para>
    /// </summary>
    [Flags]
    public enum MergeMethod
    {
        /// <summary>
        /// <para>Merge all changes.</para>
        /// </summary>
        MergeAll = 1,
        /// <summary>
        /// <para>Merge non-conflicting changes.</para>
        /// </summary>
        [Obsolete("This member is no longer supported (UnityUpgradable) -> MergeNone", true)]
        MergeNonConflicting = 2,
        /// <summary>
        /// <para>Don't merge any changes.</para>
        /// </summary>
        MergeNone = 0
    }
}

