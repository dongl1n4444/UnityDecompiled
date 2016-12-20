namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Styling information for GUI elements.</para>
    /// </summary>
    [Serializable, StructLayout(LayoutKind.Sequential), RequiredByNativeCode]
    public sealed class GUIStyle
    {
        [NonSerialized]
        internal IntPtr m_Ptr;
        [NonSerialized]
        private GUIStyleState m_Normal;
        [NonSerialized]
        private GUIStyleState m_Hover;
        [NonSerialized]
        private GUIStyleState m_Active;
        [NonSerialized]
        private GUIStyleState m_Focused;
        [NonSerialized]
        private GUIStyleState m_OnNormal;
        [NonSerialized]
        private GUIStyleState m_OnHover;
        [NonSerialized]
        private GUIStyleState m_OnActive;
        [NonSerialized]
        private GUIStyleState m_OnFocused;
        [NonSerialized]
        private RectOffset m_Border;
        [NonSerialized]
        private RectOffset m_Padding;
        [NonSerialized]
        private RectOffset m_Margin;
        [NonSerialized]
        private RectOffset m_Overflow;
        [NonSerialized]
        private Font m_FontInternal;
        internal static bool showKeyboardFocus = true;
        private static GUIStyle s_None;
        /// <summary>
        /// <para>Constructor for empty GUIStyle.</para>
        /// </summary>
        public GUIStyle()
        {
            this.Init();
        }

        /// <summary>
        /// <para>Constructs GUIStyle identical to given other GUIStyle.</para>
        /// </summary>
        /// <param name="other"></param>
        public GUIStyle(GUIStyle other)
        {
            this.InitCopy(other);
        }

        ~GUIStyle()
        {
            this.Cleanup();
        }

        internal static void CleanupRoots()
        {
            s_None = null;
        }

        internal void InternalOnAfterDeserialize()
        {
            this.m_FontInternal = this.GetFontInternalDuringLoadingThread();
            this.m_Normal = GUIStyleState.ProduceGUIStyleStateFromDeserialization(this, this.GetStyleStatePtr(0));
            this.m_Hover = GUIStyleState.ProduceGUIStyleStateFromDeserialization(this, this.GetStyleStatePtr(1));
            this.m_Active = GUIStyleState.ProduceGUIStyleStateFromDeserialization(this, this.GetStyleStatePtr(2));
            this.m_Focused = GUIStyleState.ProduceGUIStyleStateFromDeserialization(this, this.GetStyleStatePtr(3));
            this.m_OnNormal = GUIStyleState.ProduceGUIStyleStateFromDeserialization(this, this.GetStyleStatePtr(4));
            this.m_OnHover = GUIStyleState.ProduceGUIStyleStateFromDeserialization(this, this.GetStyleStatePtr(5));
            this.m_OnActive = GUIStyleState.ProduceGUIStyleStateFromDeserialization(this, this.GetStyleStatePtr(6));
            this.m_OnFocused = GUIStyleState.ProduceGUIStyleStateFromDeserialization(this, this.GetStyleStatePtr(7));
        }

        /// <summary>
        /// <para>Rendering settings for when the component is displayed normally.</para>
        /// </summary>
        public GUIStyleState normal
        {
            get
            {
                if (this.m_Normal == null)
                {
                    this.m_Normal = GUIStyleState.GetGUIStyleState(this, this.GetStyleStatePtr(0));
                }
                return this.m_Normal;
            }
            set
            {
                this.AssignStyleState(0, value.m_Ptr);
            }
        }
        /// <summary>
        /// <para>Rendering settings for when the mouse is hovering over the control.</para>
        /// </summary>
        public GUIStyleState hover
        {
            get
            {
                if (this.m_Hover == null)
                {
                    this.m_Hover = GUIStyleState.GetGUIStyleState(this, this.GetStyleStatePtr(1));
                }
                return this.m_Hover;
            }
            set
            {
                this.AssignStyleState(1, value.m_Ptr);
            }
        }
        /// <summary>
        /// <para>Rendering settings for when the control is pressed down.</para>
        /// </summary>
        public GUIStyleState active
        {
            get
            {
                if (this.m_Active == null)
                {
                    this.m_Active = GUIStyleState.GetGUIStyleState(this, this.GetStyleStatePtr(2));
                }
                return this.m_Active;
            }
            set
            {
                this.AssignStyleState(2, value.m_Ptr);
            }
        }
        /// <summary>
        /// <para>Rendering settings for when the control is turned on.</para>
        /// </summary>
        public GUIStyleState onNormal
        {
            get
            {
                if (this.m_OnNormal == null)
                {
                    this.m_OnNormal = GUIStyleState.GetGUIStyleState(this, this.GetStyleStatePtr(4));
                }
                return this.m_OnNormal;
            }
            set
            {
                this.AssignStyleState(4, value.m_Ptr);
            }
        }
        /// <summary>
        /// <para>Rendering settings for when the control is turned on and the mouse is hovering it.</para>
        /// </summary>
        public GUIStyleState onHover
        {
            get
            {
                if (this.m_OnHover == null)
                {
                    this.m_OnHover = GUIStyleState.GetGUIStyleState(this, this.GetStyleStatePtr(5));
                }
                return this.m_OnHover;
            }
            set
            {
                this.AssignStyleState(5, value.m_Ptr);
            }
        }
        /// <summary>
        /// <para>Rendering settings for when the element is turned on and pressed down.</para>
        /// </summary>
        public GUIStyleState onActive
        {
            get
            {
                if (this.m_OnActive == null)
                {
                    this.m_OnActive = GUIStyleState.GetGUIStyleState(this, this.GetStyleStatePtr(6));
                }
                return this.m_OnActive;
            }
            set
            {
                this.AssignStyleState(6, value.m_Ptr);
            }
        }
        /// <summary>
        /// <para>Rendering settings for when the element has keyboard focus.</para>
        /// </summary>
        public GUIStyleState focused
        {
            get
            {
                if (this.m_Focused == null)
                {
                    this.m_Focused = GUIStyleState.GetGUIStyleState(this, this.GetStyleStatePtr(3));
                }
                return this.m_Focused;
            }
            set
            {
                this.AssignStyleState(3, value.m_Ptr);
            }
        }
        /// <summary>
        /// <para>Rendering settings for when the element has keyboard and is turned on.</para>
        /// </summary>
        public GUIStyleState onFocused
        {
            get
            {
                if (this.m_OnFocused == null)
                {
                    this.m_OnFocused = GUIStyleState.GetGUIStyleState(this, this.GetStyleStatePtr(7));
                }
                return this.m_OnFocused;
            }
            set
            {
                this.AssignStyleState(7, value.m_Ptr);
            }
        }
        /// <summary>
        /// <para>The borders of all background images.</para>
        /// </summary>
        public RectOffset border
        {
            get
            {
                if (this.m_Border == null)
                {
                    this.m_Border = new RectOffset(this, this.GetRectOffsetPtr(0));
                }
                return this.m_Border;
            }
            set
            {
                this.AssignRectOffset(0, value.m_Ptr);
            }
        }
        /// <summary>
        /// <para>The margins between elements rendered in this style and any other GUI elements.</para>
        /// </summary>
        public RectOffset margin
        {
            get
            {
                if (this.m_Margin == null)
                {
                    this.m_Margin = new RectOffset(this, this.GetRectOffsetPtr(1));
                }
                return this.m_Margin;
            }
            set
            {
                this.AssignRectOffset(1, value.m_Ptr);
            }
        }
        /// <summary>
        /// <para>Space from the edge of GUIStyle to the start of the contents.</para>
        /// </summary>
        public RectOffset padding
        {
            get
            {
                if (this.m_Padding == null)
                {
                    this.m_Padding = new RectOffset(this, this.GetRectOffsetPtr(2));
                }
                return this.m_Padding;
            }
            set
            {
                this.AssignRectOffset(2, value.m_Ptr);
            }
        }
        /// <summary>
        /// <para>Extra space to be added to the background image.</para>
        /// </summary>
        public RectOffset overflow
        {
            get
            {
                if (this.m_Overflow == null)
                {
                    this.m_Overflow = new RectOffset(this, this.GetRectOffsetPtr(3));
                }
                return this.m_Overflow;
            }
            set
            {
                this.AssignRectOffset(3, value.m_Ptr);
            }
        }
        [Obsolete("warning Don't use clipOffset - put things inside BeginGroup instead. This functionality will be removed in a later version.")]
        public Vector2 clipOffset
        {
            get
            {
                return this.Internal_clipOffset;
            }
            set
            {
                this.Internal_clipOffset = value;
            }
        }
        /// <summary>
        /// <para>The font to use for rendering. If null, the default font for the current GUISkin is used instead.</para>
        /// </summary>
        public Font font
        {
            get
            {
                return this.GetFontInternal();
            }
            set
            {
                this.SetFontInternal(value);
                this.m_FontInternal = value;
            }
        }
        /// <summary>
        /// <para>The height of one line of text with this style, measured in pixels. (Read Only)</para>
        /// </summary>
        public float lineHeight
        {
            get
            {
                return Mathf.Round(Internal_GetLineHeight(this.m_Ptr));
            }
        }
        private static void Internal_Draw(IntPtr target, Rect position, GUIContent content, bool isHover, bool isActive, bool on, bool hasKeyboardFocus)
        {
            Internal_DrawArguments arguments = new Internal_DrawArguments {
                target = target,
                position = position,
                isHover = !isHover ? 0 : 1,
                isActive = !isActive ? 0 : 1,
                on = !on ? 0 : 1,
                hasKeyboardFocus = !hasKeyboardFocus ? 0 : 1
            };
            Internal_Draw(content, ref arguments);
        }

        /// <summary>
        /// <para>Draw this GUIStyle on to the screen, internal version.</para>
        /// </summary>
        /// <param name="position"></param>
        /// <param name="isHover"></param>
        /// <param name="isActive"></param>
        /// <param name="on"></param>
        /// <param name="hasKeyboardFocus"></param>
        public void Draw(Rect position, bool isHover, bool isActive, bool on, bool hasKeyboardFocus)
        {
            if (Event.current.type != EventType.Repaint)
            {
                Debug.LogError("Style.Draw may not be called if it is not a repaint event");
            }
            else
            {
                Internal_Draw(this.m_Ptr, position, GUIContent.none, isHover, isActive, on, hasKeyboardFocus);
            }
        }

        /// <summary>
        /// <para>Draw the GUIStyle with a text string inside.</para>
        /// </summary>
        /// <param name="position"></param>
        /// <param name="text"></param>
        /// <param name="isHover"></param>
        /// <param name="isActive"></param>
        /// <param name="on"></param>
        /// <param name="hasKeyboardFocus"></param>
        public void Draw(Rect position, string text, bool isHover, bool isActive, bool on, bool hasKeyboardFocus)
        {
            if (Event.current.type != EventType.Repaint)
            {
                Debug.LogError("Style.Draw may not be called if it is not a repaint event");
            }
            else
            {
                Internal_Draw(this.m_Ptr, position, GUIContent.Temp(text), isHover, isActive, on, hasKeyboardFocus);
            }
        }

        /// <summary>
        /// <para>Draw the GUIStyle with an image inside. If the image is too large to fit within the content area of the style it is scaled down.</para>
        /// </summary>
        /// <param name="position"></param>
        /// <param name="image"></param>
        /// <param name="isHover"></param>
        /// <param name="isActive"></param>
        /// <param name="on"></param>
        /// <param name="hasKeyboardFocus"></param>
        public void Draw(Rect position, Texture image, bool isHover, bool isActive, bool on, bool hasKeyboardFocus)
        {
            if (Event.current.type != EventType.Repaint)
            {
                Debug.LogError("Style.Draw may not be called if it is not a repaint event");
            }
            else
            {
                Internal_Draw(this.m_Ptr, position, GUIContent.Temp(image), isHover, isActive, on, hasKeyboardFocus);
            }
        }

        /// <summary>
        /// <para>Draw the GUIStyle with text and an image inside. If the image is too large to fit within the content area of the style it is scaled down.</para>
        /// </summary>
        /// <param name="position"></param>
        /// <param name="content"></param>
        /// <param name="controlID"></param>
        /// <param name="on"></param>
        /// <param name="isHover"></param>
        /// <param name="isActive"></param>
        /// <param name="hasKeyboardFocus"></param>
        public void Draw(Rect position, GUIContent content, bool isHover, bool isActive, bool on, bool hasKeyboardFocus)
        {
            if (Event.current.type != EventType.Repaint)
            {
                Debug.LogError("Style.Draw may not be called if it is not a repaint event");
            }
            else
            {
                Internal_Draw(this.m_Ptr, position, content, isHover, isActive, on, hasKeyboardFocus);
            }
        }

        /// <summary>
        /// <para>Draw the GUIStyle with text and an image inside. If the image is too large to fit within the content area of the style it is scaled down.</para>
        /// </summary>
        /// <param name="position"></param>
        /// <param name="content"></param>
        /// <param name="controlID"></param>
        /// <param name="on"></param>
        /// <param name="isHover"></param>
        /// <param name="isActive"></param>
        /// <param name="hasKeyboardFocus"></param>
        public void Draw(Rect position, GUIContent content, int controlID)
        {
            this.Draw(position, content, controlID, false);
        }

        /// <summary>
        /// <para>Draw the GUIStyle with text and an image inside. If the image is too large to fit within the content area of the style it is scaled down.</para>
        /// </summary>
        /// <param name="position"></param>
        /// <param name="content"></param>
        /// <param name="controlID"></param>
        /// <param name="on"></param>
        /// <param name="isHover"></param>
        /// <param name="isActive"></param>
        /// <param name="hasKeyboardFocus"></param>
        public void Draw(Rect position, GUIContent content, int controlID, bool on)
        {
            if (Event.current.type != EventType.Repaint)
            {
                Debug.LogError("Style.Draw may not be called if it is not a repaint event.");
            }
            else if (content != null)
            {
                Internal_Draw2(this.m_Ptr, position, content, controlID, on);
            }
            else
            {
                Debug.LogError("Style.Draw may not be called with GUIContent that is null.");
            }
        }

        internal void DrawPrefixLabel(Rect position, GUIContent content, int controlID)
        {
            if (content != null)
            {
                Internal_DrawPrefixLabel(this.m_Ptr, position, content, controlID, false);
            }
            else
            {
                Debug.LogError("Style.DrawPrefixLabel may not be called with GUIContent that is null.");
            }
        }

        /// <summary>
        /// <para>Draw this GUIStyle with selected content.</para>
        /// </summary>
        /// <param name="position"></param>
        /// <param name="content"></param>
        /// <param name="controlID"></param>
        /// <param name="Character"></param>
        public void DrawCursor(Rect position, GUIContent content, int controlID, int Character)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Color cursorColor = new Color(0f, 0f, 0f, 0f);
                float cursorFlashSpeed = GUI.skin.settings.cursorFlashSpeed;
                float num2 = ((Time.realtimeSinceStartup - Internal_GetCursorFlashOffset()) % cursorFlashSpeed) / cursorFlashSpeed;
                if ((cursorFlashSpeed == 0f) || (num2 < 0.5f))
                {
                    cursorColor = GUI.skin.settings.cursorColor;
                }
                Internal_DrawCursor(this.m_Ptr, position, content, Character, cursorColor);
            }
        }

        internal void DrawWithTextSelection(Rect position, GUIContent content, int controlID, int firstSelectedCharacter, int lastSelectedCharacter, bool drawSelectionAsComposition)
        {
            if (Event.current.type != EventType.Repaint)
            {
                Debug.LogError("Style.Draw may not be called if it is not a repaint event");
            }
            else
            {
                Event current = Event.current;
                Color cursorColor = new Color(0f, 0f, 0f, 0f);
                float cursorFlashSpeed = GUI.skin.settings.cursorFlashSpeed;
                float num2 = ((Time.realtimeSinceStartup - Internal_GetCursorFlashOffset()) % cursorFlashSpeed) / cursorFlashSpeed;
                if ((cursorFlashSpeed == 0f) || (num2 < 0.5f))
                {
                    cursorColor = GUI.skin.settings.cursorColor;
                }
                Internal_DrawWithTextSelectionArguments arguments = new Internal_DrawWithTextSelectionArguments {
                    target = this.m_Ptr,
                    position = position,
                    firstPos = firstSelectedCharacter,
                    lastPos = lastSelectedCharacter,
                    cursorColor = cursorColor,
                    selectionColor = GUI.skin.settings.selectionColor,
                    isHover = !position.Contains(current.mousePosition) ? 0 : 1,
                    isActive = (controlID != GUIUtility.hotControl) ? 0 : 1,
                    on = 0,
                    hasKeyboardFocus = ((controlID != GUIUtility.keyboardControl) || !showKeyboardFocus) ? 0 : 1,
                    drawSelectionAsComposition = !drawSelectionAsComposition ? 0 : 1
                };
                Internal_DrawWithTextSelection(content, ref arguments);
            }
        }

        /// <summary>
        /// <para>Draw this GUIStyle with selected content.</para>
        /// </summary>
        /// <param name="position"></param>
        /// <param name="content"></param>
        /// <param name="controlID"></param>
        /// <param name="firstSelectedCharacter"></param>
        /// <param name="lastSelectedCharacter"></param>
        public void DrawWithTextSelection(Rect position, GUIContent content, int controlID, int firstSelectedCharacter, int lastSelectedCharacter)
        {
            this.DrawWithTextSelection(position, content, controlID, firstSelectedCharacter, lastSelectedCharacter, false);
        }

        public static implicit operator GUIStyle(string str)
        {
            if (GUISkin.current == null)
            {
                Debug.LogError("Unable to use a named GUIStyle without a current skin. Most likely you need to move your GUIStyle initialization code to OnGUI");
                return GUISkin.error;
            }
            return GUISkin.current.GetStyle(str);
        }

        /// <summary>
        /// <para>Shortcut for an empty GUIStyle.</para>
        /// </summary>
        public static GUIStyle none
        {
            get
            {
                if (s_None == null)
                {
                    s_None = new GUIStyle();
                }
                return s_None;
            }
        }
        /// <summary>
        /// <para>Get the pixel position of a given string index.</para>
        /// </summary>
        /// <param name="position"></param>
        /// <param name="content"></param>
        /// <param name="cursorStringIndex"></param>
        public Vector2 GetCursorPixelPosition(Rect position, GUIContent content, int cursorStringIndex)
        {
            Vector2 vector;
            Internal_GetCursorPixelPosition(this.m_Ptr, position, content, cursorStringIndex, out vector);
            return vector;
        }

        /// <summary>
        /// <para>Get the cursor position (indexing into contents.text) when the user clicked at cursorPixelPosition.</para>
        /// </summary>
        /// <param name="position"></param>
        /// <param name="content"></param>
        /// <param name="cursorPixelPosition"></param>
        public int GetCursorStringIndex(Rect position, GUIContent content, Vector2 cursorPixelPosition)
        {
            return Internal_GetCursorStringIndex(this.m_Ptr, position, content, cursorPixelPosition);
        }

        internal int GetNumCharactersThatFitWithinWidth(string text, float width)
        {
            return Internal_GetNumCharactersThatFitWithinWidth(this.m_Ptr, text, width);
        }

        /// <summary>
        /// <para>Calculate the size of some content if it is rendered with this style.</para>
        /// </summary>
        /// <param name="content"></param>
        public Vector2 CalcSize(GUIContent content)
        {
            Vector2 vector;
            Internal_CalcSize(this.m_Ptr, content, out vector);
            return vector;
        }

        internal Vector2 CalcSizeWithConstraints(GUIContent content, Vector2 constraints)
        {
            Vector2 vector;
            Internal_CalcSizeWithConstraints(this.m_Ptr, content, constraints, out vector);
            return vector;
        }

        /// <summary>
        /// <para>Calculate the size of an element formatted with this style, and a given space to content.</para>
        /// </summary>
        /// <param name="contentSize"></param>
        public Vector2 CalcScreenSize(Vector2 contentSize)
        {
            return new Vector2((this.fixedWidth == 0f) ? Mathf.Ceil((contentSize.x + this.padding.left) + this.padding.right) : this.fixedWidth, (this.fixedHeight == 0f) ? Mathf.Ceil((contentSize.y + this.padding.top) + this.padding.bottom) : this.fixedHeight);
        }

        /// <summary>
        /// <para>How tall this element will be when rendered with content and a specific width.</para>
        /// </summary>
        /// <param name="content"></param>
        /// <param name="width"></param>
        public float CalcHeight(GUIContent content, float width)
        {
            return Internal_CalcHeight(this.m_Ptr, content, width);
        }

        public bool isHeightDependantOnWidth
        {
            get
            {
                return ((this.fixedHeight == 0f) && (this.wordWrap && (this.imagePosition != ImagePosition.ImageOnly)));
            }
        }
        public void CalcMinMaxWidth(GUIContent content, out float minWidth, out float maxWidth)
        {
            Internal_CalcMinMaxWidth(this.m_Ptr, content, out minWidth, out maxWidth);
        }

        public override string ToString()
        {
            object[] args = new object[] { this.name };
            return UnityString.Format("GUIStyle '{0}'", args);
        }

        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        private extern void Init();
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        private extern void InitCopy(GUIStyle other);
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        private extern void Cleanup();
        /// <summary>
        /// <para>The name of this GUIStyle. Used for getting them based on name.</para>
        /// </summary>
        public string name { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
        [ThreadAndSerializationSafe]
        private IntPtr GetStyleStatePtr(int idx)
        {
            IntPtr ptr;
            INTERNAL_CALL_GetStyleStatePtr(this, idx, out ptr);
            return ptr;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_GetStyleStatePtr(GUIStyle self, int idx, out IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void AssignStyleState(int idx, IntPtr srcStyleState);
        private IntPtr GetRectOffsetPtr(int idx)
        {
            IntPtr ptr;
            INTERNAL_CALL_GetRectOffsetPtr(this, idx, out ptr);
            return ptr;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_GetRectOffsetPtr(GUIStyle self, int idx, out IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void AssignRectOffset(int idx, IntPtr srcRectOffset);
        /// <summary>
        /// <para>How image and text of the GUIContent is combined.</para>
        /// </summary>
        public ImagePosition imagePosition { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
        /// <summary>
        /// <para>Text alignment.</para>
        /// </summary>
        public TextAnchor alignment { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
        /// <summary>
        /// <para>Should the text be wordwrapped?</para>
        /// </summary>
        public bool wordWrap { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
        /// <summary>
        /// <para>What to do when the contents to be rendered is too large to fit within the area given.</para>
        /// </summary>
        public TextClipping clipping { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
        /// <summary>
        /// <para>Pixel offset to apply to the content of this GUIstyle.</para>
        /// </summary>
        public Vector2 contentOffset
        {
            get
            {
                Vector2 vector;
                this.INTERNAL_get_contentOffset(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_contentOffset(ref value);
            }
        }
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_contentOffset(out Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_contentOffset(ref Vector2 value);
        internal Vector2 Internal_clipOffset
        {
            get
            {
                Vector2 vector;
                this.INTERNAL_get_Internal_clipOffset(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_Internal_clipOffset(ref value);
            }
        }
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_Internal_clipOffset(out Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_Internal_clipOffset(ref Vector2 value);
        /// <summary>
        /// <para>If non-0, any GUI elements rendered with this style will have the width specified here.</para>
        /// </summary>
        public float fixedWidth { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
        /// <summary>
        /// <para>If non-0, any GUI elements rendered with this style will have the height specified here.</para>
        /// </summary>
        public float fixedHeight { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
        /// <summary>
        /// <para>Can GUI elements of this style be stretched horizontally for better layouting?</para>
        /// </summary>
        public bool stretchWidth { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
        /// <summary>
        /// <para>Can GUI elements of this style be stretched vertically for better layout?</para>
        /// </summary>
        public bool stretchHeight { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern float Internal_GetLineHeight(IntPtr target);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void SetFontInternal(Font value);
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        private extern Font GetFontInternalDuringLoadingThread();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern Font GetFontInternal();
        /// <summary>
        /// <para>The font size to use (for dynamic fonts).</para>
        /// </summary>
        public int fontSize { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
        /// <summary>
        /// <para>The font style to use (for dynamic fonts).</para>
        /// </summary>
        public FontStyle fontStyle { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
        /// <summary>
        /// <para>Enable HTML-style tags for Text Formatting Markup.</para>
        /// </summary>
        public bool richText { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void Internal_Draw(GUIContent content, ref Internal_DrawArguments arguments);
        private static void Internal_Draw2(IntPtr style, Rect position, GUIContent content, int controlID, bool on)
        {
            INTERNAL_CALL_Internal_Draw2(style, ref position, content, controlID, on);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_Internal_Draw2(IntPtr style, ref Rect position, GUIContent content, int controlID, bool on);
        internal void SetMouseTooltip(string tooltip, Rect screenRect)
        {
            INTERNAL_CALL_SetMouseTooltip(this, tooltip, ref screenRect);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_SetMouseTooltip(GUIStyle self, string tooltip, ref Rect screenRect);
        private static void Internal_DrawPrefixLabel(IntPtr style, Rect position, GUIContent content, int controlID, bool on)
        {
            INTERNAL_CALL_Internal_DrawPrefixLabel(style, ref position, content, controlID, on);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_Internal_DrawPrefixLabel(IntPtr style, ref Rect position, GUIContent content, int controlID, bool on);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern float Internal_GetCursorFlashOffset();
        private static void Internal_DrawCursor(IntPtr target, Rect position, GUIContent content, int pos, Color cursorColor)
        {
            INTERNAL_CALL_Internal_DrawCursor(target, ref position, content, pos, ref cursorColor);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_Internal_DrawCursor(IntPtr target, ref Rect position, GUIContent content, int pos, ref Color cursorColor);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void Internal_DrawWithTextSelection(GUIContent content, ref Internal_DrawWithTextSelectionArguments arguments);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void SetDefaultFont(Font font);
        internal static void Internal_GetCursorPixelPosition(IntPtr target, Rect position, GUIContent content, int cursorStringIndex, out Vector2 ret)
        {
            INTERNAL_CALL_Internal_GetCursorPixelPosition(target, ref position, content, cursorStringIndex, out ret);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_Internal_GetCursorPixelPosition(IntPtr target, ref Rect position, GUIContent content, int cursorStringIndex, out Vector2 ret);
        internal static int Internal_GetCursorStringIndex(IntPtr target, Rect position, GUIContent content, Vector2 cursorPixelPosition)
        {
            return INTERNAL_CALL_Internal_GetCursorStringIndex(target, ref position, content, ref cursorPixelPosition);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern int INTERNAL_CALL_Internal_GetCursorStringIndex(IntPtr target, ref Rect position, GUIContent content, ref Vector2 cursorPixelPosition);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern int Internal_GetNumCharactersThatFitWithinWidth(IntPtr target, string text, float width);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Internal_CalcSize(IntPtr target, GUIContent content, out Vector2 ret);
        internal static void Internal_CalcSizeWithConstraints(IntPtr target, GUIContent content, Vector2 maxSize, out Vector2 ret)
        {
            INTERNAL_CALL_Internal_CalcSizeWithConstraints(target, content, ref maxSize, out ret);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_Internal_CalcSizeWithConstraints(IntPtr target, GUIContent content, ref Vector2 maxSize, out Vector2 ret);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern float Internal_CalcHeight(IntPtr target, GUIContent content, float width);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void Internal_CalcMinMaxWidth(IntPtr target, GUIContent content, out float minWidth, out float maxWidth);
    }
}

