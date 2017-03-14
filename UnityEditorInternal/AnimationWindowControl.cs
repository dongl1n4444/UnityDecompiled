namespace UnityEditorInternal
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEngine;

    internal class AnimationWindowControl : IAnimationWindowControl, IAnimationContextualResponder
    {
        [SerializeField]
        private AnimationClip m_CandidateClip;
        [SerializeField]
        private AnimationModeDriver m_CandidateDriver;
        [NonSerialized]
        private List<UndoPropertyModification> m_Candidates;
        [SerializeField]
        private AnimationModeDriver m_Driver;
        [NonSerialized]
        private float m_PreviousUpdateTime;
        [SerializeField]
        private AnimationKeyTime m_Time;
        [NonSerialized]
        public AnimationWindowState state;

        public void AddAnimatedKeys()
        {
            AnimationWindowUtility.AddKeyframes(this.state, this.state.allCurves.ToArray(), this.time);
            this.ClearCandidates();
        }

        public void AddCandidateKeys()
        {
            this.ProcessCandidates();
        }

        public void AddKey(SerializedProperty property)
        {
            PropertyModification[] modifications = AnimationWindowUtility.SerializedPropertyToPropertyModifications(property, this.state.activeRootGameObject);
            if (modifications != null)
            {
                UndoPropertyModification[] modificationArray2 = new UndoPropertyModification[modifications.Length];
                for (int i = 0; i < modifications.Length; i++)
                {
                    PropertyModification modification = modifications[i];
                    modificationArray2[i].previousValue = modification;
                    modificationArray2[i].currentValue = modification;
                }
                RecordingState state = new RecordingState(this.state);
                AnimationRecording.Process(state, modificationArray2);
                this.RemoveFromCandidates(modifications);
                this.ResampleAnimation();
                this.state.Repaint();
            }
        }

        public bool CandidateExists(SerializedProperty property)
        {
            <CandidateExists>c__AnonStorey3 storey = new <CandidateExists>c__AnonStorey3();
            if (this.m_CandidateClip == null)
            {
                return false;
            }
            EditorCurveBinding[] array = AnimationWindowUtility.SerializedPropertyToEditorCurveBindings(property, this.state.activeRootGameObject, this.state.activeAnimationClip);
            if (array.Length == 0)
            {
                return false;
            }
            storey.candidateBindings = AnimationUtility.GetCurveBindings(this.m_CandidateClip);
            if (storey.candidateBindings.Length == 0)
            {
                return false;
            }
            return Array.Exists<EditorCurveBinding>(array, new Predicate<EditorCurveBinding>(storey.<>m__0));
        }

        private void ClearCandidates()
        {
            this.m_CandidateClip = null;
            this.StopCandidateRecording();
        }

        public bool CurveExists(SerializedProperty property)
        {
            <CurveExists>c__AnonStorey5 storey = new <CurveExists>c__AnonStorey5();
            EditorCurveBinding[] array = AnimationWindowUtility.SerializedPropertyToEditorCurveBindings(property, this.state.activeRootGameObject, this.state.activeAnimationClip);
            if (array.Length == 0)
            {
                return false;
            }
            storey.clipBindings = AnimationUtility.GetCurveBindings(this.state.activeAnimationClip);
            if (storey.clipBindings.Length == 0)
            {
                return false;
            }
            return Array.Exists<EditorCurveBinding>(array, new Predicate<EditorCurveBinding>(storey.<>m__0));
        }

        public override void EndScrubTime()
        {
        }

        private AnimationModeDriver GetAnimationModeDriver()
        {
            if (this.m_Driver == null)
            {
                this.m_Driver = ScriptableObject.CreateInstance<AnimationModeDriver>();
                this.m_Driver.name = "AnimationWindowDriver";
                this.m_Driver.isKeyCallback = (AnimationModeDriver.IsKeyCallback) Delegate.Combine(this.m_Driver.isKeyCallback, delegate (UnityEngine.Object target, string propertyPath) {
                    if (UnityEditor.AnimationMode.IsPropertyAnimated(target, propertyPath))
                    {
                        SerializedProperty property = new SerializedObject(target).FindProperty(propertyPath);
                        if (property != null)
                        {
                            return this.KeyExists(property);
                        }
                    }
                    return false;
                });
            }
            return this.m_Driver;
        }

        private AnimationModeDriver GetCandidateDriver()
        {
            if (this.m_CandidateDriver == null)
            {
                this.m_CandidateDriver = ScriptableObject.CreateInstance<AnimationModeDriver>();
                this.m_CandidateDriver.name = "AnimationWindowCandidateDriver";
            }
            return this.m_CandidateDriver;
        }

        private List<AnimationWindowKeyframe> GetKeys(SerializedProperty property)
        {
            List<AnimationWindowKeyframe> list = new List<AnimationWindowKeyframe>();
            EditorCurveBinding[] array = AnimationWindowUtility.SerializedPropertyToEditorCurveBindings(property, this.state.activeRootGameObject, this.state.activeAnimationClip);
            if (array.Length != 0)
            {
                for (int i = 0; i < this.state.allCurves.Count; i++)
                {
                    <GetKeys>c__AnonStorey2 storey = new <GetKeys>c__AnonStorey2 {
                        curve = this.state.allCurves[i]
                    };
                    if (Array.Exists<EditorCurveBinding>(array, new Predicate<EditorCurveBinding>(storey.<>m__0)))
                    {
                        int keyframeIndex = storey.curve.GetKeyframeIndex(this.state.time);
                        if (keyframeIndex >= 0)
                        {
                            list.Add(storey.curve.m_Keyframes[keyframeIndex]);
                        }
                    }
                }
            }
            return list;
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

        public void GoToNextKeyframe(SerializedProperty property)
        {
            EditorCurveBinding[] array = AnimationWindowUtility.SerializedPropertyToEditorCurveBindings(property, this.state.activeRootGameObject, this.state.activeAnimationClip);
            if (array.Length != 0)
            {
                List<AnimationWindowCurve> list = new List<AnimationWindowCurve>();
                for (int i = 0; i < this.state.allCurves.Count; i++)
                {
                    <GoToNextKeyframe>c__AnonStorey1 storey = new <GoToNextKeyframe>c__AnonStorey1 {
                        curve = this.state.allCurves[i]
                    };
                    if (Array.Exists<EditorCurveBinding>(array, new Predicate<EditorCurveBinding>(storey.<>m__0)))
                    {
                        list.Add(storey.curve);
                    }
                }
                float time = AnimationWindowUtility.GetNextKeyframeTime(list.ToArray(), this.time.time, this.state.clipFrameRate);
                this.SetCurrentTime(this.state.SnapToFrame(time, AnimationWindowState.SnapMode.SnapToClipFrame));
                this.state.Repaint();
            }
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

        public void GoToPreviousKeyframe(SerializedProperty property)
        {
            EditorCurveBinding[] array = AnimationWindowUtility.SerializedPropertyToEditorCurveBindings(property, this.state.activeRootGameObject, this.state.activeAnimationClip);
            if (array.Length != 0)
            {
                List<AnimationWindowCurve> list = new List<AnimationWindowCurve>();
                for (int i = 0; i < this.state.allCurves.Count; i++)
                {
                    <GoToPreviousKeyframe>c__AnonStorey0 storey = new <GoToPreviousKeyframe>c__AnonStorey0 {
                        curve = this.state.allCurves[i]
                    };
                    if (Array.Exists<EditorCurveBinding>(array, new Predicate<EditorCurveBinding>(storey.<>m__0)))
                    {
                        list.Add(storey.curve);
                    }
                }
                float time = AnimationWindowUtility.GetPreviousKeyframeTime(list.ToArray(), this.time.time, this.state.clipFrameRate);
                this.SetCurrentTime(this.state.SnapToFrame(time, AnimationWindowState.SnapMode.SnapToClipFrame));
                this.state.Repaint();
            }
        }

        public override void GoToTime(float time)
        {
            this.SetCurrentTime(time);
        }

        public bool HasAnyCandidates() => 
            (this.m_CandidateClip != null);

        public bool HasAnyCurves() => 
            (this.state.allCurves.Count > 0);

        public bool IsAnimatable(SerializedProperty property) => 
            AnimationWindowUtility.PropertyIsAnimatable(property, this.state.activeRootGameObject);

        public bool IsEditable(SerializedProperty property)
        {
            if (!this.state.selection.disabled)
            {
                if (!this.previewing)
                {
                    return false;
                }
                AnimationWindowSelectionItem selectedItem = this.state.selectedItem;
                if (selectedItem != null)
                {
                    SerializedObject serializedObject = property.serializedObject;
                    GameObject gameObject = null;
                    if (serializedObject.targetObject is Component)
                    {
                        gameObject = ((Component) serializedObject.targetObject).gameObject;
                    }
                    else if (serializedObject.targetObject is GameObject)
                    {
                        gameObject = (GameObject) serializedObject.targetObject;
                    }
                    if (gameObject != null)
                    {
                        Component closestAnimationPlayerComponentInParents = AnimationWindowUtility.GetClosestAnimationPlayerComponentInParents(gameObject.transform);
                        if (selectedItem.animationPlayer == closestAnimationPlayerComponentInParents)
                        {
                            return selectedItem.animationIsEditable;
                        }
                    }
                }
            }
            return false;
        }

        public bool KeyExists(SerializedProperty property) => 
            (this.GetKeys(property).Count > 0);

        public void OnDisable()
        {
            this.StopPreview();
            this.StopPlayback();
            if (UnityEditor.AnimationMode.InAnimationMode(this.GetAnimationModeDriver()))
            {
                UnityEditor.AnimationMode.StopAnimationMode(this.GetAnimationModeDriver());
            }
            AnimationPropertyContextualMenu.Instance.RemoveResponder(this);
        }

        public override void OnEnable()
        {
            base.OnEnable();
            AnimationPropertyContextualMenu.Instance.AddResponder(this);
        }

        public override void OnSelectionChanged()
        {
            if (this.state != null)
            {
                this.m_Time = AnimationKeyTime.Time(0f, this.state.frameRate);
            }
            this.StopPreview();
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
            if (!UnityEditor.AnimationMode.InAnimationMode(this.GetAnimationModeDriver()))
            {
                Undo.postprocessModifications = (Undo.PostprocessModifications) Delegate.Remove(Undo.postprocessModifications, new Undo.PostprocessModifications(this.PostprocessAnimationRecordingModifications));
                return modifications;
            }
            if (this.recording)
            {
                return this.ProcessAutoKey(modifications);
            }
            if (this.previewing)
            {
                return this.RegisterCandidates(modifications);
            }
            return modifications;
        }

        private UndoPropertyModification[] ProcessAutoKey(UndoPropertyModification[] modifications)
        {
            RecordingState state = new RecordingState(this.state);
            return AnimationRecording.Process(state, modifications);
        }

        private void ProcessCandidates()
        {
            if (this.m_CandidateClip != null)
            {
                EditorCurveBinding[] curveBindings = AnimationUtility.GetCurveBindings(this.m_CandidateClip);
                UndoPropertyModification[] modifications = new UndoPropertyModification[curveBindings.Length];
                for (int i = 0; i < curveBindings.Length; i++)
                {
                    EditorCurveBinding binding = curveBindings[i];
                    PropertyModification modification = AnimationUtility.EditorCurveBindingToPropertyModification(binding, this.state.activeRootGameObject);
                    if (binding.isPPtrCurve)
                    {
                        ObjectReferenceKeyframe[] objectReferenceCurve = AnimationUtility.GetObjectReferenceCurve(this.m_CandidateClip, binding);
                        modification.value = string.Empty;
                        modification.objectReference = objectReferenceCurve[0].value;
                    }
                    else
                    {
                        AnimationCurve editorCurve = AnimationUtility.GetEditorCurve(this.m_CandidateClip, binding);
                        Keyframe keyframe = editorCurve[0];
                        modification.value = keyframe.value.ToString();
                    }
                    modifications[i].previousValue = modification;
                    modifications[i].currentValue = modification;
                }
                RecordingState state = new RecordingState(this.state);
                AnimationRecording.Process(state, modifications);
                this.ClearCandidates();
            }
        }

        private UndoPropertyModification[] RegisterCandidates(UndoPropertyModification[] modifications)
        {
            bool flag = this.m_CandidateClip == null;
            if (flag)
            {
                this.m_CandidateClip = new AnimationClip();
                this.m_CandidateClip.legacy = this.state.activeAnimationClip.legacy;
                this.StartCandidateRecording();
            }
            CandidateRecordingState state = new CandidateRecordingState(this.state, this.m_CandidateClip);
            UndoPropertyModification[] modificationArray = AnimationRecording.Process(state, modifications);
            if (flag && (modificationArray.Length == modifications.Length))
            {
                this.ClearCandidates();
            }
            InspectorWindow.RepaintAllInspectors();
            return modificationArray;
        }

        public void RemoveCurve(SerializedProperty property)
        {
            PropertyModification[] modifications = AnimationWindowUtility.SerializedPropertyToPropertyModifications(property, this.state.activeRootGameObject);
            if (modifications != null)
            {
                EditorCurveBinding[] bindingArray = AnimationWindowUtility.PropertyModificationsToEditorCurveBindings(modifications, this.state.activeRootGameObject, this.state.activeAnimationClip);
                if (bindingArray.Length != 0)
                {
                    Undo.RegisterCompleteObjectUndo(this.state.activeAnimationClip, "Remove Curve");
                    for (int i = 0; i < bindingArray.Length; i++)
                    {
                        EditorCurveBinding binding = bindingArray[i];
                        if (binding.isPPtrCurve)
                        {
                            AnimationUtility.SetObjectReferenceCurve(this.state.activeAnimationClip, binding, null);
                        }
                        else
                        {
                            AnimationUtility.SetEditorCurve(this.state.activeAnimationClip, binding, null);
                        }
                    }
                    this.RemoveFromCandidates(modifications);
                    this.ResampleAnimation();
                    this.state.Repaint();
                }
            }
        }

        private void RemoveFromCandidates(SerializedProperty property)
        {
            if (this.m_CandidateClip != null)
            {
                PropertyModification[] modifications = AnimationWindowUtility.SerializedPropertyToPropertyModifications(property, this.state.activeRootGameObject);
                if (modifications != null)
                {
                    this.RemoveFromCandidates(modifications);
                }
            }
        }

        private void RemoveFromCandidates(PropertyModification[] modifications)
        {
            if (this.m_CandidateClip != null)
            {
                EditorCurveBinding[] bindingArray = AnimationWindowUtility.PropertyModificationsToEditorCurveBindings(modifications, this.state.activeRootGameObject, this.m_CandidateClip);
                if (bindingArray.Length != 0)
                {
                    Undo.RegisterCompleteObjectUndo(this.m_CandidateClip, "Edit Candidate Curve");
                    for (int i = 0; i < bindingArray.Length; i++)
                    {
                        EditorCurveBinding binding = bindingArray[i];
                        if (binding.isPPtrCurve)
                        {
                            AnimationUtility.SetObjectReferenceCurve(this.m_CandidateClip, binding, null);
                        }
                        else
                        {
                            AnimationUtility.SetEditorCurve(this.m_CandidateClip, binding, null);
                        }
                    }
                    if (AnimationUtility.GetCurveBindings(this.m_CandidateClip).Length == 0)
                    {
                        this.m_CandidateClip = null;
                    }
                }
            }
        }

        public void RemoveKey(SerializedProperty property)
        {
            List<AnimationWindowKeyframe> keys = this.GetKeys(property);
            this.state.DeleteKeys(keys);
            this.RemoveFromCandidates(property);
            this.ResampleAnimation();
            this.state.Repaint();
        }

        public override void ResampleAnimation()
        {
            if ((!this.state.disabled && this.previewing) && this.canPreview)
            {
                bool flag = false;
                UnityEditor.AnimationMode.BeginSampling();
                foreach (AnimationWindowSelectionItem item in this.state.selection.ToArray())
                {
                    if (item.animationClip != null)
                    {
                        Undo.FlushUndoRecordObjects();
                        UnityEditor.AnimationMode.SampleAnimationClip(item.rootGameObject, item.animationClip, this.time.time - item.timeOffset);
                        if (this.m_CandidateClip != null)
                        {
                            UnityEditor.AnimationMode.SampleCandidateClip(item.rootGameObject, this.m_CandidateClip, 0f);
                        }
                        flag = true;
                    }
                }
                UnityEditor.AnimationMode.EndSampling();
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
                this.StartPreview();
                this.ClearCandidates();
                this.ResampleAnimation();
            }
        }

        private void SetCurrentTime(float value)
        {
            if (!Mathf.Approximately(value, this.time.time))
            {
                this.m_Time = AnimationKeyTime.Time(value, this.state.frameRate);
                this.StartPreview();
                this.ClearCandidates();
                this.ResampleAnimation();
            }
        }

        private void SnapTimeToFrame()
        {
            float num = this.state.FrameToTime((float) this.time.frame);
            this.SetCurrentTime(num);
        }

        private void StartCandidateRecording()
        {
            UnityEditor.AnimationMode.StartCandidateRecording(this.GetCandidateDriver());
        }

        public override bool StartPlayback()
        {
            if (!this.canPlay)
            {
                return false;
            }
            if (!UnityEditor.AnimationMode.InAnimationPlaybackMode())
            {
                UnityEditor.AnimationMode.StartAnimationPlaybackMode();
                this.m_PreviousUpdateTime = Time.realtimeSinceStartup;
                this.StartPreview();
                this.ClearCandidates();
            }
            return true;
        }

        public override bool StartPreview()
        {
            if (!this.canPreview)
            {
                return false;
            }
            if (!this.previewing)
            {
                UnityEditor.AnimationMode.StartAnimationMode(this.GetAnimationModeDriver());
                Undo.postprocessModifications = (Undo.PostprocessModifications) Delegate.Combine(Undo.postprocessModifications, new Undo.PostprocessModifications(this.PostprocessAnimationRecordingModifications));
            }
            return true;
        }

        private bool StartRecording()
        {
            if (this.canRecord)
            {
                if (this.recording)
                {
                    return true;
                }
                if (this.StartPreview())
                {
                    UnityEditor.AnimationMode.StartAnimationRecording();
                    this.ClearCandidates();
                    return true;
                }
            }
            return false;
        }

        public override bool StartRecording(UnityEngine.Object targetObject) => 
            this.StartRecording();

        public override void StartScrubTime()
        {
        }

        private void StopCandidateRecording()
        {
            UnityEditor.AnimationMode.StopCandidateRecording();
        }

        public override void StopPlayback()
        {
            if (UnityEditor.AnimationMode.InAnimationPlaybackMode())
            {
                UnityEditor.AnimationMode.StopAnimationPlaybackMode();
                this.SnapTimeToFrame();
            }
        }

        public override void StopPreview()
        {
            this.StopPlayback();
            this.StopRecording();
            this.ClearCandidates();
            UnityEditor.AnimationMode.StopAnimationMode(this.GetAnimationModeDriver());
            Undo.postprocessModifications = (Undo.PostprocessModifications) Delegate.Remove(Undo.postprocessModifications, new Undo.PostprocessModifications(this.PostprocessAnimationRecordingModifications));
        }

        public override void StopRecording()
        {
            if (this.recording)
            {
                UnityEditor.AnimationMode.StopAnimationRecording();
            }
        }

        public override bool canPlay =>
            this.canPreview;

        public override bool canPreview
        {
            get
            {
                if (!this.state.selection.canPreview)
                {
                    return false;
                }
                return (UnityEditor.AnimationMode.InAnimationMode(this.GetAnimationModeDriver()) || !UnityEditor.AnimationMode.InAnimationMode());
            }
        }

        public override bool canRecord
        {
            get
            {
                if (!this.state.selection.canRecord)
                {
                    return false;
                }
                return this.canPreview;
            }
        }

        public override bool playing =>
            (UnityEditor.AnimationMode.InAnimationPlaybackMode() && this.previewing);

        public override bool previewing =>
            UnityEditor.AnimationMode.InAnimationMode(this.GetAnimationModeDriver());

        public override bool recording =>
            (this.previewing && UnityEditor.AnimationMode.InAnimationRecording());

        public override AnimationKeyTime time =>
            this.m_Time;

        [CompilerGenerated]
        private sealed class <CandidateExists>c__AnonStorey3
        {
            internal EditorCurveBinding[] candidateBindings;

            internal bool <>m__0(EditorCurveBinding binding)
            {
                <CandidateExists>c__AnonStorey4 storey = new <CandidateExists>c__AnonStorey4 {
                    <>f__ref$3 = this,
                    binding = binding
                };
                return Array.Exists<EditorCurveBinding>(this.candidateBindings, new Predicate<EditorCurveBinding>(storey.<>m__0));
            }

            private sealed class <CandidateExists>c__AnonStorey4
            {
                internal AnimationWindowControl.<CandidateExists>c__AnonStorey3 <>f__ref$3;
                internal EditorCurveBinding binding;

                internal bool <>m__0(EditorCurveBinding candidateBinding) => 
                    candidateBinding.Equals(this.binding);
            }
        }

        [CompilerGenerated]
        private sealed class <CurveExists>c__AnonStorey5
        {
            internal EditorCurveBinding[] clipBindings;

            internal bool <>m__0(EditorCurveBinding binding)
            {
                <CurveExists>c__AnonStorey6 storey = new <CurveExists>c__AnonStorey6 {
                    <>f__ref$5 = this,
                    binding = binding
                };
                return Array.Exists<EditorCurveBinding>(this.clipBindings, new Predicate<EditorCurveBinding>(storey.<>m__0));
            }

            private sealed class <CurveExists>c__AnonStorey6
            {
                internal AnimationWindowControl.<CurveExists>c__AnonStorey5 <>f__ref$5;
                internal EditorCurveBinding binding;

                internal bool <>m__0(EditorCurveBinding clipBinding) => 
                    clipBinding.Equals(this.binding);
            }
        }

        [CompilerGenerated]
        private sealed class <GetKeys>c__AnonStorey2
        {
            internal AnimationWindowCurve curve;

            internal bool <>m__0(EditorCurveBinding binding) => 
                this.curve.binding.Equals(binding);
        }

        [CompilerGenerated]
        private sealed class <GoToNextKeyframe>c__AnonStorey1
        {
            internal AnimationWindowCurve curve;

            internal bool <>m__0(EditorCurveBinding binding) => 
                this.curve.binding.Equals(binding);
        }

        [CompilerGenerated]
        private sealed class <GoToPreviousKeyframe>c__AnonStorey0
        {
            internal AnimationWindowCurve curve;

            internal bool <>m__0(EditorCurveBinding binding) => 
                this.curve.binding.Equals(binding);
        }

        private class CandidateRecordingState : IAnimationRecordingState
        {
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private EditorCurveBinding[] <acceptedBindings>k__BackingField;
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private AnimationClip <activeAnimationClip>k__BackingField;
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private GameObject <activeGameObject>k__BackingField;
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private GameObject <activeRootGameObject>k__BackingField;

            public CandidateRecordingState(AnimationWindowState state, AnimationClip candidateClip)
            {
                this.activeGameObject = state.activeGameObject;
                this.activeRootGameObject = state.activeRootGameObject;
                this.activeAnimationClip = candidateClip;
                this.acceptedBindings = AnimationUtility.GetCurveBindings(state.activeAnimationClip);
            }

            public void AddPropertyModification(EditorCurveBinding binding, PropertyModification propertyModification, bool keepPrefabOverride)
            {
                UnityEditor.AnimationMode.AddCandidate(binding, propertyModification, keepPrefabOverride);
            }

            public void SaveCurve(AnimationWindowCurve curve)
            {
                Undo.RegisterCompleteObjectUndo(curve.clip, "Edit Candidate Curve");
                AnimationRecording.SaveModifiedCurve(curve, curve.clip);
            }

            public EditorCurveBinding[] acceptedBindings { get; private set; }

            public AnimationClip activeAnimationClip { get; private set; }

            public GameObject activeGameObject { get; private set; }

            public GameObject activeRootGameObject { get; private set; }

            public bool addZeroFrame =>
                false;

            public int currentFrame =>
                0;
        }

        private class RecordingState : IAnimationRecordingState
        {
            private AnimationWindowState m_State;

            public RecordingState(AnimationWindowState state)
            {
                this.m_State = state;
            }

            public void AddPropertyModification(EditorCurveBinding binding, PropertyModification propertyModification, bool keepPrefabOverride)
            {
                UnityEditor.AnimationMode.AddPropertyModification(binding, propertyModification, keepPrefabOverride);
            }

            public void SaveCurve(AnimationWindowCurve curve)
            {
                this.m_State.SaveCurve(curve);
            }

            public EditorCurveBinding[] acceptedBindings =>
                null;

            public AnimationClip activeAnimationClip =>
                this.m_State.activeAnimationClip;

            public GameObject activeGameObject =>
                this.m_State.activeGameObject;

            public GameObject activeRootGameObject =>
                this.m_State.activeRootGameObject;

            public bool addPropertyModification =>
                this.m_State.previewing;

            public bool addZeroFrame =>
                true;

            public int currentFrame =>
                this.m_State.currentFrame;
        }
    }
}

