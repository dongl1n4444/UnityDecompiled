namespace UnityEditor.VersionControl
{
    using System;

    /// <summary>
    /// <para>How assets should be resolved.</para>
    /// </summary>
    [Flags]
    public enum ResolveMethod
    {
        /// <summary>
        /// <para>Use merged version.</para>
        /// </summary>
        UseMerged = 3,
        /// <summary>
        /// <para>Use "mine" (local version).</para>
        /// </summary>
        UseMine = 1,
        /// <summary>
        /// <para>Use "theirs" (other/remote version).</para>
        /// </summary>
        UseTheirs = 2
    }
}

