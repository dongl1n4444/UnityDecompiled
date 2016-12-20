namespace UnityEngine.Events
{
    using System;

    /// <summary>
    /// <para>Controls the scope of UnityEvent callbacks.</para>
    /// </summary>
    public enum UnityEventCallState
    {
        Off,
        EditorAndRuntime,
        RuntimeOnly
    }
}

