namespace UnityEditor
{
    using System;
    using UnityEngine;

    [CanEditMultipleObjects, CustomEditor(typeof(IHVImageFormatImporter))]
    internal class IHVImageFormatImporterInspector : AssetImporterInspector
    {
        private SerializedProperty m_FilterMode;
        private SerializedProperty m_IsReadable;
        private SerializedProperty m_WrapMode;

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
            this.m_WrapMode = base.serializedObject.FindProperty("m_TextureSettings.m_WrapMode");
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
                    if (this.m_WrapMode.intValue != -1)
                    {
                        TextureUtil.SetWrapModeNoDirty(tex, (TextureWrapMode) this.m_WrapMode.intValue);
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
            Rect controlRect = EditorGUILayout.GetControlRect(new GUILayoutOption[0]);
            EditorGUI.BeginProperty(controlRect, Styles.wrapMode, this.m_WrapMode);
            EditorGUI.BeginChangeCheck();
            TextureWrapMode selected = (this.m_WrapMode.intValue != -1) ? ((TextureWrapMode) this.m_WrapMode.intValue) : TextureWrapMode.Repeat;
            selected = (TextureWrapMode) EditorGUI.EnumPopup(controlRect, Styles.wrapMode, selected);
            if (EditorGUI.EndChangeCheck())
            {
                this.m_WrapMode.intValue = (int) selected;
            }
            EditorGUI.EndProperty();
            controlRect = EditorGUILayout.GetControlRect(new GUILayoutOption[0]);
            EditorGUI.BeginProperty(controlRect, Styles.filterMode, this.m_FilterMode);
            EditorGUI.BeginChangeCheck();
            UnityEngine.FilterMode mode2 = (this.m_FilterMode.intValue != -1) ? ((UnityEngine.FilterMode) this.m_FilterMode.intValue) : UnityEngine.FilterMode.Bilinear;
            mode2 = (UnityEngine.FilterMode) EditorGUI.IntPopup(controlRect, Styles.filterMode, (int) mode2, Styles.filterModeOptions, Styles.filterModeValues);
            if (EditorGUI.EndChangeCheck())
            {
                this.m_FilterMode.intValue = (int) mode2;
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

