namespace UnityEngine.EventSystems
{
    using System;
    using UnityEngine;
    using UnityEngine.Serialization;

    /// <summary>
    /// <para>A BaseInputModule designed for mouse  keyboard  controller input.</para>
    /// </summary>
    [AddComponentMenu("Event/Standalone Input Module")]
    public class StandaloneInputModule : PointerInputModule
    {
        [SerializeField]
        private string m_CancelButton = "Cancel";
        private int m_ConsecutiveMoveCount = 0;
        [SerializeField, FormerlySerializedAs("m_AllowActivationOnMobileDevice")]
        private bool m_ForceModuleActive;
        [SerializeField]
        private string m_HorizontalAxis = "Horizontal";
        [SerializeField]
        private float m_InputActionsPerSecond = 10f;
        private Vector2 m_LastMousePosition;
        private Vector2 m_LastMoveVector;
        private Vector2 m_MousePosition;
        private float m_PrevActionTime;
        [SerializeField]
        private float m_RepeatDelay = 0.5f;
        [SerializeField]
        private string m_SubmitButton = "Submit";
        [SerializeField]
        private string m_VerticalAxis = "Vertical";

        protected StandaloneInputModule()
        {
        }

        /// <summary>
        /// <para>See BaseInputModule.</para>
        /// </summary>
        public override void ActivateModule()
        {
            base.ActivateModule();
            this.m_MousePosition = base.input.mousePosition;
            this.m_LastMousePosition = base.input.mousePosition;
            GameObject currentSelectedGameObject = base.eventSystem.currentSelectedGameObject;
            if (currentSelectedGameObject == null)
            {
                currentSelectedGameObject = base.eventSystem.firstSelectedGameObject;
            }
            base.eventSystem.SetSelectedGameObject(currentSelectedGameObject, this.GetBaseEventData());
        }

        /// <summary>
        /// <para>See BaseInputModule.</para>
        /// </summary>
        public override void DeactivateModule()
        {
            base.DeactivateModule();
            base.ClearSelection();
        }

        protected virtual bool ForceAutoSelect() => 
            false;

        private Vector2 GetRawMoveVector()
        {
            Vector2 zero = Vector2.zero;
            zero.x = base.input.GetAxisRaw(this.m_HorizontalAxis);
            zero.y = base.input.GetAxisRaw(this.m_VerticalAxis);
            if (base.input.GetButtonDown(this.m_HorizontalAxis))
            {
                if (zero.x < 0f)
                {
                    zero.x = -1f;
                }
                if (zero.x > 0f)
                {
                    zero.x = 1f;
                }
            }
            if (base.input.GetButtonDown(this.m_VerticalAxis))
            {
                if (zero.y < 0f)
                {
                    zero.y = -1f;
                }
                if (zero.y > 0f)
                {
                    zero.y = 1f;
                }
            }
            return zero;
        }

        /// <summary>
        /// <para>See BaseInputModule.</para>
        /// </summary>
        /// <returns>
        /// <para>Supported.</para>
        /// </returns>
        public override bool IsModuleSupported() => 
            ((this.m_ForceModuleActive || base.input.mousePresent) || base.input.touchSupported);

        /// <summary>
        /// <para>See BaseInputModule.</para>
        /// </summary>
        public override void Process()
        {
            bool flag = this.SendUpdateEventToSelectedObject();
            if (base.eventSystem.sendNavigationEvents)
            {
                if (!flag)
                {
                    flag |= this.SendMoveEventToSelectedObject();
                }
                if (!flag)
                {
                    this.SendSubmitEventToSelectedObject();
                }
            }
            if (!this.ProcessTouchEvents() && base.input.mousePresent)
            {
                this.ProcessMouseEvent();
            }
        }

        /// <summary>
        /// <para>Iterate through all the different mouse events.</para>
        /// </summary>
        /// <param name="id">The mouse pointer Event data id to get.</param>
        protected void ProcessMouseEvent()
        {
            this.ProcessMouseEvent(0);
        }

