namespace UnityEditor.Animations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEngine;
    using UnityEngineInternal;

    /// <summary>
    /// <para>A graph controlling the interaction of states. Each state references a motion.</para>
    /// </summary>
    public sealed class AnimatorStateMachine : Object
    {
        private PushUndoIfNeeded undoHandler = new PushUndoIfNeeded(true);

        public AnimatorStateMachine()
        {
            Internal_Create(this);
        }

        private AnimatorStateTransition AddAnyStateTransition()
        {
            this.undoHandler.DoUndo(this, "AnyState Transition Added");
            AnimatorStateTransition[] anyStateTransitions = this.anyStateTransitions;
            AnimatorStateTransition objectToAdd = new AnimatorStateTransition {
                hasExitTime = false,
                hasFixedDuration = true
            };
            if (AssetDatabase.GetAssetPath(this) != "")
            {
                AssetDatabase.AddObjectToAsset(objectToAdd, AssetDatabase.GetAssetPath(this));
            }
            objectToAdd.hideFlags = HideFlags.HideInHierarchy;
            ArrayUtility.Add<AnimatorStateTransition>(ref anyStateTransitions, objectToAdd);
            this.anyStateTransitions = anyStateTransitions;
            return objectToAdd;
        }

        /// <summary>
        /// <para>Utility function to add an AnyState transition to the specified state or statemachine.</para>
        /// </summary>
        /// <param name="destinationState">The destination state.</param>
        /// <param name="destinationStateMachine">The destination statemachine.</param>
        public AnimatorStateTransition AddAnyStateTransition(AnimatorState destinationState)
        {
            AnimatorStateTransition transition = this.AddAnyStateTransition();
            transition.destinationState = destinationState;
            return transition;
        }

        /// <summary>
        /// <para>Utility function to add an AnyState transition to the specified state or statemachine.</para>
        /// </summary>
        /// <param name="destinationState">The destination state.</param>
        /// <param name="destinationStateMachine">The destination statemachine.</param>
        public AnimatorStateTransition AddAnyStateTransition(AnimatorStateMachine destinationStateMachine)
        {
            AnimatorStateTransition transition = this.AddAnyStateTransition();
            transition.destinationStateMachine = destinationStateMachine;
            return transition;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern void AddBehaviour(int instanceID);
        private AnimatorTransition AddEntryTransition()
        {
            this.undoHandler.DoUndo(this, "Entry Transition Added");
            AnimatorTransition[] entryTransitions = this.entryTransitions;
            AnimatorTransition objectToAdd = new AnimatorTransition();
            if (AssetDatabase.GetAssetPath(this) != "")
            {
                AssetDatabase.AddObjectToAsset(objectToAdd, AssetDatabase.GetAssetPath(this));
            }
            objectToAdd.hideFlags = HideFlags.HideInHierarchy;
            ArrayUtility.Add<AnimatorTransition>(ref entryTransitions, objectToAdd);
            this.entryTransitions = entryTransitions;
            return objectToAdd;
        }

        /// <summary>
        /// <para>Utility function to add an incoming transition to the exit of it's parent state machine.</para>
        /// </summary>
        /// <param name="destinationState">The destination Animations.AnimatorState state.</param>
        /// <param name="destinationStateMachine">The destination Animations.AnimatorStateMachine state machine.</param>
        public AnimatorTransition AddEntryTransition(AnimatorState destinationState)
        {
            AnimatorTransition transition = this.AddEntryTransition();
            transition.destinationState = destinationState;
            return transition;
        }

        /// <summary>
        /// <para>Utility function to add an incoming transition to the exit of it's parent state machine.</para>
        /// </summary>
        /// <param name="destinationState">The destination Animations.AnimatorState state.</param>
        /// <param name="destinationStateMachine">The destination Animations.AnimatorStateMachine state machine.</param>
        public AnimatorTransition AddEntryTransition(AnimatorStateMachine destinationStateMachine)
        {
            AnimatorTransition transition = this.AddEntryTransition();
            transition.destinationStateMachine = destinationStateMachine;
            return transition;
        }

        /// <summary>
        /// <para>Utility function to add a state to the state machine.</para>
        /// </summary>
        /// <param name="name">The name of the new state.</param>
        /// <param name="position">The position of the state node.</param>
        /// <returns>
        /// <para>The AnimatorState that was created for this state.</para>
        /// </returns>
        public AnimatorState AddState(string name) => 
            this.AddState(name, (this.states.Length <= 0) ? new Vector3(200f, 0f, 0f) : (this.states[this.states.Length - 1].position + new Vector3(35f, 65f)));

        /// <summary>
        /// <para>Utility function to add a state to the state machine.</para>
        /// </summary>
        /// <param name="name">The name of the new state.</param>
        /// <param name="position">The position of the state node.</param>
        /// <returns>
        /// <para>The AnimatorState that was created for this state.</para>
        /// </returns>
        public AnimatorState AddState(string name, Vector3 position)
        {
            AnimatorState objectToAdd = new AnimatorState {
                hideFlags = HideFlags.HideInHierarchy,
                name = this.MakeUniqueStateName(name)
            };
            if (AssetDatabase.GetAssetPath(this) != "")
            {
                AssetDatabase.AddObjectToAsset(objectToAdd, AssetDatabase.GetAssetPath(this));
            }
            this.AddState(objectToAdd, position);
            return objectToAdd;
        }

        /// <summary>
        /// <para>Utility function to add a state to the state machine.</para>
        /// </summary>
        /// <param name="state">The state to add.</param>
        /// <param name="position">The position of the state node.</param>
        public void AddState(AnimatorState state, Vector3 position)
        {
            this.undoHandler.DoUndo(this, "State added");
            ChildAnimatorState item = new ChildAnimatorState {
                state = state,
                position = position
            };
            ChildAnimatorState[] states = this.states;
            ArrayUtility.Add<ChildAnimatorState>(ref states, item);
            this.states = states;
        }

        /// <summary>
        /// <para>Utility function to add a state machine to the state machine.</para>
        /// </summary>
        /// <param name="name">The name of the new state machine.</param>
        /// <param name="position">The position of the state machine node.</param>
        /// <returns>
        /// <para>The newly created Animations.AnimatorStateMachine state machine.</para>
        /// </returns>
        public AnimatorStateMachine AddStateMachine(string name) => 
            this.AddStateMachine(name, Vector3.zero);

        /// <summary>
        /// <para>Utility function to add a state machine to the state machine.</para>
        /// </summary>
        /// <param name="name">The name of the new state machine.</param>
        /// <param name="position">The position of the state machine node.</param>
        /// <returns>
        /// <para>The newly created Animations.AnimatorStateMachine state machine.</para>
        /// </returns>
        public AnimatorStateMachine AddStateMachine(string name, Vector3 position)
        {
            AnimatorStateMachine stateMachine = new AnimatorStateMachine {
                hideFlags = HideFlags.HideInHierarchy,
                name = this.MakeUniqueStateMachineName(name)
            };
            this.AddStateMachine(stateMachine, position);
            if (AssetDatabase.GetAssetPath(this) != "")
            {
                AssetDatabase.AddObjectToAsset(stateMachine, AssetDatabase.GetAssetPath(this));
            }
            return stateMachine;
        }

        /// <summary>
        /// <para>Utility function to add a state machine to the state machine.</para>
        /// </summary>
        /// <param name="stateMachine">The state machine to add.</param>
        /// <param name="position">The position of the state machine node.</param>
        public void AddStateMachine(AnimatorStateMachine stateMachine, Vector3 position)
        {
            this.undoHandler.DoUndo(this, "StateMachine " + stateMachine.name + " added");
            ChildAnimatorStateMachine item = new ChildAnimatorStateMachine {
                stateMachine = stateMachine,
                position = position
            };
            ChildAnimatorStateMachine[] stateMachines = this.stateMachines;
            ArrayUtility.Add<ChildAnimatorStateMachine>(ref stateMachines, item);
            this.stateMachines = stateMachines;
        }

        public T AddStateMachineBehaviour<T>() where T: StateMachineBehaviour => 
            (this.AddStateMachineBehaviour(typeof(T)) as T);

        /// <summary>
        /// <para>Adds a state machine behaviour class of type stateMachineBehaviourType to the AnimatorStateMachine. C# Users can use a generic version.</para>
        /// </summary>
        /// <param name="stateMachineBehaviourType"></param>
        [TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
        public StateMachineBehaviour AddStateMachineBehaviour(Type stateMachineBehaviourType) => 
            ((StateMachineBehaviour) this.Internal_AddStateMachineBehaviourWithType(stateMachineBehaviourType));

        /// <summary>
        /// <para>Utility function to add an outgoing transition from the source state machine to the exit of it's parent state machine.</para>
        /// </summary>
        /// <param name="sourceStateMachine">The source state machine.</param>
        public AnimatorTransition AddStateMachineExitTransition(AnimatorStateMachine sourceStateMachine)
        {
            AnimatorTransition transition = this.AddStateMachineTransition(sourceStateMachine);
            transition.isExit = true;
            return transition;
        }

        /// <summary>
        /// <para>Utility function to add an outgoing transition from the source state machine to the destination.</para>
        /// </summary>
        /// <param name="sourceStateMachine">The source state machine.</param>
        /// <param name="destinationStateMachine">The destination state machine.</param>
        /// <param name="destinationState">The destination state.</param>
        /// <returns>
        /// <para>The Animations.AnimatorTransition transition that was created.</para>
        /// </returns>
        public AnimatorTransition AddStateMachineTransition(AnimatorStateMachine sourceStateMachine)
        {
            AnimatorStateMachine destinationStateMachine = null;
            return this.AddStateMachineTransition(sourceStateMachine, destinationStateMachine);
        }

        /// <summary>
        /// <para>Utility function to add an outgoing transition from the source state machine to the destination.</para>
        /// </summary>
        /// <param name="sourceStateMachine">The source state machine.</param>
        /// <param name="destinationStateMachine">The destination state machine.</param>
        /// <param name="destinationState">The destination state.</param>
        /// <returns>
        /// <para>The Animations.AnimatorTransition transition that was created.</para>
        /// </returns>
        public AnimatorTransition AddStateMachineTransition(AnimatorStateMachine sourceStateMachine, AnimatorState destinationState)
        {
            AnimatorTransition transition = this.AddStateMachineTransition(sourceStateMachine);
            transition.destinationState = destinationState;
            return transition;
        }

        /// <summary>
        /// <para>Utility function to add an outgoing transition from the source state machine to the destination.</para>
        /// </summary>
        /// <param name="sourceStateMachine">The source state machine.</param>
        /// <param name="destinationStateMachine">The destination state machine.</param>
        /// <param name="destinationState">The destination state.</param>
        /// <returns>
        /// <para>The Animations.AnimatorTransition transition that was created.</para>
        /// </returns>
        public AnimatorTransition AddStateMachineTransition(AnimatorStateMachine sourceStateMachine, AnimatorStateMachine destinationStateMachine)
        {
            this.undoHandler.DoUndo(this, "StateMachine Transition Added");
            AnimatorTransition[] stateMachineTransitions = this.GetStateMachineTransitions(sourceStateMachine);
            AnimatorTransition objectToAdd = new AnimatorTransition();
            if (destinationStateMachine != null)
            {
                objectToAdd.destinationStateMachine = destinationStateMachine;
            }
            if (AssetDatabase.GetAssetPath(this) != "")
            {
                AssetDatabase.AddObjectToAsset(objectToAdd, AssetDatabase.GetAssetPath(this));
            }
            objectToAdd.hideFlags = HideFlags.HideInHierarchy;
            ArrayUtility.Add<AnimatorTransition>(ref stateMachineTransitions, objectToAdd);
            this.SetStateMachineTransitions(sourceStateMachine, stateMachineTransitions);
            return objectToAdd;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern void Clear();
        internal AnimatorStateMachine FindParent(AnimatorStateMachine stateMachine)
        {
            <FindParent>c__AnonStorey8 storey = new <FindParent>c__AnonStorey8 {
                stateMachine = stateMachine
            };
            if (Enumerable.Any<ChildAnimatorStateMachine>(this.stateMachines, new Func<ChildAnimatorStateMachine, bool>(storey, (IntPtr) this.<>m__0)))
            {
                return this;
            }
            return this.stateMachinesRecursive.Find(new Predicate<ChildAnimatorStateMachine>(storey.<>m__1)).stateMachine;
        }

        internal ChildAnimatorState FindState(int nameHash)
        {
            <FindState>c__AnonStorey2 storey = new <FindState>c__AnonStorey2 {
                nameHash = nameHash
            };
            return new List<ChildAnimatorState>(this.states).Find(new Predicate<ChildAnimatorState>(storey.<>m__0));
        }

        internal ChildAnimatorState FindState(string name)
        {
            <FindState>c__AnonStorey3 storey = new <FindState>c__AnonStorey3 {
                name = name
            };
            return new List<ChildAnimatorState>(this.states).Find(new Predicate<ChildAnimatorState>(storey.<>m__0));
        }

        internal AnimatorStateMachine FindStateMachine(string path)
        {
            <FindStateMachine>c__AnonStorey9 storey = new <FindStateMachine>c__AnonStorey9();
            char[] separator = new char[] { '.' };
            storey.smNames = path.Split(separator);
            AnimatorStateMachine machine = this;
            <FindStateMachine>c__AnonStoreyA ya = new <FindStateMachine>c__AnonStoreyA {
                <>f__ref$9 = storey,
                i = 1
            };
            while ((ya.i < (storey.smNames.Length - 1)) && (machine != null))
            {
                int index = Array.FindIndex<ChildAnimatorStateMachine>(machine.stateMachines, new Predicate<ChildAnimatorStateMachine>(ya.<>m__0));
                machine = (index < 0) ? null : machine.stateMachines[index].stateMachine;
                ya.i++;
            }
            return ((machine != null) ? machine : this);
        }

        internal AnimatorStateMachine FindStateMachine(AnimatorState state)
        {
            <FindStateMachine>c__AnonStoreyB yb = new <FindStateMachine>c__AnonStoreyB {
                state = state
            };
            if (this.HasState(yb.state, false))
            {
                return this;
            }
            List<ChildAnimatorStateMachine> stateMachinesRecursive = this.stateMachinesRecursive;
            int num = stateMachinesRecursive.FindIndex(new Predicate<ChildAnimatorStateMachine>(yb.<>m__0));
            return ((num < 0) ? null : stateMachinesRecursive[num].stateMachine);
        }

        internal AnimatorStateTransition FindTransition(AnimatorState destinationState)
        {
            <FindTransition>c__AnonStoreyC yc = new <FindTransition>c__AnonStoreyC {
                destinationState = destinationState
            };
            return new List<AnimatorStateTransition>(this.anyStateTransitions).Find(new Predicate<AnimatorStateTransition>(yc.<>m__0));
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern MonoScript GetBehaviourMonoScript(int index);
        internal Vector3 GetStateMachinePosition(AnimatorStateMachine stateMachine)
        {
            ChildAnimatorStateMachine[] stateMachines = this.stateMachines;
            for (int i = 0; i < stateMachines.Length; i++)
            {
                if (stateMachine == stateMachines[i].stateMachine)
                {
                    return stateMachines[i].position;
                }
            }
            return Vector3.zero;
        }

        /// <summary>
        /// <para>Gets the list of all outgoing state machine transitions from given state machine.</para>
        /// </summary>
        /// <param name="sourceStateMachine">The source state machine.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern AnimatorTransition[] GetStateMachineTransitions(AnimatorStateMachine sourceStateMachine);
        internal Vector3 GetStatePosition(AnimatorState state)
        {
            ChildAnimatorState[] states = this.states;
            for (int i = 0; i < states.Length; i++)
            {
                if (state == states[i].state)
                {
                    return states[i].position;
                }
            }
            return Vector3.zero;
        }

        [Obsolete("GetTransitionsFromState is obsolete. Use AnimatorState.transitions instead.", true)]
        private AnimatorState GetTransitionsFromState(AnimatorState state) => 
            null;

        internal bool HasState(AnimatorState state)
        {
            <HasState>c__AnonStorey4 storey = new <HasState>c__AnonStorey4 {
                state = state
            };
            return Enumerable.Any<ChildAnimatorState>(this.statesRecursive, new Func<ChildAnimatorState, bool>(storey, (IntPtr) this.<>m__0));
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern bool HasState(AnimatorState state, bool recursive);
        internal bool HasStateMachine(AnimatorStateMachine child)
        {
            <HasStateMachine>c__AnonStorey6 storey = new <HasStateMachine>c__AnonStorey6 {
                child = child
            };
            return Enumerable.Any<ChildAnimatorStateMachine>(this.stateMachinesRecursive, new Func<ChildAnimatorStateMachine, bool>(storey, (IntPtr) this.<>m__0));
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern bool HasStateMachine(AnimatorStateMachine state, bool recursive);
        internal bool HasTransition(AnimatorState stateA, AnimatorState stateB)
        {
            <HasTransition>c__AnonStorey7 storey = new <HasTransition>c__AnonStorey7 {
                stateB = stateB,
                stateA = stateA
            };
            return (Enumerable.Any<AnimatorStateTransition>(storey.stateA.transitions, new Func<AnimatorStateTransition, bool>(storey, (IntPtr) this.<>m__0)) || Enumerable.Any<AnimatorStateTransition>(storey.stateB.transitions, new Func<AnimatorStateTransition, bool>(storey, (IntPtr) this.<>m__1)));
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern ScriptableObject Internal_AddStateMachineBehaviourWithType(Type stateMachineBehaviourType);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void Internal_Create(AnimatorStateMachine mono);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_anyStatePosition(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_entryPosition(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_exitPosition(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_parentStateMachinePosition(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_anyStatePosition(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_entryPosition(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_exitPosition(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_parentStateMachinePosition(ref Vector3 value);
        internal bool IsDirectParent(AnimatorStateMachine stateMachine)
        {
            <IsDirectParent>c__AnonStorey5 storey = new <IsDirectParent>c__AnonStorey5 {
                stateMachine = stateMachine
            };
            return Enumerable.Any<ChildAnimatorStateMachine>(this.stateMachines, new Func<ChildAnimatorStateMachine, bool>(storey, (IntPtr) this.<>m__0));
        }

        /// <summary>
        /// <para>Makes a unique state machine name in the context of the parent state machine.</para>
        /// </summary>
        /// <param name="name">Desired name for the state machine.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern string MakeUniqueStateMachineName(string name);
        /// <summary>
        /// <para>Makes a unique state name in the context of the parent state machine.</para>
        /// </summary>
        /// <param name="name">Desired name for the state.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern string MakeUniqueStateName(string name);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern void MoveState(AnimatorState state, AnimatorStateMachine target);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern void MoveStateMachine(AnimatorStateMachine stateMachine, AnimatorStateMachine target);
        /// <summary>
        /// <para>Utility function to remove an AnyState transition from the state machine.</para>
        /// </summary>
        /// <param name="transition">The AnyStat transition to remove.</param>
        public bool RemoveAnyStateTransition(AnimatorStateTransition transition)
        {
            <RemoveAnyStateTransition>c__AnonStorey0 storey = new <RemoveAnyStateTransition>c__AnonStorey0 {
                transition = transition
            };
            if (Enumerable.Any<AnimatorStateTransition>(new List<AnimatorStateTransition>(this.anyStateTransitions), new Func<AnimatorStateTransition, bool>(storey, (IntPtr) this.<>m__0)))
            {
                this.undoHandler.DoUndo(this, "AnyState Transition Removed");
                AnimatorStateTransition[] anyStateTransitions = this.anyStateTransitions;
                ArrayUtility.Remove<AnimatorStateTransition>(ref anyStateTransitions, storey.transition);
                this.anyStateTransitions = anyStateTransitions;
                if (MecanimUtilities.AreSameAsset(this, storey.transition))
                {
                    Undo.DestroyObjectImmediate(storey.transition);
                }
                return true;
            }
            return false;
        }

        internal void RemoveAnyStateTransitionRecursive(AnimatorStateTransition transition)
        {
            if (!this.RemoveAnyStateTransition(transition))
            {
                List<ChildAnimatorStateMachine> stateMachinesRecursive = this.stateMachinesRecursive;
                foreach (ChildAnimatorStateMachine machine in stateMachinesRecursive)
                {
                    if (machine.stateMachine.RemoveAnyStateTransition(transition))
                    {
                        break;
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern void RemoveBehaviour(int index);
        /// <summary>
        /// <para>Utility function to remove an entry transition from the state machine.</para>
        /// </summary>
        /// <param name="transition">The transition to remove.</param>
        public bool RemoveEntryTransition(AnimatorTransition transition)
        {
            <RemoveEntryTransition>c__AnonStorey1 storey = new <RemoveEntryTransition>c__AnonStorey1 {
                transition = transition
            };
            if (Enumerable.Any<AnimatorTransition>(new List<AnimatorTransition>(this.entryTransitions), new Func<AnimatorTransition, bool>(storey, (IntPtr) this.<>m__0)))
            {
                this.undoHandler.DoUndo(this, "Entry Transition Removed");
                AnimatorTransition[] entryTransitions = this.entryTransitions;
                ArrayUtility.Remove<AnimatorTransition>(ref entryTransitions, storey.transition);
                this.entryTransitions = entryTransitions;
                if (MecanimUtilities.AreSameAsset(this, storey.transition))
                {
                    Undo.DestroyObjectImmediate(storey.transition);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// <para>Utility function to remove a state from the state machine.</para>
        /// </summary>
        /// <param name="state">The state to remove.</param>
        public void RemoveState(AnimatorState state)
        {
            this.undoHandler.DoUndo(this, "State removed");
            this.undoHandler.DoUndo(state, "State removed");
            this.RemoveStateInternal(state);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern void RemoveStateInternal(AnimatorState state);
        /// <summary>
        /// <para>Utility function to remove a state machine from its parent state machine.</para>
        /// </summary>
        /// <param name="stateMachine">The state machine to remove.</param>
        public void RemoveStateMachine(AnimatorStateMachine stateMachine)
        {
            this.undoHandler.DoUndo(this, "StateMachine removed");
            this.undoHandler.DoUndo(stateMachine, "StateMachine removed");
            this.RemoveStateMachineInternal(stateMachine);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern void RemoveStateMachineInternal(AnimatorStateMachine stateMachine);
        /// <summary>
        /// <para>Utility function to remove an outgoing transition from source state machine.</para>
        /// </summary>
        /// <param name="transition">The transition to remove.</param>
        /// <param name="sourceStateMachine">The source state machine.</param>
        public bool RemoveStateMachineTransition(AnimatorStateMachine sourceStateMachine, AnimatorTransition transition)
        {
            this.undoHandler.DoUndo(this, "StateMachine Transition Removed");
            AnimatorTransition[] stateMachineTransitions = this.GetStateMachineTransitions(sourceStateMachine);
            int length = stateMachineTransitions.Length;
            ArrayUtility.Remove<AnimatorTransition>(ref stateMachineTransitions, transition);
            this.SetStateMachineTransitions(sourceStateMachine, stateMachineTransitions);
            if (MecanimUtilities.AreSameAsset(this, transition))
            {
                Undo.DestroyObjectImmediate(transition);
            }
            return (length != stateMachineTransitions.Length);
        }

        internal void SetStateMachinePosition(AnimatorStateMachine stateMachine, Vector3 position)
        {
            ChildAnimatorStateMachine[] stateMachines = this.stateMachines;
            for (int i = 0; i < stateMachines.Length; i++)
            {
                if (stateMachine == stateMachines[i].stateMachine)
                {
                    stateMachines[i].position = position;
                    this.stateMachines = stateMachines;
                    break;
                }
            }
        }

        /// <summary>
        /// <para>Sets the list of all outgoing state machine transitions from given state machine.</para>
        /// </summary>
        /// <param name="stateMachine">The source state machine.</param>
        /// <param name="transitions">The outgoing transitions.</param>
        /// <param name="sourceStateMachine"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void SetStateMachineTransitions(AnimatorStateMachine sourceStateMachine, AnimatorTransition[] transitions);
        internal void SetStatePosition(AnimatorState state, Vector3 position)
        {
            ChildAnimatorState[] states = this.states;
            for (int i = 0; i < states.Length; i++)
            {
                if (state == states[i].state)
                {
                    states[i].position = position;
                    this.states = states;
                    break;
                }
            }
        }

        /// <summary>
        /// <para>The position of the AnyState node.</para>
        /// </summary>
        public Vector3 anyStatePosition
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_anyStatePosition(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_anyStatePosition(ref value);
            }
        }

        /// <summary>
        /// <para>The list of AnyState transitions.</para>
        /// </summary>
        public AnimatorStateTransition[] anyStateTransitions { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        internal List<AnimatorStateTransition> anyStateTransitionsRecursive
        {
            get
            {
                List<AnimatorStateTransition> list = new List<AnimatorStateTransition>();
                list.AddRange(this.anyStateTransitions);
                foreach (ChildAnimatorStateMachine machine in this.stateMachines)
                {
                    list.AddRange(machine.stateMachine.anyStateTransitionsRecursive);
                }
                return list;
            }
        }

        /// <summary>
        /// <para>The Behaviour list assigned to this state machine.</para>
        /// </summary>
        public StateMachineBehaviour[] behaviours { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The state that the state machine will be in when it starts.</para>
        /// </summary>
        public AnimatorState defaultState { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The position of the entry node.</para>
        /// </summary>
        public Vector3 entryPosition
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_entryPosition(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_entryPosition(ref value);
            }
        }

        /// <summary>
        /// <para>The list of entry transitions in the state machine.</para>
        /// </summary>
        public AnimatorTransition[] entryTransitions { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The position of the exit node.</para>
        /// </summary>
        public Vector3 exitPosition
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_exitPosition(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_exitPosition(ref value);
            }
        }

        /// <summary>
        /// <para>The position of the parent state machine node. Only valid when in a hierachic state machine.</para>
        /// </summary>
        public Vector3 parentStateMachinePosition
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_parentStateMachinePosition(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_parentStateMachinePosition(ref value);
            }
        }

        internal bool pushUndo
        {
            set
            {
                this.undoHandler.pushUndo = value;
            }
        }

        [Obsolete("stateCount is obsolete. Use .states.Length  instead.", true)]
        private int stateCount =>
            0;

        [Obsolete("stateMachineCount is obsolete. Use .stateMachines.Length instead.", true)]
        private int stateMachineCount =>
            0;

        /// <summary>
        /// <para>The list of sub state machines.</para>
        /// </summary>
        public ChildAnimatorStateMachine[] stateMachines { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        internal List<ChildAnimatorStateMachine> stateMachinesRecursive
        {
            get
            {
                List<ChildAnimatorStateMachine> list = new List<ChildAnimatorStateMachine>();
                list.AddRange(this.stateMachines);
                for (int i = 0; i < this.stateMachines.Length; i++)
                {
                    list.AddRange(this.stateMachines[i].stateMachine.stateMachinesRecursive);
                }
                return list;
            }
        }

        /// <summary>
        /// <para>The list of states.</para>
        /// </summary>
        public ChildAnimatorState[] states { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        internal List<ChildAnimatorState> statesRecursive
        {
            get
            {
                List<ChildAnimatorState> list = new List<ChildAnimatorState>();
                list.AddRange(this.states);
                for (int i = 0; i < this.stateMachines.Length; i++)
                {
                    list.AddRange(this.stateMachines[i].stateMachine.statesRecursive);
                }
                return list;
            }
        }

        internal int transitionCount { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        [Obsolete("uniqueNameHash does not exist anymore.", true)]
        private int uniqueNameHash =>
            -1;

        [CompilerGenerated]
        private sealed class <FindParent>c__AnonStorey8
        {
            internal AnimatorStateMachine stateMachine;

            internal bool <>m__0(ChildAnimatorStateMachine childSM) => 
                (childSM.stateMachine == this.stateMachine);

            internal bool <>m__1(ChildAnimatorStateMachine sm) => 
                Enumerable.Any<ChildAnimatorStateMachine>(sm.stateMachine.stateMachines, new Func<ChildAnimatorStateMachine, bool>(this, (IntPtr) this.<>m__2));

            internal bool <>m__2(ChildAnimatorStateMachine childSM) => 
                (childSM.stateMachine == this.stateMachine);
        }

        [CompilerGenerated]
        private sealed class <FindState>c__AnonStorey2
        {
            internal int nameHash;

            internal bool <>m__0(ChildAnimatorState s) => 
                (s.state.nameHash == this.nameHash);
        }

        [CompilerGenerated]
        private sealed class <FindState>c__AnonStorey3
        {
            internal string name;

            internal bool <>m__0(ChildAnimatorState s) => 
                (s.state.name == this.name);
        }

        [CompilerGenerated]
        private sealed class <FindStateMachine>c__AnonStorey9
        {
            internal string[] smNames;
        }

        [CompilerGenerated]
        private sealed class <FindStateMachine>c__AnonStoreyA
        {
            internal AnimatorStateMachine.<FindStateMachine>c__AnonStorey9 <>f__ref$9;
            internal int i;

            internal bool <>m__0(ChildAnimatorStateMachine t) => 
                (t.stateMachine.name == this.<>f__ref$9.smNames[this.i]);
        }

        [CompilerGenerated]
        private sealed class <FindStateMachine>c__AnonStoreyB
        {
            internal AnimatorState state;

            internal bool <>m__0(ChildAnimatorStateMachine sm) => 
                sm.stateMachine.HasState(this.state, false);
        }

        [CompilerGenerated]
        private sealed class <FindTransition>c__AnonStoreyC
        {
            internal AnimatorState destinationState;

            internal bool <>m__0(AnimatorStateTransition t) => 
                (t.destinationState == this.destinationState);
        }

        [CompilerGenerated]
        private sealed class <HasState>c__AnonStorey4
        {
            internal AnimatorState state;

            internal bool <>m__0(ChildAnimatorState s) => 
                (s.state == this.state);
        }

        [CompilerGenerated]
        private sealed class <HasStateMachine>c__AnonStorey6
        {
            internal AnimatorStateMachine child;

            internal bool <>m__0(ChildAnimatorStateMachine sm) => 
                (sm.stateMachine == this.child);
        }

        [CompilerGenerated]
        private sealed class <HasTransition>c__AnonStorey7
        {
            internal AnimatorState stateA;
            internal AnimatorState stateB;

            internal bool <>m__0(AnimatorStateTransition t) => 
                (t.destinationState == this.stateB);

            internal bool <>m__1(AnimatorStateTransition t) => 
                (t.destinationState == this.stateA);
        }

        [CompilerGenerated]
        private sealed class <IsDirectParent>c__AnonStorey5
        {
            internal AnimatorStateMachine stateMachine;

            internal bool <>m__0(ChildAnimatorStateMachine sm) => 
                (sm.stateMachine == this.stateMachine);
        }

        [CompilerGenerated]
        private sealed class <RemoveAnyStateTransition>c__AnonStorey0
        {
            internal AnimatorStateTransition transition;

            internal bool <>m__0(AnimatorStateTransition t) => 
                (t == this.transition);
        }

        [CompilerGenerated]
        private sealed class <RemoveEntryTransition>c__AnonStorey1
        {
            internal AnimatorTransition transition;

            internal bool <>m__0(AnimatorTransition t) => 
                (t == this.transition);
        }
    }
}

