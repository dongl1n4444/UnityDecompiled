﻿namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal abstract class AssetImporterTabbedEditor : AssetImporterInspector
    {
        private AssetImporterInspector m_ActiveEditor;
        private int m_ActiveEditorIndex = 0;
        protected string[] m_SubEditorNames = null;
        protected Type[] m_SubEditorTypes = null;

        protected AssetImporterTabbedEditor()
        {
        }

        public override bool HasPreviewGUI() => 
            this.activeEditor?.HasPreviewGUI();

        private void OnDestroy()
        {
            AssetImporterInspector activeEditor = this.activeEditor;
            if (activeEditor != null)
            {
                this.m_ActiveEditor = null;
                Object.DestroyImmediate(activeEditor);
            }
        }

        internal virtual void OnEnable()
        {
            this.m_ActiveEditorIndex = EditorPrefs.GetInt(base.GetType().Name + "ActiveEditorIndex", 0);
            if (this.m_ActiveEditor == null)
            {
                this.m_ActiveEditor = Editor.CreateEditor(base.targets, this.m_SubEditorTypes[this.m_ActiveEditorIndex]) as AssetImporterInspector;
            }
        }

        public override void OnInspectorGUI()
        {
            using (new EditorGUI.DisabledScope(false))
            {
                GUI.enabled = true;
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayout.FlexibleSpace();
                EditorGUI.BeginChangeCheck();
                this.m_ActiveEditorIndex = GUILayout.Toolbar(this.m_ActiveEditorIndex, this.m_SubEditorNames, new GUILayoutOption[0]);
                if (EditorGUI.EndChangeCheck())
                {
                    EditorPrefs.SetInt(base.GetType().Name + "ActiveEditorIndex", this.m_ActiveEditorIndex);
                    AssetImporterInspector activeEditor = this.activeEditor;
                    this.m_ActiveEditor = null;
                    Object.DestroyImmediate(activeEditor);
                    this.m_ActiveEditor = Editor.CreateEditor(base.targets, this.m_SubEditorTypes[this.m_ActiveEditorIndex]) as AssetImporterInspector;
                    this.m_ActiveEditor.assetEditor = this.assetEditor;
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            this.activeEditor.OnInspectorGUI();
        }

        public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
        {
            this.activeEditor.OnInteractivePreviewGUI(r, background);
        }

        public override void OnPreviewSettings()
        {
            this.activeEditor.OnPreviewSettings();
        }

        public AssetImporterInspector activeEditor =>
            this.m_ActiveEditor;

        internal override Editor assetEditor
        {
            get => 
                base.assetEditor;
            set
            {
                base.assetEditor = value;
                if (this.activeEditor != null)
                {
                    this.activeEditor.assetEditor = this.assetEditor;
                }
            }
        }
    }
}

