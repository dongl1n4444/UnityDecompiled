namespace UnityEditor.Collaboration
{
    using System;
    using UnityEditor;
    using UnityEngine;

    internal class CollabPublishDialog : EditorWindow
    {
        private static GUIContent CancelText = EditorGUIUtility.TextContent("Cancel");
        private static GUIContent ChangeAssetsText = EditorGUIUtility.TextContent("Changed assets:");
        public string Changelist;
        private static GUIContent DescribeChangesText = EditorGUIUtility.TextContent("Describe your changes here");
        public PublishDialogOptions Options;
        private static GUIContent PublishText = EditorGUIUtility.TextContent("Publish");
        public Vector2 scrollView;

        public CollabPublishDialog()
        {
            this.Options.Comments = "";
        }

        public void OnGUI()
        {
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            GUILayout.Label(DescribeChangesText, new GUILayoutOption[0]);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MinHeight(80f) };
            this.Options.Comments = GUILayout.TextArea(this.Options.Comments, 0x3e8, options);
            GUILayout.Label(ChangeAssetsText, new GUILayoutOption[0]);
            this.scrollView = EditorGUILayout.BeginScrollView(this.scrollView, false, false, new GUILayoutOption[0]);
            Vector2 vector = new GUIStyle().CalcSize(new GUIContent(this.Changelist));
            GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.ExpandHeight(true), GUILayout.MinHeight(vector.y) };
            EditorGUILayout.SelectableLabel(this.Changelist, EditorStyles.textField, optionArray2);
            EditorGUILayout.EndScrollView();
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(CancelText, new GUILayoutOption[0]))
            {
                this.Options.DoPublish = false;
                base.Close();
            }
            if (GUILayout.Button(PublishText, new GUILayoutOption[0]))
            {
                this.Options.DoPublish = true;
                base.Close();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        public static CollabPublishDialog ShowCollabWindow(string changelist)
        {
            CollabPublishDialog dialog = ScriptableObject.CreateInstance<CollabPublishDialog>();
            dialog.Changelist = changelist;
            Rect rect = new Rect(100f, 100f, 600f, 225f);
            dialog.minSize = new Vector2(rect.width, rect.height);
            dialog.maxSize = new Vector2(rect.width, rect.height);
            dialog.position = rect;
            dialog.ShowModal();
            dialog.m_Parent.window.m_DontSaveToLayout = true;
            return dialog;
        }
    }
}

