namespace UnityEditor.TestTools.TestRunner.GUI
{
    using System;
    using UnityEngine;

    internal static class Icons
    {
        public static readonly Texture2D s_FailImg = (EditorGUIUtility.IconContent("TestFailed").image as Texture2D);
        public static readonly Texture2D s_IgnoreImg = (EditorGUIUtility.IconContent("TestIgnored").image as Texture2D);
        public static readonly Texture2D s_InconclusiveImg = (EditorGUIUtility.IconContent("TestInconclusive").image as Texture2D);
        public static readonly Texture2D s_StopwatchImg = (EditorGUIUtility.IconContent("TestStopwatch").image as Texture2D);
        public static readonly Texture2D s_SuccessImg = (EditorGUIUtility.IconContent("TestPassed").image as Texture2D);
        public static readonly Texture2D s_UnknownImg = (EditorGUIUtility.IconContent("TestNormal").image as Texture2D);
    }
}

