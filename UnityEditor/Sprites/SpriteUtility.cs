namespace UnityEditor.Sprites
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    /// <summary>
    /// <para>Helper utilities for accessing Sprite data.</para>
    /// </summary>
    public sealed class SpriteUtility
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void CreateSpritePolygonAssetAtPath(string pathName, int sides);
        internal static void GenerateOutline(Texture2D texture, Rect rect, float detail, byte alphaTolerance, bool holeDetection, out Vector2[][] paths)
        {
            INTERNAL_CALL_GenerateOutline(texture, ref rect, detail, alphaTolerance, holeDetection, out paths);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void GenerateOutlineFromSprite(Sprite sprite, float detail, byte alphaTolerance, bool holeDetection, out Vector2[][] paths);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern Vector2[] GeneratePolygonOutlineVerticesOfSize(int sides, int width, int height);
        /// <summary>
        /// <para>Returns the generated Sprite mesh indices.</para>
        /// </summary>
        /// <param name="sprite">If Sprite is packed, it is possible to access data as if it was on the atlas texture.</param>
        /// <param name="getAtlasData"></param>
        [Obsolete("Use Sprite.triangles API instead. This data is the same for packed and unpacked sprites.")]
        public static ushort[] GetSpriteIndices(Sprite sprite, bool getAtlasData)
        {
            return sprite.triangles;
        }

        /// <summary>
        /// <para>Returns the generated Sprite mesh positions.</para>
        /// </summary>
        /// <param name="getAtlasData">If Sprite is packed, it is possible to access data as if it was on the atlas texture.</param>
        /// <param name="sprite"></param>
        [Obsolete("Use Sprite.vertices API instead. This data is the same for packed and unpacked sprites.")]
        public static Vector2[] GetSpriteMesh(Sprite sprite, bool getAtlasData)
        {
            return sprite.vertices;
        }

        /// <summary>
        /// <para>Returns the generated Sprite texture. If Sprite is packed, it is possible to query for both source and atlas textures.</para>
        /// </summary>
        /// <param name="getAtlasData">If Sprite is packed, it is possible to access data as if it was on the atlas texture.</param>
        /// <param name="sprite"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern Texture2D GetSpriteTexture(Sprite sprite, bool getAtlasData);
        /// <summary>
        /// <para>Returns the generated Sprite mesh uvs.</para>
        /// </summary>
        /// <param name="sprite">If Sprite is packed, it is possible to access data as if it was on the atlas texture.</param>
        /// <param name="getAtlasData"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern Vector2[] GetSpriteUVs(Sprite sprite, bool getAtlasData);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_GenerateOutline(Texture2D texture, ref Rect rect, float detail, byte alphaTolerance, bool holeDetection, out Vector2[][] paths);
    }
}

