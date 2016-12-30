namespace UnityEditorInternal
{
    using System;
    using UnityEngine;

    internal interface IAnimationRecordingState
    {
        void SaveCurve(AnimationWindowCurve curve);

        AnimationClip activeAnimationClip { get; }

        GameObject activeGameObject { get; }

        GameObject activeRootGameObject { get; }

        bool addZeroFrame { get; }

        int currentFrame { get; }
    }
}

