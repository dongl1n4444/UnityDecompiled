namespace UnityEngine
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    using UnityEngine.Internal;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Script interface for.</para>
    /// </summary>
    public sealed class Font : UnityEngine.Object
    {
        private event FontTextureRebuildCallback m_FontTextureRebuildCallback;

        public static  event Action<Font> textureRebuilt;

        /// <summary>
        /// <para>Create a new Font.</para>
        /// </summary>
        /// <param name="name">The name of the created Font object.</param>
        public Font()
        {
            Internal_CreateFont(this, null);
        }

        /// <summary>
        /// <para>Create a new Font.</para>
        /// </summary>
        /// <param name="name">The name of the created Font object.</param>
        public Font(string name)
        {
            Internal_CreateFont(this, name);
        }

        private Font(string[] names, int size)
        {
            Internal_CreateDynamicFont(this, names, size);
        }

        /// <summary>
        /// <para>Creates a Font object which lets you render a font installed on the user machine.</para>
        /// </summary>
        /// <param name="fontname">The name of the OS font to use for this font object.</param>
        /// <param name="size">The default character size of the generated font.</param>
        /// <param name="fontnames">Am array of names of OS fonts to use for this font object. When rendering characters using this font object, the first font which is installed on the machine, which contains the requested character will be used.</param>
        /// <returns>
        /// <para>The generate Font object.</para>
        /// </returns>
        public static Font CreateDynamicFontFromOSFont(string fontname, int size) => 
            new Font(new string[] { fontname }, size);

        /// <summary>
        /// <para>Creates a Font object which lets you render a font installed on the user machine.</para>
        /// </summary>
        /// <param name="fontname">The name of the OS font to use for this font object.</param>
        /// <param name="size">The default character size of the generated font.</param>
        /// <param name="fontnames">Am array of names of OS fonts to use for this font object. When rendering characters using this font object, the first font which is installed on the machine, which contains the requested character will be used.</param>
        /// <returns>
        /// <para>The generate Font object.</para>
        /// </returns>
        public static Font CreateDynamicFontFromOSFont(string[] fontnames, int size) => 
            new Font(fontnames, size);

        [ExcludeFromDocs]
        public bool GetCharacterInfo(char ch, out CharacterInfo info)
        {
            FontStyle normal = FontStyle.Normal;
            int size = 0;
            return this.GetCharacterInfo(ch, out info, size, normal);
        }

        [ExcludeFromDocs]
        public bool GetCharacterInfo(char ch, out CharacterInfo info, int size)
        {
            FontStyle normal = FontStyle.Normal;
            return this.GetCharacterInfo(ch, out info, size, normal);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern bool GetCharacterInfo(char ch, out CharacterInfo info, [UnityEngine.Internal.DefaultValue("0")] int size, [UnityEngine.Internal.DefaultValue("FontStyle.Normal")] FontStyle style);
        /// <summary>
        /// <para>Returns the maximum number of verts that the text generator may return for a given string.</para>
        /// </summary>
        /// <param name="str">Input string.</param>
        public static int GetMaxVertsForString(string str) => 
            ((str.Length * 4) + 4);

        /// <summary>
        /// <para>Get names of fonts installed on the machine.</para>
        /// </summary>
        /// <returns>
        /// <para>An array of the names of all fonts installed on the machine.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern string[] GetOSInstalledFontNames();
        /// <summary>
        /// <para>Does this font have a specific character?</para>
        /// </summary>
        /// <param name="c">The character to check for.</param>
        /// <returns>
        /// <para>Whether or not the font has the character specified.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern bool HasCharacter(char c);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void Internal_CreateDynamicFont([Writable] Font _font, string[] _names, int size);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void Internal_CreateFont([Writable] Font _font, string name);
        [RequiredByNativeCode]
        private static void InvokeTextureRebuilt_Internal(Font font)
        {
            Action<Font> textureRebuilt = Font.textureRebuilt;
            if (textureRebuilt != null)
            {
                textureRebuilt(font);
            }
            if (font.m_FontTextureRebuildCallback != null)
            {
                font.m_FontTextureRebuildCallback();
            }
        }

        [ExcludeFromDocs]
        public void RequestCharactersInTexture(string characters)
        {
            FontStyle normal = FontStyle.Normal;
            int size = 0;
            this.RequestCharactersInTexture(characters, size, normal);
        }

        [ExcludeFromDocs]
        public void RequestCharactersInTexture(string characters, int size)
        {
            FontStyle normal = FontStyle.Normal;
            this.RequestCharactersInTexture(characters, size, normal);
        }

        /// <summary>
        /// <para>Request characters to be added to the font texture (dynamic fonts only).</para>
        /// </summary>
        /// <param name="characters">The characters which are needed to be in the font texture.</param>
        /// <param name="size">The size of the requested characters (the default value of zero will use the font's default size).</param>
        /// <param name="style">The style of the requested characters.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void RequestCharactersInTexture(string characters, [UnityEngine.Internal.DefaultValue("0")] int size, [UnityEngine.Internal.DefaultValue("FontStyle.Normal")] FontStyle style);

        /// <summary>
        /// <para>The ascent of the font.</para>
        /// </summary>
        public int ascent { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Access an array of all characters contained in the font texture.</para>
        /// </summary>
        public CharacterInfo[] characterInfo { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Is the font a dynamic font.</para>
        /// </summary>
        public bool dynamic { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        public string[] fontNames { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The default size of the font.</para>
        /// </summary>
        public int fontSize { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>The line height of the font.</para>
        /// </summary>
        public int lineHeight { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>The material used for the font display.</para>
        /// </summary>
        public Material material { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        [Obsolete("Font.textureRebuildCallback has been deprecated. Use Font.textureRebuilt instead."), EditorBrowsable(EditorBrowsableState.Never)]
        public FontTextureRebuildCallback textureRebuildCallback
        {
            get => 
                this.m_FontTextureRebuildCallback;
            set
            {
                this.m_FontTextureRebuildCallback = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public delegate void FontTextureRebuildCallback();
    }
}

