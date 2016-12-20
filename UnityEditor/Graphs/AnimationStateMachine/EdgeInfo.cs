namespace UnityEditor.Graphs.AnimationStateMachine
{
    using System;
    using System.Collections.Generic;
    using UnityEditor.Animations;
    using UnityEngine;

    internal class EdgeInfo
    {
        public readonly List<TransitionEditionContext> transitions = new List<TransitionEditionContext>();

        public EdgeInfo(TransitionEditionContext context)
        {
            this.Add(context);
        }

        public void Add(TransitionEditionContext context)
        {
            this.transitions.Add(context);
        }

        public bool HasTransition(int nameHash)
        {
            foreach (TransitionEditionContext context in this.transitions)
            {
                if (Animator.StringToHash(context.fullName) == nameHash)
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasTransition(AnimatorTransitionBase transition)
        {
            foreach (TransitionEditionContext context in this.transitions)
            {
                if (context.transition == transition)
                {
                    return true;
                }
            }
            return false;
        }

        public EdgeDebugState debugState
        {
            get
            {
                int num = 0;
                int num2 = 0;
                foreach (TransitionEditionContext context in this.transitions)
                {
                    if (context.transition != null)
                    {
                        if (context.transition.mute)
                        {
                            num++;
                        }
                        else if (context.transition.solo)
                        {
                            num2++;
                        }
                    }
                }
                if (num == this.transitions.Count)
                {
                    return EdgeDebugState.MuteAll;
                }
                if (num2 == this.transitions.Count)
                {
                    return EdgeDebugState.SoloAll;
                }
                if ((num > 0) && (num2 > 0))
                {
                    return EdgeDebugState.MuteAndSolo;
                }
                if (num > 0)
                {
                    return EdgeDebugState.MuteSome;
                }
                if (num2 > 0)
                {
                    return EdgeDebugState.SoloSome;
                }
                return EdgeDebugState.Normal;
            }
        }

        public EdgeType edgeType
        {
            get
            {
                int num = 0;
                int num2 = 0;
                foreach (TransitionEditionContext context in this.transitions)
                {
                    if (context.transition is AnimatorStateTransition)
                    {
                        num++;
                    }
                    else
                    {
                        num2++;
                    }
                }
                if ((num2 > 0) && (num > 0))
                {
                    return EdgeType.MixedTransition;
                }
                if (num > 0)
                {
                    return EdgeType.StateTransition;
                }
                return EdgeType.Transition;
            }
        }

        public bool hasDefaultState
        {
            get
            {
                foreach (TransitionEditionContext context in this.transitions)
                {
                    if (context.transition == null)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public bool hasMultipleTransitions
        {
            get
            {
                return (this.transitions.Count > 1);
            }
        }
    }
}

