namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.Internal;
    using UnityEngine.Scripting;
    using UnityEngineInternal;

    /// <summary>
    /// <para>Miscellaneous helper stuff for EditorGUI.</para>
    /// </summary>
    public sealed class EditorGUIUtility : GUIUtility
    {
        [CompilerGenerated]
        private static GUISkin.SkinChangedDelegate <>f__mg$cache0;
        internal static Color kDarkViewBackground = new Color(0.22f, 0.22f, 0.22f, 0f);
        [Obsolete("This field is no longer used by any builtin controls. If passing this field to GetControlID, explicitly use the FocusType enum instead.", false)]
        public static FocusType native = FocusType.Keyboard;
        private static GUIStyle s_BasicTextureStyle;
        private static GUIContent s_BlankContent = new GUIContent(" ");
        private static float s_ContextWidth = 0f;
        private static Texture2D s_ErrorIcon;
        private static float s_FieldWidth = 0f;
        internal static int s_FontIsBold = -1;
        private static bool s_HierarchyMode = false;
        private static Hashtable s_IconGUIContents = new Hashtable();
        private static GUIContent s_Image = new GUIContent();
        private static Texture2D s_InfoIcon;
        private static float s_LabelWidth = 0f;
        internal static int s_LastControlID = 0;
        private static GUIContent s_ObjectContent = new GUIContent();
        private static GUIContent s_Text = new GUIContent();
        private static Hashtable s_TextGUIContents = new Hashtable();
        private static GUIContent s_TextImage = new GUIContent();
        private static Texture2D s_WarningIcon;
        private static GUIStyle s_WhiteTextureStyle;
        internal static bool s_WideMode = false;
        internal static SliderLabels sliderLabels = new SliderLabels();

        static EditorGUIUtility()
        {
            if (<>f__mg$cache0 == null)
            {
                <>f__mg$cache0 = new GUISkin.SkinChangedDelegate(EditorGUIUtility.SkinChanged);
            }
            GUISkin.m_SkinChanged = (GUISkin.SkinChangedDelegate) Delegate.Combine(GUISkin.m_SkinChanged, <>f__mg$cache0);
        }

        /// <summary>
        /// <para>Add a custom mouse pointer to a control.</para>
        /// </summary>
        /// <param name="position">The rectangle the control should be shown within.</param>
        /// <param name="mouse">The mouse cursor to use.</param>
        /// <param name="controlID">ID of a target control.</param>
        public static void AddCursorRect(Rect position, MouseCursor mouse)
        {
            AddCursorRect(position, mouse, 0);
        }

        /// <summary>
        /// <para>Add a custom mouse pointer to a control.</para>
        /// </summary>
        /// <param name="position">The rectangle the control should be shown within.</param>
        /// <param name="mouse">The mouse cursor to use.</param>
        /// <param name="controlID">ID of a target control.</param>
        public static void AddCursorRect(Rect position, MouseCursor mouse, int controlID)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Rect rect = GUIClip.Unclip(position);
                Rect topmostRect = GUIClip.topmostRect;
                float xmin = Mathf.Max(rect.x, topmostRect.x);
                float ymin = Mathf.Max(rect.y, topmostRect.y);
                float xmax = Mathf.Min(rect.xMax, topmostRect.xMax);
                Rect r = Rect.MinMaxRect(xmin, ymin, xmax, Mathf.Min(rect.yMax, topmostRect.yMax));
                if ((r.width > 0f) && (r.height > 0f))
                {
                    Internal_AddCursorRect(r, mouse, controlID);
                }
            }
        }

        private static float CalcContextWidth()
        {
            float width = GUIClip.GetTopRect().width;
            if ((width >= 1f) && (width < 40000f))
            {
                return width;
            }
            return currentViewWidth;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool CanHaveKeyboardFocus(int id);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void CleanCache(string text);
        /// <summary>
        /// <para>Creates an event that can be sent to another window.</para>
        /// </summary>
        /// <param name="commandName">The command to be sent.</param>
        public static Event CommandEvent(string commandName)
        {
            Event evt = new Event();
            Internal_SetupEventValues(evt);
            evt.type = EventType.ExecuteCommand;
            evt.commandName = commandName;
            return evt;
        }

        internal static Rect DragZoneRect(Rect position) => 
            new Rect(position.x, position.y, labelWidth, position.height);

        /// <summary>
        /// <para>Draw a color swatch.</para>
        /// </summary>
        /// <param name="position">The rectangle to draw the color swatch within.</param>
        /// <param name="color">The color to draw.</param>
        public static void DrawColorSwatch(Rect position, Color color)
        {
            DrawColorSwatch(position, color, true);
        }

        internal static void DrawColorSwatch(Rect position, Color color, bool showAlpha)
        {
            DrawColorSwatch(position, color, showAlpha, false);
        }

        internal static void DrawColorSwatch(Rect position, Color color, bool showAlpha, bool hdr)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Color color2 = GUI.color;
                float a = !GUI.enabled ? ((float) 2) : ((float) 1);
                GUI.color = !EditorGUI.showMixedValue ? new Color(color.r, color.g, color.b, a) : (new Color(0.82f, 0.82f, 0.82f, a) * color2);
                GUIStyle whiteTextureStyle = EditorGUIUtility.whiteTextureStyle;
                whiteTextureStyle.Draw(position, false, false, false, false);
                float maxColorComponent = GUI.color.maxColorComponent;
                if (hdr && (maxColorComponent > 1f))
                {
                    float width = position.width / 3f;
                    Rect rect = new Rect(position.x, position.y, width, position.height);
                    Rect rect2 = new Rect(position.xMax - width, position.y, width, position.height);
                    Color color4 = GUI.color.RGBMultiplied((float) (1f / maxColorComponent));
                    Color color6 = GUI.color;
                    GUI.color = color4;
                    GUIStyle basicTextureStyle = GetBasicTextureStyle(whiteTexture);
                    basicTextureStyle.Draw(rect, false, false, false, false);
                    basicTextureStyle.Draw(rect2, false, false, false, false);
                    GUI.color = color6;
                    GetBasicTextureStyle(ColorPicker.GetGradientTextureWithAlpha0To1()).Draw(rect, false, false, false, false);
                    GetBasicTextureStyle(ColorPicker.GetGradientTextureWithAlpha1To0()).Draw(rect2, false, false, false, false);
                }
                if (!EditorGUI.showMixedValue)
                {
                    if (showAlpha)
                    {
                        GUI.color = new Color(0f, 0f, 0f, a);
                        float height = Mathf.Clamp((float) (position.height * 0.2f), (float) 2f, (float) 20f);
                        Rect rect3 = new Rect(position.x, position.yMax - height, position.width, height);
                        whiteTextureStyle.Draw(rect3, false, false, false, false);
                        GUI.color = new Color(1f, 1f, 1f, a);
                        rect3.width *= Mathf.Clamp01(color.a);
                        whiteTextureStyle.Draw(rect3, false, false, false, false);
                    }
                }
                else
                {
                    EditorGUI.BeginHandleMixedValueContentColor();
                    whiteTextureStyle.Draw(position, EditorGUI.mixedValueContent, false, false, false, false);
                    EditorGUI.EndHandleMixedValueContentColor();
                }
                GUI.color = color2;
                if (hdr && (maxColorComponent > 1f))
                {
                    GUI.Label(new Rect(position.x, position.y, position.width - 3f, position.height), "HDR", EditorStyles.centeredGreyMiniLabel);
                }
            }
        }

        /// <summary>
        /// <para>Draw a curve swatch.</para>
        /// </summary>
        /// <param name="position">The rectangle to draw the color swatch within.</param>
        /// <param name="curve">The curve to draw.</param>
        /// <param name="property">The curve to draw as a SerializedProperty.</param>
        /// <param name="color">The color to draw the curve with.</param>
        /// <param name="bgColor">The color to draw the background with.</param>
        /// <param name="curveRanges">Optional parameter to specify the range of the curve which should be included in swatch.</param>
        public static void DrawCurveSwatch(Rect position, AnimationCurve curve, SerializedProperty property, Color color, Color bgColor)
        {
            DrawCurveSwatchInternal(position, curve, null, property, null, color, bgColor, false, new Rect(), Color.clear, Color.clear);
        }

        /// <summary>
        /// <para>Draw a curve swatch.</para>
        /// </summary>
        /// <param name="position">The rectangle to draw the color swatch within.</param>
        /// <param name="curve">The curve to draw.</param>
        /// <param name="property">The curve to draw as a SerializedProperty.</param>
        /// <param name="color">The color to draw the curve with.</param>
        /// <param name="bgColor">The color to draw the background with.</param>
        /// <param name="curveRanges">Optional parameter to specify the range of the curve which should be included in swatch.</param>
        public static void DrawCurveSwatch(Rect position, AnimationCurve curve, SerializedProperty property, Color color, Color bgColor, Rect curveRanges)
        {
            DrawCurveSwatchInternal(position, curve, null, property, null, color, bgColor, true, curveRanges, Color.clear, Color.clear);
        }

        public static void DrawCurveSwatch(Rect position, AnimationCurve curve, SerializedProperty property, Color color, Color bgColor, Color topFillColor, Color bottomFillColor)
        {
            DrawCurveSwatchInternal(position, curve, null, property, null, color, bgColor, false, new Rect(), topFillColor, bottomFillColor);
        }

        public static void DrawCurveSwatch(Rect position, AnimationCurve curve, SerializedProperty property, Color color, Color bgColor, Color topFillColor, Color bottomFillColor, Rect curveRanges)
        {
            DrawCurveSwatchInternal(position, curve, null, property, null, color, bgColor, true, curveRanges, topFillColor, bottomFillColor);
        }

        private static void DrawCurveSwatchInternal(Rect position, AnimationCurve curve, AnimationCurve curve2, SerializedProperty property, SerializedProperty property2, Color color, Color bgColor, bool useCurveRanges, Rect curveRanges, Color topFillColor, Color bottomFillColor)
        {
            if (Event.current.type == EventType.Repaint)
            {
                int width = (int) position.width;
                int height = (int) position.height;
                int maxTextureSize = SystemInfo.maxTextureSize;
                bool flag = width > maxTextureSize;
                bool flag2 = height > maxTextureSize;
                if (flag)
                {
                    width = Mathf.Min(width, maxTextureSize);
                }
                if (flag2)
                {
                    height = Mathf.Min(height, maxTextureSize);
                }
                Color color2 = GUI.color;
                GUI.color = bgColor;
                whiteTextureStyle.Draw(position, false, false, false, false);
                GUI.color = color2;
                if ((property != null) && property.hasMultipleDifferentValues)
                {
                    EditorGUI.BeginHandleMixedValueContentColor();
                    GUI.Label(position, EditorGUI.mixedValueContent, "PreOverlayLabel");
                    EditorGUI.EndHandleMixedValueContentColor();
                }
                else
                {
                    Texture2D tex = null;
                    if (property != null)
                    {
                        if (property2 == null)
                        {
                            tex = !useCurveRanges ? AnimationCurvePreviewCache.GetPreview(width, height, property, color, topFillColor, bottomFillColor) : AnimationCurvePreviewCache.GetPreview(width, height, property, color, topFillColor, bottomFillColor, curveRanges);
                        }
                        else
                        {
                            tex = !useCurveRanges ? AnimationCurvePreviewCache.GetPreview(width, height, property, property2, color, topFillColor, bottomFillColor) : AnimationCurvePreviewCache.GetPreview(width, height, property, property2, color, topFillColor, bottomFillColor, curveRanges);
                        }
                    }
                    else if (curve != null)
                    {
                        if (curve2 == null)
                        {
                            tex = !useCurveRanges ? AnimationCurvePreviewCache.GetPreview(width, height, curve, color, topFillColor, bottomFillColor) : AnimationCurvePreviewCache.GetPreview(width, height, curve, color, topFillColor, bottomFillColor, curveRanges);
                        }
                        else
                        {
                            tex = !useCurveRanges ? AnimationCurvePreviewCache.GetPreview(width, height, curve, curve2, color, topFillColor, bottomFillColor) : AnimationCurvePreviewCache.GetPreview(width, height, curve, curve2, color, topFillColor, bottomFillColor, curveRanges);
                        }
                    }
                    GUIStyle basicTextureStyle = GetBasicTextureStyle(tex);
                    if (!flag)
                    {
                        position.width = tex.width;
                    }
                    if (!flag2)
                    {
                        position.height = tex.height;
                    }
                    basicTextureStyle.Draw(position, false, false, false, false);
                }
            }
        }

        internal static void DrawHorizontalSplitter(Rect dragRect)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Color color = GUI.color;
                Color color2 = !isProSkin ? new Color(0.6f, 0.6f, 0.6f, 1.333f) : new Color(0.12f, 0.12f, 0.12f, 1.333f);
                GUI.color *= color2;
                Rect position = new Rect(dragRect.x - 1f, dragRect.y, 1f, dragRect.height);
                GUI.DrawTexture(position, whiteTexture);
                GUI.color = color;
            }
        }

        /// <summary>
        /// <para>Draw swatch with a filled region between two SerializedProperty curves.</para>
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="property2"></param>
        /// <param name="color"></param>
        /// <param name="bgColor"></param>
        /// <param name="curveRanges"></param>
        public static void DrawRegionSwatch(Rect position, SerializedProperty property, SerializedProperty property2, Color color, Color bgColor, Rect curveRanges)
        {
            DrawCurveSwatchInternal(position, null, null, property, property2, color, bgColor, true, curveRanges, Color.clear, Color.clear);
        }

        /// <summary>
        /// <para>Draw swatch with a filled region between two curves.</para>
        /// </summary>
        /// <param name="position"></param>
        /// <param name="curve"></param>
        /// <param name="curve2"></param>
        /// <param name="color"></param>
        /// <param name="bgColor"></param>
        /// <param name="curveRanges"></param>
        public static void DrawRegionSwatch(Rect position, AnimationCurve curve, AnimationCurve curve2, Color color, Color bgColor, Rect curveRanges)
        {
            DrawCurveSwatchInternal(position, curve, curve2, null, null, color, bgColor, true, curveRanges, Color.clear, Color.clear);
        }

        /// <summary>
        /// <para>Get a texture from its source filename.</para>
        /// </summary>
        /// <param name="name"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern Texture2D FindTexture(string name);
        internal static GUIStyle GetBasicTextureStyle(Texture2D tex)
        {
            if (s_BasicTextureStyle == null)
            {
                s_BasicTextureStyle = new GUIStyle();
            }
            s_BasicTextureStyle.normal.background = tex;
            return s_BasicTextureStyle;
        }

        internal static bool GetBoldDefaultFont() => 
            (s_FontIsBold == 1);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern Object GetBuiltinExtraResource(Type type, string path);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern BuiltinResource[] GetBuiltinResourceList(int classID);
        /// <summary>
        /// <para>Get one of the built-in GUI skins, which can be the game view, inspector or scene view skin as chosen by the parameter.</para>
        /// </summary>
        /// <param name="skin"></param>
        public static GUISkin GetBuiltinSkin(EditorSkin skin) => 
            GUIUtility.GetBuiltinSkin((int) skin);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern AssetBundle GetEditorAssetBundle();
        public static List<Rect> GetFlowLayoutedRects(Rect rect, GUIStyle style, float horizontalSpacing, float verticalSpacing, List<string> items)
        {
            List<Rect> list = new List<Rect>(items.Count);
            Vector2 position = rect.position;
            for (int i = 0; i < items.Count; i++)
            {
                GUIContent content = TempContent(items[i]);
                Vector2 size = style.CalcSize(content);
                Rect item = new Rect(position, size);
                if (((position.x + size.x) + horizontalSpacing) >= rect.xMax)
                {
                    position.x = rect.x;
                    position.y += size.y + verticalSpacing;
                    item.position = position;
                }
                list.Add(item);
                position.x += size.x + horizontalSpacing;
            }
            return list;
        }

        internal static Texture2D GetHelpIcon(MessageType type)
        {
            if (type != MessageType.Info)
            {
                if (type == MessageType.Warning)
                {
                    return warningIcon;
                }
                if (type == MessageType.Error)
                {
                    return errorIcon;
                }
            }
            else
            {
                return infoIcon;
            }
            return null;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern Texture2D GetIconForObject(Object obj);
        /// <summary>
        /// <para>Get the size that has been set using SetIconSize.</para>
        /// </summary>
        public static Vector2 GetIconSize()
        {
            Vector2 vector;
            Internal_GetIconSize(out vector);
            return vector;
        }

        internal static string[] GetNameAndTooltipString(string nameAndTooltip)
        {
            nameAndTooltip = LocalizationDatabase.GetLocalizedString(nameAndTooltip);
            string[] strArray = new string[3];
            char[] separator = new char[] { '|' };
            string[] strArray2 = nameAndTooltip.Split(separator);
            switch (strArray2.Length)
            {
                case 0:
                    strArray[0] = "";
                    strArray[1] = "";
                    return strArray;

                case 1:
                    strArray[0] = strArray2[0].Trim();
                    strArray[1] = strArray[0];
                    return strArray;

                case 2:
                    strArray[0] = strArray2[0].Trim();
                    strArray[1] = strArray[0];
                    strArray[2] = strArray2[1].Trim();
                    return strArray;
            }
            Debug.LogError("Error in Tooltips: Too many strings in line beginning with '" + strArray2[0] + "'");
            return strArray;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern string GetObjectNameWithInfo(Object obj);
        /// <summary>
        /// <para>The controlID of the currently showing object picker.</para>
        /// </summary>
        public static int GetObjectPickerControlID() => 
            ObjectSelector.get.objectSelectorID;

        /// <summary>
        /// <para>The object currently selected in the object picker.</para>
        /// </summary>
        public static Object GetObjectPickerObject() => 
            ObjectSelector.GetCurrentObject();

        internal static Color GetPasteboardColor()
        {
            Color color;
            INTERNAL_CALL_GetPasteboardColor(out color);
            return color;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern Object GetScript(string scriptClass);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern int GetSearchIndexOfControlIDList();
        private static GUIStyle GetStyle(string styleName)
        {
            GUIStyle error = GUI.skin.FindStyle(styleName);
            if (error == null)
            {
                error = GetBuiltinSkin(EditorSkin.Inspector).FindStyle(styleName);
            }
            if (error == null)
            {
                Debug.Log("Missing built-in guistyle " + styleName);
                error = GUISkin.error;
            }
            return error;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern string GetTypeNameWithInfo(string typeName);
        [RequiredByNativeCode]
        internal static void HandleControlID(int id)
        {
            s_LastControlID = id;
            EditorGUI.PrepareCurrentPrefixLabel(s_LastControlID);
        }

        internal static Rect HandleHorizontalSplitter(Rect dragRect, float width, float minLeftSide, float minRightSide)
        {
            if (Event.current.type == EventType.Repaint)
            {
                AddCursorRect(dragRect, MouseCursor.SplitResizeLeftRight);
            }
            float num = 0f;
            float x = EditorGUI.MouseDeltaReader(dragRect, true).x;
            if (x != 0f)
            {
                dragRect.x += x;
                num = Mathf.Clamp(dragRect.x, minLeftSide, width - minRightSide);
            }
            if (dragRect.x > (width - minRightSide))
            {
                num = width - minRightSide;
            }
            if (num > 0f)
            {
                dragRect.x = num;
            }
            return dragRect;
        }

        internal static bool HasCurrentWindowKeyFocus()
        {
            GUIUtility.CheckOnGUI();
            return GUIView.current.hasFocus;
        }

        internal static bool HasHolddownKeyModifiers(Event evt) => 
            (((evt.shift | evt.control) | evt.alt) | evt.command);

        /// <summary>
        /// <para>Does a given class have per-object thumbnails?</para>
        /// </summary>
        /// <param name="objType"></param>
        public static bool HasObjectThumbnail(Type objType) => 
            ((objType != null) && ((objType.IsSubclassOf(typeof(Texture)) || (objType == typeof(Texture))) || (objType == typeof(Sprite))));

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool HasPasteboardColor();
        [Obsolete("EditorGUIUtility.HSVToRGB is obsolete. Use Color.HSVToRGB instead (UnityUpgradable) -> [UnityEngine] UnityEngine.Color.HSVToRGB(*)", true)]
        public static Color HSVToRGB(float H, float S, float V) => 
            Color.HSVToRGB(H, S, V);

        [Obsolete("EditorGUIUtility.HSVToRGB is obsolete. Use Color.HSVToRGB instead (UnityUpgradable) -> [UnityEngine] UnityEngine.Color.HSVToRGB(*)", true)]
        public static Color HSVToRGB(float H, float S, float V, bool hdr) => 
            Color.HSVToRGB(H, S, V, hdr);

        /// <summary>
        /// <para>Fetch the GUIContent from the Unity builtin resources with the given name.</para>
        /// </summary>
        /// <param name="name">Content name.</param>
        /// <param name="tooltip">Tooltip.</param>
        /// <param name="text"></param>
        [ExcludeFromDocs]
        public static GUIContent IconContent(string name)
        {
            string text = null;
            return IconContent(name, text);
        }

        /// <summary>
        /// <para>Fetch the GUIContent from the Unity builtin resources with the given name.</para>
        /// </summary>
        /// <param name="name">Content name.</param>
        /// <param name="tooltip">Tooltip.</param>
        /// <param name="text"></param>
        public static GUIContent IconContent(string name, [DefaultValue("null")] string text)
        {
            GUIContent content = (GUIContent) s_IconGUIContents[name];
            if (content == null)
            {
                content = new GUIContent();
                if (text != null)
                {
                    string[] nameAndTooltipString = GetNameAndTooltipString(text);
                    if (nameAndTooltipString[2] != null)
                    {
                        content.tooltip = nameAndTooltipString[2];
                    }
                }
                content.image = LoadIconRequired(name);
                s_IconGUIContents[name] = content;
            }
            return content;
        }

        private static void Internal_AddCursorRect(Rect r, MouseCursor m, int controlID)
        {
            INTERNAL_CALL_Internal_AddCursorRect(ref r, m, controlID);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_GetPasteboardColor(out Color value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_Internal_AddCursorRect(ref Rect r, MouseCursor m, int controlID);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_RenderGameViewCameras(RenderTexture target, int targetDisplay, ref Rect screenRect, ref Vector2 mousePosition, bool gizmos);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_RenderGameViewCamerasInternal(RenderTexture target, int targetDisplay, ref Rect screenRect, ref Vector2 mousePosition, bool gizmos);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_SetIconSize(ref Vector2 size);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_SetPasteboardColor(ref Color color);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void Internal_GetIconSize(out Vector2 size);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool Internal_GetKeyboardRect(int id, out Rect rect);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern int Internal_GetNextKeyboardControlID(bool forward);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void Internal_MoveKeyboardFocus(bool forward);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void Internal_SetupEventValues(object evt);
        internal static void Internal_SwitchSkin()
        {
            skinIndex = 1 - skinIndex;
        }

        /// <summary>
        /// <para>Check if any enabled camera can render to a particular display.</para>
        /// </summary>
        /// <param name="displayIndex">Display index.</param>
        /// <returns>
        /// <para>True if a camera will render to the display.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool IsDisplayReferencedByCameras(int displayIndex);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool IsGizmosAllowedForObject(Object obj);
        /// <summary>
        /// <para>Load a built-in resource.</para>
        /// </summary>
        /// <param name="path"></param>
        public static Object Load(string path) => 
            Load(path, typeof(Object));

        [TypeInferenceRule(TypeInferenceRules.TypeReferencedBySecondArgument)]
        private static Object Load(string filename, Type type)
        {
            Object obj2 = AssetDatabase.LoadAssetAtPath("Assets/Editor Default Resources/" + filename, type);
            if (obj2 != null)
            {
                return obj2;
            }
            AssetBundle editorAssetBundle = GetEditorAssetBundle();
            if (editorAssetBundle == null)
            {
                if (!Application.isBatchmode)
                {
                    throw new NullReferenceException("Failure to load editor resource asset bundle.");
                }
                return null;
            }
            obj2 = editorAssetBundle.LoadAsset(filename, type);
            if (obj2 != null)
            {
                return obj2;
            }
            return AssetDatabase.LoadAssetAtPath(filename, type);
        }

        private static Texture2D LoadGeneratedIconOrNormalIcon(string name)
        {
            Texture2D textured = Load(EditorResourcesUtility.generatedIconsPath + name + ".asset") as Texture2D;
            if (textured == null)
            {
                textured = Load(EditorResourcesUtility.iconsPath + name + ".png") as Texture2D;
            }
            if (textured == null)
            {
                textured = Load(name) as Texture2D;
            }
            return textured;
        }

        internal static Texture2D LoadIcon(string name) => 
            LoadIconForSkin(name, skinIndex);

        internal static Texture2D LoadIconForSkin(string name, int skinIndex)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }
            if (skinIndex == 0)
            {
                return LoadGeneratedIconOrNormalIcon(name);
            }
            string str = "d_" + Path.GetFileName(name);
            string directoryName = Path.GetDirectoryName(name);
            if (!string.IsNullOrEmpty(directoryName))
            {
                str = $"{directoryName}/{str}";
            }
            Texture2D textured2 = LoadGeneratedIconOrNormalIcon(str);
            if (textured2 == null)
            {
                textured2 = LoadGeneratedIconOrNormalIcon(name);
            }
            return textured2;
        }

        internal static Texture2D LoadIconRequired(string name)
        {
            Texture2D textured = LoadIcon(name);
            if (textured == null)
            {
                object[] args = new object[] { name, "Assets/Editor Default Resources/" + EditorResourcesUtility.iconsPath };
                Debug.LogErrorFormat("Unable to load the icon: '{0}'.\nNote that either full project path should be used (with extension) or just the icon name if the icon is located in the following location: '{1}' (without extension, since png is assumed)", args);
            }
            return textured;
        }

        /// <summary>
        /// <para>Load a required built-in resource.</para>
        /// </summary>
        /// <param name="path"></param>
        public static Object LoadRequired(string path)
        {
            Object obj2 = Load(path, typeof(Object));
            if (obj2 == null)
            {
                Debug.LogError("Unable to find required resource at 'Editor Default Resources/" + path + "'");
            }
            return obj2;
        }

        internal static void LockContextWidth()
        {
            s_ContextWidth = CalcContextWidth();
        }

        [Obsolete("LookLikeControls and LookLikeInspector modes are deprecated. Use EditorGUIUtility.labelWidth and EditorGUIUtility.fieldWidth to control label and field widths."), ExcludeFromDocs]
        public static void LookLikeControls()
        {
            float fieldWidth = 0f;
            float labelWidth = 0f;
            LookLikeControls(labelWidth, fieldWidth);
        }

        /// <summary>
        /// <para>Make all EditorGUI look like regular controls.</para>
        /// </summary>
        /// <param name="labelWidth">Width to use for prefixed labels.</param>
        /// <param name="fieldWidth">Width of text entries.</param>
        [ExcludeFromDocs, Obsolete("LookLikeControls and LookLikeInspector modes are deprecated. Use EditorGUIUtility.labelWidth and EditorGUIUtility.fieldWidth to control label and field widths.")]
        public static void LookLikeControls(float labelWidth)
        {
            float fieldWidth = 0f;
            LookLikeControls(labelWidth, fieldWidth);
        }

        /// <summary>
        /// <para>Make all EditorGUI look like regular controls.</para>
        /// </summary>
        /// <param name="labelWidth">Width to use for prefixed labels.</param>
        /// <param name="fieldWidth">Width of text entries.</param>
        [Obsolete("LookLikeControls and LookLikeInspector modes are deprecated. Use EditorGUIUtility.labelWidth and EditorGUIUtility.fieldWidth to control label and field widths.")]
        public static void LookLikeControls([DefaultValue("0")] float labelWidth, [DefaultValue("0")] float fieldWidth)
        {
            EditorGUIUtility.fieldWidth = fieldWidth;
            EditorGUIUtility.labelWidth = labelWidth;
        }

        /// <summary>
        /// <para>Make all EditorGUI look like simplified outline view controls.</para>
        /// </summary>
        [Obsolete("LookLikeControls and LookLikeInspector modes are deprecated.")]
        public static void LookLikeInspector()
        {
            fieldWidth = 0f;
            labelWidth = 0f;
        }

        internal static void MoveFocusAndScroll(bool forward)
        {
            int keyboardControl = GUIUtility.keyboardControl;
            Internal_MoveKeyboardFocus(forward);
            if (keyboardControl != GUIUtility.keyboardControl)
            {
                RefreshScrollPosition();
            }
        }

        internal static void NotifyLanguageChanged(SystemLanguage newLanguage)
        {
            s_TextGUIContents = new Hashtable();
            EditorUtility.Internal_UpdateMenuTitleForLanguage(newLanguage);
            LocalizationDatabase.SetCurrentEditorLanguage(newLanguage);
            EditorApplication.RequestRepaintAllViews();
        }

        /// <summary>
        /// <para>Return a GUIContent object with the name and icon of an Object.</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="type"></param>
        public static GUIContent ObjectContent(Object obj, Type type)
        {
            if (obj != null)
            {
                s_ObjectContent.text = GetObjectNameWithInfo(obj);
                s_ObjectContent.image = AssetPreview.GetMiniThumbnail(obj);
            }
            else if (type != null)
            {
                s_ObjectContent.text = GetTypeNameWithInfo(type.Name);
                s_ObjectContent.image = AssetPreview.GetMiniTypeThumbnail(type);
            }
            else
            {
                s_ObjectContent.text = "<no type>";
                s_ObjectContent.image = null;
            }
            return s_ObjectContent;
        }

        /// <summary>
        /// <para>Ping an object in the Scene like clicking it in an inspector.</para>
        /// </summary>
        /// <param name="obj">The object to be pinged.</param>
        /// <param name="targetInstanceID"></param>
        public static void PingObject(int targetInstanceID)
        {
            foreach (SceneHierarchyWindow window in SceneHierarchyWindow.GetAllSceneHierarchyWindows())
            {
                bool ping = true;
                window.FrameObject(targetInstanceID, ping);
            }
            foreach (ProjectBrowser browser in ProjectBrowser.GetAllProjectBrowsers())
            {
                bool flag2 = true;
                browser.FrameObject(targetInstanceID, flag2);
            }
        }

        /// <summary>
        /// <para>Ping an object in the Scene like clicking it in an inspector.</para>
        /// </summary>
        /// <param name="obj">The object to be pinged.</param>
        /// <param name="targetInstanceID"></param>
        public static void PingObject(Object obj)
        {
            if (obj != null)
            {
                PingObject(obj.GetInstanceID());
            }
        }

        /// <summary>
        /// <para>Convert a Rect from pixel space to point space.</para>
        /// </summary>
        /// <param name="rect">A GUI rect measured in pixels.</param>
        /// <returns>
        /// <para>A rect representing the same area in points.</para>
        /// </returns>
        public static Rect PixelsToPoints(Rect rect)
        {
            float num = 1f / pixelsPerPoint;
            rect.x *= num;
            rect.y *= num;
            rect.width *= num;
            rect.height *= num;
            return rect;
        }

        /// <summary>
        /// <para>Convert a position from pixel to point space.</para>
        /// </summary>
        /// <param name="position">A GUI position in pixel space.</param>
        /// <returns>
        /// <para>A vector representing the same position in point space.</para>
        /// </returns>
        public static Vector2 PixelsToPoints(Vector2 position)
        {
            float num = 1f / pixelsPerPoint;
            position.x *= num;
            position.y *= num;
            return position;
        }

        /// <summary>
        /// <para>Converts a position from point to pixel space.</para>
        /// </summary>
        /// <param name="rect">A GUI position in point space.</param>
        /// <returns>
        /// <para>The same position in pixel space.</para>
        /// </returns>
        public static Rect PointsToPixels(Rect rect)
        {
            float pixelsPerPoint = EditorGUIUtility.pixelsPerPoint;
            rect.x *= pixelsPerPoint;
            rect.y *= pixelsPerPoint;
            rect.width *= pixelsPerPoint;
            rect.height *= pixelsPerPoint;
            return rect;
        }

        /// <summary>
        /// <para>Convert a Rect from point space to pixel space.</para>
        /// </summary>
        /// <param name="position">A GUI rect measured in points.</param>
        /// <returns>
        /// <para>A rect representing the same area in pixels.</para>
        /// </returns>
        public static Vector2 PointsToPixels(Vector2 position)
        {
            float pixelsPerPoint = EditorGUIUtility.pixelsPerPoint;
            position.x *= pixelsPerPoint;
            position.y *= pixelsPerPoint;
            return position;
        }

        /// <summary>
        /// <para>Send an input event into the game.</para>
        /// </summary>
        /// <param name="evt"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void QueueGameViewInputEvent(Event evt);
        internal static void RefreshScrollPosition()
        {
            Rect rect;
            if (Internal_GetKeyboardRect(GUIUtility.keyboardControl, out rect))
            {
                GUI.ScrollTo(rect);
            }
        }

        /// <summary>
        /// <para>Render all ingame cameras.</para>
        /// </summary>
        /// <param name="cameraRect">The device coordinates to render all game cameras into.</param>
        /// <param name="gizmos">Show gizmos as well.</param>
        /// <param name="gui"></param>
        /// <param name="statsRect"></param>
        [Obsolete("RenderGameViewCameras is no longer supported. Consider rendering cameras manually.", true)]
        public static void RenderGameViewCameras(Rect cameraRect, bool gizmos, bool gui)
        {
        }

        /// <summary>
        /// <para>Render all ingame cameras.</para>
        /// </summary>
        /// <param name="cameraRect">The device coordinates to render all game cameras into.</param>
        /// <param name="gizmos">Show gizmos as well.</param>
        /// <param name="gui"></param>
        /// <param name="statsRect"></param>
        [Obsolete("RenderGameViewCameras is no longer supported. Consider rendering cameras manually.", true)]
        public static void RenderGameViewCameras(Rect cameraRect, Rect statsRect, bool gizmos, bool gui)
        {
        }

        [Obsolete("RenderGameViewCameras is no longer supported. Consider rendering cameras manually.", true)]
        public static void RenderGameViewCameras(RenderTexture target, int targetDisplay, Rect screenRect, Vector2 mousePosition, bool gizmos)
        {
            INTERNAL_CALL_RenderGameViewCameras(target, targetDisplay, ref screenRect, ref mousePosition, gizmos);
        }

        internal static void RenderGameViewCamerasInternal(RenderTexture target, int targetDisplay, Rect screenRect, Vector2 mousePosition, bool gizmos)
        {
            INTERNAL_CALL_RenderGameViewCamerasInternal(target, targetDisplay, ref screenRect, ref mousePosition, gizmos);
        }

        internal static void RepaintCurrentWindow()
        {
            GUIUtility.CheckOnGUI();
            GUIView.current.Repaint();
        }

        internal static void ResetGUIState()
        {
            GUI.skin = null;
            Color white = Color.white;
            GUI.contentColor = white;
            GUI.backgroundColor = white;
            GUI.color = !EditorApplication.isPlayingOrWillChangePlaymode ? Color.white : ((Color) HostView.kPlayModeDarken);
            GUI.enabled = true;
            GUI.changed = false;
            EditorGUI.indentLevel = 0;
            EditorGUI.ClearStacks();
            fieldWidth = 0f;
            labelWidth = 0f;
            SetBoldDefaultFont(false);
            UnlockContextWidth();
            hierarchyMode = false;
            wideMode = false;
            ScriptAttributeUtility.propertyHandlerCache = null;
        }

        [Obsolete("EditorGUIUtility.RGBToHSV is obsolete. Use Color.RGBToHSV instead (UnityUpgradable) -> [UnityEngine] UnityEngine.Color.RGBToHSV(*)", true)]
        public static void RGBToHSV(Color rgbColor, out float H, out float S, out float V)
        {
            Color.RGBToHSV(rgbColor, out H, out S, out V);
        }

        internal static void ScrollForTabbing(bool forward)
        {
            Rect rect;
            if (Internal_GetKeyboardRect(Internal_GetNextKeyboardControlID(forward), out rect))
            {
                GUI.ScrollTo(rect);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string SerializeMainMenuToString();
        internal static void SetBoldDefaultFont(bool isBold)
        {
            int num = !isBold ? 0 : 1;
            if (num != s_FontIsBold)
            {
                SetDefaultFont(!isBold ? EditorStyles.standardFont : EditorStyles.boldFont);
                s_FontIsBold = num;
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void SetDefaultFont(Font font);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void SetIconForObject(Object obj, Texture2D icon);
        /// <summary>
        /// <para>Set icons rendered as part of GUIContent to be rendered at a specific size.</para>
        /// </summary>
        /// <param name="size"></param>
        public static void SetIconSize(Vector2 size)
        {
            INTERNAL_CALL_SetIconSize(ref size);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void SetLockedLayers(int layers);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void SetMenuLocalizationTestMode(bool onoff);
        internal static void SetPasteboardColor(Color color)
        {
            INTERNAL_CALL_SetPasteboardColor(ref color);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void SetRenderTextureNoViewport(RenderTexture rt);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void SetSearchIndexOfControlIDList(int index);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void SetVisibleLayers(int layers);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void SetWantsMouseJumping(int wantz);
        public static void ShowObjectPicker<T>(Object obj, bool allowSceneObjects, string searchFilter, int controlID) where T: Object
        {
            Type requiredType = typeof(T);
            ObjectSelector.get.Show(obj, requiredType, null, allowSceneObjects);
            ObjectSelector.get.objectSelectorID = controlID;
            ObjectSelector.get.searchFilter = searchFilter;
        }

        internal static void SkinChanged()
        {
            EditorStyles.UpdateSkinCache();
        }

        internal static GUIContent TempContent(string t)
        {
            s_Text.text = t;
            return s_Text;
        }

        internal static GUIContent TempContent(Texture i)
        {
            s_Image.image = i;
            return s_Image;
        }

        internal static GUIContent[] TempContent(string[] texts)
        {
            GUIContent[] contentArray = new GUIContent[texts.Length];
            for (int i = 0; i < texts.Length; i++)
            {
                contentArray[i] = new GUIContent(texts[i]);
            }
            return contentArray;
        }

        internal static GUIContent TempContent(string t, Texture i)
        {
            s_TextImage.image = i;
            s_TextImage.text = t;
            return s_TextImage;
        }

        internal static GUIContent TextContent(string textAndTooltip)
        {
            if (textAndTooltip == null)
            {
                textAndTooltip = "";
            }
            GUIContent content = (GUIContent) s_TextGUIContents[textAndTooltip];
            if (content == null)
            {
                string[] nameAndTooltipString = GetNameAndTooltipString(textAndTooltip);
                content = new GUIContent(nameAndTooltipString[1]);
                if (nameAndTooltipString[2] != null)
                {
                    content.tooltip = nameAndTooltipString[2];
                }
                s_TextGUIContents[textAndTooltip] = content;
            }
            return content;
        }

        internal static GUIContent TextContentWithIcon(string textAndTooltip, string icon)
        {
            if (textAndTooltip == null)
            {
                textAndTooltip = "";
            }
            GUIContent content = (GUIContent) s_TextGUIContents[textAndTooltip];
            if (content == null)
            {
                string[] nameAndTooltipString = GetNameAndTooltipString(textAndTooltip);
                content = new GUIContent(nameAndTooltipString[1]) {
                    image = LoadIconRequired(icon)
                };
                if (nameAndTooltipString[2] != null)
                {
                    content.tooltip = nameAndTooltipString[2];
                }
                s_TextGUIContents[textAndTooltip] = content;
            }
            return content;
        }

        internal static void UnlockContextWidth()
        {
            s_ContextWidth = 0f;
        }

        internal static GUIContent blankContent =>
            s_BlankContent;

        internal static float contextWidth
        {
            get
            {
                if (s_ContextWidth > 0f)
                {
                    return s_ContextWidth;
                }
                return CalcContextWidth();
            }
        }

        /// <summary>
        /// <para>The width of the GUI area for the current EditorWindow or other view.</para>
        /// </summary>
        public static float currentViewWidth =>
            GUIView.current.position.width;

        /// <summary>
        /// <para>Is a text field currently editing text?</para>
        /// </summary>
        public static bool editingTextField
        {
            get => 
                EditorGUI.RecycledTextEditor.s_ActuallyEditing;
            set
            {
                EditorGUI.RecycledTextEditor.s_ActuallyEditing = value;
            }
        }

        internal static Texture2D errorIcon
        {
            get
            {
                if (s_ErrorIcon == null)
                {
                    s_ErrorIcon = LoadIcon("console.erroricon");
                }
                return s_ErrorIcon;
            }
        }

        /// <summary>
        /// <para>The minimum width in pixels reserved for the fields of Editor GUI controls.</para>
        /// </summary>
        public static float fieldWidth
        {
            get
            {
                if (s_FieldWidth > 0f)
                {
                    return s_FieldWidth;
                }
                return 50f;
            }
            set
            {
                s_FieldWidth = value;
            }
        }

        /// <summary>
        /// <para>Is the Editor GUI is hierarchy mode?</para>
        /// </summary>
        public static bool hierarchyMode
        {
            get => 
                s_HierarchyMode;
            set
            {
                s_HierarchyMode = value;
            }
        }

        internal static Texture2D infoIcon
        {
            get
            {
                if (s_InfoIcon == null)
                {
                    s_InfoIcon = LoadIcon("console.infoicon");
                }
                return s_InfoIcon;
            }
        }

        /// <summary>
        /// <para>Is the user currently using the pro skin? (Read Only)</para>
        /// </summary>
        public static bool isProSkin =>
            (skinIndex == 1);

        /// <summary>
        /// <para>The width in pixels reserved for labels of Editor GUI controls.</para>
        /// </summary>
        public static float labelWidth
        {
            get
            {
                if (s_LabelWidth > 0f)
                {
                    return s_LabelWidth;
                }
                if (s_HierarchyMode)
                {
                    return Mathf.Max((float) ((contextWidth * 0.45f) - 40f), (float) 120f);
                }
                return 150f;
            }
            set
            {
                s_LabelWidth = value;
            }
        }

        internal static EventType magnifyGestureEventType =>
            ((EventType) 0x3e8);

        /// <summary>
        /// <para>The scale of GUI points relative to screen pixels for the current view
        /// 
        /// This value is the number of screen pixels per point of interface space. For instance, 2.0 on retina displays. Note that the value may differ from one view to the next if the views are on monitors with different UI scales.</para>
        /// </summary>
        public static float pixelsPerPoint =>
            GUIUtility.pixelsPerPoint;

        internal static EventType rotateGestureEventType =>
            ((EventType) 0x3ea);

        /// <summary>
        /// <para>Get the height used for a single Editor control such as a one-line EditorGUI.TextField or EditorGUI.Popup.</para>
        /// </summary>
        public static float singleLineHeight =>
            16f;

        internal static int skinIndex { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Get the height used by default for vertical spacing between controls.</para>
        /// </summary>
        public static float standardVerticalSpacing =>
            2f;

        internal static EventType swipeGestureEventType =>
            ((EventType) 0x3e9);

        /// <summary>
        /// <para>The system copy buffer.</para>
        /// </summary>
        public static string systemCopyBuffer { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        internal static Texture2D warningIcon
        {
            get
            {
                if (s_WarningIcon == null)
                {
                    s_WarningIcon = LoadIcon("console.warnicon");
                }
                return s_WarningIcon;
            }
        }

        /// <summary>
        /// <para>Get a white texture.</para>
        /// </summary>
        public static Texture2D whiteTexture { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        internal static GUIStyle whiteTextureStyle
        {
            get
            {
                if (s_WhiteTextureStyle == null)
                {
                    s_WhiteTextureStyle = new GUIStyle();
                    s_WhiteTextureStyle.normal.background = whiteTexture;
                }
                return s_WhiteTextureStyle;
            }
        }

        /// <summary>
        /// <para>Is the Editor GUI currently in wide mode?</para>
        /// </summary>
        public static bool wideMode
        {
            get => 
                s_WideMode;
            set
            {
                s_WideMode = value;
            }
        }

        internal class SkinnedColor
        {
            private Color normalColor;
            private Color proColor;

            public SkinnedColor(Color color)
            {
                this.normalColor = color;
                this.proColor = color;
            }

            public SkinnedColor(Color color, Color proColor)
            {
                this.normalColor = color;
                this.proColor = proColor;
            }

            public static implicit operator Color(EditorGUIUtility.SkinnedColor colorSkin) => 
                colorSkin.color;

            public Color color
            {
                get
                {
                    if (EditorGUIUtility.isProSkin)
                    {
                        return this.proColor;
                    }
                    return this.normalColor;
                }
                set
                {
                    if (EditorGUIUtility.isProSkin)
                    {
                        this.proColor = value;
                    }
                    else
                    {
                        this.normalColor = value;
                    }
                }
            }
        }
    }
}

