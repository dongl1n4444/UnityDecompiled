namespace UnityEngine
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Class that can be used to generate text for rendering.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), UsedByNativeCode]
    public sealed class TextGenerator : IDisposable
    {
        internal IntPtr m_Ptr;
        private string m_LastString;
        private TextGenerationSettings m_LastSettings;
        private bool m_HasGenerated;
        private TextGenerationError m_LastValid;
        private readonly List<UIVertex> m_Verts;
        private readonly List<UICharInfo> m_Characters;
        private readonly List<UILineInfo> m_Lines;
        private bool m_CachedVerts;
        private bool m_CachedCharacters;
        private bool m_CachedLines;
        private static int s_NextId = 0;
        private int m_Id;
        private static readonly Dictionary<int, WeakReference> s_Instances = new Dictionary<int, WeakReference>();
        /// <summary>
        /// <para>Create a TextGenerator.</para>
        /// </summary>
        /// <param name="initialCapacity"></param>
        public TextGenerator() : this(50)
        {
        }

        /// <summary>
        /// <para>Create a TextGenerator.</para>
        /// </summary>
        /// <param name="initialCapacity"></param>
        public TextGenerator(int initialCapacity)
        {
            this.m_Verts = new List<UIVertex>((initialCapacity + 1) * 4);
            this.m_Characters = new List<UICharInfo>(initialCapacity + 1);
            this.m_Lines = new List<UILineInfo>(20);
            this.Init();
            object obj2 = s_Instances;
            lock (obj2)
            {
                this.m_Id = s_NextId++;
                s_Instances.Add(this.m_Id, new WeakReference(this));
            }
        }

        ~TextGenerator()
        {
            ((IDisposable) this).Dispose();
        }

        void IDisposable.Dispose()
        {
            object obj2 = s_Instances;
            lock (obj2)
            {
                s_Instances.Remove(this.m_Id);
            }
            this.Dispose_cpp();
        }

        [RequiredByNativeCode]
        internal static void InvalidateAll()
        {
            object obj2 = s_Instances;
            lock (obj2)
            {
                foreach (KeyValuePair<int, WeakReference> pair in s_Instances)
                {
                    WeakReference reference = pair.Value;
                    if (reference.IsAlive)
                    {
                        (reference.Target as TextGenerator).Invalidate();
                    }
                }
            }
        }

        private TextGenerationSettings ValidatedSettings(TextGenerationSettings settings)
        {
            if ((settings.font == null) || !settings.font.dynamic)
            {
                if ((settings.fontSize != 0) || (settings.fontStyle != FontStyle.Normal))
                {
                    if (settings.font != null)
                    {
                        object[] args = new object[] { settings.font.name };
                        Debug.LogWarningFormat(settings.font, "Font size and style overrides are only supported for dynamic fonts. Font '{0}' is not dynamic.", args);
                    }
                    settings.fontSize = 0;
                    settings.fontStyle = FontStyle.Normal;
                }
                if (settings.resizeTextForBestFit)
                {
                    if (settings.font != null)
                    {
                        object[] objArray2 = new object[] { settings.font.name };
                        Debug.LogWarningFormat(settings.font, "BestFit is only supported for dynamic fonts. Font '{0}' is not dynamic.", objArray2);
                    }
                    settings.resizeTextForBestFit = false;
                }
            }
            return settings;
        }

        /// <summary>
        /// <para>Mark the text generator as invalid. This will force a full text generation the next time Populate is called.</para>
        /// </summary>
        public void Invalidate()
        {
            this.m_HasGenerated = false;
        }

        public void GetCharacters(List<UICharInfo> characters)
        {
            this.GetCharactersInternal(characters);
        }

        public void GetLines(List<UILineInfo> lines)
        {
            this.GetLinesInternal(lines);
        }

        public void GetVertices(List<UIVertex> vertices)
        {
            this.GetVerticesInternal(vertices);
        }

        /// <summary>
        /// <para>Given a string and settings, returns the preferred width for a container that would hold this text.</para>
        /// </summary>
        /// <param name="str">Generation text.</param>
        /// <param name="settings">Settings for generation.</param>
        /// <returns>
        /// <para>Preferred width.</para>
        /// </returns>
        public float GetPreferredWidth(string str, TextGenerationSettings settings)
        {
            settings.horizontalOverflow = HorizontalWrapMode.Overflow;
            settings.verticalOverflow = VerticalWrapMode.Overflow;
            settings.updateBounds = true;
            this.Populate(str, settings);
            return this.rectExtents.width;
        }

        /// <summary>
        /// <para>Given a string and settings, returns the preferred height for a container that would hold this text.</para>
        /// </summary>
        /// <param name="str">Generation text.</param>
        /// <param name="settings">Settings for generation.</param>
        /// <returns>
        /// <para>Preferred height.</para>
        /// </returns>
        public float GetPreferredHeight(string str, TextGenerationSettings settings)
        {
            settings.verticalOverflow = VerticalWrapMode.Overflow;
            settings.updateBounds = true;
            this.Populate(str, settings);
            return this.rectExtents.height;
        }

        /// <summary>
        /// <para>Will generate the vertices and other data for the given string with the given settings.</para>
        /// </summary>
        /// <param name="str">String to generate.</param>
        /// <param name="settings">Generation settings.</param>
        /// <param name="context">The object used as context of the error log message, if necessary.</param>
        /// <returns>
        /// <para>True if the generation is a success, false otherwise.</para>
        /// </returns>
        public bool PopulateWithErrors(string str, TextGenerationSettings settings, GameObject context)
        {
            TextGenerationError error = this.PopulateWithError(str, settings);
            if (error == TextGenerationError.None)
            {
                return true;
            }
            if ((error & TextGenerationError.CustomSizeOnNonDynamicFont) != TextGenerationError.None)
            {
                object[] args = new object[] { settings.font };
                Debug.LogErrorFormat(context, "Font '{0}' is not dynamic, which is required to override its size", args);
            }
            if ((error & TextGenerationError.CustomStyleOnNonDynamicFont) != TextGenerationError.None)
            {
                object[] objArray2 = new object[] { settings.font };
                Debug.LogErrorFormat(context, "Font '{0}' is not dynamic, which is required to override its style", objArray2);
            }
            return false;
        }

        /// <summary>
        /// <para>Will generate the vertices and other data for the given string with the given settings.</para>
        /// </summary>
        /// <param name="str">String to generate.</param>
        /// <param name="settings">Settings.</param>
        public bool Populate(string str, TextGenerationSettings settings)
        {
            return (this.PopulateWithError(str, settings) == TextGenerationError.None);
        }

        private TextGenerationError PopulateWithError(string str, TextGenerationSettings settings)
        {
            if ((!this.m_HasGenerated || (str != this.m_LastString)) || !settings.Equals(this.m_LastSettings))
            {
                this.m_LastValid = this.PopulateAlways(str, settings);
            }
            return this.m_LastValid;
        }

        private TextGenerationError PopulateAlways(string str, TextGenerationSettings settings)
        {
            TextGenerationError error;
            this.m_LastString = str;
            this.m_HasGenerated = true;
            this.m_CachedVerts = false;
            this.m_CachedCharacters = false;
            this.m_CachedLines = false;
            this.m_LastSettings = settings;
            TextGenerationSettings settings2 = this.ValidatedSettings(settings);
            this.Populate_Internal(str, settings2.font, settings2.color, settings2.fontSize, settings2.scaleFactor, settings2.lineSpacing, settings2.fontStyle, settings2.richText, settings2.resizeTextForBestFit, settings2.resizeTextMinSize, settings2.resizeTextMaxSize, settings2.verticalOverflow, settings2.horizontalOverflow, settings2.updateBounds, settings2.textAnchor, settings2.generationExtents, settings2.pivot, settings2.generateOutOfBounds, settings2.alignByGeometry, out error);
            this.m_LastValid = error;
            return error;
        }

        /// <summary>
        /// <para>Array of generated vertices.</para>
        /// </summary>
        public IList<UIVertex> verts
        {
            get
            {
                if (!this.m_CachedVerts)
                {
                    this.GetVertices(this.m_Verts);
                    this.m_CachedVerts = true;
                }
                return this.m_Verts;
            }
        }
        /// <summary>
        /// <para>Array of generated characters.</para>
        /// </summary>
        public IList<UICharInfo> characters
        {
            get
            {
                if (!this.m_CachedCharacters)
                {
                    this.GetCharacters(this.m_Characters);
                    this.m_CachedCharacters = true;
                }
                return this.m_Characters;
            }
        }
        /// <summary>
        /// <para>Information about each generated text line.</para>
        /// </summary>
        public IList<UILineInfo> lines
        {
            get
            {
                if (!this.m_CachedLines)
                {
                    this.GetLines(this.m_Lines);
                    this.m_CachedLines = true;
                }
                return this.m_Lines;
            }
        }
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void Init();
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        private extern void Dispose_cpp();
        internal bool Populate_Internal(string str, Font font, Color color, int fontSize, float scaleFactor, float lineSpacing, FontStyle style, bool richText, bool resizeTextForBestFit, int resizeTextMinSize, int resizeTextMaxSize, VerticalWrapMode verticalOverFlow, HorizontalWrapMode horizontalOverflow, bool updateBounds, TextAnchor anchor, Vector2 extents, Vector2 pivot, bool generateOutOfBounds, bool alignByGeometry, out TextGenerationError error)
        {
            uint num = 0;
            bool flag = this.Populate_Internal_cpp(str, font, color, fontSize, scaleFactor, lineSpacing, style, richText, resizeTextForBestFit, resizeTextMinSize, resizeTextMaxSize, (int) verticalOverFlow, (int) horizontalOverflow, updateBounds, anchor, extents.x, extents.y, pivot.x, pivot.y, generateOutOfBounds, alignByGeometry, out num);
            error = (TextGenerationError) num;
            return flag;
        }

        internal bool Populate_Internal_cpp(string str, Font font, Color color, int fontSize, float scaleFactor, float lineSpacing, FontStyle style, bool richText, bool resizeTextForBestFit, int resizeTextMinSize, int resizeTextMaxSize, int verticalOverFlow, int horizontalOverflow, bool updateBounds, TextAnchor anchor, float extentsX, float extentsY, float pivotX, float pivotY, bool generateOutOfBounds, bool alignByGeometry, out uint error)
        {
            return INTERNAL_CALL_Populate_Internal_cpp(this, str, font, ref color, fontSize, scaleFactor, lineSpacing, style, richText, resizeTextForBestFit, resizeTextMinSize, resizeTextMaxSize, verticalOverFlow, horizontalOverflow, updateBounds, anchor, extentsX, extentsY, pivotX, pivotY, generateOutOfBounds, alignByGeometry, out error);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool INTERNAL_CALL_Populate_Internal_cpp(TextGenerator self, string str, Font font, ref Color color, int fontSize, float scaleFactor, float lineSpacing, FontStyle style, bool richText, bool resizeTextForBestFit, int resizeTextMinSize, int resizeTextMaxSize, int verticalOverFlow, int horizontalOverflow, bool updateBounds, TextAnchor anchor, float extentsX, float extentsY, float pivotX, float pivotY, bool generateOutOfBounds, bool alignByGeometry, out uint error);
        /// <summary>
        /// <para>Extents of the generated text in rect format.</para>
        /// </summary>
        public Rect rectExtents
        {
            get
            {
                Rect rect;
                this.INTERNAL_get_rectExtents(out rect);
                return rect;
            }
        }
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_rectExtents(out Rect value);
        /// <summary>
        /// <para>Number of vertices generated.</para>
        /// </summary>
        public int vertexCount { [MethodImpl(MethodImplOptions.InternalCall)] get; }
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void GetVerticesInternal(object vertices);
        /// <summary>
        /// <para>Returns the current UILineInfo.</para>
        /// </summary>
        /// <returns>
        /// <para>Vertices.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern UIVertex[] GetVerticesArray();
        /// <summary>
        /// <para>The number of characters that have been generated.</para>
        /// </summary>
        public int characterCount { [MethodImpl(MethodImplOptions.InternalCall)] get; }
        /// <summary>
        /// <para>The number of characters that have been generated and are included in the visible lines.</para>
        /// </summary>
        public int characterCountVisible
        {
            get
            {
                return (this.characterCount - 1);
            }
        }
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void GetCharactersInternal(object characters);
        /// <summary>
        /// <para>Returns the current UICharInfo.</para>
        /// </summary>
        /// <returns>
        /// <para>Character information.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern UICharInfo[] GetCharactersArray();
        /// <summary>
        /// <para>Number of text lines generated.</para>
        /// </summary>
        public int lineCount { [MethodImpl(MethodImplOptions.InternalCall)] get; }
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void GetLinesInternal(object lines);
        /// <summary>
        /// <para>Returns the current UILineInfo.</para>
        /// </summary>
        /// <returns>
        /// <para>Line information.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern UILineInfo[] GetLinesArray();
        /// <summary>
        /// <para>The size of the font that was found if using best fit mode.</para>
        /// </summary>
        public int fontSizeUsedForBestFit { [MethodImpl(MethodImplOptions.InternalCall)] get; }
    }
}

