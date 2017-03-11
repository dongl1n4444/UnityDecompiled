namespace UnityEditor.Android.PostProcessor
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;
    using UnityEditor;
    using UnityEditor.Android;
    using UnityEngine;

    internal class CancelPostProcess
    {
        public static readonly string ConsoleMessage = " See the Console for details.";
        private static readonly GradleError[] gradleErrors = new GradleError[] { new GradleError(@"^(.*AndroidManifest\.xml):(\d+):(\d+).* AAPT: No resource found that matches the given name \(at '([^']+)' with value '([^']+)'\)\.", "Resource '{5}' not found in AndroidManifest.xml:{2}", "Resource Not Found"), new GradleError("Duplicate files copied in APK (.*)$.*File1: (.*?)$.*File2: (.*?)$", "File '{1}' exists in both '{2}' and '{3}'", "Duplicate Files in APK"), new GradleError(@".*> Error: A library uses the same package as this project: ([^\n]*)", "A library uses the same package as this project: {1}", "Colliding Package Names"), new GradleError(@"Attribute (\S*) value=\(([^)]*).*\n.*also present at \[([^\]]*)\]", "The attribute {1}={2} in {3} collides with another value", "Colliding Attributes"), new GradleError(@"> compileSdkVersion '([^']*)' requires JDK (\S*)", "{1} projects needs JDK {2} or later to compile", "Old Java"), new GradleError("Unable to merge android manifests", "Unrecognized Manifest merging problem", "Manifest Merging"), new GradleError(@".*Execution failed(.*?)\n> ([^\n]*)", "{2}", "") };

        public static void AbortBuild(string title, string message, CommandInvokationFailure ex = null)
        {
            string str = null;
            if ((ex != null) && message.StartsWith("Gradle"))
            {
                string input = string.Join("\n", ex.StdErr);
                foreach (GradleError error in gradleErrors)
                {
                    Match match = error.regex.Match(input);
                    if (match.Success)
                    {
                        System.Text.RegularExpressions.Group[] array = new System.Text.RegularExpressions.Group[match.Groups.Count];
                        match.Groups.CopyTo(array, 0);
                        title = "Gradle Error: " + error.id;
                        message = string.Format(error.message, (object[]) array);
                        str = error.id.ToLower().Replace(' ', '-');
                        break;
                    }
                }
            }
            if (str != null)
            {
                bool flag;
                Debug.Log($"GRADLE ERROR : {str}");
                message = message + "\n(See the Console for details)";
                if (Application.platform == RuntimePlatform.WindowsEditor)
                {
                    flag = EditorUtility.DisplayDialog(title, message, "Troubleshoot", "Ok");
                }
                else
                {
                    flag = EditorUtility.DisplayDialogComplex(title, message, "Ok", "", "Troubleshoot") == 2;
                }
                if (flag)
                {
                    Help.ShowHelpPage($"file:///unity/Manual/android-gradle-troubleshooting.html#{str}");
                }
            }
            else
            {
                EditorUtility.DisplayDialog(title, message, "Ok");
            }
            if (ex == null)
            {
                throw new UnityException(title + "\n" + message);
            }
            throw ex;
        }

        public static void AbortBuildPointToConsole(string title, string message)
        {
            AbortBuild(title, message + ConsoleMessage, null);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct GradleError
        {
            public readonly Regex regex;
            public readonly string message;
            public readonly string id;
            public GradleError(string regex, string msg, string id)
            {
                this.regex = new Regex(regex, RegexOptions.Singleline | RegexOptions.Multiline);
                this.message = msg;
                this.id = id;
            }
        }
    }
}

