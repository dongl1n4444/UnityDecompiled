namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Internal;

    /// <summary>
    /// <para>Stores and accesses player preferences between game sessions.</para>
    /// </summary>
    public sealed class PlayerPrefs
    {
        /// <summary>
        /// <para>Removes all keys and values from the preferences. Use with caution.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void DeleteAll();
        /// <summary>
        /// <para>Removes key and its corresponding value from the preferences.</para>
        /// </summary>
        /// <param name="key"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void DeleteKey(string key);
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
        [MethodImpl(MethodImplOptions.InternalCall)]
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
        [MethodImpl(MethodImplOptions.InternalCall)]
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
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string GetString(string key, [DefaultValue("\"\"")] string defaultValue);
        /// <summary>
        /// <para>Returns true if key exists in the preferences.</para>
        /// </summary>
        /// <param name="key"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool HasKey(string key);
        /// <summary>
        /// <para>Writes all modified preferences to disk.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void Save();
        /// <summary>
        /// <para>Sets the value of the preference identified by key.</para>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetFloat(string key, float value)
        {
            if (!TrySetFloat(key, value))
            {
                throw new PlayerPrefsException("Could not store preference value");
            }
        }

        /// <summary>
        /// <para>Sets the value of the preference identified by key.</para>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetInt(string key, int value)
        {
            if (!TrySetInt(key, value))
            {
                throw new PlayerPrefsException("Could not store preference value");
            }
        }

        /// <summary>
        /// <para>Sets the value of the preference identified by key.</para>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetString(string key, string value)
        {
            if (!TrySetSetString(key, value))
            {
                throw new PlayerPrefsException("Could not store preference value");
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool TrySetFloat(string key, float value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool TrySetInt(string key, int value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool TrySetSetString(string key, string value);
    }
}

