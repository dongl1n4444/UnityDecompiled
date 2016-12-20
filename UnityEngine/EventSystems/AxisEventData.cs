namespace UnityEngine.EventSystems
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    /// <summary>
    /// <para>Event Data associated with Axis Events (Controller / Keyboard).</para>
    /// </summary>
    public class AxisEventData : BaseEventData
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private MoveDirection <moveDir>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Vector2 <moveVector>k__BackingField;

        public AxisEventData(EventSystem eventSystem) : base(eventSystem)
        {
            this.moveVector = Vector2.zero;
            this.moveDir = MoveDirection.None;
        }

        /// <summary>
        /// <para>MoveDirection for this event.</para>
        /// </summary>
        public MoveDirection moveDir { get; set; }

        /// <summary>
        /// <para>Raw input vector associated with this event.</para>
        /// </summary>
        public Vector2 moveVector { get; set; }
    }
}

