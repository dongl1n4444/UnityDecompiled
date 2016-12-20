namespace UnityEditor.Graphs.AnimationStateMachine
{
    using System;
    using UnityEditor;
    using UnityEditor.Animations;
    using UnityEngine;

    internal class InvalidStateMachineBehaviour : ScriptableObject
    {
        [HideInInspector]
        public int behaviourIndex;
        [HideInInspector]
        public AnimatorController controller;
        [HideInInspector]
        public int layerIndex;
        public MonoScript monoScript = null;
        [HideInInspector]
        public AnimatorState state;
        [HideInInspector]
        public AnimatorStateMachine stateMachine;
    }
}

