namespace UnityEditorInternal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEditor.IMGUI.Controls;
    using UnityEngine;

    [Serializable]
    internal class AnimationWindowState : ScriptableObject, ICurveEditorState, IAnimationRecordingState
    {
        [SerializeField]
        public AnimEditor animEditor;
        [NonSerialized]
        public AnimationWindowHierarchyDataSource hierarchyData;
        [SerializeField]
        public AnimationWindowHierarchyState hierarchyState;
        public const float kDefaultFrameRate = 60f;
        public const string kEditCurveUndoLabel = "Edit Curve";
        private List<AnimationWindowCurve> m_ActiveCurvesCache;
        private List<CurveWrapper> m_ActiveCurveWrappersCache;
        private AnimationWindowKeyframe m_ActiveKeyframeCache;
        [SerializeField]
        private int m_ActiveKeyframeHash;
        [SerializeField]
        private int m_CurrentFrame;
        [SerializeField]
        private float m_CurrentTime;
        private List<DopeLine> m_dopelinesCache;
        [SerializeField]
        private float m_FrameRate = 60f;
        [SerializeField]
        private AnimationWindowKeySelection m_KeySelection;
        private EditorCurveBinding? m_lastAddedCurveBinding;
        private List<LiveEditCurve> m_LiveEditSnapshot;
        private HashSet<int> m_ModifiedCurves = new HashSet<int>();
        [SerializeField]
        private AnimationWindowPolicy m_Policy;
        private int m_PreviousRefreshHash;
        private AnimationRecordMode m_Recording;
        private RefreshType m_Refresh = RefreshType.None;
        private List<AnimationWindowKeyframe> m_SelectedKeysCache;
        [SerializeField]
        private AnimationWindowSelection m_Selection;
        [SerializeField]
        private TimeArea m_TimeArea;
        [SerializeField]
        private TimeArea.TimeFormat m_TimeFormat = TimeArea.TimeFormat.TimeFrame;
        [NonSerialized]
        public Action onEndLiveEdit;
        public Action<float> onFrameRateChange;
        [NonSerialized]
        public Action onStartLiveEdit;
        private static List<AnimationWindowKeyframe> s_KeyframeClipboard;
        [SerializeField]
        public bool showCurveEditor;

        public bool AnyKeyIsSelected(DopeLine dopeline)
        {
            foreach (AnimationWindowKeyframe keyframe in dopeline.keys)
            {
                if (this.KeyIsSelected(keyframe))
                {
                    return true;
                }
            }
            return false;
        }

        private void ClearCurveWrapperCache()
        {
            if (this.m_ActiveCurveWrappersCache != null)
            {
                for (int i = 0; i < this.m_ActiveCurveWrappersCache.Count; i++)
                {
                    CurveWrapper wrapper = this.m_ActiveCurveWrappersCache[i];
                    if (wrapper.renderer != null)
                    {
                        wrapper.renderer.FlushCache();
                    }
                }
                this.m_ActiveCurveWrappersCache = null;
            }
        }

        public void ClearHierarchySelection()
        {
            this.hierarchyState.selectedIDs.Clear();
            this.m_ActiveCurvesCache = null;
        }

        public void ClearKeySelections()
        {
            this.selectedKeyHashes.Clear();
            this.m_SelectedKeysCache = null;
        }

        public void ClearSelections()
        {
            this.ClearKeySelections();
            this.ClearHierarchySelection();
        }

        public void CopyAllActiveCurves()
        {
            foreach (AnimationWindowCurve curve in this.activeCurves)
            {
                foreach (AnimationWindowKeyframe keyframe in curve.m_Keyframes)
                {
                    s_KeyframeClipboard.Add(new AnimationWindowKeyframe(keyframe));
                }
            }
        }

        public void CopyKeys()
        {
            if (s_KeyframeClipboard == null)
            {
                s_KeyframeClipboard = new List<AnimationWindowKeyframe>();
            }
            float maxValue = float.MaxValue;
            s_KeyframeClipboard.Clear();
            foreach (AnimationWindowKeyframe keyframe in this.selectedKeys)
            {
                s_KeyframeClipboard.Add(new AnimationWindowKeyframe(keyframe));
                float num2 = keyframe.time + keyframe.curve.timeOffset;
                if (num2 < maxValue)
                {
                    maxValue = num2;
                }
            }
            if (s_KeyframeClipboard.Count > 0)
            {
                foreach (AnimationWindowKeyframe keyframe2 in s_KeyframeClipboard)
                {
                    keyframe2.time -= maxValue - keyframe2.curve.timeOffset;
                }
            }
            else
            {
                this.CopyAllActiveCurves();
            }
        }

        private void CurveWasModified(AnimationClip clip, EditorCurveBinding binding, AnimationUtility.CurveModifiedType type)
        {
            <CurveWasModified>c__AnonStorey0 storey = new <CurveWasModified>c__AnonStorey0 {
                clip = clip
            };
            AnimationWindowSelectionItem[] itemArray = Array.FindAll<AnimationWindowSelectionItem>(this.selection.ToArray(), new Predicate<AnimationWindowSelectionItem>(storey.<>m__0));
            if (itemArray.Length != 0)
            {
                if (type == AnimationUtility.CurveModifiedType.CurveModified)
                {
                    bool flag = false;
                    bool flag2 = false;
                    int hashCode = binding.GetHashCode();
                    foreach (AnimationWindowSelectionItem item in itemArray)
                    {
                        foreach (AnimationWindowCurve curve in item.curves)
                        {
                            if (curve.binding.GetHashCode() == hashCode)
                            {
                                this.m_ModifiedCurves.Add(curve.GetHashCode());
                                flag = true;
                                flag2 |= curve.binding.isPhantom;
                            }
                        }
                    }
                    if (flag && !flag2)
                    {
                        this.refresh = RefreshType.CurvesOnly;
                    }
                    else
                    {
                        this.m_lastAddedCurveBinding = new EditorCurveBinding?(binding);
                        this.refresh = RefreshType.Everything;
                    }
                }
                else
                {
                    this.refresh = RefreshType.Everything;
                }
            }
        }

        public void DeleteKeys(List<AnimationWindowKeyframe> keys)
        {
            this.SaveKeySelection("Edit Curve");
            foreach (AnimationWindowKeyframe keyframe in keys)
            {
                if (keyframe.curve.animationIsEditable)
                {
                    this.UnselectKey(keyframe);
                    keyframe.curve.m_Keyframes.Remove(keyframe);
                    this.SaveCurve(keyframe.curve, "Edit Curve");
                }
            }
            this.ResampleAnimation();
        }

        public void DeleteSelectedKeys()
        {
            if (this.selectedKeys.Count != 0)
            {
                this.DeleteKeys(this.selectedKeys);
            }
        }

        public void EndLiveEdit()
        {
            this.SaveSelectedKeys("Edit Curve");
            this.ResampleAnimation();
            this.m_LiveEditSnapshot = null;
            if (this.onEndLiveEdit != null)
            {
                this.onEndLiveEdit.Invoke();
            }
        }

        public void ForceRefresh()
        {
            this.refresh = RefreshType.Everything;
        }

        public string FormatFrame(int frame, int frameDigits)
        {
            int num = frame / ((int) this.frameRate);
            float num2 = ((float) frame) % this.frameRate;
            return (num.ToString() + ":" + num2.ToString().PadLeft(frameDigits, '0'));
        }

        public float FrameDeltaToPixel(Rect rect)
        {
            return (rect.width / this.visibleFrameSpan);
        }

        public float FrameToPixel(float i, Rect rect)
        {
            return (((i - this.minVisibleFrame) * rect.width) / this.visibleFrameSpan);
        }

        public float FrameToTime(float frame)
        {
            return (frame / this.frameRate);
        }

        public List<AnimationWindowCurve> GetAffectedCurves(List<AnimationWindowKeyframe> keyframes)
        {
            List<AnimationWindowCurve> list = new List<AnimationWindowCurve>();
            foreach (AnimationWindowKeyframe keyframe in keyframes)
            {
                if (!list.Contains(keyframe.curve))
                {
                    list.Add(keyframe.curve);
                }
            }
            return list;
        }

        public List<DopeLine> GetAffectedDopelines(List<AnimationWindowKeyframe> keyframes)
        {
            List<DopeLine> list = new List<DopeLine>();
            foreach (AnimationWindowCurve curve in this.GetAffectedCurves(keyframes))
            {
                foreach (DopeLine line in this.dopelines)
                {
                    if (!list.Contains(line) && Enumerable.Contains<AnimationWindowCurve>(line.curves, curve))
                    {
                        list.Add(line);
                    }
                }
            }
            return list;
        }

        public List<int> GetAffectedHierarchyIDs(List<AnimationWindowKeyframe> keyframes)
        {
            List<int> list = new List<int>();
            foreach (DopeLine line in this.GetAffectedDopelines(keyframes))
            {
                if (!list.Contains(line.hierarchyNodeID))
                {
                    list.Add(line.hierarchyNodeID);
                }
            }
            return list;
        }

        public List<AnimationWindowKeyframe> GetAggregateKeys(AnimationWindowHierarchyNode hierarchyNode)
        {
            <GetAggregateKeys>c__AnonStorey4 storey = new <GetAggregateKeys>c__AnonStorey4 {
                hierarchyNode = hierarchyNode
            };
            DopeLine line = Enumerable.FirstOrDefault<DopeLine>(this.dopelines, new Func<DopeLine, bool>(storey, (IntPtr) this.<>m__0));
            if (line == null)
            {
                return null;
            }
            return line.keys;
        }

        public DopeLine GetDopeline(int selectedInstanceID)
        {
            foreach (DopeLine line in this.dopelines)
            {
                if (line.hierarchyNodeID == selectedInstanceID)
                {
                    return line;
                }
            }
            return null;
        }

        private int GetRefreshHash()
        {
            return (((this.selection.GetRefreshHash() ^ ((this.hierarchyState == null) ? 0 : this.hierarchyState.expandedIDs.Count)) ^ ((this.hierarchyState == null) ? 0 : this.hierarchyState.GetTallInstancesCount())) ^ (!this.showCurveEditor ? 0 : 1));
        }

        public void HandleHierarchySelectionChanged(int[] selectedInstanceIDs, bool triggerSceneSelectionSync)
        {
            this.m_ActiveCurvesCache = null;
            if (triggerSceneSelectionSync)
            {
                this.SyncSceneSelection(selectedInstanceIDs);
            }
        }

        public bool InLiveEdit()
        {
            return (this.m_LiveEditSnapshot != null);
        }

        public bool KeyIsSelected(AnimationWindowKeyframe keyframe)
        {
            return this.selectedKeyHashes.Contains(keyframe.GetHash());
        }

        public void MoveSelectedKeys(float deltaTime, bool snapToFrame)
        {
            bool flag = this.InLiveEdit();
            if (!flag)
            {
                this.StartLiveEdit();
            }
            this.ClearKeySelections();
            foreach (LiveEditCurve curve in this.m_LiveEditSnapshot)
            {
                foreach (LiveEditKeyframe keyframe in curve.selectedKeys)
                {
                    if (curve.curve.animationIsEditable)
                    {
                        keyframe.key.time = Mathf.Max((float) (keyframe.keySnapshot.time + deltaTime), (float) 0f);
                        if (snapToFrame)
                        {
                            keyframe.key.time = this.SnapToFrame(keyframe.key.time, curve.curve.clip.frameRate);
                        }
                    }
                    this.SelectKey(keyframe.key);
                }
            }
            if (!flag)
            {
                this.EndLiveEdit();
            }
        }

        public void OnDestroy()
        {
            if (this.m_Selection != null)
            {
                this.m_Selection.Clear();
            }
            Object.DestroyImmediate(this.m_KeySelection);
        }

        public void OnDisable()
        {
            this.recording = false;
            this.playing = false;
            AnimationUtility.onCurveWasModified = (AnimationUtility.OnCurveWasModified) Delegate.Remove(AnimationUtility.onCurveWasModified, new AnimationUtility.OnCurveWasModified(this.CurveWasModified));
            Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
            if (this.m_Recording != null)
            {
                this.m_Recording.Dispose();
                this.m_Recording = null;
            }
        }

        public void OnEnable()
        {
            base.hideFlags = HideFlags.HideAndDontSave;
            AnimationUtility.onCurveWasModified = (AnimationUtility.OnCurveWasModified) Delegate.Combine(AnimationUtility.onCurveWasModified, new AnimationUtility.OnCurveWasModified(this.CurveWasModified));
            Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
            this.m_Recording = new AnimationRecordMode();
        }

        public void OnGUI()
        {
            this.RefreshHashCheck();
            this.Refresh();
        }

        public void OnHierarchySelectionChanged(int[] selectedInstanceIDs)
        {
            this.HandleHierarchySelectionChanged(selectedInstanceIDs, true);
        }

        private void OnNewCurveAdded(EditorCurveBinding newCurve)
        {
            string propertyGroupName = AnimationWindowUtility.GetPropertyGroupName(newCurve.propertyName);
            this.ClearHierarchySelection();
            if (this.hierarchyData != null)
            {
                foreach (AnimationWindowHierarchyNode node in this.hierarchyData.GetRows())
                {
                    if (((node.path == newCurve.path) && (node.animatableObjectType == newCurve.type)) && (node.propertyName == propertyGroupName))
                    {
                        this.SelectHierarchyItem(node.id, true, false);
                        if (newCurve.isPPtrCurve)
                        {
                            this.hierarchyState.AddTallInstance(node.id);
                        }
                    }
                }
                if (this.recording)
                {
                    this.ResampleAnimation();
                    InspectorWindow.RepaintAllInspectors();
                }
                this.m_lastAddedCurveBinding = null;
            }
        }

        public void OnSelectionChanged()
        {
            if (this.onFrameRateChange != null)
            {
                this.onFrameRateChange(this.frameRate);
            }
            if (this.recording)
            {
                this.ResampleAnimation();
            }
        }

        public void PasteKeys()
        {
            if (s_KeyframeClipboard == null)
            {
                s_KeyframeClipboard = new List<AnimationWindowKeyframe>();
            }
            this.SaveKeySelection("Edit Curve");
            HashSet<int> set = new HashSet<int>(this.selectedKeyHashes);
            this.ClearKeySelections();
            AnimationWindowCurve curve = null;
            AnimationWindowCurve curve2 = null;
            float startTime = 0f;
            List<AnimationWindowCurve> source = new List<AnimationWindowCurve>();
            foreach (AnimationWindowKeyframe keyframe in s_KeyframeClipboard)
            {
                if (!Enumerable.Any<AnimationWindowCurve>(source) || (Enumerable.Last<AnimationWindowCurve>(source) != keyframe.curve))
                {
                    source.Add(keyframe.curve);
                }
            }
            bool flag = Enumerable.Count<AnimationWindowCurve>(source) == Enumerable.Count<AnimationWindowCurve>(this.activeCurves);
            int num2 = 0;
            foreach (AnimationWindowKeyframe keyframe2 in s_KeyframeClipboard)
            {
                if ((curve2 != null) && (keyframe2.curve != curve2))
                {
                    num2++;
                }
                AnimationWindowKeyframe keyframe3 = new AnimationWindowKeyframe(keyframe2);
                if (flag)
                {
                    keyframe3.curve = this.activeCurves[num2];
                }
                else
                {
                    keyframe3.curve = AnimationWindowUtility.BestMatchForPaste(keyframe3.curve.binding, source, this.activeCurves);
                }
                if (keyframe3.curve == null)
                {
                    if (this.activeCurves.Count > 0)
                    {
                        AnimationWindowCurve curve3 = this.activeCurves[0];
                        if (curve3.animationIsEditable)
                        {
                            keyframe3.curve = new AnimationWindowCurve(curve3.clip, keyframe2.curve.binding, keyframe2.curve.valueType);
                            keyframe3.curve.selectionBinding = curve3.selectionBinding;
                            keyframe3.time = keyframe2.time;
                        }
                    }
                    else
                    {
                        AnimationWindowSelectionItem item = this.selection.First();
                        if (item.animationIsEditable)
                        {
                            keyframe3.curve = new AnimationWindowCurve(item.animationClip, keyframe2.curve.binding, keyframe2.curve.valueType);
                            keyframe3.curve.selectionBinding = item;
                            keyframe3.time = keyframe2.time;
                        }
                    }
                }
                if ((keyframe3.curve != null) && keyframe3.curve.animationIsEditable)
                {
                    keyframe3.time += this.time.time - keyframe3.curve.timeOffset;
                    if (((keyframe3.time >= 0f) && (keyframe3.curve != null)) && (keyframe3.curve.isPPtrCurve == keyframe2.curve.isPPtrCurve))
                    {
                        if (keyframe3.curve.HasKeyframe(AnimationKeyTime.Time(keyframe3.time, keyframe3.curve.clip.frameRate)))
                        {
                            keyframe3.curve.RemoveKeyframe(AnimationKeyTime.Time(keyframe3.time, keyframe3.curve.clip.frameRate));
                        }
                        if (curve == keyframe3.curve)
                        {
                            keyframe3.curve.RemoveKeysAtRange(startTime, keyframe3.time);
                        }
                        keyframe3.curve.m_Keyframes.Add(keyframe3);
                        this.SelectKey(keyframe3);
                        this.SaveCurve(keyframe3.curve, "Edit Curve");
                        curve = keyframe3.curve;
                        startTime = keyframe3.time;
                    }
                    curve2 = keyframe2.curve;
                }
            }
            if (this.selectedKeyHashes.Count == 0)
            {
                this.selectedKeyHashes = set;
            }
            else
            {
                this.ResampleAnimation();
            }
        }

        public float PixelDeltaToTime(Rect rect)
        {
            return (this.visibleTimeSpan / rect.width);
        }

        public float PixelToTime(float pixel)
        {
            return this.PixelToTime(pixel, SnapMode.Disabled);
        }

        public float PixelToTime(float pixel, SnapMode snap)
        {
            float num = pixel - this.zeroTimePixel;
            return this.SnapToFrame(num / this.pixelPerSecond, snap);
        }

        public float PixelToTime(float pixelX, Rect rect)
        {
            return (((pixelX * this.visibleTimeSpan) / rect.width) + this.minVisibleTime);
        }

        private UndoPropertyModification[] PostprocessAnimationRecordingModifications(UndoPropertyModification[] modifications)
        {
            if (!AnimationMode.InAnimationMode())
            {
                Undo.postprocessModifications = (Undo.PostprocessModifications) Delegate.Remove(Undo.postprocessModifications, new Undo.PostprocessModifications(this.PostprocessAnimationRecordingModifications));
                return modifications;
            }
            return AnimationRecording.Process(this, modifications);
        }

        private void Refresh()
        {
            this.selection.Synchronize();
            if (this.refresh == RefreshType.Everything)
            {
                this.selection.Refresh();
                this.m_ActiveKeyframeCache = null;
                this.m_ActiveCurvesCache = null;
                this.m_dopelinesCache = null;
                this.m_SelectedKeysCache = null;
                this.ClearCurveWrapperCache();
                if (this.hierarchyData != null)
                {
                    this.hierarchyData.UpdateData();
                }
                if (this.m_lastAddedCurveBinding.HasValue)
                {
                    this.OnNewCurveAdded(this.m_lastAddedCurveBinding.Value);
                }
                if ((this.activeCurves.Count == 0) && (this.dopelines.Count > 0))
                {
                    this.SelectHierarchyItem(this.dopelines[0], false, false);
                }
                this.m_Refresh = RefreshType.None;
            }
            else if (this.refresh == RefreshType.CurvesOnly)
            {
                this.m_ActiveKeyframeCache = null;
                this.m_SelectedKeysCache = null;
                this.ReloadModifiedAnimationCurveCache();
                this.ReloadModifiedDopelineCache();
                this.ReloadModifiedCurveWrapperCache();
                this.m_Refresh = RefreshType.None;
                this.m_ModifiedCurves.Clear();
            }
            if (this.selection.disabled && this.recording)
            {
                this.recording = false;
            }
        }

        private void RefreshHashCheck()
        {
            int refreshHash = this.GetRefreshHash();
            if (this.m_PreviousRefreshHash != refreshHash)
            {
                this.refresh = RefreshType.Everything;
                this.m_PreviousRefreshHash = refreshHash;
            }
        }

        private void ReloadModifiedAnimationCurveCache()
        {
            for (int i = 0; i < this.allCurves.Count; i++)
            {
                AnimationWindowCurve curve = this.allCurves[i];
                if (this.m_ModifiedCurves.Contains(curve.GetHashCode()))
                {
                    curve.LoadKeyframes(curve.clip);
                }
            }
        }

        private void ReloadModifiedCurveWrapperCache()
        {
            if (this.m_ActiveCurveWrappersCache != null)
            {
                Dictionary<int, AnimationWindowCurve> source = new Dictionary<int, AnimationWindowCurve>();
                for (int i = 0; i < this.m_ActiveCurveWrappersCache.Count; i++)
                {
                    <ReloadModifiedCurveWrapperCache>c__AnonStorey3 storey = new <ReloadModifiedCurveWrapperCache>c__AnonStorey3 {
                        curveWrapper = this.m_ActiveCurveWrappersCache[i]
                    };
                    if (this.m_ModifiedCurves.Contains(storey.curveWrapper.id))
                    {
                        AnimationWindowCurve curve = this.allCurves.Find(new Predicate<AnimationWindowCurve>(storey.<>m__0));
                        if (curve != null)
                        {
                            if ((curve.clip.startTime != storey.curveWrapper.renderer.RangeStart()) || (curve.clip.stopTime != storey.curveWrapper.renderer.RangeEnd()))
                            {
                                this.ClearCurveWrapperCache();
                                return;
                            }
                            source[i] = curve;
                        }
                    }
                }
                for (int j = 0; j < source.Count; j++)
                {
                    KeyValuePair<int, AnimationWindowCurve> pair = Enumerable.ElementAt<KeyValuePair<int, AnimationWindowCurve>>(source, j);
                    CurveWrapper wrapper = this.m_ActiveCurveWrappersCache[pair.Key];
                    if (wrapper.renderer != null)
                    {
                        wrapper.renderer.FlushCache();
                    }
                    this.m_ActiveCurveWrappersCache[pair.Key] = AnimationWindowUtility.GetCurveWrapper(pair.Value, pair.Value.clip);
                }
            }
        }

        private void ReloadModifiedDopelineCache()
        {
            if (this.m_dopelinesCache != null)
            {
                for (int i = 0; i < this.m_dopelinesCache.Count; i++)
                {
                    DopeLine line = this.m_dopelinesCache[i];
                    AnimationWindowCurve[] curves = line.curves;
                    for (int j = 0; j < curves.Length; j++)
                    {
                        if (this.m_ModifiedCurves.Contains(curves[j].GetHashCode()))
                        {
                            line.InvalidateKeyframes();
                            break;
                        }
                    }
                }
            }
        }

        public void RemoveCurve(AnimationWindowCurve curve, string undoLabel)
        {
            if (curve.animationIsEditable)
            {
                Undo.RegisterCompleteObjectUndo(curve.clip, undoLabel);
                if (curve.isPPtrCurve)
                {
                    AnimationUtility.SetObjectReferenceCurve(curve.clip, curve.binding, null);
                }
                else
                {
                    AnimationUtility.SetEditorCurve(curve.clip, curve.binding, null);
                }
            }
        }

        public void Repaint()
        {
            if (this.animEditor != null)
            {
                this.animEditor.Repaint();
            }
        }

        public void ResampleAnimation()
        {
            if ((!this.disabled && !this.animatorIsOptimized) && this.canRecord)
            {
                bool flag = false;
                foreach (AnimationWindowSelectionItem item in this.selection.ToArray())
                {
                    if (item.animationClip != null)
                    {
                        if (!this.recording)
                        {
                            this.recording = true;
                        }
                        Undo.FlushUndoRecordObjects();
                        AnimationMode.BeginSampling();
                        AnimationMode.SampleAnimationClip(item.rootGameObject, item.animationClip, this.currentTime - item.timeOffset);
                        AnimationMode.EndSampling();
                        flag = true;
                    }
                }
                if (flag)
                {
                    SceneView.RepaintAll();
                    ParticleSystemWindow instance = ParticleSystemWindow.GetInstance();
                    if (instance != null)
                    {
                        instance.Repaint();
                    }
                }
            }
        }

        public void SaveCurve(AnimationWindowCurve curve)
        {
            this.SaveCurve(curve, "Edit Curve");
        }

        public void SaveCurve(AnimationWindowCurve curve, string undoLabel)
        {
            if (!curve.animationIsEditable)
            {
                Debug.LogError("Curve is not editable and shouldn't be saved.");
            }
            Undo.RegisterCompleteObjectUndo(curve.clip, undoLabel);
            AnimationRecording.SaveModifiedCurve(curve, curve.clip);
            this.Repaint();
        }

        public void SaveKeySelection(string undoLabel)
        {
            if (this.m_KeySelection != null)
            {
                Undo.RegisterCompleteObjectUndo(this.m_KeySelection, undoLabel);
            }
        }

        private void SaveSelectedKeys(string undoLabel)
        {
            List<AnimationWindowCurve> list = new List<AnimationWindowCurve>();
            foreach (LiveEditCurve curve in this.m_LiveEditSnapshot)
            {
                if (curve.curve.animationIsEditable)
                {
                    if (!list.Contains(curve.curve))
                    {
                        list.Add(curve.curve);
                    }
                    List<AnimationWindowKeyframe> list2 = new List<AnimationWindowKeyframe>();
                    using (List<AnimationWindowKeyframe>.Enumerator enumerator2 = curve.curve.m_Keyframes.GetEnumerator())
                    {
                        while (enumerator2.MoveNext())
                        {
                            <SaveSelectedKeys>c__AnonStorey1 storey = new <SaveSelectedKeys>c__AnonStorey1 {
                                other = enumerator2.Current,
                                $this = this
                            };
                            if (!curve.selectedKeys.Exists(new Predicate<LiveEditKeyframe>(storey.<>m__0)) && curve.selectedKeys.Exists(new Predicate<LiveEditKeyframe>(storey.<>m__1)))
                            {
                                list2.Add(storey.other);
                            }
                        }
                    }
                    foreach (AnimationWindowKeyframe keyframe in list2)
                    {
                        curve.curve.m_Keyframes.Remove(keyframe);
                    }
                }
            }
            foreach (AnimationWindowCurve curve2 in list)
            {
                this.SaveCurve(curve2, undoLabel);
            }
        }

        public void SelectHierarchyItem(DopeLine dopeline, bool additive)
        {
            this.SelectHierarchyItem(dopeline.hierarchyNodeID, additive, true);
        }

        public void SelectHierarchyItem(int hierarchyNodeID, bool additive, bool triggerSceneSelectionSync)
        {
            if (!additive)
            {
                this.ClearHierarchySelection();
            }
            this.hierarchyState.selectedIDs.Add(hierarchyNodeID);
            int[] selectedInstanceIDs = this.hierarchyState.selectedIDs.ToArray();
            this.HandleHierarchySelectionChanged(selectedInstanceIDs, triggerSceneSelectionSync);
        }

        public void SelectHierarchyItem(DopeLine dopeline, bool additive, bool triggerSceneSelectionSync)
        {
            this.SelectHierarchyItem(dopeline.hierarchyNodeID, additive, triggerSceneSelectionSync);
        }

        public void SelectKey(AnimationWindowKeyframe keyframe)
        {
            int hash = keyframe.GetHash();
            if (!this.selectedKeyHashes.Contains(hash))
            {
                this.selectedKeyHashes.Add(hash);
            }
            this.m_SelectedKeysCache = null;
        }

        public void SelectKeysFromDopeline(DopeLine dopeline)
        {
            if (dopeline != null)
            {
                foreach (AnimationWindowKeyframe keyframe in dopeline.keys)
                {
                    this.SelectKey(keyframe);
                }
            }
        }

        public float SnapToFrame(float time, float fps)
        {
            return (Mathf.Round(time * fps) / fps);
        }

        public float SnapToFrame(float time, SnapMode snap)
        {
            if (snap == SnapMode.Disabled)
            {
                return time;
            }
            float fps = (snap != SnapMode.SnapToFrame) ? this.clipFrameRate : this.frameRate;
            return this.SnapToFrame(time, fps);
        }

        public void StartLiveEdit()
        {
            if (this.onStartLiveEdit != null)
            {
                this.onStartLiveEdit.Invoke();
            }
            this.m_LiveEditSnapshot = new List<LiveEditCurve>();
            this.SaveKeySelection("Edit Curve");
            using (List<AnimationWindowKeyframe>.Enumerator enumerator = this.selectedKeys.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    <StartLiveEdit>c__AnonStorey2 storey = new <StartLiveEdit>c__AnonStorey2 {
                        selectedKey = enumerator.Current
                    };
                    if (!this.m_LiveEditSnapshot.Exists(new Predicate<LiveEditCurve>(storey.<>m__0)))
                    {
                        LiveEditCurve item = new LiveEditCurve {
                            curve = storey.selectedKey.curve
                        };
                        foreach (AnimationWindowKeyframe keyframe in storey.selectedKey.curve.m_Keyframes)
                        {
                            LiveEditKeyframe keyframe2 = new LiveEditKeyframe {
                                keySnapshot = new AnimationWindowKeyframe(keyframe),
                                key = keyframe
                            };
                            if (this.KeyIsSelected(keyframe))
                            {
                                item.selectedKeys.Add(keyframe2);
                            }
                            else
                            {
                                item.unselectedKeys.Add(keyframe2);
                            }
                        }
                        this.m_LiveEditSnapshot.Add(item);
                    }
                }
            }
        }

        private void SyncSceneSelection(int[] selectedNodeIDs)
        {
            if ((this.selectedItem != null) && this.selectedItem.canSyncSceneSelection)
            {
                GameObject rootGameObject = this.selectedItem.rootGameObject;
                if (rootGameObject != null)
                {
                    List<int> list = new List<int>();
                    foreach (int num in selectedNodeIDs)
                    {
                        AnimationWindowHierarchyNode node = this.hierarchyData.FindItem(num) as AnimationWindowHierarchyNode;
                        if ((node != null) && !(node is AnimationWindowHierarchyMasterNode))
                        {
                            Transform tr = rootGameObject.transform.Find(node.path);
                            if (((tr != null) && (rootGameObject != null)) && (this.activeAnimationPlayer == AnimationWindowUtility.GetClosestAnimationPlayerComponentInParents(tr)))
                            {
                                list.Add(tr.gameObject.GetInstanceID());
                            }
                        }
                    }
                    Selection.instanceIDs = list.ToArray();
                }
            }
        }

        public float TimeToFrame(float time)
        {
            return (time * this.frameRate);
        }

        public int TimeToFrameFloor(float time)
        {
            return Mathf.FloorToInt(this.TimeToFrame(time));
        }

        public int TimeToFrameRound(float time)
        {
            return Mathf.RoundToInt(this.TimeToFrame(time));
        }

        public float TimeToPixel(float time)
        {
            return this.TimeToPixel(time, SnapMode.Disabled);
        }

        public float TimeToPixel(float time, SnapMode snap)
        {
            return ((this.SnapToFrame(time, snap) * this.pixelPerSecond) + this.zeroTimePixel);
        }

        public float TimeToPixel(float time, Rect rect)
        {
            return this.FrameToPixel(time * this.frameRate, rect);
        }

        public void TransformRippleKeys(Matrix4x4 matrix, float t1, float t2, bool flipX, bool snapToFrame)
        {
            bool flag = this.InLiveEdit();
            if (!flag)
            {
                this.StartLiveEdit();
            }
            this.ClearKeySelections();
            foreach (LiveEditCurve curve in this.m_LiveEditSnapshot)
            {
                foreach (LiveEditKeyframe keyframe in curve.selectedKeys)
                {
                    if (curve.curve.animationIsEditable)
                    {
                        Vector3 v = new Vector3(keyframe.keySnapshot.time, 0f, 0f);
                        v = matrix.MultiplyPoint3x4(v);
                        keyframe.key.time = Mathf.Max(!snapToFrame ? v.x : this.SnapToFrame(v.x, curve.curve.clip.frameRate), 0f);
                        if (flipX)
                        {
                            keyframe.key.inTangent = -keyframe.keySnapshot.outTangent;
                            keyframe.key.outTangent = -keyframe.keySnapshot.inTangent;
                        }
                    }
                    this.SelectKey(keyframe.key);
                }
                if (curve.curve.animationIsEditable)
                {
                    foreach (LiveEditKeyframe keyframe2 in curve.unselectedKeys)
                    {
                        if (keyframe2.keySnapshot.time > t2)
                        {
                            Vector3 vector2 = new Vector3(!flipX ? t2 : t1, 0f, 0f);
                            vector2 = matrix.MultiplyPoint3x4(vector2);
                            float num = vector2.x - t2;
                            if (num > 0f)
                            {
                                float time = keyframe2.keySnapshot.time + num;
                                keyframe2.key.time = Mathf.Max(!snapToFrame ? time : this.SnapToFrame(time, curve.curve.clip.frameRate), 0f);
                            }
                            else
                            {
                                keyframe2.key.time = keyframe2.keySnapshot.time;
                            }
                        }
                        else if (keyframe2.keySnapshot.time < t1)
                        {
                            Vector3 vector3 = new Vector3(!flipX ? t1 : t2, 0f, 0f);
                            vector3 = matrix.MultiplyPoint3x4(vector3);
                            float num3 = vector3.x - t1;
                            if (num3 < 0f)
                            {
                                float num4 = keyframe2.keySnapshot.time + num3;
                                keyframe2.key.time = Mathf.Max(!snapToFrame ? num4 : this.SnapToFrame(num4, curve.curve.clip.frameRate), 0f);
                            }
                            else
                            {
                                keyframe2.key.time = keyframe2.keySnapshot.time;
                            }
                        }
                    }
                }
            }
            if (!flag)
            {
                this.EndLiveEdit();
            }
        }

        public void TransformSelectedKeys(Matrix4x4 matrix, bool flipX, bool flipY, bool snapToFrame)
        {
            bool flag = this.InLiveEdit();
            if (!flag)
            {
                this.StartLiveEdit();
            }
            this.ClearKeySelections();
            foreach (LiveEditCurve curve in this.m_LiveEditSnapshot)
            {
                foreach (LiveEditKeyframe keyframe in curve.selectedKeys)
                {
                    if (curve.curve.animationIsEditable)
                    {
                        Vector3 v = new Vector3(keyframe.keySnapshot.time, !keyframe.keySnapshot.isPPtrCurve ? ((float) keyframe.keySnapshot.value) : 0f, 0f);
                        v = matrix.MultiplyPoint3x4(v);
                        keyframe.key.time = Mathf.Max(!snapToFrame ? v.x : this.SnapToFrame(v.x, curve.curve.clip.frameRate), 0f);
                        if (flipX)
                        {
                            keyframe.key.inTangent = -keyframe.keySnapshot.outTangent;
                            keyframe.key.outTangent = -keyframe.keySnapshot.inTangent;
                        }
                        if (!keyframe.key.isPPtrCurve)
                        {
                            keyframe.key.value = v.y;
                            if (flipY)
                            {
                                keyframe.key.inTangent = -keyframe.key.inTangent;
                                keyframe.key.outTangent = -keyframe.key.outTangent;
                            }
                        }
                    }
                    this.SelectKey(keyframe.key);
                }
            }
            if (!flag)
            {
                this.EndLiveEdit();
            }
        }

        public void UndoRedoPerformed()
        {
            this.refresh = RefreshType.Everything;
            if (this.recording)
            {
                this.ResampleAnimation();
            }
        }

        public void UnSelectHierarchyItem(int hierarchyNodeID)
        {
            this.hierarchyState.selectedIDs.Remove(hierarchyNodeID);
        }

        public void UnSelectHierarchyItem(DopeLine dopeline)
        {
            this.UnSelectHierarchyItem(dopeline.hierarchyNodeID);
        }

        public void UnselectKey(AnimationWindowKeyframe keyframe)
        {
            int hash = keyframe.GetHash();
            if (this.selectedKeyHashes.Contains(hash))
            {
                this.selectedKeyHashes.Remove(hash);
            }
            this.m_SelectedKeysCache = null;
        }

        public void UnselectKeysFromDopeline(DopeLine dopeline)
        {
            if (dopeline != null)
            {
                foreach (AnimationWindowKeyframe keyframe in dopeline.keys)
                {
                    this.UnselectKey(keyframe);
                }
            }
        }

        public AnimationClip activeAnimationClip
        {
            get
            {
                if (this.selectedItem != null)
                {
                    return this.selectedItem.animationClip;
                }
                return null;
            }
        }

        public Component activeAnimationPlayer
        {
            get
            {
                if (this.selectedItem != null)
                {
                    return this.selectedItem.animationPlayer;
                }
                return null;
            }
        }

        public List<AnimationWindowCurve> activeCurves
        {
            get
            {
                if (this.m_ActiveCurvesCache == null)
                {
                    this.m_ActiveCurvesCache = new List<AnimationWindowCurve>();
                    if ((this.hierarchyState != null) && (this.hierarchyData != null))
                    {
                        foreach (int num in this.hierarchyState.selectedIDs)
                        {
                            AnimationWindowHierarchyNode node = this.hierarchyData.FindItem(num) as AnimationWindowHierarchyNode;
                            if (node != null)
                            {
                                AnimationWindowCurve[] curves = node.curves;
                                if (curves != null)
                                {
                                    foreach (AnimationWindowCurve curve in curves)
                                    {
                                        if (!this.m_ActiveCurvesCache.Contains(curve))
                                        {
                                            this.m_ActiveCurvesCache.Add(curve);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return this.m_ActiveCurvesCache;
            }
        }

        public List<CurveWrapper> activeCurveWrappers
        {
            get
            {
                if ((this.m_ActiveCurveWrappersCache == null) || (this.m_ActiveCurvesCache == null))
                {
                    List<CurveWrapper> source = new List<CurveWrapper>();
                    foreach (AnimationWindowCurve curve in this.activeCurves)
                    {
                        if (!curve.isPPtrCurve)
                        {
                            source.Add(AnimationWindowUtility.GetCurveWrapper(curve, curve.clip));
                        }
                    }
                    if (!Enumerable.Any<CurveWrapper>(source))
                    {
                        foreach (AnimationWindowCurve curve2 in this.allCurves)
                        {
                            if (!curve2.isPPtrCurve)
                            {
                                source.Add(AnimationWindowUtility.GetCurveWrapper(curve2, curve2.clip));
                            }
                        }
                    }
                    this.m_ActiveCurveWrappersCache = source;
                }
                return this.m_ActiveCurveWrappersCache;
            }
        }

        public GameObject activeGameObject
        {
            get
            {
                if (this.selectedItem != null)
                {
                    return this.selectedItem.gameObject;
                }
                return null;
            }
        }

        public AnimationWindowKeyframe activeKeyframe
        {
            get
            {
                if (this.m_ActiveKeyframeCache == null)
                {
                    foreach (AnimationWindowCurve curve in this.allCurves)
                    {
                        foreach (AnimationWindowKeyframe keyframe in curve.m_Keyframes)
                        {
                            if (keyframe.GetHash() == this.m_ActiveKeyframeHash)
                            {
                                this.m_ActiveKeyframeCache = keyframe;
                            }
                        }
                    }
                }
                return this.m_ActiveKeyframeCache;
            }
            set
            {
                this.m_ActiveKeyframeCache = null;
                this.m_ActiveKeyframeHash = (value == null) ? 0 : value.GetHash();
            }
        }

        public GameObject activeRootGameObject
        {
            get
            {
                if (this.selectedItem != null)
                {
                    return this.selectedItem.rootGameObject;
                }
                return null;
            }
        }

        public List<AnimationWindowCurve> allCurves
        {
            get
            {
                return this.selection.curves;
            }
        }

        public bool animatorIsOptimized
        {
            get
            {
                if (this.activeRootGameObject == null)
                {
                    return false;
                }
                Animator component = this.activeRootGameObject.GetComponent<Animator>();
                return ((component != null) && (component.isOptimizable && !component.hasTransformHierarchy));
            }
        }

        public bool canRecord
        {
            get
            {
                if (!this.selection.canRecord)
                {
                    return false;
                }
                return ((this.m_Recording != null) && this.m_Recording.canEnable);
            }
        }

        public float clipFrameRate
        {
            get
            {
                if (this.activeAnimationClip == null)
                {
                    return 60f;
                }
                return this.activeAnimationClip.frameRate;
            }
            set
            {
                if (((this.activeAnimationClip != null) && (value > 0f)) && (value <= 10000f))
                {
                    this.ClearKeySelections();
                    this.SaveKeySelection("Edit Curve");
                    foreach (AnimationWindowCurve curve in this.allCurves)
                    {
                        foreach (AnimationWindowKeyframe keyframe in curve.m_Keyframes)
                        {
                            int frame = AnimationKeyTime.Time(keyframe.time, this.clipFrameRate).frame;
                            keyframe.time = AnimationKeyTime.Frame(frame, value).time;
                        }
                        this.SaveCurve(curve, "Edit Curve");
                    }
                    AnimationEvent[] animationEvents = AnimationUtility.GetAnimationEvents(this.activeAnimationClip);
                    foreach (AnimationEvent event2 in animationEvents)
                    {
                        int num3 = AnimationKeyTime.Time(event2.time, this.clipFrameRate).frame;
                        event2.time = AnimationKeyTime.Frame(num3, value).time;
                    }
                    AnimationUtility.SetAnimationEvents(this.activeAnimationClip, animationEvents);
                    this.activeAnimationClip.frameRate = value;
                }
            }
        }

        public float currentTime
        {
            get
            {
                return this.m_CurrentTime;
            }
            set
            {
                if (!Mathf.Approximately(this.m_CurrentTime, value))
                {
                    this.m_CurrentTime = Mathf.Max(value, 0f);
                    this.m_CurrentFrame = this.TimeToFrameFloor(this.m_CurrentTime);
                    this.ResampleAnimation();
                }
            }
        }

        public bool disabled
        {
            get
            {
                return this.selection.disabled;
            }
        }

        public List<DopeLine> dopelines
        {
            get
            {
                if (this.m_dopelinesCache == null)
                {
                    this.m_dopelinesCache = new List<DopeLine>();
                    if (this.hierarchyData != null)
                    {
                        foreach (TreeViewItem item in this.hierarchyData.GetRows())
                        {
                            AnimationWindowHierarchyNode node = item as AnimationWindowHierarchyNode;
                            if ((node != null) && !(node is AnimationWindowHierarchyAddButtonNode))
                            {
                                AnimationWindowCurve[] curves = node.curves;
                                if (curves != null)
                                {
                                    DopeLine line = new DopeLine(item.id, curves) {
                                        tallMode = this.hierarchyState.GetTallMode(node),
                                        objectType = node.animatableObjectType,
                                        hasChildren = !(node is AnimationWindowHierarchyPropertyNode),
                                        isMasterDopeline = item is AnimationWindowHierarchyMasterNode
                                    };
                                    this.m_dopelinesCache.Add(line);
                                }
                            }
                        }
                    }
                }
                return this.m_dopelinesCache;
            }
        }

        public int frame
        {
            get
            {
                return this.m_CurrentFrame;
            }
            set
            {
                if (this.m_CurrentFrame != value)
                {
                    this.m_CurrentFrame = Math.Max(value, 0);
                    this.m_CurrentTime = this.FrameToTime((float) this.m_CurrentFrame);
                    this.ResampleAnimation();
                }
            }
        }

        public float frameRate
        {
            get
            {
                return this.m_FrameRate;
            }
            set
            {
                if (this.m_FrameRate != value)
                {
                    this.m_FrameRate = value;
                    if (this.onFrameRateChange != null)
                    {
                        this.onFrameRateChange(this.m_FrameRate);
                    }
                }
            }
        }

        public bool locked
        {
            get
            {
                return this.selection.locked;
            }
            set
            {
                this.selection.locked = value;
            }
        }

        public float maxTime
        {
            get
            {
                return this.timeRange.y;
            }
        }

        public float maxVisibleFrame
        {
            get
            {
                return (this.maxVisibleTime * this.frameRate);
            }
        }

        public float maxVisibleTime
        {
            get
            {
                return this.m_TimeArea.shownArea.xMax;
            }
        }

        public float minTime
        {
            get
            {
                return this.timeRange.x;
            }
        }

        public float minVisibleFrame
        {
            get
            {
                return (this.minVisibleTime * this.frameRate);
            }
        }

        public float minVisibleTime
        {
            get
            {
                return this.m_TimeArea.shownArea.xMin;
            }
        }

        public float pixelPerSecond
        {
            get
            {
                return this.timeArea.m_Scale.x;
            }
        }

        public bool playing
        {
            get
            {
                return AnimationMode.InAnimationPlaybackMode();
            }
            set
            {
                if (value && !AnimationMode.InAnimationPlaybackMode())
                {
                    AnimationMode.StartAnimationPlaybackMode();
                    this.recording = true;
                }
                if (!value && AnimationMode.InAnimationPlaybackMode())
                {
                    AnimationMode.StopAnimationPlaybackMode();
                    this.currentTime = this.FrameToTime((float) this.frame);
                }
            }
        }

        public AnimationWindowPolicy policy
        {
            get
            {
                return this.m_Policy;
            }
            set
            {
                this.m_Policy = value;
            }
        }

        public bool recording
        {
            get
            {
                return ((this.m_Recording != null) && this.m_Recording.enable);
            }
            set
            {
                if (this.canRecord && (this.m_Recording != null))
                {
                    bool enable = this.m_Recording.enable;
                    this.m_Recording.enable = value;
                    bool flag2 = this.m_Recording.enable;
                    if (enable != flag2)
                    {
                        if (flag2)
                        {
                            Undo.postprocessModifications = (Undo.PostprocessModifications) Delegate.Combine(Undo.postprocessModifications, new Undo.PostprocessModifications(this.PostprocessAnimationRecordingModifications));
                        }
                        else
                        {
                            Undo.postprocessModifications = (Undo.PostprocessModifications) Delegate.Remove(Undo.postprocessModifications, new Undo.PostprocessModifications(this.PostprocessAnimationRecordingModifications));
                        }
                    }
                }
            }
        }

        public RefreshType refresh
        {
            get
            {
                return this.m_Refresh;
            }
            set
            {
                if (this.m_Refresh < value)
                {
                    this.m_Refresh = value;
                }
            }
        }

        public List<AnimationWindowHierarchyNode> selectedHierarchyNodes
        {
            get
            {
                List<AnimationWindowHierarchyNode> list = new List<AnimationWindowHierarchyNode>();
                if ((this.activeAnimationClip != null) && (this.hierarchyData != null))
                {
                    foreach (int num in this.hierarchyState.selectedIDs)
                    {
                        AnimationWindowHierarchyNode item = (AnimationWindowHierarchyNode) this.hierarchyData.FindItem(num);
                        if ((item != null) && !(item is AnimationWindowHierarchyAddButtonNode))
                        {
                            list.Add(item);
                        }
                    }
                }
                return list;
            }
        }

        public AnimationWindowSelectionItem selectedItem
        {
            get
            {
                if ((this.m_Selection != null) && (this.m_Selection.count > 0))
                {
                    return this.m_Selection.First();
                }
                return null;
            }
            set
            {
                if (this.m_Selection == null)
                {
                    this.m_Selection = new AnimationWindowSelection();
                }
                if (value == null)
                {
                    this.m_Selection.Clear();
                }
                else
                {
                    this.m_Selection.Set(value);
                }
            }
        }

        private HashSet<int> selectedKeyHashes
        {
            get
            {
                if (this.m_KeySelection == null)
                {
                    this.m_KeySelection = ScriptableObject.CreateInstance<AnimationWindowKeySelection>();
                    this.m_KeySelection.hideFlags = HideFlags.HideAndDontSave;
                }
                return this.m_KeySelection.selectedKeyHashes;
            }
            set
            {
                if (this.m_KeySelection == null)
                {
                    this.m_KeySelection = ScriptableObject.CreateInstance<AnimationWindowKeySelection>();
                    this.m_KeySelection.hideFlags = HideFlags.HideAndDontSave;
                }
                this.m_KeySelection.selectedKeyHashes = value;
            }
        }

        public List<AnimationWindowKeyframe> selectedKeys
        {
            get
            {
                if (this.m_SelectedKeysCache == null)
                {
                    this.m_SelectedKeysCache = new List<AnimationWindowKeyframe>();
                    foreach (AnimationWindowCurve curve in this.allCurves)
                    {
                        foreach (AnimationWindowKeyframe keyframe in curve.m_Keyframes)
                        {
                            if (this.KeyIsSelected(keyframe))
                            {
                                this.m_SelectedKeysCache.Add(keyframe);
                            }
                        }
                    }
                }
                return this.m_SelectedKeysCache;
            }
        }

        public AnimationWindowSelection selection
        {
            get
            {
                if (this.m_Selection == null)
                {
                    this.m_Selection = new AnimationWindowSelection();
                }
                return this.m_Selection;
            }
        }

        public bool syncTimeDuringDrag
        {
            get
            {
                return false;
            }
        }

        public AnimationKeyTime time
        {
            get
            {
                return AnimationKeyTime.Frame(this.frame, this.frameRate);
            }
        }

        public TimeArea timeArea
        {
            get
            {
                return this.m_TimeArea;
            }
            set
            {
                this.m_TimeArea = value;
            }
        }

        public TimeArea.TimeFormat timeFormat
        {
            get
            {
                return this.m_TimeFormat;
            }
            set
            {
                this.m_TimeFormat = value;
            }
        }

        public Vector2 timeRange
        {
            get
            {
                float a = 0f;
                float minValue = 0f;
                if (this.selection.count > 0)
                {
                    a = float.MaxValue;
                    minValue = float.MinValue;
                    foreach (AnimationWindowSelectionItem item in this.selection.ToArray())
                    {
                        a = Mathf.Min(a, item.animationClip.startTime + item.timeOffset);
                        minValue = Mathf.Max(minValue, item.animationClip.stopTime + item.timeOffset);
                    }
                }
                return new Vector2(a, minValue);
            }
        }

        public float visibleFrameSpan
        {
            get
            {
                return (this.visibleTimeSpan * this.frameRate);
            }
        }

        public float visibleTimeSpan
        {
            get
            {
                return (this.maxVisibleTime - this.minVisibleTime);
            }
        }

        public float zeroTimePixel
        {
            get
            {
                return ((this.timeArea.shownArea.xMin * this.timeArea.m_Scale.x) * -1f);
            }
        }

        [CompilerGenerated]
        private sealed class <CurveWasModified>c__AnonStorey0
        {
            internal AnimationClip clip;

            internal bool <>m__0(AnimationWindowSelectionItem item)
            {
                return (item.animationClip == this.clip);
            }
        }

        [CompilerGenerated]
        private sealed class <GetAggregateKeys>c__AnonStorey4
        {
            internal AnimationWindowHierarchyNode hierarchyNode;

            internal bool <>m__0(DopeLine e)
            {
                return (e.hierarchyNodeID == this.hierarchyNode.id);
            }
        }

        [CompilerGenerated]
        private sealed class <ReloadModifiedCurveWrapperCache>c__AnonStorey3
        {
            internal CurveWrapper curveWrapper;

            internal bool <>m__0(AnimationWindowCurve c)
            {
                return (c.GetHashCode() == this.curveWrapper.id);
            }
        }

        [CompilerGenerated]
        private sealed class <SaveSelectedKeys>c__AnonStorey1
        {
            internal AnimationWindowState $this;
            internal AnimationWindowKeyframe other;

            internal bool <>m__0(AnimationWindowState.LiveEditKeyframe liveEditKey)
            {
                return (liveEditKey.key == this.other);
            }

            internal bool <>m__1(AnimationWindowState.LiveEditKeyframe liveEditKey)
            {
                return (AnimationKeyTime.Time(liveEditKey.key.time, this.$this.frameRate).frame == AnimationKeyTime.Time(this.other.time, this.$this.frameRate).frame);
            }
        }

        [CompilerGenerated]
        private sealed class <StartLiveEdit>c__AnonStorey2
        {
            internal AnimationWindowKeyframe selectedKey;

            internal bool <>m__0(AnimationWindowState.LiveEditCurve snapshot)
            {
                return (snapshot.curve == this.selectedKey.curve);
            }
        }

        private class LiveEditCurve
        {
            public AnimationWindowCurve curve;
            public List<AnimationWindowState.LiveEditKeyframe> selectedKeys = new List<AnimationWindowState.LiveEditKeyframe>();
            public List<AnimationWindowState.LiveEditKeyframe> unselectedKeys = new List<AnimationWindowState.LiveEditKeyframe>();
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct LiveEditKeyframe
        {
            public AnimationWindowKeyframe keySnapshot;
            public AnimationWindowKeyframe key;
        }

        public enum RefreshType
        {
            None,
            CurvesOnly,
            Everything
        }

        public enum SnapMode
        {
            Disabled,
            SnapToFrame,
            SnapToClipFrame
        }
    }
}

