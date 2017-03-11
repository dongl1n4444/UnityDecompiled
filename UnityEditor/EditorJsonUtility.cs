namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Utility functions for working with JSON data and engine objects.</para>
    /// </summary>
    public static class EditorJsonUtility
    {
        /// <summary>
        /// <para>Overwrite data in an object by reading from its JSON representation.</para>
        /// </summary>
        /// <param name="json">The JSON representation of the object.</param>
        /// <param name="objectToOverwrite">The object to overwrite.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void FromJsonOverwrite(string json, object objectToOverwrite);
        /// <summary>
        /// <para>Generate a JSON representation of an object.</para>
        /// </summary>
        /// <param name="obj">The object to convert to JSON form.</param>
        /// <param name="prettyPrint">If true, format the output for readability. If false, format the output for minimum size. Default is false.</param>
        /// <returns>
        /// <para>The object's data in JSON format.</para>
        /// </returns>
        public static string ToJson(object obj) => 
            ToJson(obj, false);

        /// <summary>
        /// <para>Generate a JSON representation of an object.</para>
        /// </summary>
        /// <param name="obj">The object to convert to JSON form.</param>
        /// <param name="prettyPrint">If true, format the output for readability. If false, format the output for minimum size. Default is false.</param>
        /// <returns>
        /// <para>The object's data in JSON format.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern string ToJson(object obj, bool prettyPrint);
    }
}

