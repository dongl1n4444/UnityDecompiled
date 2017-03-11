namespace UnityEditor
{
    using System;
    using UnityEditor.AnimatedValues;
    using UnityEngine;

    internal class CameraControllerStandard : CameraController
    {
        private const float kFlyAcceleration = 1.1f;
        private static PrefKey kFPSBack = new PrefKey("View/FPS Back", "s");
        private static PrefKey kFPSDown = new PrefKey("View/FPS Strafe Down", "q");
        private static PrefKey kFPSForward = new PrefKey("View/FPS Forward", "w");
        private static PrefKey kFPSLeft = new PrefKey("View/FPS Strafe Left", "a");
        private static PrefKey kFPSRight = new PrefKey("View/FPS Strafe Right", "d");
        private static PrefKey kFPSUp = new PrefKey("View/FPS Strafe Up", "e");
        private UnityEditor.ViewTool m_CurrentViewTool = UnityEditor.ViewTool.None;
        private float m_FlySpeed = 0f;
        private static TimeHelper m_FPSTiming = new TimeHelper();
        private Vector3 m_Motion = new Vector3();
        private float m_StartZoom = 0f;
        private float m_TotalMotion = 0f;
        private float m_ZoomSpeed = 0f;

        private Vector3 GetMovementDirection()
        {
            float p = m_FPSTiming.Update();
            if (this.m_Motion.sqrMagnitude == 0f)
            {
                this.m_FlySpeed = 0f;
                return Vector3.zero;
            }
            float num2 = !Event.current.shift ? ((float) 1) : ((float) 5);
            if (this.m_FlySpeed == 0f)
            {
                this.m_FlySpeed = 9f;
            }
            else
            {
                this.m_FlySpeed *= Mathf.Pow(1.1f, p);
            }
            return (Vector3) (((this.m_Motion.normalized * this.m_FlySpeed) * num2) * p);
        }

        private void HandleCameraKeyDown()
        {
            if (Event.current.keyCode == KeyCode.Escape)
            {
                this.ResetCameraControl();
            }
            if (this.m_CurrentViewTool == UnityEditor.ViewTool.FPS)
            {
                Event current = Event.current;
                Vector3 motion = this.m_Motion;
                if (current.keyCode == kFPSForward.keyCode)
                {
                    this.m_Motion.z = 1f;
                    current.Use();
                }
                else if (current.keyCode == kFPSBack.keyCode)
                {
                    this.m_Motion.z = -1f;
                    current.Use();
                }
                else if (current.keyCode == kFPSLeft.keyCode)
                {
                    this.m_Motion.x = -1f;
                    current.Use();
                }
                else if (current.keyCode == kFPSRight.keyCode)
                {
                    this.m_Motion.x = 1f;
                    current.Use();
                }
                else if (current.keyCode == kFPSUp.keyCode)
                {
                    this.m_Motion.y = 1f;
                    current.Use();
                }
                else if (current.keyCode == kFPSDown.keyCode)
                {
                    this.m_Motion.y = -1f;
                    current.Use();
                }
                if ((current.type != EventType.KeyDown) && (motion.sqrMagnitude == 0f))
                {
                    m_FPSTiming.Begin();
                }
            }
        }

        private void HandleCameraKeyUp()
        {
            if (this.m_CurrentViewTool == UnityEditor.ViewTool.FPS)
            {
                Event current = Event.current;
                if (current.keyCode == kFPSForward.keyCode)
                {
                    this.m_Motion.z = 0f;
                    current.Use();
                }
                else if (current.keyCode == kFPSBack.keyCode)
                {
                    this.m_Motion.z = 0f;
                    current.Use();
                }
                else if (current.keyCode == kFPSLeft.keyCode)
                {
                    this.m_Motion.x = 0f;
                    current.Use();
                }
                else if (current.keyCode == kFPSRight.keyCode)
                {
                    this.m_Motion.x = 0f;
                    current.Use();
                }
                else if (current.keyCode == kFPSUp.keyCode)
                {
                    this.m_Motion.y = 0f;
                    current.Use();
                }
                else if (current.keyCode == kFPSDown.keyCode)
                {
                    this.m_Motion.y = 0f;
                    current.Use();
                }
            }
        }

        private void HandleCameraMouseDrag(CameraState cameraState, Camera cam)
        {
            Event current = Event.current;
            switch (this.m_CurrentViewTool)
            {
                case UnityEditor.ViewTool.Orbit:
                    this.OrbitCameraBehavior(cameraState, cam);
                    break;

                case UnityEditor.ViewTool.Pan:
                {
                    cameraState.FixNegativeSize();
                    Vector3 position = cam.WorldToScreenPoint(cameraState.pivot.value) + new Vector3(-Event.current.delta.x, Event.current.delta.y, 0f);
                    Vector3 vector7 = cam.ScreenToWorldPoint(position) - cameraState.pivot.value;
                    if (current.shift)
                    {
                        vector7 = (Vector3) (vector7 * 4f);
                    }
                    AnimVector3 pivot = cameraState.pivot;
                    pivot.value += vector7;
                    break;
                }
                case UnityEditor.ViewTool.Zoom:
                {
                    float num = HandleUtility.niceMouseDeltaZoom * (!current.shift ? ((float) 3) : ((float) 9));
                    this.m_TotalMotion += num;
                    if (this.m_TotalMotion >= 0f)
                    {
                        cameraState.viewSize.value += (num * this.m_ZoomSpeed) * 0.003f;
                        break;
                    }
                    cameraState.viewSize.value = this.m_StartZoom * (1f + (this.m_TotalMotion * 0.001f));
                    break;
                }
                case UnityEditor.ViewTool.FPS:
                {
                    Vector3 vector = cameraState.pivot.value - ((Vector3) ((cameraState.rotation.value * Vector3.forward) * cameraState.GetCameraDistance()));
                    Quaternion quaternion = cameraState.rotation.value;
                    quaternion = Quaternion.AngleAxis((current.delta.y * 0.003f) * 57.29578f, (Vector3) (quaternion * Vector3.right)) * quaternion;
                    quaternion = Quaternion.AngleAxis((current.delta.x * 0.003f) * 57.29578f, Vector3.up) * quaternion;
                    cameraState.rotation.value = quaternion;
                    cameraState.pivot.value = vector + ((Vector3) ((quaternion * Vector3.forward) * cameraState.GetCameraDistance()));
                    break;
                }
            }
            current.Use();
        }

        private void HandleCameraMouseUp()
        {
            this.ResetCameraControl();
            Event.current.Use();
        }

        private void HandleCameraScrollWheel(CameraState cameraState)
        {
            float y = Event.current.delta.y;
            float num2 = (Mathf.Abs(cameraState.viewSize.value) * y) * 0.015f;
            if ((num2 > 0f) && (num2 < 0.3f))
            {
                num2 = 0.3f;
            }
            else if ((num2 < 0f) && (num2 > -0.3f))
            {
                num2 = -0.3f;
            }
            AnimFloat viewSize = cameraState.viewSize;
            viewSize.value += num2;
            Event.current.Use();
        }

        private void OrbitCameraBehavior(CameraState cameraState, Camera cam)
        {
            Event current = Event.current;
            cameraState.FixNegativeSize();
            Quaternion target = cameraState.rotation.target;
            target = Quaternion.AngleAxis((current.delta.y * 0.003f) * 57.29578f, (Vector3) (target * Vector3.right)) * target;
            target = Quaternion.AngleAxis((current.delta.x * 0.003f) * 57.29578f, Vector3.up) * target;
            if (cameraState.viewSize.value < 0f)
            {
                cameraState.pivot.value = cam.transform.position;
                cameraState.viewSize.value = 0f;
            }
            cameraState.rotation.value = target;
        }

        private void ResetCameraControl()
        {
            this.m_CurrentViewTool = UnityEditor.ViewTool.None;
            this.m_Motion = Vector3.zero;
        }

        public override void Update(CameraState cameraState, Camera cam)
        {
            Event current = Event.current;
            if (current.type == EventType.MouseUp)
            {
                this.m_CurrentViewTool = UnityEditor.ViewTool.None;
            }
            if (current.type == EventType.MouseDown)
            {
                int button = current.button;
                bool flag = current.control && (Application.platform == RuntimePlatform.OSXEditor);
                if (button == 2)
                {
                    this.m_CurrentViewTool = UnityEditor.ViewTool.Pan;
                }
                else if (((button <= 0) && flag) || ((button == 1) && current.alt))
                {
                    this.m_CurrentViewTool = UnityEditor.ViewTool.Zoom;
                    this.m_StartZoom = cameraState.viewSize.value;
                    this.m_ZoomSpeed = Mathf.Max(Mathf.Abs(this.m_StartZoom), 0.3f);
                    this.m_TotalMotion = 0f;
                }
                else if (button <= 0)
                {
                    this.m_CurrentViewTool = UnityEditor.ViewTool.Orbit;
                }
                else if ((button == 1) && !current.alt)
                {
                    this.m_CurrentViewTool = UnityEditor.ViewTool.FPS;
                }
            }
            switch (current.type)
            {
                case EventType.MouseUp:
                    this.HandleCameraMouseUp();
                    break;

                case EventType.MouseDrag:
                    this.HandleCameraMouseDrag(cameraState, cam);
                    break;

                case EventType.KeyDown:
                    this.HandleCameraKeyDown();
                    break;

                case EventType.KeyUp:
                    this.HandleCameraKeyUp();
                    break;

                case EventType.ScrollWheel:
                    this.HandleCameraScrollWheel(cameraState);
                    break;

                case EventType.Layout:
                {
                    Vector3 movementDirection = this.GetMovementDirection();
                    if (movementDirection.sqrMagnitude != 0f)
                    {
                        cameraState.pivot.value += cameraState.rotation.value * movementDirection;
                    }
                    break;
                }
            }
        }

        public UnityEditor.ViewTool currentViewTool =>
            this.m_CurrentViewTool;
    }
}

