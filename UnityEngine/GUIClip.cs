namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    internal sealed class GUIClip
    {
        public static Rect Clip(Rect absoluteRect)
        {
            Internal_Clip_Rect(ref absoluteRect);
            return absoluteRect;
        }

        public static Vector2 Clip(Vector2 absolutePos)
        {
            Clip_Vector2(ref absolutePos);
            return absolutePos;
        }

        private static void Clip_Vector2(ref Vector2 absolutePos)
        {
            INTERNAL_CALL_Clip_Vector2(ref absolutePos);
        }

        public static Vector2 GetAbsoluteMousePosition()
        {
            Vector2 vector;
            Internal_GetAbsoluteMousePosition(out vector);
            return vector;
        }

        internal static Matrix4x4 GetMatrix()
        {
            Matrix4x4 matrixx;
            INTERNAL_CALL_GetMatrix(out matrixx);
            return matrixx;
        }

        internal static Rect GetTopRect()
        {
            Rect rect;
            INTERNAL_CALL_GetTopRect(out rect);
            return rect;
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_Clip_Vector2(ref Vector2 absolutePos);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetMatrix(out Matrix4x4 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetTopRect(out Rect value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_Internal_Clip_Rect(ref Rect absoluteRect);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_Internal_Push(ref Rect screenRect, ref Vector2 scrollOffset, ref Vector2 renderOffset, bool resetOffset);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_SetMatrix(ref Matrix4x4 m);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_SetTransform(ref Matrix4x4 clipTransform, ref Matrix4x4 objectTransform, ref Rect clipRect);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_Unclip_Rect(ref Rect rect);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_Unclip_Vector2(ref Vector2 pos);
        private static void Internal_Clip_Rect(ref Rect absoluteRect)
        {
            INTERNAL_CALL_Internal_Clip_Rect(ref absoluteRect);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_get_topmostRect(out Rect value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_get_visibleRect(out Rect value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void Internal_GetAbsoluteMousePosition(out Vector2 output);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void Internal_Pop();
        internal static void Internal_Push(Rect screenRect, Vector2 scrollOffset, Vector2 renderOffset, bool resetOffset)
        {
            INTERNAL_CALL_Internal_Push(ref screenRect, ref scrollOffset, ref renderOffset, resetOffset);
        }

        internal static void Pop()
        {
            Internal_Pop();
        }

        internal static void Push(Rect screenRect, Vector2 scrollOffset, Vector2 renderOffset, bool resetOffset)
        {
            Internal_Push(screenRect, scrollOffset, renderOffset, resetOffset);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void Reapply();
        internal static void SetMatrix(Matrix4x4 m)
        {
            INTERNAL_CALL_SetMatrix(ref m);
        }

        internal static void SetTransform(Matrix4x4 clipTransform, Matrix4x4 objectTransform, Rect clipRect)
        {
            INTERNAL_CALL_SetTransform(ref clipTransform, ref objectTransform, ref clipRect);
        }

        public static Rect Unclip(Rect rect)
        {
            Unclip_Rect(ref rect);
            return rect;
        }

        public static Vector2 Unclip(Vector2 pos)
        {
            Unclip_Vector2(ref pos);
            return pos;
        }

        private static void Unclip_Rect(ref Rect rect)
        {
            INTERNAL_CALL_Unclip_Rect(ref rect);
        }

        private static void Unclip_Vector2(ref Vector2 pos)
        {
            INTERNAL_CALL_Unclip_Vector2(ref pos);
        }

        public static bool enabled { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        public static Rect topmostRect
        {
            get
            {
                Rect rect;
                INTERNAL_get_topmostRect(out rect);
                return rect;
            }
        }

        public static Rect visibleRect
        {
            get
            {
                Rect rect;
                INTERNAL_get_visibleRect(out rect);
                return rect;
            }
        }
    }
}

