namespace UnityEditorInternal
{
    using System;
    using UnityEditor;
    using UnityEngine;

    internal interface IAnimationRecordingState
    {
        void AddPropertyModification(EditorCurveBinding binding, PropertyModification propertyModification, bool keepPrefabOverride);
        void SaveCurve(AnimationWindowCurve curve);

        EditorCurveBinding[] acceptedBindings { get; }

        AnimationClip activeAnimationClip { get; }

        GameObject activeGameObject { get; }

        GameObject activeRootGameObject { get; }

        bool addZeroFrame { get; }

        int currentFrame { get; }
    }
}

