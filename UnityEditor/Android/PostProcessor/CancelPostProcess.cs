namespace UnityEditor.Android.PostProcessor
{
    using System;
    using UnityEditor;
    using UnityEngine;

    internal class CancelPostProcess
    {
        public static readonly string ConsoleMessage = " See the Console for details.";

        public static void AbortBuild(string title, string message)
        {
            AbortBuild(title, message, new UnityException(title + "\n" + message));
        }

        public static void AbortBuild(string title, string message, Exception ex)
        {
            EditorUtility.DisplayDialog(title, message, "Ok");
            throw ex;
        }

        public static void AbortBuildPointToConsole(string title, string message)
        {
            AbortBuild(title, message + ConsoleMessage);
        }
    }
}

