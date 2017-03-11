namespace UnityEditor.Animations
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Base class for animator transitions. Transitions define when and how the state machine switches from one state to another.</para>
    /// </summary>
    public class AnimatorTransitionBase : UnityEngine.Object
    {
        private PushUndoIfNeeded undoHandler = new PushUndoIfNeeded(true);

        /// <summary>
        /// <para>Utility function to add a condition to a transition.</para>
        /// </summary>
        /// <param name="mode">The Animations.AnimatorCondition mode of the condition.</param>
        /// <param name="threshold">The threshold value of the condition.</param>
        /// <param name="parameter">The name of the parameter.</param>
        public void AddCondition(AnimatorConditionMode mode, float threshold, string parameter)
        {
            this.undoHandler.DoUndo(this, "Condition added");
            AnimatorCondition[] conditions = this.conditions;
            AnimatorCondition item = new AnimatorCondition {
                mode = mode,
                parameter = parameter,
                threshold = threshold
            };
            ArrayUtility.Add<AnimatorCondition>(ref conditions, item);
            this.conditions = conditions;
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern string BuildTransitionName(string source, string destination);
        public string GetDisplayName(UnityEngine.Object source) => 
            (!(source is AnimatorState) ? this.GetDisplayNameStateMachineSource(source as AnimatorStateMachine) : this.GetDisplayNameStateSource(source as AnimatorState));

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern string GetDisplayNameStateMachineSource(AnimatorStateMachine source);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern string GetDisplayNameStateSource(AnimatorState source);
        /// <summary>
        /// <para>Utility function to remove a condition from the transition.</para>
        /// </summary>
        /// <param name="condition">The condition to remove.</param>
        public void RemoveCondition(AnimatorCondition condition)
        {
            this.undoHandler.DoUndo(this, "Condition removed");
            AnimatorCondition[] conditions = this.conditions;
            ArrayUtility.Remove<AnimatorCondition>(ref conditions, condition);
            this.conditions = conditions;
        }

        /// <summary>
        /// <para>Animations.AnimatorCondition conditions that need to be met for a transition to happen.</para>
        /// </summary>
        public AnimatorCondition[] conditions { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The destination state of the transition.</para>
        /// </summary>
        public AnimatorState destinationState { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The destination state machine of the transition.</para>
        /// </summary>
        public AnimatorStateMachine destinationStateMachine { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Is the transition destination the exit of the current state machine.</para>
        /// </summary>
        public bool isExit { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Mutes the transition. The transition will never occur.</para>
        /// </summary>
        public bool mute { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        internal bool pushUndo
        {
            set
            {
                this.undoHandler.pushUndo = value;
            }
        }

        /// <summary>
        /// <para>Mutes all other transitions in the source state.</para>
        /// </summary>
        public bool solo { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}

