namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.Rendering;

    [CustomEditor(typeof(CustomRenderTexture)), CanEditMultipleObjects]
    internal class CustomRenderTextureEditor : RenderTextureEditor
    {
        private const float kCubefaceToggleWidth = 70f;
        private const float kIndentSize = 15f;
        private const float kRListAddButtonOffset = 16f;
        private const float kToggleWidth = 100f;
        private SerializedProperty m_CubeFaceMask;
        private SerializedProperty m_DoubleBuffered;
        private SerializedProperty m_InitColor;
        private SerializedProperty m_InitializationMode;
        private SerializedProperty m_InitMaterial;
        private SerializedProperty m_InitTexture;
        private SerializedProperty m_Material;
        private ReorderableList m_RectList;
        private int m_SelectedRectIndex = -1;
        private SerializedProperty m_ShaderPass;
        private SerializedProperty m_UpdateMode;
        private SerializedProperty m_UpdatePeriod;
        private SerializedProperty m_UpdateZones;
        private SerializedProperty m_UpdateZoneSpace;
        private SerializedProperty m_WrapUpdateZones;
        private static Styles s_Styles = null;

        private void BuildShaderPassPopup(Material material, List<GUIContent> names, List<int> values, bool addDefaultPass)
        {
            names.Clear();
            values.Clear();
            int passCount = material.passCount;
            for (int i = 0; i < passCount; i++)
            {
                string passName = material.GetPassName(i);
                if (passName.Length == 0)
                {
                    passName = $"Unnamed Pass {i}";
                }
                names.Add(EditorGUIUtility.TextContent(passName));
                values.Add(i);
            }
            if (addDefaultPass)
            {
                CustomRenderTexture target = base.target as CustomRenderTexture;
                GUIContent item = EditorGUIUtility.TextContent($"{names[target.shaderPass].text} (Default)");
                names.Insert(0, item);
                values.Insert(0, -1);
            }
        }

        private void DisplayCustomRenderTextureGUI()
        {
            CustomRenderTexture target = base.target as CustomRenderTexture;
            this.DisplayMaterialGUI();
            EditorGUILayout.Space();
            this.DisplayInitializationGUI();
            EditorGUILayout.Space();
            this.DisplayUpdateGUI();
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Space(EditorGUIUtility.labelWidth);
            if (GUILayout.Button(styles.saveButton, new GUILayoutOption[0]))
            {
                this.SaveToDisk();
            }
            GUILayout.EndHorizontal();
            if ((target.updateMode != CustomRenderTextureUpdateMode.Realtime) && (target.initializationMode == CustomRenderTextureUpdateMode.Realtime))
            {
                EditorGUILayout.HelpBox("Initialization Mode is set to Realtime but Update Mode is not. This will result in update never being visible.", MessageType.Warning);
            }
        }

        private void DisplayInitializationGUI()
        {
            EditorGUILayout.LabelField(styles.initGUI, EditorStyles.boldLabel, new GUILayoutOption[0]);
            EditorGUI.indentLevel++;
            EditorGUILayout.IntPopup(this.m_InitializationMode, styles.updateModeStrings, styles.updateModeValues, styles.initializationMode, new GUILayoutOption[0]);
            using (new EditorGUI.DisabledScope(this.m_InitMaterial.objectReferenceValue != null))
            {
                EditorGUILayout.PropertyField(this.m_InitColor, styles.initColor, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_InitTexture, styles.initTexture, new GUILayoutOption[0]);
            }
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(this.m_InitMaterial, styles.initMaterial, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck() && (this.m_InitMaterial.objectReferenceValue != null))
            {
                this.m_InitColor.colorValue = new Color(1f, 1f, 1f, 1f);
                this.m_InitTexture.objectReferenceValue = null;
            }
            EditorGUI.indentLevel--;
        }

        private void DisplayMaterialGUI()
        {
            EditorGUILayout.LabelField(styles.materialGUI, EditorStyles.boldLabel, new GUILayoutOption[0]);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(this.m_Material, true, new GUILayoutOption[0]);
            List<GUIContent> names = new List<GUIContent>();
            List<int> values = new List<int>();
            Material objectReferenceValue = this.m_Material.objectReferenceValue as Material;
            if (objectReferenceValue != null)
            {
                this.BuildShaderPassPopup(objectReferenceValue, names, values, false);
            }
            using (new EditorGUI.DisabledScope((names.Count == 0) || this.m_Material.hasMultipleDifferentValues))
            {
                EditorGUILayout.IntPopup(this.m_ShaderPass, names.ToArray(), values.ToArray(), styles.shaderPass, new GUILayoutOption[0]);
            }
            EditorGUI.indentLevel--;
        }

        private void DisplayRenderTextureGUI()
        {
            base.OnRenderTextureGUI(RenderTextureEditor.GUIElements.RenderTargetNoneGUI);
            GUILayout.Space(10f);
        }

        private void DisplayUpdateGUI()
        {
            EditorGUILayout.LabelField(styles.updateGUI, EditorStyles.boldLabel, new GUILayoutOption[0]);
            EditorGUI.indentLevel++;
            EditorGUILayout.IntPopup(this.m_UpdateMode, styles.updateModeStrings, styles.updateModeValues, styles.updateMode, new GUILayoutOption[0]);
            using (new EditorGUI.DisabledScope(this.m_UpdateMode.intValue != 1))
            {
                EditorGUILayout.PropertyField(this.m_UpdatePeriod, styles.updatePeriod, new GUILayoutOption[0]);
            }
            EditorGUILayout.PropertyField(this.m_WrapUpdateZones, styles.wrapUpdateZones, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_DoubleBuffered, styles.doubleBuffered, new GUILayoutOption[0]);
            bool flag = true;
            foreach (Object obj2 in base.targets)
            {
                CustomRenderTexture texture = obj2 as CustomRenderTexture;
                if ((texture != null) && (texture.dimension != TextureDimension.Cube))
                {
                    flag = false;
                }
            }
            if (flag)
            {
                int num2 = 0;
                int intValue = this.m_CubeFaceMask.intValue;
                Rect totalPosition = GUILayoutUtility.GetRect((float) 0f, (float) ((EditorGUIUtility.singleLineHeight * 3f) + (EditorGUIUtility.standardVerticalSpacing * 2f)));
                EditorGUI.BeginProperty(totalPosition, GUIContent.none, this.m_CubeFaceMask);
                Rect position = totalPosition;
                position.width = 100f;
                position.height = EditorGUIUtility.singleLineHeight;
                int index = 0;
                Rect rect3 = totalPosition;
                EditorGUI.LabelField(rect3, styles.cubemapFacesLabel);
                EditorGUI.BeginChangeCheck();
                for (int i = 0; i < 3; i++)
                {
                    position.x = (totalPosition.x + EditorGUIUtility.labelWidth) - 15f;
                    for (int j = 0; j < 2; j++)
                    {
                        if (EditorGUI.ToggleLeft(position, styles.cubemapFaces[index], (intValue & (((int) 1) << index)) != 0))
                        {
                            num2 |= ((int) 1) << index;
                        }
                        index++;
                        position.x += 100f;
                    }
                    position.y += EditorGUIUtility.singleLineHeight;
                }
                if (EditorGUI.EndChangeCheck())
                {
                    this.m_CubeFaceMask.intValue = num2;
                }
                EditorGUI.EndProperty();
            }
            EditorGUILayout.IntPopup(this.m_UpdateZoneSpace, styles.updateZoneSpaceStrings, styles.updateZoneSpaceValues, styles.updateZoneSpace, new GUILayoutOption[0]);
            if (!this.multipleEditing)
            {
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(true) };
                Rect rect = GUILayoutUtility.GetRect((float) 0f, (float) (this.m_RectList.GetHeight() + 16f), options);
                float num7 = 15f;
                rect.x += num7;
                rect.width -= num7;
                this.m_RectList.DoList(rect);
            }
            else
            {
                EditorGUILayout.HelpBox("Update Zones cannot be changed while editing multiple Custom Textures.", MessageType.Info);
            }
            EditorGUI.indentLevel--;
        }

        public override string GetInfoString() => 
            base.GetInfoString();

        private void OnAdd(ReorderableList l)
        {
            CustomRenderTexture target = base.target as CustomRenderTexture;
            int arraySize = l.serializedProperty.arraySize;
            SerializedProperty serializedProperty = l.serializedProperty;
            serializedProperty.arraySize++;
            l.index = arraySize;
            SerializedProperty arrayElementAtIndex = l.serializedProperty.GetArrayElementAtIndex(arraySize);
            Vector3 vector = new Vector3(0.5f, 0.5f, 0.5f);
            Vector3 vector2 = new Vector3(1f, 1f, 1f);
            if (target.updateZoneSpace == CustomRenderTextureUpdateZoneSpace.Pixel)
            {
                Vector3 scale = new Vector3((float) target.width, (float) target.height, (float) target.volumeDepth);
                vector.Scale(scale);
                vector2.Scale(scale);
            }
            arrayElementAtIndex.FindPropertyRelative("updateZoneCenter").vector3Value = vector;
            arrayElementAtIndex.FindPropertyRelative("updateZoneSize").vector3Value = vector2;
            arrayElementAtIndex.FindPropertyRelative("rotation").floatValue = 0f;
            arrayElementAtIndex.FindPropertyRelative("passIndex").intValue = -1;
            arrayElementAtIndex.FindPropertyRelative("needSwap").boolValue = false;
            this.m_SelectedRectIndex = arraySize;
        }

        private void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            CustomRenderTexture target = base.target as CustomRenderTexture;
            bool flag = target.dimension == TextureDimension.Tex3D;
            bool doubleBuffered = target.doubleBuffered;
            SerializedProperty arrayElementAtIndex = this.m_RectList.serializedProperty.GetArrayElementAtIndex(index);
            if (isActive)
            {
                float singleLineHeight = EditorGUIUtility.singleLineHeight;
                rect.y += EditorGUIUtility.standardVerticalSpacing;
                rect.height = singleLineHeight;
                EditorGUI.LabelField(rect, $"[{index}]");
                rect.y += singleLineHeight;
                SerializedProperty prop = arrayElementAtIndex.FindPropertyRelative("updateZoneCenter");
                this.UpdateZoneVec3PropertyField(rect, prop, styles.updateZoneCenter, !flag);
                rect.y += singleLineHeight;
                SerializedProperty property3 = arrayElementAtIndex.FindPropertyRelative("updateZoneSize");
                this.UpdateZoneVec3PropertyField(rect, property3, styles.updateZoneSize, !flag);
                if (!flag)
                {
                    rect.y += EditorGUIUtility.standardVerticalSpacing + singleLineHeight;
                    EditorGUI.PropertyField(rect, arrayElementAtIndex.FindPropertyRelative("rotation"), styles.updateZoneRotation);
                }
                List<GUIContent> names = new List<GUIContent>();
                List<int> values = new List<int>();
                Material objectReferenceValue = this.m_Material.objectReferenceValue as Material;
                if (objectReferenceValue != null)
                {
                    this.BuildShaderPassPopup(objectReferenceValue, names, values, true);
                }
                using (new EditorGUI.DisabledScope(names.Count == 0))
                {
                    SerializedProperty property = arrayElementAtIndex.FindPropertyRelative("passIndex");
                    rect.y += EditorGUIUtility.standardVerticalSpacing + singleLineHeight;
                    EditorGUI.IntPopup(rect, property, names.ToArray(), values.ToArray(), styles.shaderPass);
                }
                if (doubleBuffered)
                {
                    rect.y += EditorGUIUtility.standardVerticalSpacing + singleLineHeight;
                    EditorGUI.PropertyField(rect, arrayElementAtIndex.FindPropertyRelative("needSwap"), styles.updateZoneRotation);
                }
            }
            else
            {
                string str;
                Vector3 vector = arrayElementAtIndex.FindPropertyRelative("updateZoneCenter").vector3Value;
                Vector3 vector2 = arrayElementAtIndex.FindPropertyRelative("updateZoneSize").vector3Value;
                float floatValue = arrayElementAtIndex.FindPropertyRelative("rotation").floatValue;
                int intValue = arrayElementAtIndex.FindPropertyRelative("passIndex").intValue;
                if (flag)
                {
                    str = $"[{index}] Update Zone C({vector.x}, {vector.y}, {vector.z}) S({vector2.x}, {vector2.y}, {vector2.z}) P: {intValue}";
                }
                else
                {
                    str = $"[{index}] Update Zone C({vector.x}, {vector.y}) S({vector2.x}, {vector2.y}) R: {floatValue} P: {intValue}";
                }
                GUI.Label(rect, str);
            }
        }

        private void OnDrawHeader(Rect rect)
        {
            GUI.Label(rect, styles.updateZoneList);
        }

        private float OnElementHeight(int index)
        {
            CustomRenderTexture target = base.target as CustomRenderTexture;
            bool flag = target.dimension == TextureDimension.Tex3D;
            bool doubleBuffered = target.doubleBuffered;
            int num = 4;
            if (!flag)
            {
                num++;
            }
            if (doubleBuffered)
            {
                num++;
            }
            return ((index != this.m_SelectedRectIndex) ? EditorGUIUtility.singleLineHeight : ((EditorGUIUtility.singleLineHeight + 2f) * num));
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.m_Material = base.serializedObject.FindProperty("m_Material");
            this.m_ShaderPass = base.serializedObject.FindProperty("m_ShaderPass");
            this.m_InitializationMode = base.serializedObject.FindProperty("m_InitializationMode");
            this.m_InitColor = base.serializedObject.FindProperty("m_InitColor");
            this.m_InitTexture = base.serializedObject.FindProperty("m_InitTexture");
            this.m_InitMaterial = base.serializedObject.FindProperty("m_InitMaterial");
            this.m_UpdateMode = base.serializedObject.FindProperty("m_UpdateMode");
            this.m_UpdatePeriod = base.serializedObject.FindProperty("m_UpdatePeriod");
            this.m_UpdateZoneSpace = base.serializedObject.FindProperty("m_UpdateZoneSpace");
            this.m_UpdateZones = base.serializedObject.FindProperty("m_UpdateZones");
            this.m_WrapUpdateZones = base.serializedObject.FindProperty("m_WrapUpdateZones");
            this.m_DoubleBuffered = base.serializedObject.FindProperty("m_DoubleBuffered");
            this.m_CubeFaceMask = base.serializedObject.FindProperty("m_CubemapFaceMask");
            this.m_RectList = new ReorderableList(base.serializedObject, this.m_UpdateZones);
            this.m_RectList.drawElementCallback = new ReorderableList.ElementCallbackDelegate(this.OnDrawElement);
            this.m_RectList.drawHeaderCallback = new ReorderableList.HeaderCallbackDelegate(this.OnDrawHeader);
            this.m_RectList.onAddCallback = new ReorderableList.AddCallbackDelegate(this.OnAdd);
            this.m_RectList.onRemoveCallback = new ReorderableList.RemoveCallbackDelegate(this.OnRemove);
            this.m_RectList.onSelectCallback = new ReorderableList.SelectCallbackDelegate(this.OnSelect);
            this.m_RectList.elementHeightCallback = new ReorderableList.ElementHeightCallbackDelegate(this.OnElementHeight);
            this.m_RectList.footerHeight = 0f;
        }

        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            this.DisplayRenderTextureGUI();
            this.DisplayCustomRenderTextureGUI();
            base.serializedObject.ApplyModifiedProperties();
        }

        private void OnRemove(ReorderableList l)
        {
            SerializedProperty serializedProperty = l.serializedProperty;
            serializedProperty.arraySize--;
            if (l.index == l.serializedProperty.arraySize)
            {
                l.index--;
            }
            this.m_SelectedRectIndex = l.index;
        }

        private void OnSelect(ReorderableList list)
        {
            this.m_SelectedRectIndex = list.index;
        }

        private void SaveToDisk()
        {
            foreach (Object obj2 in base.targets)
            {
                CustomRenderTexture rt = obj2 as CustomRenderTexture;
                int width = rt.width;
                int height = rt.height;
                bool flag = base.IsHDRFormat(rt.format);
                bool flag2 = (rt.format == RenderTextureFormat.ARGBFloat) || (rt.format == RenderTextureFormat.RFloat);
                TextureFormat format = !flag ? TextureFormat.RGBA32 : TextureFormat.RGBAFloat;
                Texture2D textured = new Texture2D(width, height, format, false);
                Graphics.SetRenderTarget(rt);
                textured.ReadPixels(new Rect(0f, 0f, (float) width, (float) height), 0, 0);
                textured.Apply();
                byte[] bytes = null;
                if (flag)
                {
                    bytes = textured.EncodeToEXR(Texture2D.EXRFlags.CompressZIP | (!flag2 ? Texture2D.EXRFlags.None : Texture2D.EXRFlags.OutputAsFloat));
                }
                else
                {
                    bytes = textured.EncodeToPNG();
                }
                Object.DestroyImmediate(textured);
                string extension = !flag ? "png" : "exr";
                string directoryName = Path.GetDirectoryName(AssetDatabase.GetAssetPath(rt.GetInstanceID()));
                string str3 = EditorUtility.SaveFilePanel("Save Custom Texture", directoryName, rt.name, extension);
                if (!string.IsNullOrEmpty(str3))
                {
                    File.WriteAllBytes(str3, bytes);
                    AssetDatabase.Refresh();
                }
            }
        }

        private void UpdateZoneVec3PropertyField(Rect rect, SerializedProperty prop, GUIContent label, bool as2D)
        {
            EditorGUI.BeginProperty(rect, label, prop);
            if (!as2D)
            {
                prop.vector3Value = EditorGUI.Vector3Field(rect, label, prop.vector3Value);
            }
            else
            {
                Vector2 vector = EditorGUI.Vector2Field(rect, label, new Vector2(prop.vector3Value.x, prop.vector3Value.y));
                prop.vector3Value = new Vector3(vector.x, vector.y, prop.vector3Value.z);
            }
            EditorGUI.EndProperty();
        }

        private bool multipleEditing =>
            (base.targets.Length > 1);

        private static Styles styles
        {
            get
            {
                if (s_Styles == null)
                {
                    s_Styles = new Styles();
                }
                return s_Styles;
            }
        }

        private class Styles
        {
            public readonly GUIContent[] cubemapFaces;
            public readonly GUIContent cubemapFacesLabel = EditorGUIUtility.TextContent("Cubemap Faces|Enable or disable rendering on each face of the cubemap.");
            public readonly GUIContent doubleBuffered = EditorGUIUtility.TextContent("Double Buffered|If ticked, the custom Texture is double buffered so that you can access it during its own update. If unticked, the custom Texture will be not be double buffered.");
            public readonly GUIContent initColor = EditorGUIUtility.TextContent("Initialization Color|Color with which the custom texture is initialized.");
            public readonly GUIContent initGUI = EditorGUIUtility.TextContent("Initialization parameters");
            public readonly GUIContent initializationMode = EditorGUIUtility.TextContent("Initialization|Specify how the texture should be initialized.");
            public readonly GUIContent initMaterial = EditorGUIUtility.TextContent("Initialization Material|Material with which the custom texture is initialized (mutually exclusive with initialization texture and/or color).");
            public readonly GUIContent initTexture = EditorGUIUtility.TextContent("Initialization Texture|Texture with which the custom texture is initialized (multiplied by the initialization color).");
            public readonly GUIContent materialGUI = EditorGUIUtility.TextContent("Material parameters");
            public readonly GUIContent materials = EditorGUIUtility.TextContent("Materials");
            public readonly GUIContent needSwap = EditorGUIUtility.TextContent("Swap (Double Buffer)|If ticked, and if the texture is double buffered, a request is made to swap the buffers before the next update. If this is not ticked, the buffers will not be swapped.");
            public readonly GUIContent saveButton = EditorGUIUtility.TextContent("Save Texture|Save the content of the custom texture to an EXR or PNG file.");
            public readonly GUIStyle separator = "sv_iconselector_sep";
            public readonly GUIContent shaderPass = EditorGUIUtility.TextContent("Shader Pass|Shader Pass used to update the Custom Texture.");
            public readonly GUIContent updateGUI = EditorGUIUtility.TextContent("Update parameters");
            public readonly GUIContent updateMode = EditorGUIUtility.TextContent("Update|Specify how the texture should be updated.");
            public readonly GUIContent[] updateModeStrings = new GUIContent[] { EditorGUIUtility.TextContent("OnLoad"), EditorGUIUtility.TextContent("Realtime"), EditorGUIUtility.TextContent("OnDemand") };
            public readonly int[] updateModeValues;
            public readonly GUIContent updatePeriod = EditorGUIUtility.TextContent("Update Period|Period in seconds at which real-time textures are updated (0.0 will update every frame).");
            public readonly GUIContent updateZoneCenter = EditorGUIUtility.TextContent("Update Zone Center|Center of the partial update zone.");
            public readonly GUIContent updateZoneList = EditorGUIUtility.TextContent("Update Zone List|List of partial update zones.");
            public readonly GUIContent updateZoneRotation = EditorGUIUtility.TextContent("Update Zone Rotation|Rotation of the update zone.");
            public readonly GUIContent updateZoneSize = EditorGUIUtility.TextContent("Update Zone Size|Size of the partial update zone.");
            public readonly GUIContent updateZoneSpace = EditorGUIUtility.TextContent("Update Zone Space|Space in which the update zones are expressed (Normalized or Pixel space).");
            public readonly GUIContent[] updateZoneSpaceStrings;
            public readonly int[] updateZoneSpaceValues;
            public readonly GUIContent wrapUpdateZones = EditorGUIUtility.TextContent("Wrap Update Zones|If ticked, Update zones will wrap around the border of the Custom Texture. If unticked, Update zones will be clamped at the border of the Custom Texture.");

            public Styles()
            {
                int[] numArray1 = new int[3];
                numArray1[1] = 1;
                numArray1[2] = 2;
                this.updateModeValues = numArray1;
                this.updateZoneSpaceStrings = new GUIContent[] { EditorGUIUtility.TextContent("Normalized"), EditorGUIUtility.TextContent("Pixel") };
                int[] numArray2 = new int[2];
                numArray2[1] = 1;
                this.updateZoneSpaceValues = numArray2;
                this.cubemapFaces = new GUIContent[] { EditorGUIUtility.TextContent("+X"), EditorGUIUtility.TextContent("-X"), EditorGUIUtility.TextContent("+Y"), EditorGUIUtility.TextContent("-Y"), EditorGUIUtility.TextContent("+Z"), EditorGUIUtility.TextContent("-Z") };
            }
        }
    }
}

