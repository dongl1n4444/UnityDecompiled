namespace UnityEditorInternal
{
    using System;
    using UnityEditor;

    internal class AnimationRecordMode : IDisposable
    {
        private bool m_IgnoreCallback;
        private bool m_Recording;

        public AnimationRecordMode()
        {
            AnimationMode.animationModeChangedCallback = (AnimationMode.AnimationModeChangedCallback) Delegate.Combine(AnimationMode.animationModeChangedCallback, new AnimationMode.AnimationModeChangedCallback(this.StateChangedCallback));
        }

        public void Dispose()
        {
            this.enable = false;
            AnimationMode.animationModeChangedCallback = (AnimationMode.AnimationModeChangedCallback) Delegate.Remove(AnimationMode.animationModeChangedCallback, new AnimationMode.AnimationModeChangedCallback(this.StateChangedCallback));
        }

        private void StateChangedCallback(bool newValue)
        {
            if (!this.m_IgnoreCallback)
            {
                this.m_Recording = false;
            }
        }

        public bool canEnable =>
            (!AnimationMode.InAnimationMode() || this.m_Recording);

        public bool enable
        {
            get
            {
                if (this.m_Recording)
                {
                    this.m_Recording = AnimationMode.InAnimationMode();
                }
                return this.m_Recording;
            }
            set
            {
                if (value)
                {
                    if (!AnimationMode.InAnimationMode())
                    {
                        this.m_IgnoreCallback = true;
                        AnimationMode.StartAnimationMode();
                        this.m_IgnoreCallback = false;
                        this.m_Recording = true;
                    }
                }
                else if (this.m_Recording)
                {
                    this.m_IgnoreCallback = true;
                    AnimationMode.StopAnimationMode();
                    this.m_IgnoreCallback = false;
                    this.m_Recording = false;
                }
            }
        }
    }
}

