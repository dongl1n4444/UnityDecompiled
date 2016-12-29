namespace UnityEngine.UI.CoroutineTween
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;

    [StructLayout(LayoutKind.Sequential)]
    internal struct ColorTween : ITweenValue
    {
        private ColorTweenCallback m_Target;
        private Color m_StartColor;
        private Color m_TargetColor;
        private ColorTweenMode m_TweenMode;
        private float m_Duration;
        private bool m_IgnoreTimeScale;
        public Color startColor
        {
            get => 
                this.m_StartColor;
            set
            {
                this.m_StartColor = value;
            }
        }
        public Color targetColor
        {
            get => 
                this.m_TargetColor;
            set
            {
                this.m_TargetColor = value;
            }
        }
        public ColorTweenMode tweenMode
        {
            get => 
                this.m_TweenMode;
            set
            {
                this.m_TweenMode = value;
            }
        }
        public float duration
        {
            get => 
                this.m_Duration;
            set
            {
                this.m_Duration = value;
            }
        }
        public bool ignoreTimeScale
        {
            get => 
                this.m_IgnoreTimeScale;
            set
            {
                this.m_IgnoreTimeScale = value;
            }
        }
        public void TweenValue(float floatPercentage)
        {
            if (this.ValidTarget())
            {
                Color color = Color.Lerp(this.m_StartColor, this.m_TargetColor, floatPercentage);
                if (this.m_TweenMode == ColorTweenMode.Alpha)
                {
                    color.r = this.m_StartColor.r;
                    color.g = this.m_StartColor.g;
                    color.b = this.m_StartColor.b;
                }
                else if (this.m_TweenMode == ColorTweenMode.RGB)
                {
                    color.a = this.m_StartColor.a;
                }
                this.m_Target.Invoke(color);
            }
        }

        public void AddOnChangedCallback(UnityAction<Color> callback)
        {
            if (this.m_Target == null)
            {
                this.m_Target = new ColorTweenCallback();
            }
            this.m_Target.AddListener(callback);
        }

        public bool GetIgnoreTimescale() => 
            this.m_IgnoreTimeScale;

        public float GetDuration() => 
            this.m_Duration;

        public bool ValidTarget() => 
            (this.m_Target != null);
        public class ColorTweenCallback : UnityEvent<Color>
        {
        }

        public enum ColorTweenMode
        {
            All,
            RGB,
            Alpha
        }
    }
}

