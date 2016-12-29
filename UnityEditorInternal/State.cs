namespace UnityEditorInternal
{
    using System;
    using UnityEngine;

    [Obsolete("State is obsolete. Use UnityEditor.Animations.AnimatorState instead (UnityUpgradable) -> UnityEditor.Animations.AnimatorState", true)]
    public class State : UnityEngine.Object
    {
        public BlendTree CreateBlendTree() => 
            null;

        public BlendTree CreateBlendTree(AnimatorControllerLayer layer) => 
            null;

        public Motion GetMotion() => 
            null;

        public Motion GetMotion(AnimatorControllerLayer layer) => 
            null;

        public bool iKOnFeet
        {
            get => 
                false;
            set
            {
            }
        }

        public bool mirror
        {
            get => 
                false;
            set
            {
            }
        }

        public float speed
        {
            get => 
                -1f;
            set
            {
            }
        }

        public string tag
        {
            get => 
                string.Empty;
            set
            {
            }
        }

        public string uniqueName =>
            string.Empty;

        public int uniqueNameHash =>
            -1;
    }
}

