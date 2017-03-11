namespace UnityEngine
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Utility class for making new GUI controls.</para>
    /// </summary>
    public class GUIUtility
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static bool <guiIsExiting>k__BackingField;
        internal static Vector2 s_EditorScreenPointOffset = Vector2.zero;
        internal static int s_OriginalID;
        internal static int s_SkinMode;

        [RequiredByNativeCode]
        internal static void BeginGUI(int skinMode, int instanceID, int useGUILayout)
        {
            s_SkinMode = skinMode;
            s_OriginalID = instanceID;
            GUI.skin = null;
            guiIsExiting = false;
            if (useGUILayout != 0)
            {
                GUILayoutUtility.Begin(instanceID);
            }
            GUI.changed = false;
        }

        internal static void CheckOnGUI()
        {
            if (Internal_GetGUIDepth() <= 0)
            {
                throw new ArgumentException("You can only call GUI functions from inside OnGUI.");
            }
        }

        internal static void CleanupRoots()
        {
        }

        [RequiredByNativeCode]
        internal static bool EndContainerGUIFromException(Exception exception) => 
            ShouldRethrowException(exception);

        [RequiredByNativeCode]
        internal static void EndGUI(int layoutType)
        {
            try
            {
                if ((Event.current.type == EventType.Layout) && (layoutType != 0))
                {
                    if (layoutType == 1)
                    {
                        goto Label_0031;
                    }
                    if (layoutType == 2)
                    {
                        goto Label_003B;
                    }
                }
                goto Label_0046;
            Label_0031:
                GUILayoutUtility.Layout();
                goto Label_0046;
            Label_003B:
                GUILayoutUtility.LayoutFromEditorWindow();
            Label_0046:
                GUILayoutUtility.SelectIDList(s_OriginalID, false);
                GUIContent.ClearStaticCache();
            }
            finally
            {
                Internal_ExitGUI();
            }
        }

        [RequiredByNativeCode]
        internal static bool EndGUIFromException(Exception exception)
        {
            Internal_ExitGUI();
            return ShouldRethrowException(exception);
        }

        public static void ExitGUI()
        {
            guiIsExiting = true;
            throw new ExitGUIException();
        }

        internal static GUISkin GetBuiltinSkin(int skin) => 
            (Internal_GetBuiltinSkin(skin) as GUISkin);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern bool GetChanged();
        /// <summary>
        /// <para>Get a unique ID for a control.</para>
        /// </summary>
        /// <param name="focus"></param>
        /// <param name="position"></param>
        public static int GetControlID(FocusType focus) => 
            GetControlID(0, focus);

        /// <summary>
        /// <para>Get a unique ID for a control, using an integer as a hint to help ensure correct matching of IDs to controls.</para>
        /// </summary>
        /// <param name="hint"></param>
        /// <param name="focus"></param>
        /// <param name="position"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern int GetControlID(int hint, FocusType focus);
        /// <summary>
        /// <para>Get a unique ID for a control.</para>
        /// </summary>
        /// <param name="focus"></param>
        /// <param name="position"></param>
        public static int GetControlID(FocusType focus, Rect position) => 
            Internal_GetNextControlID2(0, focus, position);

        /// <summary>
        /// <para>Get a unique ID for a control, using a the label content as a hint to help ensure correct matching of IDs to controls.</para>
        /// </summary>
        /// <param name="contents"></param>
        /// <param name="focus"></param>
        /// <param name="position"></param>
        public static int GetControlID(GUIContent contents, FocusType focus) => 
            GetControlID(contents.hash, focus);

        /// <summary>
        /// <para>Get a unique ID for a control, using an integer as a hint to help ensure correct matching of IDs to controls.</para>
        /// </summary>
        /// <param name="hint"></param>
        /// <param name="focus"></param>
        /// <param name="position"></param>
        public static int GetControlID(int hint, FocusType focus, Rect position) => 
            Internal_GetNextControlID2(hint, focus, position);

        /// <summary>
        /// <para>Get a unique ID for a control, using a the label content as a hint to help ensure correct matching of IDs to controls.</para>
        /// </summary>
        /// <param name="contents"></param>
        /// <param name="focus"></param>
        /// <param name="position"></param>
        public static int GetControlID(GUIContent contents, FocusType focus, Rect position) => 
            Internal_GetNextControlID2(contents.hash, focus, position);

        internal static GUISkin GetDefaultSkin() => 
            Internal_GetDefaultSkin(s_SkinMode);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern int GetPermanentControlID();
        /// <summary>
        /// <para>Get a state object from a controlID.</para>
        /// </summary>
        /// <param name="t"></param>
        /// <param name="controlID"></param>
        public static object GetStateObject(System.Type t, int controlID) => 
            GUIStateObjects.GetStateObject(t, controlID);

        /// <summary>
        /// <para>Convert a point from GUI position to screen space.</para>
        /// </summary>
        /// <param name="guiPoint"></param>
        public static Vector2 GUIToScreenPoint(Vector2 guiPoint) => 
            (GUIClip.Unclip(guiPoint) + s_EditorScreenPointOffset);

        internal static Rect GUIToScreenRect(Rect guiRect)
        {
            Vector2 vector = GUIToScreenPoint(new Vector2(guiRect.x, guiRect.y));
            guiRect.x = vector.x;
            guiRect.y = vector.y;
            return guiRect;
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void Internal_BeginContainer(int instanceID);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern int INTERNAL_CALL_Internal_GetNextControlID2(int hint, FocusType focusType, ref Rect rect);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void Internal_EndContainer();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void Internal_ExitGUI();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern UnityEngine.Object Internal_GetBuiltinSkin(int skin);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern GUISkin Internal_GetDefaultSkin(int skinMode);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern int Internal_GetGUIDepth();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern int Internal_GetHotControl();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern int Internal_GetKeyboardControl();
        private static int Internal_GetNextControlID2(int hint, FocusType focusType, Rect rect) => 
            INTERNAL_CALL_Internal_GetNextControlID2(hint, focusType, ref rect);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern float Internal_GetPixelsPerPoint();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void Internal_SetHotControl(int value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void Internal_SetKeyboardControl(int value);
        [RequiredByNativeCode]
        internal static bool ProcessEvent(int instanceID, IntPtr nativeEventPtr) => 
            false;

        /// <summary>
        /// <para>Get an existing state object from a controlID.</para>
        /// </summary>
        /// <param name="t"></param>
        /// <param name="controlID"></param>
        public static object QueryStateObject(System.Type t, int controlID) => 
            GUIStateObjects.QueryStateObject(t, controlID);

        /// <summary>
        /// <para>Helper function to rotate the GUI around a point.</para>
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="pivotPoint"></param>
        public static void RotateAroundPivot(float angle, Vector2 pivotPoint)
        {
            Matrix4x4 matrix = GUI.matrix;
            GUI.matrix = Matrix4x4.identity;
            Vector2 vector = GUIClip.Unclip(pivotPoint);
            Matrix4x4 matrixx2 = Matrix4x4.TRS((Vector3) vector, Quaternion.Euler(0f, 0f, angle), Vector3.one) * Matrix4x4.TRS((Vector3) -vector, Quaternion.identity, Vector3.one);
            GUI.matrix = matrixx2 * matrix;
        }

        /// <summary>
        /// <para>Helper function to scale the GUI around a point.</para>
        /// </summary>
        /// <param name="scale"></param>
        /// <param name="pivotPoint"></param>
        public static void ScaleAroundPivot(Vector2 scale, Vector2 pivotPoint)
        {
            Matrix4x4 matrix = GUI.matrix;
            Vector2 vector = GUIClip.Unclip(pivotPoint);
            Matrix4x4 matrixx2 = Matrix4x4.TRS((Vector3) vector, Quaternion.identity, new Vector3(scale.x, scale.y, 1f)) * Matrix4x4.TRS((Vector3) -vector, Quaternion.identity, Vector3.one);
            GUI.matrix = matrixx2 * matrix;
        }

        /// <summary>
        /// <para>Convert a point from screen space to GUI position.</para>
        /// </summary>
        /// <param name="screenPoint"></param>
        public static Vector2 ScreenToGUIPoint(Vector2 screenPoint) => 
            (GUIClip.Clip(screenPoint) - s_EditorScreenPointOffset);

        public static Rect ScreenToGUIRect(Rect screenRect)
        {
            Vector2 vector = ScreenToGUIPoint(new Vector2(screenRect.x, screenRect.y));
            screenRect.x = vector.x;
            screenRect.y = vector.y;
            return screenRect;
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void SetChanged(bool changed);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void SetDidGUIWindowsEatLastEvent(bool value);
        [RequiredByNativeCode]
        internal static void SetSkin(int skinMode)
        {
            s_SkinMode = skinMode;
            GUI.DoSetSkin(null);
        }

        internal static bool ShouldRethrowException(Exception exception)
        {
            while ((exception is TargetInvocationException) && (exception.InnerException != null))
            {
                exception = exception.InnerException;
            }
            return (exception is ExitGUIException);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void UpdateUndoName();

        internal static bool guiIsExiting
        {
            [CompilerGenerated]
            get => 
                <guiIsExiting>k__BackingField;
            [CompilerGenerated]
            set
            {
                <guiIsExiting>k__BackingField = value;
            }
        }

        /// <summary>
        /// <para>A global property, which is true if a ModalWindow is being displayed, false otherwise.</para>
        /// </summary>
        public static bool hasModalWindow { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>The controlID of the current hot control.</para>
        /// </summary>
        public static int hotControl
        {
            get => 
                Internal_GetHotControl();
            set
            {
                Internal_SetHotControl(value);
            }
        }

        /// <summary>
        /// <para>The controlID of the control that has keyboard focus.</para>
        /// </summary>
        public static int keyboardControl
        {
            get => 
                Internal_GetKeyboardControl();
            set
            {
                Internal_SetKeyboardControl(value);
            }
        }

        internal static bool mouseUsed { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        internal static float pixelsPerPoint =>
            Internal_GetPixelsPerPoint();

        /// <summary>
        /// <para>Get access to the system-wide pasteboard.</para>
        /// </summary>
        public static string systemCopyBuffer { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        internal static bool textFieldInput { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}

