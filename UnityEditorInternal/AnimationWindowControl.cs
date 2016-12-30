namespace UnityEditorInternal
{
    using System;
    using UnityEditor;
    using UnityEngine;

    internal class AnimationWindowControl : IAnimationWindowControl
    {
        [NonSerialized]
        private float m_PreviousUpdateTime;
        [NonSerialized]
        private AnimationRecordMode m_Recording;
        [SerializeField]
        private AnimationKeyTime m_Time;
        [NonSerialized]
        public AnimationWindowState state;

        public override void EndScrubTime()
        {
        }

        public override void GoToFirstKeyframe()
        {
            if (this.state.activeAnimationClip != null)
            {
                this.SetCurrentTime(this.state.activeAnimationClip.startTime);
            }
        }

        public override void GoToFrame(int frame)
        {
            this.SetCurrentFrame(frame);
        }

        public override void GoToLastKeyframe()
        {
            if (this.state.activeAnimationClip != null)
            {
                this.SetCurrentTime(this.state.activeAnimationClip.stopTime);
            }
        }

        public override void GoToNextFrame()
        {
            this.SetCurrentFrame(this.time.frame + 1);
        }

        public override void GoToNextKeyframe()
        {
            float time = AnimationWindowUtility.GetNextKeyframeTime(((!this.state.showCurveEditor || (this.state.activeCurves.Count <= 0)) ? this.state.allCurves : this.state.activeCurves).ToArray(), this.time.time, this.state.clipFrameRate);
            this.SetCurrentTime(this.state.SnapToFrame(time, AnimationWindowState.SnapMode.SnapToClipFrame));
        }

        public override void GoToPreviousFrame()
        {
            this.SetCurrentFrame(this.time.frame - 1);
        }

        public override void GoToPreviousKeyframe()
        {
            float time = AnimationWindowUtility.GetPreviousKeyframeTime(((!this.state.showCurveEditor || (this.state.activeCurves.Count <= 0)) ? this.state.allCurves : this.state.activeCurves).ToArray(), this.time.time, this.state.clipFrameRate);
            this.SetCurrentTime(this.state.SnapToFrame(time, AnimationWindowState.SnapMode.SnapToClipFrame));
        }

        public override void GoToTime(float time)
        {
            this.SetCurrentTime(time);
        }

        public void OnDisable()
        {
            this.StopRecording();
            this.StopPlayback();
            if (this.m_Recording != null)
            {
                this.m_Recording.Dispose();
                this.m_Recording = null;
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();
            this.m_Recording = new AnimationRecordMode();
        }

        public override void OnSelectionChanged()
        {
            if (this.state != null)
            {
                this.m_Time = AnimationKeyTime.Time(0f, this.state.frameRate);
            }
            this.StopRecording();
            this.StopPlayback();
        }

        public override bool PlaybackUpdate()
        {
            if (!this.playing)
            {
                return false;
            }
            float num = Time.realtimeSinceStartup - this.m_PreviousUpdateTime;
            this.m_PreviousUpdateTime = Time.realtimeSinceStartup;
            float minTime = this.time.time + num;
            if (minTime > this.state.maxTime)
            {
                minTime = this.state.minTime;
            }
            this.m_Time = AnimationKeyTime.Time(Mathf.Clamp(minTime, this.state.minTime, this.state.maxTime), this.state.frameRate);
            this.ResampleAnimation();
            return true;
        }

        private UndoPropertyModification[] PostprocessAnimationRecordingModifications(UndoPropertyModification[] modifications)
        {
            if (!AnimationMode.InAnimationMode())
            {
                Undo.postprocessModifications = (Undo.PostprocessModifications) Delegate.Remove(Undo.postprocessModifications, new Undo.PostprocessModifications(this.PostprocessAnimationRecordingModifications));
                return modifications;
            }
            return AnimationRecording.Process(this.state, modifications);
        }

        public override void ResampleAnimation()
        {
            if ((!this.state.disabled && this.recording) && this.canRecord)
            {
                bool flag = false;
                foreach (AnimationWindowSelectionItem item in this.state.selection.ToArray())
                {
                    if (item.animationClip != null)
                    {
                        Undo.FlushUndoRecordObjects();
                        AnimationMode.BeginSampling();
                        AnimationMode.SampleAnimationClip(item.rootGameObject, item.animationClip, this.time.time - item.timeOffset);
                        AnimationMode.EndSampling();
                        flag = true;
                    }
                }
                if (flag)
                {
                    SceneView.RepaintAll();
                    InspectorWindow.RepaintAllInspectors();
                    ParticleSystemWindow instance = ParticleSystemWindow.GetInstance();
                    if (instance != null)
                    {
                        instance.Repaint();
                    }
                }
            }
        }

        public override void ScrubTime(float time)
        {
            this.SetCurrentTime(time);
        }

        private void SetCurrentFrame(int value)
        {
            if (value != this.time.frame)
            {
                this.m_Time = AnimationKeyTime.Frame(value, this.state.frameRate);
                this.StartRecording();
                this.ResampleAnimation();
            }
        }

        private void SetCurrentTime(float value)
        {
            if (!Mathf.Approximately(value, this.time.time))
            {
                this.m_Time = AnimationKeyTime.Time(value, this.state.frameRate);
                this.StartRecording();
                this.ResampleAnimation();
            }
        }

        private void SnapTimeToFrame()
        {
            float num = this.state.FrameToTime((float) this.time.frame);
            this.SetCurrentTime(num);
        }

        public override void StartPlayback()
        {
            if (this.canPlay && !AnimationMode.InAnimationPlaybackMode())
            {
                AnimationMode.StartAnimationPlaybackMode();
                this.m_PreviousUpdateTime = Time.realtimeSinceStartup;
                this.StartRecording();
            }
        }

        private void StartRecording()
        {
            if ((this.canRecord && (this.m_Recording != null)) && !this.m_Recording.enable)
            {
                this.m_Recording.enable = true;
                Undo.postprocessModifications = (Undo.PostprocessModifications) Delegate.Combine(Undo.postprocessModifications, new Undo.PostprocessModifications(this.PostprocessAnimationRecordingModifications));
            }
        }

        public override void StartRecording(Object targetObject)
        {
            this.StartRecording();
        }

        public override void StartScrubTime()
        {
        }

        public override void StopPlayback()
        {
            if (AnimationMode.InAnimationPlaybackMode())
            {
                AnimationMode.StopAnimationPlaybackMode();
                this.SnapTimeToFrame();
            }
        }

        public override void StopRecording()
        {
            this.StopPlayback();
            if ((this.m_Recording != null) && this.m_Recording.enable)
            {
                this.m_Recording.enable = false;
                Undo.postprocessModifications = (Undo.PostprocessModifications) Delegate.Remove(Undo.postprocessModifications, new Undo.PostprocessModifications(this.PostprocessAnimationRecordingModifications));
            }
        }

        public override bool canPlay =>
            this.canRecord;

        public override bool canRecord
        {
            get
            {
                if (!this.state.selection.canRecord)
                {
                    return false;
                }
                return ((this.m_Recording != null) && this.m_Recording.canEnable);
            }
        }

        public override bool playing =>
            (AnimationMode.InAnimationPlaybackMode() && this.recording);

        public override bool recording =>
            ((this.m_Recording != null) && this.m_Recording.enable);

        public override AnimationKeyTime time =>
            this.m_Time;
    }
}