        /// <summary>
        /// <para>Iterate through all the different mouse events.</para>
        /// </summary>
        /// <param name="id">The mouse pointer Event data id to get.</param>
        protected void ProcessMouseEvent(int id)
        {
            PointerInputModule.MouseState mousePointerEventData = this.GetMousePointerEventData(id);
            PointerInputModule.MouseButtonEventData eventData = mousePointerEventData.GetButtonState(PointerEventData.InputButton.Left).eventData;
            if (this.ForceAutoSelect())
            {
                base.eventSystem.SetSelectedGameObject(eventData.buttonData.pointerCurrentRaycast.gameObject, eventData.buttonData);
            }
            this.ProcessMousePress(eventData);
            this.ProcessMove(eventData.buttonData);
            this.ProcessDrag(eventData.buttonData);
            this.ProcessMousePress(mousePointerEventData.GetButtonState(PointerEventData.InputButton.Right).eventData);
            this.ProcessDrag(mousePointerEventData.GetButtonState(PointerEventData.InputButton.Right).eventData.buttonData);
            this.ProcessMousePress(mousePointerEventData.GetButtonState(PointerEventData.InputButton.Middle).eventData);
            this.ProcessDrag(mousePointerEventData.GetButtonState(PointerEventData.InputButton.Middle).eventData.buttonData);
            if (!Mathf.Approximately(eventData.buttonData.scrollDelta.sqrMagnitude, 0f))
            {
                ExecuteEvents.ExecuteHierarchy<IScrollHandler>(ExecuteEvents.GetEventHandler<IScrollHandler>(eventData.buttonData.pointerCurrentRaycast.gameObject), eventData.buttonData, ExecuteEvents.scrollHandler);
            }
        }

