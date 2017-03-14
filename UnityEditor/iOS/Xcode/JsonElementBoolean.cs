namespace UnityEditor.iOS.Xcode
{
    using System;

    internal class JsonElementBoolean : JsonElement
    {
        public bool value;

        public JsonElementBoolean(bool v)
        {
            this.value = v;
        }
    }
}

