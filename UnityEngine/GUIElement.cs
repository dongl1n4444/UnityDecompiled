namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Internal;

    /// <summary>
    /// <para>Base class for images &amp; text strings displayed in a GUI.</para>
    /// </summary>
    public class GUIElement : Behaviour
    {
        [ExcludeFromDocs]
        public Rect GetScreenRect()
        {
            Camera camera = null;
            Rect rect;
            INTERNAL_CALL_GetScreenRect(this, camera, out rect);
            return rect;
        }

        /// <summary>
        /// <para>Returns bounding rectangle of GUIElement in screen coordinates.</para>
        /// </summary>
        /// <param name="camera"></param>
        public Rect GetScreenRect([DefaultValue("null")] Camera camera)
        {
            Rect rect;
            INTERNAL_CALL_GetScreenRect(this, camera, out rect);
            return rect;
        }

        /// <summary>
        /// <para>Is a point on screen inside the element?</para>
        /// </summary>
        /// <param name="screenPosition"></param>
        /// <param name="camera"></param>
        [ExcludeFromDocs]
        public bool HitTest(Vector3 screenPosition)
        {
            Camera camera = null;
            return INTERNAL_CALL_HitTest(this, ref screenPosition, camera);
        }

        /// <summary>
        /// <para>Is a point on screen inside the element?</para>
        /// </summary>
        /// <param name="screenPosition"></param>
        /// <param name="camera"></param>
        public bool HitTest(Vector3 screenPosition, [DefaultValue("null")] Camera camera) => 
            INTERNAL_CALL_HitTest(this, ref screenPosition, camera);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_GetScreenRect(GUIElement self, Camera camera, out Rect value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool INTERNAL_CALL_HitTest(GUIElement self, ref Vector3 screenPosition, Camera camera);
    }
}

