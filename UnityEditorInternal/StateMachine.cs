namespace UnityEditorInternal
{
    using System;
    using UnityEngine;

    [Obsolete("StateMachine is obsolete. Use UnityEditor.Animations.AnimatorStateMachine instead (UnityUpgradable) -> UnityEditor.Animations.AnimatorStateMachine", true)]
    public class StateMachine : UnityEngine.Object
    {
        public Transition AddAnyStateTransition(UnityEditorInternal.State dst)
        {
            return null;
        }

        public UnityEditorInternal.State AddState(string stateName)
        {
            return null;
        }

        public StateMachine AddStateMachine(string stateMachineName)
        {
            return null;
        }

        public Transition AddTransition(UnityEditorInternal.State src, UnityEditorInternal.State dst)
        {
            return null;
        }

        public UnityEditorInternal.State GetState(int index)
        {
            return null;
        }

        public StateMachine GetStateMachine(int index)
        {
            return null;
        }

        public Vector3 GetStateMachinePosition(int i)
        {
            return new Vector3();
        }

        public Transition[] GetTransitionsFromState(UnityEditorInternal.State srcState)
        {
            return null;
        }

        public Vector3 anyStatePosition
        {
            get
            {
                return new Vector3();
            }
            set
            {
            }
        }

        public UnityEditorInternal.State defaultState
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        public Vector3 parentStateMachinePosition
        {
            get
            {
                return new Vector3();
            }
            set
            {
            }
        }
    }
}

