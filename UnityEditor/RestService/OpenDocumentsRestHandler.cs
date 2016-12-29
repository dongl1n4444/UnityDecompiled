namespace UnityEditor.RestService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditorInternal;

    internal class OpenDocumentsRestHandler : Handler
    {
        [CompilerGenerated]
        private static Func<JSONValue, string> <>f__am$cache0;

        protected override JSONValue HandleGet(Request request, JSONValue payload) => 
            new JSONValue { ["documents"] = Handler.ToJSON(ScriptEditorSettings.OpenDocuments) };

        protected override JSONValue HandlePost(Request request, JSONValue payload)
        {
            JSONValue value2;
            if (payload.ContainsKey("documents"))
            {
                value2 = payload["documents"];
            }
            ScriptEditorSettings.OpenDocuments = (<>f__am$cache0 != null) ? new List<string>() : Enumerable.Select<JSONValue, string>(value2.AsList(), <>f__am$cache0).ToList<string>();
            ScriptEditorSettings.Save();
            return new JSONValue();
        }

        internal static void Register()
        {
            Router.RegisterHandler("/unity/opendocuments", new OpenDocumentsRestHandler());
        }
    }
}

