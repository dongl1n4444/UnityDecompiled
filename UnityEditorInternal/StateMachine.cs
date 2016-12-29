namespace UnityEditorInternal
{
    using System;
    using UnityEngine;

    [Obsolete("StateMachine is obsolete. Use UnityEditor.Animations.AnimatorStateMachine instead (UnityUpgradable) -> UnityEditor.Animations.AnimatorStateMachine", true)]
    public class StateMachine : UnityEngine.Object
    {
        public Transition AddAnyStateTransition(UnityEditorInternal.State dst) => 
            null;

        public UnityEditorInternal.State AddState(string stateName) => 
            null;

        public StateMachine AddStateMachine(string stateMachineName) => 
            null;

        public Transition AddTransition(UnityEditorInternal.State src, UnityEditorInternal.State dst) => 
            null;

        public UnityEditorInternal.State GetState(int index) => 
            null;

        public StateMachine GetStateMachine(int index) => 
            null;

        public Vector3 GetStateMachinePosition(int i) => 
            new Vector3();

        public Transition[] GetTransitionsFromState(UnityEditorInternal.State srcState) => 
            null;

        public Vector3 anyStatePosition
        {
            get => 
                new Vector3();
            set
            {
            }
        }

        public UnityEditorInternal.State defaultState
        {
            get => 
                null;
            set
            {
            }
        }

        public Vector3 parentStateMachinePosition
        {
            get => 
                new Vector3();
            set
            {
            }
        }
    }
}

