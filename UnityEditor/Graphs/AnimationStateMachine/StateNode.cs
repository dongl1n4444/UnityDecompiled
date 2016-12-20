namespace UnityEditor.Graphs.AnimationStateMachine
{
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEditor.Animations;
    using UnityEditor.Graphs;
    using UnityEngine;

    internal class StateNode : UnityEditor.Graphs.AnimationStateMachine.Node
    {
        public AnimatorState state;

        private void AddNewBlendTreeCallback()
        {
            BlendTree stateEffectiveMotion = AnimatorControllerTool.tool.animatorController.GetStateEffectiveMotion(this.state, AnimatorControllerTool.tool.selectedLayerIndex) as BlendTree;
            AnimatorStateMachine stateMachine = AnimatorControllerTool.tool.animatorController.layers[AnimatorControllerTool.tool.selectedLayerIndex].stateMachine;
            bool flag = true;
            if (stateEffectiveMotion != null)
            {
                string title = "This will delete current BlendTree in state.";
                string message = "You cannot undo this action.";
                if (EditorUtility.DisplayDialog(title, message, "Delete", "Cancel"))
                {
                    MecanimUtilities.DestroyBlendTreeRecursive(stateEffectiveMotion);
                }
                else
                {
                    flag = false;
                }
            }
            else
            {
                Undo.RegisterCompleteObjectUndo(stateMachine, "Blend Tree Added");
            }
            if (flag)
            {
                BlendTree objectToAdd = new BlendTree {
                    hideFlags = HideFlags.HideInHierarchy
                };
                if (AssetDatabase.GetAssetPath(AnimatorControllerTool.tool.animatorController) != "")
                {
                    AssetDatabase.AddObjectToAsset(objectToAdd, AssetDatabase.GetAssetPath(AnimatorControllerTool.tool.animatorController));
                }
                objectToAdd.name = "Blend Tree";
                string defaultBlendTreeParameter = AnimatorControllerTool.tool.animatorController.GetDefaultBlendTreeParameter();
                objectToAdd.blendParameterY = defaultBlendTreeParameter;
                objectToAdd.blendParameter = defaultBlendTreeParameter;
                AnimatorControllerTool.tool.animatorController.SetStateEffectiveMotion(this.state, objectToAdd, AnimatorControllerTool.tool.selectedLayerIndex);
            }
        }

        public override void Connect(UnityEditor.Graphs.AnimationStateMachine.Node toNode, UnityEditor.Graphs.Edge edge)
        {
            if (toNode is StateNode)
            {
                this.state.AddTransition((toNode as StateNode).state, true);
                base.graphGUI.stateMachineGraph.RebuildGraph();
            }
            else if (toNode is StateMachineNode)
            {
                UnityEditor.Graphs.AnimationStateMachine.Node.GenericMenuForStateMachineNode(toNode as StateMachineNode, true, delegate (object data) {
                    if (data is AnimatorState)
                    {
                        this.state.AddTransition(data as AnimatorState, true);
                    }
                    else
                    {
                        this.state.AddTransition(data as AnimatorStateMachine, true);
                    }
                    base.graphGUI.stateMachineGraph.RebuildGraph();
                });
            }
            else if (toNode is ExitNode)
            {
                this.state.AddExitTransition(true);
                base.graphGUI.stateMachineGraph.RebuildGraph();
            }
        }

        private void CopyStateCallback()
        {
            base.graphGUI.CopySelectionToPasteboard();
        }

        public void DeleteStateCallback()
        {
            if (!base.graphGUI.selection.Contains(this))
            {
                base.graphGUI.selection.Add(this);
            }
            base.graphGUI.DeleteSelection();
            AnimatorControllerTool.tool.RebuildGraph();
        }

        public override void EndDrag()
        {
            base.EndDrag();
            AnimatorStateMachine hoveredStateMachine = AnimatorControllerTool.tool.stateMachineGraphGUI.hoveredStateMachine;
            if (hoveredStateMachine != null)
            {
                Undo.RecordObjects(new List<UnityEngine.Object> { 
                    AnimatorControllerTool.tool.stateMachineGraph.activeStateMachine,
                    hoveredStateMachine
                }.ToArray(), "Move in StateMachine");
                AnimatorControllerTool.tool.stateMachineGraph.activeStateMachine.MoveState(this.state, hoveredStateMachine);
            }
        }

        private void MakeTransitionCallback()
        {
            base.graphGUI.edgeGUI.BeginSlotDragging(Enumerable.First<Slot>(base.outputSlots), true, false);
        }

        public override void NodeUI(UnityEditor.Graphs.GraphGUI host)
        {
            base.graphGUI = host as UnityEditor.Graphs.AnimationStateMachine.GraphGUI;
            Assert.NotNull(base.graphGUI);
            Event current = Event.current;
            if (UnityEditor.Graphs.AnimationStateMachine.Node.IsLeftClick())
            {
                host.edgeGUI.EndSlotDragging(Enumerable.First<Slot>(base.inputSlots), true);
            }
            if (UnityEditor.Graphs.AnimationStateMachine.Node.IsDoubleClick())
            {
                Motion stateEffectiveMotion = AnimatorControllerTool.tool.animatorController.GetStateEffectiveMotion(this.state, AnimatorControllerTool.tool.selectedLayerIndex);
                if (stateEffectiveMotion is BlendTree)
                {
                    base.graphGUI.tool.AddBreadCrumb(this.state);
                }
                else if (stateEffectiveMotion is AnimationClip)
                {
                    Selection.activeObject = stateEffectiveMotion;
                    AnimationClipEditor.EditWithImporter(stateEffectiveMotion as AnimationClip);
                }
                current.Use();
            }
            if (UnityEditor.Graphs.AnimationStateMachine.Node.IsRightClick())
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Make Transition"), false, new GenericMenu.MenuFunction(this.MakeTransitionCallback));
                if (base.graphGUI.rootStateMachine.defaultState == this.state)
                {
                    menu.AddDisabledItem(new GUIContent("Set as Layer Default State"));
                }
                else
                {
                    menu.AddItem(new GUIContent("Set as Layer Default State"), false, new GenericMenu.MenuFunction(this.SetDefaultCallback));
                }
                menu.AddItem(new GUIContent("Copy"), false, new GenericMenu.MenuFunction(this.CopyStateCallback));
                menu.AddItem(new GUIContent("Create new BlendTree in State"), false, new GenericMenu.MenuFunction(this.AddNewBlendTreeCallback));
                menu.AddItem(new GUIContent("Delete"), false, new GenericMenu.MenuFunction(this.DeleteStateCallback));
                menu.ShowAsContext();
                current.Use();
            }
            Rect rect = GUILayoutUtility.GetRect((float) 200f, (float) 10f);
            if ((Event.current.type == EventType.Repaint) && ((base.graphGUI.liveLinkInfo.currentState == this.state) || (base.graphGUI.liveLinkInfo.nextState == this.state)))
            {
                GUIStyle style = "MeLivePlayBackground";
                GUIStyle style2 = "MeLivePlayBar";
                float f = (base.graphGUI.liveLinkInfo.currentState != this.state) ? base.graphGUI.liveLinkInfo.nextStateNormalizedTime : base.graphGUI.liveLinkInfo.currentStateNormalizedTime;
                bool flag = (base.graphGUI.liveLinkInfo.currentState != this.state) ? base.graphGUI.liveLinkInfo.nextStateLoopTime : base.graphGUI.liveLinkInfo.currentStateLoopTime;
                rect = style.margin.Remove(rect);
                Rect position = style.padding.Remove(rect);
                if (flag)
                {
                    if (f < 0f)
                    {
                        position.width = (position.width * (1f - (Mathf.Abs(f) % 1f))) + 2f;
                    }
                    else
                    {
                        position.width = (position.width * (f % 1f)) + 2f;
                    }
                }
                else
                {
                    position.width = (position.width * Mathf.Clamp(f, 0f, 1f)) + 2f;
                }
                style2.Draw(position, false, false, false, false);
                style.Draw(rect, false, false, false, false);
            }
        }

        public override void OnDrag()
        {
            base.OnDrag();
            AnimatorControllerTool.tool.stateMachineGraph.activeStateMachine.SetStatePosition(this.state, (Vector3) new Vector2(this.position.x, this.position.y));
        }

        private void SetDefaultCallback()
        {
            Undo.RegisterCompleteObjectUndo(base.graphGUI.rootStateMachine, "Set Default State");
            base.graphGUI.rootStateMachine.defaultState = this.state;
            AnimatorControllerTool.tool.RebuildGraph();
        }

        public override UnityEngine.Object selectionObject
        {
            get
            {
                return this.state;
            }
        }

        public override string title
        {
            get
            {
                return this.state.name;
            }
            set
            {
            }
        }

        public override UnityEngine.Object undoableObject
        {
            get
            {
                return AnimatorControllerTool.tool.stateMachineGraph.activeStateMachine;
            }
        }
    }
}

