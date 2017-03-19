namespace UnityEditor.RestService
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using UnityEditorInternal;
    using UnityEngine;

    internal class ScriptEditorSettings
    {
        [CompilerGenerated]
        private static Func<string, string> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<JSONValue, string> <>f__am$cache1;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static string <Name>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static List<string> <OpenDocuments>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static int <ProcessId>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private static string <ServerURL>k__BackingField;

        static ScriptEditorSettings()
        {
            OpenDocuments = new List<string>();
            Clear();
        }

        private static void Clear()
        {
            Name = null;
            ServerURL = null;
            ProcessId = -1;
        }

        public static void Load()
        {
            try
            {
                JSONValue value6;
                JSONValue value2 = new JSONParser(File.ReadAllText(FilePath)).Parse();
                Name = !value2.ContainsKey("name") ? null : value2["name"].AsString();
                ServerURL = !value2.ContainsKey("serverurl") ? null : value2["serverurl"].AsString();
                ProcessId = !value2.ContainsKey("processid") ? -1 : ((int) value2["processid"].AsFloat());
                if (value2.ContainsKey("opendocuments"))
                {
                    value6 = value2["opendocuments"];
                }
                OpenDocuments = (<>f__am$cache1 != null) ? new List<string>() : Enumerable.Select<JSONValue, string>(value6.AsList(), <>f__am$cache1).ToList<string>();
                if (ProcessId >= 0)
                {
                    Process.GetProcessById(ProcessId);
                }
            }
            catch (FileNotFoundException)
            {
                Clear();
                Save();
            }
            catch (Exception exception)
            {
                UnityEditor.RestService.Logger.Log(exception);
                Clear();
                Save();
            }
        }

        public static void Save()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("{{\n\t\"name\" : \"{0}\",\n\t\"serverurl\" : \"{1}\",\n\t\"processid\" : {2},\n\t", Name, ServerURL, ProcessId);
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = d => "\"" + d + "\"";
            }
            builder.AppendFormat("\"opendocuments\" : [{0}]\n}}", string.Join(",", Enumerable.Select<string, string>(OpenDocuments, <>f__am$cache0).ToArray<string>()));
            File.WriteAllText(FilePath, builder.ToString());
        }

        private static string FilePath =>
            (Application.dataPath + "/../Library/UnityScriptEditorSettings.json");

        public static string Name
        {
            [CompilerGenerated]
            get => 
                <Name>k__BackingField;
            [CompilerGenerated]
            set
            {
                <Name>k__BackingField = value;
            }
        }

        public static List<string> OpenDocuments
        {
            [CompilerGenerated]
            get => 
                <OpenDocuments>k__BackingField;
            [CompilerGenerated]
            set
            {
                <OpenDocuments>k__BackingField = value;
            }
        }

        public static int ProcessId
        {
            [CompilerGenerated]
            get => 
                <ProcessId>k__BackingField;
            [CompilerGenerated]
            set
            {
                <ProcessId>k__BackingField = value;
            }
        }

        public static string ServerURL
        {
            [CompilerGenerated]
            get => 
                <ServerURL>k__BackingField;
            [CompilerGenerated]
            set
            {
                <ServerURL>k__BackingField = value;
            }
        }
    }
}

