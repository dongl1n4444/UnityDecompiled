namespace UnityEditor.VersionControl
{
    using System;

    /// <summary>
    /// <para>The status of an operation returned by the VCS.</para>
    /// </summary>
    public enum SubmitResult
    {
        /// <summary>
        /// <para>Files conflicted.</para>
        /// </summary>
        ConflictingFiles = 4,
        /// <summary>
        /// <para>An error was returned.</para>
        /// </summary>
        Error = 2,
        /// <summary>
        /// <para>Submission worked.</para>
        /// </summary>
        OK = 1,
        /// <summary>
        /// <para>Files were unable to be added.</para>
        /// </summary>
        UnaddedFiles = 8
    }
}

