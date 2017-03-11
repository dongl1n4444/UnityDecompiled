namespace UnityEngine
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security;
    using UnityEngine.Scripting;
    using UnityEngineInternal;

    /// <summary>
    /// <para>Utility functions for implementing and extending the GUILayout class.</para>
    /// </summary>
    public class GUILayoutUtility
    {
        internal static LayoutCache current = new LayoutCache();
        internal static readonly Rect kDummyRect = new Rect(0f, 0f, 1f, 1f);
        private static GUIStyle s_SpaceStyle;
        private static Dictionary<int, LayoutCache> s_StoredLayouts = new Dictionary<int, LayoutCache>();
        private static Dictionary<int, LayoutCache> s_StoredWindows = new Dictionary<int, LayoutCache>();

        internal static void Begin(int instanceID)
        {
            LayoutCache cache = SelectIDList(instanceID, false);
            if (Event.current.type == EventType.Layout)
            {
                current.topLevel = cache.topLevel = new GUILayoutGroup();
                current.layoutGroups.Clear();
                current.layoutGroups.Push(current.topLevel);
                current.windows = cache.windows = new GUILayoutGroup();
            }
            else
            {
                current.topLevel = cache.topLevel;
                current.layoutGroups = cache.layoutGroups;
                current.windows = cache.windows;
            }
        }

        internal static void BeginContainer(LayoutCache cache)
        {
            if (Event.current.type == EventType.Layout)
            {
                current.topLevel = cache.topLevel = new GUILayoutGroup();
                current.layoutGroups.Clear();
                current.layoutGroups.Push(current.topLevel);
                current.windows = cache.windows = new GUILayoutGroup();
            }
            else
            {
                current.topLevel = cache.topLevel;
                current.layoutGroups = cache.layoutGroups;
                current.windows = cache.windows;
            }
        }

        [Obsolete("BeginGroup has no effect and will be removed", false)]
        public static void BeginGroup(string GroupName)
        {
        }

        internal static GUILayoutGroup BeginLayoutArea(GUIStyle style, System.Type layoutType)
        {
            GUILayoutGroup next;
            switch (Event.current.type)
            {
                case EventType.Used:
                case EventType.Layout:
                    next = CreateGUILayoutGroupInstanceOfType(layoutType);
                    next.style = style;
                    current.windows.Add(next);
                    break;

                default:
                    next = current.windows.GetNext() as GUILayoutGroup;
                    if (next == null)
                    {
                        throw new ArgumentException("GUILayout: Mismatched LayoutGroup." + Event.current.type);
                    }
                    next.ResetCursor();
                    GUIDebugger.LogLayoutGroupEntry(next.rect, next.margin, next.style, next.isVertical);
                    break;
            }
            current.layoutGroups.Push(next);
            current.topLevel = next;
            return next;
        }

        internal static GUILayoutGroup BeginLayoutGroup(GUIStyle style, GUILayoutOption[] options, System.Type layoutType)
        {
            GUILayoutGroup next;
            switch (Event.current.type)
            {
                case EventType.Used:
                case EventType.Layout:
                    next = CreateGUILayoutGroupInstanceOfType(layoutType);
                    next.style = style;
                    if (options != null)
                    {
                        next.ApplyOptions(options);
                    }
                    current.topLevel.Add(next);
                    break;

                default:
                    next = current.topLevel.GetNext() as GUILayoutGroup;
                    if (next == null)
                    {
                        throw new ArgumentException("GUILayout: Mismatched LayoutGroup." + Event.current.type);
                    }
                    next.ResetCursor();
                    GUIDebugger.LogLayoutGroupEntry(next.rect, next.margin, next.style, next.isVertical);
                    break;
            }
            current.layoutGroups.Push(next);
            current.topLevel = next;
            return next;
        }

        internal static void BeginWindow(int windowID, GUIStyle style, GUILayoutOption[] options)
        {
            LayoutCache cache = SelectIDList(windowID, true);
            if (Event.current.type == EventType.Layout)
            {
                current.topLevel = cache.topLevel = new GUILayoutGroup();
                current.topLevel.style = style;
                current.topLevel.windowID = windowID;
                if (options != null)
                {
                    current.topLevel.ApplyOptions(options);
                }
                current.layoutGroups.Clear();
                current.layoutGroups.Push(current.topLevel);
                current.windows = cache.windows = new GUILayoutGroup();
            }
            else
            {
                current.topLevel = cache.topLevel;
                current.layoutGroups = cache.layoutGroups;
                current.windows = cache.windows;
            }
        }

        internal static void CleanupRoots()
        {
            s_SpaceStyle = null;
            s_StoredLayouts = null;
            s_StoredWindows = null;
            current = null;
        }

        [SecuritySafeCritical]
        private static GUILayoutGroup CreateGUILayoutGroupInstanceOfType(System.Type LayoutType)
        {
            if (!typeof(GUILayoutGroup).IsAssignableFrom(LayoutType))
            {
                throw new ArgumentException("LayoutType needs to be of type GUILayoutGroup");
            }
            return (GUILayoutGroup) Activator.CreateInstance(LayoutType);
        }

        internal static GUILayoutGroup DoBeginLayoutArea(GUIStyle style, System.Type layoutType) => 
            BeginLayoutArea(style, layoutType);

        private static Rect DoGetAspectRect(float aspect, GUIStyle style, GUILayoutOption[] options)
        {
            switch (Event.current.type)
            {
                case EventType.Layout:
                    current.topLevel.Add(new GUIAspectSizer(aspect, options));
                    return kDummyRect;

                case EventType.Used:
                    return kDummyRect;
            }
            return current.topLevel.GetNext().rect;
        }

        private static Rect DoGetRect(GUIContent content, GUIStyle style, GUILayoutOption[] options)
        {
            GUIUtility.CheckOnGUI();
            switch (Event.current.type)
            {
                case EventType.Layout:
                    if (style.isHeightDependantOnWidth)
                    {
                        current.topLevel.Add(new GUIWordWrapSizer(style, content, options));
                    }
                    else
                    {
                        Vector2 constraints = new Vector2(0f, 0f);
                        if (options != null)
                        {
                            foreach (GUILayoutOption option in options)
                            {
                                switch (option.type)
                                {
                                    case GUILayoutOption.Type.maxHeight:
                                        constraints.y = (float) option.value;
                                        break;

                                    case GUILayoutOption.Type.maxWidth:
                                        constraints.x = (float) option.value;
                                        break;
                                }
                            }
                        }
                        Vector2 vector2 = style.CalcSizeWithConstraints(content, constraints);
                        current.topLevel.Add(new GUILayoutEntry(vector2.x, vector2.x, vector2.y, vector2.y, style, options));
                    }
                    return kDummyRect;

                case EventType.Used:
                    return kDummyRect;
            }
            GUILayoutEntry next = current.topLevel.GetNext();
            GUIDebugger.LogLayoutEntry(next.rect, next.margin, next.style);
            return next.rect;
        }

        private static Rect DoGetRect(float minWidth, float maxWidth, float minHeight, float maxHeight, GUIStyle style, GUILayoutOption[] options)
        {
            switch (Event.current.type)
            {
                case EventType.Layout:
                    current.topLevel.Add(new GUILayoutEntry(minWidth, maxWidth, minHeight, maxHeight, style, options));
                    return kDummyRect;

                case EventType.Used:
                    return kDummyRect;
            }
            return current.topLevel.GetNext().rect;
        }

        [Obsolete("EndGroup has no effect and will be removed", false)]
        public static void EndGroup(string groupName)
        {
        }

        internal static void EndLayoutGroup()
        {
            if ((Event.current.type != EventType.Layout) && (Event.current.type != EventType.Used))
            {
                GUIDebugger.LogLayoutEndGroup();
            }
            EventType type = Event.current.type;
            current.layoutGroups.Pop();
            current.topLevel = (0 >= current.layoutGroups.Count) ? null : ((GUILayoutGroup) current.layoutGroups.Peek());
        }

        /// <summary>
        /// <para>Reserve layout space for a rectangle with a specific aspect ratio.</para>
        /// </summary>
        /// <param name="aspect">The aspect ratio of the element (width / height).</param>
        /// <param name="style">An optional style. If specified, the style's padding value will be added to the sizes of the returned rectangle &amp; the style's margin values will be used for spacing.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The rect for the control.</para>
        /// </returns>
        public static Rect GetAspectRect(float aspect) => 
            DoGetAspectRect(aspect, GUIStyle.none, null);

        /// <summary>
        /// <para>Reserve layout space for a rectangle with a specific aspect ratio.</para>
        /// </summary>
        /// <param name="aspect">The aspect ratio of the element (width / height).</param>
        /// <param name="style">An optional style. If specified, the style's padding value will be added to the sizes of the returned rectangle &amp; the style's margin values will be used for spacing.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The rect for the control.</para>
        /// </returns>
        public static Rect GetAspectRect(float aspect, GUIStyle style) => 
            DoGetAspectRect(aspect, style, null);

        /// <summary>
        /// <para>Reserve layout space for a rectangle with a specific aspect ratio.</para>
        /// </summary>
        /// <param name="aspect">The aspect ratio of the element (width / height).</param>
        /// <param name="style">An optional style. If specified, the style's padding value will be added to the sizes of the returned rectangle &amp; the style's margin values will be used for spacing.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The rect for the control.</para>
        /// </returns>
        public static Rect GetAspectRect(float aspect, params GUILayoutOption[] options) => 
            DoGetAspectRect(aspect, GUIStyle.none, options);

        /// <summary>
        /// <para>Reserve layout space for a rectangle with a specific aspect ratio.</para>
        /// </summary>
        /// <param name="aspect">The aspect ratio of the element (width / height).</param>
        /// <param name="style">An optional style. If specified, the style's padding value will be added to the sizes of the returned rectangle &amp; the style's margin values will be used for spacing.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The rect for the control.</para>
        /// </returns>
        public static Rect GetAspectRect(float aspect, GUIStyle style, params GUILayoutOption[] options) => 
            DoGetAspectRect(aspect, GUIStyle.none, options);

        /// <summary>
        /// <para>Get the rectangle last used by GUILayout for a control.</para>
        /// </summary>
        /// <returns>
        /// <para>The last used rectangle.</para>
        /// </returns>
        public static Rect GetLastRect()
        {
            switch (Event.current.type)
            {
                case EventType.Layout:
                case EventType.Used:
                    return kDummyRect;
            }
            return current.topLevel.GetLast();
        }

        /// <summary>
        /// <para>Reserve layout space for a rectangle with a fixed content area.</para>
        /// </summary>
        /// <param name="width">The width of the area you want.</param>
        /// <param name="height">The height of the area you want.</param>
        /// <param name="style">An optional GUIStyle to layout for. If specified, the style's padding value will be added to your sizes &amp; its margin value will be used for spacing.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The rectanlge to put your control in.</para>
        /// </returns>
        public static Rect GetRect(float width, float height) => 
            DoGetRect(width, width, height, height, GUIStyle.none, null);

        /// <summary>
        /// <para>Reserve layout space for a rectangle for displaying some contents with a specific style.</para>
        /// </summary>
        /// <param name="content">The content to make room for displaying.</param>
        /// <param name="style">The GUIStyle to layout for.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>A rectangle that is large enough to contain content when rendered in style.</para>
        /// </returns>
        public static Rect GetRect(GUIContent content, GUIStyle style) => 
            DoGetRect(content, style, null);

        /// <summary>
        /// <para>Reserve layout space for a rectangle with a fixed content area.</para>
        /// </summary>
        /// <param name="width">The width of the area you want.</param>
        /// <param name="height">The height of the area you want.</param>
        /// <param name="style">An optional GUIStyle to layout for. If specified, the style's padding value will be added to your sizes &amp; its margin value will be used for spacing.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The rectanlge to put your control in.</para>
        /// </returns>
        public static Rect GetRect(float width, float height, GUIStyle style) => 
            DoGetRect(width, width, height, height, style, null);

        /// <summary>
        /// <para>Reserve layout space for a rectangle with a fixed content area.</para>
        /// </summary>
        /// <param name="width">The width of the area you want.</param>
        /// <param name="height">The height of the area you want.</param>
        /// <param name="style">An optional GUIStyle to layout for. If specified, the style's padding value will be added to your sizes &amp; its margin value will be used for spacing.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The rectanlge to put your control in.</para>
        /// </returns>
        public static Rect GetRect(float width, float height, params GUILayoutOption[] options) => 
            DoGetRect(width, width, height, height, GUIStyle.none, options);

        /// <summary>
        /// <para>Reserve layout space for a rectangle for displaying some contents with a specific style.</para>
        /// </summary>
        /// <param name="content">The content to make room for displaying.</param>
        /// <param name="style">The GUIStyle to layout for.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>A rectangle that is large enough to contain content when rendered in style.</para>
        /// </returns>
        public static Rect GetRect(GUIContent content, GUIStyle style, params GUILayoutOption[] options) => 
            DoGetRect(content, style, options);

        /// <summary>
        /// <para>Reserve layout space for a flexible rect.</para>
        /// </summary>
        /// <param name="minWidth">The minimum width of the area passed back.</param>
        /// <param name="maxWidth">The maximum width of the area passed back.</param>
        /// <param name="minHeight">The minimum width of the area passed back.</param>
        /// <param name="maxHeight">The maximum width of the area passed back.</param>
        /// <param name="style">An optional style. If specified, the style's padding value will be added to the sizes requested &amp; the style's margin values will be used for spacing.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>A rectangle with size between minWidth &amp; maxWidth on both axes.</para>
        /// </returns>
        public static Rect GetRect(float minWidth, float maxWidth, float minHeight, float maxHeight) => 
            DoGetRect(minWidth, maxWidth, minHeight, maxHeight, GUIStyle.none, null);

        /// <summary>
        /// <para>Reserve layout space for a rectangle with a fixed content area.</para>
        /// </summary>
        /// <param name="width">The width of the area you want.</param>
        /// <param name="height">The height of the area you want.</param>
        /// <param name="style">An optional GUIStyle to layout for. If specified, the style's padding value will be added to your sizes &amp; its margin value will be used for spacing.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The rectanlge to put your control in.</para>
        /// </returns>
        public static Rect GetRect(float width, float height, GUIStyle style, params GUILayoutOption[] options) => 
            DoGetRect(width, width, height, height, style, options);

        /// <summary>
        /// <para>Reserve layout space for a flexible rect.</para>
        /// </summary>
        /// <param name="minWidth">The minimum width of the area passed back.</param>
        /// <param name="maxWidth">The maximum width of the area passed back.</param>
        /// <param name="minHeight">The minimum width of the area passed back.</param>
        /// <param name="maxHeight">The maximum width of the area passed back.</param>
        /// <param name="style">An optional style. If specified, the style's padding value will be added to the sizes requested &amp; the style's margin values will be used for spacing.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>A rectangle with size between minWidth &amp; maxWidth on both axes.</para>
        /// </returns>
        public static Rect GetRect(float minWidth, float maxWidth, float minHeight, float maxHeight, GUIStyle style) => 
            DoGetRect(minWidth, maxWidth, minHeight, maxHeight, style, null);

        /// <summary>
        /// <para>Reserve layout space for a flexible rect.</para>
        /// </summary>
        /// <param name="minWidth">The minimum width of the area passed back.</param>
        /// <param name="maxWidth">The maximum width of the area passed back.</param>
        /// <param name="minHeight">The minimum width of the area passed back.</param>
        /// <param name="maxHeight">The maximum width of the area passed back.</param>
        /// <param name="style">An optional style. If specified, the style's padding value will be added to the sizes requested &amp; the style's margin values will be used for spacing.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>A rectangle with size between minWidth &amp; maxWidth on both axes.</para>
        /// </returns>
        public static Rect GetRect(float minWidth, float maxWidth, float minHeight, float maxHeight, params GUILayoutOption[] options) => 
            DoGetRect(minWidth, maxWidth, minHeight, maxHeight, GUIStyle.none, options);

        /// <summary>
        /// <para>Reserve layout space for a flexible rect.</para>
        /// </summary>
        /// <param name="minWidth">The minimum width of the area passed back.</param>
        /// <param name="maxWidth">The maximum width of the area passed back.</param>
        /// <param name="minHeight">The minimum width of the area passed back.</param>
        /// <param name="maxHeight">The maximum width of the area passed back.</param>
        /// <param name="style">An optional style. If specified, the style's padding value will be added to the sizes requested &amp; the style's margin values will be used for spacing.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>A rectangle with size between minWidth &amp; maxWidth on both axes.</para>
        /// </returns>
        public static Rect GetRect(float minWidth, float maxWidth, float minHeight, float maxHeight, GUIStyle style, params GUILayoutOption[] options) => 
            DoGetRect(minWidth, maxWidth, minHeight, maxHeight, style, options);

        internal static Rect GetWindowsBounds()
        {
            Rect rect;
            INTERNAL_CALL_GetWindowsBounds(out rect);
            return rect;
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetWindowsBounds(out Rect value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_Internal_GetWindowRect(int windowID, out Rect value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_Internal_MoveWindow(int windowID, ref Rect r);
        private static Rect Internal_GetWindowRect(int windowID)
        {
            Rect rect;
            INTERNAL_CALL_Internal_GetWindowRect(windowID, out rect);
            return rect;
        }

        private static void Internal_MoveWindow(int windowID, Rect r)
        {
            INTERNAL_CALL_Internal_MoveWindow(windowID, ref r);
        }

        internal static void Layout()
        {
            if (current.topLevel.windowID == -1)
            {
                current.topLevel.CalcWidth();
                current.topLevel.SetHorizontal(0f, Mathf.Min(((float) Screen.width) / GUIUtility.pixelsPerPoint, current.topLevel.maxWidth));
                current.topLevel.CalcHeight();
                current.topLevel.SetVertical(0f, Mathf.Min(((float) Screen.height) / GUIUtility.pixelsPerPoint, current.topLevel.maxHeight));
                LayoutFreeGroup(current.windows);
            }
            else
            {
                LayoutSingleGroup(current.topLevel);
                LayoutFreeGroup(current.windows);
            }
        }

        internal static void LayoutFreeGroup(GUILayoutGroup toplevel)
        {
            foreach (GUILayoutGroup group in toplevel.entries)
            {
                LayoutSingleGroup(group);
            }
            toplevel.ResetCursor();
        }

        internal static void LayoutFromContainer(float w, float h)
        {
            current.topLevel.CalcWidth();
            current.topLevel.SetHorizontal(0f, w);
            current.topLevel.CalcHeight();
            current.topLevel.SetVertical(0f, h);
            LayoutFreeGroup(current.windows);
        }

        internal static void LayoutFromEditorWindow()
        {
            current.topLevel.CalcWidth();
            current.topLevel.SetHorizontal(0f, ((float) Screen.width) / GUIUtility.pixelsPerPoint);
            current.topLevel.CalcHeight();
            current.topLevel.SetVertical(0f, ((float) Screen.height) / GUIUtility.pixelsPerPoint);
            LayoutFreeGroup(current.windows);
        }

        internal static float LayoutFromInspector(float width)
        {
            if ((current.topLevel != null) && (current.topLevel.windowID == -1))
            {
                current.topLevel.CalcWidth();
                current.topLevel.SetHorizontal(0f, width);
                current.topLevel.CalcHeight();
                current.topLevel.SetVertical(0f, Mathf.Min(((float) Screen.height) / GUIUtility.pixelsPerPoint, current.topLevel.maxHeight));
                float minHeight = current.topLevel.minHeight;
                LayoutFreeGroup(current.windows);
                return minHeight;
            }
            if (current.topLevel != null)
            {
                LayoutSingleGroup(current.topLevel);
            }
            return 0f;
        }

        private static void LayoutSingleGroup(GUILayoutGroup i)
        {
            if (!i.isWindow)
            {
                float minWidth = i.minWidth;
                float maxWidth = i.maxWidth;
                i.CalcWidth();
                i.SetHorizontal(i.rect.x, Mathf.Clamp(i.maxWidth, minWidth, maxWidth));
                float minHeight = i.minHeight;
                float maxHeight = i.maxHeight;
                i.CalcHeight();
                i.SetVertical(i.rect.y, Mathf.Clamp(i.maxHeight, minHeight, maxHeight));
            }
            else
            {
                i.CalcWidth();
                Rect rect = Internal_GetWindowRect(i.windowID);
                i.SetHorizontal(rect.x, Mathf.Clamp(rect.width, i.minWidth, i.maxWidth));
                i.CalcHeight();
                i.SetVertical(rect.y, Mathf.Clamp(rect.height, i.minHeight, i.maxHeight));
                Internal_MoveWindow(i.windowID, i.rect);
            }
        }

        internal static LayoutCache SelectIDList(int instanceID, bool isWindow)
        {
            LayoutCache cache;
            Dictionary<int, LayoutCache> dictionary = !isWindow ? s_StoredLayouts : s_StoredWindows;
            if (!dictionary.TryGetValue(instanceID, out cache))
            {
                cache = new LayoutCache();
                dictionary[instanceID] = cache;
            }
            current.topLevel = cache.topLevel;
            current.layoutGroups = cache.layoutGroups;
            current.windows = cache.windows;
            return cache;
        }

        internal static GUIStyle spaceStyle
        {
            get
            {
                if (s_SpaceStyle == null)
                {
                    s_SpaceStyle = new GUIStyle();
                }
                s_SpaceStyle.stretchWidth = false;
                return s_SpaceStyle;
            }
        }

        internal static GUILayoutGroup topLevel =>
            current.topLevel;

        internal sealed class LayoutCache
        {
            internal GenericStack layoutGroups;
            internal GUILayoutGroup topLevel;
            internal GUILayoutGroup windows;

            internal LayoutCache()
            {
                this.topLevel = new GUILayoutGroup();
                this.layoutGroups = new GenericStack();
                this.windows = new GUILayoutGroup();
                this.layoutGroups.Push(this.topLevel);
            }

            internal LayoutCache(GUILayoutUtility.LayoutCache other)
            {
                this.topLevel = new GUILayoutGroup();
                this.layoutGroups = new GenericStack();
                this.windows = new GUILayoutGroup();
                this.topLevel = other.topLevel;
                this.layoutGroups = other.layoutGroups;
                this.windows = other.windows;
            }
        }
    }
}

