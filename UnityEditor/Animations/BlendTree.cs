namespace UnityEditor.Animations
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Blend trees are used to blend continuously animation between their childs. They can either be 1D or 2D.</para>
    /// </summary>
    public sealed class BlendTree : Motion
    {
        public BlendTree()
        {
            Internal_Create(this);
        }

        /// <summary>
        /// <para>Utility function to add a child motion to a blend trees.</para>
        /// </summary>
        /// <param name="motion">The motion to add as child.</param>
        /// <param name="position">The position of the child. When using 2D blend trees.</param>
        /// <param name="threshold">The threshold of the child. When using 1D blend trees.</param>
        public void AddChild(Motion motion)
        {
            this.AddChild(motion, Vector2.zero, 0f);
        }

        /// <summary>
        /// <para>Utility function to add a child motion to a blend trees.</para>
        /// </summary>
        /// <param name="motion">The motion to add as child.</param>
        /// <param name="position">The position of the child. When using 2D blend trees.</param>
        /// <param name="threshold">The threshold of the child. When using 1D blend trees.</param>
        public void AddChild(Motion motion, float threshold)
        {
            this.AddChild(motion, Vector2.zero, threshold);
        }

        /// <summary>
        /// <para>Utility function to add a child motion to a blend trees.</para>
        /// </summary>
        /// <param name="motion">The motion to add as child.</param>
        /// <param name="position">The position of the child. When using 2D blend trees.</param>
        /// <param name="threshold">The threshold of the child. When using 1D blend trees.</param>
        public void AddChild(Motion motion, Vector2 position)
        {
            this.AddChild(motion, position, 0f);
        }

        internal void AddChild(Motion motion, Vector2 position, float threshold)
        {
            Undo.RecordObject(this, "Added BlendTree Child");
            ChildMotion[] children = this.children;
            ChildMotion item = new ChildMotion {
                timeScale = 1f,
                motion = motion,
                position = position,
                threshold = threshold,
                directBlendParameter = "Blend"
            };
            ArrayUtility.Add<ChildMotion>(ref children, item);
            this.children = children;
        }

        /// <summary>
        /// <para>Utility function to add a child blend tree to a blend tree.</para>
        /// </summary>
        /// <param name="position">The position of the child. When using 2D blend trees.</param>
        /// <param name="threshold">The threshold of the child. When using 1D blend trees.</param>
        public BlendTree CreateBlendTreeChild(float threshold) => 
            this.CreateBlendTreeChild(Vector2.zero, threshold);

        /// <summary>
        /// <para>Utility function to add a child blend tree to a blend tree.</para>
        /// </summary>
        /// <param name="position">The position of the child. When using 2D blend trees.</param>
        /// <param name="threshold">The threshold of the child. When using 1D blend trees.</param>
        public BlendTree CreateBlendTreeChild(Vector2 position) => 
            this.CreateBlendTreeChild(position, 0f);

        internal BlendTree CreateBlendTreeChild(Vector2 position, float threshold)
        {
            Undo.RecordObject(this, "Created BlendTree Child");
            BlendTree objectToAdd = new BlendTree {
                name = "BlendTree",
                hideFlags = HideFlags.HideInHierarchy
            };
            if (AssetDatabase.GetAssetPath(this) != "")
            {
                AssetDatabase.AddObjectToAsset(objectToAdd, AssetDatabase.GetAssetPath(this));
            }
            this.AddChild(objectToAdd, position, threshold);
            return objectToAdd;
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern AnimationClip[] GetAnimationClipsFlattened();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern int GetChildCount();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern Motion GetChildMotion(int index);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern string GetDirectBlendTreeParameter(int index);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern float GetInputBlendValue(string blendValueName);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern string GetRecursiveBlendParameter(int index);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern float GetRecursiveBlendParameterMax(int index);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern float GetRecursiveBlendParameterMin(int index);
        internal bool HasChild(BlendTree childTree, bool recursive)
        {
            foreach (ChildMotion motion in this.children)
            {
                if (motion.motion == childTree)
                {
                    return true;
                }
                if ((recursive && (motion.motion is BlendTree)) && (motion.motion as BlendTree).HasChild(childTree, true))
                {
                    return true;
                }
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void Internal_Create(BlendTree mono);
        /// <summary>
        /// <para>Utility function to remove the child of a blend tree.</para>
        /// </summary>
        /// <param name="index">The index of the blend tree to remove.</param>
        public void RemoveChild(int index)
        {
            Undo.RecordObject(this, "Remove Child");
            ChildMotion[] children = this.children;
            ArrayUtility.RemoveAt<ChildMotion>(ref children, index);
            this.children = children;
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern void SetDirectBlendTreeParameter(int index, string parameter);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern void SetInputBlendValue(string blendValueName, float value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern void SortChildren();

        /// <summary>
        /// <para>Parameter that is used to compute the blending weight of the childs in 1D blend trees or on the X axis of a 2D blend tree.</para>
        /// </summary>
        public string blendParameter { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Parameter that is used to compute the blending weight of the childs on the Y axis of a 2D blend tree.</para>
        /// </summary>
        public string blendParameterY { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The Blending type can be either 1D or different types of 2D.</para>
        /// </summary>
        public BlendTreeType blendType { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>A copy of the list of the blend tree child motions.</para>
        /// </summary>
        public ChildMotion[] children { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Sets the maximum threshold that will be used by the ChildMotion. Only used when useAutomaticThresholds is true.</para>
        /// </summary>
        public float maxThreshold { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Sets the minimum threshold that will be used by the ChildMotion. Only used when useAutomaticThresholds is true.</para>
        /// </summary>
        public float minThreshold { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        internal int recursiveBlendParameterCount { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>When active, the children's thresholds are automatically spread between 0 and 1.</para>
        /// </summary>
        public bool useAutomaticThresholds { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}

