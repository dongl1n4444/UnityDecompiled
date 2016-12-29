namespace UnityEditor.Animations
{
    using System;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// <para>The Animation Layer contains a state machine that controls animations of a model or part of it.</para>
    /// </summary>
    public sealed class AnimatorControllerLayer
    {
        private AvatarMask m_AvatarMask;
        private StateBehavioursPair[] m_Behaviours;
        private AnimatorLayerBlendingMode m_BlendingMode;
        private float m_DefaultWeight;
        private bool m_IKPass;
        private StateMotionPair[] m_Motions;
        private string m_Name;
        private AnimatorStateMachine m_StateMachine;
        private bool m_SyncedLayerAffectsTiming;
        private int m_SyncedLayerIndex = -1;

        /// <summary>
        /// <para>Gets the override behaviour list for the state on the given layer.</para>
        /// </summary>
        /// <param name="state">The state which we want to get the behaviour list.</param>
        public StateMachineBehaviour[] GetOverrideBehaviours(AnimatorState state)
        {
            if (this.m_Behaviours != null)
            {
                foreach (StateBehavioursPair pair in this.m_Behaviours)
                {
                    if (pair.m_State == state)
                    {
                        return pair.m_Behaviours;
                    }
                }
            }
            return new StateMachineBehaviour[0];
        }

        /// <summary>
        /// <para>Gets the override motion for the state on the given layer.</para>
        /// </summary>
        /// <param name="state">The state which we want to get the motion.</param>
        public Motion GetOverrideMotion(AnimatorState state)
        {
            if (this.m_Motions != null)
            {
                foreach (StateMotionPair pair in this.m_Motions)
                {
                    if (pair.m_State == state)
                    {
                        return pair.m_Motion;
                    }
                }
            }
            return null;
        }

        public void SetOverrideBehaviours(AnimatorState state, StateMachineBehaviour[] behaviours)
        {
            StateBehavioursPair pair;
            if (this.m_Behaviours == null)
            {
                this.m_Behaviours = new StateBehavioursPair[0];
            }
            for (int i = 0; i < this.m_Behaviours.Length; i++)
            {
                if (this.m_Behaviours[i].m_State == state)
                {
                    this.m_Behaviours[i].m_Behaviours = behaviours;
                    return;
                }
            }
            pair.m_State = state;
            pair.m_Behaviours = behaviours;
            ArrayUtility.Add<StateBehavioursPair>(ref this.m_Behaviours, pair);
        }

        /// <summary>
        /// <para>Sets the override motion for the state on the given layer.</para>
        /// </summary>
        /// <param name="state">The state which we want to set the motion.</param>
        /// <param name="motion">The motion that will be set.</param>
        public void SetOverrideMotion(AnimatorState state, Motion motion)
        {
            StateMotionPair pair;
            if (this.m_Motions == null)
            {
                this.m_Motions = new StateMotionPair[0];
            }
            for (int i = 0; i < this.m_Motions.Length; i++)
            {
                if (this.m_Motions[i].m_State == state)
                {
                    this.m_Motions[i].m_Motion = motion;
                    return;
                }
            }
            pair.m_State = state;
            pair.m_Motion = motion;
            ArrayUtility.Add<StateMotionPair>(ref this.m_Motions, pair);
        }

        /// <summary>
        /// <para>The AvatarMask that is used to mask the animation on the given layer.</para>
        /// </summary>
        public AvatarMask avatarMask
        {
            get => 
                this.m_AvatarMask;
            set
            {
                this.m_AvatarMask = value;
            }
        }

        /// <summary>
        /// <para>The blending mode used by the layer. It is not taken into account for the first layer.</para>
        /// </summary>
        public AnimatorLayerBlendingMode blendingMode
        {
            get => 
                this.m_BlendingMode;
            set
            {
                this.m_BlendingMode = value;
            }
        }

        /// <summary>
        /// <para>The default blending weight that the layers has. It is not taken into account for the first layer.</para>
        /// </summary>
        public float defaultWeight
        {
            get => 
                this.m_DefaultWeight;
            set
            {
                this.m_DefaultWeight = value;
            }
        }

        /// <summary>
        /// <para>When active, the layer will have an IK pass when evaluated. It will trigger an OnAnimatorIK callback.</para>
        /// </summary>
        public bool iKPass
        {
            get => 
                this.m_IKPass;
            set
            {
                this.m_IKPass = value;
            }
        }

        /// <summary>
        /// <para>The name of the layer.</para>
        /// </summary>
        public string name
        {
            get => 
                this.m_Name;
            set
            {
                this.m_Name = value;
            }
        }

        /// <summary>
        /// <para>The state machine for the layer.</para>
        /// </summary>
        public AnimatorStateMachine stateMachine
        {
            get => 
                this.m_StateMachine;
            set
            {
                this.m_StateMachine = value;
            }
        }

        /// <summary>
        /// <para>When active, the layer will take control of the duration of the Synced Layer.</para>
        /// </summary>
        public bool syncedLayerAffectsTiming
        {
            get => 
                this.m_SyncedLayerAffectsTiming;
            set
            {
                this.m_SyncedLayerAffectsTiming = value;
            }
        }

        /// <summary>
        /// <para>Specifies the index of the Synced Layer.</para>
        /// </summary>
        public int syncedLayerIndex
        {
            get => 
                this.m_SyncedLayerIndex;
            set
            {
                this.m_SyncedLayerIndex = value;
            }
        }
    }
}

