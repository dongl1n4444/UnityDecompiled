namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor.AnimatedValues;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.Events;
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
        private SerializedProperty m_InitSource;
        private SerializedProperty m_InitTexture;
        private SerializedProperty m_Material;
        private ReorderableList m_RectList;
        private SerializedProperty m_ShaderPass;
        private readonly AnimBool m_ShowInitSourceAsMaterial = new AnimBool();
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
                GUIContent item = EditorGUIUtility.TextContent($"Default ({names[target.shaderPass].text})");
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
            if ((target.updateMode != CustomRenderTextureUpdateMode.Realtime) && (target.initializationMode == CustomRenderTextureUpdateMode.Realtime))
            {
                EditorGUILayout.HelpBox("Initialization Mode is set to Realtime but Update Mode is not. This will result in update never being visible.", MessageType.Warning);
            }
        }

        private void DisplayInitializationGUI()
        {
            this.m_ShowInitSourceAsMaterial.target = !this.m_InitSource.hasMultipleDifferentValues && (this.m_InitSource.intValue == 1);
            EditorGUILayout.IntPopup(this.m_InitializationMode, styles.updateModeStrings, styles.updateModeValues, styles.initializationMode, new GUILayoutOption[0]);
            EditorGUI.indentLevel++;
            EditorGUILayout.IntPopup(this.m_InitSource, styles.initSourceStrings, styles.initSourceValues, styles.initSource, new GUILayoutOption[0]);
            if (!this.m_InitSource.hasMultipleDifferentValues)
            {
                EditorGUI.indentLevel++;
                if (EditorGUILayout.BeginFadeGroup(this.m_ShowInitSourceAsMaterial.faded))
                {
                    EditorGUILayout.PropertyField(this.m_InitMaterial, styles.initMaterial, new GUILayoutOption[0]);
                }
                EditorGUILayout.EndFadeGroup();
                if (EditorGUILayout.BeginFadeGroup(1f - this.m_ShowInitSourceAsMaterial.faded))
                {
                    EditorGUILayout.PropertyField(this.m_InitColor, styles.initColor, new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_InitTexture, styles.initTexture, new GUILayoutOption[0]);
                }
                EditorGUILayout.EndFadeGroup();
                EditorGUI.indentLevel--;
            }
            EditorGUI.indentLevel--;
        }

        private void DisplayMaterialGUI()
        {
            EditorGUILayout.PropertyField(this.m_Material, true, new GUILayoutOption[0]);
            EditorGUI.indentLevel++;
            List<GUIContent> names = new List<GUIContent>();
            List<int> values = new List<int>();
            Material objectReferenceValue = this.m_Material.objectReferenceValue as Material;
            if (objectReferenceValue != null)
            {
                this.BuildShaderPassPopup(objectReferenceValue, names, values, false);
            }
            using (new EditorGUI.DisabledScope((names.Count == 0) || this.m_Material.hasMultipleDifferentValues))
            {
                if (objectReferenceValue != null)
                {
                    EditorGUILayout.IntPopup(this.m_ShaderPass, names.ToArray(), values.ToArray(), styles.shaderPass, new GUILayoutOption[0]);
                }
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
            EditorGUILayout.IntPopup(this.m_UpdateMode, styles.updateModeStrings, styles.updateModeValues, styles.updateMode, new GUILayoutOption[0]);
            EditorGUI.indentLevel++;
            if (this.m_UpdateMode.intValue == 1)
            {
                EditorGUILayout.PropertyField(this.m_UpdatePeriod, styles.updatePeriod, new GUILayoutOption[0]);
            }
            EditorGUILayout.PropertyField(this.m_DoubleBuffered, styles.doubleBuffered, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_WrapUpdateZones, styles.wrapUpdateZones, new GUILayoutOption[0]);
            bool flag = true;
            foreach (UnityEngine.Object obj2 in base.targets)
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
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            this.m_ShowInitSourceAsMaterial.valueChanged.RemoveListener(new UnityAction(this.Repaint));
        }

        private void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            CustomRenderTexture target = base.target as CustomRenderTexture;
            bool flag = target.dimension == TextureDimension.Tex3D;
            bool doubleBuffered = target.doubleBuffered;
            SerializedProperty arrayElementAtIndex = this.m_RectList.serializedProperty.GetArrayElementAtIndex(index);
            float singleLineHeight = EditorGUIUtility.singleLineHeight;
            rect.y += EditorGUIUtility.standardVerticalSpacing;
            rect.height = singleLineHeight;
            EditorGUI.LabelField(rect, $"Update Zone {index}");
            rect.y += singleLineHeight;
            SerializedProperty prop = arrayElementAtIndex.FindPropertyRelative("updateZoneCenter");
            this.UpdateZoneVec3PropertyField(rect, prop, styles.updateZoneCenter, !flag);
            rect.y += singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
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
            return ((EditorGUIUtility.singleLineHeight + 2f) * num);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.m_Material = base.serializedObject.FindProperty("m_Material");
            this.m_ShaderPass = base.serializedObject.FindProperty("m_ShaderPass");
            this.m_InitializationMode = base.serializedObject.FindProperty("m_InitializationMode");
            this.m_InitSource = base.serializedObject.FindProperty("m_InitSource");
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
            this.m_RectList.elementHeightCallback = new ReorderableList.ElementHeightCallbackDelegate(this.OnElementHeight);
            this.m_RectList.footerHeight = 0f;
            this.m_ShowInitSourceAsMaterial.value = !this.m_InitSource.hasMultipleDifferentValues && (this.m_InitSource.intValue == 1);
            this.m_ShowInitSourceAsMaterial.valueChanged.AddListener(new UnityAction(this.Repaint));
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
        }

        [UnityEditor.MenuItem("CONTEXT/CustomRenderTexture/Export", false)]
        private static void SaveToDisk(MenuCommand command)
        {
            CustomRenderTexture context = command.context as CustomRenderTexture;
            int width = context.width;
            int height = context.height;
            int volumeDepth = context.volumeDepth;
            bool flag = RenderTextureEditor.IsHDRFormat(context.format);
            bool flag2 = (context.format == RenderTextureFormat.ARGBFloat) || (context.format == RenderTextureFormat.RFloat);
            TextureFormat format = !flag ? TextureFormat.RGBA32 : TextureFormat.RGBAFloat;
            int num4 = width;
            if (context.dimension == TextureDimension.Tex3D)
            {
                num4 = width * volumeDepth;
            }
            else if (context.dimension == TextureDimension.Cube)
            {
                num4 = width * 6;
            }
            Texture2D tex = new Texture2D(num4, height, format, false);
            if (context.dimension == TextureDimension.Tex2D)
            {
                Graphics.SetRenderTarget(context);
                tex.ReadPixels(new Rect(0f, 0f, (float) width, (float) height), 0, 0);
                tex.Apply();
            }
            else if (context.dimension == TextureDimension.Tex3D)
            {
                int destX = 0;
                for (int i = 0; i < volumeDepth; i++)
                {
                    Graphics.SetRenderTarget(context, 0, CubemapFace.Unknown, i);
                    tex.ReadPixels(new Rect(0f, 0f, (float) width, (float) height), destX, 0);
                    tex.Apply();
                    destX += width;
                }
            }
            else
            {
                int num7 = 0;
                for (int j = 0; j < 6; j++)
                {
                    Graphics.SetRenderTarget(context, 0, (CubemapFace) j);
                    tex.ReadPixels(new Rect(0f, 0f, (float) width, (float) height), num7, 0);
                    tex.Apply();
                    num7 += width;
                }
            }
            byte[] bytes = null;
            if (flag)
            {
                bytes = tex.EncodeToEXR(Texture2D.EXRFlags.CompressZIP | (!flag2 ? Texture2D.EXRFlags.None : Texture2D.EXRFlags.OutputAsFloat));
            }
            else
            {
                bytes = tex.EncodeToPNG();
            }
            UnityEngine.Object.DestroyImmediate(tex);
            string extension = !flag ? "png" : "exr";
            string directoryName = Path.GetDirectoryName(AssetDatabase.GetAssetPath(context.GetInstanceID()));
            string str3 = EditorUtility.SaveFilePanel("Save Custom Texture", directoryName, context.name, extension);
            if (!string.IsNullOrEmpty(str3))
            {
                File.WriteAllBytes(str3, bytes);
                AssetDatabase.Refresh();
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
            public readonly GUIContent initColor = EditorGUIUtility.TextContent("Color|Color with which the custom texture is initialized.");
            public readonly GUIContent initializationMode = EditorGUIUtility.TextContent("Initialization Mode|Specify how the texture should be initialized.");
            public readonly GUIContent initMaterial = EditorGUIUtility.TextContent("Material|Material with which the custom texture is initialized.");
            public readonly GUIContent initSource = EditorGUIUtility.TextContent("Source|Specify if the texture is initialized by a Material or by a Texture and a Color.");
            public readonly GUIContent[] initSourceStrings;
            public readonly int[] initSourceValues;
            public readonly GUIContent initTexture = EditorGUIUtility.TextContent("Texture|Texture with which the custom texture is initialized (multiplied by the initialization color).");
            public readonly GUIContent materials = EditorGUIUtility.TextContent("Materials");
            public readonly GUIContent needSwap = EditorGUIUtility.TextContent("Swap (Double Buffer)|If ticked, and if the texture is double buffered, a request is made to swap the buffers before the next update. If this is not ticked, the buffers will not be swapped.");
            public readonly GUIContent saveButton = EditorGUIUtility.TextContent("Save Texture|Save the content of the custom texture to an EXR or PNG file.");
            public readonly GUIStyle separator = "sv_iconselector_sep";
            public readonly GUIContent shaderPass = EditorGUIUtility.TextContent("Shader Pass|Shader Pass used to update the Custom Texture.");
            public readonly GUIContent updateMode = EditorGUIUtility.TextContent("Update Mode|Specify how the texture should be updated.");
            public readonly GUIContent[] updateModeStrings = new GUIContent[] { EditorGUIUtility.TextContent("OnLoad"), EditorGUIUtility.TextContent("Realtime"), EditorGUIUtility.TextContent("OnDemand") };
            public readonly int[] updateModeValues;
            public readonly GUIContent updatePeriod = EditorGUIUtility.TextContent("Period|Period in seconds at which real-time textures are updated (0.0 will update every frame).");
            public readonly GUIContent updateZoneCenter = EditorGUIUtility.TextContent("Center|Center of the partial update zone.");
            public readonly GUIContent updateZoneList = EditorGUIUtility.TextContent("Update Zones|List of partial update zones.");
            public readonly GUIContent updateZoneRotation = EditorGUIUtility.TextContent("Rotation|Rotation of the update zone.");
            public readonly GUIContent updateZoneSize = EditorGUIUtility.TextContent("Size|Size of the partial update zone.");
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
                this.initSourceStrings = new GUIContent[] { EditorGUIUtility.TextContent("Texture and Color"), EditorGUIUtility.TextContent("Material") };
                int[] numArray2 = new int[2];
                numArray2[1] = 1;
                this.initSourceValues = numArray2;
                this.updateZoneSpaceStrings = new GUIContent[] { EditorGUIUtility.TextContent("Normalized"), EditorGUIUtility.TextContent("Pixel") };
                int[] numArray3 = new int[2];
                numArray3[1] = 1;
                this.updateZoneSpaceValues = numArray3;
                this.cubemapFaces = new GUIContent[] { EditorGUIUtility.TextContent("+X"), EditorGUIUtility.TextContent("-X"), EditorGUIUtility.TextContent("+Y"), EditorGUIUtility.TextContent("-Y"), EditorGUIUtility.TextContent("+Z"), EditorGUIUtility.TextContent("-Z") };
            }
        }
    }
}

