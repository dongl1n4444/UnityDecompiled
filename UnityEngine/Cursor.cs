namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Cursor API for setting the cursor (mouse pointer).</para>
    /// </summary>
    public sealed class Cursor
    {
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_SetCursor(Texture2D texture, ref Vector2 hotspot, CursorMode cursorMode);
        private static void SetCursor(Texture2D texture, CursorMode cursorMode)
        {
            SetCursor(texture, Vector2.zero, cursorMode);
        }

        /// <summary>
        /// <para>Specify a custom cursor that you wish to use as a cursor.</para>
        /// </summary>
        /// <param name="texture">The texture to use for the cursor or null to set the default cursor. Note that a texture needs to be imported with "Read/Write enabled" in the texture importer (or using the "Cursor" defaults), in order to be used as a cursor.</param>
        /// <param name="hotspot">The offset from the top left of the texture to use as the target point (must be within the bounds of the cursor).</param>
        /// <param name="cursorMode">Allow this cursor to render as a hardware cursor on supported platforms, or force software cursor.</param>
        public static void SetCursor(Texture2D texture, Vector2 hotspot, CursorMode cursorMode)
        {
            INTERNAL_CALL_SetCursor(texture, ref hotspot, cursorMode);
        }

        /// <summary>
        /// <para>Determines whether the hardware pointer is locked to the center of the view, constrained to the window, or not constrained at all.</para>
        /// </summary>
        public static CursorLockMode lockState { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Determines whether the hardware pointer is visible or not.</para>
        /// </summary>
        public static bool visible { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}

