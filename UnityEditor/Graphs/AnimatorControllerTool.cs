namespace UnityEditor.Graphs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditor.Animations;
    using UnityEditor.Graphs.AnimationBlendTree;
    using UnityEditor.Graphs.AnimationStateMachine;
    using UnityEngine;

    [EditorWindowTitle(title="Animator", icon="UnityEditor.Graphs.AnimatorControllerTool")]
    internal class AnimatorControllerTool : EditorWindow, IAnimatorControllerEditor
    {
        [SerializeField]
        public UnityEditor.Graphs.AnimationBlendTree.Graph blendTreeGraph;
        [SerializeField]
        public UnityEditor.Graphs.AnimationBlendTree.GraphGUI blendTreeGraphGUI;
        public static System.Action graphDirtyCallback;
        private const float kBottombarHeight = 18f;
        [SerializeField]
        private AnimatorController m_AnimatorController;
        [SerializeField]
        private bool m_AutoLiveLink = true;
        [SerializeField]
        private List<BreadCrumbElement> m_BreadCrumbs;
        [SerializeField]
        private int m_CurrentEditor = 0;
        [SerializeField]
        private LayerControllerView m_LayerEditor = null;
        [SerializeField]
        private bool m_MiniTool = false;
        private ParameterControllerView m_ParameterEditor = null;
        [SerializeField]
        private Animator m_PreviewAnimator;
        [SerializeField]
        private bool m_SerializedLocked = false;
        protected SplitterState m_VerticalSplitter;
        [SerializeField]
        private AnimatorViewPositionCache m_ViewPositions = new AnimatorViewPositionCache();
        private static Styles s_Styles;
        private const int sLayerTab = 0;
        private const int sParameterTab = 1;
        [SerializeField]
        public UnityEditor.Graphs.AnimationStateMachine.Graph stateMachineGraph;
        [SerializeField]
        public UnityEditor.Graphs.AnimationStateMachine.GraphGUI stateMachineGraphGUI;
        public static AnimatorControllerTool tool;

        public void AddBreadCrumb(UnityEngine.Object target, bool updateViewPosition)
        {
            if (updateViewPosition)
            {
                this.StoreCurrentViewPosition();
            }
            this.m_BreadCrumbs.Add(new BreadCrumbElement(target));
            if (updateViewPosition)
            {
                this.CenterOnStoredPosition(target);
            }
        }

        public virtual void AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Lock"), this.isLocked, new GenericMenu.MenuFunction(this.FlipLocked));
        }

        public void AddLayer(string layerName)
        {
            Undo.RegisterCompleteObjectUndo(this.animatorController, "Layer Added");
            this.animatorController.AddLayer(layerName);
            this.selectedLayerIndex = this.animatorController.layers.Length - 1;
        }

        public void AddNewLayer()
        {
            this.AddLayer("New Layer");
        }

        public void BlendParameterInputChanged(BlendTree blendTree)
        {
            base.Repaint();
        }

        public void BlendTreeHierarchyChanged(BlendTree blendTree)
        {
            this.blendTreeGraph.BuildFromBlendTree(this.blendTreeGraph.rootBlendTree);
            base.Repaint();
        }

        private void BlendTreeView(Rect position)
        {
            Motion stateEffectiveMotion;
            this.blendTreeGraph.previewAvatar = this.m_PreviewAnimator;
            if (this.m_BreadCrumbs.Last<BreadCrumbElement>().target is AnimatorState)
            {
                stateEffectiveMotion = tool.animatorController.GetStateEffectiveMotion(this.m_BreadCrumbs.Last<BreadCrumbElement>().target as AnimatorState, tool.selectedLayerIndex);
            }
            else
            {
                stateEffectiveMotion = this.m_BreadCrumbs.Last<BreadCrumbElement>().target as Motion;
            }
            this.blendTreeGraph.rootBlendTree = stateEffectiveMotion as BlendTree;
            this.blendTreeGraphGUI.BeginGraphGUI(this, position);
            this.blendTreeGraphGUI.OnGraphGUI();
            this.blendTreeGraphGUI.EndGraphGUI();
        }

        public void BuildBreadCrumbsFromSMHierarchy(IEnumerable<AnimatorStateMachine> hierarchy)
        {
            this.m_BreadCrumbs.Clear();
            foreach (AnimatorStateMachine machine in hierarchy)
            {
                this.AddBreadCrumb(machine, false);
            }
            this.CenterView();
        }

        private void CenterOnStoredPosition(UnityEngine.Object target)
        {
            Vector2 viewPosition = this.m_ViewPositions.GetViewPosition(target);
            if (this.stateMachineGraphGUI != null)
            {
                this.stateMachineGraphGUI.CenterGraph(viewPosition);
            }
            if (this.blendTreeGraphGUI != null)
            {
                this.blendTreeGraphGUI.CenterGraph(viewPosition);
            }
        }

        public void CenterView()
        {
            if (this.m_BreadCrumbs.Count > 0)
            {
                this.CenterOnStoredPosition(this.m_BreadCrumbs.Last<BreadCrumbElement>().target);
            }
            else
            {
                this.CenterOnStoredPosition(null);
            }
        }

        private void DetectAnimatorControllerFromSelection()
        {
            AnimatorController activeObject = null;
            if ((Selection.activeObject == null) && (this.animatorController == null))
            {
                this.animatorController = null;
            }
            if ((Selection.activeObject is AnimatorController) && EditorUtility.IsPersistent(Selection.activeObject))
            {
                activeObject = Selection.activeObject as AnimatorController;
            }
            if (Selection.activeGameObject != null)
            {
                Animator component = Selection.activeGameObject.GetComponent<Animator>();
                if ((component != null) && (AnimatorController.FindAnimatorControllerPlayable(component, this.animatorController) == null))
                {
                    AnimatorController effectiveAnimatorController = AnimatorController.GetEffectiveAnimatorController(component);
                    if (effectiveAnimatorController != null)
                    {
                        activeObject = effectiveAnimatorController;
                    }
                }
            }
            if (((activeObject != null) && (activeObject != this.animatorController)) && !this.IsPreviewController(activeObject))
            {
                this.animatorController = activeObject;
                if (this.animatorController == null)
                {
                }
            }
        }

        private void DetectPreviewObjectFromSelection()
        {
            if (Selection.activeGameObject != null)
            {
                Animator component = Selection.activeGameObject.GetComponent<Animator>();
                if (((component != null) && ((AnimatorController.GetEffectiveAnimatorController(component) != null) || (component.runtimeAnimatorController != null))) && !AssetDatabase.Contains(Selection.activeGameObject))
                {
                    this.m_PreviewAnimator = component;
                }
            }
        }

        private void DoGraphBottomBar(Rect nameRect)
        {
            GUILayout.BeginArea(nameRect);
            GUILayout.BeginHorizontal(s_Styles.bottomBarDarkBg, new GUILayoutOption[0]);
            if (this.liveLink && (this.previewAnimator != null))
            {
                GUILayout.Label(this.previewAnimator.name, s_Styles.liveLinkLabel, new GUILayoutOption[0]);
            }
            if (this.animatorController != null)
            {
                string str = "Assets/";
                string assetPath = AssetDatabase.GetAssetPath(this.animatorController);
                if (assetPath.StartsWith(str))
                {
                    assetPath = assetPath.Remove(0, str.Length);
                }
                GUILayout.Label(assetPath, s_Styles.nameLabel, new GUILayoutOption[0]);
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private void DoGraphToolbar(Rect toolbarRect)
        {
            GUILayout.BeginArea(toolbarRect);
            GUILayout.BeginHorizontal(EditorStyles.toolbar, new GUILayoutOption[0]);
            if (this.miniTool)
            {
                if (GUILayout.Button(s_Styles.visibleOFF, s_Styles.invisibleButton, new GUILayoutOption[0]))
                {
                    this.miniTool = false;
                }
                GUILayout.Space(10f);
            }
            BreadCrumbElement[] elementArray = this.m_BreadCrumbs.ToArray();
            for (int i = 0; i < elementArray.Length; i++)
            {
                BreadCrumbElement element = elementArray[i];
                if (this.miniTool && (i == 0))
                {
                    if (EditorGUILayout.ButtonMouseDown(new GUIContent(element.name), FocusType.Keyboard, EditorStyles.toolbarPopup, new GUILayoutOption[0]))
                    {
                        AnimatorControllerLayer[] layers = this.animatorController.layers;
                        GenericMenu menu = new GenericMenu();
                        for (int j = 0; j < layers.Length; j++)
                        {
                            menu.AddItem(new GUIContent(layers[j].name), false, new GenericMenu.MenuFunction2(this.SetCurrentLayer), j);
                        }
                        menu.AddSeparator(string.Empty);
                        menu.AddItem(new GUIContent("Create New Layer"), false, new GenericMenu.MenuFunction(this.AddNewLayer));
                        menu.ShowAsContext();
                    }
                }
                else
                {
                    EditorGUI.BeginChangeCheck();
                    GUILayout.Toggle(i == (elementArray.Length - 1), element.name, (i != 0) ? s_Styles.breadCrumbMid : s_Styles.breadCrumbLeft, new GUILayoutOption[0]);
                    if (EditorGUI.EndChangeCheck())
                    {
                        this.GoToBreadCrumbTarget(element.target);
                    }
                }
            }
            GUILayout.FlexibleSpace();
            using (new EditorGUI.DisabledScope(this.animatorController == null))
            {
                if (Unsupported.IsDeveloperBuild() && GUILayout.Button("Select Graph", EditorStyles.toolbarButton, new GUILayoutOption[0]))
                {
                    if (this.m_BreadCrumbs.Last<BreadCrumbElement>().target is AnimatorStateMachine)
                    {
                        Selection.activeObject = this.stateMachineGraph;
                    }
                    else
                    {
                        Selection.activeObject = this.blendTreeGraph;
                    }
                }
            }
            BreadCrumbElement element2 = this.m_BreadCrumbs.LastOrDefault<BreadCrumbElement>();
            if ((element2 != null) && (element2.target is AnimatorStateMachine))
            {
                this.m_AutoLiveLink = GUILayout.Toggle(this.m_AutoLiveLink, "Auto Live Link", EditorStyles.toolbarButton, new GUILayoutOption[0]);
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        [UnityEditor.MenuItem("Window/Animator", false, 0x7dc)]
        public static void DoWindow()
        {
            System.Type[] desiredDockNextTo = new System.Type[] { typeof(SceneView) };
            EditorWindow.GetWindow<AnimatorControllerTool>(desiredDockNextTo);
        }

        private void FlipLocked()
        {
            this.isLocked = !this.isLocked;
            if (!this.isLocked)
            {
                this.OnSelectionChange();
            }
        }

        private AnimatorStateMachine GetParentStateMachine()
        {
            if (this.m_BreadCrumbs.Count == 1)
            {
                return null;
            }
            return (this.m_BreadCrumbs[this.m_BreadCrumbs.Count - 2].target as AnimatorStateMachine);
        }

        public void GoToBreadCrumbTarget(UnityEngine.Object target)
        {
            <GoToBreadCrumbTarget>c__AnonStorey0 storey = new <GoToBreadCrumbTarget>c__AnonStorey0 {
                target = target
            };
            this.StoreCurrentViewPosition();
            int num = this.m_BreadCrumbs.FindIndex(new Predicate<BreadCrumbElement>(storey.<>m__0));
            while (this.m_BreadCrumbs.Count > (num + 1))
            {
                this.m_BreadCrumbs.RemoveAt(num + 1);
            }
            this.CenterOnStoredPosition(this.m_BreadCrumbs.Last<BreadCrumbElement>().target);
            this.UpdateStateMachineSelection();
        }

        private void GoToParentBreadcrumb()
        {
            this.StoreCurrentViewPosition();
            if (this.m_BreadCrumbs.Count > 1)
            {
                this.m_BreadCrumbs.RemoveAt(this.m_BreadCrumbs.Count - 1);
                this.CenterOnStoredPosition(this.m_BreadCrumbs.Last<BreadCrumbElement>().target);
                this.UpdateStateMachineSelection();
            }
        }

        private void Init()
        {
            if (this.m_LayerEditor == null)
            {
                this.m_LayerEditor = new LayerControllerView();
            }
            this.m_LayerEditor.Init(this);
            if (this.m_ParameterEditor == null)
            {
                this.m_ParameterEditor = new ParameterControllerView();
                this.m_ParameterEditor.Init(this);
            }
            if (this.stateMachineGraph == null)
            {
                this.stateMachineGraph = ScriptableObject.CreateInstance<UnityEditor.Graphs.AnimationStateMachine.Graph>();
                this.stateMachineGraph.hideFlags = HideFlags.HideAndDontSave;
            }
            if (this.stateMachineGraphGUI == null)
            {
                this.stateMachineGraphGUI = this.stateMachineGraph.GetEditor() as UnityEditor.Graphs.AnimationStateMachine.GraphGUI;
            }
            if (this.blendTreeGraph == null)
            {
                this.blendTreeGraph = ScriptableObject.CreateInstance<UnityEditor.Graphs.AnimationBlendTree.Graph>();
                this.blendTreeGraph.hideFlags = HideFlags.HideAndDontSave;
            }
            if (this.blendTreeGraphGUI == null)
            {
                this.blendTreeGraphGUI = this.blendTreeGraph.GetEditor() as UnityEditor.Graphs.AnimationBlendTree.GraphGUI;
            }
            if (this.m_BreadCrumbs == null)
            {
                this.m_BreadCrumbs = new List<BreadCrumbElement>();
                this.ResetBreadCrumbs();
            }
            tool = this;
        }

        private bool IsPreviewController(AnimatorController controller) => 
            ((((controller != null) && (controller.name == "")) && ((controller.layers.Length > 0) && (controller.layers[0].name == "preview"))) && (controller.hideFlags == HideFlags.DontSave));

        private void OnControllerChange()
        {
            if ((this.m_PreviewAnimator != null) && (!EditorApplication.isPlaying || (this.m_PreviewAnimator.runtimeAnimatorController != null)))
            {
                AnimatorController effectiveAnimatorController = AnimatorController.GetEffectiveAnimatorController(this.m_PreviewAnimator);
                if ((effectiveAnimatorController != null) && (effectiveAnimatorController != this.animatorController))
                {
                    this.animatorController = effectiveAnimatorController;
                }
            }
        }

        public void OnDisable()
        {
            BlendTreeInspector.blendParameterInputChanged = (Action<BlendTree>) Delegate.Remove(BlendTreeInspector.blendParameterInputChanged, new Action<BlendTree>(this.BlendParameterInputChanged));
            this.editor.OnDisable();
            Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
        }

        public void OnEnable()
        {
            base.titleContent = base.GetLocalizedTitleContent();
            this.Init();
            this.DetectAnimatorControllerFromSelection();
            BlendTreeInspector.blendParameterInputChanged = (Action<BlendTree>) Delegate.Combine(BlendTreeInspector.blendParameterInputChanged, new Action<BlendTree>(this.BlendParameterInputChanged));
            BlendTreeInspector.currentController = this.m_AnimatorController;
            BlendTreeInspector.currentAnimator = this.m_PreviewAnimator;
            this.editor.OnEnable();
            Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
        }

        public void OnFocus()
        {
            this.Init();
            this.DetectAnimatorControllerFromSelection();
            this.DetectPreviewObjectFromSelection();
            this.editor.OnFocus();
        }

        public void OnGUI()
        {
            using (new ScopedPreventWarnings(this.previewAnimator))
            {
                EventType type = Event.current.type;
                int button = Event.current.button;
                base.autoRepaintOnSceneChange = true;
                if (s_Styles == null)
                {
                    s_Styles = new Styles();
                }
                this.OnControllerChange();
                BlendTreeInspector.currentController = this.m_AnimatorController;
                BlendTreeInspector.currentAnimator = this.m_PreviewAnimator;
                if (this.miniTool)
                {
                    Rect paneRect = new Rect(0f, 0f, base.position.width, base.position.height);
                    this.OnGUIGraph(paneRect);
                }
                else
                {
                    if ((this.m_VerticalSplitter == null) || (this.m_VerticalSplitter.realSizes.Length != 2))
                    {
                        int[] realSizes = new int[] { (int) (base.position.width * 0.25f), (int) (base.position.width * 0.75f) };
                        int[] minSizes = new int[] { 150, 100 };
                        this.m_VerticalSplitter = new SplitterState(realSizes, minSizes, null);
                    }
                    GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true) };
                    SplitterGUILayout.BeginHorizontalSplit(this.m_VerticalSplitter, options);
                    SplitterGUILayout.EndHorizontalSplit();
                    int num2 = this.m_VerticalSplitter.realSizes[0];
                    int num3 = this.m_VerticalSplitter.realSizes[1];
                    Rect rect6 = new Rect(0f, 0f, (float) num2, base.position.height);
                    Rect rect8 = new Rect((float) num2, 0f, (float) num3, base.position.height);
                    Rect topToolBarRect = new Rect(rect6.x, rect6.y, rect6.width, 17f);
                    this.OnGUIEditorToolbar(topToolBarRect);
                    Rect editorRect = new Rect(0f, 17f, rect6.width, rect6.height - 17f);
                    this.OnGUIEditor(editorRect);
                    if ((Event.current.type == EventType.MouseDown) && this.editor.HasKeyboardControl())
                    {
                        this.editor.ReleaseKeyboardFocus();
                    }
                    else if (((this.activeGraphGUI != null) && (type == EventType.MouseDown)) && (type != Event.current.type))
                    {
                        this.activeGraphGUI.ClearSelection();
                    }
                    this.OnGUIGraph(rect8);
                }
                if (Event.current.type == EventType.MouseDown)
                {
                    GUIUtility.keyboardControl = 0;
                    EditorGUI.EndEditingActiveTextField();
                }
                if ((((this.activeGraphGUI != null) && (type == EventType.MouseDown)) && ((button == 0) && (this.activeGraphGUI.selection.Count == 0))) && (this.activeGraphGUI.edgeGUI.edgeSelection.Count == 0))
                {
                    this.activeGraphGUI.DoBackgroundClickAction();
                }
            }
        }

        public void OnGUIEditor(Rect editorRect)
        {
            this.editor.OnEvent();
            GUILayout.BeginArea(editorRect);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Height(17f) };
            GUILayout.BeginHorizontal(EditorStyles.toolbar, options);
            GUILayout.Space(10f);
            GUILayout.FlexibleSpace();
            this.editor.OnToolbarGUI();
            GUILayout.EndHorizontal();
            this.editor.OnGUI(editorRect);
            GUILayout.EndArea();
            if ((Event.current.type == EventType.ContextClick) && editorRect.Contains(Event.current.mousePosition))
            {
                Event.current.Use();
            }
        }

        public void OnGUIEditorToolbar(Rect topToolBarRect)
        {
            GUILayout.BeginArea(topToolBarRect);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Height(17f) };
            GUILayout.BeginHorizontal(EditorStyles.toolbar, options);
            GUILayout.Space(4f);
            EditorGUI.BeginChangeCheck();
            GUILayout.Toggle(this.currentEditor == 0, s_Styles.layers, EditorStyles.toolbarButton, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                this.currentEditor = 0;
            }
            GUILayout.Space(4f);
            EditorGUI.BeginChangeCheck();
            GUILayout.Toggle(this.currentEditor == 1, s_Styles.parameters, EditorStyles.toolbarButton, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                this.currentEditor = 1;
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(s_Styles.visibleON, s_Styles.invisibleButton, new GUILayoutOption[0]))
            {
                this.miniTool = true;
            }
            GUILayout.Space(4f);
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private void OnGUIGraph(Rect paneRect)
        {
            Rect toolbarRect = new Rect(paneRect.x, paneRect.y, paneRect.width, 17f);
            Rect screenRect = new Rect(paneRect.x, 17f, paneRect.width, (paneRect.height - 17f) - 18f);
            Rect nameRect = new Rect(paneRect.x, paneRect.height - 18f, paneRect.width, 18f);
            EventType type = Event.current.type;
            this.DoGraphToolbar(toolbarRect);
            if (this.animatorController != null)
            {
                if (this.animatorController.isAssetBundled)
                {
                    GUILayout.BeginArea(screenRect);
                    GUILayout.Label("Cannot show controller from asset bundle", new GUILayoutOption[0]);
                    GUILayout.EndArea();
                    return;
                }
                BlendTreeInspector.parentBlendTree = null;
                if (this.m_BreadCrumbs.Count > 0)
                {
                    UnityEngine.Object target = this.m_BreadCrumbs.Last<BreadCrumbElement>().target;
                    if (this.m_BreadCrumbs.Last<BreadCrumbElement>().target is AnimatorStateMachine)
                    {
                        this.StateMachineView(screenRect);
                    }
                    if (this.m_BreadCrumbs.Last<BreadCrumbElement>().target is AnimatorState)
                    {
                        AnimatorState state = this.m_BreadCrumbs.Last<BreadCrumbElement>().target as AnimatorState;
                        this.BlendTreeView(screenRect);
                        BlendTreeInspector.parentBlendTree = tool.animatorController.GetStateEffectiveMotion(state, tool.selectedLayerIndex) as BlendTree;
                    }
                    if (this.m_BreadCrumbs.Last<BreadCrumbElement>().target is BlendTree)
                    {
                        this.BlendTreeView(screenRect);
                        BlendTreeInspector.parentBlendTree = this.m_BreadCrumbs.Last<BreadCrumbElement>().target as BlendTree;
                    }
                }
                if (((this.activeGraphGUI != null) && (type == EventType.MouseDown)) && ((Event.current.type == type) && (Event.current.clickCount == 1)))
                {
                    this.activeGraphGUI.ClearSelection();
                }
                if (((Event.current.type == EventType.MouseDown) && (Event.current.clickCount == 2)) && (Event.current.button == 0))
                {
                    this.GoToParentBreadcrumb();
                    Event.current.Use();
                }
            }
            else
            {
                this.StateMachineView(screenRect);
            }
            this.DoGraphBottomBar(nameRect);
        }

        public void OnInvalidateAnimatorController()
        {
            this.editor.ResetUI();
            base.Repaint();
            if (this.stateMachineGraph != null)
            {
                if (this.stateMachineGraph.activeStateMachine == null)
                {
                    this.RebuildGraph();
                }
                this.stateMachineGraph.ReadNodePositions();
            }
            if (this.blendTreeGraph != null)
            {
                this.blendTreeGraph.rootBlendTree = null;
            }
        }

        private void OnLostFocus()
        {
            this.editor.OnLostFocus();
        }

        public void OnProjectChange()
        {
            this.DetectAnimatorControllerFromSelection();
        }

        public void OnSelectionChange()
        {
            if (!this.isLocked)
            {
                this.DetectAnimatorControllerFromSelection();
                this.DetectPreviewObjectFromSelection();
                base.Repaint();
            }
        }

        public void RebuildGraph()
        {
            if (!this.ValidateBreadCrumbs())
            {
                this.UpdateStateMachineSelection();
            }
            this.stateMachineGraph.RebuildGraph();
            if (graphDirtyCallback != null)
            {
                graphDirtyCallback();
            }
        }

        private void ResetBreadCrumbs()
        {
            this.m_BreadCrumbs.Clear();
            if (((this.animatorController != null) && (this.animatorController.layers.Length != 0)) && !this.animatorController.isAssetBundled)
            {
                if (this.selectedLayerIndex == -1)
                {
                    this.selectedLayerIndex = 0;
                }
                if (this.selectedLayerIndex < this.animatorController.layers.Length)
                {
                    int syncedLayerIndex = this.animatorController.layers[this.selectedLayerIndex].syncedLayerIndex;
                    AnimatorStateMachine target = (syncedLayerIndex != -1) ? this.animatorController.layers[syncedLayerIndex].stateMachine : this.animatorController.layers[this.selectedLayerIndex].stateMachine;
                    if (target == null)
                    {
                        return;
                    }
                    this.AddBreadCrumb(target, false);
                    this.stateMachineGraphGUI.ClearSelection();
                    this.blendTreeGraphGUI.ClearSelection();
                }
                base.Repaint();
            }
        }

        public void ResetUI()
        {
            this.ResetBreadCrumbs();
        }

        private void SetCurrentLayer(object i)
        {
            this.selectedLayerIndex = (int) i;
        }

        private void ShowButton(Rect position)
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            if (GUI.Toggle(position, this.isLocked, GUIContent.none, s_Styles.lockButtonStyle) != this.isLocked)
            {
                this.FlipLocked();
            }
        }

        private void StateDirty()
        {
            this.RebuildGraph();
        }

        private void StateMachineView(Rect position)
        {
            if (this.animatorController != null)
            {
                this.stateMachineGraph.SetStateMachines(this.m_BreadCrumbs.Last<BreadCrumbElement>().target as AnimatorStateMachine, this.GetParentStateMachine(), this.m_BreadCrumbs.First<BreadCrumbElement>().target as AnimatorStateMachine);
            }
            this.stateMachineGraphGUI.BeginGraphGUI(this, position);
            if (this.animatorController != null)
            {
                this.stateMachineGraphGUI.OnGraphGUI();
            }
            this.stateMachineGraphGUI.EndGraphGUI();
        }

        private void StoreCurrentViewPosition()
        {
            if ((this.m_BreadCrumbs.Count > 0) && (this.activeGraphGUI != null))
            {
                this.m_ViewPositions.SetViewPosition(this.m_BreadCrumbs.Last<BreadCrumbElement>().target, this.activeGraphGUI.scrollPosition);
            }
        }

        public void UndoRedoPerformed()
        {
            this.StateDirty();
            this.stateMachineGraphGUI.SyncGraphToUnitySelection();
            this.blendTreeGraphGUI.SyncGraphToUnitySelection();
        }

        void IAnimatorControllerEditor.Repaint()
        {
            base.Repaint();
        }

        private void UpdateStateMachineSelection()
        {
            AnimatorStateMachine activeStateMachine = this.stateMachineGraph.activeStateMachine;
            AnimatorStateMachine target = this.m_BreadCrumbs.Last<BreadCrumbElement>().target as AnimatorStateMachine;
            if ((target != activeStateMachine) && (target != null))
            {
                this.stateMachineGraph.SetStateMachines(target, this.GetParentStateMachine(), this.m_BreadCrumbs.First<BreadCrumbElement>().target as AnimatorStateMachine);
                this.stateMachineGraphGUI.ClearSelection();
                this.blendTreeGraphGUI.ClearSelection();
                InspectorWindow.RepaintAllInspectors();
            }
        }

        private bool ValidateBreadCrumbs()
        {
            int count = this.m_BreadCrumbs.Count;
            if (count > 1)
            {
                UnityEngine.Object target = this.m_BreadCrumbs.First<BreadCrumbElement>().target;
                int index = 1;
                while (index < count)
                {
                    UnityEngine.Object obj3 = this.m_BreadCrumbs[index].target;
                    if (target is AnimatorStateMachine)
                    {
                        if (((obj3 is AnimatorState) && !(target as AnimatorStateMachine).HasState(obj3 as AnimatorState)) || ((obj3 is AnimatorStateMachine) && !(target as AnimatorStateMachine).HasStateMachine(obj3 as AnimatorStateMachine)))
                        {
                            break;
                        }
                    }
                    else if ((((target is BlendTree) && (obj3 is BlendTree)) && !(target as BlendTree).HasChild(obj3 as BlendTree, true)) || (((target is AnimatorState) && (obj3 is BlendTree)) && !((target as AnimatorState).motion as BlendTree).HasChild(obj3 as BlendTree, true)))
                    {
                        break;
                    }
                    target = this.m_BreadCrumbs[index].target;
                    index++;
                }
                if (index < count)
                {
                    this.m_BreadCrumbs.RemoveRange(index, count - index);
                    return false;
                }
            }
            return true;
        }

        protected UnityEditor.Graphs.GraphGUI activeGraphGUI
        {
            get
            {
                if (this.m_BreadCrumbs.Count == 0)
                {
                    return null;
                }
                if (this.m_BreadCrumbs.Last<BreadCrumbElement>().target is AnimatorStateMachine)
                {
                    return this.stateMachineGraphGUI;
                }
                return this.blendTreeGraphGUI;
            }
        }

        public AnimatorController animatorController
        {
            get => 
                this.m_AnimatorController;
            set
            {
                if (!this.IsPreviewController(value))
                {
                    this.m_AnimatorController = value;
                    this.editor.ResetUI();
                    this.ResetBreadCrumbs();
                    if (this.m_PreviewAnimator != null)
                    {
                        if ((this.m_PreviewAnimator.runtimeAnimatorController != null) && (this.m_PreviewAnimator.runtimeAnimatorController != this.m_AnimatorController))
                        {
                            this.m_PreviewAnimator = null;
                        }
                        else if (AnimatorController.FindAnimatorControllerPlayable(this.m_PreviewAnimator, this.m_AnimatorController) == null)
                        {
                            this.m_PreviewAnimator = null;
                        }
                    }
                }
            }
        }

        public bool autoLiveLink =>
            this.m_AutoLiveLink;

        protected int currentEditor
        {
            get => 
                this.m_CurrentEditor;
            set
            {
                this.editor.OnDisable();
                this.m_CurrentEditor = value;
                this.editor.OnEnable();
            }
        }

        protected IAnimatorControllerSubEditor editor
        {
            get
            {
                switch (this.m_CurrentEditor)
                {
                    case 1:
                        return this.m_ParameterEditor;
                }
                return this.m_LayerEditor;
            }
        }

        public bool isLocked
        {
            get
            {
                if (this.animatorController == null)
                {
                    return false;
                }
                return this.m_SerializedLocked;
            }
            set
            {
                this.m_SerializedLocked = value;
            }
        }

        public bool liveLink =>
            (((EditorApplication.isPlaying && (this.m_PreviewAnimator != null)) && this.m_PreviewAnimator.enabled) && this.m_PreviewAnimator.gameObject.activeInHierarchy);

        public bool miniTool
        {
            get => 
                this.m_MiniTool;
            set
            {
                this.m_MiniTool = value;
            }
        }

        public Animator previewAnimator =>
            this.m_PreviewAnimator;

        public int selectedLayerIndex
        {
            get => 
                ((this.m_LayerEditor == null) ? 0 : this.m_LayerEditor.selectedLayerIndex);
            set
            {
                if (this.m_LayerEditor != null)
                {
                    this.m_LayerEditor.selectedLayerIndex = value;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <GoToBreadCrumbTarget>c__AnonStorey0
        {
            internal UnityEngine.Object target;

            internal bool <>m__0(AnimatorControllerTool.BreadCrumbElement o) => 
                (o.target == this.target);
        }

        [Serializable]
        private class AnimatorViewPositionCache : ISerializationCallbackReceiver
        {
            [SerializeField]
            private List<UnityEngine.Object> m_KeySerializationHelper = new List<UnityEngine.Object>();
            [SerializeField]
            private List<Vector2> m_ValueSerializationHelper = new List<Vector2>();
            private Dictionary<UnityEngine.Object, Vector2> m_ViewPositions = new Dictionary<UnityEngine.Object, Vector2>();

            public Vector2 GetViewPosition(UnityEngine.Object target)
            {
                Vector2 vector;
                if (!this.m_ViewPositions.TryGetValue(target, out vector))
                {
                    this.m_ViewPositions.Add(target, vector);
                }
                return vector;
            }

            public void OnAfterDeserialize()
            {
                this.m_ViewPositions = new Dictionary<UnityEngine.Object, Vector2>();
                for (int i = 0; i != Math.Min(this.m_KeySerializationHelper.Count, this.m_ValueSerializationHelper.Count); i++)
                {
                    this.m_ViewPositions.Add(this.m_KeySerializationHelper[i], this.m_ValueSerializationHelper[i]);
                }
            }

            public void OnBeforeSerialize()
            {
                this.m_KeySerializationHelper.Clear();
                this.m_ValueSerializationHelper.Clear();
                foreach (KeyValuePair<UnityEngine.Object, Vector2> pair in this.m_ViewPositions)
                {
                    this.m_KeySerializationHelper.Add(pair.Key);
                    this.m_ValueSerializationHelper.Add(pair.Value);
                }
            }

            public void SetViewPosition(UnityEngine.Object target, Vector2 position)
            {
                if (!this.m_ViewPositions.ContainsKey(target))
                {
                    this.m_ViewPositions.Add(target, position);
                }
                else
                {
                    this.m_ViewPositions[target] = position;
                }
            }
        }

        [Serializable]
        private class BreadCrumbElement
        {
            [SerializeField]
            private Vector2 m_ScrollPosition;
            [SerializeField]
            private UnityEngine.Object m_Target;

            public BreadCrumbElement(UnityEngine.Object target)
            {
                this.m_Target = target;
            }

            public string name =>
                ((this.m_Target == null) ? "" : this.m_Target.name);

            public Vector2 scrollPosition
            {
                get => 
                    this.m_ScrollPosition;
                set
                {
                    this.m_ScrollPosition = value;
                }
            }

            public UnityEngine.Object target =>
                this.m_Target;
        }

        private class ScopedPreventWarnings : IDisposable
        {
            private Animator m_Animator;
            private bool m_WasLoggingWarning = false;

            public ScopedPreventWarnings(Animator animator)
            {
                this.m_Animator = animator;
                if (this.m_Animator != null)
                {
                    this.m_WasLoggingWarning = this.m_Animator.logWarnings;
                    this.m_Animator.logWarnings = false;
                }
            }

            public virtual void Dispose()
            {
                if (this.m_Animator != null)
                {
                    this.m_Animator.logWarnings = this.m_WasLoggingWarning;
                }
            }
        }

        private class Styles
        {
            public readonly GUIStyle bottomBarDarkBg = "In BigTitle";
            public readonly GUIStyle breadCrumbLeft = "GUIEditor.BreadcrumbLeft";
            public readonly GUIStyle breadCrumbMid = "GUIEditor.BreadcrumbMid";
            public readonly GUIStyle invisibleButton = "InvisibleButton";
            public readonly GUIContent layers = EditorGUIUtility.TextContent("Layers|Click to edit controller's layers.");
            public readonly GUIStyle liveLinkLabel = new GUIStyle("miniLabel");
            public readonly GUIStyle lockButtonStyle = "IN LockButton";
            public readonly GUIStyle nameLabel = new GUIStyle("miniLabel");
            public readonly GUIContent parameters = EditorGUIUtility.TextContent("Parameters|Click to edit controller's parameters.");
            public readonly GUIContent visibleOFF = EditorGUIUtility.IconContent("animationvisibilitytoggleoff");
            public readonly GUIContent visibleON = EditorGUIUtility.IconContent("animationvisibilitytoggleon");

            public Styles()
            {
                this.nameLabel.alignment = TextAnchor.MiddleRight;
                this.nameLabel.padding = new RectOffset(0, 0, 0, 0);
                this.nameLabel.margin = new RectOffset(0, 0, 0, 0);
                this.liveLinkLabel.alignment = TextAnchor.MiddleLeft;
                this.liveLinkLabel.padding = new RectOffset(0, 0, 0, 0);
                this.liveLinkLabel.margin = new RectOffset(0, 0, 0, 0);
            }
        }
    }
}

