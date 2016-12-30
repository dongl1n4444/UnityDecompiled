namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using UnityEngine.Internal;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Accesses remote settings (common for all game instances).</para>
    /// </summary>
    public sealed class RemoteSettings
    {
        public static  event UpdatedEventHandler Updated;

        [RequiredByNativeCode]
        public static void CallOnUpdate()
        {
            UpdatedEventHandler updated = Updated;
            if (updated != null)
            {
                updated();
            }
        }

        /// <summary>
        /// <para>Returns the value corresponding to key in the remote settings if it exists.</para>
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
        /// <para>Returns the value corresponding to key in the remote settings if it exists.</para>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool GetBool(string key, [DefaultValue("false")] bool defaultValue);
        /// <summary>
        /// <para>Returns the value corresponding to key in the remote settings if it exists.</para>
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
        /// <para>Returns the value corresponding to key in the remote settings if it exists.</para>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern float GetFloat(string key, [DefaultValue("0.0F")] float defaultValue);
        /// <summary>
        /// <para>Returns the value corresponding to key in the remote settings if it exists.</para>
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
        /// <para>Returns the value corresponding to key in the remote settings if it exists.</para>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern int GetInt(string key, [DefaultValue("0")] int defaultValue);
        /// <summary>
        /// <para>Returns the value corresponding to key in the remote settings if it exists.</para>
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
        /// <para>Returns the value corresponding to key in the remote settings if it exists.</para>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern string GetString(string key, [DefaultValue("\"\"")] string defaultValue);
        /// <summary>
        /// <para>Returns true if key exists in the remote settings.</para>
        /// </summary>
        /// <param name="key"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool HasKey(string key);

        /// <summary>
        /// <para>This event occurs when a new RemoteSettings is fetched and successfully parsed from the server.</para>
        /// </summary>
        public delegate void UpdatedEventHandler();
    }
}

