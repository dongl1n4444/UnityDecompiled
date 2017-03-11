namespace UnityEditor.Collaboration
{
    using System;
    using UnityEditor;
    using UnityEngine;

    internal class CollabCannotPublishDialog : EditorWindow
    {
        private static GUIContent AcceptText = EditorGUIUtility.TextContent("Accept");
        public string InfoMessage;
        private static GUIContent IssuesText = EditorGUIUtility.TextContent("Issues:");
        public Vector2 scrollPosition;
        private static GUIContent WarningText = EditorGUIUtility.TextContent(string.Format("Files that have been moved or in a changed folder cannot be selectively published, please use the Publish option in the collab window to publish all your changes.", new object[0]));

        public void OnGUI()
        {
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            GUI.skin.label.wordWrap = true;
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            GUILayout.Label(WarningText, new GUILayoutOption[0]);
            GUILayout.Label(IssuesText, new GUILayoutOption[0]);
            this.scrollPosition = EditorGUILayout.BeginScrollView(this.scrollPosition, new GUILayoutOption[0]);
            GUIStyle style = new GUIStyle {
                normal = { textColor = new Color(1f, 0.28f, 0f) }
            };
            GUILayout.Label(string.Format(this.InfoMessage, new object[0]), style, new GUILayoutOption[0]);
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(AcceptText, new GUILayoutOption[0]))
            {
                base.Close();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        public static CollabCannotPublishDialog ShowCollabWindow(string infoMessage)
        {
            CollabCannotPublishDialog dialog = ScriptableObject.CreateInstance<CollabCannotPublishDialog>();
            dialog.InfoMessage = infoMessage;
            Rect rect = new Rect(100f, 100f, 600f, 150f);
            dialog.minSize = new Vector2(rect.width, rect.height);
            dialog.maxSize = new Vector2(rect.width, rect.height);
            dialog.position = rect;
            dialog.ShowModal();
            dialog.m_Parent.window.m_DontSaveToLayout = true;
            return dialog;
        }
    }
}

