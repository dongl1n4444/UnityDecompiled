namespace UnityEditor
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditorInternal;
    using UnityEngine;

    /// <summary>
    /// <para>Use this class to highlight elements in the editor for use in in-editor tutorials and similar.</para>
    /// </summary>
    public sealed class Highlighter
    {
        [CompilerGenerated]
        private static EditorApplication.CallbackFunction <>f__mg$cache0;
        [CompilerGenerated]
        private static EditorApplication.CallbackFunction <>f__mg$cache1;
        [CompilerGenerated]
        private static EditorApplication.CallbackFunction <>f__mg$cache2;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static bool <active>k__BackingField;
        private const int kExpansionMovementSize = 5;
        private const float kPopupDuration = 0.33f;
        private const float kPulseSpeed = 0.45f;
        private static float s_HighlightElapsedTime = 0f;
        private static GUIStyle s_HighlightStyle;
        private static float s_LastTime = 0f;
        private static Rect s_RepaintRegion;
        private static HighlightSearchMode s_SearchMode;
        private static GUIView s_View;

        internal static void ControlHighlightGUI(GUIView self)
        {
            if (((s_View != null) && (self.window == s_View.window)) && (activeVisible && !searching))
            {
                if ((Event.current.type == EventType.ExecuteCommand) && (Event.current.commandName == "HandleControlHighlight"))
                {
                    if (self.screenPosition.Overlaps(s_RepaintRegion))
                    {
                        self.Repaint();
                    }
                }
                else if (Event.current.type == EventType.Repaint)
                {
                    Rect rect = GUIUtility.ScreenToGUIRect(activeRect);
                    rect = highlightStyle.padding.Add(rect);
                    float num = (Mathf.Cos(((s_HighlightElapsedTime * 3.141593f) * 2f) * 0.45f) + 1f) * 0.5f;
                    float num2 = Mathf.Min((float) 1f, (float) (0.01f + (s_HighlightElapsedTime / 0.33f)));
                    num2 += Mathf.Sin(num2 * 3.141593f) * 0.5f;
                    Vector2 vector = (Vector2) (new Vector2(((rect.width + 5f) / rect.width) - 1f, ((rect.height + 5f) / rect.height) - 1f) * num);
                    Vector2 scale = (Vector2) ((Vector2.one + vector) * num2);
                    Matrix4x4 matrix = GUI.matrix;
                    Color color = GUI.color;
                    GUI.color = new Color(1f, 1f, 1f, 0.8f - (0.3f * num));
                    GUIUtility.ScaleAroundPivot(scale, rect.center);
                    highlightStyle.Draw(rect, false, false, false, false);
                    GUI.color = color;
                    GUI.matrix = matrix;
                }
            }
        }

        internal static void Handle(Rect position, string text)
        {
            INTERNAL_CALL_Handle(ref position, text);
        }

        /// <summary>
        /// <para>Highlights an element in the editor.</para>
        /// </summary>
        /// <param name="windowTitle">The title of the window the element is inside.</param>
        /// <param name="text">The text to identify the element with.</param>
        /// <param name="mode">Optional mode to specify how to search for the element.</param>
        /// <returns>
        /// <para>true if the requested element was found; otherwise false.</para>
        /// </returns>
        public static bool Highlight(string windowTitle, string text)
        {
            return Highlight(windowTitle, text, HighlightSearchMode.Auto);
        }

        /// <summary>
        /// <para>Highlights an element in the editor.</para>
        /// </summary>
        /// <param name="windowTitle">The title of the window the element is inside.</param>
        /// <param name="text">The text to identify the element with.</param>
        /// <param name="mode">Optional mode to specify how to search for the element.</param>
        /// <returns>
        /// <para>true if the requested element was found; otherwise false.</para>
        /// </returns>
        public static bool Highlight(string windowTitle, string text, HighlightSearchMode mode)
        {
            Stop();
            active = true;
            if (!SetWindow(windowTitle))
            {
                Debug.LogWarning("Window " + windowTitle + " not found.");
                return false;
            }
            activeText = text;
            s_SearchMode = mode;
            s_LastTime = Time.realtimeSinceStartup;
            bool flag2 = Search();
            if (flag2)
            {
                if (<>f__mg$cache0 == null)
                {
                    <>f__mg$cache0 = new EditorApplication.CallbackFunction(Highlighter.Update);
                }
                EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.update, <>f__mg$cache0);
                if (<>f__mg$cache1 == null)
                {
                    <>f__mg$cache1 = new EditorApplication.CallbackFunction(Highlighter.Update);
                }
                EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.update, <>f__mg$cache1);
            }
            else
            {
                Debug.LogWarning("Item " + text + " not found in window " + windowTitle + ".");
                Stop();
            }
            InternalEditorUtility.RepaintAllViews();
            return flag2;
        }

        /// <summary>
        /// <para>Call this method to create an identifiable rect that the Highlighter can find.</para>
        /// </summary>
        /// <param name="position">The position to make highlightable.</param>
        /// <param name="identifier">The identifier text of the rect.</param>
        public static void HighlightIdentifier(Rect position, string identifier)
        {
            if ((searchMode == HighlightSearchMode.Identifier) || (searchMode == HighlightSearchMode.Auto))
            {
                Handle(position, identifier);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_Handle(ref Rect position, string text);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_internal_get_activeRect(out Rect value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_internal_set_activeRect(ref Rect value);
        internal static Rect internal_get_activeRect()
        {
            Rect rect;
            INTERNAL_CALL_internal_get_activeRect(out rect);
            return rect;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern string internal_get_activeText();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool internal_get_activeVisible();
        internal static void internal_set_activeRect(Rect value)
        {
            INTERNAL_CALL_internal_set_activeRect(ref value);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void internal_set_activeText(string value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void internal_set_activeVisible(bool value);
        private static bool Search()
        {
            searchMode = s_SearchMode;
            s_View.RepaintImmediately();
            if (searchMode == HighlightSearchMode.None)
            {
                return true;
            }
            searchMode = HighlightSearchMode.None;
            Stop();
            return false;
        }

        private static bool SetWindow(string windowTitle)
        {
            Object[] objArray = Resources.FindObjectsOfTypeAll(typeof(GUIView));
            GUIView view = null;
            foreach (GUIView view2 in objArray)
            {
                if (view2 is HostView)
                {
                    if ((view2 as HostView).actualView.titleContent.text == windowTitle)
                    {
                        view = view2;
                        break;
                    }
                }
                else if ((view2.window != null) && (view2.GetType().Name == windowTitle))
                {
                    view = view2;
                    break;
                }
            }
            s_View = view;
            return (view != null);
        }

        /// <summary>
        /// <para>Stops the active highlight.</para>
        /// </summary>
        public static void Stop()
        {
            active = false;
            activeVisible = false;
            activeText = string.Empty;
            activeRect = new Rect();
            s_LastTime = 0f;
            s_HighlightElapsedTime = 0f;
        }

        private static void Update()
        {
            Rect activeRect = Highlighter.activeRect;
            if ((Highlighter.activeRect.width == 0f) || (s_View == null))
            {
                if (<>f__mg$cache2 == null)
                {
                    <>f__mg$cache2 = new EditorApplication.CallbackFunction(Highlighter.Update);
                }
                EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.update, <>f__mg$cache2);
                Stop();
                InternalEditorUtility.RepaintAllViews();
            }
            else
            {
                Search();
            }
            if (activeVisible)
            {
                s_HighlightElapsedTime += Time.realtimeSinceStartup - s_LastTime;
            }
            s_LastTime = Time.realtimeSinceStartup;
            Rect rect = Highlighter.activeRect;
            if (activeRect.width > 0f)
            {
                rect.xMin = Mathf.Min(rect.xMin, activeRect.xMin);
                rect.xMax = Mathf.Max(rect.xMax, activeRect.xMax);
                rect.yMin = Mathf.Min(rect.yMin, activeRect.yMin);
                rect.yMax = Mathf.Max(rect.yMax, activeRect.yMax);
            }
            rect = highlightStyle.padding.Add(rect);
            rect = highlightStyle.overflow.Add(rect);
            rect = new RectOffset(7, 7, 7, 7).Add(rect);
            if (s_HighlightElapsedTime < 0.43f)
            {
                rect = new RectOffset(((int) rect.width) / 2, ((int) rect.width) / 2, ((int) rect.height) / 2, ((int) rect.height) / 2).Add(rect);
            }
            s_RepaintRegion = rect;
            foreach (GUIView view in Resources.FindObjectsOfTypeAll(typeof(GUIView)))
            {
                if (view.window == s_View.window)
                {
                    view.SendEvent(EditorGUIUtility.CommandEvent("HandleControlHighlight"));
                }
            }
        }

        /// <summary>
        /// <para>Is there currently an active highlight?</para>
        /// </summary>
        public static bool active
        {
            [CompilerGenerated]
            get
            {
                return <active>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                <active>k__BackingField = value;
            }
        }

        /// <summary>
        /// <para>The rect in screenspace of the current active highlight.</para>
        /// </summary>
        public static Rect activeRect
        {
            get
            {
                return internal_get_activeRect();
            }
            private set
            {
                internal_set_activeRect(value);
            }
        }

        /// <summary>
        /// <para>The text of the current active highlight.</para>
        /// </summary>
        public static string activeText
        {
            get
            {
                return internal_get_activeText();
            }
            private set
            {
                internal_set_activeText(value);
            }
        }

        /// <summary>
        /// <para>Is the current active highlight visible yet?</para>
        /// </summary>
        public static bool activeVisible
        {
            get
            {
                return internal_get_activeVisible();
            }
            private set
            {
                internal_set_activeVisible(value);
            }
        }

        private static GUIStyle highlightStyle
        {
            get
            {
                if (s_HighlightStyle == null)
                {
                    s_HighlightStyle = new GUIStyle("ControlHighlight");
                }
                return s_HighlightStyle;
            }
        }

        internal static bool searching { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        internal static HighlightSearchMode searchMode { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

