namespace UnityEditorInternal
{
    using System;
    using UnityEditor;

    internal interface IAnimationContextualResponder
    {
        void AddAnimatedKeys();
        void AddCandidateKeys();
        void AddKey(SerializedProperty property);
        bool CandidateExists(SerializedProperty property);
        bool CurveExists(SerializedProperty property);
        void GoToNextKeyframe(SerializedProperty property);
        void GoToPreviousKeyframe(SerializedProperty property);
        bool HasAnyCandidates();
        bool HasAnyCurves();
        bool IsAnimatable(SerializedProperty property);
        bool IsEditable(SerializedProperty property);
        bool KeyExists(SerializedProperty property);
        void RemoveCurve(SerializedProperty property);
        void RemoveKey(SerializedProperty property);
    }
}

