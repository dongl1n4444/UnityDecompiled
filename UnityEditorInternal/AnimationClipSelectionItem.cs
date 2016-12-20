namespace UnityEditorInternal
{
    using System;
    using UnityEngine;

    internal class AnimationClipSelectionItem : AnimationWindowSelectionItem
    {
        public static AnimationClipSelectionItem Create(AnimationClip animationClip, Object sourceObject)
        {
            AnimationClipSelectionItem item = ScriptableObject.CreateInstance(typeof(AnimationClipSelectionItem)) as AnimationClipSelectionItem;
            item.hideFlags = HideFlags.HideAndDontSave;
            item.gameObject = sourceObject as GameObject;
            item.scriptableObject = sourceObject as ScriptableObject;
            item.animationClip = animationClip;
            item.timeOffset = 0f;
            item.id = 0;
            return item;
        }

        public override bool canChangeAnimationClip
        {
            get
            {
                return false;
            }
        }

        public override bool canRecord
        {
            get
            {
                return false;
            }
        }

        public override bool canSyncSceneSelection
        {
            get
            {
                return false;
            }
        }
    }
}

