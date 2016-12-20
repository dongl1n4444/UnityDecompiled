namespace UnityEngine
{
    using System;

    /// <summary>
    /// <para>Culling mode for the Animator.</para>
    /// </summary>
    public enum AnimatorCullingMode
    {
        /// <summary>
        /// <para>Always animate the entire character. Object is animated even when offscreen.</para>
        /// </summary>
        AlwaysAnimate = 0,
        [Obsolete("Enum member AnimatorCullingMode.BasedOnRenderers has been deprecated. Use AnimatorCullingMode.CullUpdateTransforms instead (UnityUpgradable) -> CullUpdateTransforms", true)]
        BasedOnRenderers = 1,
        /// <summary>
        /// <para>Animation is completely disabled when renderers are not visible.</para>
        /// </summary>
        CullCompletely = 2,
        /// <summary>
        /// <para>Retarget, IK and write of Transforms are disabled when renderers are not visible.</para>
        /// </summary>
        CullUpdateTransforms = 1
    }
}

