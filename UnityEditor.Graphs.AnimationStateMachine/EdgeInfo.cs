using System;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

namespace UnityEditor.Graphs.AnimationStateMachine
{
	internal class EdgeInfo
	{
		public readonly List<TransitionEditionContext> transitions = new List<TransitionEditionContext>();

		public bool hasMultipleTransitions
		{
			get
			{
				return this.transitions.Count > 1;
			}
		}

		public bool hasDefaultState
		{
			get
			{
				bool result;
				foreach (TransitionEditionContext current in this.transitions)
				{
					if (current.transition == null)
					{
						result = true;
						return result;
					}
				}
				result = false;
				return result;
			}
		}

		public EdgeType edgeType
		{
			get
			{
				int num = 0;
				int num2 = 0;
				foreach (TransitionEditionContext current in this.transitions)
				{
					if (current.transition is AnimatorStateTransition)
					{
						num++;
					}
					else
					{
						num2++;
					}
				}
				EdgeType result;
				if (num2 > 0 && num > 0)
				{
					result = EdgeType.MixedTransition;
				}
				else if (num > 0)
				{
					result = EdgeType.StateTransition;
				}
				else
				{
					result = EdgeType.Transition;
				}
				return result;
			}
		}

		public EdgeDebugState debugState
		{
			get
			{
				int num = 0;
				int num2 = 0;
				foreach (TransitionEditionContext current in this.transitions)
				{
					if (current.transition != null)
					{
						if (current.transition.mute)
						{
							num++;
						}
						else if (current.transition.solo)
						{
							num2++;
						}
					}
				}
				EdgeDebugState result;
				if (num == this.transitions.Count)
				{
					result = EdgeDebugState.MuteAll;
				}
				else if (num2 == this.transitions.Count)
				{
					result = EdgeDebugState.SoloAll;
				}
				else if (num > 0 && num2 > 0)
				{
					result = EdgeDebugState.MuteAndSolo;
				}
				else if (num > 0)
				{
					result = EdgeDebugState.MuteSome;
				}
				else if (num2 > 0)
				{
					result = EdgeDebugState.SoloSome;
				}
				else
				{
					result = EdgeDebugState.Normal;
				}
				return result;
			}
		}

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
			bool result;
			foreach (TransitionEditionContext current in this.transitions)
			{
				if (Animator.StringToHash(current.fullName) == nameHash)
				{
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}

		public bool HasTransition(AnimatorTransitionBase transition)
		{
			bool result;
			foreach (TransitionEditionContext current in this.transitions)
			{
				if (current.transition == transition)
				{
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}
	}
}
