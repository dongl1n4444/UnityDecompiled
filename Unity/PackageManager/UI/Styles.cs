namespace Unity.PackageManager.UI
{
    using System;
    using UnityEditor;
    using UnityEngine;

    internal class Styles
    {
        public static GUIContent activateContent = new GUIContent("Activate");
        public static GUIContent cancelButtonContent = new GUIContent("Cancel");
        public static GUIStyle debugBox = new GUIStyle("box");
        public static GUIContent downloadButtonContent = new GUIContent("Download");
        public static GUIStyle infoContainer = new GUIStyle();
        public static GUIStyle infoHeader = new GUIStyle("LargeLabel");
        public static GUIStyle infoReleaseNotesBackground = new GUIStyle("OL Box");
        public static GUIStyle infoText = new GUIStyle("Label");
        public static GUIStyle infoToggle = new GUIStyle("Toggle");
        public static GUIStyle listBackground = new GUIStyle("OL Box");
        public static GUIStyle listElement = new GUIStyle("PR Label");
        public static GUIStyle listHeader = new GUIStyle("Label");
        public static GUIStyle listHelpBox = new GUIStyle("HelpBox");
        public static GUIContent loadedContent = new GUIContent("Loaded");
        public static GUIContent okButtonContent = new GUIContent("OK");
        public static GUIContent restartUnityContent = new GUIContent("Unity Restart Required");

        static Styles()
        {
            debugBox.overflow = new RectOffset(0, 0, 0, 0);
            debugBox.normal.textColor = Color.white;
            bool flag = true;
            debugBox.stretchHeight = flag;
            debugBox.stretchWidth = flag;
            listBackground.overflow = new RectOffset(1, 1, 1, 0);
            listElement.alignment = TextAnchor.MiddleLeft;
            listElement.fixedHeight = 16f;
            listElement.stretchWidth = true;
            listElement.margin = new RectOffset(0, 0, 0, 0);
            listElement.padding = new RectOffset(0x12, 0, 0, 0);
            listElement.border = new RectOffset(0, 0, 0, 0);
            listElement.overflow = new RectOffset(0, 0, 3, -3);
            listElement.contentOffset = new Vector2(0f, -4f);
            listHeader.alignment = TextAnchor.MiddleLeft;
            listHeader.font = EditorStyles.boldLabel.font;
            listHeader.fontStyle = EditorStyles.boldLabel.fontStyle;
            listHeader.stretchWidth = true;
            infoContainer.padding = new RectOffset(10, 10, 5, 10);
            infoHeader.fontStyle = FontStyle.Bold;
            infoHeader.fontSize = 0x12;
            infoHeader.margin = new RectOffset(0, 0, 0, 5);
            if (!EditorGUIUtility.isProSkin)
            {
                infoHeader.normal.textColor = new Color(0.4f, 0.4f, 0.4f, 1f);
            }
            else
            {
                infoHeader.normal.textColor = new Color(0.7f, 0.7f, 0.7f, 1f);
            }
            infoText.wordWrap = true;
            infoText.margin = new RectOffset(0, 0, 0, 0);
            infoText.richText = true;
            infoReleaseNotesBackground.overflow = new RectOffset(1, 1, 1, 1);
            infoToggle.overflow = new RectOffset(-2, 0, 0, 0);
            infoToggle.margin = new RectOffset(0, 5, 8, 0);
            infoToggle.fixedWidth = 15f;
        }
    }
}

