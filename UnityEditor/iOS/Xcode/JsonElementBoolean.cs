namespace UnityEditor.iOS.Xcode
{
    using System;

    public class JsonElementBoolean : JsonElement
    {
        public bool value;

        public JsonElementBoolean(bool v)
        {
            this.value = v;
        }
    }
}

