namespace UnityEditor.Graphs.AnimationStateMachine
{
    using System;
    using System.Linq;
    using UnityEditor.Graphs;
    using UnityEngine;

    internal class ExitNode : UnityEditor.Graphs.AnimationStateMachine.Node
    {
        private void MakeTransitionCallback()
        {
            base.graphGUI.edgeGUI.BeginSlotDragging(Enumerable.First<Slot>(base.outputSlots), true, false);
        }

        public override void NodeUI(UnityEditor.Graphs.GraphGUI host)
        {
            if (UnityEditor.Graphs.AnimationStateMachine.Node.IsLeftClick())
            {
                host.edgeGUI.EndSlotDragging(Enumerable.First<Slot>(base.inputSlots), true);
            }
        }

        public override void OnDrag()
        {
            base.OnDrag();
            AnimatorControllerTool.tool.stateMachineGraph.activeStateMachine.exitPosition = (Vector3) new Vector2(this.position.x, this.position.y);
        }

        public override UnityEngine.Object selectionObject
        {
            get
            {
                return this;
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

