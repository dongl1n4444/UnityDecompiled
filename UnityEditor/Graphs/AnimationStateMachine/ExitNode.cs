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
            base.graphGUI.edgeGUI.BeginSlotDragging(base.outputSlots.First<Slot>(), true, false);
        }

        public override void NodeUI(UnityEditor.Graphs.GraphGUI host)
        {
            if (UnityEditor.Graphs.AnimationStateMachine.Node.IsLeftClick())
            {
                host.edgeGUI.EndSlotDragging(base.inputSlots.First<Slot>(), true);
            }
        }

        public override void OnDrag()
        {
            base.OnDrag();
            AnimatorControllerTool.tool.stateMachineGraph.activeStateMachine.exitPosition = (Vector3) new Vector2(this.position.x, this.position.y);
        }

        public override UnityEngine.Object selectionObject =>
            this;

        public override UnityEngine.Object undoableObject =>
            AnimatorControllerTool.tool.stateMachineGraph.activeStateMachine;
    }
}

