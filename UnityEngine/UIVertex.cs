namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Vertex class used by a Canvas for managing vertices.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), UsedByNativeCode]
    public struct UIVertex
    {
        /// <summary>
        /// <para>Vertex position.</para>
        /// </summary>
        public Vector3 position;
        /// <summary>
        /// <para>Normal.</para>
        /// </summary>
        public Vector3 normal;
        /// <summary>
        /// <para>Vertex color.</para>
        /// </summary>
        public Color32 color;
        /// <summary>
        /// <para>UV0.</para>
        /// </summary>
        public Vector2 uv0;
        /// <summary>
        /// <para>UV1.</para>
        /// </summary>
        public Vector2 uv1;
        /// <summary>
        /// <para>Tangent.</para>
        /// </summary>
        public Vector4 tangent;
        private static readonly Color32 s_DefaultColor;
        private static readonly Vector4 s_DefaultTangent;
        /// <summary>
        /// <para>Simple UIVertex with sensible settings for use in the UI system.</para>
        /// </summary>
        public static UIVertex simpleVert;
        static UIVertex()
        {
            s_DefaultColor = new Color32(0xff, 0xff, 0xff, 0xff);
            s_DefaultTangent = new Vector4(1f, 0f, 0f, -1f);
            UIVertex vertex = new UIVertex {
                position = Vector3.zero,
                normal = Vector3.back,
                tangent = s_DefaultTangent,
                color = s_DefaultColor,
                uv0 = Vector2.zero,
                uv1 = Vector2.zero
            };
            simpleVert = vertex;
        }
    }
}

