namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Helper class to access Unity documentation.</para>
    /// </summary>
    public sealed class Help
    {
        /// <summary>
        /// <para>Open url in the default web browser.</para>
        /// </summary>
        /// <param name="url"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void BrowseURL(string url);
        /// <summary>
        /// <para>Get the URL for this object's documentation.</para>
        /// </summary>
        /// <param name="obj">The object to retrieve documentation for.</param>
        /// <returns>
        /// <para>The documentation URL for the object. Note that this could use the http: or file: schemas.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern string GetHelpURLForObject(UnityEngine.Object obj);
        internal static string GetNiceHelpNameForObject(UnityEngine.Object obj) => 
            GetNiceHelpNameForObject(obj, true);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern string GetNiceHelpNameForObject(UnityEngine.Object obj, bool defaultToMonoBehaviour);
        /// <summary>
        /// <para>Is there a help page for this object?</para>
        /// </summary>
        /// <param name="obj"></param>
        public static bool HasHelpForObject(UnityEngine.Object obj) => 
            HasHelpForObject(obj, true);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern bool HasHelpForObject(UnityEngine.Object obj, bool defaultToMonoBehaviour);
        /// <summary>
        /// <para>Show help page for this object.</para>
        /// </summary>
        /// <param name="obj"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void ShowHelpForObject(UnityEngine.Object obj);
        /// <summary>
        /// <para>Show a help page.</para>
        /// </summary>
        /// <param name="page"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void ShowHelpPage(string page);
    }
}

