namespace UnityEditor
{
    using System;
    using UnityEngine;

    [CustomEditor(typeof(IHVImageFormatImporter)), CanEditMultipleObjects]
    internal class IHVImageFormatImporterInspector : AssetImporterInspector
    {
        private SerializedProperty m_FilterMode;
        private SerializedProperty m_IsReadable;
        private bool m_ShowPerAxisWrapModes = false;
        private SerializedProperty m_WrapU;
        private SerializedProperty m_WrapV;
        private SerializedProperty m_WrapW;

        public void IsReadableGUI()
        {
            Rect controlRect = EditorGUILayout.GetControlRect(new GUILayoutOption[0]);
            EditorGUI.BeginProperty(controlRect, Styles.readWrite, this.m_IsReadable);
            EditorGUI.BeginChangeCheck();
            bool flag = EditorGUI.Toggle(controlRect, Styles.readWrite, this.m_IsReadable.boolValue);
            if (EditorGUI.EndChangeCheck())
            {
                this.m_IsReadable.boolValue = flag;
            }
            EditorGUI.EndProperty();
        }

        public virtual void OnEnable()
        {
            this.m_IsReadable = base.serializedObject.FindProperty("m_IsReadable");
            this.m_FilterMode = base.serializedObject.FindProperty("m_TextureSettings.m_FilterMode");
            this.m_WrapU = base.serializedObject.FindProperty("m_TextureSettings.m_WrapU");
            this.m_WrapV = base.serializedObject.FindProperty("m_TextureSettings.m_WrapV");
            this.m_WrapW = base.serializedObject.FindProperty("m_TextureSettings.m_WrapW");
        }

        public override void OnInspectorGUI()
        {
            this.IsReadableGUI();
            EditorGUI.BeginChangeCheck();
            this.TextureSettingsGUI();
            if (EditorGUI.EndChangeCheck())
            {
                foreach (AssetImporter importer in base.targets)
                {
                    Texture tex = AssetDatabase.LoadMainAssetAtPath(importer.assetPath) as Texture;
                    if (this.m_FilterMode.intValue != -1)
                    {
                        TextureUtil.SetFilterModeNoDirty(tex, (UnityEngine.FilterMode) this.m_FilterMode.intValue);
                    }
                    if ((((this.m_WrapU.intValue != -1) || (this.m_WrapV.intValue != -1)) || (this.m_WrapW.intValue != -1)) && ((!this.m_WrapU.hasMultipleDifferentValues && !this.m_WrapV.hasMultipleDifferentValues) && !this.m_WrapW.hasMultipleDifferentValues))
                    {
                        TextureUtil.SetWrapModeNoDirty(tex, (TextureWrapMode) this.m_WrapU.intValue, (TextureWrapMode) this.m_WrapV.intValue, (TextureWrapMode) this.m_WrapW.intValue);
                    }
                }
                SceneView.RepaintAll();
            }
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            base.ApplyRevertGUI();
            GUILayout.EndHorizontal();
        }

        public void TextureSettingsGUI()
        {
            bool isVolumeTexture = false;
            TextureInspector.WrapModePopup(this.m_WrapU, this.m_WrapV, this.m_WrapW, isVolumeTexture, ref this.m_ShowPerAxisWrapModes);
            Rect controlRect = EditorGUILayout.GetControlRect(new GUILayoutOption[0]);
            EditorGUI.BeginProperty(controlRect, Styles.filterMode, this.m_FilterMode);
            EditorGUI.BeginChangeCheck();
            UnityEngine.FilterMode mode = (this.m_FilterMode.intValue != -1) ? ((UnityEngine.FilterMode) this.m_FilterMode.intValue) : UnityEngine.FilterMode.Bilinear;
            mode = (UnityEngine.FilterMode) EditorGUI.IntPopup(controlRect, Styles.filterMode, (int) mode, Styles.filterModeOptions, Styles.filterModeValues);
            if (EditorGUI.EndChangeCheck())
            {
                this.m_FilterMode.intValue = (int) mode;
            }
            EditorGUI.EndProperty();
        }

        internal override bool showImportedObject =>
            false;

        internal class Styles
        {
            public static readonly GUIContent filterMode = EditorGUIUtility.TextContent("Filter Mode");
            public static readonly GUIContent[] filterModeOptions;
            public static readonly int[] filterModeValues;
            public static readonly GUIContent readWrite = EditorGUIUtility.TextContent("Read/Write Enabled|Enable to be able to access the raw pixel data from code.");
            public static readonly GUIContent wrapMode = EditorGUIUtility.TextContent("Wrap Mode");

            static Styles()
            {
                int[] numArray1 = new int[3];
                numArray1[1] = 1;
                numArray1[2] = 2;
                filterModeValues = numArray1;
                filterModeOptions = new GUIContent[] { EditorGUIUtility.TextContent("Point (no filter)"), EditorGUIUtility.TextContent("Bilinear"), EditorGUIUtility.TextContent("Trilinear") };
            }
        }
    }
}

