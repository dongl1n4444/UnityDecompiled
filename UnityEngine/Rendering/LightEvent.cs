namespace UnityEngine.Rendering
{
    using System;

    /// <summary>
    /// <para>Defines a place in light's rendering to attach Rendering.CommandBuffer objects to.</para>
    /// </summary>
    public enum LightEvent
    {
        BeforeShadowMap,
        AfterShadowMap,
        BeforeScreenspaceMask,
        AfterScreenspaceMask
    }
}

