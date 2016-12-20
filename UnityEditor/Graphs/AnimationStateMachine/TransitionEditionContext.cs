namespace UnityEditor.Graphs.AnimationStateMachine
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEditor.Animations;
    using UnityEditor.Graphs;

    internal class TransitionEditionContext
    {
        private string m_DisplayName;
        private string m_FullName;
        public AnimatorStateMachine ownerStateMachine;
        public AnimatorState sourceState;
        public AnimatorStateMachine sourceStateMachine;
        public AnimatorTransitionBase transition;

        public TransitionEditionContext(AnimatorTransitionBase aTransition, AnimatorState aSourceState, AnimatorStateMachine aSourceStateMachine, AnimatorStateMachine aOwnerStateMachine)
        {
            this.transition = aTransition;
            this.sourceState = aSourceState;
            this.sourceStateMachine = aSourceStateMachine;
            this.ownerStateMachine = aOwnerStateMachine;
            this.BuildNames();
        }

        private void BuildNames()
        {
            if (this.sourceState != null)
            {
                this.m_DisplayName = this.transition.GetDisplayName(this.sourceState);
            }
            else if (this.sourceStateMachine != null)
            {
                this.m_DisplayName = this.transition.GetDisplayName(this.sourceStateMachine);
            }
            else if (this.transition != null)
            {
                this.m_DisplayName = this.transition.GetDisplayName(null);
            }
            else
            {
                this.m_DisplayName = "To Default State";
            }
            this.m_FullName = "";
            if (((AnimatorControllerTool.tool != null) && (AnimatorControllerTool.tool.stateMachineGraph != null)) && ((AnimatorControllerTool.tool.stateMachineGraph.rootStateMachine != null) && (this.transition != null)))
            {
                string source = !this.isAnyStateTransition ? "" : "AnyState";
                string destination = "";
                UnityEditor.Graphs.AnimationStateMachine.Graph stateMachineGraph = AnimatorControllerTool.tool.stateMachineGraph;
                if (this.sourceState != null)
                {
                    source = stateMachineGraph.GetStatePath(this.sourceState);
                }
                else if (this.sourceStateMachine != null)
                {
                    source = stateMachineGraph.GetStateMachinePath(this.sourceStateMachine);
                }
                if (this.transition.destinationState != null)
                {
                    destination = destination + stateMachineGraph.GetStatePath(this.transition.destinationState);
                }
                else if (this.transition.destinationStateMachine != null)
                {
                    destination = destination + stateMachineGraph.GetStateMachinePath(this.transition.destinationStateMachine);
                }
                this.m_FullName = AnimatorTransitionBase.BuildTransitionName(source, destination);
            }
        }

        public void Remove([Optional, DefaultParameterValue(true)] bool rebuildGraph)
        {
            if (this.sourceState != null)
            {
                this.sourceState.RemoveTransition(this.transition as AnimatorStateTransition);
            }
            else if (this.ownerStateMachine != null)
            {
                if (this.transition is AnimatorStateTransition)
                {
                    this.ownerStateMachine.RemoveAnyStateTransition(this.transition as AnimatorStateTransition);
                }
                else if (this.sourceStateMachine != null)
                {
                    this.ownerStateMachine.RemoveStateMachineTransition(this.sourceStateMachine, this.transition as AnimatorTransition);
                }
                else
                {
                    this.ownerStateMachine.RemoveEntryTransition(this.transition as AnimatorTransition);
                }
            }
            if (rebuildGraph)
            {
                AnimatorControllerTool.tool.RebuildGraph();
            }
        }

        public string displayName
        {
            get
            {
                return this.m_DisplayName;
            }
        }

        public string fullName
        {
            get
            {
                return this.m_FullName;
            }
        }

        public bool isAnyStateTransition
        {
            get
            {
                return ((this.ownerStateMachine != null) && (this.transition is AnimatorStateTransition));
            }
        }

        public bool isDefaultTransition
        {
            get
            {
                return (((this.transition == null) && (this.sourceState == null)) && (this.sourceStateMachine == null));
            }
        }
    }
}

