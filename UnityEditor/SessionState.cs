namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>SessionState is a Key-Value Store intended for storing and retrieving Editor session state that should survive assembly reloading.</para>
    /// </summary>
    public static class SessionState
    {
        /// <summary>
        /// <para>Erase a Boolean entry in the key-value store.</para>
        /// </summary>
        /// <param name="key"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void EraseBool(string key);
        /// <summary>
        /// <para>Erase a Float entry in the key-value store.</para>
        /// </summary>
        /// <param name="key"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void EraseFloat(string key);
        /// <summary>
        /// <para>Erase an Integer entry in the key-value store.</para>
        /// </summary>
        /// <param name="key"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void EraseInt(string key);
        /// <summary>
        /// <para>Erase an Integer array entry in the key-value store.</para>
        /// </summary>
        /// <param name="key"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void EraseIntArray(string key);
        /// <summary>
        /// <para>Erase a String entry in the key-value store.</para>
        /// </summary>
        /// <param name="key"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void EraseString(string key);
        /// <summary>
        /// <para>Erase a Vector3 entry in the key-value store.</para>
        /// </summary>
        /// <param name="key"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void EraseVector3(string key);
        /// <summary>
        /// <para>Retrieve a Boolean value.</para>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool GetBool(string key, bool defaultValue);
        /// <summary>
        /// <para>Retrieve a Float value.</para>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern float GetFloat(string key, float defaultValue);
        /// <summary>
        /// <para>Retrieve an Integer value.</para>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern int GetInt(string key, int defaultValue);
        /// <summary>
        /// <para>Retrieve an Integer array.</para>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern int[] GetIntArray(string key, int[] defaultValue);
        /// <summary>
        /// <para>Retrieve a String value.</para>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern string GetString(string key, string defaultValue);
        /// <summary>
        /// <para>Retrieve a Vector3.</para>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        public static Vector3 GetVector3(string key, Vector3 defaultValue)
        {
            Vector3 vector;
            INTERNAL_CALL_GetVector3(key, ref defaultValue, out vector);
            return vector;
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetVector3(string key, ref Vector3 defaultValue, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_SetVector3(string key, ref Vector3 value);
        /// <summary>
        /// <para>Store a Boolean value.</para>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void SetBool(string key, bool value);
        /// <summary>
        /// <para>Store a Float value.</para>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void SetFloat(string key, float value);
        /// <summary>
        /// <para>Store an Integer value.</para>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void SetInt(string key, int value);
        /// <summary>
        /// <para>Store an Integer array.</para>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void SetIntArray(string key, int[] value);
        /// <summary>
        /// <para>Store a String value.</para>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void SetString(string key, string value);
        /// <summary>
        /// <para>Store a Vector3.</para>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetVector3(string key, Vector3 value)
        {
            INTERNAL_CALL_SetVector3(key, ref value);
        }
    }
}

