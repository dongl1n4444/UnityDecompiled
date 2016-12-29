namespace UnityEditor.Graphs.AnimationStateMachine
{
    using NUnit.Framework;
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditor.Animations;
    using UnityEditor.Graphs;
    using UnityEngine;

    internal class EntryNode : UnityEditor.Graphs.AnimationStateMachine.Node
    {
        private bool draggingDefaultState = false;
        private AnimatorStateMachine m_StateMachine = null;

        public override void Connect(UnityEditor.Graphs.AnimationStateMachine.Node toNode, UnityEditor.Graphs.Edge edge)
        {
            if (toNode is StateNode)
            {
                if (this.draggingDefaultState)
                {
                    this.m_StateMachine.defaultState = (toNode as StateNode).state;
                }
                else
                {
                    this.m_StateMachine.AddEntryTransition((toNode as StateNode).state);
                }
                base.graphGUI.stateMachineGraph.RebuildGraph();
            }
            else if (toNode is StateMachineNode)
            {
                <Connect>c__AnonStorey0 storey = new <Connect>c__AnonStorey0 {
                    $this = this,
                    isDragginDefaultState = this.draggingDefaultState
                };
                UnityEditor.Graphs.AnimationStateMachine.Node.GenericMenuForStateMachineNode(toNode as StateMachineNode, !this.draggingDefaultState, new GenericMenu.MenuFunction2(storey.<>m__0));
            }
            this.draggingDefaultState = false;
        }

        private void MakeDefaultStateCallback()
        {
            this.draggingDefaultState = true;
            base.graphGUI.edgeGUI.BeginSlotDragging(base.outputSlots.First<Slot>(), true, false);
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
                menu.AddItem(new GUIContent("Set StateMachine Default State"), false, new GenericMenu.MenuFunction(this.MakeDefaultStateCallback));
                menu.ShowAsContext();
                current.Use();
            }
            if (UnityEditor.Graphs.AnimationStateMachine.Node.IsLeftClick())
            {
                host.edgeGUI.EndSlotDragging(base.inputSlots.First<Slot>(), true);
            }
        }

        public override void OnDrag()
        {
            base.OnDrag();
            this.m_StateMachine.entryPosition = (Vector3) new Vector2(this.position.x, this.position.y);
        }

        public override UnityEngine.Object selectionObject =>
            this;

        public AnimatorStateMachine stateMachine
        {
            get => 
                this.m_StateMachine;
            set
            {
                this.m_StateMachine = value;
            }
        }

        public override UnityEngine.Object undoableObject =>
            this.m_StateMachine;

        [CompilerGenerated]
        private sealed class <Connect>c__AnonStorey0
        {
            internal EntryNode $this;
            internal bool isDragginDefaultState;

            internal void <>m__0(object data)
            {
                if (data is AnimatorState)
                {
                    if (this.isDragginDefaultState)
                    {
                        this.$this.m_StateMachine.defaultState = data as AnimatorState;
                    }
                    else
                    {
                        this.$this.m_StateMachine.AddEntryTransition(data as AnimatorState);
                    }
                }
                else if (data is AnimatorStateMachine)
                {
                    this.$this.m_StateMachine.AddEntryTransition(data as AnimatorStateMachine);
                }
                this.$this.graphGUI.stateMachineGraph.RebuildGraph();
            }
        }
    }
}

