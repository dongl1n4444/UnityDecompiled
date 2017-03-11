namespace UnityEngine.EventSystems
{
    using System;
    using UnityEngine;
    using UnityEngine.VR;

    /// <summary>
    /// <para>A BaseInputModule designed for HoloLens input.</para>
    /// </summary>
    [RequireComponent(typeof(HoloLensInput)), AddComponentMenu("Event/HoloLens Input Module")]
    public class HoloLensInputModule : StandaloneInputModule
    {
        private bool m_HasBeenActivated = false;
        private bool m_HasGestureToProcess = false;
        private HoloLensInput m_HoloLensInput;
        [SerializeField, Tooltip("Maximum number of pixels in screen space to move a widget during a navigation gesture")]
        private float m_NormalizedNavigationToScreenOffsetScalar = 500f;
        [SerializeField, Tooltip("Amount of time to show things that responds to clicks in their on-pressed state")]
        private float m_TimeToPressOnTap = 0.3f;

        protected HoloLensInputModule()
        {
        }

        /// <summary>
        /// <para>See BaseInputModule.</para>
        /// </summary>
        public override void ActivateModule()
        {
            this.m_HasBeenActivated = true;
            base.ActivateModule();
        }

        protected override void Awake()
        {
            base.Awake();
            this.m_HoloLensInput = base.GetComponent<HoloLensInput>();
            if (this.m_HoloLensInput == null)
            {
                this.m_HoloLensInput = base.gameObject.AddComponent<HoloLensInput>();
            }
            base.m_InputOverride = this.m_HoloLensInput;
        }

        [Obsolete("This method was never intended for public consumption - if you needed it as a workaround for something, please report the accompanying bug.", true)]
        public void HoloLensInput_GestureNotifier()
        {
        }

        [Obsolete("This method was never intended for public consumption - if you needed it as a workaround for something, please report the accompanying bug.")]
        public EventSystem HoloLensInput_GetEventSystem() => 
            base.eventSystem;

        [Obsolete("HoloLensInput_GetScreenOffsetScalar has been deprecated. Use normalizedNavigationToScreenOffsetScalar instead. (UnityUpgradable) -> normalizedNavigationToScreenOffsetScalar")]
        public float HoloLensInput_GetScreenOffsetScalar() => 
            this.normalizedNavigationToScreenOffsetScalar;

        [Obsolete("HoloLensInput_GetTimeToPressOnTap has been deprecated. Use timeToPressOnTap instead. (UnityUpgradable) -> timeToPressOnTap")]
        public float HoloLensInput_GetTimeToPressOnTap() => 
            this.timeToPressOnTap;

        internal void Internal_GestureNotifier()
        {
            this.m_HasGestureToProcess = true;
        }

        internal GameObject Internal_GetCurrentFocusedGameObject() => 
            base.GetCurrentFocusedGameObject();

        /// <summary>
        /// <para>See BaseInputModule.</para>
        /// </summary>
        public override bool IsModuleSupported() => 
            (base.IsModuleSupported() && string.Equals(VRSettings.loadedDeviceName, "HoloLens"));

        /// <summary>
        /// <para>See BaseInputModule.</para>
        /// </summary>
        public override bool ShouldActivateModule() => 
            ((base.forceModuleActive || this.m_HasGestureToProcess) || !this.m_HasBeenActivated);

        /// <summary>
        /// <para>See BaseInputModule.</para>
        /// </summary>
        public override void UpdateModule()
        {
            this.m_HoloLensInput.UpdateInput();
            base.UpdateModule();
        }

        /// <summary>
        /// <para>Defines the number of pixels of emulated mouse movement when a value of positive or negative 1 is reported by the device for a navigation gesture.</para>
        /// </summary>
        public float normalizedNavigationToScreenOffsetScalar
        {
            get => 
                this.m_NormalizedNavigationToScreenOffsetScalar;
            set
            {
                this.m_NormalizedNavigationToScreenOffsetScalar = value;
            }
        }

        /// <summary>
        /// <para>Controls the number of seconds that taps (emulated mouse clicks) will leave any tapped UI object in the Pressed state for better user feedback.</para>
        /// </summary>
        public float timeToPressOnTap
        {
            get => 
                this.m_TimeToPressOnTap;
            set
            {
                this.m_TimeToPressOnTap = value;
            }
        }
    }
}

