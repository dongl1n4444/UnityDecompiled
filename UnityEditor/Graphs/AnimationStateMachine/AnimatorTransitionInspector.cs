namespace UnityEditor.Graphs.AnimationStateMachine
{
    using System;
    using UnityEditor;
    using UnityEditor.Animations;

    [CustomEditor(typeof(AnimatorTransition)), CanEditMultipleObjects]
    internal class AnimatorTransitionInspector : AnimatorTransitionInspectorBase
    {
        protected override void DoPreview()
        {
            EditorGUILayout.HelpBox("StateMachine Transitions (displayed in grey) are not previewable. To preview a transition please select a State Transition (displayed in white)", MessageType.Info);
        }
    }
}

