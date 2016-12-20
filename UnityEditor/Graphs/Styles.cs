namespace UnityEditor.Graphs
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    public class Styles
    {
        public static GUIContent connectionTexture = FindContent("flow connection texture.png");
        public static GUIStyle graphBackground = "flow background";
        private static readonly Dictionary<string, GUIStyle> m_NodeStyleCache = new Dictionary<string, GUIStyle>();
        public static GUIStyle nodeAddButton = "Label";
        public static GUIStyle nodeGroupButton;
        public static GUIStyle nodeTitlebar = "flow node titlebar";
        public static GUIContent selectedConnectionTexture = FindContent("flow selected connection texture.png");
        public static GUIStyle selectionRect = "SelectionRect";
        public static GUIStyle targetPinIn = "flow target in";
        public static GUIStyle triggerPinIn = "flow triggerPin in";
        public static GUIStyle triggerPinOut = "flow triggerPin out";
        public static GUIStyle varPinIn = "flow varPin in";
        public static GUIStyle varPinOut = "flow varPin out";
        public static GUIStyle varPinTooltip = "flow varPin tooltip";

        static Styles()
        {
            GUIStyle style = new GUIStyle(EditorStyles.toolbarButton) {
                alignment = TextAnchor.MiddleLeft
            };
            nodeGroupButton = style;
        }

        private static GUIContent FindContent(string contentName)
        {
            Texture image = null;
            if (EditorGUIUtility.isProSkin)
            {
                image = EditorGUIUtility.Load("Graph/Dark/" + contentName) as Texture;
            }
            if (image == null)
            {
                image = EditorGUIUtility.Load("Graph/Light/" + contentName) as Texture;
            }
            if (image == null)
            {
                Debug.LogError("Unable to load " + contentName);
                return new GUIContent(contentName);
            }
            return new GUIContent(image);
        }

        public static GUIStyle GetNodeStyle(string styleName, Color color, bool on)
        {
            string key = string.Format("flow {0} {1}{2}", styleName, (int) color, !on ? "" : " on");
            if (!m_NodeStyleCache.ContainsKey(key))
            {
                m_NodeStyleCache[key] = key;
            }
            return m_NodeStyleCache[key];
        }

        public enum Color
        {
            Aqua = 2,
            Blue = 1,
            Gray = 0,
            Green = 3,
            Grey = 0,
            Orange = 5,
            Red = 6,
            Yellow = 4
        }
    }
}

