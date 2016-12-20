namespace Unity.PackageManager.Internal.BackingStore
{
    using System;
    using System.Collections.Generic;

    internal class EditorPrefs
    {
        private static Dictionary<string, object> store = new Dictionary<string, object>();

        public static int GetInt(string key)
        {
            return GetValue<int>(key);
        }

        public static string GetString(string key)
        {
            return GetValue<string>(key);
        }

        private static T GetValue<T>(string key)
        {
            if (store.ContainsKey(key))
            {
                return (T) store[key];
            }
            return default(T);
        }

        public static void SetInt(string key, int value)
        {
            SetValue(key, value);
        }

        public static void SetString(string key, string value)
        {
            SetValue(key, value);
        }

        private static void SetValue(string key, object value)
        {
            if (!store.ContainsKey(key))
            {
                store.Add(key, value);
            }
            else
            {
                store[key] = value;
            }
        }
    }
}

