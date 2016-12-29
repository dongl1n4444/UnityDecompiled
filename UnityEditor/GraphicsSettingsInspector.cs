namespace UnityEditor
{
    using System;
    using UnityEditor.AnimatedValues;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.Rendering;

    [CustomEditor(typeof(GraphicsSettings))]
    internal class GraphicsSettingsInspector : Editor
    {
        private Editor m_AlwaysIncludedShadersEditor;
        private Editor m_BuiltinShadersEditor;
        private Editor m_ShaderPreloadEditor;
        private Editor m_ShaderStrippingEditor;
        private Editor m_TierSettingsEditor;
        private bool showTierSettingsUI = true;
        private AnimBool tierSettingsAnimator = null;

        public void OnEnable()
        {
        }

        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            this.TierSettingsGUI();
            GUILayout.Label(Styles.builtinSettings, EditorStyles.boldLabel, new GUILayoutOption[0]);
            this.builtinShadersEditor.OnInspectorGUI();
            this.alwaysIncludedShadersEditor.OnInspectorGUI();
            EditorGUILayout.Space();
            GUILayout.Label(Styles.shaderStrippingSettings, EditorStyles.boldLabel, new GUILayoutOption[0]);
            this.shaderStrippingEditor.OnInspectorGUI();
            EditorGUILayout.Space();
            GUILayout.Label(Styles.shaderPreloadSettings, EditorStyles.boldLabel, new GUILayoutOption[0]);
            this.shaderPreloadEditor.OnInspectorGUI();
            base.serializedObject.ApplyModifiedProperties();
        }

        private void TierSettingsGUI()
        {
            if (this.tierSettingsAnimator == null)
            {
                this.tierSettingsAnimator = new AnimBool(this.showTierSettingsUI, new UnityAction(this.Repaint));
            }
            bool enabled = GUI.enabled;
            GUI.enabled = true;
            EditorGUILayout.BeginVertical(GUI.skin.box, new GUILayoutOption[0]);
            Rect position = GUILayoutUtility.GetRect((float) 20f, (float) 18f);
            position.x += 3f;
            position.width += 6f;
            this.showTierSettingsUI = GUI.Toggle(position, this.showTierSettingsUI, Styles.tierSettings, EditorStyles.inspectorTitlebarText);
            this.tierSettingsAnimator.target = this.showTierSettingsUI;
            GUI.enabled = enabled;
            if (EditorGUILayout.BeginFadeGroup(this.tierSettingsAnimator.faded))
            {
                this.tierSettingsEditor.OnInspectorGUI();
            }
            EditorGUILayout.EndFadeGroup();
            EditorGUILayout.EndVertical();
        }

        private Editor alwaysIncludedShadersEditor
        {
            get
            {
                Editor.CreateCachedEditor(this.graphicsSettings, typeof(GraphicsSettingsWindow.AlwaysIncludedShadersEditor), ref this.m_AlwaysIncludedShadersEditor);
                return this.m_AlwaysIncludedShadersEditor;
            }
        }

        private Editor builtinShadersEditor
        {
            get
            {
                Editor.CreateCachedEditor(this.graphicsSettings, typeof(GraphicsSettingsWindow.BuiltinShadersEditor), ref this.m_BuiltinShadersEditor);
                return this.m_BuiltinShadersEditor;
            }
        }

        private Object graphicsSettings =>
            GraphicsSettings.GetGraphicsSettings();

        private Editor shaderPreloadEditor
        {
            get
            {
                Editor.CreateCachedEditor(this.graphicsSettings, typeof(GraphicsSettingsWindow.ShaderPreloadEditor), ref this.m_ShaderPreloadEditor);
                return this.m_ShaderPreloadEditor;
            }
        }

        private Editor shaderStrippingEditor
        {
            get
            {
                Editor.CreateCachedEditor(this.graphicsSettings, typeof(GraphicsSettingsWindow.ShaderStrippingEditor), ref this.m_ShaderStrippingEditor);
                return this.m_ShaderStrippingEditor;
            }
        }

        private Editor tierSettingsEditor
        {
            get
            {
                Editor.CreateCachedEditor(this.graphicsSettings, typeof(GraphicsSettingsWindow.TierSettingsEditor), ref this.m_TierSettingsEditor);
                ((GraphicsSettingsWindow.TierSettingsEditor) this.m_TierSettingsEditor).verticalLayout = false;
                return this.m_TierSettingsEditor;
            }
        }

        internal class Styles
        {
            public static readonly GUIContent builtinSettings = EditorGUIUtility.TextContent("Built-in shader settings");
            public static readonly GUIContent shaderPreloadSettings = EditorGUIUtility.TextContent("Shader preloading");
            public static readonly GUIContent shaderStrippingSettings = EditorGUIUtility.TextContent("Shader stripping");
            public static readonly GUIContent tierSettings = EditorGUIUtility.TextContent("Tier settings");
        }
    }
}

