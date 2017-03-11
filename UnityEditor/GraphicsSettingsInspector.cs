namespace UnityEditor
{
    using System;
    using UnityEditor.AnimatedValues;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.Experimental.Rendering;
    using UnityEngine.Rendering;

    [CustomEditor(typeof(GraphicsSettings))]
    internal class GraphicsSettingsInspector : Editor
    {
        private Editor m_AlwaysIncludedShadersEditor;
        private Editor m_BuiltinShadersEditor;
        private SerializedProperty m_ScriptableRenderLoop;
        private Editor m_ShaderPreloadEditor;
        private Editor m_ShaderStrippingEditor;
        private Editor m_TierSettingsEditor;
        private SerializedProperty m_TransparencySortAxis;
        private SerializedProperty m_TransparencySortMode;
        private bool showTierSettingsUI = true;
        private AnimBool tierSettingsAnimator = null;

        private void HandleEditorWindowButton()
        {
            TierSettingsWindow instance = TierSettingsWindow.GetInstance();
            GUIContent content = (instance != null) ? Styles.closeEditorWindow : Styles.showEditorWindow;
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(110f) };
            if (GUILayout.Button(content, EditorStyles.miniButton, options))
            {
                if (instance != null)
                {
                    instance.Close();
                }
                else
                {
                    TierSettingsWindow.CreateWindow();
                    TierSettingsWindow.GetInstance().Show();
                }
            }
        }

        public void OnEnable()
        {
            this.m_TransparencySortMode = base.serializedObject.FindProperty("m_TransparencySortMode");
            this.m_TransparencySortAxis = base.serializedObject.FindProperty("m_TransparencySortAxis");
            this.m_ScriptableRenderLoop = base.serializedObject.FindProperty("m_CustomRenderPipeline");
        }

        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            GUILayout.Label(Styles.renderLoopSettings, EditorStyles.boldLabel, new GUILayoutOption[0]);
            Rect totalPosition = EditorGUILayout.GetControlRect(true, EditorGUI.GetPropertyHeight(this.m_ScriptableRenderLoop), new GUILayoutOption[0]);
            EditorGUI.BeginProperty(totalPosition, Styles.renderLoopLabel, this.m_ScriptableRenderLoop);
            this.m_ScriptableRenderLoop.objectReferenceValue = EditorGUI.ObjectField(totalPosition, this.m_ScriptableRenderLoop.objectReferenceValue, typeof(RenderPipelineAsset), false);
            EditorGUI.EndProperty();
            EditorGUILayout.Space();
            GUILayout.Label(Styles.cameraSettings, EditorStyles.boldLabel, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_TransparencySortMode, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_TransparencySortAxis, new GUILayoutOption[0]);
            EditorGUILayout.Space();
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
            EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
            Rect position = GUILayoutUtility.GetRect((float) 20f, (float) 18f);
            position.x += 3f;
            position.width += 6f;
            this.showTierSettingsUI = GUI.Toggle(position, this.showTierSettingsUI, Styles.tierSettings, EditorStyles.inspectorTitlebarText);
            this.HandleEditorWindowButton();
            EditorGUILayout.EndHorizontal();
            this.tierSettingsAnimator.target = this.showTierSettingsUI;
            GUI.enabled = enabled;
            if (EditorGUILayout.BeginFadeGroup(this.tierSettingsAnimator.faded) && (TierSettingsWindow.GetInstance() == null))
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

        private UnityEngine.Object graphicsSettings =>
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
                ((GraphicsSettingsWindow.TierSettingsEditor) this.m_TierSettingsEditor).verticalLayout = true;
                return this.m_TierSettingsEditor;
            }
        }

        internal class Styles
        {
            public static readonly GUIContent builtinSettings = EditorGUIUtility.TextContent("Built-in shader settings");
            public static readonly GUIContent cameraSettings = EditorGUIUtility.TextContent("Camera settings");
            public static readonly GUIContent closeEditorWindow = new GUIContent("Close Editor");
            public static readonly GUIContent renderLoopLabel = EditorGUIUtility.TextContent("Scriptable Render Loop");
            public static readonly GUIContent renderLoopSettings = EditorGUIUtility.TextContent("Scriptable RenderLoop settings");
            public static readonly GUIContent shaderPreloadSettings = EditorGUIUtility.TextContent("Shader preloading");
            public static readonly GUIContent shaderStrippingSettings = EditorGUIUtility.TextContent("Shader stripping");
            public static readonly GUIContent showEditorWindow = new GUIContent("Open Editor...");
            public static readonly GUIContent tierSettings = EditorGUIUtility.TextContent("Tier settings");
        }
    }
}

