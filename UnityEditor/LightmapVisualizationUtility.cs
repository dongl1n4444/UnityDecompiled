namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting;
    using UnityEngineInternal;

    internal sealed class LightmapVisualizationUtility
    {
        public static void DrawTextureWithUVOverlay(Texture2D texture, GameObject gameObject, Rect drawableArea, Rect position, GITextureType textureType)
        {
            INTERNAL_CALL_DrawTextureWithUVOverlay(texture, gameObject, ref drawableArea, ref position, textureType);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern Texture2D GetGITexture(GITextureType textureType);
        public static Vector4 GetLightmapTilingOffset(LightmapType lightmapType)
        {
            Vector4 vector;
            INTERNAL_CALL_GetLightmapTilingOffset(lightmapType, out vector);
            return vector;
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_DrawTextureWithUVOverlay(Texture2D texture, GameObject gameObject, ref Rect drawableArea, ref Rect position, GITextureType textureType);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetLightmapTilingOffset(LightmapType lightmapType, out Vector4 value);
    }
}

