namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    internal class LightTableColumns
    {
        [CompilerGenerated]
        private static SerializedPropertyTreeView.Column.DrawEntry <>f__am$cache0;
        [CompilerGenerated]
        private static SerializedPropertyTreeView.Column.DrawEntry <>f__am$cache1;
        [CompilerGenerated]
        private static SerializedPropertyTreeView.Column.DrawEntry <>f__am$cache2;
        [CompilerGenerated]
        private static SerializedPropertyTreeView.Column.DrawEntry <>f__am$cache3;
        [CompilerGenerated]
        private static SerializedPropertyTreeView.Column.DrawEntry <>f__am$cache4;
        [CompilerGenerated]
        private static SerializedPropertyTreeView.Column.CompareEntry <>f__am$cache5;
        [CompilerGenerated]
        private static SerializedPropertyTreeView.Column.DrawEntry <>f__am$cache6;
        [CompilerGenerated]
        private static SerializedPropertyTreeView.Column.CopyDelegate <>f__am$cache7;
        private static ColorPickerHDRConfig s_ColorPickerHDRConfig = new ColorPickerHDRConfig(0f, 99f, 0.01010101f, 3f);

        public static SerializedPropertyTreeView.Column[] CreateEmissivesColumns(out string[] propNames)
        {
            SerializedPropertyTreeView.Column[] columnArray1 = new SerializedPropertyTreeView.Column[4];
            SerializedPropertyTreeView.Column column = new SerializedPropertyTreeView.Column {
                headerContent = Styles.SelectObjects,
                headerTextAlignment = TextAlignment.Left,
                sortedAscending = true,
                sortingArrowAlignment = TextAlignment.Center,
                width = 20f,
                minWidth = 20f,
                maxWidth = 20f,
                autoResize = false,
                allowToggleVisibility = true,
                propertyName = "m_LightmapFlags",
                dependencyIndices = null,
                compareDelegate = null
            };
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = delegate (Rect r, SerializedProperty prop, SerializedProperty[] dep) {
                    if (GUI.Button(r, Styles.SelectObjectsButton, "label"))
                    {
                        SearchableEditorWindow.SearchForReferencesToInstanceID(prop.serializedObject.targetObject.GetInstanceID());
                    }
                };
            }
            column.drawDelegate = <>f__am$cache3;
            columnArray1[0] = column;
            column = new SerializedPropertyTreeView.Column {
                headerContent = Styles.Name,
                headerTextAlignment = TextAlignment.Left,
                sortedAscending = true,
                sortingArrowAlignment = TextAlignment.Center,
                width = 150f,
                minWidth = 30f,
                autoResize = false,
                allowToggleVisibility = true,
                propertyName = null,
                dependencyIndices = null,
                compareDelegate = SerializedPropertyTreeView.DefaultDelegates.s_CompareName,
                drawDelegate = SerializedPropertyTreeView.DefaultDelegates.s_DrawName,
                filter = new SerializedPropertyFilters.Name()
            };
            columnArray1[1] = column;
            column = new SerializedPropertyTreeView.Column {
                headerContent = Styles.GlobalIllumination,
                headerTextAlignment = TextAlignment.Left,
                sortedAscending = true,
                sortingArrowAlignment = TextAlignment.Center,
                width = 130f,
                minWidth = 90f,
                autoResize = true,
                allowToggleVisibility = true,
                propertyName = "m_LightmapFlags",
                dependencyIndices = null,
                compareDelegate = SerializedPropertyTreeView.DefaultDelegates.s_CompareInt
            };
            if (<>f__am$cache4 == null)
            {
                <>f__am$cache4 = delegate (Rect r, SerializedProperty prop, SerializedProperty[] dep) {
                    if (prop.serializedObject.targetObject.GetType().Equals(typeof(Material)))
                    {
                        using (new EditorGUI.DisabledScope(!IsEditable(prop.serializedObject.targetObject)))
                        {
                            MaterialGlobalIlluminationFlags flags = ((prop.intValue & 2) == 0) ? MaterialGlobalIlluminationFlags.RealtimeEmissive : MaterialGlobalIlluminationFlags.BakedEmissive;
                            int[] optionValues = new int[] { 1, 2 };
                            EditorGUI.BeginChangeCheck();
                            flags = (MaterialGlobalIlluminationFlags) EditorGUI.IntPopup(r, (int) flags, Styles.LightmapEmissiveStrings, optionValues);
                            if (EditorGUI.EndChangeCheck())
                            {
                                Material targetObject = (Material) prop.serializedObject.targetObject;
                                Material[] objectsToUndo = new Material[] { targetObject };
                                Undo.RecordObjects(objectsToUndo, "Modify GI Settings of " + targetObject.name);
                                targetObject.globalIlluminationFlags = flags;
                                prop.serializedObject.Update();
                            }
                        }
                    }
                };
            }
            column.drawDelegate = <>f__am$cache4;
            columnArray1[2] = column;
            column = new SerializedPropertyTreeView.Column {
                headerContent = Styles.Intensity,
                headerTextAlignment = TextAlignment.Left,
                sortedAscending = true,
                sortingArrowAlignment = TextAlignment.Center,
                width = 120f,
                minWidth = 50f,
                autoResize = true,
                allowToggleVisibility = true,
                propertyName = "m_Shader",
                dependencyIndices = null
            };
            if (<>f__am$cache5 == null)
            {
                <>f__am$cache5 = delegate (SerializedProperty lhs, SerializedProperty rhs) {
                    float num;
                    float num2;
                    float num3;
                    float num4;
                    float num5;
                    float num6;
                    Color.RGBToHSV(((Material) lhs.serializedObject.targetObject).GetColor("_EmissionColor"), out num, out num2, out num3);
                    Color.RGBToHSV(((Material) rhs.serializedObject.targetObject).GetColor("_EmissionColor"), out num4, out num5, out num6);
                    return num3.CompareTo(num6);
                };
            }
            column.compareDelegate = <>f__am$cache5;
            if (<>f__am$cache6 == null)
            {
                <>f__am$cache6 = delegate (Rect r, SerializedProperty prop, SerializedProperty[] dep) {
                    if (prop.serializedObject.targetObject.GetType().Equals(typeof(Material)))
                    {
                        using (new EditorGUI.DisabledScope(!IsEditable(prop.serializedObject.targetObject)))
                        {
                            ColorPickerHDRConfig defaultHDRConfig;
                            Material targetObject = (Material) prop.serializedObject.targetObject;
                            Color color = targetObject.GetColor("_EmissionColor");
                            if (s_ColorPickerHDRConfig != null)
                            {
                                defaultHDRConfig = s_ColorPickerHDRConfig;
                            }
                            else
                            {
                                defaultHDRConfig = ColorPicker.defaultHDRConfig;
                            }
                            EditorGUI.BeginChangeCheck();
                            Color color2 = EditorGUI.ColorBrightnessField(r, GUIContent.Temp(""), color, defaultHDRConfig.minBrightness, defaultHDRConfig.maxBrightness);
                            if (EditorGUI.EndChangeCheck())
                            {
                                Material[] objectsToUndo = new Material[] { targetObject };
                                Undo.RecordObjects(objectsToUndo, "Modify Color of " + targetObject.name);
                                targetObject.SetColor("_EmissionColor", color2);
                            }
                        }
                    }
                };
            }
            column.drawDelegate = <>f__am$cache6;
            if (<>f__am$cache7 == null)
            {
                <>f__am$cache7 = delegate (SerializedProperty target, SerializedProperty source) {
                    Color color = ((Material) source.serializedObject.targetObject).GetColor("_EmissionColor");
                    ((Material) target.serializedObject.targetObject).SetColor("_EmissionColor", color);
                };
            }
            column.copyDelegate = <>f__am$cache7;
            columnArray1[3] = column;
            SerializedPropertyTreeView.Column[] columns = columnArray1;
            return FinalizeColumns(columns, out propNames);
        }

        public static SerializedPropertyTreeView.Column[] CreateLightColumns(out string[] propNames)
        {
            SerializedPropertyTreeView.Column[] columnArray1 = new SerializedPropertyTreeView.Column[8];
            SerializedPropertyTreeView.Column column = new SerializedPropertyTreeView.Column {
                headerContent = Styles.Name,
                headerTextAlignment = TextAlignment.Left,
                sortedAscending = true,
                sortingArrowAlignment = TextAlignment.Center,
                width = 150f,
                minWidth = 30f,
                autoResize = false,
                allowToggleVisibility = true,
                propertyName = null,
                dependencyIndices = null,
                compareDelegate = SerializedPropertyTreeView.DefaultDelegates.s_CompareName,
                drawDelegate = SerializedPropertyTreeView.DefaultDelegates.s_DrawName,
                filter = new SerializedPropertyFilters.Name()
            };
            columnArray1[0] = column;
            column = new SerializedPropertyTreeView.Column {
                headerContent = Styles.On,
                headerTextAlignment = TextAlignment.Center,
                sortedAscending = true,
                sortingArrowAlignment = TextAlignment.Center,
                width = 24f,
                minWidth = 24f,
                maxWidth = 24f,
                autoResize = false,
                allowToggleVisibility = true,
                propertyName = "m_Enabled",
                dependencyIndices = null,
                compareDelegate = SerializedPropertyTreeView.DefaultDelegates.s_CompareCheckbox,
                drawDelegate = SerializedPropertyTreeView.DefaultDelegates.s_DrawCheckbox
            };
            columnArray1[1] = column;
            column = new SerializedPropertyTreeView.Column {
                headerContent = Styles.Type,
                headerTextAlignment = TextAlignment.Left,
                sortedAscending = true,
                sortingArrowAlignment = TextAlignment.Center,
                width = 90f,
                minWidth = 30f,
                autoResize = true,
                allowToggleVisibility = true,
                propertyName = "m_Type",
                dependencyIndices = null,
                compareDelegate = SerializedPropertyTreeView.DefaultDelegates.s_CompareEnum,
                drawDelegate = SerializedPropertyTreeView.DefaultDelegates.s_DrawDefault
            };
            columnArray1[2] = column;
            column = new SerializedPropertyTreeView.Column {
                headerContent = Styles.Mode,
                headerTextAlignment = TextAlignment.Left,
                sortedAscending = true,
                sortingArrowAlignment = TextAlignment.Center,
                width = 90f,
                minWidth = 90f,
                autoResize = false,
                allowToggleVisibility = true,
                propertyName = "m_Lightmapping"
            };
            column.dependencyIndices = new int[] { 2 };
            column.compareDelegate = SerializedPropertyTreeView.DefaultDelegates.s_CompareEnum;
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = delegate (Rect r, SerializedProperty prop, SerializedProperty[] dep) {
                    LightModeUtil.Get().DrawElement(r, prop, dep[0]);
                };
            }
            column.drawDelegate = <>f__am$cache0;
            columnArray1[3] = column;
            column = new SerializedPropertyTreeView.Column {
                headerContent = Styles.Color,
                headerTextAlignment = TextAlignment.Left,
                sortedAscending = true,
                sortingArrowAlignment = TextAlignment.Center,
                width = 50f,
                minWidth = 50f,
                autoResize = true,
                allowToggleVisibility = true,
                propertyName = "m_Color",
                dependencyIndices = null,
                compareDelegate = SerializedPropertyTreeView.DefaultDelegates.s_CompareColor,
                drawDelegate = SerializedPropertyTreeView.DefaultDelegates.s_DrawDefault
            };
            columnArray1[4] = column;
            column = new SerializedPropertyTreeView.Column {
                headerContent = Styles.Intensity,
                headerTextAlignment = TextAlignment.Left,
                sortedAscending = true,
                sortingArrowAlignment = TextAlignment.Center,
                width = 60f,
                minWidth = 30f,
                autoResize = true,
                allowToggleVisibility = true,
                propertyName = "m_Intensity",
                dependencyIndices = null,
                compareDelegate = SerializedPropertyTreeView.DefaultDelegates.s_CompareFloat,
                drawDelegate = SerializedPropertyTreeView.DefaultDelegates.s_DrawDefault
            };
            columnArray1[5] = column;
            column = new SerializedPropertyTreeView.Column {
                headerContent = Styles.IndirectMultiplier,
                headerTextAlignment = TextAlignment.Left,
                sortedAscending = true,
                sortingArrowAlignment = TextAlignment.Center,
                width = 120f,
                minWidth = 30f,
                autoResize = true,
                allowToggleVisibility = true,
                propertyName = "m_BounceIntensity",
                dependencyIndices = null,
                compareDelegate = SerializedPropertyTreeView.DefaultDelegates.s_CompareFloat,
                drawDelegate = SerializedPropertyTreeView.DefaultDelegates.s_DrawDefault
            };
            columnArray1[6] = column;
            column = new SerializedPropertyTreeView.Column {
                headerContent = Styles.ShadowType,
                headerTextAlignment = TextAlignment.Left,
                sortedAscending = true,
                sortingArrowAlignment = TextAlignment.Center,
                width = 100f,
                minWidth = 40f,
                autoResize = true,
                allowToggleVisibility = true,
                propertyName = "m_Shadows.m_Type",
                dependencyIndices = null,
                compareDelegate = SerializedPropertyTreeView.DefaultDelegates.s_CompareEnum,
                drawDelegate = SerializedPropertyTreeView.DefaultDelegates.s_DrawDefault
            };
            columnArray1[7] = column;
            SerializedPropertyTreeView.Column[] columns = columnArray1;
            return FinalizeColumns(columns, out propNames);
        }

        public static SerializedPropertyTreeView.Column[] CreateLightProbeColumns(out string[] propNames)
        {
            SerializedPropertyTreeView.Column[] columnArray1 = new SerializedPropertyTreeView.Column[2];
            SerializedPropertyTreeView.Column column = new SerializedPropertyTreeView.Column {
                headerContent = Styles.Name,
                headerTextAlignment = TextAlignment.Left,
                sortedAscending = true,
                sortingArrowAlignment = TextAlignment.Center,
                width = 150f,
                minWidth = 30f,
                autoResize = false,
                allowToggleVisibility = true,
                propertyName = null,
                dependencyIndices = null,
                compareDelegate = SerializedPropertyTreeView.DefaultDelegates.s_CompareName,
                drawDelegate = SerializedPropertyTreeView.DefaultDelegates.s_DrawName,
                filter = new SerializedPropertyFilters.Name()
            };
            columnArray1[0] = column;
            column = new SerializedPropertyTreeView.Column {
                headerContent = Styles.On,
                headerTextAlignment = TextAlignment.Center,
                sortedAscending = true,
                sortingArrowAlignment = TextAlignment.Center,
                width = 24f,
                minWidth = 24f,
                maxWidth = 24f,
                autoResize = false,
                allowToggleVisibility = true,
                propertyName = "m_Enabled",
                dependencyIndices = null,
                compareDelegate = SerializedPropertyTreeView.DefaultDelegates.s_CompareCheckbox,
                drawDelegate = SerializedPropertyTreeView.DefaultDelegates.s_DrawCheckbox
            };
            columnArray1[1] = column;
            SerializedPropertyTreeView.Column[] columns = columnArray1;
            return FinalizeColumns(columns, out propNames);
        }

        public static SerializedPropertyTreeView.Column[] CreateReflectionColumns(out string[] propNames)
        {
            SerializedPropertyTreeView.Column[] columnArray1 = new SerializedPropertyTreeView.Column[8];
            SerializedPropertyTreeView.Column column = new SerializedPropertyTreeView.Column {
                headerContent = Styles.Name,
                headerTextAlignment = TextAlignment.Left,
                sortedAscending = true,
                sortingArrowAlignment = TextAlignment.Center,
                width = 150f,
                minWidth = 30f,
                autoResize = false,
                allowToggleVisibility = true,
                propertyName = null,
                dependencyIndices = null,
                compareDelegate = SerializedPropertyTreeView.DefaultDelegates.s_CompareName,
                drawDelegate = SerializedPropertyTreeView.DefaultDelegates.s_DrawName,
                filter = new SerializedPropertyFilters.Name()
            };
            columnArray1[0] = column;
            column = new SerializedPropertyTreeView.Column {
                headerContent = Styles.On,
                headerTextAlignment = TextAlignment.Center,
                sortedAscending = true,
                sortingArrowAlignment = TextAlignment.Center,
                width = 24f,
                minWidth = 24f,
                maxWidth = 24f,
                autoResize = false,
                allowToggleVisibility = true,
                propertyName = "m_Enabled",
                dependencyIndices = null,
                compareDelegate = SerializedPropertyTreeView.DefaultDelegates.s_CompareCheckbox,
                drawDelegate = SerializedPropertyTreeView.DefaultDelegates.s_DrawCheckbox
            };
            columnArray1[1] = column;
            column = new SerializedPropertyTreeView.Column {
                headerContent = Styles.Mode,
                headerTextAlignment = TextAlignment.Left,
                sortedAscending = true,
                sortingArrowAlignment = TextAlignment.Center,
                width = 70f,
                minWidth = 40f,
                autoResize = true,
                allowToggleVisibility = true,
                propertyName = "m_Mode",
                dependencyIndices = null,
                compareDelegate = SerializedPropertyTreeView.DefaultDelegates.s_CompareInt
            };
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = delegate (Rect r, SerializedProperty prop, SerializedProperty[] dep) {
                    EditorGUI.IntPopup(r, prop, ReflectionProbeEditor.Styles.reflectionProbeMode, ReflectionProbeEditor.Styles.reflectionProbeModeValues, GUIContent.none);
                };
            }
            column.drawDelegate = <>f__am$cache1;
            columnArray1[2] = column;
            column = new SerializedPropertyTreeView.Column {
                headerContent = Styles.Projection,
                headerTextAlignment = TextAlignment.Left,
                sortedAscending = true,
                sortingArrowAlignment = TextAlignment.Center,
                width = 80f,
                minWidth = 40f,
                autoResize = true,
                allowToggleVisibility = true,
                propertyName = "m_BoxProjection",
                dependencyIndices = null,
                compareDelegate = SerializedPropertyTreeView.DefaultDelegates.s_CompareCheckbox
            };
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = delegate (Rect r, SerializedProperty prop, SerializedProperty[] dep) {
                    int[] numArray1 = new int[2];
                    numArray1[1] = 1;
                    int[] optionValues = numArray1;
                    prop.boolValue = EditorGUI.IntPopup(r, !prop.boolValue ? 0 : 1, Styles.ProjectionStrings, optionValues) == 1;
                };
            }
            column.drawDelegate = <>f__am$cache2;
            columnArray1[3] = column;
            column = new SerializedPropertyTreeView.Column {
                headerContent = Styles.HDR,
                headerTextAlignment = TextAlignment.Center,
                sortedAscending = true,
                sortingArrowAlignment = TextAlignment.Center,
                width = 30f,
                minWidth = 30f,
                maxWidth = 30f,
                autoResize = true,
                allowToggleVisibility = true,
                propertyName = "m_HDR",
                dependencyIndices = null,
                compareDelegate = SerializedPropertyTreeView.DefaultDelegates.s_CompareCheckbox,
                drawDelegate = SerializedPropertyTreeView.DefaultDelegates.s_DrawCheckbox
            };
            columnArray1[4] = column;
            column = new SerializedPropertyTreeView.Column {
                headerContent = Styles.ShadowDistance,
                headerTextAlignment = TextAlignment.Left,
                sortedAscending = true,
                sortingArrowAlignment = TextAlignment.Center,
                width = 130f,
                minWidth = 50f,
                autoResize = true,
                allowToggleVisibility = true,
                propertyName = "m_ShadowDistance",
                dependencyIndices = null,
                compareDelegate = SerializedPropertyTreeView.DefaultDelegates.s_CompareFloat,
                drawDelegate = SerializedPropertyTreeView.DefaultDelegates.s_DrawDefault
            };
            columnArray1[5] = column;
            column = new SerializedPropertyTreeView.Column {
                headerContent = Styles.NearPlane,
                headerTextAlignment = TextAlignment.Left,
                sortedAscending = true,
                sortingArrowAlignment = TextAlignment.Center,
                width = 80f,
                minWidth = 50f,
                autoResize = true,
                allowToggleVisibility = true,
                propertyName = "m_NearClip",
                dependencyIndices = null,
                compareDelegate = SerializedPropertyTreeView.DefaultDelegates.s_CompareFloat,
                drawDelegate = SerializedPropertyTreeView.DefaultDelegates.s_DrawDefault
            };
            columnArray1[6] = column;
            column = new SerializedPropertyTreeView.Column {
                headerContent = Styles.FarPlane,
                headerTextAlignment = TextAlignment.Left,
                sortedAscending = true,
                sortingArrowAlignment = TextAlignment.Center,
                width = 80f,
                minWidth = 50f,
                autoResize = true,
                allowToggleVisibility = true,
                propertyName = "m_FarClip",
                dependencyIndices = null,
                compareDelegate = SerializedPropertyTreeView.DefaultDelegates.s_CompareFloat,
                drawDelegate = SerializedPropertyTreeView.DefaultDelegates.s_DrawDefault
            };
            columnArray1[7] = column;
            SerializedPropertyTreeView.Column[] columns = columnArray1;
            return FinalizeColumns(columns, out propNames);
        }

        private static SerializedPropertyTreeView.Column[] FinalizeColumns(SerializedPropertyTreeView.Column[] columns, out string[] propNames)
        {
            propNames = new string[columns.Length];
            for (int i = 0; i < columns.Length; i++)
            {
                propNames[i] = columns[i].propertyName;
            }
            return columns;
        }

        private static bool IsEditable(UnityEngine.Object target) => 
            ((target.hideFlags & HideFlags.NotEditable) == HideFlags.None);

        private static class Styles
        {
            public static readonly GUIContent Color = EditorGUIUtility.TextContent("Color");
            public static readonly GUIContent FarPlane = EditorGUIUtility.TextContent("Far Plane");
            public static readonly GUIContent GlobalIllumination = EditorGUIUtility.TextContent("Global Illumination");
            public static readonly GUIContent HDR = EditorGUIUtility.TextContent("HDR");
            public static readonly GUIContent IndirectMultiplier = EditorGUIUtility.TextContent("Indirect Multiplier");
            public static readonly GUIContent Intensity = EditorGUIUtility.TextContent("Intensity");
            public static readonly GUIContent[] LightmapEmissiveStrings = new GUIContent[] { EditorGUIUtility.TextContent("Realtime"), EditorGUIUtility.TextContent("Baked") };
            public static readonly GUIContent Mode = EditorGUIUtility.TextContent("Mode");
            public static readonly GUIContent Name = EditorGUIUtility.TextContent("Name");
            public static readonly GUIContent NearPlane = EditorGUIUtility.TextContent("Near Plane");
            public static readonly GUIContent On = EditorGUIUtility.TextContent("On");
            public static readonly GUIContent Projection = EditorGUIUtility.TextContent("Projection");
            public static readonly GUIContent[] ProjectionStrings = new GUIContent[] { EditorGUIUtility.TextContent("Infinite"), EditorGUIUtility.TextContent("Box") };
            public static readonly GUIContent SelectObjects = EditorGUIUtility.TextContent("");
            public static readonly GUIContent SelectObjectsButton = EditorGUIUtility.TextContentWithIcon("|Find References in Scene", "UnityEditor.FindDependencies");
            public static readonly GUIContent ShadowDistance = EditorGUIUtility.TextContent("Shadow Distance");
            public static readonly GUIContent ShadowType = EditorGUIUtility.TextContent("Shadow Type");
            public static readonly GUIContent Type = EditorGUIUtility.TextContent("Type");
        }
    }
}

