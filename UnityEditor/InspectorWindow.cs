namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text;
    using UnityEditor.VersionControl;
    using UnityEditorInternal;
    using UnityEditorInternal.VersionControl;
    using UnityEngine;
    using UnityEngine.Profiling;

    [EditorWindowTitle(title="Inspector", useTypeNameAsIconName=true)]
    internal class InspectorWindow : EditorWindow, IHasCustomMenu
    {
        [CompilerGenerated]
        private static Func<UnityEngine.Object, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<UnityEngine.Object, bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<UnityEngine.Object, GameObject> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<UnityEngine.Object, bool> <>f__am$cache3;
        [CompilerGenerated]
        private static Converter<UnityEngine.Object, Component> <>f__am$cache4;
        [CompilerGenerated]
        private static Converter<UnityEngine.Object, Component> <>f__am$cache5;
        [CompilerGenerated]
        private static Converter<UnityEngine.Object, Component> <>f__am$cache6;
        [CompilerGenerated]
        private static Converter<UnityEngine.Object, Component> <>f__am$cache7;
        private const long delayRepaintWhilePlayingAnimation = 150L;
        private const float kBottomToolbarHeight = 17f;
        private const string kEditorDraggingApplicableDragString = "InspectorEditorDraggingApplicable";
        internal const int kInspectorPaddingLeft = 14;
        internal const int kInspectorPaddingRight = 4;
        private static readonly List<InspectorWindow> m_AllInspectors = new List<InspectorWindow>();
        private AssetBundleNameGUI m_AssetBundleNameGUI = new AssetBundleNameGUI();
        private int m_EditorDraggingLastIndex = -1;
        private float m_EditorDraggingLastMarkerY = 0f;
        private bool m_EditorDraggingTargetAbove;
        private int m_EditorDraggingTargetIndex = -1;
        public InspectorMode m_InspectorMode = InspectorMode.Normal;
        private bool m_InvalidateGUIBlockCache = true;
        private bool m_IsOpenForEdit = false;
        private LabelGUI m_LabelGUI = new LabelGUI();
        private Editor m_LastInteractedEditor;
        private double m_lastRenderedTime;
        [SerializeField]
        private PreviewResizer m_PreviewResizer = new PreviewResizer();
        private List<IPreviewable> m_Previews;
        [SerializeField]
        private PreviewWindow m_PreviewWindow;
        private bool m_ResetKeyboardControl;
        public Vector2 m_ScrollPosition;
        private IPreviewable m_SelectedPreview;
        protected ActiveEditorTracker m_Tracker;
        private TypeSelectionList m_TypeSelectionList = null;
        private static bool s_AllOptimizedGUIBlocksNeedsRebuild;
        public static InspectorWindow s_CurrentInspectorWindow;
        private long s_LastUpdateWhilePlayingAnimation = 0L;
        private static Styles s_Styles;

        private void AddComponentButton(Editor[] editors)
        {
            Editor firstNonImportInspectorEditor = this.GetFirstNonImportInspectorEditor(editors);
            if (((firstNonImportInspectorEditor != null) && (firstNonImportInspectorEditor.target != null)) && ((firstNonImportInspectorEditor.target is GameObject) && firstNonImportInspectorEditor.IsEnabled()))
            {
                EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayout.FlexibleSpace();
                GUIContent addComponentLabel = s_Styles.addComponentLabel;
                Rect position = GUILayoutUtility.GetRect(addComponentLabel, styles.addComponentButtonStyle);
                if (Event.current.type == EventType.Repaint)
                {
                    this.DrawSplitLine(position.y - 11f);
                }
                Event current = Event.current;
                bool flag = false;
                if ((current.type == EventType.ExecuteCommand) && (current.commandName == "OpenAddComponentDropdown"))
                {
                    flag = true;
                    current.Use();
                }
                if (EditorGUI.DropdownButton(position, addComponentLabel, FocusType.Passive, styles.addComponentButtonStyle) || flag)
                {
                    if (<>f__am$cache2 == null)
                    {
                        <>f__am$cache2 = o => (GameObject) o;
                    }
                    if (AddComponentWindow.Show(position, Enumerable.Select<UnityEngine.Object, GameObject>(firstNonImportInspectorEditor.targets, <>f__am$cache2).ToArray<GameObject>()))
                    {
                        GUIUtility.ExitGUI();
                    }
                }
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
        }

        public virtual void AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Normal"), this.m_InspectorMode == InspectorMode.Normal, new GenericMenu.MenuFunction(this.SetNormal));
            menu.AddItem(new GUIContent("Debug"), this.m_InspectorMode == InspectorMode.Debug, new GenericMenu.MenuFunction(this.SetDebug));
            if (Unsupported.IsDeveloperBuild())
            {
                menu.AddItem(new GUIContent("Debug-Internal"), this.m_InspectorMode == InspectorMode.DebugInternal, new GenericMenu.MenuFunction(this.SetDebugInternal));
            }
            menu.AddSeparator(string.Empty);
            menu.AddItem(new GUIContent("Lock"), this.isLocked, new GenericMenu.MenuFunction(this.FlipLocked));
        }

        protected void AssignAssetEditor(Editor[] editors)
        {
            if ((editors.Length > 1) && (editors[0] is AssetImporterInspector))
            {
                (editors[0] as AssetImporterInspector).assetEditor = editors[1];
            }
        }

        private void Awake()
        {
            if (!m_AllInspectors.Contains(this))
            {
                m_AllInspectors.Add(this);
            }
        }

        protected string BuildTooltip(Asset asset, Asset metaAsset)
        {
            StringBuilder builder = new StringBuilder();
            if (asset != null)
            {
                builder.AppendLine("Asset:");
                builder.AppendLine(asset.AllStateToString());
            }
            if (metaAsset != null)
            {
                builder.AppendLine("Meta file:");
                builder.AppendLine(metaAsset.AllStateToString());
            }
            return builder.ToString();
        }

        private void CheckDragAndDrop(Editor[] editors)
        {
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandHeight(true) };
            Rect rect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, options);
            if (rect.Contains(Event.current.mousePosition))
            {
                Editor firstNonImportInspectorEditor = this.GetFirstNonImportInspectorEditor(editors);
                if (firstNonImportInspectorEditor != null)
                {
                    DoInspectorDragAndDrop(rect, firstNonImportInspectorEditor.targets);
                }
                if (Event.current.type == EventType.MouseDown)
                {
                    GUIUtility.keyboardControl = 0;
                    Event.current.Use();
                }
            }
            this.HandleEditorDragging(rect);
        }

        protected virtual void CreatePreviewables()
        {
            if (this.m_Previews == null)
            {
                this.m_Previews = new List<IPreviewable>();
                if (this.tracker.activeEditors.Length != 0)
                {
                    foreach (Editor editor in this.tracker.activeEditors)
                    {
                        IEnumerable<IPreviewable> previewsForType = this.GetPreviewsForType(editor);
                        foreach (IPreviewable previewable in previewsForType)
                        {
                            this.m_Previews.Add(previewable);
                        }
                    }
                }
            }
        }

        protected virtual void CreateTracker()
        {
            <CreateTracker>c__AnonStorey0 storey = new <CreateTracker>c__AnonStorey0();
            if (this.m_Tracker != null)
            {
                this.m_Tracker.inspectorMode = this.m_InspectorMode;
            }
            else
            {
                storey.sharedTracker = ActiveEditorTracker.sharedTracker;
                bool flag = Enumerable.Any<InspectorWindow>(m_AllInspectors, new Func<InspectorWindow, bool>(storey.<>m__0));
                this.m_Tracker = !flag ? ActiveEditorTracker.sharedTracker : new ActiveEditorTracker();
                this.m_Tracker.inspectorMode = this.m_InspectorMode;
                this.m_Tracker.RebuildIfNecessary();
            }
        }

        private void DetachPreview()
        {
            Event.current.Use();
            this.m_PreviewWindow = ScriptableObject.CreateInstance(typeof(PreviewWindow)) as PreviewWindow;
            this.m_PreviewWindow.SetParentInspector(this);
            this.m_PreviewWindow.Show();
            base.Repaint();
            GUIUtility.ExitGUI();
        }

        private void DisplayDeprecationMessageIfNecessary(Editor editor)
        {
            if ((editor != null) && (editor.target != null))
            {
                ObsoleteAttribute customAttribute = (ObsoleteAttribute) Attribute.GetCustomAttribute(editor.target.GetType(), typeof(ObsoleteAttribute));
                if (customAttribute != null)
                {
                    string message = !string.IsNullOrEmpty(customAttribute.Message) ? customAttribute.Message : "This component has been marked as obsolete.";
                    EditorGUILayout.HelpBox(message, !customAttribute.IsError ? MessageType.Warning : MessageType.Error);
                }
            }
        }

        private static void DoInspectorDragAndDrop(Rect rect, UnityEngine.Object[] targets)
        {
            if (Dragging(rect))
            {
                DragAndDrop.visualMode = InternalEditorUtility.InspectorWindowDrag(targets, Event.current.type == EventType.DragPerform);
                if (Event.current.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                }
            }
        }

        private static bool Dragging(Rect rect) => 
            (((Event.current.type == EventType.DragUpdated) || (Event.current.type == EventType.DragPerform)) && rect.Contains(Event.current.mousePosition));

        private void DrawEditor(Editor editor, int editorIndex, bool rebuildOptimizedGUIBlock, ref bool showImportedObjectBarNext, ref Rect importedObjectBarRect)
        {
            if (editor != null)
            {
                bool isInspectorExpanded;
                UnityEngine.Object target = editor.target;
                GUIUtility.GetControlID(target.GetInstanceID(), FocusType.Passive);
                EditorGUIUtility.ResetGUIState();
                GUILayoutGroup topLevel = GUILayoutUtility.current.topLevel;
                int visible = this.tracker.GetVisible(editorIndex);
                if (visible == -1)
                {
                    isInspectorExpanded = InternalEditorUtility.GetIsInspectorExpanded(target);
                    this.tracker.SetVisible(editorIndex, !isInspectorExpanded ? 0 : 1);
                }
                else
                {
                    isInspectorExpanded = visible == 1;
                }
                rebuildOptimizedGUIBlock |= editor.isInspectorDirty;
                if (Event.current.type == EventType.Repaint)
                {
                    editor.isInspectorDirty = false;
                }
                ScriptAttributeUtility.propertyHandlerCache = editor.propertyHandlerCache;
                bool flag2 = this.EditorHasLargeHeader(editorIndex);
                if (flag2)
                {
                    string message = string.Empty;
                    bool flag3 = editor.IsOpenForEdit(out message);
                    if (showImportedObjectBarNext)
                    {
                        showImportedObjectBarNext = false;
                        GUILayout.Space(15f);
                        importedObjectBarRect = GUILayoutUtility.GetRect((float) 16f, (float) 16f);
                        importedObjectBarRect.height = 17f;
                    }
                    isInspectorExpanded = true;
                    using (new EditorGUI.DisabledScope(!flag3))
                    {
                        editor.DrawHeader();
                    }
                }
                if (editor.target is AssetImporter)
                {
                    showImportedObjectBarNext = true;
                }
                bool flag4 = false;
                if (((editor is GenericInspector) && (CustomEditorAttributes.FindCustomEditorType(target, false) != null)) && (this.m_InspectorMode != InspectorMode.DebugInternal))
                {
                    if (this.m_InspectorMode == InspectorMode.Normal)
                    {
                        flag4 = true;
                    }
                    else if (target is AssetImporter)
                    {
                        flag4 = true;
                    }
                }
                Rect dragRect = new Rect();
                if (!flag2)
                {
                    using (new EditorGUI.DisabledScope(!editor.IsEnabled()))
                    {
                        bool isExpanded = EditorGUILayout.InspectorTitlebar(isInspectorExpanded, editor.targets, editor.CanBeExpandedViaAFoldout());
                        if (isInspectorExpanded != isExpanded)
                        {
                            this.tracker.SetVisible(editorIndex, !isExpanded ? 0 : 1);
                            InternalEditorUtility.SetIsInspectorExpanded(target, isExpanded);
                            if (isExpanded)
                            {
                                this.m_LastInteractedEditor = editor;
                            }
                            else if (this.m_LastInteractedEditor == editor)
                            {
                                this.m_LastInteractedEditor = null;
                            }
                        }
                    }
                    dragRect = GUILayoutUtility.GetLastRect();
                }
                if (flag4 && isInspectorExpanded)
                {
                    GUILayout.Label("Multi-object editing not supported.", EditorStyles.helpBox, new GUILayoutOption[0]);
                }
                else
                {
                    this.DisplayDeprecationMessageIfNecessary(editor);
                    EditorGUIUtility.ResetGUIState();
                    Rect componentRect = new Rect();
                    using (new EditorGUI.DisabledScope(!editor.IsEnabled()))
                    {
                        float num2;
                        OptimizedGUIBlock block;
                        GenericInspector inspector = editor as GenericInspector;
                        if (inspector != null)
                        {
                            inspector.m_InspectorMode = this.m_InspectorMode;
                        }
                        EditorGUIUtility.hierarchyMode = true;
                        EditorGUIUtility.wideMode = base.position.width > 330f;
                        ScriptAttributeUtility.propertyHandlerCache = editor.propertyHandlerCache;
                        if (editor.GetOptimizedGUIBlock(rebuildOptimizedGUIBlock, isInspectorExpanded, out block, out num2))
                        {
                            componentRect = GUILayoutUtility.GetRect(0f, !isInspectorExpanded ? 0f : num2);
                            this.HandleLastInteractedEditor(componentRect, editor);
                            if (Event.current.type == EventType.Layout)
                            {
                                return;
                            }
                            if (block.Begin(rebuildOptimizedGUIBlock, componentRect) && isInspectorExpanded)
                            {
                                GUI.changed = false;
                                editor.OnOptimizedInspectorGUI(componentRect);
                            }
                            block.End();
                        }
                        else
                        {
                            if (isInspectorExpanded)
                            {
                                GUIStyle style = !editor.UseDefaultMargins() ? GUIStyle.none : EditorStyles.inspectorDefaultMargins;
                                componentRect = EditorGUILayout.BeginVertical(style, new GUILayoutOption[0]);
                                this.HandleLastInteractedEditor(componentRect, editor);
                                GUI.changed = false;
                                try
                                {
                                    editor.OnInspectorGUI();
                                }
                                catch (Exception exception)
                                {
                                    if (GUIUtility.ShouldRethrowException(exception))
                                    {
                                        throw;
                                    }
                                    Debug.LogException(exception);
                                }
                                EditorGUILayout.EndVertical();
                            }
                            if (Event.current.type == EventType.Used)
                            {
                                return;
                            }
                        }
                    }
                    this.HandleEditorDragging(editorIndex, dragRect, componentRect);
                    if (GUILayoutUtility.current.topLevel != topLevel)
                    {
                        if (!GUILayoutUtility.current.layoutGroups.Contains(topLevel))
                        {
                            Debug.LogError("Expected top level layout group missing! Too many GUILayout.EndScrollView/EndVertical/EndHorizontal?");
                            GUIUtility.ExitGUI();
                        }
                        else
                        {
                            Debug.LogWarning("Unexpected top level layout group! Missing GUILayout.EndScrollView/EndVertical/EndHorizontal?");
                            while (GUILayoutUtility.current.topLevel != topLevel)
                            {
                                GUILayoutUtility.EndLayoutGroup();
                            }
                        }
                    }
                    this.HandleComponentScreenshot(componentRect, editor);
                }
            }
        }

        private void DrawEditors(Editor[] editors)
        {
            if (editors.Length != 0)
            {
                UnityEngine.Object inspectedObject = this.GetInspectedObject();
                string message = string.Empty;
                GUILayout.Space(0f);
                if (inspectedObject is Material)
                {
                    for (int j = 0; (j <= 1) && (j < editors.Length); j++)
                    {
                        MaterialEditor editor = editors[j] as MaterialEditor;
                        if (editor != null)
                        {
                            editor.forceVisible = true;
                            break;
                        }
                    }
                }
                bool rebuildOptimizedGUIBlock = false;
                if (Event.current.type == EventType.Repaint)
                {
                    if ((inspectedObject != null) && (this.m_IsOpenForEdit != Editor.IsAppropriateFileOpenForEdit(inspectedObject, out message)))
                    {
                        this.m_IsOpenForEdit = !this.m_IsOpenForEdit;
                        rebuildOptimizedGUIBlock = true;
                    }
                    if (this.m_InvalidateGUIBlockCache)
                    {
                        rebuildOptimizedGUIBlock = true;
                        this.m_InvalidateGUIBlockCache = false;
                    }
                }
                else if ((Event.current.type == EventType.ExecuteCommand) && (Event.current.commandName == "EyeDropperUpdate"))
                {
                    rebuildOptimizedGUIBlock = true;
                }
                Editor.m_AllowMultiObjectAccess = true;
                bool showImportedObjectBarNext = false;
                Rect importedObjectBarRect = new Rect();
                for (int i = 0; i < editors.Length; i++)
                {
                    if (this.ShouldCullEditor(editors, i))
                    {
                        if (Event.current.type == EventType.Repaint)
                        {
                            editors[i].isInspectorDirty = false;
                        }
                    }
                    else
                    {
                        bool textFieldInput = GUIUtility.textFieldInput;
                        this.DrawEditor(editors[i], i, rebuildOptimizedGUIBlock, ref showImportedObjectBarNext, ref importedObjectBarRect);
                        if (((Event.current.type == EventType.Repaint) && !textFieldInput) && GUIUtility.textFieldInput)
                        {
                            FlushOptimizedGUIBlock(editors[i]);
                        }
                    }
                }
                EditorGUIUtility.ResetGUIState();
                if (importedObjectBarRect.height > 0f)
                {
                    GUI.BeginGroup(importedObjectBarRect);
                    GUI.Label(new Rect(0f, 0f, importedObjectBarRect.width, importedObjectBarRect.height), "Imported Object", "OL Title");
                    GUI.EndGroup();
                }
            }
        }

        private void DrawPreviewAndLabels()
        {
            if ((this.m_PreviewWindow != null) && (Event.current.type == EventType.Repaint))
            {
                this.m_PreviewWindow.Repaint();
            }
            IPreviewable[] editorsWithPreviews = this.GetEditorsWithPreviews(this.tracker.activeEditors);
            IPreviewable editorThatControlsPreview = this.GetEditorThatControlsPreview(editorsWithPreviews);
            bool flag = ((editorThatControlsPreview != null) && editorThatControlsPreview.HasPreviewGUI()) && (this.m_PreviewWindow == null);
            UnityEngine.Object[] inspectedAssets = this.GetInspectedAssets();
            bool flag2 = inspectedAssets.Length > 0;
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = a => !(a is MonoScript) && AssetDatabase.IsMainAsset(a);
            }
            bool flag3 = Enumerable.Any<UnityEngine.Object>(inspectedAssets, <>f__am$cache0);
            if (flag || flag2)
            {
                GUIContent preTitle;
                float num5;
                Event current = Event.current;
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Height(17f) };
                Rect position = EditorGUILayout.BeginHorizontal(GUIContent.none, styles.preToolbar, options);
                Rect rect3 = new Rect();
                GUILayout.FlexibleSpace();
                Rect lastRect = GUILayoutUtility.GetLastRect();
                if (flag)
                {
                    GUIContent previewTitle = editorThatControlsPreview.GetPreviewTitle();
                    if (previewTitle != null)
                    {
                        preTitle = previewTitle;
                    }
                    else
                    {
                        preTitle = styles.preTitle;
                    }
                }
                else
                {
                    preTitle = styles.labelTitle;
                }
                rect3.x = lastRect.x + 3f;
                rect3.y = (lastRect.y + ((17f - s_Styles.dragHandle.fixedHeight) / 2f)) + 1f;
                rect3.width = lastRect.width - 6f;
                rect3.height = s_Styles.dragHandle.fixedHeight;
                if (editorsWithPreviews.Length > 1)
                {
                    Vector2 vector = styles.preDropDown.CalcSize(preTitle);
                    Rect rect4 = new Rect(lastRect.x, lastRect.y, vector.x, vector.y);
                    lastRect.xMin += vector.x;
                    rect3.xMin += vector.x;
                    GUIContent[] contentArray = new GUIContent[editorsWithPreviews.Length];
                    int selected = -1;
                    for (int i = 0; i < editorsWithPreviews.Length; i++)
                    {
                        GUIContent content3;
                        string text;
                        IPreviewable previewable2 = editorsWithPreviews[i];
                        GUIContent content4 = previewable2.GetPreviewTitle();
                        if (content4 != null)
                        {
                            content3 = content4;
                        }
                        else
                        {
                            content3 = styles.preTitle;
                        }
                        if (content3 == styles.preTitle)
                        {
                            string typeName = ObjectNames.GetTypeName(previewable2.target);
                            if (previewable2.target is MonoBehaviour)
                            {
                                typeName = MonoScript.FromMonoBehaviour(previewable2.target as MonoBehaviour).GetClass().Name;
                            }
                            text = content3.text + " - " + typeName;
                        }
                        else
                        {
                            text = content3.text;
                        }
                        contentArray[i] = new GUIContent(text);
                        if (editorsWithPreviews[i] == editorThatControlsPreview)
                        {
                            selected = i;
                        }
                    }
                    if (GUI.Button(rect4, preTitle, styles.preDropDown))
                    {
                        EditorUtility.DisplayCustomMenu(rect4, contentArray, selected, new EditorUtility.SelectMenuItemFunction(this.OnPreviewSelected), editorsWithPreviews);
                    }
                }
                else
                {
                    float num3 = ((rect3.xMax - lastRect.xMin) - 3f) - 20f;
                    float width = Mathf.Min(num3, styles.preToolbar2.CalcSize(preTitle).x);
                    Rect rect5 = new Rect(lastRect.x, lastRect.y, width, lastRect.height);
                    rect3.xMin = rect5.xMax + 3f;
                    GUI.Label(rect5, preTitle, styles.preToolbar2);
                }
                if (flag && (Event.current.type == EventType.Repaint))
                {
                    s_Styles.dragHandle.Draw(rect3, GUIContent.none, false, false, false, false);
                }
                if (flag && this.m_PreviewResizer.GetExpandedBeforeDragging())
                {
                    editorThatControlsPreview.OnPreviewSettings();
                }
                EditorGUILayout.EndHorizontal();
                if (((current.type == EventType.MouseUp) && (current.button == 1)) && (position.Contains(current.mousePosition) && (this.m_PreviewWindow == null)))
                {
                    this.DetachPreview();
                }
                if (flag)
                {
                    Rect windowPosition = base.position;
                    if (((EditorSettings.externalVersionControl != ExternalVersionControl.Disabled) && (EditorSettings.externalVersionControl != ExternalVersionControl.AutoDetect)) && (EditorSettings.externalVersionControl != ExternalVersionControl.Generic))
                    {
                        windowPosition.height -= 17f;
                    }
                    num5 = this.m_PreviewResizer.ResizeHandle(windowPosition, 100f, 100f, 17f, lastRect);
                }
                else
                {
                    if (GUI.Button(position, GUIContent.none, GUIStyle.none))
                    {
                        this.m_PreviewResizer.ToggleExpanded();
                    }
                    num5 = 0f;
                }
                if (this.m_PreviewResizer.GetExpanded())
                {
                    GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.Height(num5) };
                    GUILayout.BeginVertical(styles.preBackground, optionArray2);
                    if (flag)
                    {
                        editorThatControlsPreview.DrawPreview(GUILayoutUtility.GetRect(0f, 10240f, (float) 64f, (float) 10240f));
                    }
                    if (flag2)
                    {
                        if (<>f__am$cache1 == null)
                        {
                            <>f__am$cache1 = a => EditorUtility.IsPersistent(a) && !Editor.IsAppropriateFileOpenForEdit(a);
                        }
                        using (new EditorGUI.DisabledScope(Enumerable.Any<UnityEngine.Object>(inspectedAssets, <>f__am$cache1)))
                        {
                            this.m_LabelGUI.OnLabelGUI(inspectedAssets);
                        }
                    }
                    if (flag3)
                    {
                        this.m_AssetBundleNameGUI.OnAssetBundleNameGUI(inspectedAssets);
                    }
                    GUILayout.EndVertical();
                }
            }
        }

        private void DrawSelectionPickerList()
        {
            if (this.m_TypeSelectionList == null)
            {
                this.m_TypeSelectionList = new TypeSelectionList(Selection.objects);
            }
            GUILayout.Space(0f);
            Editor.DrawHeaderGUI(null, Selection.objects.Length + " Objects");
            GUILayout.Label("Narrow the Selection:", EditorStyles.label, new GUILayoutOption[0]);
            GUILayout.Space(4f);
            Vector2 iconSize = EditorGUIUtility.GetIconSize();
            EditorGUIUtility.SetIconSize(new Vector2(16f, 16f));
            foreach (TypeSelection selection in this.m_TypeSelectionList.typeSelections)
            {
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(true) };
                Rect position = GUILayoutUtility.GetRect((float) 16f, (float) 16f, options);
                if (GUI.Button(position, selection.label, styles.typeSelection))
                {
                    Selection.objects = selection.objects;
                    Event.current.Use();
                }
                if (GUIUtility.hotControl == 0)
                {
                    EditorGUIUtility.AddCursorRect(position, MouseCursor.Link);
                }
                GUILayout.Space(4f);
            }
            EditorGUIUtility.SetIconSize(iconSize);
        }

        private void DrawSplitLine(float y)
        {
            Rect position = new Rect(0f, y, this.m_Pos.width + 1f, 1f);
            Rect texCoords = new Rect(0f, 1f, 1f, 1f - (1f / ((float) EditorStyles.inspectorTitlebar.normal.background.height)));
            GUI.DrawTextureWithTexCoords(position, EditorStyles.inspectorTitlebar.normal.background, texCoords);
        }

        private void DrawVCSShortInfo()
        {
            if (EditorSettings.externalVersionControl == ExternalVersionControl.AssetServer)
            {
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Height(17f) };
                EditorGUILayout.BeginHorizontal(GUIContent.none, styles.preToolbar, options);
                Editor firstNonImportInspectorEditor = this.GetFirstNonImportInspectorEditor(this.tracker.activeEditors);
                UnityEngine.Object target = firstNonImportInspectorEditor?.target;
                int controlID = GUIUtility.GetControlID(FocusType.Passive);
                GUILayout.FlexibleSpace();
                Rect lastRect = GUILayoutUtility.GetLastRect();
                EditorGUILayout.EndHorizontal();
                AssetInspector.Get().OnAssetStatusGUI(lastRect, controlID, target, styles.preToolbar2);
            }
            if ((Provider.isActive && (EditorSettings.externalVersionControl != ExternalVersionControl.Disabled)) && ((EditorSettings.externalVersionControl != ExternalVersionControl.AutoDetect) && (EditorSettings.externalVersionControl != ExternalVersionControl.Generic)))
            {
                Editor editor2 = this.GetFirstNonImportInspectorEditor(this.tracker.activeEditors);
                string assetPath = AssetDatabase.GetAssetPath(editor2.target);
                Asset assetByPath = Provider.GetAssetByPath(assetPath);
                if ((assetByPath != null) && (assetByPath.path.StartsWith("Assets") || assetByPath.path.StartsWith("ProjectSettings")))
                {
                    char[] trimChars = new char[] { '/' };
                    Asset asset = Provider.GetAssetByPath(assetPath.Trim(trimChars) + ".meta");
                    string currentState = assetByPath.StateToString();
                    string str3 = (asset != null) ? asset.StateToString() : string.Empty;
                    bool flag = (asset != null) && ((asset.state & ~Asset.States.MetaFile) != assetByPath.state);
                    bool flag2 = currentState != "";
                    float height = (!flag || !flag2) ? 17f : 34f;
                    GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.Height(height) };
                    GUILayout.Label(GUIContent.none, styles.preToolbar, optionArray2);
                    Rect rect = GUILayoutUtility.GetLastRect();
                    bool flag3 = (Event.current.type == EventType.Layout) || (Event.current.type == EventType.Repaint);
                    if (flag2 && flag3)
                    {
                        Texture2D cachedIcon = AssetDatabase.GetCachedIcon(assetPath) as Texture2D;
                        if (flag)
                        {
                            Rect rect3 = rect;
                            rect3.height = 17f;
                            this.DrawVCSShortInfoAsset(assetByPath, this.BuildTooltip(assetByPath, null), rect3, cachedIcon, currentState);
                            Texture2D iconForFile = InternalEditorUtility.GetIconForFile(asset.path);
                            rect3.y += 17f;
                            this.DrawVCSShortInfoAsset(asset, this.BuildTooltip(null, asset), rect3, iconForFile, str3);
                        }
                        else
                        {
                            this.DrawVCSShortInfoAsset(assetByPath, this.BuildTooltip(assetByPath, asset), rect, cachedIcon, currentState);
                        }
                    }
                    else if ((str3 != "") && flag3)
                    {
                        Texture2D icon = InternalEditorUtility.GetIconForFile(asset.path);
                        this.DrawVCSShortInfoAsset(asset, this.BuildTooltip(assetByPath, asset), rect, icon, str3);
                    }
                    string message = "";
                    if (!Editor.IsAppropriateFileOpenForEdit(editor2.target, out message))
                    {
                        float width = 80f;
                        Rect position = new Rect((rect.x + rect.width) - width, rect.y, width, rect.height);
                        if (GUI.Button(position, "Check out", styles.lockedHeaderButton))
                        {
                            EditorPrefs.SetBool("vcssticky", true);
                            Provider.Checkout(editor2.targets, CheckoutMode.Both).Wait();
                            base.Repaint();
                        }
                        this.DrawVCSSticky(rect.height / 2f);
                    }
                }
            }
        }

        protected void DrawVCSShortInfoAsset(Asset asset, string tooltip, Rect rect, Texture2D icon, string currentState)
        {
            Rect itemRect = new Rect(rect.x, rect.y, 28f, 16f);
            Rect position = itemRect;
            position.x += 6f;
            position.width = 16f;
            if (icon != null)
            {
                GUI.DrawTexture(position, icon);
            }
            Overlay.DrawOverlay(asset, itemRect);
            Rect rect4 = new Rect(rect.x + 26f, rect.y, rect.width - 31f, rect.height);
            GUIContent label = GUIContent.Temp(currentState);
            label.tooltip = tooltip;
            EditorGUI.LabelField(rect4, label, styles.preToolbar2);
        }

        protected virtual void DrawVCSSticky(float offset)
        {
            string message = "";
            Editor firstNonImportInspectorEditor = this.GetFirstNonImportInspectorEditor(this.tracker.activeEditors);
            if (!EditorPrefs.GetBool("vcssticky") && !Editor.IsAppropriateFileOpenForEdit(firstNonImportInspectorEditor.target, out message))
            {
                Rect position = new Rect(10f, base.position.height - 94f, base.position.width - 20f, 80f);
                position.y -= offset;
                if (Event.current.type == EventType.Repaint)
                {
                    styles.stickyNote.Draw(position, false, false, false, false);
                    Rect rect4 = new Rect(position.x, (position.y + (position.height / 2f)) - 32f, 64f, 64f);
                    if (EditorSettings.externalVersionControl == "Perforce")
                    {
                        styles.stickyNotePerforce.Draw(rect4, false, false, false, false);
                    }
                    Rect rect5 = new Rect(position.x + rect4.width, position.y, position.width - rect4.width, position.height);
                    GUI.Label(rect5, new GUIContent("<b>Under Version Control</b>\nCheck out this asset in order to make changes."), styles.stickyNoteLabel);
                    Rect rect6 = new Rect(position.x + (position.width / 2f), position.y + 80f, 19f, 14f);
                    styles.stickyNoteArrow.Draw(rect6, false, false, false, false);
                }
            }
        }

        private bool EditorHasLargeHeader(int editorIndex)
        {
            UnityEngine.Object target = this.m_Tracker.activeEditors[editorIndex].target;
            return (((AssetDatabase.IsMainAsset(target) || AssetDatabase.IsSubAsset(target)) || (editorIndex == 0)) || (target is Material));
        }

        private void FlipLocked()
        {
            this.isLocked = !this.isLocked;
        }

        private static void FlushAllOptimizedGUIBlocksIfNeeded()
        {
            if (s_AllOptimizedGUIBlocksNeedsRebuild)
            {
                s_AllOptimizedGUIBlocksNeedsRebuild = false;
                foreach (InspectorWindow window in m_AllInspectors)
                {
                    foreach (Editor editor in window.tracker.activeEditors)
                    {
                        FlushOptimizedGUIBlock(editor);
                    }
                }
            }
        }

        private static void FlushOptimizedGUI()
        {
            s_AllOptimizedGUIBlocksNeedsRebuild = true;
        }

        private static void FlushOptimizedGUIBlock(Editor editor)
        {
            OptimizedGUIBlock block;
            float num;
            if ((editor != null) && editor.GetOptimizedGUIBlock(false, false, out block, out num))
            {
                block.valid = false;
            }
        }

        public static InspectorWindow[] GetAllInspectorWindows() => 
            m_AllInspectors.ToArray();

        public IPreviewable[] GetEditorsWithPreviews(Editor[] editors)
        {
            IList<IPreviewable> source = new List<IPreviewable>();
            int editorIndex = -1;
            foreach (Editor editor in editors)
            {
                editorIndex++;
                if ((((editor.target != null) && (!EditorUtility.IsPersistent(editor.target) || (AssetDatabase.GetAssetPath(editor.target) == AssetDatabase.GetAssetPath(editors[0].target)))) && ((EditorUtility.IsPersistent(editors[0].target) || !EditorUtility.IsPersistent(editor.target)) && !this.ShouldCullEditor(editors, editorIndex))) && ((!(editors[0] is AssetImporterInspector) || (editor is AssetImporterInspector)) && editor.HasPreviewGUI()))
                {
                    source.Add(editor);
                }
            }
            foreach (IPreviewable previewable in this.m_Previews)
            {
                if (previewable.HasPreviewGUI())
                {
                    source.Add(previewable);
                }
            }
            return source.ToArray<IPreviewable>();
        }

        public IPreviewable GetEditorThatControlsPreview(IPreviewable[] editors)
        {
            if (editors.Length != 0)
            {
                if (this.m_SelectedPreview != null)
                {
                    return this.m_SelectedPreview;
                }
                IPreviewable lastInteractedEditor = this.GetLastInteractedEditor();
                System.Type type = lastInteractedEditor?.GetType();
                IPreviewable previewable3 = null;
                IPreviewable previewable4 = null;
                foreach (IPreviewable previewable5 in editors)
                {
                    if ((((previewable5 != null) && (previewable5.target != null)) && ((!EditorUtility.IsPersistent(previewable5.target) || (AssetDatabase.GetAssetPath(previewable5.target) == AssetDatabase.GetAssetPath(editors[0].target))) && (!(editors[0] is AssetImporterInspector) || (previewable5 is AssetImporterInspector)))) && previewable5.HasPreviewGUI())
                    {
                        if (previewable5 == lastInteractedEditor)
                        {
                            return previewable5;
                        }
                        if ((previewable4 == null) && (previewable5.GetType() == type))
                        {
                            previewable4 = previewable5;
                        }
                        if (previewable3 == null)
                        {
                            previewable3 = previewable5;
                        }
                    }
                }
                if (previewable4 != null)
                {
                    return previewable4;
                }
                if (previewable3 != null)
                {
                    return previewable3;
                }
            }
            return null;
        }

        private Editor GetFirstNonImportInspectorEditor(Editor[] editors)
        {
            foreach (Editor editor in editors)
            {
                if (!(editor.target is AssetImporter))
                {
                    return editor;
                }
            }
            return null;
        }

        private UnityEngine.Object[] GetInspectedAssets()
        {
            Editor firstNonImportInspectorEditor = this.GetFirstNonImportInspectorEditor(this.tracker.activeEditors);
            if ((firstNonImportInspectorEditor != null) && (firstNonImportInspectorEditor.targets.Length == 1))
            {
                string assetPath = AssetDatabase.GetAssetPath(firstNonImportInspectorEditor.target);
                if (assetPath.ToLower().StartsWith("assets") && !Directory.Exists(assetPath))
                {
                    return firstNonImportInspectorEditor.targets;
                }
            }
            return Selection.GetFiltered(typeof(UnityEngine.Object), UnityEditor.SelectionMode.Assets);
        }

        public UnityEngine.Object GetInspectedObject()
        {
            Editor firstNonImportInspectorEditor = this.GetFirstNonImportInspectorEditor(this.tracker.activeEditors);
            return firstNonImportInspectorEditor?.target;
        }

        internal static List<InspectorWindow> GetInspectors() => 
            m_AllInspectors;

        public virtual Editor GetLastInteractedEditor() => 
            this.m_LastInteractedEditor;

        private IEnumerable<IPreviewable> GetPreviewsForType(Editor editor)
        {
            List<IPreviewable> list = new List<IPreviewable>();
            foreach (Assembly assembly in EditorAssemblies.loadedAssemblies)
            {
                System.Type[] typesFromAssembly = AssemblyHelper.GetTypesFromAssembly(assembly);
                foreach (System.Type type in typesFromAssembly)
                {
                    if (typeof(IPreviewable).IsAssignableFrom(type) && !typeof(Editor).IsAssignableFrom(type))
                    {
                        object[] customAttributes = type.GetCustomAttributes(typeof(CustomPreviewAttribute), false);
                        foreach (CustomPreviewAttribute attribute in customAttributes)
                        {
                            if (attribute.m_Type == editor.target.GetType())
                            {
                                IPreviewable item = Activator.CreateInstance(type) as IPreviewable;
                                item.Initialize(editor.targets);
                                list.Add(item);
                            }
                        }
                    }
                }
            }
            return list;
        }

        protected UnityEngine.Object[] GetTargetsForPreview(IPreviewable previewEditor)
        {
            Editor editor = null;
            foreach (Editor editor2 in this.tracker.activeEditors)
            {
                if (editor2.target.GetType() == previewEditor.target.GetType())
                {
                    editor = editor2;
                    break;
                }
            }
            return editor.targets;
        }

        private void HandleComponentScreenshot(Rect contentRect, Editor editor)
        {
            if (ScreenShots.s_TakeComponentScreenshot)
            {
                contentRect.yMin -= 16f;
                if (contentRect.Contains(Event.current.mousePosition))
                {
                    Rect rect = GUIClip.Unclip(contentRect);
                    rect.position += base.m_Parent.screenPosition.position;
                    ScreenShots.ScreenShotComponent(rect, editor.target);
                }
            }
        }

        private void HandleEditorDragging(Rect bottomRect)
        {
            this.HandleEditorDragging(this.m_EditorDraggingLastIndex, bottomRect, this.m_EditorDraggingLastMarkerY, true);
        }

        private void HandleEditorDragging(int editorIndex, Rect dragRect, Rect contentRect)
        {
            if (dragRect.height != 0f)
            {
                if (contentRect.height == 0f)
                {
                    contentRect = dragRect;
                }
                float num = 8f;
                Rect targetRect = new Rect(contentRect.x, contentRect.yMax - (num - 2f), contentRect.width, (num * 2f) + 1f);
                float yMax = contentRect.yMax;
                this.m_EditorDraggingLastIndex = editorIndex;
                this.m_EditorDraggingLastMarkerY = yMax;
                this.HandleEditorDragging(editorIndex, targetRect, yMax, false);
            }
        }

        private void HandleEditorDragging(int editorIndex, Rect targetRect, float markerY, bool bottomTarget)
        {
            Event current = Event.current;
            EventType type = current.type;
            switch (type)
            {
                case EventType.Repaint:
                    if ((this.m_EditorDraggingTargetIndex != -1) && targetRect.Contains(current.mousePosition))
                    {
                        Rect position = new Rect(targetRect.x, markerY, targetRect.width, 3f);
                        if (!this.m_EditorDraggingTargetAbove)
                        {
                            position.y += 2f;
                        }
                        styles.insertionMarker.Draw(position, false, false, false, false);
                    }
                    return;

                case EventType.DragUpdated:
                {
                    if (!targetRect.Contains(current.mousePosition))
                    {
                        this.m_EditorDraggingTargetIndex = -1;
                        return;
                    }
                    bool? genericData = DragAndDrop.GetGenericData("InspectorEditorDraggingApplicable") as bool?;
                    if (!genericData.HasValue)
                    {
                        UnityEngine.Object[] objectReferences = DragAndDrop.objectReferences;
                        if (objectReferences.Length > 0)
                        {
                        }
                        genericData = new bool?((<>f__am$cache3 == null) && Enumerable.All<UnityEngine.Object>(objectReferences, <>f__am$cache3));
                        DragAndDrop.SetGenericData("InspectorEditorDraggingApplicable", genericData);
                    }
                    if (genericData.Value)
                    {
                        Editor[] activeEditors = this.m_Tracker.activeEditors;
                        UnityEngine.Object[] objArray2 = DragAndDrop.objectReferences;
                        if (bottomTarget)
                        {
                            this.m_EditorDraggingTargetAbove = false;
                            this.m_EditorDraggingTargetIndex = this.m_EditorDraggingLastIndex;
                        }
                        else
                        {
                            this.m_EditorDraggingTargetAbove = current.mousePosition.y < (targetRect.y + (targetRect.height / 2f));
                            this.m_EditorDraggingTargetIndex = editorIndex;
                            if (this.m_EditorDraggingTargetAbove)
                            {
                                this.m_EditorDraggingTargetIndex++;
                                while ((this.m_EditorDraggingTargetIndex < activeEditors.Length) && this.ShouldCullEditor(activeEditors, this.m_EditorDraggingTargetIndex))
                                {
                                    this.m_EditorDraggingTargetIndex++;
                                }
                                if (this.m_EditorDraggingTargetIndex == activeEditors.Length)
                                {
                                    this.m_EditorDraggingTargetIndex = -1;
                                    return;
                                }
                            }
                        }
                        if (this.m_EditorDraggingTargetAbove && this.EditorHasLargeHeader(this.m_EditorDraggingTargetIndex))
                        {
                            this.m_EditorDraggingTargetIndex--;
                            while ((this.m_EditorDraggingTargetIndex >= 0) && this.ShouldCullEditor(activeEditors, this.m_EditorDraggingTargetIndex))
                            {
                                this.m_EditorDraggingTargetIndex--;
                            }
                            if (this.m_EditorDraggingTargetIndex == -1)
                            {
                                return;
                            }
                            this.m_EditorDraggingTargetAbove = false;
                        }
                        if (<>f__am$cache4 == null)
                        {
                            <>f__am$cache4 = o => o as Component;
                        }
                        Component[] sourceComponents = Array.ConvertAll<UnityEngine.Object, Component>(DragAndDrop.objectReferences, <>f__am$cache4).ToArray<Component>();
                        if (<>f__am$cache5 == null)
                        {
                            <>f__am$cache5 = o => o as Component;
                        }
                        Component[] targetComponents = Array.ConvertAll<UnityEngine.Object, Component>(activeEditors[this.m_EditorDraggingTargetIndex].targets, <>f__am$cache5).ToArray<Component>();
                        if (this.MoveOrCopyComponents(sourceComponents, targetComponents, EditorUtility.EventHasDragCopyModifierPressed(current), true))
                        {
                            DragAndDrop.visualMode = !EditorUtility.EventHasDragCopyModifierPressed(current) ? DragAndDropVisualMode.Move : DragAndDropVisualMode.Copy;
                        }
                        else
                        {
                            DragAndDrop.visualMode = DragAndDropVisualMode.None;
                            this.m_EditorDraggingTargetIndex = -1;
                        }
                        current.Use();
                    }
                    return;
                }
                case EventType.DragPerform:
                    if (this.m_EditorDraggingTargetIndex != -1)
                    {
                        Editor[] editorArray2 = this.m_Tracker.activeEditors;
                        if (<>f__am$cache6 == null)
                        {
                            <>f__am$cache6 = o => o as Component;
                        }
                        Component[] componentArray3 = Array.ConvertAll<UnityEngine.Object, Component>(DragAndDrop.objectReferences, <>f__am$cache6).ToArray<Component>();
                        if (<>f__am$cache7 == null)
                        {
                            <>f__am$cache7 = o => o as Component;
                        }
                        Component[] componentArray4 = Array.ConvertAll<UnityEngine.Object, Component>(editorArray2[this.m_EditorDraggingTargetIndex].targets, <>f__am$cache7).ToArray<Component>();
                        if ((componentArray3.Length != 0) && (componentArray4.Length != 0))
                        {
                            this.MoveOrCopyComponents(componentArray3, componentArray4, EditorUtility.EventHasDragCopyModifierPressed(current), false);
                            this.m_EditorDraggingTargetIndex = -1;
                            DragAndDrop.AcceptDrag();
                            current.Use();
                            GUIUtility.ExitGUI();
                        }
                    }
                    return;
            }
            if (type == EventType.DragExited)
            {
                this.m_EditorDraggingTargetIndex = -1;
            }
        }

        private void HandleLastInteractedEditor(Rect componentRect, Editor editor)
        {
            if (((editor != this.m_LastInteractedEditor) && (Event.current.type == EventType.MouseDown)) && componentRect.Contains(Event.current.mousePosition))
            {
                this.m_LastInteractedEditor = editor;
                base.Repaint();
            }
        }

        private void MoveFocusOnKeyPress()
        {
            KeyCode keyCode = Event.current.keyCode;
            if ((Event.current.type == EventType.KeyDown) && (((keyCode == KeyCode.DownArrow) || (keyCode == KeyCode.UpArrow)) || (keyCode == KeyCode.Tab)))
            {
                if (keyCode != KeyCode.Tab)
                {
                    EditorGUIUtility.MoveFocusAndScroll(keyCode == KeyCode.DownArrow);
                }
                else
                {
                    EditorGUIUtility.ScrollForTabbing(!Event.current.shift);
                }
                Event.current.Use();
            }
        }

        private bool MoveOrCopyComponents(Component[] sourceComponents, Component[] targetComponents, bool copy, bool validateOnly)
        {
            if (copy)
            {
                return false;
            }
            if ((sourceComponents.Length == 1) && (targetComponents.Length == 1))
            {
                if (sourceComponents[0].gameObject != targetComponents[0].gameObject)
                {
                    return false;
                }
                return ComponentUtility.MoveComponentRelativeToComponent(sourceComponents[0], targetComponents[0], this.m_EditorDraggingTargetAbove, validateOnly);
            }
            return ComponentUtility.MoveComponentsRelativeToComponents(sourceComponents, targetComponents, this.m_EditorDraggingTargetAbove, validateOnly);
        }

        private void OnDestroy()
        {
            if (this.m_PreviewWindow != null)
            {
                this.m_PreviewWindow.Close();
            }
            if ((this.m_Tracker != null) && !this.m_Tracker.Equals(ActiveEditorTracker.sharedTracker))
            {
                this.m_Tracker.Destroy();
            }
        }

        protected virtual void OnDisable()
        {
            m_AllInspectors.Remove(this);
        }

        protected virtual void OnEnable()
        {
            this.RefreshTitle();
            base.minSize = new Vector2(275f, 50f);
            if (!m_AllInspectors.Contains(this))
            {
                m_AllInspectors.Add(this);
            }
            this.m_PreviewResizer.Init("InspectorPreview");
            this.m_LabelGUI.OnEnable();
            this.CreateTracker();
        }

        protected virtual void OnGUI()
        {
            Profiler.BeginSample("InspectorWindow.OnGUI");
            this.CreatePreviewables();
            FlushAllOptimizedGUIBlocksIfNeeded();
            this.ResetKeyboardControl();
            this.m_ScrollPosition = EditorGUILayout.BeginVerticalScrollView(this.m_ScrollPosition, new GUILayoutOption[0]);
            if (Event.current.type == EventType.Repaint)
            {
                this.tracker.ClearDirty();
            }
            s_CurrentInspectorWindow = this;
            Editor[] activeEditors = this.tracker.activeEditors;
            this.AssignAssetEditor(activeEditors);
            Profiler.BeginSample("InspectorWindow.DrawEditors()");
            this.DrawEditors(activeEditors);
            Profiler.EndSample();
            if (this.tracker.hasComponentsWhichCannotBeMultiEdited)
            {
                if (((activeEditors.Length == 0) && !this.tracker.isLocked) && (Selection.objects.Length > 0))
                {
                    this.DrawSelectionPickerList();
                }
                else
                {
                    Rect rect = GUILayoutUtility.GetRect(10f, 4f, EditorStyles.inspectorTitlebar);
                    if (Event.current.type == EventType.Repaint)
                    {
                        this.DrawSplitLine(rect.y);
                    }
                    GUILayout.Label("Components that are only on some of the selected objects cannot be multi-edited.", EditorStyles.helpBox, new GUILayoutOption[0]);
                    GUILayout.Space(4f);
                }
            }
            s_CurrentInspectorWindow = null;
            EditorGUI.indentLevel = 0;
            this.AddComponentButton(this.tracker.activeEditors);
            GUI.enabled = true;
            this.CheckDragAndDrop(this.tracker.activeEditors);
            this.MoveFocusOnKeyPress();
            EditorGUILayout.EndScrollView();
            Profiler.BeginSample("InspectorWindow.DrawPreviewAndLabels");
            this.DrawPreviewAndLabels();
            Profiler.EndSample();
            if (this.tracker.activeEditors.Length > 0)
            {
                this.DrawVCSShortInfo();
            }
            Profiler.EndSample();
        }

        private void OnInspectorUpdate()
        {
            this.tracker.VerifyModifiedMonoBehaviours();
            if (this.tracker.isDirty && this.ReadyToRepaint())
            {
                base.Repaint();
            }
        }

        private void OnLostFocus()
        {
            EditorGUI.EndEditingActiveTextField();
            this.m_LabelGUI.OnLostFocus();
        }

        private void OnPreviewSelected(object userData, string[] options, int selected)
        {
            IPreviewable[] previewableArray = userData as IPreviewable[];
            this.m_SelectedPreview = previewableArray[selected];
        }

        internal override void OnResized()
        {
            this.m_InvalidateGUIBlockCache = true;
        }

        private void OnSelectionChange()
        {
            this.m_Previews = null;
            this.m_SelectedPreview = null;
            this.m_TypeSelectionList = null;
            base.m_Parent.ClearKeyboardControl();
            ScriptAttributeUtility.ClearGlobalCache();
            base.Repaint();
        }

        private bool ReadyToRepaint()
        {
            if (UnityEditor.AnimationMode.InAnimationPlaybackMode())
            {
                long num = DateTime.Now.Ticks / 0x2710L;
                if ((num - this.s_LastUpdateWhilePlayingAnimation) < 150L)
                {
                    return false;
                }
                this.s_LastUpdateWhilePlayingAnimation = num;
            }
            return true;
        }

        private void RefreshTitle()
        {
            string icon = "UnityEditor.InspectorWindow";
            if (this.m_InspectorMode == InspectorMode.Normal)
            {
                base.titleContent = EditorGUIUtility.TextContentWithIcon("Inspector", icon);
            }
            else
            {
                base.titleContent = EditorGUIUtility.TextContentWithIcon("Debug", icon);
            }
        }

        internal static void RepaintAllInspectors()
        {
            foreach (InspectorWindow window in m_AllInspectors)
            {
                window.Repaint();
            }
        }

        private void ResetKeyboardControl()
        {
            if (this.m_ResetKeyboardControl)
            {
                GUIUtility.keyboardControl = 0;
                this.m_ResetKeyboardControl = false;
            }
        }

        private void SetDebug()
        {
            this.SetMode(InspectorMode.Debug);
        }

        private void SetDebugInternal()
        {
            this.SetMode(InspectorMode.DebugInternal);
        }

        private void SetMode(InspectorMode mode)
        {
            this.m_InspectorMode = mode;
            this.RefreshTitle();
            this.tracker.inspectorMode = mode;
            this.m_ResetKeyboardControl = true;
        }

        private void SetNormal()
        {
            this.SetMode(InspectorMode.Normal);
        }

        private bool ShouldCullEditor(Editor[] editors, int editorIndex)
        {
            if (editors[editorIndex].hideInspector)
            {
                return true;
            }
            UnityEngine.Object target = editors[editorIndex].target;
            if ((target is SubstanceImporter) || (target is ParticleSystemRenderer))
            {
                return true;
            }
            if (target.GetType() == typeof(AssetImporter))
            {
                return true;
            }
            if ((this.m_InspectorMode == InspectorMode.Normal) && (editorIndex != 0))
            {
                AssetImporterInspector inspector = editors[0] as AssetImporterInspector;
                if ((inspector != null) && !inspector.showImportedObject)
                {
                    return true;
                }
            }
            return false;
        }

        protected virtual void ShowButton(Rect r)
        {
            bool flag = GUI.Toggle(r, this.isLocked, GUIContent.none, styles.lockButton);
            if (flag != this.isLocked)
            {
                this.isLocked = flag;
                this.tracker.RebuildIfNecessary();
            }
        }

        internal static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(InspectorWindow));
        }

        private void Update()
        {
            Editor[] activeEditors = this.tracker.activeEditors;
            if (activeEditors != null)
            {
                bool flag = false;
                foreach (Editor editor in activeEditors)
                {
                    if (editor.RequiresConstantRepaint() && !editor.hideInspector)
                    {
                        flag = true;
                    }
                }
                if (flag && ((this.m_lastRenderedTime + 0.032999999821186066) < EditorApplication.timeSinceStartup))
                {
                    this.m_lastRenderedTime = EditorApplication.timeSinceStartup;
                    base.Repaint();
                }
            }
        }

        public bool isLocked
        {
            get => 
                this.tracker.isLocked;
            set
            {
                this.tracker.isLocked = value;
            }
        }

        internal static Styles styles
        {
            get
            {
                Styles styles;
                if (s_Styles != null)
                {
                    styles = s_Styles;
                }
                else
                {
                    styles = s_Styles = new Styles();
                }
                return styles;
            }
        }

        public ActiveEditorTracker tracker
        {
            get
            {
                this.CreateTracker();
                return this.m_Tracker;
            }
        }

        [CompilerGenerated]
        private sealed class <CreateTracker>c__AnonStorey0
        {
            internal ActiveEditorTracker sharedTracker;

            internal bool <>m__0(InspectorWindow i) => 
                ((i.m_Tracker != null) && i.m_Tracker.Equals(this.sharedTracker));
        }

        internal class Styles
        {
            public GUIStyle addComponentArea = EditorStyles.inspectorTitlebar;
            public GUIStyle addComponentButtonStyle = "AC Button";
            public readonly GUIContent addComponentLabel = EditorGUIUtility.TextContent("Add Component");
            public readonly GUIStyle dragHandle = "RL DragHandle";
            public readonly GUIStyle insertionMarker = "InsertionMarker";
            public readonly GUIContent labelTitle = EditorGUIUtility.TextContent("Asset Labels");
            public readonly GUIStyle lockButton = "IN LockButton";
            public GUIStyle lockedHeaderButton = "preButton";
            public GUIStyle preBackground = "preBackground";
            public readonly GUIStyle preDropDown = "preDropDown";
            public readonly GUIContent preTitle = EditorGUIUtility.TextContent("Preview");
            public readonly GUIStyle preToolbar = "preToolbar";
            public readonly GUIStyle preToolbar2 = "preToolbar2";
            public GUIStyle previewMiniLabel = new GUIStyle(EditorStyles.whiteMiniLabel);
            public GUIStyle stickyNote = new GUIStyle("VCS_StickyNote");
            public GUIStyle stickyNoteArrow = new GUIStyle("VCS_StickyNoteArrow");
            public GUIStyle stickyNoteLabel = new GUIStyle("VCS_StickyNoteLabel");
            public GUIStyle stickyNotePerforce = new GUIStyle("VCS_StickyNoteP4");
            public GUIStyle typeSelection = new GUIStyle("PR Label");

            public Styles()
            {
                this.typeSelection.padding.left = 12;
            }
        }
    }
}

