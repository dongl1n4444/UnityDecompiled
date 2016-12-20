namespace UnityEditor
{
    using System;

    /// <summary>
    /// <para>AnimationClip mask options for ModelImporterClipAnimation.</para>
    /// </summary>
    public enum ClipAnimationMaskType
    {
        /// <summary>
        /// <para>Use a mask from your project to specify which transforms animation should be imported.</para>
        /// </summary>
        CopyFromOther = 1,
        /// <summary>
        /// <para>A mask containing all the transform in the file will be created internally.</para>
        /// </summary>
        CreateFromThisModel = 0,
        /// <summary>
        /// <para>No Mask. All the animation will be imported.</para>
        /// </summary>
        None = 3
    }
}

