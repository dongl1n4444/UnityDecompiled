namespace UnityEditor.Android.PostProcessor
{
    using System;
    using System.Collections.Generic;

    internal class PostProcessorContext
    {
        private Dictionary<string, object> _data = new Dictionary<string, object>();

        public T Get<T>(string key) => 
            ((T) this._data[key]);

        public void Set<T>(string key, T value)
        {
            this._data[key] = value;
        }
    }
}

