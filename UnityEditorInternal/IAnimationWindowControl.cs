namespace UnityEditorInternal
{
    using System;
    using UnityEngine;

    internal abstract class IAnimationWindowControl : ScriptableObject
    {
        protected IAnimationWindowControl()
        {
        }

        public abstract void EndScrubTime();
        public abstract void GoToFirstKeyframe();
        public abstract void GoToFrame(int frame);
        public abstract void GoToLastKeyframe();
        public abstract void GoToNextFrame();
        public abstract void GoToNextKeyframe();
        public abstract void GoToPreviousFrame();
        public abstract void GoToPreviousKeyframe();
        public abstract void GoToTime(float time);
        public virtual void OnEnable()
        {
            base.hideFlags = HideFlags.HideAndDontSave;
        }

        public abstract void OnSelectionChanged();
        public abstract bool PlaybackUpdate();
        public abstract void ResampleAnimation();
        public abstract void ScrubTime(float time);
        public abstract bool StartPlayback();
        public abstract bool StartPreview();
        public abstract bool StartRecording(UnityEngine.Object targetObject);
        public abstract void StartScrubTime();
        public abstract void StopPlayback();
        public abstract void StopPreview();
        public abstract void StopRecording();

        public abstract bool canPlay { get; }

        public abstract bool canPreview { get; }

        public abstract bool canRecord { get; }

        public abstract bool playing { get; }

        public abstract bool previewing { get; }

        public abstract bool recording { get; }

        public abstract AnimationKeyTime time { get; }
    }
}

