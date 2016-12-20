namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal interface ISelectionBinding
    {
        AnimationClip animationClip { get; }

        bool animationIsEditable { get; }

        bool clipIsEditable { get; }

        int id { get; }

        GameObject rootGameObject { get; }

        float timeOffset { get; }
    }
}

