namespace UnityEngine.UI
{
    using System;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;

    /// <summary>
    /// <para>A standard slider that can be moved between a minimum and maximum value.</para>
    /// </summary>
    [AddComponentMenu("UI/Slider", 0x21), RequireComponent(typeof(RectTransform))]
    public class Slider : Selectable, IDragHandler, IInitializePotentialDragHandler, ICanvasElement, IEventSystemHandler
    {
        [Space, SerializeField]
        private Direction m_Direction = Direction.LeftToRight;
        private RectTransform m_FillContainerRect;
        private Image m_FillImage;
        [SerializeField]
        private RectTransform m_FillRect;
        private Transform m_FillTransform;
        private RectTransform m_HandleContainerRect;
        [SerializeField]
        private RectTransform m_HandleRect;
        private Transform m_HandleTransform;
        [SerializeField]
        private float m_MaxValue = 1f;
        [SerializeField]
        private float m_MinValue = 0f;
        private Vector2 m_Offset = Vector2.zero;
        [Space, SerializeField]
        private SliderEvent m_OnValueChanged = new SliderEvent();
        private DrivenRectTransformTracker m_Tracker;
        [SerializeField]
        protected float m_Value;
        [SerializeField]
        private bool m_WholeNumbers = false;

        protected Slider()
        {
        }

        private float ClampValue(float input)
        {
            float f = Mathf.Clamp(input, this.minValue, this.maxValue);
            if (this.wholeNumbers)
            {
                f = Mathf.Round(f);
            }
            return f;
        }

        /// <summary>
        /// <para>See member in base class.</para>
        /// </summary>
        public override Selectable FindSelectableOnDown()
        {
            if ((base.navigation.mode == Navigation.Mode.Automatic) && (this.axis == Axis.Vertical))
            {
                return null;
            }
            return base.FindSelectableOnDown();
        }

        /// <summary>
        /// <para>See member in base class.</para>
        /// </summary>
        public override Selectable FindSelectableOnLeft()
        {
            if ((base.navigation.mode == Navigation.Mode.Automatic) && (this.axis == Axis.Horizontal))
            {
                return null;
            }
            return base.FindSelectableOnLeft();
        }

        /// <summary>
        /// <para>See member in base class.</para>
        /// </summary>
        public override Selectable FindSelectableOnRight()
        {
            if ((base.navigation.mode == Navigation.Mode.Automatic) && (this.axis == Axis.Horizontal))
            {
                return null;
            }
            return base.FindSelectableOnRight();
        }

        /// <summary>
        /// <para>See member in base class.</para>
        /// </summary>
        public override Selectable FindSelectableOnUp()
        {
            if ((base.navigation.mode == Navigation.Mode.Automatic) && (this.axis == Axis.Vertical))
            {
                return null;
            }
            return base.FindSelectableOnUp();
        }

        /// <summary>
        /// <para>See ICanvasElement.GraphicUpdateComplete.</para>
        /// </summary>
        public virtual void GraphicUpdateComplete()
        {
        }

        /// <summary>
        /// <para>See ICanvasElement.LayoutComplete.</para>
        /// </summary>
        public virtual void LayoutComplete()
        {
        }

        private bool MayDrag(PointerEventData eventData) => 
            ((this.IsActive() && this.IsInteractable()) && (eventData.button == PointerEventData.InputButton.Left));

        protected override void OnDidApplyAnimationProperties()
        {
            this.m_Value = this.ClampValue(this.m_Value);
            float normalizedValue = this.normalizedValue;
            if (this.m_FillContainerRect != null)
            {
                if ((this.m_FillImage != null) && (this.m_FillImage.type == Image.Type.Filled))
                {
                    normalizedValue = this.m_FillImage.fillAmount;
                }
                else
                {
                    normalizedValue = !this.reverseValue ? this.m_FillRect.anchorMax[(int) this.axis] : (1f - this.m_FillRect.anchorMin[(int) this.axis]);
                }
            }
            else if (this.m_HandleContainerRect != null)
            {
                normalizedValue = !this.reverseValue ? this.m_HandleRect.anchorMin[(int) this.axis] : (1f - this.m_HandleRect.anchorMin[(int) this.axis]);
            }
            this.UpdateVisuals();
            if (normalizedValue != this.normalizedValue)
            {
                UISystemProfilerApi.AddMarker("Slider.value", this);
                this.onValueChanged.Invoke(this.m_Value);
            }
        }

