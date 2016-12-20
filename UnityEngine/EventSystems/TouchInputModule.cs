namespace UnityEngine.EventSystems
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using UnityEngine;
    using UnityEngine.Serialization;

    /// <summary>
    /// <para>Input module used for touch based input.</para>
    /// </summary>
    [Obsolete("TouchInputModule is no longer required as Touch input is now handled in StandaloneInputModule."), AddComponentMenu("Event/Touch Input Module")]
    public class TouchInputModule : PointerInputModule
    {
        [SerializeField, FormerlySerializedAs("m_AllowActivationOnStandalone")]
        private bool m_ForceModuleActive;
        private Vector2 m_LastMousePosition;
        private Vector2 m_MousePosition;

        protected TouchInputModule()
        {
        }

        /// <summary>
        /// <para>See BaseInputModule.</para>
        /// </summary>
        public override void DeactivateModule()
        {
            base.DeactivateModule();
            base.ClearSelection();
        }

        private void FakeTouches()
        {
            PointerInputModule.MouseButtonEventData eventData = this.GetMousePointerEventData(0).GetButtonState(PointerEventData.InputButton.Left).eventData;
            if (eventData.PressedThisFrame())
            {
                eventData.buttonData.delta = Vector2.zero;
            }
            this.ProcessTouchPress(eventData.buttonData, eventData.PressedThisFrame(), eventData.ReleasedThisFrame());
            if (base.input.GetMouseButton(0))
            {
                this.ProcessMove(eventData.buttonData);
                this.ProcessDrag(eventData.buttonData);
            }
        }

        /// <summary>
        /// <para>See BaseInputModule.</para>
        /// </summary>
        /// <returns>
        /// <para>Supported.</para>
        /// </returns>
        public override bool IsModuleSupported()
        {
            return (this.forceModuleActive || base.input.touchSupported);
        }

        /// <summary>
        /// <para>See BaseInputModule.</para>
        /// </summary>
        public override void Process()
        {
            if (this.UseFakeInput())
            {
                this.FakeTouches();
            }
            else
            {
                this.ProcessTouchEvents();
            }
        }

        private void ProcessTouchEvents()
        {
            for (int i = 0; i < base.input.touchCount; i++)
            {
                Touch input = base.input.GetTouch(i);
                if (input.type != TouchType.Indirect)
                {
                    bool flag;
                    bool flag2;
                    PointerEventData pointerEvent = base.GetTouchPointerEventData(input, out flag2, out flag);
                    this.ProcessTouchPress(pointerEvent, flag2, flag);
                    if (!flag)
                    {
                        this.ProcessMove(pointerEvent);
                        this.ProcessDrag(pointerEvent);
                    }
                    else
                    {
                        base.RemovePointerData(pointerEvent);
                    }
                }
            }
        }

        protected void ProcessTouchPress(PointerEventData pointerEvent, bool pressed, bool released)
        {
            GameObject gameObject = pointerEvent.pointerCurrentRaycast.gameObject;
            if (pressed)
            {
                pointerEvent.eligibleForClick = true;
                pointerEvent.delta = Vector2.zero;
                pointerEvent.dragging = false;
                pointerEvent.useDragThreshold = true;
                pointerEvent.pressPosition = pointerEvent.position;
                pointerEvent.pointerPressRaycast = pointerEvent.pointerCurrentRaycast;
                base.DeselectIfSelectionChanged(gameObject, pointerEvent);
                if (pointerEvent.pointerEnter != gameObject)
                {
                    base.HandlePointerExitAndEnter(pointerEvent, gameObject);
                    pointerEvent.pointerEnter = gameObject;
                }
                GameObject eventHandler = ExecuteEvents.ExecuteHierarchy<IPointerDownHandler>(gameObject, pointerEvent, ExecuteEvents.pointerDownHandler);
                if (eventHandler == null)
                {
                    eventHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject);
                }
                float unscaledTime = Time.unscaledTime;
                if (eventHandler == pointerEvent.lastPress)
                {
                    float num2 = unscaledTime - pointerEvent.clickTime;
                    if (num2 < 0.3f)
                    {
                        pointerEvent.clickCount++;
                    }
                    else
                    {
                        pointerEvent.clickCount = 1;
                    }
                    pointerEvent.clickTime = unscaledTime;
                }
                else
                {
                    pointerEvent.clickCount = 1;
                }
                pointerEvent.pointerPress = eventHandler;
                pointerEvent.rawPointerPress = gameObject;
                pointerEvent.clickTime = unscaledTime;
                pointerEvent.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(gameObject);
                if (pointerEvent.pointerDrag != null)
                {
                    ExecuteEvents.Execute<IInitializePotentialDragHandler>(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.initializePotentialDrag);
                }
            }
            if (released)
            {
                ExecuteEvents.Execute<IPointerUpHandler>(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);
                GameObject obj4 = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject);
                if ((pointerEvent.pointerPress == obj4) && pointerEvent.eligibleForClick)
                {
                    ExecuteEvents.Execute<IPointerClickHandler>(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerClickHandler);
                }
                else if ((pointerEvent.pointerDrag != null) && pointerEvent.dragging)
                {
                    ExecuteEvents.ExecuteHierarchy<IDropHandler>(gameObject, pointerEvent, ExecuteEvents.dropHandler);
                }
                pointerEvent.eligibleForClick = false;
                pointerEvent.pointerPress = null;
                pointerEvent.rawPointerPress = null;
                if ((pointerEvent.pointerDrag != null) && pointerEvent.dragging)
                {
                    ExecuteEvents.Execute<IEndDragHandler>(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);
                }
                pointerEvent.dragging = false;
                pointerEvent.pointerDrag = null;
                if (pointerEvent.pointerDrag != null)
                {
                    ExecuteEvents.Execute<IEndDragHandler>(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);
                }
                pointerEvent.pointerDrag = null;
                ExecuteEvents.ExecuteHierarchy<IPointerExitHandler>(pointerEvent.pointerEnter, pointerEvent, ExecuteEvents.pointerExitHandler);
                pointerEvent.pointerEnter = null;
            }
        }

        /// <summary>
        /// <para>See BaseInputModule.</para>
        /// </summary>
        /// <returns>
        /// <para>Should activate.</para>
        /// </returns>
        public override bool ShouldActivateModule()
        {
            if (!base.ShouldActivateModule())
            {
                return false;
            }
            if (this.m_ForceModuleActive)
            {
                return true;
            }
            if (this.UseFakeInput())
            {
                Vector2 vector = this.m_MousePosition - this.m_LastMousePosition;
                return (base.input.GetMouseButtonDown(0) | (vector.sqrMagnitude > 0f));
            }
            return (base.input.touchCount > 0);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(!this.UseFakeInput() ? "Input: Touch" : "Input: Faked");
            if (this.UseFakeInput())
            {
                PointerEventData lastPointerEventData = base.GetLastPointerEventData(-1);
                if (lastPointerEventData != null)
                {
                    builder.AppendLine(lastPointerEventData.ToString());
                }
            }
            else
            {
                foreach (KeyValuePair<int, PointerEventData> pair in base.m_PointerData)
                {
                    builder.AppendLine(pair.ToString());
                }
            }
            return builder.ToString();
        }

        /// <summary>
        /// <para>See BaseInputModule.</para>
        /// </summary>
        public override void UpdateModule()
        {
            this.m_LastMousePosition = this.m_MousePosition;
            this.m_MousePosition = base.input.mousePosition;
        }

        private bool UseFakeInput()
        {
            return !base.input.touchSupported;
        }

        /// <summary>
        /// <para>Can this module be activated on a standalone platform?</para>
        /// </summary>
        [Obsolete("allowActivationOnStandalone has been deprecated. Use forceModuleActive instead (UnityUpgradable) -> forceModuleActive")]
        public bool allowActivationOnStandalone
        {
            get
            {
                return this.m_ForceModuleActive;
            }
            set
            {
                this.m_ForceModuleActive = value;
            }
        }

        /// <summary>
        /// <para>Force this module to be active.</para>
        /// </summary>
        public bool forceModuleActive
        {
            get
            {
                return this.m_ForceModuleActive;
            }
            set
            {
                this.m_ForceModuleActive = value;
            }
        }
    }
}

