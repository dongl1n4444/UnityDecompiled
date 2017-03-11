namespace UnityEditorInternal
{
    using System;

    public enum BatchBreakingReason
    {
        CanvasInjectionIndex = 2,
        DifferentA8TextureUsage = 0x20,
        DifferentClipRect = 0x40,
        DifferentMaterialInstance = 4,
        DifferentRectClipping = 8,
        DifferentTexture = 0x10,
        NoBreaking = 0,
        NotCoplanarWithCanvas = 1,
        Unknown = 0x80
    }
}

