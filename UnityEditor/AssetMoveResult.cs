namespace UnityEditor
{
    using System;

    /// <summary>
    /// <para>Result of Asset move</para>
    /// </summary>
    [Flags]
    public enum AssetMoveResult
    {
        DidNotMove,
        FailedMove,
        DidMove
    }
}

