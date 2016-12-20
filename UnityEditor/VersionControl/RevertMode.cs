namespace UnityEditor.VersionControl
{
    using System;

    /// <summary>
    /// <para>Defines the behaviour of the version control revert methods.</para>
    /// </summary>
    [Flags]
    public enum RevertMode
    {
        Normal,
        Unchanged,
        KeepModifications
    }
}

