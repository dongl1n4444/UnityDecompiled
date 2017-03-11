namespace UnityEditor.Animations
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Scripting;
    using UnityEngineInternal;

    /// <summary>
    /// <para>States are the basic building blocks of a state machine. Each state contains a Motion ( AnimationClip or BlendTree) which will play while the character is in that state. When an event in the game triggers a state transition, the character will be left in a new state whose animation sequence will then take over.</para>
    /// </summary>
    public sealed class AnimatorState : UnityEngine.Object
    {
        private PushUndoIfNeeded undoHandler = new PushUndoIfNeeded(true);

        public AnimatorState()
        {
            Internal_Create(this);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern void AddBehaviour(int instanceID);
        /// <summary>
        /// <para>Utility function to add an outgoing transition to the exit of the state's parent state machine.</para>
        /// </summary>
        /// <param name="defaultExitTime">If true, the exit time will be the equivalent of 0.25 second.</param>
        /// <returns>
        /// <para>The Animations.AnimatorStateTransition that was added.</para>
        /// </returns>
        public AnimatorStateTransition AddExitTransition() => 
            this.AddExitTransition(false);

        /// <summary>
        /// <para>Utility function to add an outgoing transition to the exit of the state's parent state machine.</para>
        /// </summary>
        /// <param name="defaultExitTime">If true, the exit time will be the equivalent of 0.25 second.</param>
        /// <returns>
        /// <para>The Animations.AnimatorStateTransition that was added.</para>
        /// </returns>
        public AnimatorStateTransition AddExitTransition(bool defaultExitTime)
        {
            AnimatorStateTransition transition = this.CreateTransition(defaultExitTime);
            transition.isExit = true;
            this.AddTransition(transition);
            return transition;
        }

        public T AddStateMachineBehaviour<T>() where T: StateMachineBehaviour => 
            (this.AddStateMachineBehaviour(typeof(T)) as T);

        /// <summary>
        /// <para>Adds a state machine behaviour class of type stateMachineBehaviourType to the AnimatorState. C# Users can use a generic version.</para>
        /// </summary>
        /// <param name="stateMachineBehaviourType"></param>
        [TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
        public StateMachineBehaviour AddStateMachineBehaviour(System.Type stateMachineBehaviourType) => 
            ((StateMachineBehaviour) this.Internal_AddStateMachineBehaviourWithType(stateMachineBehaviourType));

        /// <summary>
        /// <para>Utility function to add an outgoing transition to the destination state.</para>
        /// </summary>
        /// <param name="defaultExitTime">If true, the exit time will be the equivalent of 0.25 second.</param>
        /// <param name="destinationState">The destination state.</param>
        public AnimatorStateTransition AddTransition(AnimatorState destinationState)
        {
            AnimatorStateTransition transition = this.CreateTransition(false);
            transition.destinationState = destinationState;
            this.AddTransition(transition);
            return transition;
        }

        /// <summary>
        /// <para>Utility function to add an outgoing transition to the destination state machine.</para>
        /// </summary>
        /// <param name="defaultExitTime">If true, the exit time will be the equivalent of 0.25 second.</param>
        /// <param name="destinationStateMachine">The destination state machine.</param>
        public AnimatorStateTransition AddTransition(AnimatorStateMachine destinationStateMachine)
        {
            AnimatorStateTransition transition = this.CreateTransition(false);
            transition.destinationStateMachine = destinationStateMachine;
            this.AddTransition(transition);
            return transition;
        }

        /// <summary>
        /// <para>Utility function to add an outgoing transition.</para>
        /// </summary>
        /// <param name="transition">The transition to add.</param>
        public void AddTransition(AnimatorStateTransition transition)
        {
            this.undoHandler.DoUndo(this, "Transition added");
            AnimatorStateTransition[] transitions = this.transitions;
            ArrayUtility.Add<AnimatorStateTransition>(ref transitions, transition);
            this.transitions = transitions;
        }

        /// <summary>
        /// <para>Utility function to add an outgoing transition to the destination state.</para>
        /// </summary>
        /// <param name="defaultExitTime">If true, the exit time will be the equivalent of 0.25 second.</param>
        /// <param name="destinationState">The destination state.</param>
        public AnimatorStateTransition AddTransition(AnimatorState destinationState, bool defaultExitTime)
        {
            AnimatorStateTransition transition = this.CreateTransition(defaultExitTime);
            transition.destinationState = destinationState;
            this.AddTransition(transition);
            return transition;
        }

        /// <summary>
        /// <para>Utility function to add an outgoing transition to the destination state machine.</para>
        /// </summary>
        /// <param name="defaultExitTime">If true, the exit time will be the equivalent of 0.25 second.</param>
        /// <param name="destinationStateMachine">The destination state machine.</param>
        public AnimatorStateTransition AddTransition(AnimatorStateMachine destinationStateMachine, bool defaultExitTime)
        {
            AnimatorStateTransition transition = this.CreateTransition(defaultExitTime);
            transition.destinationStateMachine = destinationStateMachine;
            this.AddTransition(transition);
            return transition;
        }

        private AnimatorStateTransition CreateTransition(bool setDefaultExitTime)
        {
            AnimatorStateTransition objectToAdd = new AnimatorStateTransition {
                hasExitTime = false,
                hasFixedDuration = true
            };
            if (AssetDatabase.GetAssetPath(this) != "")
            {
                AssetDatabase.AddObjectToAsset(objectToAdd, AssetDatabase.GetAssetPath(this));
            }
            objectToAdd.hideFlags = HideFlags.HideInHierarchy;
            if (setDefaultExitTime)
            {
                this.SetDefaultTransitionExitTime(ref objectToAdd);
            }
            return objectToAdd;
        }

        internal AnimatorStateMachine FindParent(AnimatorStateMachine root)
        {
            if (root.HasState(this, false))
            {
                return root;
            }
            return root.stateMachinesRecursive.Find(sm => sm.stateMachine.HasState(this, false)).stateMachine;
        }

        internal AnimatorStateTransition FindTransition(AnimatorState destinationState)
        {
            <FindTransition>c__AnonStorey0 storey = new <FindTransition>c__AnonStorey0 {
                destinationState = destinationState
            };
            return new List<AnimatorStateTransition>(this.transitions).Find(new Predicate<AnimatorStateTransition>(storey.<>m__0));
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern MonoScript GetBehaviourMonoScript(int index);
        [Obsolete("GetMotion() is obsolete. Use motion", true)]
        public Motion GetMotion() => 
            null;

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern ScriptableObject Internal_AddStateMachineBehaviourWithType(System.Type stateMachineBehaviourType);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void Internal_Create(AnimatorState mono);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern void RemoveBehaviour(int index);
        /// <summary>
        /// <para>Utility function to remove a transition from the state.</para>
        /// </summary>
        /// <param name="transition">Transition to remove.</param>
        public void RemoveTransition(AnimatorStateTransition transition)
        {
            this.undoHandler.DoUndo(this, "Transition removed");
            AnimatorStateTransition[] transitions = this.transitions;
            ArrayUtility.Remove<AnimatorStateTransition>(ref transitions, transition);
            this.transitions = transitions;
            if (MecanimUtilities.AreSameAsset(this, transition))
            {
                Undo.DestroyObjectImmediate(transition);
            }
        }

        private void SetDefaultTransitionExitTime(ref AnimatorStateTransition newTransition)
        {
            newTransition.hasExitTime = true;
            if ((this.motion != null) && (this.motion.averageDuration > 0f))
            {
                float num = 0.25f / this.motion.averageDuration;
                newTransition.duration = 0.25f;
                newTransition.exitTime = 1f - num;
            }
            else
            {
                newTransition.duration = 0.25f;
                newTransition.exitTime = 0.75f;
            }
        }

        /// <summary>
        /// <para>The Behaviour list assigned to this state.</para>
        /// </summary>
        public StateMachineBehaviour[] behaviours { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Offset at which the animation loop starts. Useful for synchronizing looped animations.
        /// Units is normalized time.</para>
        /// </summary>
        public float cycleOffset { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The animator controller parameter that drives the cycle offset value.</para>
        /// </summary>
        public string cycleOffsetParameter { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Define if the cycle offset value is driven by an Animator controller parameter or by the value set in the editor.</para>
        /// </summary>
        public bool cycleOffsetParameterActive { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Should Foot IK be respected for this state.</para>
        /// </summary>
        public bool iKOnFeet { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Should the state be mirrored.</para>
        /// </summary>
        public bool mirror { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The animator controller parameter that drives the mirror value.</para>
        /// </summary>
        public string mirrorParameter { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Define if the mirror value is driven by an Animator controller parameter or by the value set in the editor.</para>
        /// </summary>
        public bool mirrorParameterActive { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The motion assigned to this state.</para>
        /// </summary>
        public Motion motion { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The hashed name of the state.</para>
        /// </summary>
        public int nameHash { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        internal bool pushUndo
        {
            set
            {
                this.undoHandler.pushUndo = value;
            }
        }

        /// <summary>
        /// <para>The default speed of the motion.</para>
        /// </summary>
        public float speed { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The animator controller parameter that drives the speed value.</para>
        /// </summary>
        public string speedParameter { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Define if the speed value is driven by an Animator controller parameter or by the value set in the editor.</para>
        /// </summary>
        public bool speedParameterActive { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>A tag can be used to identify a state.</para>
        /// </summary>
        public string tag { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The transitions that are going out of the state.</para>
        /// </summary>
        public AnimatorStateTransition[] transitions { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        [Obsolete("uniqueName does not exist anymore. Consider using .name instead.", true)]
        public string uniqueName =>
            "";

        [Obsolete("uniqueNameHash does not exist anymore.", true)]
        public int uniqueNameHash =>
            -1;

        /// <summary>
        /// <para>Whether or not the AnimatorStates writes back the default values for properties that are not animated by its Motion.</para>
        /// </summary>
        public bool writeDefaultValues { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        [CompilerGenerated]
        private sealed class <FindTransition>c__AnonStorey0
        {
            internal AnimatorState destinationState;

            internal bool <>m__0(AnimatorStateTransition t) => 
                (t.destinationState == this.destinationState);
        }
    }
}

