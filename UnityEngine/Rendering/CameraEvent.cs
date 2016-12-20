namespace UnityEngine.Rendering
{
    using System;

    /// <summary>
    /// <para>Defines a place in camera's rendering to attach Rendering.CommandBuffer objects to.</para>
    /// </summary>
    public enum CameraEvent
    {
        BeforeDepthTexture,
        AfterDepthTexture,
        BeforeDepthNormalsTexture,
        AfterDepthNormalsTexture,
        BeforeGBuffer,
        AfterGBuffer,
        BeforeLighting,
        AfterLighting,
        BeforeFinalPass,
        AfterFinalPass,
        BeforeForwardOpaque,
        AfterForwardOpaque,
        BeforeImageEffectsOpaque,
        AfterImageEffectsOpaque,
        BeforeSkybox,
        AfterSkybox,
        BeforeForwardAlpha,
        AfterForwardAlpha,
        BeforeImageEffects,
        AfterImageEffects,
        AfterEverything,
        BeforeReflections,
        AfterReflections
    }
}

