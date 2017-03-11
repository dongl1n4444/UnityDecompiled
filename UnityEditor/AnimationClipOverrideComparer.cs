namespace UnityEditor
{
    using System;
    using System.Collections.Generic;

    internal class AnimationClipOverrideComparer : IComparer<KeyValuePair<AnimationClip, AnimationClip>>
    {
        public int Compare(KeyValuePair<AnimationClip, AnimationClip> x, KeyValuePair<AnimationClip, AnimationClip> y) => 
            string.Compare(x.Key.name, y.Key.name, StringComparison.OrdinalIgnoreCase);
    }
}

