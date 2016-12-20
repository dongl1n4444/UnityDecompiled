namespace UnityEngine.Events
{
    using System;

    /// <summary>
    /// <para>THe mode that a listener is operating in.</para>
    /// </summary>
    [Serializable]
    public enum PersistentListenerMode
    {
        EventDefined,
        Void,
        Object,
        Int,
        Float,
        String,
        Bool
    }
}

