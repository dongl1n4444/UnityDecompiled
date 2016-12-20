namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class ButtonWithAnimatedIconRotation
    {
        private readonly Func<float> m_AngleCallback;
        private readonly CallbackController m_CallbackController;
        private readonly bool m_MouseDownButton;

        public ButtonWithAnimatedIconRotation(Func<float> angleCallback, Action repaintCallback, float repaintsPerSecond, bool mouseDownButton)
        {
            this.m_CallbackController = new CallbackController(repaintCallback, repaintsPerSecond);
            this.m_AngleCallback = angleCallback;
            this.m_MouseDownButton = mouseDownButton;
        }

        public void Clear()
        {
            this.m_CallbackController.Stop();
        }

        public bool OnGUI(Rect rect, GUIContent guiContent, bool animate, GUIStyle style)
        {
            if (animate && !this.m_CallbackController.active)
            {
                this.m_CallbackController.Start();
            }
            if (!animate && this.m_CallbackController.active)
            {
                this.m_CallbackController.Stop();
            }
            float iconAngle = !animate ? 0f : this.m_AngleCallback.Invoke();
            return EditorGUI.ButtonWithRotatedIcon(rect, guiContent, iconAngle, this.m_MouseDownButton, style);
        }
    }
}

