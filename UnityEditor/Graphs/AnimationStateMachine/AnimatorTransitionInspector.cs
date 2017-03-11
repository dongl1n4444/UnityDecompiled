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
            EditorGUILayout.HelpBox("Entry transitions (displayed in grey) are not previewable. To preview a transition please select a State transition (displayed in white).", MessageType.Info);
        }
    }
}

