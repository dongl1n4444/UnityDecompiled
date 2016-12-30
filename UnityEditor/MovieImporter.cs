namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>AssetImporter for importing MovieTextures.</para>
    /// </summary>
    public sealed class MovieImporter : AssetImporter
    {
        /// <summary>
        /// <para>Duration of the Movie to be imported in seconds.</para>
        /// </summary>
        public float duration { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Is the movie texture storing non-color data?</para>
        /// </summary>
        public bool linearTexture { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Quality setting to use when importing the movie. This is a float value from 0 to 1.</para>
        /// </summary>
        public float quality { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}

