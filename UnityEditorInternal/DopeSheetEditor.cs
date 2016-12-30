namespace UnityEditorInternal
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEngine;

    [Serializable]
    internal class DopeSheetEditor : TimeArea, CurveUpdater
    {
        [CompilerGenerated]
        private static Comparison<Object> <>f__am$cache0;
        private const float k_KeyframeOffset = -6.5f;
        private const float k_PptrKeyframeOffset = -1f;
        private const int kLabelMarginHorizontal = 8;
        private const int kLabelMarginVertical = 2;
        public Bounds m_Bounds;
        private Texture m_DefaultDopeKeyIcon;
        private float m_DragStartTime;
        private bool m_Initialized;
        private bool m_IsDragging;
        private bool m_IsDraggingPlayhead;
        private bool m_IsDraggingPlayheadStarted;
        private bool m_MousedownOnKeyframe;
        [SerializeField]
        public EditorWindow m_Owner;
        private DopeSheetControlPointRenderer m_PointRenderer;
        private DopeSheetEditorRectangleTool m_RectangleTool;
        private DopeSheetSelectionRect m_SelectionRect;
        private int m_SpritePreviewCacheSize;
        private bool m_SpritePreviewLoading;
        public AnimationWindowState state;

        public DopeSheetEditor(EditorWindow owner) : base(false)
        {
            this.m_Bounds = new Bounds(Vector3.zero, Vector3.zero);
            this.m_Owner = owner;
        }

        private void AddKeyToDopeline(object obj)
        {
            this.AddKeyToDopeline((AddKeyToDopelineContext) obj);
        }

        private void AddKeyToDopeline(AddKeyToDopelineContext context)
        {
            AnimationWindowUtility.AddKeyframes(this.state, context.dopeline.curves.ToArray<AnimationWindowCurve>(), context.time);
        }

        private bool AnyKeyIsSelectedAtTime(DopeLine dopeLine, int keyIndex)
        {
            AnimationWindowKeyframe keyframe = dopeLine.keys[keyIndex];
            int num = keyframe.m_TimeHash ^ keyframe.curve.timeOffset.GetHashCode();
            int count = dopeLine.keys.Count;
            for (int i = keyIndex; i < count; i++)
            {
                keyframe = dopeLine.keys[i];
                int num5 = keyframe.m_TimeHash ^ keyframe.curve.timeOffset.GetHashCode();
                if (num5 != num)
                {
                    return false;
                }
                if (this.state.KeyIsSelected(keyframe))
                {
                    return true;
                }
            }
            return false;
        }

        private EditorCurveBinding? CreateNewPptrDopeline(AnimationWindowSelectionItem selectedItem, Type valueType)
        {
            List<EditorCurveBinding> animatableProperties = null;
            if (selectedItem.rootGameObject != null)
            {
                animatableProperties = AnimationWindowUtility.GetAnimatableProperties(selectedItem.rootGameObject, selectedItem.rootGameObject, valueType);
                if ((animatableProperties.Count == 0) && (valueType == typeof(Sprite)))
                {
                    return this.CreateNewSpriteRendererDopeline(selectedItem.rootGameObject, selectedItem.rootGameObject);
                }
            }
            else if (selectedItem.scriptableObject != null)
            {
                animatableProperties = AnimationWindowUtility.GetAnimatableProperties(selectedItem.scriptableObject, valueType);
            }
            if ((animatableProperties != null) && (animatableProperties.Count != 0))
            {
                if (animatableProperties.Count == 1)
                {
                    return new EditorCurveBinding?(animatableProperties[0]);
                }
                List<string> list2 = new List<string>();
                foreach (EditorCurveBinding binding in animatableProperties)
                {
                    list2.Add(binding.type.Name);
                }
                List<object> userData = new List<object> {
                    selectedItem.animationClip,
                    animatableProperties
                };
                Rect position = new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 1f, 1f);
                EditorUtility.DisplayCustomMenu(position, EditorGUIUtility.TempContent(list2.ToArray()), -1, new EditorUtility.SelectMenuItemFunction(this.SelectTypeForCreatingNewPptrDopeline), userData);
            }
            return null;
        }

        private void CreateNewPPtrKeyframe(float time, Object value, AnimationWindowCurve targetCurve)
        {
            ObjectReferenceKeyframe key = new ObjectReferenceKeyframe {
                time = time,
                value = value
            };
            AnimationWindowKeyframe keyframe2 = new AnimationWindowKeyframe(targetCurve, key);
            AnimationKeyTime keyTime = AnimationKeyTime.Time(keyframe2.time, this.state.frameRate);
            targetCurve.AddKeyframe(keyframe2, keyTime);
            this.state.SelectKey(keyframe2);
        }

        private EditorCurveBinding? CreateNewSpriteRendererDopeline(GameObject targetGameObject, GameObject rootGameObject)
        {
            if (targetGameObject.GetComponent<SpriteRenderer>() == null)
            {
                targetGameObject.AddComponent<SpriteRenderer>();
            }
            List<EditorCurveBinding> list = AnimationWindowUtility.GetAnimatableProperties(targetGameObject, rootGameObject, typeof(SpriteRenderer), typeof(Sprite));
            if (list.Count == 1)
            {
                return new EditorCurveBinding?(list[0]);
            }
            Debug.LogError("Unable to create animatable SpriteRenderer component");
            return null;
        }

        private void DeleteKeys(List<AnimationWindowKeyframe> keys)
        {
            this.state.DeleteKeys(keys);
        }

        private void DeleteKeys(object obj)
        {
            this.DeleteKeys((List<AnimationWindowKeyframe>) obj);
        }

        private bool DoDragAndDrop(DopeLine dopeLine, Rect position, bool perform)
        {
            if (!position.Contains(Event.current.mousePosition))
            {
                return false;
            }
            if (!ValidateDragAndDropObjects())
            {
                return false;
            }
            Type type = DragAndDrop.objectReferences[0].GetType();
            AnimationWindowCurve curve = null;
            if (dopeLine.valueType == type)
            {
                curve = dopeLine.curves[0];
            }
            else
            {
                foreach (AnimationWindowCurve curve2 in dopeLine.curves)
                {
                    if (curve2.isPPtrCurve)
                    {
                        if (curve2.valueType == type)
                        {
                            curve = curve2;
                        }
                        List<Sprite> list = SpriteUtility.GetSpriteFromPathsOrObjects(DragAndDrop.objectReferences, DragAndDrop.paths, Event.current.type);
                        if ((curve2.valueType == typeof(Sprite)) && (list.Count > 0))
                        {
                            curve = curve2;
                            type = typeof(Sprite);
                        }
                    }
                }
            }
            if (curve == null)
            {
                return false;
            }
            if (!curve.clipIsEditable)
            {
                return false;
            }
            if (perform)
            {
                if (DragAndDrop.objectReferences.Length == 1)
                {
                    UsabilityAnalytics.Event("Sprite Drag and Drop", "Drop single sprite into existing dopeline", "null", 1);
                }
                else
                {
                    UsabilityAnalytics.Event("Sprite Drag and Drop", "Drop multiple sprites into existing dopeline", "null", 1);
                }
                float time = Mathf.Max(this.state.PixelToTime(Event.current.mousePosition.x, AnimationWindowState.SnapMode.SnapToClipFrame), 0f);
                AnimationWindowCurve curveOfType = this.GetCurveOfType(dopeLine, type);
                this.PerformDragAndDrop(curveOfType, time);
            }
            return true;
        }

        private bool DopelineForValueTypeExists(Type valueType)
        {
            <DopelineForValueTypeExists>c__AnonStorey0 storey = new <DopelineForValueTypeExists>c__AnonStorey0 {
                valueType = valueType
            };
            return this.state.allCurves.Exists(new Predicate<AnimationWindowCurve>(storey.<>m__0));
        }

        private void DopeLineRepaint(DopeLine dopeline)
        {
            Color color = GUI.color;
            AnimationWindowHierarchyNode node = (AnimationWindowHierarchyNode) this.state.hierarchyData.FindItem(dopeline.hierarchyNodeID);
            Color color2 = ((node == null) || (node.depth <= 0)) ? Color.gray.AlphaMultiplied(0.16f) : Color.gray.AlphaMultiplied(0.05f);
            if (!dopeline.isMasterDopeline)
            {
                DrawBox(dopeline.position, color2);
            }
            int? nullable = null;
            int count = dopeline.keys.Count;
            for (int i = 0; i < count; i++)
            {
                AnimationWindowKeyframe keyframe = dopeline.keys[i];
                int num3 = keyframe.m_TimeHash ^ keyframe.curve.timeOffset.GetHashCode();
                if ((nullable.GetValueOrDefault() != num3) || !nullable.HasValue)
                {
                    nullable = new int?(num3);
                    Rect keyframeRect = this.GetKeyframeRect(dopeline, keyframe);
                    color2 = !dopeline.isMasterDopeline ? Color.gray.RGBMultiplied((float) 1.2f) : Color.gray.RGBMultiplied((float) 0.85f);
                    Texture2D texture = null;
                    if (keyframe.isPPtrCurve && dopeline.tallMode)
                    {
                        texture = (keyframe.value != null) ? AssetPreview.GetAssetPreview(((Object) keyframe.value).GetInstanceID(), this.assetPreviewManagerID) : null;
                    }
                    if (texture != null)
                    {
                        keyframeRect = this.GetPreviewRectFromKeyFrameRect(keyframeRect);
                        color2 = Color.white.AlphaMultiplied(0.5f);
                    }
                    else if (((keyframe.value != null) && keyframe.isPPtrCurve) && dopeline.tallMode)
                    {
                        this.m_SpritePreviewLoading = true;
                    }
                    if (Mathf.Approximately(keyframe.time, 0f))
                    {
                        keyframeRect.xMin -= 0.01f;
                    }
                    if (this.AnyKeyIsSelectedAtTime(dopeline, i))
                    {
                        color2 = (!dopeline.tallMode || !dopeline.isPptrDopeline) ? new Color(0.34f, 0.52f, 0.85f, 1f) : Color.white;
                        if (dopeline.isMasterDopeline)
                        {
                            color2 = color2.RGBMultiplied((float) 0.85f);
                        }
                        this.m_PointRenderer.AddSelectedKey(new DrawElement(keyframeRect, color2, texture));
                    }
                    else
                    {
                        this.m_PointRenderer.AddUnselectedKey(new DrawElement(keyframeRect, color2, texture));
                    }
                }
            }
            if (this.DoDragAndDrop(dopeline, dopeline.position, false))
            {
                float time = Mathf.Max(this.state.PixelToTime(Event.current.mousePosition.x, AnimationWindowState.SnapMode.SnapToClipFrame), 0f);
                Color color8 = Color.gray.RGBMultiplied((float) 1.2f);
                Texture2D assetPreview = null;
                foreach (Object obj2 in this.GetSortedDragAndDropObjectReferences())
                {
                    Rect dragAndDropRect = this.GetDragAndDropRect(dopeline, time);
                    if (dopeline.isPptrDopeline && dopeline.tallMode)
                    {
                        assetPreview = AssetPreview.GetAssetPreview(obj2.GetInstanceID(), this.assetPreviewManagerID);
                    }
                    if (assetPreview != null)
                    {
                        dragAndDropRect = this.GetPreviewRectFromKeyFrameRect(dragAndDropRect);
                        color8 = Color.white.AlphaMultiplied(0.5f);
                    }
                    this.m_PointRenderer.AddDragDropKey(new DrawElement(dragAndDropRect, color8, assetPreview));
                    time += 1f / this.state.frameRate;
                }
            }
            GUI.color = color;
        }

        private Rect DopelinesGUI(Rect position, Vector2 scrollPosition)
        {
            Color color = GUI.color;
            Rect rect = position;
            this.m_PointRenderer.Clear();
            if (Event.current.type == EventType.Repaint)
            {
                this.m_SpritePreviewLoading = false;
            }
            if (Event.current.type == EventType.MouseDown)
            {
                this.m_IsDragging = false;
            }
            this.UpdateSpritePreviewCacheSize();
            List<DopeLine> dopelines = this.state.dopelines;
            for (int i = 0; i < dopelines.Count; i++)
            {
                DopeLine dopeline = dopelines[i];
                dopeline.position = rect;
                dopeline.position.height = !dopeline.tallMode ? 16f : 32f;
                if ((((dopeline.position.yMin + scrollPosition.y) >= position.yMin) && ((dopeline.position.yMin + scrollPosition.y) <= position.yMax)) || (((dopeline.position.yMax + scrollPosition.y) >= position.yMin) && ((dopeline.position.yMax + scrollPosition.y) <= position.yMax)))
                {
                    Event current = Event.current;
                    EventType type = current.type;
                    switch (type)
                    {
                        case EventType.Repaint:
                            this.DopeLineRepaint(dopeline);
                            goto Label_01A6;

                        case EventType.DragUpdated:
                        case EventType.DragPerform:
                            this.HandleDragAndDrop(dopeline);
                            goto Label_01A6;
                    }
                    if (type != EventType.MouseDown)
                    {
                        if ((type == EventType.ContextClick) && !this.m_IsDraggingPlayhead)
                        {
                            this.HandleContextMenu(dopeline);
                        }
                    }
                    else if (current.button == 0)
                    {
                        this.HandleMouseDown(dopeline);
                    }
                }
            Label_01A6:
                rect.y += dopeline.position.height;
            }
            if (Event.current.type == EventType.MouseUp)
            {
                this.m_IsDraggingPlayheadStarted = false;
                this.m_IsDraggingPlayhead = false;
            }
            Rect rect2 = new Rect(position.xMin, position.yMin, position.width, rect.yMax - position.yMin);
            this.m_PointRenderer.Render();
            GUI.color = color;
            return rect2;
        }

        private void DoSpriteDropAfterGeneratingNewDopeline(AnimationClip animationClip, EditorCurveBinding? spriteBinding)
        {
            if (DragAndDrop.objectReferences.Length == 1)
            {
                UsabilityAnalytics.Event("Sprite Drag and Drop", "Drop single sprite into empty dopesheet", "null", 1);
            }
            else
            {
                UsabilityAnalytics.Event("Sprite Drag and Drop", "Drop multiple sprites into empty dopesheet", "null", 1);
            }
            AnimationWindowCurve targetCurve = new AnimationWindowCurve(animationClip, spriteBinding.Value, typeof(Sprite));
            this.PerformDragAndDrop(targetCurve, 0f);
        }

        private static void DrawBox(Rect position, Color color)
        {
            Color color2 = GUI.color;
            GUI.color = color;
            DopeLine.dopekeyStyle.Draw(position, GUIContent.none, 0, false);
            GUI.color = color2;
        }

        private void DrawGrid(Rect position)
        {
            base.TimeRuler(position, this.state.frameRate, false, true, 0.2f);
        }

        public void DrawMasterDopelineBackground(Rect position)
        {
            if (Event.current.type == EventType.Repaint)
            {
                AnimationWindowStyles.eventBackground.Draw(position, false, false, false, false);
            }
        }

        public void FrameClip()
        {
            if (!this.state.disabled)
            {
                Vector2 timeRange = this.state.timeRange;
                timeRange.y = Mathf.Max(timeRange.x + 0.1f, timeRange.y);
                base.SetShownHRangeInsideMargins(timeRange.x, timeRange.y);
            }
        }

        public void FrameSelected()
        {
            Bounds bounds = new Bounds();
            bool flag = true;
            bool flag2 = this.state.selectedKeys.Count > 0;
            if (flag2)
            {
                foreach (AnimationWindowKeyframe keyframe in this.state.selectedKeys)
                {
                    Vector2 vector = new Vector2(keyframe.time + keyframe.curve.timeOffset, 0f);
                    if (flag)
                    {
                        bounds.SetMinMax((Vector3) vector, (Vector3) vector);
                        flag = false;
                    }
                    else
                    {
                        bounds.Encapsulate((Vector3) vector);
                    }
                }
            }
            bool flag3 = !flag2;
            if (!flag2 && (this.state.hierarchyState.selectedIDs.Count > 0))
            {
                foreach (AnimationWindowCurve curve in this.state.activeCurves)
                {
                    int count = curve.m_Keyframes.Count;
                    if (count > 1)
                    {
                        Vector2 vector2 = new Vector2(curve.m_Keyframes[0].time + curve.timeOffset, 0f);
                        Vector2 vector3 = new Vector2(curve.m_Keyframes[count - 1].time + curve.timeOffset, 0f);
                        if (flag)
                        {
                            bounds.SetMinMax((Vector3) vector2, (Vector3) vector3);
                            flag = false;
                        }
                        else
                        {
                            bounds.Encapsulate((Vector3) vector2);
                            bounds.Encapsulate((Vector3) vector3);
                        }
                        flag3 = false;
                    }
                }
            }
            if (flag3)
            {
                this.FrameClip();
            }
            else
            {
                float x = Mathf.Max(bounds.size.x, 0.1f);
                bounds.size = new Vector3(x, Mathf.Max(bounds.size.y, 0.1f), 0f);
                base.SetShownHRangeInsideMargins(bounds.min.x, bounds.max.x);
            }
        }

        private GenericMenu GenerateMenu(DopeLine dopeline)
        {
            GenericMenu menu = new GenericMenu();
            List<AnimationWindowKeyframe> list = new List<AnimationWindowKeyframe>();
            foreach (AnimationWindowKeyframe keyframe in dopeline.keys)
            {
                if (this.GetKeyframeRect(dopeline, keyframe).Contains(Event.current.mousePosition))
                {
                    list.Add(keyframe);
                }
            }
            AnimationKeyTime time = AnimationKeyTime.Time(this.state.PixelToTime(Event.current.mousePosition.x, AnimationWindowState.SnapMode.SnapToClipFrame), this.state.frameRate);
            this.state.StartRecording();
            string text = "Add Key";
            if (dopeline.isEditable && (list.Count == 0))
            {
                AddKeyToDopelineContext userData = new AddKeyToDopelineContext {
                    dopeline = dopeline,
                    time = time
                };
                menu.AddItem(new GUIContent(text), false, new GenericMenu.MenuFunction2(this.AddKeyToDopeline), userData);
            }
            else
            {
                menu.AddDisabledItem(new GUIContent(text));
            }
            text = (this.state.selectedKeys.Count <= 1) ? "Delete Key" : "Delete Keys";
            if (dopeline.isEditable && ((this.state.selectedKeys.Count > 0) || (list.Count > 0)))
            {
                menu.AddItem(new GUIContent(text), false, new GenericMenu.MenuFunction2(this.DeleteKeys), (this.state.selectedKeys.Count <= 0) ? list : this.state.selectedKeys);
            }
            else
            {
                menu.AddDisabledItem(new GUIContent(text));
            }
            if (dopeline.isEditable && AnimationWindowUtility.ContainsFloatKeyframes(this.state.selectedKeys))
            {
                menu.AddSeparator(string.Empty);
                List<KeyIdentifier> keyList = new List<KeyIdentifier>();
                Hashtable hashtable = new Hashtable();
                foreach (AnimationWindowKeyframe keyframe2 in this.state.selectedKeys)
                {
                    if (!keyframe2.isPPtrCurve)
                    {
                        int keyframeIndex = keyframe2.curve.GetKeyframeIndex(AnimationKeyTime.Time(keyframe2.time, this.state.frameRate));
                        if (keyframeIndex != -1)
                        {
                            int hashCode = keyframe2.curve.GetHashCode();
                            AnimationCurve editorCurve = (AnimationCurve) hashtable[hashCode];
                            if (editorCurve == null)
                            {
                                editorCurve = AnimationUtility.GetEditorCurve(keyframe2.curve.clip, keyframe2.curve.binding);
                                if (editorCurve == null)
                                {
                                    editorCurve = new AnimationCurve();
                                }
                                hashtable.Add(hashCode, editorCurve);
                            }
                            keyList.Add(new KeyIdentifier(editorCurve, hashCode, keyframeIndex, keyframe2.curve.binding));
                        }
                    }
                }
                new CurveMenuManager(this).AddTangentMenuItems(menu, keyList);
            }
            return menu;
        }

        private AnimationWindowCurve GetCurveOfType(DopeLine dopeLine, Type type)
        {
            foreach (AnimationWindowCurve curve in dopeLine.curves)
            {
                if (curve.valueType == type)
                {
                    return curve;
                }
            }
            return null;
        }

        private Rect GetDragAndDropRect(DopeLine dopeline, float time)
        {
            Rect keyframeRect = this.GetKeyframeRect(dopeline, null);
            float keyframeOffset = this.GetKeyframeOffset(dopeline, null);
            keyframeRect.center = new Vector2((this.state.TimeToPixel(time) + (keyframeRect.width * 0.5f)) + keyframeOffset, keyframeRect.center.y);
            return keyframeRect;
        }

        private float GetKeyframeOffset(DopeLine dopeline, AnimationWindowKeyframe keyframe)
        {
            if ((dopeline.isPptrDopeline && dopeline.tallMode) && ((keyframe == null) || (keyframe.value != null)))
            {
                return -1f;
            }
            return -6.5f;
        }

        private Rect GetKeyframeRect(DopeLine dopeline, AnimationWindowKeyframe keyframe)
        {
            float time = (keyframe == null) ? 0f : (keyframe.time + keyframe.curve.timeOffset);
            float width = 10f;
            if ((dopeline.isPptrDopeline && dopeline.tallMode) && ((keyframe == null) || (keyframe.value != null)))
            {
                width = dopeline.position.height;
            }
            if (dopeline.isPptrDopeline && dopeline.tallMode)
            {
                return new Rect(this.state.TimeToPixel(this.state.SnapToFrame(time, AnimationWindowState.SnapMode.SnapToClipFrame)) + this.GetKeyframeOffset(dopeline, keyframe), dopeline.position.yMin, width, dopeline.position.height);
            }
            return new Rect(this.state.TimeToPixel(this.state.SnapToFrame(time, AnimationWindowState.SnapMode.SnapToClipFrame)) + this.GetKeyframeOffset(dopeline, keyframe), dopeline.position.yMin, width, dopeline.position.height);
        }

        private Rect GetPreviewRectFromKeyFrameRect(Rect keyframeRect)
        {
            keyframeRect.width -= 2f;
            keyframeRect.height -= 2f;
            keyframeRect.xMin += 2f;
            keyframeRect.yMin += 2f;
            return keyframeRect;
        }

        private Object[] GetSortedDragAndDropObjectReferences()
        {
            Object[] objectReferences = DragAndDrop.objectReferences;
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = (a, b) => EditorUtility.NaturalCompare(a.name, b.name);
            }
            Array.Sort<Object>(objectReferences, <>f__am$cache0);
            return objectReferences;
        }

        private void HandleContextMenu(DopeLine dopeline)
        {
            if (dopeline.position.Contains(Event.current.mousePosition))
            {
                this.GenerateMenu(dopeline).ShowAsContext();
            }
        }

        private void HandleDelete()
        {
            if (this.state.selectedKeys.Count != 0)
            {
                switch (Event.current.type)
                {
                    case EventType.ValidateCommand:
                    case EventType.ExecuteCommand:
                        if ((Event.current.commandName == "SoftDelete") || (Event.current.commandName == "Delete"))
                        {
                            if (Event.current.type == EventType.ExecuteCommand)
                            {
                                this.state.DeleteSelectedKeys();
                            }
                            Event.current.Use();
                        }
                        break;

                    case EventType.KeyDown:
                        if ((Event.current.keyCode == KeyCode.Backspace) || (Event.current.keyCode == KeyCode.Delete))
                        {
                            this.state.DeleteSelectedKeys();
                            Event.current.Use();
                        }
                        break;
                }
            }
        }

        private void HandleDopelineDoubleclick(DopeLine dopeline)
        {
            AnimationKeyTime time = AnimationKeyTime.Time(this.state.PixelToTime(Event.current.mousePosition.x, AnimationWindowState.SnapMode.SnapToClipFrame), this.state.frameRate);
            AnimationWindowUtility.AddKeyframes(this.state, dopeline.curves.ToArray<AnimationWindowCurve>(), time);
            Event.current.Use();
        }

        private void HandleDragAndDrop(DopeLine dopeline)
        {
            Event current = Event.current;
            if ((current.type == EventType.DragPerform) || (current.type == EventType.DragUpdated))
            {
                if (this.DoDragAndDrop(dopeline, dopeline.position, current.type == EventType.DragPerform))
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    current.Use();
                }
                else
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                }
            }
        }

        private void HandleDragAndDropToEmptyArea()
        {
            Event current = Event.current;
            if (((current.type == EventType.DragPerform) || (current.type == EventType.DragUpdated)) && ValidateDragAndDropObjects())
            {
                if ((DragAndDrop.objectReferences[0].GetType() == typeof(Sprite)) || (DragAndDrop.objectReferences[0].GetType() == typeof(Texture2D)))
                {
                    foreach (AnimationWindowSelectionItem item in this.state.selection.ToArray())
                    {
                        if ((item.clipIsEditable && item.canAddCurves) && !this.DopelineForValueTypeExists(typeof(Sprite)))
                        {
                            if (current.type == EventType.DragPerform)
                            {
                                EditorCurveBinding? spriteBinding = this.CreateNewPptrDopeline(item, typeof(Sprite));
                                if (spriteBinding.HasValue)
                                {
                                    this.DoSpriteDropAfterGeneratingNewDopeline(item.animationClip, spriteBinding);
                                }
                            }
                            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                            current.Use();
                            return;
                        }
                    }
                }
                DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
            }
        }

        private void HandleDragging()
        {
            int controlID = GUIUtility.GetControlID("dopesheetdrag".GetHashCode(), FocusType.Passive, new Rect());
            EventType typeForControl = Event.current.GetTypeForControl(controlID);
            if (((typeForControl == EventType.MouseDrag) || (typeForControl == EventType.MouseUp)) && this.m_MousedownOnKeyframe)
            {
                if ((((typeForControl == EventType.MouseDrag) && !EditorGUI.actionKey) && !Event.current.shift) && (!this.m_IsDragging && (this.state.selectedKeys.Count > 0)))
                {
                    this.m_IsDragging = true;
                    this.m_IsDraggingPlayheadStarted = true;
                    GUIUtility.hotControl = controlID;
                    this.m_DragStartTime = this.state.PixelToTime(Event.current.mousePosition.x);
                    this.state.StartLiveEdit();
                    Event.current.Use();
                }
                float maxValue = float.MaxValue;
                foreach (AnimationWindowKeyframe keyframe in this.state.selectedKeys)
                {
                    maxValue = Mathf.Min(keyframe.time, maxValue);
                }
                float a = this.state.SnapToFrame(this.state.PixelToTime(Event.current.mousePosition.x), AnimationWindowState.SnapMode.SnapToClipFrame);
                float deltaTime = a - this.m_DragStartTime;
                if (this.m_IsDragging && !Mathf.Approximately(a, this.m_DragStartTime))
                {
                    this.state.MoveSelectedKeys(deltaTime, true);
                    Event.current.Use();
                }
                if (typeForControl == EventType.MouseUp)
                {
                    if (this.m_IsDragging && (GUIUtility.hotControl == controlID))
                    {
                        this.state.MoveSelectedKeys(deltaTime, true);
                        this.state.EndLiveEdit();
                        Event.current.Use();
                        this.m_IsDragging = false;
                    }
                    this.m_MousedownOnKeyframe = false;
                    GUIUtility.hotControl = 0;
                }
            }
            if ((this.m_IsDraggingPlayheadStarted && (typeForControl == EventType.MouseDrag)) && (Event.current.button == 1))
            {
                this.m_IsDraggingPlayhead = true;
                Event.current.Use();
            }
            if (this.m_IsDragging)
            {
                Vector2 mousePosition = Event.current.mousePosition;
                Rect position = new Rect(mousePosition.x - 10f, mousePosition.y - 10f, 20f, 20f);
                EditorGUIUtility.AddCursorRect(position, MouseCursor.MoveArrow);
            }
        }

        private void HandleKeyboard()
        {
            if ((Event.current.type == EventType.ValidateCommand) || (Event.current.type == EventType.ExecuteCommand))
            {
                switch (Event.current.commandName)
                {
                    case "SelectAll":
                        if (Event.current.type == EventType.ExecuteCommand)
                        {
                            this.HandleSelectAll();
                        }
                        Event.current.Use();
                        break;

                    case "FrameSelected":
                        if (Event.current.type == EventType.ExecuteCommand)
                        {
                            this.FrameSelected();
                        }
                        Event.current.Use();
                        break;
                }
            }
            if ((Event.current.type == EventType.KeyDown) && (Event.current.keyCode == KeyCode.A))
            {
                this.FrameClip();
                Event.current.Use();
            }
        }

        private void HandleMouseDown(DopeLine dopeline)
        {
            Event current = Event.current;
            if (dopeline.position.Contains(current.mousePosition))
            {
                bool flag = false;
                foreach (AnimationWindowKeyframe keyframe in dopeline.keys)
                {
                    if (this.GetKeyframeRect(dopeline, keyframe).Contains(current.mousePosition) && this.state.KeyIsSelected(keyframe))
                    {
                        flag = true;
                        break;
                    }
                }
                bool flag2 = flag && EditorGUI.actionKey;
                bool flag3 = !flag;
                if ((!flag && !EditorGUI.actionKey) && !current.shift)
                {
                    this.state.ClearSelections();
                }
                float time = this.state.PixelToTime(Event.current.mousePosition.x);
                float num2 = time;
                if (Event.current.shift)
                {
                    foreach (AnimationWindowKeyframe keyframe2 in dopeline.keys)
                    {
                        if (this.state.KeyIsSelected(keyframe2))
                        {
                            if (keyframe2.time < time)
                            {
                                time = keyframe2.time;
                            }
                            if (keyframe2.time > num2)
                            {
                                num2 = keyframe2.time;
                            }
                        }
                    }
                }
                bool flag4 = false;
                foreach (AnimationWindowKeyframe keyframe3 in dopeline.keys)
                {
                    if (this.GetKeyframeRect(dopeline, keyframe3).Contains(current.mousePosition))
                    {
                        flag4 = true;
                        if (flag2)
                        {
                            if (this.state.KeyIsSelected(keyframe3))
                            {
                                this.state.UnselectKey(keyframe3);
                                if (!this.state.AnyKeyIsSelected(dopeline))
                                {
                                    this.state.UnSelectHierarchyItem(dopeline);
                                }
                            }
                        }
                        else if (flag3 && !this.state.KeyIsSelected(keyframe3))
                        {
                            if (Event.current.shift)
                            {
                                foreach (AnimationWindowKeyframe keyframe4 in dopeline.keys)
                                {
                                    if ((keyframe4 == keyframe3) || ((keyframe4.time > time) && (keyframe4.time < num2)))
                                    {
                                        this.state.SelectKey(keyframe4);
                                    }
                                }
                            }
                            else
                            {
                                this.state.SelectKey(keyframe3);
                            }
                            if (!dopeline.isMasterDopeline)
                            {
                                this.state.SelectHierarchyItem(dopeline, EditorGUI.actionKey || current.shift);
                            }
                        }
                        this.state.activeKeyframe = keyframe3;
                        this.m_MousedownOnKeyframe = true;
                        current.Use();
                    }
                }
                if (dopeline.isMasterDopeline)
                {
                    this.state.ClearHierarchySelection();
                    List<int> affectedHierarchyIDs = this.state.GetAffectedHierarchyIDs(this.state.selectedKeys);
                    foreach (int num3 in affectedHierarchyIDs)
                    {
                        this.state.SelectHierarchyItem(num3, true, true);
                    }
                }
                if (((current.clickCount == 2) && (current.button == 0)) && (!Event.current.shift && !EditorGUI.actionKey))
                {
                    this.HandleDopelineDoubleclick(dopeline);
                }
                if (((current.button == 1) && !this.state.controlInterface.playing) && !flag4)
                {
                    this.state.ClearSelections();
                    this.m_IsDraggingPlayheadStarted = true;
                    HandleUtility.Repaint();
                    current.Use();
                }
            }
        }

        private void HandleRectangleToolEvents()
        {
            this.m_RectangleTool.HandleEvents();
        }

        private void HandleSelectAll()
        {
            foreach (DopeLine line in this.state.dopelines)
            {
                foreach (AnimationWindowKeyframe keyframe in line.keys)
                {
                    this.state.SelectKey(keyframe);
                }
                this.state.SelectHierarchyItem(line, true, false);
            }
        }

        private void HandleSelectionRect(Rect rect)
        {
            if (this.m_SelectionRect == null)
            {
                this.m_SelectionRect = new DopeSheetSelectionRect(this);
            }
            if (!this.m_MousedownOnKeyframe)
            {
                this.m_SelectionRect.OnGUI(rect);
            }
        }

        public void Init()
        {
            if (!this.m_Initialized)
            {
                base.hSlider = true;
                base.vSlider = false;
                base.hRangeLocked = false;
                base.vRangeLocked = true;
                base.hRangeMin = 0f;
                base.margin = 40f;
                base.scaleWithWindow = true;
                base.ignoreScrollWheelUntilClicked = false;
            }
            this.m_Initialized = true;
            if (this.m_PointRenderer == null)
            {
                this.m_PointRenderer = new DopeSheetControlPointRenderer();
            }
            if (this.m_RectangleTool == null)
            {
                this.m_RectangleTool = new DopeSheetEditorRectangleTool();
                this.m_RectangleTool.Initialize(this);
            }
        }

        internal void OnDestroy()
        {
            AssetPreview.DeletePreviewTextureManagerByID(this.assetPreviewManagerID);
        }

        public void OnDisable()
        {
            if (this.m_PointRenderer != null)
            {
                this.m_PointRenderer.FlushCache();
            }
        }

        public void OnGUI(Rect position, Vector2 scrollPosition)
        {
            this.Init();
            this.HandleDragAndDropToEmptyArea();
            GUIClip.Push(position, scrollPosition, Vector2.zero, false);
            this.HandleRectangleToolEvents();
            Rect rect = new Rect(0f, 0f, position.width, position.height);
            Rect rect2 = this.DopelinesGUI(rect, scrollPosition);
            this.HandleKeyboard();
            this.HandleDragging();
            this.HandleSelectionRect(rect2);
            this.HandleDelete();
            this.RectangleToolGUI();
            GUIClip.Pop();
        }

        private void PerformDragAndDrop(AnimationWindowCurve targetCurve, float time)
        {
            if ((DragAndDrop.objectReferences.Length != 0) && (targetCurve != null))
            {
                string undoLabel = "Drop Key";
                this.state.SaveKeySelection(undoLabel);
                this.state.ClearSelections();
                Object[] sortedDragAndDropObjectReferences = this.GetSortedDragAndDropObjectReferences();
                foreach (Object obj2 in sortedDragAndDropObjectReferences)
                {
                    Object obj3 = obj2;
                    if (obj3 is Texture2D)
                    {
                        obj3 = SpriteUtility.TextureToSprite(obj2 as Texture2D);
                    }
                    this.CreateNewPPtrKeyframe(time, obj3, targetCurve);
                    time += 1f / targetCurve.clip.frameRate;
                }
                this.state.SaveCurve(targetCurve, undoLabel);
                DragAndDrop.AcceptDrag();
            }
        }

        public void RecalculateBounds()
        {
            if (!this.state.disabled)
            {
                Vector2 timeRange = this.state.timeRange;
                this.m_Bounds.SetMinMax(new Vector3(timeRange.x, 0f, 0f), new Vector3(timeRange.y, 0f, 0f));
            }
        }

        private void RectangleToolGUI()
        {
            this.m_RectangleTool.OnGUI();
        }

        private void SelectTypeForCreatingNewPptrDopeline(object userData, string[] options, int selected)
        {
            List<object> list = userData as List<object>;
            AnimationClip animationClip = list[0] as AnimationClip;
            List<EditorCurveBinding> list2 = list[1] as List<EditorCurveBinding>;
            if (list2.Count > selected)
            {
                this.DoSpriteDropAfterGeneratingNewDopeline(animationClip, new EditorCurveBinding?(list2[selected]));
            }
        }

        public void UpdateCurves(List<ChangedCurve> changedCurves, string undoText)
        {
            Undo.RegisterCompleteObjectUndo(this.state.activeAnimationClip, undoText);
            using (List<ChangedCurve>.Enumerator enumerator = changedCurves.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    <UpdateCurves>c__AnonStorey1 storey = new <UpdateCurves>c__AnonStorey1 {
                        changedCurve = enumerator.Current
                    };
                    AnimationWindowCurve curve = this.state.allCurves.Find(new Predicate<AnimationWindowCurve>(storey.<>m__0));
                    if (curve != null)
                    {
                        AnimationUtility.SetEditorCurve(curve.clip, storey.changedCurve.binding, storey.changedCurve.curve);
                    }
                    else
                    {
                        Debug.LogError("Could not match ChangedCurve data to destination curves.");
                    }
                }
            }
        }

        private void UpdateSpritePreviewCacheSize()
        {
            int size = 1;
            foreach (DopeLine line in this.state.dopelines)
            {
                if (line.tallMode && line.isPptrDopeline)
                {
                    size += line.keys.Count;
                }
            }
            size += DragAndDrop.objectReferences.Length;
            if (size > this.m_SpritePreviewCacheSize)
            {
                AssetPreview.SetPreviewTextureCacheSize(size, this.assetPreviewManagerID);
                this.m_SpritePreviewCacheSize = size;
            }
        }

        private static bool ValidateDragAndDropObjects()
        {
            if (DragAndDrop.objectReferences.Length == 0)
            {
                return false;
            }
            for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
            {
                Object obj2 = DragAndDrop.objectReferences[i];
                if (obj2 == null)
                {
                    return false;
                }
                if (i < (DragAndDrop.objectReferences.Length - 1))
                {
                    Object obj3 = DragAndDrop.objectReferences[i + 1];
                    bool flag2 = ((obj2 is Texture2D) || (obj2 is Sprite)) && ((obj3 is Texture2D) || (obj3 is Sprite));
                    if ((obj2.GetType() != obj3.GetType()) && !flag2)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        internal int assetPreviewManagerID =>
            ((this.m_Owner == null) ? 0 : this.m_Owner.GetInstanceID());

        public float contentHeight
        {
            get
            {
                float num = 0f;
                foreach (DopeLine line in this.state.dopelines)
                {
                    num += !line.tallMode ? 16f : 32f;
                }
                return (num + 40f);
            }
        }

        public override Bounds drawingBounds =>
            this.m_Bounds;

        public bool isDragging =>
            this.m_IsDragging;

        public bool spritePreviewLoading =>
            this.m_SpritePreviewLoading;

        [CompilerGenerated]
        private sealed class <DopelineForValueTypeExists>c__AnonStorey0
        {
            internal Type valueType;

            internal bool <>m__0(AnimationWindowCurve curve) => 
                (curve.valueType == this.valueType);
        }

        [CompilerGenerated]
        private sealed class <UpdateCurves>c__AnonStorey1
        {
            internal ChangedCurve changedCurve;

            internal bool <>m__0(AnimationWindowCurve c) => 
                (this.changedCurve.curveId == c.GetHashCode());
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct AddKeyToDopelineContext
        {
            public DopeLine dopeline;
            public AnimationKeyTime time;
        }

        private class DopeSheetControlPointRenderer
        {
            private Texture2D m_DefaultDopeKeyIcon = EditorGUIUtility.LoadIcon("blendKey");
            private List<DopeSheetEditor.DrawElement> m_DragDropKeysDrawBuffer = new List<DopeSheetEditor.DrawElement>();
            private ControlPointRenderer m_DragDropKeysRenderer;
            private List<DopeSheetEditor.DrawElement> m_SelectedKeysDrawBuffer = new List<DopeSheetEditor.DrawElement>();
            private ControlPointRenderer m_SelectedKeysRenderer;
            private List<DopeSheetEditor.DrawElement> m_UnselectedKeysDrawBuffer = new List<DopeSheetEditor.DrawElement>();
            private ControlPointRenderer m_UnselectedKeysRenderer;

            public DopeSheetControlPointRenderer()
            {
                this.m_UnselectedKeysRenderer = new ControlPointRenderer(this.m_DefaultDopeKeyIcon);
                this.m_SelectedKeysRenderer = new ControlPointRenderer(this.m_DefaultDopeKeyIcon);
                this.m_DragDropKeysRenderer = new ControlPointRenderer(this.m_DefaultDopeKeyIcon);
            }

            public void AddDragDropKey(DopeSheetEditor.DrawElement element)
            {
                if (element.texture != null)
                {
                    this.m_DragDropKeysDrawBuffer.Add(element);
                }
                else
                {
                    Rect position = element.position;
                    position.size = new Vector2((float) this.m_DefaultDopeKeyIcon.width, (float) this.m_DefaultDopeKeyIcon.height);
                    this.m_DragDropKeysRenderer.AddPoint(position, element.color);
                }
            }

            public void AddSelectedKey(DopeSheetEditor.DrawElement element)
            {
                if (element.texture != null)
                {
                    this.m_SelectedKeysDrawBuffer.Add(element);
                }
                else
                {
                    Rect position = element.position;
                    position.size = new Vector2((float) this.m_DefaultDopeKeyIcon.width, (float) this.m_DefaultDopeKeyIcon.height);
                    this.m_SelectedKeysRenderer.AddPoint(position, element.color);
                }
            }

            public void AddUnselectedKey(DopeSheetEditor.DrawElement element)
            {
                if (element.texture != null)
                {
                    this.m_UnselectedKeysDrawBuffer.Add(element);
                }
                else
                {
                    Rect position = element.position;
                    position.size = new Vector2((float) this.m_DefaultDopeKeyIcon.width, (float) this.m_DefaultDopeKeyIcon.height);
                    this.m_UnselectedKeysRenderer.AddPoint(position, element.color);
                }
            }

            public void Clear()
            {
                this.m_UnselectedKeysDrawBuffer.Clear();
                this.m_SelectedKeysDrawBuffer.Clear();
                this.m_DragDropKeysDrawBuffer.Clear();
                this.m_UnselectedKeysRenderer.Clear();
                this.m_SelectedKeysRenderer.Clear();
                this.m_DragDropKeysRenderer.Clear();
            }

            private void DrawElements(List<DopeSheetEditor.DrawElement> elements)
            {
                if (elements.Count != 0)
                {
                    Color color = GUI.color;
                    Color white = Color.white;
                    GUI.color = white;
                    Texture defaultDopeKeyIcon = this.m_DefaultDopeKeyIcon;
                    for (int i = 0; i < elements.Count; i++)
                    {
                        DopeSheetEditor.DrawElement element = elements[i];
                        if (element.color != white)
                        {
                            white = !GUI.enabled ? ((Color) (element.color * 0.8f)) : element.color;
                            GUI.color = white;
                        }
                        if (element.texture != null)
                        {
                            GUI.Label(element.position, element.texture, GUIStyle.none);
                        }
                        else
                        {
                            Rect position = new Rect(element.position.center.x - (defaultDopeKeyIcon.width / 2), element.position.center.y - (defaultDopeKeyIcon.height / 2), (float) defaultDopeKeyIcon.width, (float) defaultDopeKeyIcon.height);
                            GUI.Label(position, defaultDopeKeyIcon, GUIStyle.none);
                        }
                    }
                    GUI.color = color;
                }
            }

            public void FlushCache()
            {
                this.m_UnselectedKeysRenderer.FlushCache();
                this.m_SelectedKeysRenderer.FlushCache();
                this.m_DragDropKeysRenderer.FlushCache();
            }

            public void Render()
            {
                this.DrawElements(this.m_UnselectedKeysDrawBuffer);
                this.m_UnselectedKeysRenderer.Render();
                this.DrawElements(this.m_SelectedKeysDrawBuffer);
                this.m_SelectedKeysRenderer.Render();
                this.DrawElements(this.m_DragDropKeysDrawBuffer);
                this.m_DragDropKeysRenderer.Render();
            }
        }

        internal class DopeSheetSelectionRect
        {
            public readonly GUIStyle createRect = "U2D.createRect";
            private Vector2 m_SelectMousePoint;
            private Vector2 m_SelectStartPoint;
            private bool m_ValidRect;
            private DopeSheetEditor owner;
            private static int s_RectSelectionID = GUIUtility.GetPermanentControlID();

            public DopeSheetSelectionRect(DopeSheetEditor owner)
            {
                this.owner = owner;
            }

            public Rect GetCurrentPixelRect()
            {
                float num = 16f;
                Rect rect = AnimationWindowUtility.FromToRect(this.m_SelectStartPoint, this.m_SelectMousePoint);
                rect.xMin = this.owner.state.TimeToPixel(this.owner.state.PixelToTime(rect.xMin, AnimationWindowState.SnapMode.SnapToClipFrame), AnimationWindowState.SnapMode.SnapToClipFrame);
                rect.xMax = this.owner.state.TimeToPixel(this.owner.state.PixelToTime(rect.xMax, AnimationWindowState.SnapMode.SnapToClipFrame), AnimationWindowState.SnapMode.SnapToClipFrame);
                rect.yMin = Mathf.Floor(rect.yMin / num) * num;
                rect.yMax = (Mathf.Floor(rect.yMax / num) + 1f) * num;
                return rect;
            }

            public Rect GetCurrentTimeRect()
            {
                float num = 16f;
                Rect rect = AnimationWindowUtility.FromToRect(this.m_SelectStartPoint, this.m_SelectMousePoint);
                rect.xMin = this.owner.state.PixelToTime(rect.xMin, AnimationWindowState.SnapMode.SnapToClipFrame);
                rect.xMax = this.owner.state.PixelToTime(rect.xMax, AnimationWindowState.SnapMode.SnapToClipFrame);
                rect.yMin = Mathf.Floor(rect.yMin / num) * num;
                rect.yMax = (Mathf.Floor(rect.yMax / num) + 1f) * num;
                return rect;
            }

            public void OnGUI(Rect position)
            {
                Event current = Event.current;
                Vector2 mousePosition = current.mousePosition;
                int controlID = s_RectSelectionID;
                switch (current.GetTypeForControl(controlID))
                {
                    case EventType.MouseDown:
                        if ((current.button == 0) && position.Contains(mousePosition))
                        {
                            GUIUtility.hotControl = controlID;
                            this.m_SelectStartPoint = mousePosition;
                            this.m_ValidRect = false;
                            current.Use();
                        }
                        return;

                    case EventType.MouseUp:
                    {
                        if ((GUIUtility.hotControl != controlID) || (current.button != 0))
                        {
                            return;
                        }
                        if (!this.m_ValidRect)
                        {
                            this.owner.state.ClearSelections();
                            break;
                        }
                        if (!current.shift && !EditorGUI.actionKey)
                        {
                            this.owner.state.ClearSelections();
                        }
                        float frameRate = this.owner.state.frameRate;
                        Rect currentTimeRect = this.GetCurrentTimeRect();
                        GUI.changed = true;
                        this.owner.state.ClearHierarchySelection();
                        List<AnimationWindowKeyframe> list = new List<AnimationWindowKeyframe>();
                        List<AnimationWindowKeyframe> list2 = new List<AnimationWindowKeyframe>();
                        foreach (DopeLine line in this.owner.state.dopelines)
                        {
                            if ((line.position.yMin >= currentTimeRect.yMin) && (line.position.yMax <= currentTimeRect.yMax))
                            {
                                foreach (AnimationWindowKeyframe keyframe in line.keys)
                                {
                                    AnimationKeyTime time = AnimationKeyTime.Time(currentTimeRect.xMin - keyframe.curve.timeOffset, frameRate);
                                    AnimationKeyTime time2 = AnimationKeyTime.Time(currentTimeRect.xMax - keyframe.curve.timeOffset, frameRate);
                                    AnimationKeyTime time3 = AnimationKeyTime.Time(keyframe.time, frameRate);
                                    if ((((!line.tallMode && (time3.frame >= time.frame)) && (time3.frame <= time2.frame)) || ((line.tallMode && (time3.frame >= time.frame)) && (time3.frame < time2.frame))) && (!list2.Contains(keyframe) && !list.Contains(keyframe)))
                                    {
                                        if (!this.owner.state.KeyIsSelected(keyframe))
                                        {
                                            list2.Add(keyframe);
                                        }
                                        else if (this.owner.state.KeyIsSelected(keyframe))
                                        {
                                            list.Add(keyframe);
                                        }
                                    }
                                }
                            }
                        }
                        if (list2.Count == 0)
                        {
                            foreach (AnimationWindowKeyframe keyframe2 in list)
                            {
                                this.owner.state.UnselectKey(keyframe2);
                            }
                        }
                        foreach (AnimationWindowKeyframe keyframe3 in list2)
                        {
                            this.owner.state.SelectKey(keyframe3);
                        }
                        foreach (DopeLine line2 in this.owner.state.dopelines)
                        {
                            if (this.owner.state.AnyKeyIsSelected(line2))
                            {
                                this.owner.state.SelectHierarchyItem(line2, true, false);
                            }
                        }
                        break;
                    }
                    case EventType.MouseMove:
                    case EventType.KeyDown:
                    case EventType.KeyUp:
                    case EventType.ScrollWheel:
                        return;

                    case EventType.MouseDrag:
                        if (GUIUtility.hotControl == controlID)
                        {
                            Vector2 vector2 = mousePosition - this.m_SelectStartPoint;
                            this.m_ValidRect = Mathf.Abs(vector2.x) > 1f;
                            if (this.m_ValidRect)
                            {
                                this.m_SelectMousePoint = new Vector2(mousePosition.x, mousePosition.y);
                            }
                            current.Use();
                        }
                        return;

                    case EventType.Repaint:
                        if ((GUIUtility.hotControl == controlID) && this.m_ValidRect)
                        {
                            EditorStyles.selectionRect.Draw(this.GetCurrentPixelRect(), GUIContent.none, false, false, false, false);
                        }
                        return;

                    default:
                        return;
                }
                current.Use();
                GUIUtility.hotControl = 0;
            }

            private enum SelectionType
            {
                Normal,
                Additive,
                Subtractive
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DrawElement
        {
            public Rect position;
            public Color color;
            public Texture2D texture;
            public DrawElement(Rect position, Color color, Texture2D texture)
            {
                this.position = position;
                this.color = color;
                this.texture = texture;
            }
        }
    }
}

