namespace UnityEngine.EventSystems
{
    using System;
    using UnityEngine;
    using UnityEngine.VR;

    /// <summary>
    /// <para>A BaseInputModule designed for HoloLens input.</para>
    /// </summary>
    [AddComponentMenu("Event/HoloLens Input Module")]
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

        protected override bool ForceAutoSelect() => 
            true;

        public void HoloLensInput_GestureNotifier()
        {
            this.m_HasGestureToProcess = true;
        }

        public EventSystem HoloLensInput_GetEventSystem() => 
            base.eventSystem;

        public float HoloLensInput_GetScreenOffsetScalar() => 
            this.m_NormalizedNavigationToScreenOffsetScalar;

        public float HoloLensInput_GetTimeToPressOnTap() => 
            this.m_TimeToPressOnTap;

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

