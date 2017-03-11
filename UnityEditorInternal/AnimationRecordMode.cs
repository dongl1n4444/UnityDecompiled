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
            UnityEditor.AnimationMode.animationModeChangedCallback = (UnityEditor.AnimationMode.AnimationModeChangedCallback) Delegate.Combine(UnityEditor.AnimationMode.animationModeChangedCallback, new UnityEditor.AnimationMode.AnimationModeChangedCallback(this.StateChangedCallback));
        }

        public void Dispose()
        {
            this.enable = false;
            UnityEditor.AnimationMode.animationModeChangedCallback = (UnityEditor.AnimationMode.AnimationModeChangedCallback) Delegate.Remove(UnityEditor.AnimationMode.animationModeChangedCallback, new UnityEditor.AnimationMode.AnimationModeChangedCallback(this.StateChangedCallback));
        }

        private void StateChangedCallback(bool newValue)
        {
            if (!this.m_IgnoreCallback)
            {
                this.m_Recording = false;
            }
        }

        public bool canEnable
        {
            get
            {
                bool flag = UnityEditor.AnimationMode.InAnimationMode();
                return (!flag || (this.m_Recording && flag));
            }
        }

        public bool enable
        {
            get
            {
                if (this.m_Recording)
                {
                    this.m_Recording = UnityEditor.AnimationMode.InAnimationMode();
                }
                return this.m_Recording;
            }
            set
            {
                if (value)
                {
                    if (!UnityEditor.AnimationMode.InAnimationMode())
                    {
                        this.m_IgnoreCallback = true;
                        UnityEditor.AnimationMode.StartAnimationMode();
                        this.m_IgnoreCallback = false;
                        this.m_Recording = true;
                    }
                }
                else if (this.m_Recording)
                {
                    this.m_IgnoreCallback = true;
                    UnityEditor.AnimationMode.StopAnimationMode();
                    this.m_IgnoreCallback = false;
                    this.m_Recording = false;
                }
            }
        }
    }
}

