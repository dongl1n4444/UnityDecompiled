namespace UnityEditorInternal
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEditor.IMGUI.Controls;
    using UnityEngine;

    internal class AnimationWindowHierarchyGUI : TreeViewGUI
    {
        [CompilerGenerated]
        private static Predicate<AnimationWindowCurve> <>f__am$cache0;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private AnimationWindowState <state>k__BackingField;
        public const float k_AddCurveButtonNodeHeight = 40f;
        private readonly GUIContent k_AnimatePropertyLabel;
        private const float k_ColorIndicatorTopMargin = 3f;
        public const float k_DopeSheetRowHeight = 16f;
        public const float k_DopeSheetRowHeightTall = 32f;
        private static readonly Color k_KeyColorForNonCurves = new Color(0.7f, 0.7f, 0.7f, 0.5f);
        private static readonly Color k_KeyColorInDopesheetMode = new Color(0.7f, 0.7f, 0.7f, 1f);
        private static readonly Color k_LeftoverCurveColor = Color.yellow;
        public const float k_RowBackgroundColorBrightness = 0.28f;
        private const float k_RowRightOffset = 10f;
        private const float k_SelectedPhantomCurveColorMultiplier = 1.4f;
        private const float k_ValueFieldDragWidth = 15f;
        private const float k_ValueFieldOffsetFromRightSide = 75f;
        private const float k_ValueFieldWidth = 50f;
        private GUIStyle m_AnimationCurveDropdown;
        private GUIStyle m_AnimationLineStyle;
        private GUIStyle m_AnimationRowEvenStyle;
        private GUIStyle m_AnimationRowOddStyle;
        private GUIStyle m_AnimationSelectionTextField;
        private int[] m_HierarchyItemButtonControlIDs;
        private int[] m_HierarchyItemFoldControlIDs;
        private int[] m_HierarchyItemValueControlIDs;
        private Color m_LightSkinPropertyTextColor;
        private Color m_PhantomCurveColor;
        private AnimationWindowHierarchyNode m_RenamedNode;
        internal static int s_WasInsideValueRectFrame = -1;

        public AnimationWindowHierarchyGUI(TreeViewController treeView, AnimationWindowState state) : base(treeView)
        {
            this.k_AnimatePropertyLabel = new GUIContent("Add Property");
            this.m_LightSkinPropertyTextColor = new Color(0.35f, 0.35f, 0.35f);
            this.m_PhantomCurveColor = new Color(0f, 0.6f, 0.6f);
            this.state = state;
            this.InitStyles();
        }

        private void AddKeysAtCurrentTime(List<AnimationWindowCurve> curves)
        {
            AnimationWindowUtility.AddKeyframes(this.state, curves.ToArray(), this.state.time);
        }

        private void AddKeysAtCurrentTime(object obj)
        {
            this.AddKeysAtCurrentTime((List<AnimationWindowCurve>) obj);
        }

        public override bool BeginRename(TreeViewItem item, float delay)
        {
            this.m_RenamedNode = item as AnimationWindowHierarchyNode;
            GameObject rootGameObject = null;
            if (this.m_RenamedNode.curves.Length > 0)
            {
                AnimationWindowSelectionItem selectionBinding = this.m_RenamedNode.curves[0].selectionBinding;
                if (selectionBinding != null)
                {
                    rootGameObject = selectionBinding.rootGameObject;
                }
            }
            return base.GetRenameOverlay().BeginRename(this.m_RenamedNode.path, item.id, delay);
        }

        public override void BeginRowGUI()
        {
            base.BeginRowGUI();
            this.HandleDelete();
            int rowCount = base.m_TreeView.data.rowCount;
            this.m_HierarchyItemFoldControlIDs = new int[rowCount];
            this.m_HierarchyItemValueControlIDs = new int[rowCount];
            this.m_HierarchyItemButtonControlIDs = new int[rowCount];
            for (int i = 0; i < rowCount; i++)
            {
                this.m_HierarchyItemFoldControlIDs[i] = GUIUtility.GetControlID(FocusType.Passive);
                this.m_HierarchyItemValueControlIDs[i] = GUIUtility.GetControlID(FocusType.Passive);
                this.m_HierarchyItemButtonControlIDs[i] = GUIUtility.GetControlID(FocusType.Passive);
            }
        }

        private void ChangeRotationInterpolation(object interpolationMode)
        {
            RotationCurveInterpolation.Mode newInterpolationMode = (RotationCurveInterpolation.Mode) interpolationMode;
            AnimationWindowCurve[] curveArray = this.state.activeCurves.ToArray();
            EditorCurveBinding[] curveBindings = new EditorCurveBinding[curveArray.Length];
            for (int i = 0; i < curveArray.Length; i++)
            {
                curveBindings[i] = curveArray[i].binding;
            }
            RotationCurveInterpolation.SetInterpolation(this.state.activeAnimationClip, curveBindings, newInterpolationMode);
            this.MaintainTreeviewStateAfterRotationInterpolation(newInterpolationMode);
            this.state.hierarchyData.ReloadData();
        }

        private void DeleteKeysAtCurrentTime(List<AnimationWindowCurve> curves)
        {
            AnimationWindowUtility.RemoveKeyframes(this.state, curves.ToArray(), this.state.time);
        }

        private void DeleteKeysAtCurrentTime(object obj)
        {
            this.DeleteKeysAtCurrentTime((List<AnimationWindowCurve>) obj);
        }

        private void DoAddCurveButton(Rect rect, AnimationWindowHierarchyNode node, int row)
        {
            float num = (rect.width - 230f) / 2f;
            float num2 = 10f;
            Rect position = new Rect(rect.xMin + num, rect.yMin + num2, rect.width - (num * 2f), rect.height - (num2 * 2f));
            if (this.DoTreeViewButton(this.m_HierarchyItemButtonControlIDs[row], position, this.k_AnimatePropertyLabel, GUI.skin.button))
            {
                AddCurvesPopup.selection = this.state.selection;
                AddCurvesPopupHierarchyDataSource.showEntireHierarchy = true;
                if (AddCurvesPopup.ShowAtPosition(position, this.state, new UnityEditorInternal.AddCurvesPopup.OnNewCurveAdded(this.OnNewCurveAdded)))
                {
                    GUIUtility.ExitGUI();
                }
            }
        }

        private void DoCurveColorIndicator(Rect rect, AnimationWindowHierarchyNode node)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Color color = GUI.color;
                if (!this.state.showCurveEditor)
                {
                    GUI.color = k_KeyColorInDopesheetMode;
                }
                else if ((node.curves.Length == 1) && !node.curves[0].isPPtrCurve)
                {
                    GUI.color = CurveUtility.GetPropertyColor(node.curves[0].binding.propertyName);
                }
                else
                {
                    GUI.color = k_KeyColorForNonCurves;
                }
                bool flag = false;
                if (this.state.previewing)
                {
                    foreach (AnimationWindowCurve curve in node.curves)
                    {
                        if (Enumerable.Any<AnimationWindowKeyframe>(curve.m_Keyframes, (Func<AnimationWindowKeyframe, bool>) (key => this.state.time.ContainsTime(key.time))))
                        {
                            flag = true;
                        }
                    }
                }
                Texture image = !flag ? CurveUtility.GetIconCurve() : CurveUtility.GetIconKey();
                rect = new Rect(((rect.xMax - 10f) - (image.width / 2)) - 5f, rect.yMin + 3f, (float) image.width, (float) image.height);
                GUI.DrawTexture(rect, image, ScaleMode.ScaleToFit, true, 1f);
                GUI.color = color;
            }
        }

        private void DoCurveDropdown(Rect rect, AnimationWindowHierarchyNode node, int row, bool enabled)
        {
            rect = new Rect((rect.xMax - 10f) - 12f, rect.yMin + 2f, 22f, 12f);
            if (this.DoTreeViewButton(this.m_HierarchyItemButtonControlIDs[row], rect, GUIContent.none, this.m_AnimationCurveDropdown))
            {
                this.state.SelectHierarchyItem(node.id, false, false);
                AnimationWindowHierarchyNode[] source = new AnimationWindowHierarchyNode[] { node };
                this.GenerateMenu(source.ToList<AnimationWindowHierarchyNode>(), enabled).DropDown(rect);
                Event.current.Use();
            }
        }

        private void DoFoldout(AnimationWindowHierarchyNode node, Rect rect, float indent, int row)
        {
            if (base.m_TreeView.data.IsExpandable(node))
            {
                Rect position = rect;
                position.x = indent;
                position.width = TreeViewGUI.Styles.foldoutWidth;
                EditorGUI.BeginChangeCheck();
                bool expand = GUI.Toggle(position, this.m_HierarchyItemFoldControlIDs[row], base.m_TreeView.data.IsExpanded(node), GUIContent.none, TreeViewGUI.Styles.foldout);
                if (EditorGUI.EndChangeCheck())
                {
                    if (Event.current.alt)
                    {
                        base.m_TreeView.data.SetExpandedWithChildren(node, expand);
                    }
                    else
                    {
                        base.m_TreeView.data.SetExpanded(node, expand);
                    }
                }
            }
            else
            {
                AnimationWindowHierarchyPropertyNode node2 = node as AnimationWindowHierarchyPropertyNode;
                AnimationWindowHierarchyState state = base.m_TreeView.state as AnimationWindowHierarchyState;
                if ((node2 != null) && node2.isPptrNode)
                {
                    Rect rect3 = rect;
                    rect3.x = indent;
                    rect3.width = TreeViewGUI.Styles.foldoutWidth;
                    EditorGUI.BeginChangeCheck();
                    bool tallMode = state.GetTallMode(node2);
                    tallMode = GUI.Toggle(rect3, this.m_HierarchyItemFoldControlIDs[row], tallMode, GUIContent.none, TreeViewGUI.Styles.foldout);
                    if (EditorGUI.EndChangeCheck())
                    {
                        state.SetTallMode(node2, tallMode);
                    }
                }
            }
        }

        private void DoIconAndName(Rect rect, AnimationWindowHierarchyNode node, bool selected, bool focused, float indent)
        {
            EditorGUIUtility.SetIconSize(new Vector2(13f, 13f));
            if (Event.current.type == EventType.Repaint)
            {
                if (selected)
                {
                    TreeViewGUI.Styles.selectionStyle.Draw(rect, false, false, true, focused);
                }
                if (node is AnimationWindowHierarchyPropertyNode)
                {
                    rect.width -= 77f;
                }
                bool flag = AnimationWindowUtility.IsNodeLeftOverCurve(node);
                bool flag2 = AnimationWindowUtility.IsNodeAmbiguous(node);
                bool flag3 = AnimationWindowUtility.IsNodePhantom(node);
                string str = "";
                string tooltip = "";
                if (flag3)
                {
                    str = " (Default Value)";
                    tooltip = "Transform position, rotation and scale can't be partially animated. This value will be animated to the default value";
                }
                if (flag)
                {
                    str = " (Missing!)";
                    tooltip = "The GameObject or Component is missing (" + node.path + ")";
                }
                if (flag2)
                {
                    str = " (Duplicate GameObject name!)";
                    tooltip = "Target for curve is ambiguous since there are multiple GameObjects with same name (" + node.path + ")";
                }
                Color textColor = this.m_AnimationLineStyle.normal.textColor;
                Color color = textColor;
                if (node.depth == 0)
                {
                    string str3 = "";
                    if (node.curves.Length > 0)
                    {
                        AnimationWindowSelectionItem selectionBinding = node.curves[0].selectionBinding;
                        if (((selectionBinding != null) && (selectionBinding.rootGameObject != null)) && (selectionBinding.rootGameObject.transform.Find(node.path) == null))
                        {
                            flag = true;
                        }
                        string gameObjectName = this.GetGameObjectName(selectionBinding?.rootGameObject, node.path);
                        str3 = !string.IsNullOrEmpty(gameObjectName) ? (gameObjectName + " : ") : "";
                    }
                    TreeViewGUI.Styles.content = new GUIContent(str3 + node.displayName + str, this.GetIconForItem(node), tooltip);
                    color = !EditorGUIUtility.isProSkin ? Color.black : ((Color) (Color.gray * 1.35f));
                }
                else
                {
                    TreeViewGUI.Styles.content = new GUIContent(node.displayName + str, this.GetIconForItem(node), tooltip);
                    color = !EditorGUIUtility.isProSkin ? this.m_LightSkinPropertyTextColor : Color.gray;
                    Color color3 = !selected ? this.m_PhantomCurveColor : ((Color) (this.m_PhantomCurveColor * 1.4f));
                    color = !flag3 ? color : color3;
                }
                color = (!flag && !flag2) ? color : k_LeftoverCurveColor;
                this.SetStyleTextColor(this.m_AnimationLineStyle, color);
                rect.xMin += ((int) (indent + TreeViewGUI.Styles.foldoutWidth)) + this.m_AnimationLineStyle.margin.left;
                GUI.Label(rect, TreeViewGUI.Styles.content, this.m_AnimationLineStyle);
                this.SetStyleTextColor(this.m_AnimationLineStyle, textColor);
            }
            if (this.IsRenaming(node.id) && (Event.current.type != EventType.Layout))
            {
                base.GetRenameOverlay().editFieldRect = new Rect(rect.x + base.k_IndentWidth, rect.y, (rect.width - base.k_IndentWidth) - 1f, rect.height);
            }
        }

        protected void DoNodeGUI(Rect rect, AnimationWindowHierarchyNode node, bool selected, bool focused, int row)
        {
            this.InitStyles();
            if (!(node is AnimationWindowHierarchyMasterNode))
            {
                float indent = base.k_BaseIndent + ((node.depth + node.indent) * base.k_IndentWidth);
                if (node is AnimationWindowHierarchyAddButtonNode)
                {
                    if ((Event.current.type == EventType.MouseMove) && (s_WasInsideValueRectFrame >= 0))
                    {
                        if (s_WasInsideValueRectFrame >= (Time.frameCount - 1))
                        {
                            Event.current.Use();
                        }
                        else
                        {
                            s_WasInsideValueRectFrame = -1;
                        }
                    }
                    using (new EditorGUI.DisabledScope(!this.state.selection.canAddCurves))
                    {
                        this.DoAddCurveButton(rect, node, row);
                    }
                }
                else
                {
                    this.DoRowBackground(rect, row);
                    this.DoIconAndName(rect, node, selected, focused, indent);
                    this.DoFoldout(node, rect, indent, row);
                    bool enabled = false;
                    if (node.curves != null)
                    {
                        if (<>f__am$cache0 == null)
                        {
                            <>f__am$cache0 = curve => !curve.animationIsEditable;
                        }
                        enabled = !Array.Exists<AnimationWindowCurve>(node.curves, <>f__am$cache0);
                    }
                    using (new EditorGUI.DisabledScope(!enabled))
                    {
                        this.DoValueField(rect, node, row);
                    }
                    this.DoCurveDropdown(rect, node, row, enabled);
                    this.HandleContextMenu(rect, node, enabled);
                    this.DoCurveColorIndicator(rect, node);
                }
                EditorGUIUtility.SetIconSize(Vector2.zero);
            }
        }

        private void DoRowBackground(Rect rect, int row)
        {
            if (Event.current.type == EventType.Repaint)
            {
                if ((row % 2) == 0)
                {
                    this.m_AnimationRowEvenStyle.Draw(rect, false, false, false, false);
                }
                else
                {
                    this.m_AnimationRowOddStyle.Draw(rect, false, false, false, false);
                }
            }
        }

        private bool DoTreeViewButton(int id, Rect position, GUIContent content, GUIStyle style)
        {
            Event current = Event.current;
            switch (current.GetTypeForControl(id))
            {
                case EventType.Repaint:
                    style.Draw(position, content, id, false);
                    break;

                case EventType.MouseDown:
                    if (position.Contains(current.mousePosition) && (current.button == 0))
                    {
                        GUIUtility.hotControl = id;
                        current.Use();
                    }
                    break;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl == id)
                    {
                        GUIUtility.hotControl = 0;
                        current.Use();
                        if (position.Contains(current.mousePosition))
                        {
                            return true;
                        }
                    }
                    break;
            }
            return false;
        }

        private void DoValueField(Rect rect, AnimationWindowHierarchyNode node, int row)
        {
            bool flag = false;
            if (node is AnimationWindowHierarchyPropertyNode)
            {
                AnimationWindowCurve[] curves = node.curves;
                if ((curves == null) || (curves.Length == 0))
                {
                    return;
                }
                AnimationWindowCurve curve = curves[0];
                object currentValue = CurveBindingUtility.GetCurrentValue(this.state, curve);
                if (currentValue is float)
                {
                    float num = (float) currentValue;
                    Rect dragHotZone = new Rect((rect.xMax - 75f) - 15f, rect.y, 15f, rect.height);
                    Rect position = new Rect(rect.xMax - 75f, rect.y, 50f, rect.height);
                    if ((Event.current.type == EventType.MouseMove) && position.Contains(Event.current.mousePosition))
                    {
                        s_WasInsideValueRectFrame = Time.frameCount;
                    }
                    EditorGUI.BeginChangeCheck();
                    if (curve.valueType == typeof(bool))
                    {
                        num = !GUI.Toggle(position, this.m_HierarchyItemValueControlIDs[row], !(num == 0f), GUIContent.none, EditorStyles.toggle) ? ((float) 0) : ((float) 1);
                    }
                    else
                    {
                        int id = this.m_HierarchyItemValueControlIDs[row];
                        bool flag2 = (((GUIUtility.keyboardControl == id) && EditorGUIUtility.editingTextField) && (Event.current.type == EventType.KeyDown)) && ((Event.current.character == '\n') || (Event.current.character == '\x0003'));
                        if (((EditorGUI.s_RecycledEditor.controlID == id) && (Event.current.type == EventType.MouseDown)) && position.Contains(Event.current.mousePosition))
                        {
                            GUIUtility.keyboardControl = id;
                        }
                        num = EditorGUI.DoFloatField(EditorGUI.s_RecycledEditor, position, dragHotZone, id, num, EditorGUI.kFloatFieldFormatString, this.m_AnimationSelectionTextField, true);
                        if (flag2)
                        {
                            GUI.changed = true;
                            Event.current.Use();
                        }
                    }
                    if (float.IsInfinity(num) || float.IsNaN(num))
                    {
                        num = 0f;
                    }
                    if (EditorGUI.EndChangeCheck())
                    {
                        string undoLabel = "Edit Key";
                        float num3 = this.state.currentTime - curve.timeOffset;
                        AnimationKeyTime time = AnimationKeyTime.Time(num3, curve.clip.frameRate);
                        AnimationWindowKeyframe keyframe = null;
                        foreach (AnimationWindowKeyframe keyframe2 in curve.m_Keyframes)
                        {
                            if (Mathf.Approximately(keyframe2.time, num3))
                            {
                                keyframe = keyframe2;
                            }
                        }
                        if (keyframe == null)
                        {
                            AnimationWindowUtility.AddKeyframeToCurve(curve, num, curve.valueType, time);
                        }
                        else
                        {
                            keyframe.value = num;
                        }
                        this.state.SaveCurve(curve, undoLabel);
                        flag = true;
                    }
                }
            }
            if (flag)
            {
                this.state.ResampleAnimation();
            }
        }

        private GenericMenu GenerateMenu(List<AnimationWindowHierarchyNode> interactedNodes, bool enabled)
        {
            List<AnimationWindowCurve> curvesAffectedByNodes = this.GetCurvesAffectedByNodes(interactedNodes, false);
            List<AnimationWindowCurve> list2 = this.GetCurvesAffectedByNodes(interactedNodes, true);
            bool flag = (curvesAffectedByNodes.Count == 1) && AnimationWindowUtility.ForceGrouping(curvesAffectedByNodes[0].binding);
            GenericMenu menu = new GenericMenu();
            GUIContent content = new GUIContent(((curvesAffectedByNodes.Count <= 1) && !flag) ? "Remove Property" : "Remove Properties");
            if (!enabled)
            {
                menu.AddDisabledItem(content);
            }
            else
            {
                menu.AddItem(content, false, new GenericMenu.MenuFunction(this.RemoveCurvesFromSelectedNodes));
            }
            bool flag2 = true;
            EditorCurveBinding[] curves = new EditorCurveBinding[list2.Count];
            for (int i = 0; i < list2.Count; i++)
            {
                curves[i] = list2[i].binding;
            }
            RotationCurveInterpolation.Mode rotationInterpolationMode = this.GetRotationInterpolationMode(curves);
            if (rotationInterpolationMode == RotationCurveInterpolation.Mode.Undefined)
            {
                flag2 = false;
            }
            else
            {
                foreach (AnimationWindowHierarchyNode node in interactedNodes)
                {
                    if (!(node is AnimationWindowHierarchyPropertyGroupNode))
                    {
                        flag2 = false;
                    }
                }
            }
            if (flag2)
            {
                string str = !this.state.activeAnimationClip.legacy ? "" : " (Not fully supported in Legacy)";
                GenericMenu.MenuFunction2 function = null;
                menu.AddItem(new GUIContent("Interpolation/Euler Angles" + str), rotationInterpolationMode == RotationCurveInterpolation.Mode.RawEuler, !enabled ? function : new GenericMenu.MenuFunction2(this.ChangeRotationInterpolation), RotationCurveInterpolation.Mode.RawEuler);
                menu.AddItem(new GUIContent("Interpolation/Euler Angles (Quaternion)"), rotationInterpolationMode == RotationCurveInterpolation.Mode.Baked, !enabled ? function : new GenericMenu.MenuFunction2(this.ChangeRotationInterpolation), RotationCurveInterpolation.Mode.Baked);
                menu.AddItem(new GUIContent("Interpolation/Quaternion"), rotationInterpolationMode == RotationCurveInterpolation.Mode.NonBaked, !enabled ? function : new GenericMenu.MenuFunction2(this.ChangeRotationInterpolation), RotationCurveInterpolation.Mode.NonBaked);
            }
            if (this.state.previewing)
            {
                menu.AddSeparator("");
                bool flag3 = true;
                bool flag4 = true;
                foreach (AnimationWindowCurve curve in curvesAffectedByNodes)
                {
                    if (!curve.HasKeyframe(this.state.time))
                    {
                        flag3 = false;
                    }
                    else
                    {
                        flag4 = false;
                    }
                }
                string text = "Add Key";
                if (flag3 || !enabled)
                {
                    menu.AddDisabledItem(new GUIContent(text));
                }
                else
                {
                    menu.AddItem(new GUIContent(text), false, new GenericMenu.MenuFunction2(this.AddKeysAtCurrentTime), curvesAffectedByNodes);
                }
                text = "Delete Key";
                if (flag4 || !enabled)
                {
                    menu.AddDisabledItem(new GUIContent(text));
                    return menu;
                }
                menu.AddItem(new GUIContent(text), false, new GenericMenu.MenuFunction2(this.DeleteKeysAtCurrentTime), curvesAffectedByNodes);
            }
            return menu;
        }

        private List<AnimationWindowCurve> GetCurvesAffectedByNodes(List<AnimationWindowHierarchyNode> nodes, bool includeLinkedCurves)
        {
            List<AnimationWindowCurve> source = new List<AnimationWindowCurve>();
            foreach (AnimationWindowHierarchyNode node in nodes)
            {
                AnimationWindowHierarchyNode parent = node;
                if ((parent.parent is AnimationWindowHierarchyPropertyGroupNode) && includeLinkedCurves)
                {
                    parent = (AnimationWindowHierarchyNode) parent.parent;
                }
                if ((parent.curves != null) && (parent.curves.Length > 0))
                {
                    if ((parent is AnimationWindowHierarchyPropertyGroupNode) || (parent is AnimationWindowHierarchyPropertyNode))
                    {
                        source.AddRange(AnimationWindowUtility.FilterCurves(parent.curves, parent.path, parent.animatableObjectType, parent.propertyName));
                    }
                    else
                    {
                        source.AddRange(AnimationWindowUtility.FilterCurves(parent.curves, parent.path, parent.animatableObjectType));
                    }
                }
            }
            return source.Distinct<AnimationWindowCurve>().ToList<AnimationWindowCurve>();
        }

        public override void GetFirstAndLastRowVisible(out int firstRowVisible, out int lastRowVisible)
        {
            firstRowVisible = 0;
            lastRowVisible = base.m_TreeView.data.rowCount - 1;
        }

        private string GetGameObjectName(GameObject rootGameObject, string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return ((rootGameObject == null) ? "" : rootGameObject.name);
            }
            char[] separator = new char[] { '/' };
            string[] strArray = path.Split(separator);
            return strArray[strArray.Length - 1];
        }

        protected override Texture GetIconForItem(TreeViewItem item)
        {
            if (item != null)
            {
                return item.icon;
            }
            return null;
        }

        public float GetNodeHeight(AnimationWindowHierarchyNode node)
        {
            if (node is AnimationWindowHierarchyAddButtonNode)
            {
                return 40f;
            }
            AnimationWindowHierarchyState state = base.m_TreeView.state as AnimationWindowHierarchyState;
            return (!state.GetTallMode(node) ? 16f : 32f);
        }

        private string GetPathWithoutChildmostGameObject(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return "";
            }
            int num = path.LastIndexOf('/');
            return path.Substring(0, num + 1);
        }

        private RotationCurveInterpolation.Mode GetRotationInterpolationMode(EditorCurveBinding[] curves)
        {
            if ((curves == null) || (curves.Length == 0))
            {
                return RotationCurveInterpolation.Mode.Undefined;
            }
            RotationCurveInterpolation.Mode modeFromCurveData = RotationCurveInterpolation.GetModeFromCurveData(curves[0]);
            for (int i = 1; i < curves.Length; i++)
            {
                RotationCurveInterpolation.Mode mode3 = RotationCurveInterpolation.GetModeFromCurveData(curves[i]);
                if (modeFromCurveData != mode3)
                {
                    return RotationCurveInterpolation.Mode.Undefined;
                }
            }
            return modeFromCurveData;
        }

        public override Rect GetRowRect(int row, float rowWidth)
        {
            IList<TreeViewItem> rows = base.m_TreeView.data.GetRows();
            AnimationWindowHierarchyNode node = rows[row] as AnimationWindowHierarchyNode;
            if (!node.topPixel.HasValue)
            {
                node.topPixel = new float?(this.GetTopPixelOfRow(row, rows));
            }
            return new Rect(0f, node.topPixel.Value, rowWidth, this.GetNodeHeight(node));
        }

        private float GetTopPixelOfRow(int row, IList<TreeViewItem> rows)
        {
            float num = 0f;
            for (int i = 0; (i < row) && (i < rows.Count); i++)
            {
                AnimationWindowHierarchyNode node = rows[i] as AnimationWindowHierarchyNode;
                num += this.GetNodeHeight(node);
            }
            return num;
        }

        public override Vector2 GetTotalSize()
        {
            IList<TreeViewItem> rows = base.m_TreeView.data.GetRows();
            float y = 0f;
            for (int i = 0; i < rows.Count; i++)
            {
                AnimationWindowHierarchyNode node = rows[i] as AnimationWindowHierarchyNode;
                y += this.GetNodeHeight(node);
            }
            return new Vector2(1f, y);
        }

        private void HandleContextMenu(Rect rect, AnimationWindowHierarchyNode node, bool enabled)
        {
            if ((Event.current.type == EventType.ContextClick) && rect.Contains(Event.current.mousePosition))
            {
                this.state.SelectHierarchyItem(node.id, true, true);
                this.GenerateMenu(this.state.selectedHierarchyNodes, enabled).ShowAsContext();
                Event.current.Use();
            }
        }

        private void HandleDelete()
        {
            if (base.m_TreeView.HasFocus())
            {
                switch (Event.current.type)
                {
                    case EventType.ExecuteCommand:
                        if ((Event.current.commandName == "SoftDelete") || (Event.current.commandName == "Delete"))
                        {
                            if (Event.current.type == EventType.ExecuteCommand)
                            {
                                this.RemoveCurvesFromSelectedNodes();
                            }
                            Event.current.Use();
                        }
                        break;

                    case EventType.KeyDown:
                        if ((Event.current.keyCode == KeyCode.Backspace) || (Event.current.keyCode == KeyCode.Delete))
                        {
                            this.RemoveCurvesFromSelectedNodes();
                            Event.current.Use();
                        }
                        break;
                }
            }
        }

        protected void InitStyles()
        {
            if (this.m_AnimationRowEvenStyle == null)
            {
                this.m_AnimationRowEvenStyle = new GUIStyle("AnimationRowEven");
            }
            if (this.m_AnimationRowOddStyle == null)
            {
                this.m_AnimationRowOddStyle = new GUIStyle("AnimationRowOdd");
            }
            if (this.m_AnimationSelectionTextField == null)
            {
                this.m_AnimationSelectionTextField = new GUIStyle("AnimationSelectionTextField");
            }
            if (this.m_AnimationLineStyle == null)
            {
                this.m_AnimationLineStyle = new GUIStyle(TreeViewGUI.Styles.lineStyle);
                this.m_AnimationLineStyle.padding.left = 0;
            }
            if (this.m_AnimationCurveDropdown == null)
            {
                this.m_AnimationCurveDropdown = new GUIStyle("AnimPropDropdown");
            }
        }

        private void MaintainTreeviewStateAfterRotationInterpolation(RotationCurveInterpolation.Mode newMode)
        {
            List<int> selectedIDs = this.state.hierarchyState.selectedIDs;
            List<int> expandedIDs = this.state.hierarchyState.expandedIDs;
            List<int> list3 = new List<int>();
            List<int> list4 = new List<int>();
            for (int i = 0; i < selectedIDs.Count; i++)
            {
                AnimationWindowHierarchyNode node = this.state.hierarchyData.FindItem(selectedIDs[i]) as AnimationWindowHierarchyNode;
                if ((node != null) && !node.propertyName.Equals(RotationCurveInterpolation.GetPrefixForInterpolation(newMode)))
                {
                    char[] separator = new char[] { '.' };
                    string oldValue = node.propertyName.Split(separator)[0];
                    string str2 = node.propertyName.Replace(oldValue, RotationCurveInterpolation.GetPrefixForInterpolation(newMode));
                    list3.Add(selectedIDs[i]);
                    list4.Add((node.path + node.animatableObjectType.Name + str2).GetHashCode());
                }
            }
            for (int j = 0; j < list3.Count; j++)
            {
                if (selectedIDs.Contains(list3[j]))
                {
                    int index = selectedIDs.IndexOf(list3[j]);
                    selectedIDs[index] = list4[j];
                }
                if (expandedIDs.Contains(list3[j]))
                {
                    int num4 = expandedIDs.IndexOf(list3[j]);
                    expandedIDs[num4] = list4[j];
                }
                if (this.state.hierarchyState.lastClickedID == list3[j])
                {
                    this.state.hierarchyState.lastClickedID = list4[j];
                }
            }
            this.state.hierarchyState.selectedIDs = new List<int>(selectedIDs);
            this.state.hierarchyState.expandedIDs = new List<int>(expandedIDs);
        }

        private void OnNewCurveAdded(AddCurvesPopupPropertyNode node)
        {
        }

        public override void OnRowGUI(Rect rowRect, TreeViewItem node, int row, bool selected, bool focused)
        {
            AnimationWindowHierarchyNode node2 = node as AnimationWindowHierarchyNode;
            this.DoNodeGUI(rowRect, node2, selected, focused, row);
        }

        private void RemoveCurvesFromNodes(List<AnimationWindowHierarchyNode> nodes)
        {
            string undoLabel = "Remove Curve";
            this.state.SaveKeySelection(undoLabel);
            foreach (AnimationWindowHierarchyNode node in nodes)
            {
                AnimationWindowHierarchyNode parent = node;
                if (((parent.parent is AnimationWindowHierarchyPropertyGroupNode) && parent.binding.HasValue) && AnimationWindowUtility.ForceGrouping(parent.binding.Value))
                {
                    parent = (AnimationWindowHierarchyNode) parent.parent;
                }
                if (parent.curves != null)
                {
                    List<AnimationWindowCurve> list = null;
                    if ((parent is AnimationWindowHierarchyPropertyGroupNode) || (parent is AnimationWindowHierarchyPropertyNode))
                    {
                        list = AnimationWindowUtility.FilterCurves(parent.curves.ToArray<AnimationWindowCurve>(), parent.path, parent.animatableObjectType, parent.propertyName);
                    }
                    else
                    {
                        list = AnimationWindowUtility.FilterCurves(parent.curves.ToArray<AnimationWindowCurve>(), parent.path, parent.animatableObjectType);
                    }
                    foreach (AnimationWindowCurve curve in list)
                    {
                        this.state.RemoveCurve(curve, undoLabel);
                    }
                }
            }
            base.m_TreeView.ReloadData();
            this.state.controlInterface.ResampleAnimation();
        }

        private void RemoveCurvesFromSelectedNodes()
        {
            this.RemoveCurvesFromNodes(this.state.selectedHierarchyNodes);
        }

        protected override void RenameEnded()
        {
            string name = base.GetRenameOverlay().name;
            string originalName = base.GetRenameOverlay().originalName;
            if (name != originalName)
            {
                Undo.RecordObject(this.state.activeAnimationClip, "Rename Curve");
                foreach (AnimationWindowCurve curve in this.m_RenamedNode.curves)
                {
                    EditorCurveBinding renamedBinding = AnimationWindowUtility.GetRenamedBinding(curve.binding, name);
                    if (AnimationWindowUtility.CurveExists(renamedBinding, this.state.allCurves.ToArray()))
                    {
                        UnityEngine.Debug.LogWarning("Curve already exists, renaming cancelled.");
                    }
                    else
                    {
                        AnimationWindowUtility.RenameCurvePath(curve, renamedBinding, curve.clip);
                    }
                }
            }
            this.m_RenamedNode = null;
        }

        private void SetStyleTextColor(GUIStyle style, Color color)
        {
            style.normal.textColor = color;
            style.focused.textColor = color;
            style.active.textColor = color;
            style.hover.textColor = color;
        }

        protected override void SyncFakeItem()
        {
        }

        public AnimationWindowState state { get; set; }
    }
}