        protected void ProcessMousePress(PointerInputModule.MouseButtonEventData data)
        {
            PointerEventData buttonData = data.buttonData;
            GameObject gameObject = buttonData.pointerCurrentRaycast.gameObject;
            if (data.PressedThisFrame())
            {
                buttonData.eligibleForClick = true;
                buttonData.delta = Vector2.zero;
                buttonData.dragging = false;
                buttonData.useDragThreshold = true;
                buttonData.pressPosition = buttonData.position;
                buttonData.pointerPressRaycast = buttonData.pointerCurrentRaycast;
                base.DeselectIfSelectionChanged(gameObject, buttonData);
                GameObject eventHandler = ExecuteEvents.ExecuteHierarchy<IPointerDownHandler>(gameObject, buttonData, ExecuteEvents.pointerDownHandler);
                if (eventHandler == null)
                {
                    eventHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject);
                }
                float unscaledTime = Time.unscaledTime;
                if (eventHandler == buttonData.lastPress)
                {
                    float num2 = unscaledTime - buttonData.clickTime;
                    if (num2 < 0.3f)
                    {
                        buttonData.clickCount++;
                    }
                    else
                    {
                        buttonData.clickCount = 1;
                    }
                    buttonData.clickTime = unscaledTime;
                }
                else
                {
                    buttonData.clickCount = 1;
                }
                buttonData.pointerPress = eventHandler;
                buttonData.rawPointerPress = gameObject;
                buttonData.clickTime = unscaledTime;
                buttonData.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(gameObject);
                if (buttonData.pointerDrag != null)
                {
                    ExecuteEvents.Execute<IInitializePotentialDragHandler>(buttonData.pointerDrag, buttonData, ExecuteEvents.initializePotentialDrag);
                }
            }
            if (data.ReleasedThisFrame())
            {
                ExecuteEvents.Execute<IPointerUpHandler>(buttonData.pointerPress, buttonData, ExecuteEvents.pointerUpHandler);
                GameObject obj4 = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject);
                if ((buttonData.pointerPress == obj4) && buttonData.eligibleForClick)
                {
                    ExecuteEvents.Execute<IPointerClickHandler>(buttonData.pointerPress, buttonData, ExecuteEvents.pointerClickHandler);
                }
                else if ((buttonData.pointerDrag != null) && buttonData.dragging)
                {
                    ExecuteEvents.ExecuteHierarchy<IDropHandler>(gameObject, buttonData, ExecuteEvents.dropHandler);
                }
                buttonData.eligibleForClick = false;
                buttonData.pointerPress = null;
                buttonData.rawPointerPress = null;
                if ((buttonData.pointerDrag != null) && buttonData.dragging)
                {
                    ExecuteEvents.Execute<IEndDragHandler>(buttonData.pointerDrag, buttonData, ExecuteEvents.endDragHandler);
                }
                buttonData.dragging = false;
                buttonData.pointerDrag = null;
                if (gameObject != buttonData.pointerEnter)
                {
                    base.HandlePointerExitAndEnter(buttonData, null);
                    base.HandlePointerExitAndEnter(buttonData, gameObject);
                }
            }
        }

        private bool ProcessTouchEvents()
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
            return (base.input.touchCount > 0);
        }

        /// <summary>
        /// <para>How should the touch press be processed.</para>
        /// </summary>
        /// <param name="pointerEvent">The data to be passed to the final object.</param>
        /// <param name="pressed">If the touch was pressed this frame.</param>
        /// <param name="released">If the touch was released this frame.</param>
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
        /// <para>Calculate and send a move event to the current selected object.</para>
        /// </summary>
        /// <returns>
        /// <para>If the move event was used by the selected object.</para>
        /// </returns>
        protected bool SendMoveEventToSelectedObject()
        {
            float unscaledTime = Time.unscaledTime;
            Vector2 rawMoveVector = this.GetRawMoveVector();
            if (Mathf.Approximately(rawMoveVector.x, 0f) && Mathf.Approximately(rawMoveVector.y, 0f))
            {
                this.m_ConsecutiveMoveCount = 0;
                return false;
            }
            bool flag2 = base.input.GetButtonDown(this.m_HorizontalAxis) || base.input.GetButtonDown(this.m_VerticalAxis);
            bool flag3 = Vector2.Dot(rawMoveVector, this.m_LastMoveVector) > 0f;
            if (!flag2)
            {
                if (flag3 && (this.m_ConsecutiveMoveCount == 1))
                {
                    flag2 = unscaledTime > (this.m_PrevActionTime + this.m_RepeatDelay);
                }
                else
                {
                    flag2 = unscaledTime > (this.m_PrevActionTime + (1f / this.m_InputActionsPerSecond));
                }
            }
            if (!flag2)
            {
                return false;
            }
            AxisEventData eventData = this.GetAxisEventData(rawMoveVector.x, rawMoveVector.y, 0.6f);
            if (eventData.moveDir != MoveDirection.None)
            {
                ExecuteEvents.Execute<IMoveHandler>(base.eventSystem.currentSelectedGameObject, eventData, ExecuteEvents.moveHandler);
                if (!flag3)
                {
                    this.m_ConsecutiveMoveCount = 0;
                }
                this.m_ConsecutiveMoveCount++;
                this.m_PrevActionTime = unscaledTime;
                this.m_LastMoveVector = rawMoveVector;
            }
            else
            {
                this.m_ConsecutiveMoveCount = 0;
            }
            return eventData.used;
        }

        /// <summary>
        /// <para>Calculate and send a submit event to the current selected object.</para>
        /// </summary>
        /// <returns>
        /// <para>If the submit event was used by the selected object.</para>
        /// </returns>
        protected bool SendSubmitEventToSelectedObject()
        {
            if (base.eventSystem.currentSelectedGameObject == null)
            {
                return false;
            }
            BaseEventData baseEventData = this.GetBaseEventData();
            if (base.input.GetButtonDown(this.m_SubmitButton))
            {
                ExecuteEvents.Execute<ISubmitHandler>(base.eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.submitHandler);
            }
            if (base.input.GetButtonDown(this.m_CancelButton))
            {
                ExecuteEvents.Execute<ICancelHandler>(base.eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.cancelHandler);
            }
            return baseEventData.used;
        }

        /// <summary>
        /// <para>Send a update event to the currently selected object.</para>
        /// </summary>
        /// <returns>
        /// <para>If the update event was used by the selected object.</para>
        /// </returns>
        protected bool SendUpdateEventToSelectedObject()
        {
            if (base.eventSystem.currentSelectedGameObject == null)
            {
                return false;
            }
            BaseEventData baseEventData = this.GetBaseEventData();
            ExecuteEvents.Execute<IUpdateSelectedHandler>(base.eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.updateSelectedHandler);
            return baseEventData.used;
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
            bool flag2 = this.m_ForceModuleActive | base.input.GetButtonDown(this.m_SubmitButton);
            flag2 |= base.input.GetButtonDown(this.m_CancelButton);
            flag2 |= !Mathf.Approximately(base.input.GetAxisRaw(this.m_HorizontalAxis), 0f);
            flag2 |= !Mathf.Approximately(base.input.GetAxisRaw(this.m_VerticalAxis), 0f);
            Vector2 vector = this.m_MousePosition - this.m_LastMousePosition;
            flag2 |= vector.sqrMagnitude > 0f;
            flag2 |= base.input.GetMouseButtonDown(0);
            if (base.input.touchCount > 0)
            {
                flag2 = true;
            }
            return flag2;
        }

        /// <summary>
        /// <para>See BaseInputModule.</para>
        /// </summary>
        public override void UpdateModule()
        {
            this.m_LastMousePosition = this.m_MousePosition;
            this.m_MousePosition = base.input.mousePosition;
        }

        /// <summary>
        /// <para>Is this module allowed to be activated if you are on mobile.</para>
        /// </summary>
        [Obsolete("allowActivationOnMobileDevice has been deprecated. Use forceModuleActive instead (UnityUpgradable) -> forceModuleActive")]
        public bool allowActivationOnMobileDevice
        {
            get => 
                this.m_ForceModuleActive;
            set
            {
                this.m_ForceModuleActive = value;
            }
        }

        /// <summary>
        /// <para>Input manager name for the 'cancel' button.</para>
        /// </summary>
        public string cancelButton
        {
            get => 
                this.m_CancelButton;
            set
            {
                this.m_CancelButton = value;
            }
        }

        /// <summary>
        /// <para>Force this module to be active.</para>
        /// </summary>
        public bool forceModuleActive
        {
            get => 
                this.m_ForceModuleActive;
            set
            {
                this.m_ForceModuleActive = value;
            }
        }

        /// <summary>
        /// <para>Input manager name for the horizontal axis button.</para>
        /// </summary>
        public string horizontalAxis
        {
            get => 
                this.m_HorizontalAxis;
            set
            {
                this.m_HorizontalAxis = value;
            }
        }

        /// <summary>
        /// <para>Number of keyboard / controller inputs allowed per second.</para>
        /// </summary>
        public float inputActionsPerSecond
        {
            get => 
                this.m_InputActionsPerSecond;
            set
            {
                this.m_InputActionsPerSecond = value;
            }
        }

        [Obsolete("Mode is no longer needed on input module as it handles both mouse and keyboard simultaneously.", false)]
        public InputMode inputMode =>
            InputMode.Mouse;

        /// <summary>
        /// <para>Delay in seconds before the input actions per second repeat rate takes effect.</para>
        /// </summary>
        public float repeatDelay
        {
            get => 
                this.m_RepeatDelay;
            set
            {
                this.m_RepeatDelay = value;
            }
        }

        /// <summary>
        /// <para>Maximum number of input events handled per second.</para>
        /// </summary>
        public string submitButton
        {
            get => 
                this.m_SubmitButton;
            set
            {
                this.m_SubmitButton = value;
            }
        }

        /// <summary>
        /// <para>Input manager name for the vertical axis.</para>
        /// </summary>
        public string verticalAxis
        {
            get => 
                this.m_VerticalAxis;
            set
            {
                this.m_VerticalAxis = value;
            }
        }

        [Obsolete("Mode is no longer needed on input module as it handles both mouse and keyboard simultaneously.", false)]
        public enum InputMode
        {
            Mouse,
            Buttons
        }
    }
}

