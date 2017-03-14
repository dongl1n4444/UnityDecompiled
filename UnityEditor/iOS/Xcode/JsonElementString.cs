namespace UnityEditor.iOS.Xcode
{
    using System;

    internal class JsonElementString : JsonElement
    {
        public string value;

        public JsonElementString(string v)
        {
            this.value = v;
        }
    }
}

