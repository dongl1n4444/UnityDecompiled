namespace UnityEngine
{
    using System;

    /// <summary>
    /// <para>This enum controlls culling of Animation component.</para>
    /// </summary>
    public enum AnimationCullingType
    {
        /// <summary>
        /// <para>Animation culling is disabled - object is animated even when offscreen.</para>
        /// </summary>
        AlwaysAnimate = 0,
        [Obsolete("Enum member AnimatorCullingMode.BasedOnClipBounds has been deprecated. Use AnimationCullingType.AlwaysAnimate or AnimationCullingType.BasedOnRenderers instead")]
        BasedOnClipBounds = 2,
        /// <summary>
        /// <para>Animation is disabled when renderers are not visible.</para>
        /// </summary>
        BasedOnRenderers = 1,
        [Obsolete("Enum member AnimatorCullingMode.BasedOnUserBounds has been deprecated. Use AnimationCullingType.AlwaysAnimate or AnimationCullingType.BasedOnRenderers instead")]
        BasedOnUserBounds = 3
    }
}

