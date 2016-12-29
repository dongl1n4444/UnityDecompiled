namespace UnityEngine.EventSystems
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Text;
    using UnityEngine;

    /// <summary>
    /// <para>Event payload associated with pointer (mouse / touch) events.</para>
    /// </summary>
    public class PointerEventData : BaseEventData
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private InputButton <button>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int <clickCount>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private float <clickTime>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Vector2 <delta>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool <dragging>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool <eligibleForClick>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private GameObject <lastPress>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private RaycastResult <pointerCurrentRaycast>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private GameObject <pointerDrag>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private GameObject <pointerEnter>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int <pointerId>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private RaycastResult <pointerPressRaycast>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Vector2 <position>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Vector2 <pressPosition>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private GameObject <rawPointerPress>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Vector2 <scrollDelta>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool <useDragThreshold>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Vector3 <worldNormal>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Vector3 <worldPosition>k__BackingField;
        /// <summary>
        /// <para>List of objects in the hover stack.</para>
        /// </summary>
        public List<GameObject> hovered;
        private GameObject m_PointerPress;

        public PointerEventData(EventSystem eventSystem) : base(eventSystem)
        {
            this.hovered = new List<GameObject>();
            this.eligibleForClick = false;
            this.pointerId = -1;
            this.position = Vector2.zero;
            this.delta = Vector2.zero;
            this.pressPosition = Vector2.zero;
            this.clickTime = 0f;
            this.clickCount = 0;
            this.scrollDelta = Vector2.zero;
            this.useDragThreshold = true;
            this.dragging = false;
            this.button = InputButton.Left;
        }

        /// <summary>
        /// <para>Is the pointer moving.</para>
        /// </summary>
        /// <returns>
        /// <para>Moving.</para>
        /// </returns>
        public bool IsPointerMoving() => 
            (this.delta.sqrMagnitude > 0f);

        /// <summary>
        /// <para>Is scroll being used on the input device.</para>
        /// </summary>
        /// <returns>
        /// <para>Scrolling.</para>
        /// </returns>
        public bool IsScrolling() => 
            (this.scrollDelta.sqrMagnitude > 0f);

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("<b>Position</b>: " + this.position);
            builder.AppendLine("<b>delta</b>: " + this.delta);
            builder.AppendLine("<b>eligibleForClick</b>: " + this.eligibleForClick);
            builder.AppendLine("<b>pointerEnter</b>: " + this.pointerEnter);
            builder.AppendLine("<b>pointerPress</b>: " + this.pointerPress);
            builder.AppendLine("<b>lastPointerPress</b>: " + this.lastPress);
            builder.AppendLine("<b>pointerDrag</b>: " + this.pointerDrag);
            builder.AppendLine("<b>Use Drag Threshold</b>: " + this.useDragThreshold);
            builder.AppendLine("<b>Current Rayast:</b>");
            builder.AppendLine(this.pointerCurrentRaycast.ToString());
            builder.AppendLine("<b>Press Rayast:</b>");
            builder.AppendLine(this.pointerPressRaycast.ToString());
            return builder.ToString();
        }

        /// <summary>
        /// <para>The EventSystems.PointerEventData.InputButton for this event.</para>
        /// </summary>
        public InputButton button { get; set; }

        /// <summary>
        /// <para>Number of clicks in a row.</para>
        /// </summary>
        public int clickCount { get; set; }

        /// <summary>
        /// <para>The last time a click event was sent.</para>
        /// </summary>
        public float clickTime { get; set; }

        /// <summary>
        /// <para>Pointer delta since last update.</para>
        /// </summary>
        public Vector2 delta { get; set; }

        /// <summary>
        /// <para>Is a drag operation currently occuring.</para>
        /// </summary>
        public bool dragging { get; set; }

        public bool eligibleForClick { get; set; }

        /// <summary>
        /// <para>The camera associated with the last OnPointerEnter event.</para>
        /// </summary>
        public Camera enterEventCamera =>
            this.pointerCurrentRaycast.module?.eventCamera;

        /// <summary>
        /// <para>The GameObject for the last press event.</para>
        /// </summary>
        public GameObject lastPress { get; private set; }

        /// <summary>
        /// <para>RaycastResult associated with the current event.</para>
        /// </summary>
        public RaycastResult pointerCurrentRaycast { get; set; }

        /// <summary>
        /// <para>The object that is receiving 'OnDrag'.</para>
        /// </summary>
        public GameObject pointerDrag { get; set; }

        /// <summary>
        /// <para>The object that received 'OnPointerEnter'.</para>
        /// </summary>
        public GameObject pointerEnter { get; set; }

        /// <summary>
        /// <para>Id of the pointer (touch id).</para>
        /// </summary>
        public int pointerId { get; set; }

        /// <summary>
        /// <para>The GameObject that received the OnPointerDown.</para>
        /// </summary>
        public GameObject pointerPress
        {
            get => 
                this.m_PointerPress;
            set
            {
                if (this.m_PointerPress != value)
                {
                    this.lastPress = this.m_PointerPress;
                    this.m_PointerPress = value;
                }
            }
        }

        /// <summary>
        /// <para>RaycastResult associated with the pointer press.</para>
        /// </summary>
        public RaycastResult pointerPressRaycast { get; set; }

        /// <summary>
        /// <para>Current pointer position.</para>
        /// </summary>
        public Vector2 position { get; set; }

        /// <summary>
        /// <para>The camera associated with the last OnPointerPress event.</para>
        /// </summary>
        public Camera pressEventCamera =>
            this.pointerPressRaycast.module?.eventCamera;

        /// <summary>
        /// <para>Position of the press.</para>
        /// </summary>
        public Vector2 pressPosition { get; set; }

        /// <summary>
        /// <para>The object that the press happened on even if it can not handle the press event.</para>
        /// </summary>
        public GameObject rawPointerPress { get; set; }

        /// <summary>
        /// <para>The amount of scroll since the last update.</para>
        /// </summary>
        public Vector2 scrollDelta { get; set; }

        /// <summary>
        /// <para>Should a drag threshold be used?</para>
        /// </summary>
        public bool useDragThreshold { get; set; }

        [Obsolete("Use either pointerCurrentRaycast.worldNormal or pointerPressRaycast.worldNormal")]
        public Vector3 worldNormal { get; set; }

        [Obsolete("Use either pointerCurrentRaycast.worldPosition or pointerPressRaycast.worldPosition")]
        public Vector3 worldPosition { get; set; }

        /// <summary>
        /// <para>The state of a press for the given frame.</para>
        /// </summary>
        public enum FramePressState
        {
            Pressed,
            Released,
            PressedAndReleased,
            NotChanged
        }

        /// <summary>
        /// <para>Input press tracking.</para>
        /// </summary>
        public enum InputButton
        {
            Left,
            Right,
            Middle
        }
    }
}

