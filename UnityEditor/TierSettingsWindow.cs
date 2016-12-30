namespace UnityEditor
{
    using System;
    using UnityEngine;
    using UnityEngine.Rendering;

    internal class TierSettingsWindow : EditorWindow
    {
        private Editor m_TierSettingsEditor;
        private static TierSettingsWindow s_Instance;

        public static void CreateWindow()
        {
            s_Instance = EditorWindow.GetWindow<TierSettingsWindow>();
            s_Instance.minSize = new Vector2(600f, 300f);
            s_Instance.titleContent = EditorGUIUtility.TextContent("Tier Settings");
        }

        internal static TierSettingsWindow GetInstance() => 
            s_Instance;

        private void OnDisable()
        {
            Object.DestroyImmediate(this.m_TierSettingsEditor);
            this.m_TierSettingsEditor = null;
            if (s_Instance == this)
            {
                s_Instance = null;
            }
        }

        private void OnEnable()
        {
            s_Instance = this;
        }

        private void OnGUI()
        {
            this.tierSettingsEditor.OnInspectorGUI();
        }

        private Object graphicsSettings =>
            GraphicsSettings.GetGraphicsSettings();

        private Editor tierSettingsEditor
        {
            get
            {
                Editor.CreateCachedEditor(this.graphicsSettings, typeof(GraphicsSettingsWindow.TierSettingsEditor), ref this.m_TierSettingsEditor);
                ((GraphicsSettingsWindow.TierSettingsEditor) this.m_TierSettingsEditor).verticalLayout = false;
                return this.m_TierSettingsEditor;
            }
        }
    }
}

