namespace UnityEditor.Graphs.AnimationBlendTree
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditor.Animations;
    using UnityEditor.Graphs;
    using UnityEditorInternal;
    using UnityEngine;

    internal class GraphGUI : UnityEditor.Graphs.GraphGUI
    {
        private float[] m_Weights = new float[0];

        public override void ClearSelection()
        {
            base.selection.Clear();
            this.edgeGUI.edgeSelection.Clear();
        }

        private List<string> CollectSelectionNames()
        {
            List<string> list = new List<string>();
            foreach (UnityEditor.Graphs.AnimationBlendTree.Node node in base.selection)
            {
                list.Add(node.motion.name);
            }
            return list;
        }

        private void CreateBlendTreeCallback(object obj)
        {
            UnityEditor.Animations.BlendTree tree = obj as UnityEditor.Animations.BlendTree;
            if (tree != null)
            {
                UnityEditor.Animations.BlendTree tree2 = tree.CreateBlendTreeChild((float) 0f);
                if ((this.m_Tool != null) && (this.m_Tool.animatorController != null))
                {
                    string defaultBlendTreeParameter = this.m_Tool.animatorController.GetDefaultBlendTreeParameter();
                    tree2.blendParameterY = defaultBlendTreeParameter;
                    tree2.blendParameter = defaultBlendTreeParameter;
                    tree.SetDirectBlendTreeParameter(tree.children.Length - 1, this.m_Tool.animatorController.GetDefaultBlendTreeParameter());
                }
                else
                {
                    tree2.blendParameter = tree.blendParameter;
                    tree2.blendParameterY = tree.blendParameterY;
                }
            }
        }

        private void CreateMotionCallback(object obj)
        {
            UnityEditor.Animations.BlendTree tree = obj as UnityEditor.Animations.BlendTree;
            if (tree != null)
            {
                tree.AddChild(null);
                tree.SetDirectBlendTreeParameter(tree.children.Length - 1, this.m_Tool.animatorController.GetDefaultBlendTreeParameter());
            }
        }

        private void DeleteNodeCallback(object obj)
        {
            UnityEditor.Graphs.AnimationBlendTree.Node node = obj as UnityEditor.Graphs.AnimationBlendTree.Node;
            if (node != null)
            {
                string[] toDelete = new string[] { node.motion.name };
                if (DeleteNodeDialog(toDelete))
                {
                    UnityEditor.Graphs.AnimationBlendTree.Node[] nodes = new UnityEditor.Graphs.AnimationBlendTree.Node[] { node };
                    this.blendTreeGraph.RemoveNodeMotions(nodes);
                    this.blendTreeGraph.BuildFromBlendTree(this.blendTreeGraph.rootBlendTree);
                }
            }
        }

        public static bool DeleteNodeDialog(string[] toDelete)
        {
            string title = "Delete selected Blend Tree asset";
            if (toDelete.Length > 1)
            {
                title = title + "s";
            }
            title = title + "?";
            string message = "";
            foreach (string str3 in toDelete)
            {
                message = message + str3 + "\n";
            }
            return EditorUtility.DisplayDialog(title, message, "Delete", "Cancel");
        }

        private void DeleteSelection()
        {
            List<string> list = this.CollectSelectionNames();
            if (list.Count != 0)
            {
                if (DeleteNodeDialog(list.ToArray()))
                {
                    this.blendTreeGraph.RemoveNodeMotions(base.selection);
                    this.blendTreeGraph.BuildFromBlendTree(this.blendTreeGraph.rootBlendTree);
                }
                this.ClearSelection();
                this.UpdateUnitySelection();
            }
        }

        public override void DoBackgroundClickAction()
        {
            base.selection.Clear();
            UnityEditor.Graphs.AnimationBlendTree.Node rootNode = (base.graph as UnityEditor.Graphs.AnimationBlendTree.Graph).rootNode;
            base.selection.Add(rootNode);
            Selection.activeObject = rootNode.motion;
        }

        private void HandleGraphInput()
        {
            Event current = Event.current;
            switch (current.type)
            {
                case EventType.ValidateCommand:
                    if ((current.commandName == "SoftDelete") || (current.commandName == "Delete"))
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
                    break;

                case EventType.MouseDown:
                    if ((((current.button == 0) && !current.alt) && (Event.current.clickCount == 1)) && ((Application.platform != RuntimePlatform.OSXEditor) || !current.control))
                    {
                        this.DoBackgroundClickAction();
                        current.Use();
                    }
                    break;

                case EventType.KeyDown:
                    if (current.keyCode == KeyCode.Delete)
                    {
                        this.DeleteSelection();
                        current.Use();
                    }
                    break;
            }
        }

        private void HandleNodeInput(UnityEditor.Graphs.AnimationBlendTree.Node node)
        {
            Event current = Event.current;
            if ((current.type == EventType.MouseDown) && (current.button == 0))
            {
                base.selection.Clear();
                base.selection.Add(node);
                Selection.activeObject = node.motion;
                UnityEditor.Graphs.AnimationBlendTree.Node rootNode = this.blendTreeGraph.rootNode;
                if (((current.clickCount == 2) && (node.motion is UnityEditor.Animations.BlendTree)) && (rootNode != node.motion))
                {
                    base.selection.Clear();
                    Stack<UnityEditor.Graphs.AnimationBlendTree.Node> stack = new Stack<UnityEditor.Graphs.AnimationBlendTree.Node>();
                    for (UnityEditor.Graphs.AnimationBlendTree.Node node3 = node; (node3.motion is UnityEditor.Animations.BlendTree) && (node3 != rootNode); node3 = node3.parent)
                    {
                        stack.Push(node3);
                    }
                    foreach (UnityEditor.Graphs.AnimationBlendTree.Node node4 in stack)
                    {
                        this.m_Tool.AddBreadCrumb(node4.motion, false);
                    }
                    this.m_Tool.CenterView();
                }
                current.Use();
            }
            if ((current.type == EventType.MouseDown) && (current.button == 1))
            {
                GenericMenu menu = new GenericMenu();
                UnityEditor.Animations.BlendTree motion = node.motion as UnityEditor.Animations.BlendTree;
                if (motion != null)
                {
                    menu.AddItem(new GUIContent("Add Motion"), false, new GenericMenu.MenuFunction2(this.CreateMotionCallback), motion);
                    menu.AddItem(new GUIContent("Add Blend Tree"), false, new GenericMenu.MenuFunction2(this.CreateBlendTreeCallback), motion);
                }
                if (node != this.blendTreeGraph.rootNode)
                {
                    menu.AddItem(new GUIContent("Delete"), false, new GenericMenu.MenuFunction2(this.DeleteNodeCallback), node);
                }
                menu.ShowAsContext();
                Event.current.Use();
            }
        }

        private string LimitStringWidth(string content, float width, GUIStyle style)
        {
            int numCharactersThatFitWithinWidth = style.GetNumCharactersThatFitWithinWidth(content, width);
            if (content.Length > numCharactersThatFitWithinWidth)
            {
                return (content.Substring(0, Mathf.Min(numCharactersThatFitWithinWidth - 3, content.Length)) + "...");
            }
            return content;
        }

        public override void NodeGUI(UnityEditor.Graphs.Node n)
        {
            UnityEditor.Graphs.AnimationBlendTree.Node node = n as UnityEditor.Graphs.AnimationBlendTree.Node;
            UnityEditor.Animations.BlendTree motion = node.motion as UnityEditor.Animations.BlendTree;
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(200f) };
            GUILayout.BeginVertical(options);
            foreach (Slot slot in n.inputSlots)
            {
                base.LayoutSlot(slot, this.LimitStringWidth(slot.title, 180f, UnityEditor.Graphs.Styles.varPinIn), false, false, false, UnityEditor.Graphs.Styles.varPinIn);
            }
            foreach (Slot slot2 in n.outputSlots)
            {
                base.LayoutSlot(slot2, this.LimitStringWidth(slot2.title, 180f, UnityEditor.Graphs.Styles.varPinOut), false, false, false, UnityEditor.Graphs.Styles.varPinOut);
            }
            n.NodeUI(this);
            EditorGUIUtility.labelWidth = 50f;
            if (motion != null)
            {
                if (motion.recursiveBlendParameterCount > 0)
                {
                    for (int i = 0; i < motion.recursiveBlendParameterCount; i++)
                    {
                        string recursiveBlendParameter = motion.GetRecursiveBlendParameter(i);
                        float recursiveBlendParameterMin = motion.GetRecursiveBlendParameterMin(i);
                        float recursiveBlendParameterMax = motion.GetRecursiveBlendParameterMax(i);
                        EventType type = Event.current.type;
                        if ((Event.current.button != 0) && Event.current.isMouse)
                        {
                            Event.current.type = EventType.Ignore;
                        }
                        if (Mathf.Approximately(recursiveBlendParameterMax, recursiveBlendParameterMin))
                        {
                            recursiveBlendParameterMax = recursiveBlendParameterMin + 1f;
                        }
                        EditorGUI.BeginChangeCheck();
                        float parameterValue = EditorGUILayout.Slider(GUIContent.Temp(recursiveBlendParameter, recursiveBlendParameter), this.blendTreeGraph.GetParameterValue(recursiveBlendParameter), recursiveBlendParameterMin, recursiveBlendParameterMax, new GUILayoutOption[0]);
                        if (EditorGUI.EndChangeCheck())
                        {
                            this.blendTreeGraph.SetParameterValue(recursiveBlendParameter, parameterValue);
                            InspectorWindow.RepaintAllInspectors();
                        }
                        if (Event.current.button != 0)
                        {
                            Event.current.type = type;
                        }
                    }
                }
                else
                {
                    EditorGUILayout.LabelField("No blend parameter to display", new GUILayoutOption[0]);
                }
                if (node.animator != null)
                {
                    List<UnityEditor.Graphs.Edge> list = new List<UnityEditor.Graphs.Edge>(n.outputEdges);
                    node.UpdateAnimator();
                    if (this.m_Weights.Length != list.Count)
                    {
                        this.m_Weights = new float[list.Count];
                    }
                    BlendTreePreviewUtility.GetRootBlendTreeChildWeights(node.animator, 0, node.animator.GetCurrentAnimatorStateInfo(0).fullPathHash, this.m_Weights);
                    for (int j = 0; j < list.Count; j++)
                    {
                        UnityEditor.Graphs.AnimationBlendTree.Node node2 = list[j].toSlot.node as UnityEditor.Graphs.AnimationBlendTree.Node;
                        node2.weight = node.weight * this.m_Weights[j];
                        list[j].color = node2.weightEdgeColor;
                    }
                }
            }
            GUILayout.EndVertical();
            this.HandleNodeInput(n as UnityEditor.Graphs.AnimationBlendTree.Node);
        }

        public override void OnGraphGUI()
        {
            this.blendTreeGraph.PopulateParameterValues();
            bool flag = false;
            foreach (UnityEditor.Graphs.AnimationBlendTree.Node node in base.graph.nodes)
            {
                if (node.controllerDirty)
                {
                    flag = true;
                }
            }
            if (flag)
            {
                this.blendTreeGraph.BuildFromBlendTree(this.blendTreeGraph.rootBlendTree);
            }
            base.m_Host.BeginWindows();
            foreach (UnityEditor.Graphs.AnimationBlendTree.Node node2 in base.graph.nodes)
            {
                <OnGraphGUI>c__AnonStorey0 storey = new <OnGraphGUI>c__AnonStorey0 {
                    $this = this,
                    n2 = node2
                };
                bool on = base.selection.Contains(node2);
                UnityEngine.Color color = GUI.color;
                GUI.color = !on ? node2.weightColor : UnityEngine.Color.white;
                GUIStyle style = UnityEditor.Graphs.Styles.GetNodeStyle(node2.style, node2.color, on);
                float x = Mathf.Round(node2.position.x);
                Rect screenRect = new Rect(x, Mathf.Round(node2.position.y), 0f, 0f);
                node2.position = GUILayout.Window(node2.GetInstanceID(), screenRect, new GUI.WindowFunction(storey.<>m__0), this.LimitStringWidth(node2.title, 180f, style), style, new GUILayoutOption[0]);
                GUI.color = color;
            }
            base.m_Host.EndWindows();
            this.edgeGUI.DoEdges();
            this.HandleGraphInput();
        }

        public override void SyncGraphToUnitySelection()
        {
            if (GUIUtility.hotControl == 0)
            {
                base.selection.Clear();
                foreach (UnityEngine.Object obj2 in Selection.objects)
                {
                    UnityEditor.Graphs.AnimationBlendTree.Node item = null;
                    Motion motion = obj2 as Motion;
                    if (motion != null)
                    {
                        item = this.blendTreeGraph.FindNode(motion);
                    }
                    if (item != null)
                    {
                        base.selection.Add(item);
                    }
                }
            }
        }

        public UnityEditor.Graphs.AnimationBlendTree.Graph blendTreeGraph =>
            (base.graph as UnityEditor.Graphs.AnimationBlendTree.Graph);

        private AnimatorControllerTool m_Tool =>
            (base.m_Host as AnimatorControllerTool);

        [CompilerGenerated]
        private sealed class <OnGraphGUI>c__AnonStorey0
        {
            internal UnityEditor.Graphs.AnimationBlendTree.GraphGUI $this;
            internal UnityEditor.Graphs.AnimationBlendTree.Node n2;

            internal void <>m__0(int)
            {
                this.$this.NodeGUI(this.n2);
            }
        }
    }
}

