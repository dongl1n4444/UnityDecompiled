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

    internal class StateMachineNode : UnityEditor.Graphs.AnimationStateMachine.Node
    {
        public AnimatorStateMachine stateMachine;

        public override void Connect(UnityEditor.Graphs.AnimationStateMachine.Node toNode, UnityEditor.Graphs.Edge edge)
        {
            if (toNode is StateNode)
            {
                base.graphGUI.stateMachineGraph.activeStateMachine.AddStateMachineTransition(this.stateMachine, (toNode as StateNode).state);
                base.graphGUI.stateMachineGraph.RebuildGraph();
            }
            else if (toNode is StateMachineNode)
            {
                UnityEditor.Graphs.AnimationStateMachine.Node.GenericMenuForStateMachineNode(toNode as StateMachineNode, true, delegate (object data) {
                    if (data is AnimatorState)
                    {
                        base.graphGUI.stateMachineGraph.activeStateMachine.AddStateMachineTransition(this.stateMachine, data as AnimatorState);
                    }
                    else
                    {
                        base.graphGUI.stateMachineGraph.activeStateMachine.AddStateMachineTransition(this.stateMachine, data as AnimatorStateMachine);
                    }
                    base.graphGUI.stateMachineGraph.RebuildGraph();
                });
            }
            else if (toNode is ExitNode)
            {
                base.graphGUI.stateMachineGraph.activeStateMachine.AddStateMachineExitTransition(this.stateMachine);
                base.graphGUI.stateMachineGraph.RebuildGraph();
            }
        }

        private void CopyStateMachineCallback()
        {
            base.graphGUI.CopySelectionToPasteboard();
        }

        public void DeleteStateMachineCallback()
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
            if (((hoveredStateMachine != null) && (hoveredStateMachine != this.stateMachine)) && (this.stateMachine != base.graphGUI.stateMachineGraph.parentStateMachine))
            {
                Undo.RecordObjects(new List<UnityEngine.Object> { 
                    AnimatorControllerTool.tool.stateMachineGraph.activeStateMachine,
                    hoveredStateMachine
                }.ToArray(), "Move in StateMachine");
                AnimatorControllerTool.tool.stateMachineGraph.activeStateMachine.MoveStateMachine(this.stateMachine, hoveredStateMachine);
                AnimatorControllerTool.tool.RebuildGraph();
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
            if (UnityEditor.Graphs.AnimationStateMachine.Node.IsLeftClick())
            {
                host.edgeGUI.EndSlotDragging(Enumerable.First<Slot>(base.inputSlots), true);
            }
            if (UnityEditor.Graphs.AnimationStateMachine.Node.IsDoubleClick())
            {
                if (this.stateMachine == base.graphGUI.stateMachineGraph.parentStateMachine)
                {
                    base.graphGUI.tool.GoToBreadCrumbTarget(this.stateMachine);
                }
                else
                {
                    base.graphGUI.tool.AddBreadCrumb(this.stateMachine);
                }
                Event.current.Use();
            }
            if (UnityEditor.Graphs.AnimationStateMachine.Node.IsRightClick() && (this.stateMachine != base.graphGUI.stateMachineGraph.parentStateMachine))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Make Transition"), false, new GenericMenu.MenuFunction(this.MakeTransitionCallback));
                menu.AddItem(new GUIContent("Copy"), false, new GenericMenu.MenuFunction(this.CopyStateMachineCallback));
                menu.AddItem(new GUIContent("Delete"), false, new GenericMenu.MenuFunction(this.DeleteStateMachineCallback));
                menu.ShowAsContext();
                Event.current.Use();
            }
        }

        public override void OnDrag()
        {
            base.OnDrag();
            if (this.stateMachine == AnimatorControllerTool.tool.stateMachineGraph.parentStateMachine)
            {
                AnimatorControllerTool.tool.stateMachineGraph.activeStateMachine.parentStateMachinePosition = (Vector3) new Vector2(this.position.x, this.position.y);
            }
            else
            {
                AnimatorControllerTool.tool.stateMachineGraph.activeStateMachine.SetStateMachinePosition(this.stateMachine, (Vector3) new Vector2(this.position.x, this.position.y));
            }
        }

        public override UnityEngine.Object selectionObject
        {
            get
            {
                return this.stateMachine;
            }
        }

        public override string title
        {
            get
            {
                return (base.title + this.stateMachine.name);
            }
            set
            {
                base.title = value;
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

