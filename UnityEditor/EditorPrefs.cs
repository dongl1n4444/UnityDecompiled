namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Internal;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Stores and accesses Unity editor preferences.</para>
    /// </summary>
    public sealed class EditorPrefs
    {
        /// <summary>
        /// <para>Removes all keys and values from the preferences. Use with caution.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void DeleteAll();
        /// <summary>
        /// <para>Removes key and its corresponding value from the preferences.</para>
        /// </summary>
        /// <param name="key"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void DeleteKey(string key);
        /// <summary>
        /// <para>Returns the value corresponding to key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        [ExcludeFromDocs]
        public static bool GetBool(string key)
        {
            bool defaultValue = false;
            return GetBool(key, defaultValue);
        }

        /// <summary>
        /// <para>Returns the value corresponding to key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool GetBool(string key, [DefaultValue("false")] bool defaultValue);
        /// <summary>
        /// <para>Returns the value corresponding to key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        [ExcludeFromDocs]
        public static float GetFloat(string key)
        {
            float defaultValue = 0f;
            return GetFloat(key, defaultValue);
        }

        /// <summary>
        /// <para>Returns the value corresponding to key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern float GetFloat(string key, [DefaultValue("0.0F")] float defaultValue);
        /// <summary>
        /// <para>Returns the value corresponding to key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        [ExcludeFromDocs]
        public static int GetInt(string key)
        {
            int defaultValue = 0;
            return GetInt(key, defaultValue);
        }

        /// <summary>
        /// <para>Returns the value corresponding to key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern int GetInt(string key, [DefaultValue("0")] int defaultValue);
        /// <summary>
        /// <para>Returns the value corresponding to key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        [ExcludeFromDocs]
        public static string GetString(string key)
        {
            string defaultValue = "";
            return GetString(key, defaultValue);
        }

        /// <summary>
        /// <para>Returns the value corresponding to key in the preference file if it exists.</para>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern string GetString(string key, [DefaultValue("\"\"")] string defaultValue);
        /// <summary>
        /// <para>Returns true if key exists in the preferences.</para>
        /// </summary>
        /// <param name="key"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool HasKey(string key);
        /// <summary>
        /// <para>Sets the value of the preference identified by key.</para>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void SetBool(string key, bool value);
        /// <summary>
        /// <para>Sets the value of the preference identified by key.</para>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void SetFloat(string key, float value);
        /// <summary>
        /// <para>Sets the value of the preference identified by key as an integer.</para>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void SetInt(string key, int value);
        /// <summary>
        /// <para>Sets the value of the preference identified by key. Note that EditorPrefs does not support null strings and will store an empty string instead.</para>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void SetString(string key, string value);
    }
}

