namespace UnityEngine.EventSystems
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Text;
    using UnityEngine;

    /// <summary>
    /// <para>A BaseInputModule for pointer input.</para>
    /// </summary>
    public abstract class PointerInputModule : BaseInputModule
    {
        /// <summary>
        /// <para>Touch id for when simulating touches on a non touch device.</para>
        /// </summary>
        public const int kFakeTouchesId = -4;
        /// <summary>
        /// <para>Id of the cached left mouse pointer event.</para>
        /// </summary>
        public const int kMouseLeftId = -1;
        /// <summary>
        /// <para>Id of the cached middle mouse pointer event.</para>
        /// </summary>
        public const int kMouseMiddleId = -3;
        /// <summary>
        /// <para>Id of the cached right mouse pointer event.</para>
        /// </summary>
        public const int kMouseRightId = -2;
        private readonly MouseState m_MouseState = new MouseState();
        protected Dictionary<int, PointerEventData> m_PointerData = new Dictionary<int, PointerEventData>();

        protected PointerInputModule()
        {
        }

        /// <summary>
        /// <para>Clear all pointers and deselect any selected objects in the EventSystem.</para>
        /// </summary>
        protected void ClearSelection()
        {
            BaseEventData baseEventData = this.GetBaseEventData();
            foreach (PointerEventData data2 in this.m_PointerData.Values)
            {
                base.HandlePointerExitAndEnter(data2, null);
            }
            this.m_PointerData.Clear();
            base.eventSystem.SetSelectedGameObject(null, baseEventData);
        }

        /// <summary>
        /// <para>Copy one PointerEventData to another.</para>
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        protected void CopyFromTo(PointerEventData from, PointerEventData to)
        {
            to.position = from.position;
            to.delta = from.delta;
            to.scrollDelta = from.scrollDelta;
            to.pointerCurrentRaycast = from.pointerCurrentRaycast;
            to.pointerEnter = from.pointerEnter;
        }

        /// <summary>
        /// <para>Deselect the current selected GameObject if the currently pointed-at GameObject is different.</para>
        /// </summary>
        /// <param name="currentOverGo">The GameObject the pointer is currently over.</param>
        /// <param name="pointerEvent">Current event data.</param>
        protected void DeselectIfSelectionChanged(GameObject currentOverGo, BaseEventData pointerEvent)
        {
            if (ExecuteEvents.GetEventHandler<ISelectHandler>(currentOverGo) != base.eventSystem.currentSelectedGameObject)
            {
                base.eventSystem.SetSelectedGameObject(null, pointerEvent);
            }
        }

        /// <summary>
        /// <para>Return the last PointerEventData for the given touch / mouse id.</para>
        /// </summary>
        /// <param name="id"></param>
        protected PointerEventData GetLastPointerEventData(int id)
        {
            PointerEventData data;
            this.GetPointerData(id, out data, false);
            return data;
        }

        /// <summary>
        /// <para>Return the current MouseState.</para>
        /// </summary>
        /// <param name="id"></param>
        protected virtual MouseState GetMousePointerEventData() => 
            this.GetMousePointerEventData(0);

        /// <summary>
        /// <para>Return the current MouseState.</para>
        /// </summary>
        /// <param name="id"></param>
        protected virtual MouseState GetMousePointerEventData(int id)
        {
            PointerEventData data;
            PointerEventData data2;
            PointerEventData data3;
            bool flag = this.GetPointerData(-1, out data, true);
            data.Reset();
            if (flag)
            {
                data.position = base.input.mousePosition;
            }
            Vector2 mousePosition = base.input.mousePosition;
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                data.position = new Vector2(-1f, -1f);
                data.delta = Vector2.zero;
            }
            else
            {
                data.delta = mousePosition - data.position;
                data.position = mousePosition;
            }
            data.scrollDelta = base.input.mouseScrollDelta;
            data.button = PointerEventData.InputButton.Left;
            base.eventSystem.RaycastAll(data, base.m_RaycastResultCache);
            RaycastResult result = BaseInputModule.FindFirstRaycast(base.m_RaycastResultCache);
            data.pointerCurrentRaycast = result;
            base.m_RaycastResultCache.Clear();
            this.GetPointerData(-2, out data2, true);
            this.CopyFromTo(data, data2);
            data2.button = PointerEventData.InputButton.Right;
            this.GetPointerData(-3, out data3, true);
            this.CopyFromTo(data, data3);
            data3.button = PointerEventData.InputButton.Middle;
            this.m_MouseState.SetButtonState(PointerEventData.InputButton.Left, this.StateForMouseButton(0), data);
            this.m_MouseState.SetButtonState(PointerEventData.InputButton.Right, this.StateForMouseButton(1), data2);
            this.m_MouseState.SetButtonState(PointerEventData.InputButton.Middle, this.StateForMouseButton(2), data3);
            return this.m_MouseState;
        }

        protected bool GetPointerData(int id, out PointerEventData data, bool create)
        {
            if (!this.m_PointerData.TryGetValue(id, out data) && create)
            {
                PointerEventData data2 = new PointerEventData(base.eventSystem) {
                    pointerId = id
                };
                data = data2;
                this.m_PointerData.Add(id, data);
                return true;
            }
            return false;
        }

        protected PointerEventData GetTouchPointerEventData(Touch input, out bool pressed, out bool released)
        {
            PointerEventData data;
            bool flag = this.GetPointerData(input.fingerId, out data, true);
            data.Reset();
            pressed = flag || (input.phase == TouchPhase.Began);
            released = (input.phase == TouchPhase.Canceled) || (input.phase == TouchPhase.Ended);
            if (flag)
            {
                data.position = input.position;
            }
            if (pressed)
            {
                data.delta = Vector2.zero;
            }
            else
            {
                data.delta = input.position - data.position;
            }
            data.position = input.position;
            data.button = PointerEventData.InputButton.Left;
            base.eventSystem.RaycastAll(data, base.m_RaycastResultCache);
            RaycastResult result = BaseInputModule.FindFirstRaycast(base.m_RaycastResultCache);
            data.pointerCurrentRaycast = result;
            base.m_RaycastResultCache.Clear();
            return data;
        }

        public override bool IsPointerOverGameObject(int pointerId)
        {
            PointerEventData lastPointerEventData = this.GetLastPointerEventData(pointerId);
            return ((lastPointerEventData != null) && (lastPointerEventData.pointerEnter != null));
        }

        /// <summary>
        /// <para>Process the drag for the current frame with the given pointer event.</para>
        /// </summary>
        /// <param name="pointerEvent"></param>
        protected virtual void ProcessDrag(PointerEventData pointerEvent)
        {
            if ((pointerEvent.IsPointerMoving() && (Cursor.lockState != CursorLockMode.Locked)) && (pointerEvent.pointerDrag != null))
            {
                if (!pointerEvent.dragging && ShouldStartDrag(pointerEvent.pressPosition, pointerEvent.position, (float) base.eventSystem.pixelDragThreshold, pointerEvent.useDragThreshold))
                {
                    ExecuteEvents.Execute<IBeginDragHandler>(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.beginDragHandler);
                    pointerEvent.dragging = true;
                }
                if (pointerEvent.dragging)
                {
                    if (pointerEvent.pointerPress != pointerEvent.pointerDrag)
                    {
                        ExecuteEvents.Execute<IPointerUpHandler>(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);
                        pointerEvent.eligibleForClick = false;
                        pointerEvent.pointerPress = null;
                        pointerEvent.rawPointerPress = null;
                    }
                    ExecuteEvents.Execute<IDragHandler>(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.dragHandler);
                }
            }
        }

        /// <summary>
        /// <para>Process movement for the current frame with the given pointer event.</para>
        /// </summary>
        /// <param name="pointerEvent"></param>
        protected virtual void ProcessMove(PointerEventData pointerEvent)
        {
            GameObject newEnterTarget = (Cursor.lockState != CursorLockMode.Locked) ? pointerEvent.pointerCurrentRaycast.gameObject : null;
            base.HandlePointerExitAndEnter(pointerEvent, newEnterTarget);
        }

        /// <summary>
        /// <para>Remove the PointerEventData from the cache.</para>
        /// </summary>
        /// <param name="data"></param>
        protected void RemovePointerData(PointerEventData data)
        {
            this.m_PointerData.Remove(data.pointerId);
        }

        private static bool ShouldStartDrag(Vector2 pressPos, Vector2 currentPos, float threshold, bool useDragThreshold)
        {
            if (!useDragThreshold)
            {
                return true;
            }
            Vector2 vector = pressPos - currentPos;
            return (vector.sqrMagnitude >= (threshold * threshold));
        }

        /// <summary>
        /// <para>Given a mouse button return the current state for the frame.</para>
        /// </summary>
        /// <param name="buttonId">Mouse Button id.</param>
        protected PointerEventData.FramePressState StateForMouseButton(int buttonId)
        {
            bool mouseButtonDown = base.input.GetMouseButtonDown(buttonId);
            bool mouseButtonUp = base.input.GetMouseButtonUp(buttonId);
            if (mouseButtonDown && mouseButtonUp)
            {
                return PointerEventData.FramePressState.PressedAndReleased;
            }
            if (mouseButtonDown)
            {
                return PointerEventData.FramePressState.Pressed;
            }
            if (mouseButtonUp)
            {
                return PointerEventData.FramePressState.Released;
            }
            return PointerEventData.FramePressState.NotChanged;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder("<b>Pointer Input Module of type: </b>" + base.GetType());
            builder.AppendLine();
            foreach (KeyValuePair<int, PointerEventData> pair in this.m_PointerData)
            {
                if (pair.Value != null)
                {
                    builder.AppendLine("<B>Pointer:</b> " + pair.Key);
                    builder.AppendLine(pair.Value.ToString());
                }
            }
            return builder.ToString();
        }

        protected class ButtonState
        {
            private PointerEventData.InputButton m_Button = PointerEventData.InputButton.Left;
            private PointerInputModule.MouseButtonEventData m_EventData;

            public PointerEventData.InputButton button
            {
                get => 
                    this.m_Button;
                set
                {
                    this.m_Button = value;
                }
            }

            public PointerInputModule.MouseButtonEventData eventData
            {
                get => 
                    this.m_EventData;
                set
                {
                    this.m_EventData = value;
                }
            }
        }

        /// <summary>
        /// <para>Information about a mouse button event.</para>
        /// </summary>
        public class MouseButtonEventData
        {
            /// <summary>
            /// <para>Pointer data associated with the mouse event.</para>
            /// </summary>
            public PointerEventData buttonData;
            /// <summary>
            /// <para>The state of the button this frame.</para>
            /// </summary>
            public PointerEventData.FramePressState buttonState;

            /// <summary>
            /// <para>Was the button pressed this frame?</para>
            /// </summary>
            public bool PressedThisFrame() => 
                ((this.buttonState == PointerEventData.FramePressState.Pressed) || (this.buttonState == PointerEventData.FramePressState.PressedAndReleased));

            /// <summary>
            /// <para>Was the button released this frame?</para>
            /// </summary>
            public bool ReleasedThisFrame() => 
                ((this.buttonState == PointerEventData.FramePressState.Released) || (this.buttonState == PointerEventData.FramePressState.PressedAndReleased));
        }

        protected class MouseState
        {
            private List<PointerInputModule.ButtonState> m_TrackedButtons = new List<PointerInputModule.ButtonState>();

            public bool AnyPressesThisFrame()
            {
                for (int i = 0; i < this.m_TrackedButtons.Count; i++)
                {
                    if (this.m_TrackedButtons[i].eventData.PressedThisFrame())
                    {
                        return true;
                    }
                }
                return false;
            }

            public bool AnyReleasesThisFrame()
            {
                for (int i = 0; i < this.m_TrackedButtons.Count; i++)
                {
                    if (this.m_TrackedButtons[i].eventData.ReleasedThisFrame())
                    {
                        return true;
                    }
                }
                return false;
            }

            public PointerInputModule.ButtonState GetButtonState(PointerEventData.InputButton button)
            {
                PointerInputModule.ButtonState item = null;
                for (int i = 0; i < this.m_TrackedButtons.Count; i++)
                {
                    if (this.m_TrackedButtons[i].button == button)
                    {
                        item = this.m_TrackedButtons[i];
                        break;
                    }
                }
                if (item == null)
                {
                    item = new PointerInputModule.ButtonState {
                        button = button,
                        eventData = new PointerInputModule.MouseButtonEventData()
                    };
                    this.m_TrackedButtons.Add(item);
                }
                return item;
            }

            public void SetButtonState(PointerEventData.InputButton button, PointerEventData.FramePressState stateForMouseButton, PointerEventData data)
            {
                PointerInputModule.ButtonState buttonState = this.GetButtonState(button);
                buttonState.eventData.buttonState = stateForMouseButton;
                buttonState.eventData.buttonData = data;
            }
        }
    }
}

