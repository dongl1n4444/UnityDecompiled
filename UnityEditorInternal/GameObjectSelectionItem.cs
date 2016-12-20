namespace UnityEditorInternal
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEngine;

    internal class GameObjectSelectionItem : AnimationWindowSelectionItem
    {
        public static GameObjectSelectionItem Create(GameObject gameObject)
        {
            <Create>c__AnonStorey0 storey = new <Create>c__AnonStorey0 {
                selectionItem = ScriptableObject.CreateInstance(typeof(GameObjectSelectionItem)) as GameObjectSelectionItem
            };
            storey.selectionItem.hideFlags = HideFlags.HideAndDontSave;
            storey.selectionItem.gameObject = gameObject;
            storey.selectionItem.animationClip = null;
            storey.selectionItem.timeOffset = 0f;
            storey.selectionItem.id = 0;
            if (storey.selectionItem.rootGameObject != null)
            {
                AnimationClip[] animationClips = AnimationUtility.GetAnimationClips(storey.selectionItem.rootGameObject);
                if ((storey.selectionItem.animationClip == null) && (storey.selectionItem.gameObject != null))
                {
                    storey.selectionItem.animationClip = (animationClips.Length <= 0) ? null : animationClips[0];
                }
                else if (!Array.Exists<AnimationClip>(animationClips, new Predicate<AnimationClip>(storey.<>m__0)))
                {
                    storey.selectionItem.animationClip = (animationClips.Length <= 0) ? null : animationClips[0];
                }
            }
            return storey.selectionItem;
        }

        public override void Synchronize()
        {
            if (this.rootGameObject != null)
            {
                AnimationClip[] animationClips = AnimationUtility.GetAnimationClips(this.rootGameObject);
                if (animationClips.Length > 0)
                {
                    if (!Array.Exists<AnimationClip>(animationClips, x => x == this.animationClip))
                    {
                        this.animationClip = animationClips[0];
                    }
                }
                else
                {
                    this.animationClip = null;
                }
            }
        }

        public override AnimationClip animationClip
        {
            get
            {
                if (this.animationPlayer == null)
                {
                    return null;
                }
                return base.animationClip;
            }
            set
            {
                base.animationClip = value;
            }
        }

        [CompilerGenerated]
        private sealed class <Create>c__AnonStorey0
        {
            internal GameObjectSelectionItem selectionItem;

            internal bool <>m__0(AnimationClip x)
            {
                return (x == this.selectionItem.animationClip);
            }
        }
    }
}