        /// <summary>
        /// <para>See MonoBehaviour.OnDisable.</para>
        /// </summary>
        protected override void OnDisable()
        {
            this.m_Tracker.Clear();
            base.OnDisable();
        }

        /// <summary>
        /// <para>Handling for when the slider is dragged.</para>
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnDrag(PointerEventData eventData)
        {
            if (this.MayDrag(eventData))
            {
                this.UpdateDrag(eventData, eventData.pressEventCamera);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.UpdateCachedReferences();
            this.Set(this.m_Value, false);
            this.UpdateVisuals();
        }

        /// <summary>
        /// <para>See: IInitializePotentialDragHandler.OnInitializePotentialDrag.</para>
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnInitializePotentialDrag(PointerEventData eventData)
        {
            eventData.useDragThreshold = false;
        }

        /// <summary>
        /// <para>Handling for movement events.</para>
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnMove(AxisEventData eventData)
        {
            if (!this.IsActive() || !this.IsInteractable())
            {
                base.OnMove(eventData);
            }
            else
            {
                switch (eventData.moveDir)
                {
                    case MoveDirection.Left:
                        if ((this.axis != Axis.Horizontal) || (this.FindSelectableOnLeft() != null))
                        {
                            base.OnMove(eventData);
                            break;
                        }
                        this.Set(!this.reverseValue ? (this.value - this.stepSize) : (this.value + this.stepSize));
                        break;

                    case MoveDirection.Up:
                        if ((this.axis != Axis.Vertical) || (this.FindSelectableOnUp() != null))
                        {
                            base.OnMove(eventData);
                            break;
                        }
                        this.Set(!this.reverseValue ? (this.value + this.stepSize) : (this.value - this.stepSize));
                        break;

                    case MoveDirection.Right:
                        if ((this.axis != Axis.Horizontal) || (this.FindSelectableOnRight() != null))
                        {
                            base.OnMove(eventData);
                            break;
                        }
                        this.Set(!this.reverseValue ? (this.value + this.stepSize) : (this.value - this.stepSize));
                        break;

                    case MoveDirection.Down:
                        if ((this.axis != Axis.Vertical) || (this.FindSelectableOnDown() != null))
                        {
                            base.OnMove(eventData);
                            break;
                        }
                        this.Set(!this.reverseValue ? (this.value - this.stepSize) : (this.value + this.stepSize));
                        break;
                }
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (this.MayDrag(eventData))
            {
                base.OnPointerDown(eventData);
                this.m_Offset = Vector2.zero;
                if ((this.m_HandleContainerRect != null) && RectTransformUtility.RectangleContainsScreenPoint(this.m_HandleRect, eventData.position, eventData.enterEventCamera))
                {
                    Vector2 vector;
                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(this.m_HandleRect, eventData.position, eventData.pressEventCamera, out vector))
                    {
                        this.m_Offset = vector;
                    }
                }
                else
                {
                    this.UpdateDrag(eventData, eventData.pressEventCamera);
                }
            }
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            if (this.IsActive())
            {
                this.UpdateVisuals();
            }
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            if (this.wholeNumbers)
            {
                this.m_MinValue = Mathf.Round(this.m_MinValue);
                this.m_MaxValue = Mathf.Round(this.m_MaxValue);
            }
            if (this.IsActive())
            {
                this.UpdateCachedReferences();
                this.Set(this.m_Value, false);
                this.UpdateVisuals();
            }
            if ((PrefabUtility.GetPrefabType(this) != PrefabType.Prefab) && !Application.isPlaying)
            {
                CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
            }
        }

        /// <summary>
        /// <para>Handling for when the canvas is rebuilt.</para>
        /// </summary>
        /// <param name="executing"></param>
        public virtual void Rebuild(CanvasUpdate executing)
        {
            if (executing == CanvasUpdate.Prelayout)
            {
                this.onValueChanged.Invoke(this.value);
            }
        }

        private void Set(float input)
        {
            this.Set(input, true);
        }

        /// <summary>
        /// <para>Set the value of the slider.</para>
        /// </summary>
        /// <param name="input">The new value for the slider.</param>
        /// <param name="sendCallback">If the OnValueChanged callback should be invoked.</param>
        protected virtual void Set(float input, bool sendCallback)
        {
            float num = this.ClampValue(input);
            if (this.m_Value != num)
            {
                this.m_Value = num;
                this.UpdateVisuals();
                if (sendCallback)
                {
                    UISystemProfilerApi.AddMarker("Slider.value", this);
                    this.m_OnValueChanged.Invoke(num);
                }
            }
        }

        public void SetDirection(Direction direction, bool includeRectLayouts)
        {
            Axis axis = this.axis;
            bool reverseValue = this.reverseValue;
            this.direction = direction;
            if (includeRectLayouts)
            {
                if (this.axis != axis)
                {
                    RectTransformUtility.FlipLayoutAxes(base.transform as RectTransform, true, true);
                }
                if (this.reverseValue != reverseValue)
                {
                    RectTransformUtility.FlipLayoutOnAxis(base.transform as RectTransform, (int) this.axis, true, true);
                }
            }
        }

        Transform ICanvasElement.get_transform() => 
            base.transform;

        private void UpdateCachedReferences()
        {
            if (this.m_FillRect != null)
            {
                this.m_FillTransform = this.m_FillRect.transform;
                this.m_FillImage = this.m_FillRect.GetComponent<Image>();
                if (this.m_FillTransform.parent != null)
                {
                    this.m_FillContainerRect = this.m_FillTransform.parent.GetComponent<RectTransform>();
                }
            }
            else
            {
                this.m_FillContainerRect = null;
                this.m_FillImage = null;
            }
            if (this.m_HandleRect != null)
            {
                this.m_HandleTransform = this.m_HandleRect.transform;
                if (this.m_HandleTransform.parent != null)
                {
                    this.m_HandleContainerRect = this.m_HandleTransform.parent.GetComponent<RectTransform>();
                }
            }
            else
            {
                this.m_HandleContainerRect = null;
            }
        }

        private void UpdateDrag(PointerEventData eventData, Camera cam)
        {
            RectTransform handleContainerRect;
            Vector2 vector2;
            if (this.m_HandleContainerRect != null)
            {
                handleContainerRect = this.m_HandleContainerRect;
            }
            else
            {
                handleContainerRect = this.m_FillContainerRect;
            }
            if (((handleContainerRect != null) && (handleContainerRect.rect.size[(int) this.axis] > 0f)) && RectTransformUtility.ScreenPointToLocalPointInRectangle(handleContainerRect, eventData.position, cam, out vector2))
            {
                vector2 -= handleContainerRect.rect.position;
                Vector2 vector3 = vector2 - this.m_Offset;
                float num = Mathf.Clamp01(vector3[(int) this.axis] / handleContainerRect.rect.size[(int) this.axis]);
                this.normalizedValue = !this.reverseValue ? num : (1f - num);
            }
        }

        private void UpdateVisuals()
        {
            if (!Application.isPlaying)
            {
                this.UpdateCachedReferences();
            }
            this.m_Tracker.Clear();
            if (this.m_FillContainerRect != null)
            {
                this.m_Tracker.Add(this, this.m_FillRect, DrivenTransformProperties.Anchors);
                Vector2 zero = Vector2.zero;
                Vector2 one = Vector2.one;
                if ((this.m_FillImage != null) && (this.m_FillImage.type == Image.Type.Filled))
                {
                    this.m_FillImage.fillAmount = this.normalizedValue;
                }
                else if (this.reverseValue)
                {
                    zero[(int) this.axis] = 1f - this.normalizedValue;
                }
                else
                {
                    one[(int) this.axis] = this.normalizedValue;
                }
                this.m_FillRect.anchorMin = zero;
                this.m_FillRect.anchorMax = one;
            }
            if (this.m_HandleContainerRect != null)
            {
                this.m_Tracker.Add(this, this.m_HandleRect, DrivenTransformProperties.Anchors);
                Vector2 vector3 = Vector2.zero;
                Vector2 vector4 = Vector2.one;
                float num = !this.reverseValue ? this.normalizedValue : (1f - this.normalizedValue);
                vector4[(int) this.axis] = num;
                vector3[(int) this.axis] = num;
                this.m_HandleRect.anchorMin = vector3;
                this.m_HandleRect.anchorMax = vector4;
            }
        }

        private Axis axis =>
            (((this.m_Direction != Direction.LeftToRight) && (this.m_Direction != Direction.RightToLeft)) ? Axis.Vertical : Axis.Horizontal);

        /// <summary>
        /// <para>The direction of the slider, from minimum to maximum value.</para>
        /// </summary>
        public Direction direction
        {
            get => 
                this.m_Direction;
            set
            {
                if (SetPropertyUtility.SetStruct<Direction>(ref this.m_Direction, value))
                {
                    this.UpdateVisuals();
                }
            }
        }

        /// <summary>
        /// <para>Optional RectTransform to use as fill for the slider.</para>
        /// </summary>
        public RectTransform fillRect
        {
            get => 
                this.m_FillRect;
            set
            {
                if (SetPropertyUtility.SetClass<RectTransform>(ref this.m_FillRect, value))
                {
                    this.UpdateCachedReferences();
                    this.UpdateVisuals();
                }
            }
        }

        /// <summary>
        /// <para>Optional RectTransform to use as a handle for the slider.</para>
        /// </summary>
        public RectTransform handleRect
        {
            get => 
                this.m_HandleRect;
            set
            {
                if (SetPropertyUtility.SetClass<RectTransform>(ref this.m_HandleRect, value))
                {
                    this.UpdateCachedReferences();
                    this.UpdateVisuals();
                }
            }
        }

        /// <summary>
        /// <para>The maximum allowed value of the slider.</para>
        /// </summary>
        public float maxValue
        {
            get => 
                this.m_MaxValue;
            set
            {
                if (SetPropertyUtility.SetStruct<float>(ref this.m_MaxValue, value))
                {
                    this.Set(this.m_Value);
                    this.UpdateVisuals();
                }
            }
        }

        /// <summary>
        /// <para>The minimum allowed value of the slider.</para>
        /// </summary>
        public float minValue
        {
            get => 
                this.m_MinValue;
            set
            {
                if (SetPropertyUtility.SetStruct<float>(ref this.m_MinValue, value))
                {
                    this.Set(this.m_Value);
                    this.UpdateVisuals();
                }
            }
        }

        /// <summary>
        /// <para>The current value of the slider normalized into a value between 0 and 1.</para>
        /// </summary>
        public float normalizedValue
        {
            get
            {
                if (Mathf.Approximately(this.minValue, this.maxValue))
                {
                    return 0f;
                }
                return Mathf.InverseLerp(this.minValue, this.maxValue, this.value);
            }
            set
            {
                this.value = Mathf.Lerp(this.minValue, this.maxValue, value);
            }
        }

        /// <summary>
        /// <para>Callback executed when the value of the slider is changed.</para>
        /// </summary>
        public SliderEvent onValueChanged
        {
            get => 
                this.m_OnValueChanged;
            set
            {
                this.m_OnValueChanged = value;
            }
        }

        private bool reverseValue =>
            ((this.m_Direction == Direction.RightToLeft) || (this.m_Direction == Direction.TopToBottom));

        private float stepSize =>
            (!this.wholeNumbers ? ((this.maxValue - this.minValue) * 0.1f) : 1f);

        /// <summary>
        /// <para>The current value of the slider.</para>
        /// </summary>
        public virtual float value
        {
            get
            {
                if (this.wholeNumbers)
                {
                    return Mathf.Round(this.m_Value);
                }
                return this.m_Value;
            }
            set
            {
                this.Set(value);
            }
        }

        /// <summary>
        /// <para>Should the value only be allowed to be whole numbers?</para>
        /// </summary>
        public bool wholeNumbers
        {
            get => 
                this.m_WholeNumbers;
            set
            {
                if (SetPropertyUtility.SetStruct<bool>(ref this.m_WholeNumbers, value))
                {
                    this.Set(this.m_Value);
                    this.UpdateVisuals();
                }
            }
        }

        private enum Axis
        {
            Horizontal,
            Vertical
        }

        /// <summary>
        /// <para>Setting that indicates one of four directions.</para>
        /// </summary>
        public enum Direction
        {
            LeftToRight,
            RightToLeft,
            BottomToTop,
            TopToBottom
        }

        /// <summary>
        /// <para>Event type used by the Slider.</para>
        /// </summary>
        [Serializable]
        public class SliderEvent : UnityEvent<float>
        {
        }
    }
}

