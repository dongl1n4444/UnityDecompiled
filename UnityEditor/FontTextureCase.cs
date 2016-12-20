namespace UnityEditor
{
    using System;

    /// <summary>
    /// <para>Texture case constants for TrueTypeFontImporter.</para>
    /// </summary>
    public enum FontTextureCase
    {
        /// <summary>
        /// <para>Import basic ASCII character set.</para>
        /// </summary>
        ASCII = 0,
        /// <summary>
        /// <para>Only import lower case ASCII character set.</para>
        /// </summary>
        ASCIILowerCase = 2,
        /// <summary>
        /// <para>Only import upper case ASCII character set.</para>
        /// </summary>
        ASCIIUpperCase = 1,
        /// <summary>
        /// <para>Custom set of characters.</para>
        /// </summary>
        CustomSet = 3,
        /// <summary>
        /// <para>Render characters into font texture at runtime as needed.</para>
        /// </summary>
        Dynamic = -2,
        /// <summary>
        /// <para>Import a set of Unicode characters common for latin scripts.</para>
        /// </summary>
        Unicode = -1
    }
}

