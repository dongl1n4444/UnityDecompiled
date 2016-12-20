namespace UnityEngine.EventSystems
{
    using System;

    /// <summary>
    /// <para>The type of event the TriggerEvent is intercepting.</para>
    /// </summary>
    public enum EventTriggerType
    {
        PointerEnter,
        PointerExit,
        PointerDown,
        PointerUp,
        PointerClick,
        Drag,
        Drop,
        Scroll,
        UpdateSelected,
        Select,
        Deselect,
        Move,
        InitializePotentialDrag,
        BeginDrag,
        EndDrag,
        Submit,
        Cancel
    }
}

