namespace UnityEditor.Graphs.AnimationStateMachine
{
    using NUnit.Framework;
    using System;
    using System.Linq;
    using UnityEditor;
    using UnityEditor.Animations;
    using UnityEditor.Graphs;
    using UnityEngine;

    internal class AnyStateNode : UnityEditor.Graphs.AnimationStateMachine.Node
    {
        public override void Connect(UnityEditor.Graphs.AnimationStateMachine.Node toNode, UnityEditor.Graphs.Edge edge)
        {
            if (toNode is StateNode)
            {
                base.graphGUI.stateMachineGraph.rootStateMachine.AddAnyStateTransition((toNode as StateNode).state);
                base.graphGUI.stateMachineGraph.RebuildGraph();
            }
            if (toNode is StateMachineNode)
            {
                StateMachineNode node = toNode as StateMachineNode;
                if (node.stateMachine != base.graphGUI.parentStateMachine)
                {
                    UnityEditor.Graphs.AnimationStateMachine.Node.GenericMenuForStateMachineNode(toNode as StateMachineNode, true, delegate (object data) {
                        if (data is AnimatorState)
                        {
                            base.graphGUI.stateMachineGraph.rootStateMachine.AddAnyStateTransition(data as AnimatorState);
                        }
                        else if (data is AnimatorStateMachine)
                        {
                            base.graphGUI.stateMachineGraph.rootStateMachine.AddAnyStateTransition(data as AnimatorStateMachine);
                        }
                        base.graphGUI.stateMachineGraph.RebuildGraph();
                    });
                }
            }
            if (toNode is EntryNode)
            {
                base.graphGUI.stateMachineGraph.rootStateMachine.AddAnyStateTransition(base.graphGUI.activeStateMachine);
            }
        }

        private void MakeTransitionCallback()
        {
            base.graphGUI.edgeGUI.BeginSlotDragging(base.outputSlots.First<Slot>(), true, false);
        }

        public override void NodeUI(UnityEditor.Graphs.GraphGUI host)
        {
            base.graphGUI = host as UnityEditor.Graphs.AnimationStateMachine.GraphGUI;
            Assert.NotNull(base.graphGUI);
            Event current = Event.current;
            if (UnityEditor.Graphs.AnimationStateMachine.Node.IsRightClick())
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Make Transition"), false, new GenericMenu.MenuFunction(this.MakeTransitionCallback));
                menu.ShowAsContext();
                current.Use();
            }
        }

        public override void OnDrag()
        {
            base.OnDrag();
            AnimatorControllerTool.tool.stateMachineGraph.activeStateMachine.anyStatePosition = (Vector3) new Vector2(this.position.x, this.position.y);
        }

        public override UnityEngine.Object selectionObject =>
            this;

        public override UnityEngine.Object undoableObject =>
            AnimatorControllerTool.tool.stateMachineGraph.rootStateMachine;
    }
}

