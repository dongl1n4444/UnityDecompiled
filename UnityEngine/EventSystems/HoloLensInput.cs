namespace UnityEngine.EventSystems
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.VR.WSA.Input;

    public class HoloLensInput : BaseInput
    {
        private GestureRecognizer m_GestureRecognizer;
        private bool m_IsEmulatedMouseDownCurr = false;
        private bool m_IsEmulatedMouseDownPrev = false;
        private float m_LastTapTime = 0f;
        private HoloLensInputModule m_Module;
        private MouseEmulationMode m_MouseEmulationMode = MouseEmulationMode.Inactive;
        private Vector3 m_NavigationAnchorWorldSpace = Vector3.zero;
        private Vector3 m_NavigationNormalizedOffset = Vector3.zero;
        private Vector3 m_TapAnchorWorldSpace = Vector3.zero;

        protected override void Awake()
        {
            base.Awake();
            this.m_Module = base.GetComponent<HoloLensInputModule>();
            this.m_GestureRecognizer = new GestureRecognizer();
            this.m_GestureRecognizer.TappedEvent += new GestureRecognizer.TappedEventDelegate(this.GestureHandler_Tapped);
            this.m_GestureRecognizer.NavigationStartedEvent += new GestureRecognizer.NavigationStartedEventDelegate(this.GestureHandler_NavigationStarted);
            this.m_GestureRecognizer.NavigationUpdatedEvent += new GestureRecognizer.NavigationUpdatedEventDelegate(this.GestureHandler_NavigationUpdated);
            this.m_GestureRecognizer.NavigationCompletedEvent += new GestureRecognizer.NavigationCompletedEventDelegate(this.GestureHandler_NavigationCompletedOrCanceled);
            this.m_GestureRecognizer.NavigationCanceledEvent += new GestureRecognizer.NavigationCanceledEventDelegate(this.GestureHandler_NavigationCompletedOrCanceled);
            this.m_GestureRecognizer.SetRecognizableGestures(GestureSettings.NavigationZ | GestureSettings.NavigationY | GestureSettings.NavigationX | GestureSettings.Tap);
            this.m_GestureRecognizer.StartCapturingGestures();
        }

        private Vector2 EmulateMousePosition(Vector3 anchorWorldspace, Vector2 finalOffset) => 
            (Camera.main.WorldToScreenPoint(anchorWorldspace) + finalOffset);

        private void GestureHandler_NavigationCompletedOrCanceled(InteractionSourceKind source, Vector3 normalizedOffset, Ray headRay)
        {
            this.m_Module.HoloLensInput_GestureNotifier();
            this.m_NavigationNormalizedOffset = Vector3.zero;
            this.m_MouseEmulationMode = MouseEmulationMode.Inactive;
        }

        private void GestureHandler_NavigationStarted(InteractionSourceKind source, Vector3 normalizedOffset, Ray headRay)
        {
            this.m_Module.HoloLensInput_GestureNotifier();
            if (this.TryGetAnchorWorldSpace(out this.m_NavigationAnchorWorldSpace))
            {
                this.m_MouseEmulationMode = MouseEmulationMode.Navigation;
                this.m_NavigationNormalizedOffset = normalizedOffset;
            }
        }

        private void GestureHandler_NavigationUpdated(InteractionSourceKind source, Vector3 normalizedOffset, Ray headRay)
        {
            this.m_Module.HoloLensInput_GestureNotifier();
            this.m_NavigationNormalizedOffset = normalizedOffset;
        }

        private void GestureHandler_Tapped(InteractionSourceKind source, int tapCount, Ray headRay)
        {
            this.m_Module.HoloLensInput_GestureNotifier();
            if (this.TryGetAnchorWorldSpace(out this.m_TapAnchorWorldSpace))
            {
                this.m_MouseEmulationMode = MouseEmulationMode.Tap;
                this.m_LastTapTime = Time.time;
            }
        }

        private Vector2 GetGazeAndGestureScreenPosition()
        {
            switch (this.m_MouseEmulationMode)
            {
                case MouseEmulationMode.Navigation:
                    return this.EmulateMousePosition(this.m_NavigationAnchorWorldSpace, (Vector2) (this.m_Module.HoloLensInput_GetScreenOffsetScalar() * new Vector2(this.m_NavigationNormalizedOffset.x, this.m_NavigationNormalizedOffset.y)));

                case MouseEmulationMode.Tap:
                    return this.EmulateMousePosition(this.m_TapAnchorWorldSpace, Vector2.zero);
            }
            return GetGazeScreenPosition();
        }

        private static Vector2 GetGazeScreenPosition() => 
            new Vector2(0.5f * Screen.width, 0.5f * Screen.height);

        private Vector2 GetGestureScrollDelta() => 
            ((this.m_MouseEmulationMode != MouseEmulationMode.Navigation) ? Vector2.zero : new Vector2(0f, this.m_NavigationNormalizedOffset.z));

        public override bool GetMouseButton(int button) => 
            ((button == 0) && this.m_IsEmulatedMouseDownCurr);

        public override bool GetMouseButtonDown(int button) => 
            (((button == 0) && !this.m_IsEmulatedMouseDownPrev) && this.m_IsEmulatedMouseDownCurr);

        public override bool GetMouseButtonUp(int button) => 
            (((button == 0) && this.m_IsEmulatedMouseDownPrev) && !this.m_IsEmulatedMouseDownCurr);

        protected override void OnDestroy()
        {
            this.m_GestureRecognizer.StopCapturingGestures();
            this.m_GestureRecognizer.TappedEvent -= new GestureRecognizer.TappedEventDelegate(this.GestureHandler_Tapped);
            this.m_GestureRecognizer.NavigationStartedEvent -= new GestureRecognizer.NavigationStartedEventDelegate(this.GestureHandler_NavigationStarted);
            this.m_GestureRecognizer.NavigationUpdatedEvent -= new GestureRecognizer.NavigationUpdatedEventDelegate(this.GestureHandler_NavigationUpdated);
            this.m_GestureRecognizer.NavigationCompletedEvent -= new GestureRecognizer.NavigationCompletedEventDelegate(this.GestureHandler_NavigationCompletedOrCanceled);
            this.m_GestureRecognizer.NavigationCanceledEvent -= new GestureRecognizer.NavigationCanceledEventDelegate(this.GestureHandler_NavigationCompletedOrCanceled);
            base.OnDestroy();
        }

        private bool TryGetAnchorWorldSpace(out Vector3 anchor)
        {
            EventSystem system = this.m_Module.HoloLensInput_GetEventSystem();
            if ((null == system) || (null == system.currentSelectedGameObject))
            {
                anchor = Vector3.zero;
                return false;
            }
            RectTransform component = system.currentSelectedGameObject.GetComponent<RectTransform>();
            if (null == component)
            {
                anchor = Vector3.zero;
                return false;
            }
            return RectTransformUtility.ScreenPointToWorldPointInRectangle(component, GetGazeScreenPosition(), Camera.main, out anchor);
        }

        public void UpdateInput()
        {
            if ((this.m_MouseEmulationMode == MouseEmulationMode.Tap) && ((this.m_LastTapTime + this.m_Module.HoloLensInput_GetTimeToPressOnTap()) < Time.time))
            {
                this.m_MouseEmulationMode = MouseEmulationMode.Inactive;
            }
            this.m_IsEmulatedMouseDownPrev = this.m_IsEmulatedMouseDownCurr;
            this.m_IsEmulatedMouseDownCurr = this.m_MouseEmulationMode != MouseEmulationMode.Inactive;
        }

        public override Vector2 mousePosition =>
            this.GetGazeAndGestureScreenPosition();

        public override bool mousePresent =>
            true;

        public override Vector2 mouseScrollDelta =>
            this.GetGestureScrollDelta();

        public override int touchCount =>
            0;

        public override bool touchSupported =>
            false;

        private enum MouseEmulationMode
        {
            Inactive,
            Navigation,
            Tap
        }
    }
}

