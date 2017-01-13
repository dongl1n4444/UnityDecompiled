namespace UnityEditor
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    [CustomEditor(typeof(TrueTypeFontImporter)), CanEditMultipleObjects]
    internal class TrueTypeFontImporterInspector : AssetImporterInspector
    {
        [CompilerGenerated]
        private static Func<char, bool> <>f__am$cache0;
        private static GUIContent[] kAscentCalculationModeStrings = new GUIContent[] { new GUIContent("Legacy version 2 mode (glyph bounding boxes)"), new GUIContent("Face ascender metric"), new GUIContent("Face bounding box metric") };
        private static int[] kAscentCalculationModeValues;
        private static GUIContent[] kCharacterStrings = new GUIContent[] { new GUIContent("Dynamic"), new GUIContent("Unicode"), new GUIContent("ASCII default set"), new GUIContent("ASCII upper case"), new GUIContent("ASCII lower case"), new GUIContent("Custom set") };
        private static int[] kCharacterValues = new int[] { -2, -1, 0, 1, 2, 3 };
        private static GUIContent[] kRenderingModeStrings = new GUIContent[] { new GUIContent("Smooth"), new GUIContent("Hinted Smooth"), new GUIContent("Hinted Raster"), new GUIContent("OS Default") };
        private static int[] kRenderingModeValues = new int[] { 0, 1, 2, 3 };
        private SerializedProperty m_AscentCalculationMode;
        private SerializedProperty m_CustomCharacters;
        private string m_DefaultFontNamesString = "";
        private Font[] m_FallbackFontReferences = null;
        private SerializedProperty m_FontNamesArraySize;
        private string m_FontNamesString = "";
        private SerializedProperty m_FontRenderingMode;
        private SerializedProperty m_FontSize;
        private bool? m_FormatSupported = null;
        private bool m_GotFontNamesFromImporter = false;
        private SerializedProperty m_IncludeFontData;
        private bool m_ReferencesExpanded = false;
        private SerializedProperty m_TextureCase;

        static TrueTypeFontImporterInspector()
        {
            int[] numArray1 = new int[3];
            numArray1[1] = 1;
            numArray1[2] = 2;
            kAscentCalculationModeValues = numArray1;
        }

        [MenuItem("CONTEXT/TrueTypeFontImporter/Create Editable Copy")]
        private static void CreateEditableCopy(MenuCommand command)
        {
            TrueTypeFontImporter context = (TrueTypeFontImporter) command.context;
            if (context.fontTextureCase == FontTextureCase.Dynamic)
            {
                EditorUtility.DisplayDialog("Cannot generate editable font asset for dynamic fonts", "Please reimport the font in a different mode.", "Ok");
            }
            else
            {
                string str = Path.Combine(Path.GetDirectoryName(context.assetPath), Path.GetFileNameWithoutExtension(context.assetPath));
                EditorGUIUtility.PingObject(context.GenerateEditableFont(GetUniquePath(str + "_copy", "fontsettings")));
            }
        }

        private string GetDefaultFontNames() => 
            ((TrueTypeFontImporter) base.target).fontTTFName;

        private string GetFontNames()
        {
            TrueTypeFontImporter target = (TrueTypeFontImporter) base.target;
            string str = string.Join(", ", target.fontNames);
            if (string.IsNullOrEmpty(str))
            {
                return this.m_DefaultFontNamesString;
            }
            this.m_GotFontNamesFromImporter = true;
            return str;
        }

        private static string GetUniquePath(string basePath, string extension)
        {
            for (int i = 0; i < 0x2710; i++)
            {
                string path = $"{basePath}{(i != 0) ? i.ToString() : string.Empty}.{extension}";
                if (!File.Exists(path))
                {
                    return path;
                }
            }
            return "";
        }

        private void OnEnable()
        {
            this.m_FontSize = base.serializedObject.FindProperty("m_FontSize");
            this.m_TextureCase = base.serializedObject.FindProperty("m_ForceTextureCase");
            this.m_IncludeFontData = base.serializedObject.FindProperty("m_IncludeFontData");
            this.m_FontNamesArraySize = base.serializedObject.FindProperty("m_FontNames.Array.size");
            this.m_CustomCharacters = base.serializedObject.FindProperty("m_CustomCharacters");
            this.m_FontRenderingMode = base.serializedObject.FindProperty("m_FontRenderingMode");
            this.m_AscentCalculationMode = base.serializedObject.FindProperty("m_AscentCalculationMode");
            if (base.targets.Length == 1)
            {
                this.m_DefaultFontNamesString = this.GetDefaultFontNames();
                this.m_FontNamesString = this.GetFontNames();
            }
        }

        public override void OnInspectorGUI()
        {
            if (!this.m_FormatSupported.HasValue)
            {
                this.m_FormatSupported = true;
                foreach (Object obj2 in base.targets)
                {
                    TrueTypeFontImporter importer = obj2 as TrueTypeFontImporter;
                    if ((importer == null) || !importer.IsFormatSupported())
                    {
                        this.m_FormatSupported = false;
                    }
                }
            }
            if (this.m_FormatSupported == false)
            {
                this.ShowFormatUnsupportedGUI();
            }
            else
            {
                EditorGUILayout.PropertyField(this.m_FontSize, new GUILayoutOption[0]);
                if (this.m_FontSize.intValue < 1)
                {
                    this.m_FontSize.intValue = 1;
                }
                if (this.m_FontSize.intValue > 500)
                {
                    this.m_FontSize.intValue = 500;
                }
                EditorGUILayout.IntPopup(this.m_FontRenderingMode, kRenderingModeStrings, kRenderingModeValues, new GUIContent("Rendering Mode"), new GUILayoutOption[0]);
                EditorGUILayout.IntPopup(this.m_TextureCase, kCharacterStrings, kCharacterValues, new GUIContent("Character"), new GUILayoutOption[0]);
                EditorGUILayout.IntPopup(this.m_AscentCalculationMode, kAscentCalculationModeStrings, kAscentCalculationModeValues, new GUIContent("Ascent Calculation Mode"), new GUILayoutOption[0]);
                if (!this.m_TextureCase.hasMultipleDifferentValues)
                {
                    if (this.m_TextureCase.intValue != -2)
                    {
                        if (this.m_TextureCase.intValue == 3)
                        {
                            EditorGUI.BeginChangeCheck();
                            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                            EditorGUILayout.PrefixLabel("Custom Chars");
                            EditorGUI.showMixedValue = this.m_CustomCharacters.hasMultipleDifferentValues;
                            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MinHeight(32f) };
                            string source = EditorGUILayout.TextArea(this.m_CustomCharacters.stringValue, GUI.skin.textArea, options);
                            EditorGUI.showMixedValue = false;
                            GUILayout.EndHorizontal();
                            if (EditorGUI.EndChangeCheck())
                            {
                                if (<>f__am$cache0 == null)
                                {
                                    <>f__am$cache0 = c => (c != '\n') && (c != '\r');
                                }
                                this.m_CustomCharacters.stringValue = new string(Enumerable.Where<char>(source.Distinct<char>(), <>f__am$cache0).ToArray<char>());
                            }
                        }
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(this.m_IncludeFontData, new GUIContent("Incl. Font Data"), new GUILayoutOption[0]);
                        if (base.targets.Length == 1)
                        {
                            EditorGUI.BeginChangeCheck();
                            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                            EditorGUILayout.PrefixLabel("Font Names");
                            GUI.SetNextControlName("fontnames");
                            GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.MinHeight(32f) };
                            this.m_FontNamesString = EditorGUILayout.TextArea(this.m_FontNamesString, "TextArea", optionArray2);
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                            GUILayout.FlexibleSpace();
                            using (new EditorGUI.DisabledScope(this.m_FontNamesString == this.m_DefaultFontNamesString))
                            {
                                if (GUILayout.Button("Reset", "MiniButton", new GUILayoutOption[0]))
                                {
                                    GUI.changed = true;
                                    if (GUI.GetNameOfFocusedControl() == "fontnames")
                                    {
                                        GUIUtility.keyboardControl = 0;
                                    }
                                    this.m_FontNamesString = this.m_DefaultFontNamesString;
                                }
                            }
                            GUILayout.EndHorizontal();
                            if (EditorGUI.EndChangeCheck())
                            {
                                this.SetFontNames(this.m_FontNamesString);
                            }
                            this.m_ReferencesExpanded = EditorGUILayout.Foldout(this.m_ReferencesExpanded, "References to other fonts in project", true);
                            if (this.m_ReferencesExpanded)
                            {
                                EditorGUILayout.HelpBox("These are automatically generated by the inspector if any of the font names you supplied match fonts present in your project, which will then be used as fallbacks for this font.", MessageType.Info);
                                using (new EditorGUI.DisabledScope(true))
                                {
                                    if ((this.m_FallbackFontReferences != null) && (this.m_FallbackFontReferences.Length > 0))
                                    {
                                        for (int i = 0; i < this.m_FallbackFontReferences.Length; i++)
                                        {
                                            EditorGUILayout.ObjectField(this.m_FallbackFontReferences[i], typeof(Font), false, new GUILayoutOption[0]);
                                        }
                                    }
                                    else
                                    {
                                        GUILayout.Label("No references to other fonts in project.", new GUILayoutOption[0]);
                                    }
                                }
                            }
                        }
                    }
                }
                base.ApplyRevertGUI();
            }
        }

        private void SetFontNames(string fontNames)
        {
            string[] strArray;
            if (!this.m_GotFontNamesFromImporter)
            {
                strArray = new string[0];
            }
            else
            {
                char[] separator = new char[] { ',' };
                strArray = fontNames.Split(separator);
                for (int j = 0; j < strArray.Length; j++)
                {
                    strArray[j] = strArray[j].Trim();
                }
            }
            this.m_FontNamesArraySize.intValue = strArray.Length;
            SerializedProperty property = this.m_FontNamesArraySize.Copy();
            for (int i = 0; i < strArray.Length; i++)
            {
                property.Next(false);
                property.stringValue = strArray[i];
            }
            this.m_FallbackFontReferences = ((TrueTypeFontImporter) base.target).LookupFallbackFontReferences(strArray);
        }

        private void ShowFormatUnsupportedGUI()
        {
            GUILayout.Space(5f);
            EditorGUILayout.HelpBox("Format of selected font is not supported by Unity.", MessageType.Warning);
        }
    }
}

