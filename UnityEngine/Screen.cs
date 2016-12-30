namespace UnityEngine
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine.Internal;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Access to display information.</para>
    /// </summary>
    public sealed class Screen
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static bool <showCursor>k__BackingField;

        /// <summary>
        /// <para>Switches the screen resolution.</para>
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="fullscreen"></param>
        /// <param name="preferredRefreshRate"></param>
        [ExcludeFromDocs]
        public static void SetResolution(int width, int height, bool fullscreen)
        {
            int preferredRefreshRate = 0;
            SetResolution(width, height, fullscreen, preferredRefreshRate);
        }

        /// <summary>
        /// <para>Switches the screen resolution.</para>
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="fullscreen"></param>
        /// <param name="preferredRefreshRate"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void SetResolution(int width, int height, bool fullscreen, [UnityEngine.Internal.DefaultValue("0")] int preferredRefreshRate);

        /// <summary>
        /// <para>Allow auto-rotation to landscape left?</para>
        /// </summary>
        public static bool autorotateToLandscapeLeft { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Allow auto-rotation to landscape right?</para>
        /// </summary>
        public static bool autorotateToLandscapeRight { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Allow auto-rotation to portrait?</para>
        /// </summary>
        public static bool autorotateToPortrait { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Allow auto-rotation to portrait, upside down?</para>
        /// </summary>
        public static bool autorotateToPortraitUpsideDown { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The current screen resolution (Read Only).</para>
        /// </summary>
        public static Resolution currentResolution { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>The current DPI of the screen / device (Read Only).</para>
        /// </summary>
        public static float dpi { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Is the game running fullscreen?</para>
        /// </summary>
        public static bool fullScreen { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        [Obsolete("Property GetResolution has been deprecated. Use resolutions instead (UnityUpgradable) -> resolutions", true), EditorBrowsable(EditorBrowsableState.Never)]
        public static Resolution[] GetResolution =>
            null;

        /// <summary>
        /// <para>The current height of the screen window in pixels (Read Only).</para>
        /// </summary>
        [ThreadAndSerializationSafe]
        public static int height { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Should the cursor be locked?</para>
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Use Cursor.lockState and Cursor.visible instead.", false)]
        public static bool lockCursor
        {
            get => 
                (CursorLockMode.Locked == Cursor.lockState);
            set
            {
                if (value)
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
            }
        }

        /// <summary>
        /// <para>Specifies logical orientation of the screen.</para>
        /// </summary>
        public static ScreenOrientation orientation { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>All fullscreen resolutions supported by the monitor (Read Only).</para>
        /// </summary>
        public static Resolution[] resolutions { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Should the cursor be visible?</para>
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Property showCursor has been deprecated. Use Cursor.visible instead (UnityUpgradable) -> UnityEngine.Cursor.visible", true)]
        public static bool showCursor
        {
            [CompilerGenerated]
            get => 
                <showCursor>k__BackingField;
            [CompilerGenerated]
            set
            {
                <showCursor>k__BackingField = value;
            }
        }

        /// <summary>
        /// <para>A power saving setting, allowing the screen to dim some time after the last active user interaction.</para>
        /// </summary>
        public static int sleepTimeout { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The current width of the screen window in pixels (Read Only).</para>
        /// </summary>
        [ThreadAndSerializationSafe]
        public static int width { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }
    }
}

