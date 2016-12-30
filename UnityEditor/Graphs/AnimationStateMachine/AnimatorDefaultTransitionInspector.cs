namespace UnityEditor.Graphs.AnimationStateMachine
{
    using System;
    using UnityEditor;
    using UnityEditor.Animations;
    using UnityEngine;

    [CustomEditor(typeof(AnimatorDefaultTransition)), CanEditMultipleObjects]
    internal class AnimatorDefaultTransitionInspector : AnimatorTransitionInspectorBase
    {
        private AnimatorDefaultTransitionInspector()
        {
            base.showTransitionList = false;
        }

        internal override void OnHeaderTitleGUI(Rect titleRect, string header)
        {
            if (base.m_SerializedTransition != null)
            {
                Rect position = titleRect;
                position.height = 16f;
                string label = "Entry -> Default State ( " + base.m_TransitionContexts[base.m_TransitionList.index].ownerStateMachine.defaultState.name + " )";
                EditorGUI.LabelField(position, label);
                position.y += 18f;
                EditorGUI.LabelField(position, base.targets.Length + " " + ((base.targets.Length != 1) ? "Transitions" : "AnimatorTransitionBase"));
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.HelpBox("StateMachine Transitions (displayed in orange) are not previewable. To preview a transition please select a State Transition (displayed in white)", MessageType.Info);
        }
    }
}

