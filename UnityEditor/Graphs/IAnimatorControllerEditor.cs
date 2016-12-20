namespace UnityEditor.Graphs
{
    using System;
    using UnityEditor.Animations;
    using UnityEngine;

    internal interface IAnimatorControllerEditor
    {
        void Repaint();
        void ResetUI();

        AnimatorController animatorController { get; set; }

        bool liveLink { get; }

        Animator previewAnimator { get; }
    }
}

