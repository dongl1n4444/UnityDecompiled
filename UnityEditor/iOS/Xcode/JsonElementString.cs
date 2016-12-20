namespace UnityEditor.iOS.Xcode
{
    using System;

    public class JsonElementString : JsonElement
    {
        public string value;

        public JsonElementString(string v)
        {
            this.value = v;
        }
    }
}

