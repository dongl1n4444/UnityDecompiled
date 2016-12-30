namespace UnityEditor.Graphs.AnimationStateMachine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEditor.Animations;
    using UnityEditor.Graphs;
    using UnityEngine;
    using UnityEngine.Experimental.Director;

    internal class GraphGUI : UnityEditor.Graphs.GraphGUI
    {
        private AnimatorDefaultTransition m_DefaultTransition = null;
        private StateMachineNode m_HoveredStateMachineNode;
        private LiveLinkInfo m_LiveLinkInfo;

        private void AddStateEmptyCallback(object data)
        {
            this.activeStateMachine.AddState("New State", (Vector3) ((Vector2) data));
            AnimatorControllerTool.tool.RebuildGraph();
        }

        private void AddStateFromNewBlendTreeCallback(object data)
        {
            Undo.RegisterCompleteObjectUndo(this.activeStateMachine, "Blend Tree State Added");
            AnimatorState state = this.activeStateMachine.AddState("Blend Tree", (Vector3) ((Vector2) data));
            BlendTree objectToAdd = new BlendTree {
                hideFlags = HideFlags.HideInHierarchy
            };
            if (AssetDatabase.GetAssetPath(this.tool.animatorController) != "")
            {
                AssetDatabase.AddObjectToAsset(objectToAdd, AssetDatabase.GetAssetPath(this.tool.animatorController));
            }
            objectToAdd.name = "Blend Tree";
            string defaultBlendTreeParameter = this.tool.animatorController.GetDefaultBlendTreeParameter();
            objectToAdd.blendParameterY = defaultBlendTreeParameter;
            objectToAdd.blendParameter = defaultBlendTreeParameter;
            this.tool.animatorController.SetStateEffectiveMotion(state, objectToAdd, this.tool.selectedLayerIndex);
            AnimatorControllerTool.tool.RebuildGraph();
        }

        private void AddStateFromSelectedMotionCallback(object data)
        {
            AnimationClip activeObject = Selection.activeObject as AnimationClip;
            AnimatorState state = this.activeStateMachine.AddState(activeObject.name, (Vector3) ((Vector2) data));
            this.tool.animatorController.SetStateEffectiveMotion(state, activeObject, this.tool.selectedLayerIndex);
            AnimatorControllerTool.tool.RebuildGraph();
        }

        private void AddStateMachineCallback(object data)
        {
            Undo.RegisterCompleteObjectUndo(this.activeStateMachine, "Sub-State Machine Added");
            this.activeStateMachine.AddStateMachine("New StateMachine", (Vector3) ((Vector2) data));
            AnimatorControllerTool.tool.RebuildGraph();
        }

        public override void ClearSelection()
        {
            base.selection.Clear();
            this.edgeGUI.edgeSelection.Clear();
            this.UpdateUnitySelection();
        }

        private List<string> CollectSelectionNames()
        {
            List<string> list = new List<string>();
            foreach (UnityEditor.Graphs.AnimationStateMachine.Node node in base.selection)
            {
                if (node is StateNode)
                {
                    list.Add((node as StateNode).state.name);
                }
                else if (node is StateMachineNode)
                {
                    AnimatorStateMachine stateMachine = (node as StateMachineNode).stateMachine;
                    if (this.parentStateMachine != stateMachine)
                    {
                        list.Add(stateMachine.name);
                    }
                }
            }
            foreach (int num in this.edgeGUI.edgeSelection)
            {
                EdgeInfo edgeInfo = this.stateMachineGraph.GetEdgeInfo(base.graph.edges[num]);
                foreach (TransitionEditionContext context in edgeInfo.transitions)
                {
                    if (!context.isDefaultTransition)
                    {
                        list.Add(context.displayName);
                    }
                }
            }
            return list;
        }

        private List<AnimationClip> ComputeDraggedClipsFromModelImporter()
        {
            List<AnimationClip> list = new List<AnimationClip>();
            List<GameObject> list2 = DragAndDrop.objectReferences.OfType<GameObject>().ToList<GameObject>();
            for (int i = 0; i < list2.Count; i++)
            {
                ModelImporter atPath = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(list2[i])) as ModelImporter;
                if (atPath != null)
                {
                    UnityEngine.Object[] objArray = AssetDatabase.LoadAllAssetsAtPath(atPath.assetPath);
                    foreach (UnityEngine.Object obj2 in objArray)
                    {
                        if ((obj2.hideFlags & HideFlags.HideInHierarchy) == HideFlags.None)
                        {
                            AnimationClip item = obj2 as AnimationClip;
                            if (item != null)
                            {
                                list.Add(item);
                            }
                        }
                    }
                }
            }
            return list;
        }

        public bool CopySelectionToPasteboard()
        {
            UnityEngine.Object[] array = new UnityEngine.Object[0];
            Vector3[] vectorArray = new Vector3[0];
            foreach (UnityEditor.Graphs.AnimationStateMachine.Node node in base.selection)
            {
                if (node is StateNode)
                {
                    ArrayUtility.Add<UnityEngine.Object>(ref array, (node as StateNode).state);
                    ArrayUtility.Add<Vector3>(ref vectorArray, new Vector3(node.position.x, node.position.y, 0f));
                }
                else if ((node is StateMachineNode) && ((node as StateMachineNode).stateMachine != AnimatorControllerTool.tool.stateMachineGraph.parentStateMachine))
                {
                    ArrayUtility.Add<UnityEngine.Object>(ref array, (node as StateMachineNode).stateMachine);
                    ArrayUtility.Add<Vector3>(ref vectorArray, new Vector3(node.position.x, node.position.y, 0f));
                }
            }
            foreach (int num in this.edgeGUI.edgeSelection)
            {
                EdgeInfo edgeInfo = this.stateMachineGraph.GetEdgeInfo(base.graph.edges[num]);
                foreach (TransitionEditionContext context in edgeInfo.transitions)
                {
                    if (context.transition != null)
                    {
                        ArrayUtility.Add<UnityEngine.Object>(ref array, context.transition);
                        ArrayUtility.Add<Vector3>(ref vectorArray, Vector3.zero);
                    }
                }
            }
            Unsupported.CopyStateMachineDataToPasteboard(array, vectorArray, this.tool.animatorController, this.tool.selectedLayerIndex);
            return (array.Length > 0);
        }

        private void CopyStateMachineCallback(object data)
        {
            Unsupported.CopyStateMachineDataToPasteboard(this.activeStateMachine, AnimatorControllerTool.tool.animatorController, AnimatorControllerTool.tool.selectedLayerIndex);
        }

        private void DeleteSelectedEdges()
        {
            bool flag = false;
            List<UnityEditor.Graphs.Edge> list = new List<UnityEditor.Graphs.Edge>();
            foreach (int num in this.edgeGUI.edgeSelection)
            {
                list.Add(base.graph.edges[num]);
                flag = true;
            }
            foreach (UnityEditor.Graphs.Edge edge in list)
            {
                EdgeInfo edgeInfo = this.stateMachineGraph.GetEdgeInfo(edge);
                foreach (TransitionEditionContext context in edgeInfo.transitions)
                {
                    context.Remove(false);
                    flag = true;
                }
            }
            if (flag)
            {
                this.stateMachineGraph.RebuildGraph();
            }
            this.edgeGUI.edgeSelection.Clear();
        }

        private void DeleteSelectedNodes()
        {
            foreach (UnityEditor.Graphs.AnimationStateMachine.Node node in base.selection)
            {
                if (node is StateNode)
                {
                    AnimatorState state = (node as StateNode).state;
                    this.activeStateMachine.RemoveState(state);
                    this.stateMachineGraph.RemoveNode(node, false);
                }
                if (node is StateMachineNode)
                {
                    AnimatorStateMachine stateMachine = (node as StateMachineNode).stateMachine;
                    if (this.parentStateMachine != stateMachine)
                    {
                        this.activeStateMachine.RemoveStateMachine(stateMachine);
                        this.stateMachineGraph.RemoveNode(node, false);
                    }
                }
            }
        }

        public void DeleteSelection()
        {
            if (this.CollectSelectionNames().Count != 0)
            {
                this.DeleteSelectedEdges();
                this.DeleteSelectedNodes();
                this.ClearSelection();
                this.UpdateUnitySelection();
            }
        }

        public override void DoBackgroundClickAction()
        {
            Selection.objects = new List<UnityEngine.Object> { this.activeStateMachine }.ToArray();
        }

        private void HandleContextMenu()
        {
            if (Event.current.type == EventType.ContextClick)
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Create State/Empty"), false, new GenericMenu.MenuFunction2(this.AddStateEmptyCallback), Event.current.mousePosition);
                if (HasMotionSelected())
                {
                    menu.AddItem(new GUIContent("Create State/From Selected Clip"), false, new GenericMenu.MenuFunction2(this.AddStateFromSelectedMotionCallback), Event.current.mousePosition);
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent("Create State/From Selected Clip"));
                }
                menu.AddItem(new GUIContent("Create State/From New Blend Tree"), false, new GenericMenu.MenuFunction2(this.AddStateFromNewBlendTreeCallback), Event.current.mousePosition);
                menu.AddItem(new GUIContent("Create Sub-State Machine"), false, new GenericMenu.MenuFunction2(this.AddStateMachineCallback), Event.current.mousePosition);
                if (Unsupported.HasStateMachineDataInPasteboard())
                {
                    menu.AddItem(new GUIContent("Paste"), false, new GenericMenu.MenuFunction2(this.PasteCallback), Event.current.mousePosition);
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent("Paste"));
                }
                menu.AddItem(new GUIContent("Copy current StateMachine"), false, new GenericMenu.MenuFunction2(this.CopyStateMachineCallback), Event.current.mousePosition);
                menu.ShowAsContext();
            }
        }

        private void HandleEvents()
        {
            Event current = Event.current;
            switch (current.type)
            {
                case EventType.KeyDown:
                    if (current.keyCode == KeyCode.Delete)
                    {
                        this.DeleteSelection();
                        current.Use();
                    }
                    break;

                case EventType.ValidateCommand:
                    if (((current.commandName == "SoftDelete") || (current.commandName == "Delete")) || (((current.commandName == "Copy") || (current.commandName == "Paste")) || (current.commandName == "Duplicate")))
                    {
                        current.Use();
                    }
                    break;

                case EventType.ExecuteCommand:
                    if ((current.commandName == "SoftDelete") || (current.commandName == "Delete"))
                    {
                        this.DeleteSelection();
                        current.Use();
                    }
                    if (current.commandName == "Copy")
                    {
                        this.CopySelectionToPasteboard();
                        current.Use();
                    }
                    else if (current.commandName == "Paste")
                    {
                        Unsupported.PasteToStateMachineFromPasteboard(this.activeStateMachine, this.tool.animatorController, this.tool.selectedLayerIndex, Vector3.zero);
                        current.Use();
                    }
                    else if ((current.commandName == "Duplicate") && this.CopySelectionToPasteboard())
                    {
                        Vector3 zero = Vector3.zero;
                        if (base.selection.Count > 0)
                        {
                            zero.Set(base.selection[0].position.x, base.selection[0].position.y, 0f);
                        }
                        Unsupported.PasteToStateMachineFromPasteboard(this.activeStateMachine, this.tool.animatorController, this.tool.selectedLayerIndex, zero + new Vector3(40f, 40f, 0f));
                        current.Use();
                    }
                    break;
            }
        }

        private void HandleObjectDragging()
        {
            Event current = Event.current;
            List<Motion> list = DragAndDrop.objectReferences.OfType<Motion>().ToList<Motion>();
            List<AnimatorState> list2 = DragAndDrop.objectReferences.OfType<AnimatorState>().ToList<AnimatorState>();
            List<AnimatorStateMachine> list3 = DragAndDrop.objectReferences.OfType<AnimatorStateMachine>().ToList<AnimatorStateMachine>();
            List<AnimationClip> list4 = this.ComputeDraggedClipsFromModelImporter();
            switch (current.type)
            {
                case EventType.Repaint:
                    if (this.isSelectionMoving && (((this.m_HoveredStateMachineNode != null) && !base.selection.Contains(this.m_HoveredStateMachineNode)) && !this.SelectionOnlyUpNode()))
                    {
                        EditorGUIUtility.AddCursorRect(this.m_HoveredStateMachineNode.position, MouseCursor.ArrowPlus);
                    }
                    return;

                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (((list.Count <= 0) && (list2.Count <= 0)) && ((list3.Count <= 0) && (list4.Count <= 0)))
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.None;
                    }
                    else
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                    }
                    if ((current.type == EventType.DragPerform) && (DragAndDrop.visualMode != DragAndDropVisualMode.None))
                    {
                        DragAndDrop.AcceptDrag();
                        Undo.RegisterCompleteObjectUndo(this.activeStateMachine, "Drag motion to state machine.");
                        for (int i = 0; i < list.Count; i++)
                        {
                            AnimatorState state = this.activeStateMachine.AddState(list[i].name, (Vector3) (current.mousePosition + (new Vector2(12f, 12f) * i)));
                            this.tool.animatorController.SetStateEffectiveMotion(state, list[i], this.tool.selectedLayerIndex);
                        }
                        for (int j = 0; j < list2.Count; j++)
                        {
                            this.activeStateMachine.AddState(list2[j], (Vector3) (current.mousePosition + (new Vector2(12f, 12f) * j)));
                        }
                        for (int k = 0; k < list3.Count; k++)
                        {
                            this.activeStateMachine.AddStateMachine(list3[k], (Vector3) (current.mousePosition + (new Vector2(12f, 12f) * k)));
                        }
                        for (int m = 0; m < list4.Count; m++)
                        {
                            AnimatorState state2 = this.activeStateMachine.AddState(list4[m].name, (Vector3) (current.mousePosition + (new Vector2(12f, 12f) * m)));
                            this.tool.animatorController.SetStateEffectiveMotion(state2, list4[m], this.tool.selectedLayerIndex);
                        }
                        this.stateMachineGraph.RebuildGraph();
                    }
                    current.Use();
                    return;

                case EventType.DragExited:
                    current.Use();
                    return;
            }
        }

        private static bool HasMotionSelected() => 
            (Selection.activeObject is Motion);

        private bool IsCurrentStateMachineNodeLiveLinked(UnityEditor.Graphs.AnimationStateMachine.Node n)
        {
            StateMachineNode node = n as StateMachineNode;
            if (node != null)
            {
                AnimatorState currentState = this.liveLinkInfo.currentState;
                bool flag = this.activeStateMachine.HasState(currentState, true);
                bool flag2 = node.stateMachine.HasState(currentState, true);
                bool flag3 = node.stateMachine.HasStateMachine(this.activeStateMachine, false);
                if (((flag3 && flag2) && !flag) || (!flag3 && flag2))
                {
                    return true;
                }
            }
            return false;
        }

        private void LiveLink()
        {
            this.m_LiveLinkInfo.Clear();
            if (this.tool.liveLink)
            {
                AnimatorControllerPlayable controller = AnimatorController.FindAnimatorControllerPlayable(this.tool.previewAnimator, this.tool.animatorController);
                if (controller != null)
                {
                    AnimatorStateInfo currentAnimatorStateInfo = controller.GetCurrentAnimatorStateInfo(AnimatorControllerTool.tool.selectedLayerIndex);
                    AnimatorStateInfo nextAnimatorStateInfo = controller.GetNextAnimatorStateInfo(AnimatorControllerTool.tool.selectedLayerIndex);
                    AnimatorTransitionInfo animatorTransitionInfo = controller.GetAnimatorTransitionInfo(AnimatorControllerTool.tool.selectedLayerIndex);
                    int shortNameHash = currentAnimatorStateInfo.shortNameHash;
                    int nameHash = nextAnimatorStateInfo.shortNameHash;
                    this.m_LiveLinkInfo.currentStateMachine = (shortNameHash == 0) ? null : this.rootStateMachine.FindStateMachine(this.ResolveHash(controller, currentAnimatorStateInfo.fullPathHash));
                    this.m_LiveLinkInfo.currentState = (shortNameHash == 0) ? null : this.m_LiveLinkInfo.currentStateMachine.FindState(shortNameHash).state;
                    this.m_LiveLinkInfo.currentStateNormalizedTime = currentAnimatorStateInfo.normalizedTime;
                    this.m_LiveLinkInfo.currentStateLoopTime = currentAnimatorStateInfo.loop;
                    if (this.m_LiveLinkInfo.currentState != null)
                    {
                        this.m_LiveLinkInfo.nextStateMachine = (nameHash == 0) ? null : this.rootStateMachine.FindStateMachine(this.ResolveHash(controller, nextAnimatorStateInfo.fullPathHash));
                        this.m_LiveLinkInfo.nextState = (nameHash == 0) ? null : this.m_LiveLinkInfo.nextStateMachine.FindState(nameHash).state;
                        this.m_LiveLinkInfo.nextStateNormalizedTime = nextAnimatorStateInfo.normalizedTime;
                        this.m_LiveLinkInfo.nextStateLoopTime = nextAnimatorStateInfo.loop;
                        this.m_LiveLinkInfo.srcNode = this.stateMachineGraph.FindNode(this.m_LiveLinkInfo.currentState);
                        this.m_LiveLinkInfo.dstNode = (this.m_LiveLinkInfo.nextState == null) ? null : this.stateMachineGraph.FindNode(this.m_LiveLinkInfo.nextState);
                        this.m_LiveLinkInfo.transitionInfo = animatorTransitionInfo;
                        if (this.tool.autoLiveLink)
                        {
                            AnimatorStateMachine currentStateMachine = this.m_LiveLinkInfo.currentStateMachine;
                            if (((this.m_LiveLinkInfo.currentState != null) && (this.m_LiveLinkInfo.nextState != null)) && ((this.m_LiveLinkInfo.transitionInfo.normalizedTime > 0.5) || animatorTransitionInfo.anyState))
                            {
                                currentStateMachine = this.m_LiveLinkInfo.nextStateMachine;
                            }
                            if (((shortNameHash != 0) && (currentStateMachine != this.activeStateMachine)) && (Event.current.type == EventType.Repaint))
                            {
                                List<AnimatorStateMachine> hierarchy = new List<AnimatorStateMachine>();
                                MecanimUtilities.StateMachineRelativePath(this.rootStateMachine, currentStateMachine, ref hierarchy);
                                this.tool.BuildBreadCrumbsFromSMHierarchy(hierarchy);
                            }
                        }
                    }
                }
            }
        }

        private bool MoveSelectionTo(StateMachineNode targetSM)
        {
            bool flag = false;
            List<UnityEngine.Object> list = new List<UnityEngine.Object> {
                this.rootStateMachine
            };
            foreach (ChildAnimatorStateMachine machine in this.rootStateMachine.stateMachinesRecursive)
            {
                list.Add(machine.stateMachine);
            }
            foreach (UnityEditor.Graphs.AnimationStateMachine.Node node in base.selection)
            {
                if (node is StateNode)
                {
                    list.Add((node as StateNode).state);
                }
            }
            Undo.RegisterCompleteObjectUndo(list.ToArray(), "Move in StateMachine");
            foreach (UnityEditor.Graphs.AnimationStateMachine.Node node2 in base.selection)
            {
                if (node2 is StateNode)
                {
                    this.activeStateMachine.MoveState((node2 as StateNode).state, targetSM.stateMachine);
                }
                else if (node2 is StateMachineNode)
                {
                    this.activeStateMachine.MoveStateMachine((node2 as StateMachineNode).stateMachine, targetSM.stateMachine);
                }
                flag = true;
            }
            return flag;
        }

        public override void NodeGUI(UnityEditor.Graphs.Node n)
        {
            GUILayoutUtility.GetRect((float) 160f, (float) 0f);
            base.SelectNode(n);
            n.NodeUI(this);
            base.DragNodes();
        }

        public override void OnGraphGUI()
        {
            if (this.stateMachineGraph.DisplayDirty())
            {
                this.stateMachineGraph.RebuildGraph();
            }
            this.SyncGraphToUnitySelection();
            this.LiveLink();
            this.SetHoveredStateMachine();
            base.m_Host.BeginWindows();
            foreach (UnityEditor.Graphs.AnimationStateMachine.Node node in base.m_Graph.nodes)
            {
                <OnGraphGUI>c__AnonStorey0 storey = new <OnGraphGUI>c__AnonStorey0 {
                    $this = this,
                    n2 = node
                };
                bool on = base.selection.Contains(node);
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(0f), GUILayout.Height(0f) };
                node.position = GUILayout.Window(node.GetInstanceID(), node.position, new GUI.WindowFunction(storey.<>m__0), node.title, UnityEditor.Graphs.Styles.GetNodeStyle(node.style, !this.IsCurrentStateMachineNodeLiveLinked(node) ? node.color : UnityEditor.Graphs.Styles.Color.Blue, on), options);
                if ((Event.current.type == EventType.MouseMove) && node.position.Contains(Event.current.mousePosition))
                {
                    this.edgeGUI.SlotDragging(node.inputSlots.First<Slot>(), true, true);
                }
            }
            this.edgeGUI.DoEdges();
            base.m_Host.EndWindows();
            if ((Event.current.type == EventType.MouseDown) && (Event.current.button != 2))
            {
                this.edgeGUI.EndDragging();
            }
            this.HandleEvents();
            this.HandleContextMenu();
            this.HandleObjectDragging();
            base.DragSelection(new Rect(-5000f, -5000f, 10000f, 10000f));
        }

        private void PasteCallback(object data)
        {
            Undo.RegisterCompleteObjectUndo(this.activeStateMachine, "Paste");
            Unsupported.PasteToStateMachineFromPasteboard(this.activeStateMachine, this.tool.animatorController, this.tool.selectedLayerIndex, (Vector3) ((Vector2) data));
            AnimatorControllerTool.tool.RebuildGraph();
        }

        internal string ResolveHash(Animator controller, int fullPathHash) => 
            controller.ResolveHash(fullPathHash);

        internal string ResolveHash(AnimatorControllerPlayable controller, int fullPathHash) => 
            controller.ResolveHash(fullPathHash);

        private bool SelectionOnlyUpNode()
        {
            if ((base.selection.Count == 1) && (base.selection[0] is StateMachineNode))
            {
                AnimatorStateMachine stateMachine = (base.selection[0] as StateMachineNode).stateMachine;
                if (this.parentStateMachine == stateMachine)
                {
                    return true;
                }
            }
            return false;
        }

        private void SetHoveredStateMachine()
        {
            Vector2 mousePosition = Event.current.mousePosition;
            this.m_HoveredStateMachineNode = null;
            foreach (UnityEditor.Graphs.AnimationStateMachine.Node node in base.m_Graph.nodes)
            {
                StateMachineNode item = node as StateMachineNode;
                if ((item != null) && (item.position.Contains(mousePosition) && !base.selection.Contains(item)))
                {
                    this.m_HoveredStateMachineNode = item;
                    break;
                }
            }
        }

        public override void SyncGraphToUnitySelection()
        {
            if (GUIUtility.hotControl == 0)
            {
                base.selection.Clear();
                this.edgeGUI.edgeSelection.Clear();
                foreach (UnityEngine.Object obj2 in Selection.objects)
                {
                    UnityEditor.Graphs.AnimationStateMachine.Node item = null;
                    AnimatorState state = obj2 as AnimatorState;
                    if (state != null)
                    {
                        item = this.stateMachineGraph.FindNode(state);
                    }
                    else
                    {
                        AnimatorStateMachine stateMachine = obj2 as AnimatorStateMachine;
                        if (stateMachine != null)
                        {
                            item = this.stateMachineGraph.FindNode(stateMachine);
                        }
                        else
                        {
                            AnimatorTransitionBase base2 = obj2 as AnimatorTransitionBase;
                            AnimatorDefaultTransition transition = obj2 as AnimatorDefaultTransition;
                            if ((base2 != null) || (transition != null))
                            {
                                foreach (UnityEditor.Graphs.Edge edge in base.m_Graph.edges)
                                {
                                    EdgeInfo edgeInfo = this.stateMachineGraph.GetEdgeInfo(edge);
                                    foreach (TransitionEditionContext context in edgeInfo.transitions)
                                    {
                                        if (((base2 != null) && (context.transition == base2)) || ((transition != null) && (context.transition == null)))
                                        {
                                            int index = base.m_Graph.edges.IndexOf(edge);
                                            if (!this.edgeGUI.edgeSelection.Contains(index))
                                            {
                                                this.edgeGUI.edgeSelection.Add(index);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                item = obj2 as UnityEditor.Graphs.AnimationStateMachine.Node;
                            }
                        }
                    }
                    if (item != null)
                    {
                        base.selection.Add(item);
                    }
                }
            }
        }

        protected override void UpdateUnitySelection()
        {
            List<UnityEngine.Object> list = new List<UnityEngine.Object>();
            foreach (UnityEditor.Graphs.Node node in base.selection)
            {
                if (node is StateNode)
                {
                    list.Add((node as StateNode).state);
                }
                if (node is StateMachineNode)
                {
                    list.Add((node as StateMachineNode).stateMachine);
                }
                if (node is AnyStateNode)
                {
                    list.Add(node);
                }
                if (node is EntryNode)
                {
                    list.Add(node);
                }
                if (node is ExitNode)
                {
                    list.Add(node);
                }
            }
            foreach (int num in this.edgeGUI.edgeSelection)
            {
                EdgeInfo edgeInfo = this.stateMachineGraph.GetEdgeInfo(base.graph.edges[num]);
                foreach (TransitionEditionContext context in edgeInfo.transitions)
                {
                    if (context.transition != null)
                    {
                        list.Add(context.transition);
                    }
                    else
                    {
                        this.m_DefaultTransition = ScriptableObject.CreateInstance<AnimatorDefaultTransition>();
                        list.Add(this.m_DefaultTransition);
                    }
                }
            }
            if (list.Count > 0)
            {
                Selection.objects = list.ToArray();
            }
        }

        public AnimatorStateMachine activeStateMachine =>
            this.stateMachineGraph.activeStateMachine;

        public AnimatorDefaultTransition defaultTransition =>
            this.m_DefaultTransition;

        public override IEdgeGUI edgeGUI
        {
            get
            {
                if (base.m_EdgeGUI == null)
                {
                    UnityEditor.Graphs.AnimationStateMachine.EdgeGUI egui = new UnityEditor.Graphs.AnimationStateMachine.EdgeGUI {
                        host = this
                    };
                    base.m_EdgeGUI = egui;
                }
                return base.m_EdgeGUI;
            }
        }

        public AnimatorStateMachine hoveredStateMachine =>
            ((this.m_HoveredStateMachineNode == null) ? null : this.m_HoveredStateMachineNode.stateMachine);

        private bool isSelectionMoving =>
            ((base.selection.Count > 0) && base.selection[0].isDragging);

        public LiveLinkInfo liveLinkInfo =>
            this.m_LiveLinkInfo;

        public AnimatorStateMachine parentStateMachine =>
            this.stateMachineGraph.parentStateMachine;

        public AnimatorStateMachine rootStateMachine =>
            this.stateMachineGraph.rootStateMachine;

        public UnityEditor.Graphs.AnimationStateMachine.Graph stateMachineGraph =>
            (base.graph as UnityEditor.Graphs.AnimationStateMachine.Graph);

        public AnimatorControllerTool tool =>
            (base.m_Host as AnimatorControllerTool);

        [CompilerGenerated]
        private sealed class <OnGraphGUI>c__AnonStorey0
        {
            internal UnityEditor.Graphs.AnimationStateMachine.GraphGUI $this;
            internal UnityEditor.Graphs.AnimationStateMachine.Node n2;

            internal void <>m__0(int)
            {
                this.$this.NodeGUI(this.n2);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LiveLinkInfo
        {
            private AnimatorState m_CurrentState;
            private float m_CurrentStateNormalizedTime;
            private bool m_CurrentStateLoopTime;
            private AnimatorState m_NextState;
            private float m_NextStateNormalizedTime;
            private bool m_NextStateLoopTime;
            private AnimatorStateMachine m_CurrentStateMachine;
            private AnimatorStateMachine m_NextStateMachine;
            private AnimatorTransitionInfo m_TransitionInfo;
            public UnityEditor.Graphs.AnimationStateMachine.Node srcNode;
            public UnityEditor.Graphs.AnimationStateMachine.Node dstNode;
            public AnimatorState currentState
            {
                get => 
                    this.m_CurrentState;
                set
                {
                    this.m_CurrentState = value;
                }
            }
            public float currentStateNormalizedTime
            {
                get => 
                    this.m_CurrentStateNormalizedTime;
                set
                {
                    this.m_CurrentStateNormalizedTime = value;
                }
            }
            public bool currentStateLoopTime
            {
                get => 
                    this.m_CurrentStateLoopTime;
                set
                {
                    this.m_CurrentStateLoopTime = value;
                }
            }
            public AnimatorState nextState
            {
                get => 
                    this.m_NextState;
                set
                {
                    this.m_NextState = value;
                }
            }
            public float nextStateNormalizedTime
            {
                get => 
                    this.m_NextStateNormalizedTime;
                set
                {
                    this.m_NextStateNormalizedTime = value;
                }
            }
            public bool nextStateLoopTime
            {
                get => 
                    this.m_NextStateLoopTime;
                set
                {
                    this.m_NextStateLoopTime = value;
                }
            }
            public AnimatorStateMachine currentStateMachine
            {
                get => 
                    this.m_CurrentStateMachine;
                set
                {
                    this.m_CurrentStateMachine = value;
                }
            }
            public AnimatorStateMachine nextStateMachine
            {
                get => 
                    this.m_NextStateMachine;
                set
                {
                    this.m_NextStateMachine = value;
                }
            }
            public AnimatorTransitionInfo transitionInfo
            {
                get => 
                    this.m_TransitionInfo;
                set
                {
                    this.m_TransitionInfo = value;
                }
            }
            public void Clear()
            {
                this.m_CurrentState = null;
                this.m_NextState = null;
                this.m_CurrentStateNormalizedTime = 0f;
                this.m_NextStateNormalizedTime = 0f;
                this.m_TransitionInfo = new AnimatorTransitionInfo();
            }
        }
    }
}

