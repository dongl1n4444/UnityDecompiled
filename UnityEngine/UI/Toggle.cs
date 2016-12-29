namespace UnityEngine.UI
{
    using System;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.Serialization;

    /// <summary>
    /// <para>A standard toggle that has an on / off state.</para>
    /// </summary>
    [AddComponentMenu("UI/Toggle", 0x1f), RequireComponent(typeof(RectTransform))]
    public class Toggle : Selectable, IPointerClickHandler, ISubmitHandler, ICanvasElement, IEventSystemHandler
    {
        /// <summary>
        /// <para>Graphic affected by the toggle.</para>
        /// </summary>
        public Graphic graphic;
        [SerializeField]
        private ToggleGroup m_Group;
        [FormerlySerializedAs("m_IsActive"), Tooltip("Is the toggle currently on or off?"), SerializeField]
        private bool m_IsOn;
        /// <summary>
        /// <para>Callback executed when the value of the toggle is changed.</para>
        /// </summary>
        public ToggleEvent onValueChanged = new ToggleEvent();
        /// <summary>
        /// <para>Transition mode for the toggle.</para>
        /// </summary>
        public ToggleTransition toggleTransition = ToggleTransition.Fade;

        protected Toggle()
        {
        }

        /// <summary>
        /// <para>See ICanvasElement.GraphicUpdateComplete.</para>
        /// </summary>
        public virtual void GraphicUpdateComplete()
        {
        }

        private void InternalToggle()
        {
            if (this.IsActive() && this.IsInteractable())
            {
                this.isOn = !this.isOn;
            }
        }

        /// <summary>
        /// <para>See ICanvasElement.LayoutComplete.</para>
        /// </summary>
        public virtual void LayoutComplete()
        {
        }

        protected override void OnDidApplyAnimationProperties()
        {
            if (this.graphic != null)
            {
                bool flag = !Mathf.Approximately(this.graphic.canvasRenderer.GetColor().a, 0f);
                if (this.m_IsOn != flag)
                {
                    this.m_IsOn = flag;
                    this.Set(!flag);
                }
            }
            base.OnDidApplyAnimationProperties();
        }

        /// <summary>
        /// <para>See MonoBehaviour.OnDisable.</para>
        /// </summary>
        protected override void OnDisable()
        {
            this.SetToggleGroup(null, false);
            base.OnDisable();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.SetToggleGroup(this.m_Group, false);
            this.PlayEffect(true);
        }

        /// <summary>
        /// <para>Handling for when the toggle is 'clicked'.</para>
        /// </summary>
        /// <param name="eventData">Current event.</param>
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                this.InternalToggle();
            }
        }

        /// <summary>
        /// <para>Handling for when the submit key is pressed.</para>
        /// </summary>
        /// <param name="eventData">Current event.</param>
        public virtual void OnSubmit(BaseEventData eventData)
        {
            this.InternalToggle();
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            if ((PrefabUtility.GetPrefabType(this) != PrefabType.Prefab) && !Application.isPlaying)
            {
                CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
            }
        }

        private void PlayEffect(bool instant)
        {
            if (this.graphic != null)
            {
                if (!Application.isPlaying)
                {
                    this.graphic.canvasRenderer.SetAlpha(!this.m_IsOn ? 0f : 1f);
                }
                else
                {
                    this.graphic.CrossFadeAlpha(!this.m_IsOn ? 0f : 1f, !instant ? 0.1f : 0f, true);
                }
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
                this.onValueChanged.Invoke(this.m_IsOn);
            }
        }

        private void Set(bool value)
        {
            this.Set(value, true);
        }

        private void Set(bool value, bool sendCallback)
        {
            if (this.m_IsOn != value)
            {
                this.m_IsOn = value;
                if (((this.m_Group != null) && this.IsActive()) && (this.m_IsOn || (!this.m_Group.AnyTogglesOn() && !this.m_Group.allowSwitchOff)))
                {
                    this.m_IsOn = true;
                    this.m_Group.NotifyToggleOn(this);
                }
                this.PlayEffect(this.toggleTransition == ToggleTransition.None);
                if (sendCallback)
                {
                    this.onValueChanged.Invoke(this.m_IsOn);
                }
            }
        }

        private void SetToggleGroup(ToggleGroup newGroup, bool setMemberValue)
        {
            ToggleGroup group = this.m_Group;
            if (this.m_Group != null)
            {
                this.m_Group.UnregisterToggle(this);
            }
            if (setMemberValue)
            {
                this.m_Group = newGroup;
            }
            if ((newGroup != null) && this.IsActive())
            {
                newGroup.RegisterToggle(this);
            }
            if (((newGroup != null) && (newGroup != group)) && (this.isOn && this.IsActive()))
            {
                newGroup.NotifyToggleOn(this);
            }
        }

        protected override void Start()
        {
            this.PlayEffect(true);
        }

        Transform ICanvasElement.get_transform() => 
            base.transform;

        /// <summary>
        /// <para>Group the toggle belongs to.</para>
        /// </summary>
        public ToggleGroup group
        {
            get => 
                this.m_Group;
            set
            {
                this.m_Group = value;
                if (Application.isPlaying)
                {
                    this.SetToggleGroup(this.m_Group, true);
                    this.PlayEffect(true);
                }
            }
        }

        /// <summary>
        /// <para>Is the toggle on.</para>
        /// </summary>
        public bool isOn
        {
            get => 
                this.m_IsOn;
            set
            {
                this.Set(value);
            }
        }

        /// <summary>
        /// <para>UnityEvent callback for when a toggle is toggled.</para>
        /// </summary>
        [Serializable]
        public class ToggleEvent : UnityEvent<bool>
        {
        }

        /// <summary>
        /// <para>Display settings for when a toggle is activated or deactivated.</para>
        /// </summary>
        public enum ToggleTransition
        {
            None,
            Fade
        }
    }
}

