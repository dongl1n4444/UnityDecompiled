namespace UnityEditor.Web
{
    using System;

    internal class JspmStubInfo
    {
        public string[] events = null;
        public JspmMethodInfo[] methods = null;
        public JspmPropertyInfo[] properties = null;

        public JspmStubInfo(JspmPropertyInfo[] properties, JspmMethodInfo[] methods, string[] events)
        {
            this.methods = methods;
            this.properties = properties;
            this.events = events;
        }
    }
}

