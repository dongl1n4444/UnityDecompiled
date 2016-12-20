namespace UnityEditor
{
    using System;

    /// <summary>
    /// <para>Result of Asset delete operation</para>
    /// </summary>
    [Flags]
    public enum AssetDeleteResult
    {
        DidNotDelete,
        FailedDelete,
        DidDelete
    }
}

