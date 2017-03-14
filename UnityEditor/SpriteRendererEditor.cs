namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEditor.AnimatedValues;
    using UnityEngine;
    using UnityEngine.Events;

    [CustomEditor(typeof(SpriteRenderer)), CanEditMultipleObjects]
    internal class SpriteRendererEditor : RendererEditorBase
    {
        private SerializedProperty m_AdaptiveModeThreshold;
        private SerializedProperty m_Color;
        private SerializedProperty m_DrawMode;
        private SerializedProperty m_FlipX;
        private SerializedProperty m_FlipY;
        private SerializedProperty m_Material;
        private AnimBool m_ShowAdaptiveThreshold;
        private AnimBool m_ShowDrawMode;
        private AnimBool m_ShowTileMode;
        private SerializedProperty m_Size;
        private SerializedProperty m_Sprite;
        private SerializedProperty m_SpriteTileMode;

        private void CheckForErrors()
        {
            bool flag;
            if (this.IsMaterialTextureAtlasConflict())
            {
                ShowError("Material has CanUseSpriteAtlas=False tag. Sprite texture has atlasHint set. Rendering artifacts possible.");
            }
            if (!this.DoesMaterialHaveSpriteTexture(out flag))
            {
                ShowError("Material does not have a _MainTex texture property. It is required for SpriteRenderer.");
            }
            else if (flag)
            {
                ShowError("Material texture property _MainTex has offset/scale set. It is incompatible with SpriteRenderer.");
            }
        }

        private bool DoesMaterialHaveSpriteTexture(out bool tiled)
        {
            tiled = false;
            Material sharedMaterial = (base.target as SpriteRenderer).sharedMaterial;
            if (sharedMaterial == null)
            {
                return true;
            }
            if (sharedMaterial.HasProperty("_MainTex"))
            {
                Vector2 textureOffset = sharedMaterial.GetTextureOffset("_MainTex");
                Vector2 textureScale = sharedMaterial.GetTextureScale("_MainTex");
                if (((textureOffset.x != 0f) || (textureOffset.y != 0f)) || ((textureScale.x != 1f) || (textureScale.y != 1f)))
                {
                    tiled = true;
                }
            }
            return sharedMaterial.HasProperty("_MainTex");
        }

        private void FlipToggle(Rect r, GUIContent label, SerializedProperty property)
        {
            EditorGUI.BeginProperty(r, label, property);
            bool boolValue = property.boolValue;
            EditorGUI.BeginChangeCheck();
            int indentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            boolValue = EditorGUI.ToggleLeft(r, label, boolValue);
            EditorGUI.indentLevel = indentLevel;
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects(base.targets, "Edit Constraints");
                property.boolValue = boolValue;
            }
            EditorGUI.EndProperty();
        }

        private void FlipToggles()
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            Rect position = GUILayoutUtility.GetRect(EditorGUIUtility.fieldWidth, EditorGUILayout.kLabelFloatMaxW, 16f, 16f, EditorStyles.numberField);
            int id = GUIUtility.GetControlID(Contents.flipToggleHash, FocusType.Keyboard, position);
            position = EditorGUI.PrefixLabel(position, id, Contents.flipLabel);
            position.width = 30f;
            this.FlipToggle(position, Contents.flipXLabel, this.m_FlipX);
            position.x += 30f;
            this.FlipToggle(position, Contents.flipYLabel, this.m_FlipY);
            GUILayout.EndHorizontal();
        }

        private void FloatFieldLabelAbove(GUIContent contentLabel, SerializedProperty sp)
        {
            EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
            Rect totalPosition = GUILayoutUtility.GetRect(contentLabel, EditorStyles.label);
            GUIContent label = EditorGUI.BeginProperty(totalPosition, contentLabel, sp);
            int id = GUIUtility.GetControlID(Contents.sizeFieldHash, FocusType.Keyboard, totalPosition);
            EditorGUI.HandlePrefixLabel(totalPosition, totalPosition, label, id);
            Rect rect = GUILayoutUtility.GetRect(contentLabel, EditorStyles.textField);
            EditorGUI.BeginChangeCheck();
            float num2 = EditorGUI.DoFloatField(EditorGUI.s_RecycledEditor, rect, totalPosition, id, sp.floatValue, EditorGUI.kFloatFieldFormatString, EditorStyles.textField, true);
            if (EditorGUI.EndChangeCheck())
            {
                sp.floatValue = num2;
            }
            EditorGUI.EndProperty();
            EditorGUILayout.EndVertical();
        }

        private string GetSpriteNotFullRectWarning()
        {
            foreach (UnityEngine.Object obj2 in base.targets)
            {
                if (!(obj2 as SpriteRenderer).shouldSupportTiling)
                {
                    return ((base.targets.Length != 1) ? Contents.notFullRectMultiEditWarningLabel.text : Contents.notFullRectWarningLabel.text);
                }
            }
            return null;
        }

        private bool IsMaterialTextureAtlasConflict()
        {
            Material sharedMaterial = (base.target as SpriteRenderer).sharedMaterial;
            if ((sharedMaterial != null) && (sharedMaterial.GetTag("CanUseSpriteAtlas", false).ToLower() == "false"))
            {
                Sprite objectReferenceValue = this.m_Sprite.objectReferenceValue as Sprite;
                TextureImporter atPath = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(objectReferenceValue)) as TextureImporter;
                if (((atPath != null) && (atPath.spritePackingTag != null)) && (atPath.spritePackingTag.Length > 0))
                {
                    return true;
                }
            }
            return false;
        }

        public override void OnEnable()
        {
            base.OnEnable();
            this.m_Sprite = base.serializedObject.FindProperty("m_Sprite");
            this.m_Color = base.serializedObject.FindProperty("m_Color");
            this.m_FlipX = base.serializedObject.FindProperty("m_FlipX");
            this.m_FlipY = base.serializedObject.FindProperty("m_FlipY");
            this.m_Material = base.serializedObject.FindProperty("m_Materials.Array");
            this.m_DrawMode = base.serializedObject.FindProperty("m_DrawMode");
            this.m_Size = base.serializedObject.FindProperty("m_Size");
            this.m_SpriteTileMode = base.serializedObject.FindProperty("m_SpriteTileMode");
            this.m_AdaptiveModeThreshold = base.serializedObject.FindProperty("m_AdaptiveModeThreshold");
            this.m_ShowDrawMode = new AnimBool(this.ShouldShowDrawMode());
            this.m_ShowTileMode = new AnimBool(this.ShouldShowTileMode());
            this.m_ShowAdaptiveThreshold = new AnimBool(this.ShouldShowAdaptiveThreshold());
            this.m_ShowDrawMode.valueChanged.AddListener(new UnityAction(this.Repaint));
            this.m_ShowTileMode.valueChanged.AddListener(new UnityAction(this.Repaint));
            this.m_ShowAdaptiveThreshold.valueChanged.AddListener(new UnityAction(this.Repaint));
        }

        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            EditorGUILayout.PropertyField(this.m_Sprite, Contents.spriteLabel, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_Color, Contents.colorLabel, true, new GUILayoutOption[0]);
            this.FlipToggles();
            if (this.m_Material.arraySize == 0)
            {
                this.m_Material.InsertArrayElementAtIndex(0);
            }
            Rect position = GUILayoutUtility.GetRect(EditorGUILayout.kLabelFloatMinW, EditorGUILayout.kLabelFloatMaxW, (float) 16f, (float) 16f);
            EditorGUI.showMixedValue = this.m_Material.hasMultipleDifferentValues;
            UnityEngine.Object objectReferenceValue = this.m_Material.GetArrayElementAtIndex(0).objectReferenceValue;
            UnityEngine.Object obj3 = EditorGUI.ObjectField(position, Contents.materialLabel, objectReferenceValue, typeof(Material), false);
            if (obj3 != objectReferenceValue)
            {
                this.m_Material.GetArrayElementAtIndex(0).objectReferenceValue = obj3;
            }
            EditorGUI.showMixedValue = false;
            EditorGUILayout.PropertyField(this.m_DrawMode, Contents.drawModeLabel, new GUILayoutOption[0]);
            this.m_ShowDrawMode.target = this.ShouldShowDrawMode();
            if (EditorGUILayout.BeginFadeGroup(this.m_ShowDrawMode.faded))
            {
                string spriteNotFullRectWarning = this.GetSpriteNotFullRectWarning();
                if (spriteNotFullRectWarning != null)
                {
                    EditorGUILayout.HelpBox(spriteNotFullRectWarning, MessageType.Warning);
                }
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
                EditorGUILayout.PrefixLabel(Contents.sizeLabel);
                EditorGUI.showMixedValue = this.m_Size.hasMultipleDifferentValues;
                this.FloatFieldLabelAbove(Contents.widthLabel, this.m_Size.FindPropertyRelative("x"));
                this.FloatFieldLabelAbove(Contents.heightLabel, this.m_Size.FindPropertyRelative("y"));
                EditorGUI.showMixedValue = false;
                EditorGUILayout.EndHorizontal();
                this.m_ShowTileMode.target = this.ShouldShowTileMode();
                if (EditorGUILayout.BeginFadeGroup(this.m_ShowTileMode.faded))
                {
                    EditorGUILayout.PropertyField(this.m_SpriteTileMode, Contents.fullTileLabel, new GUILayoutOption[0]);
                    this.m_ShowAdaptiveThreshold.target = this.ShouldShowAdaptiveThreshold();
                    if (EditorGUILayout.BeginFadeGroup(this.m_ShowAdaptiveThreshold.faded))
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.Slider(this.m_AdaptiveModeThreshold, 0f, 1f, Contents.fullTileThresholdLabel, new GUILayoutOption[0]);
                        EditorGUI.indentLevel--;
                    }
                    EditorGUILayout.EndFadeGroup();
                }
                EditorGUILayout.EndFadeGroup();
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFadeGroup();
            base.RenderSortingLayerFields();
            this.CheckForErrors();
            base.serializedObject.ApplyModifiedProperties();
        }

        private bool ShouldShowAdaptiveThreshold() => 
            ((this.m_SpriteTileMode.intValue == 1) && !this.m_SpriteTileMode.hasMultipleDifferentValues);

        private bool ShouldShowDrawMode() => 
            ((this.m_DrawMode.intValue != 0) && !this.m_DrawMode.hasMultipleDifferentValues);

        private bool ShouldShowTileMode() => 
            ((this.m_DrawMode.intValue == 2) && !this.m_DrawMode.hasMultipleDifferentValues);

        private static void ShowError(string error)
        {
            GUIContent content = new GUIContent(error) {
                image = Contents.warningIcon
            };
            GUILayout.Space(5f);
            GUILayout.BeginVertical(EditorStyles.helpBox, new GUILayoutOption[0]);
            GUILayout.Label(content, EditorStyles.wordWrappedMiniLabel, new GUILayoutOption[0]);
            GUILayout.EndVertical();
        }

        private static class Contents
        {
            public static readonly GUIContent colorLabel = EditorGUIUtility.TextContent("Color|Rendering color for the Sprite graphic");
            public static readonly GUIContent drawModeLabel = EditorGUIUtility.TextContent("Draw Mode|Specify the draw mode for the sprite");
            public static readonly GUIContent flipLabel = EditorGUIUtility.TextContent("Flip|Sprite flipping");
            public static readonly int flipToggleHash = "FlipToggleHash".GetHashCode();
            public static readonly GUIContent flipXLabel = EditorGUIUtility.TextContent("X|Sprite horizontal flipping");
            public static readonly GUIContent flipYLabel = EditorGUIUtility.TextContent("Y|Sprite vertical flipping");
            public static readonly GUIContent fullTileLabel = EditorGUIUtility.TextContent("Tile Mode|Specify the 9 slice tiling behaviour");
            public static readonly GUIContent fullTileThresholdLabel = EditorGUIUtility.TextContent("Stretch Value|This value defines how much the center portion will stretch before it tiles.");
            public static readonly GUIContent heightLabel = EditorGUIUtility.TextContent("Height|The height dimension value for the sprite");
            public static readonly GUIContent materialLabel = EditorGUIUtility.TextContent("Material|Material to be used by SpriteRenderer");
            public static readonly GUIContent notFullRectMultiEditWarningLabel = EditorGUIUtility.TextContent("Sprite Tiling might not appear correctly because some of the Sprites used are not generated with Full Rect. To fix this, change the Mesh Type in the Sprite's import setting to Full Rect");
            public static readonly GUIContent notFullRectWarningLabel = EditorGUIUtility.TextContent("Sprite Tiling might not appear correctly because the Sprite used is not generated with Full Rect. To fix this, change the Mesh Type in the Sprite's import setting to Full Rect");
            public static readonly int sizeFieldHash = "SpriteRendererSizeField".GetHashCode();
            public static readonly GUIContent sizeLabel = EditorGUIUtility.TextContent("Size|The rendering dimension for the sprite");
            public static readonly GUIContent spriteLabel = EditorGUIUtility.TextContent("Sprite|The Sprite to render");
            public static readonly Texture2D warningIcon = EditorGUIUtility.LoadIcon("console.warnicon");
            public static readonly GUIContent widthLabel = EditorGUIUtility.TextContent("Width|The width dimension value for the sprite");
        }
    }
}

