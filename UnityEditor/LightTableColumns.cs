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
        private static int s_DontCareWidth = 0x2710;
        private static GUIContent[] s_ProjectionStrings = new GUIContent[] { EditorGUIUtility.TextContent("Infinite"), EditorGUIUtility.TextContent("Box") };

        public static SerializedPropertyTreeView.Column[] CreateLightColumns(out string[] propNames)
        {
            SerializedPropertyTreeView.Column[] columnArray1 = new SerializedPropertyTreeView.Column[8];
            SerializedPropertyTreeView.Column column = new SerializedPropertyTreeView.Column {
                headerContent = new GUIContent("Name"),
                headerTextAlignment = TextAlignment.Left,
                sortedAscending = true,
                sortingArrowAlignment = TextAlignment.Center,
                width = 150f,
                minWidth = 30f,
                maxWidth = s_DontCareWidth,
                autoResize = false,
                allowToggleVisibility = true,
                propertyName = null,
                dependencyIndices = null,
                drawDelegate = SerializedPropertyTreeView.DefaultDelegates.s_DrawName,
                compareDelegate = SerializedPropertyTreeView.DefaultDelegates.s_CompareName,
                filter = new SerializedPropertyFilters.Name()
            };
            columnArray1[0] = column;
            column = new SerializedPropertyTreeView.Column {
                headerContent = new GUIContent("On"),
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
                drawDelegate = SerializedPropertyTreeView.DefaultDelegates.s_DrawCheckbox,
                compareDelegate = SerializedPropertyTreeView.DefaultDelegates.s_CompareCheckbox,
                filter = new SerializedPropertyFilters.Checkbox()
            };
            columnArray1[1] = column;
            column = new SerializedPropertyTreeView.Column {
                headerContent = new GUIContent("Type"),
                headerTextAlignment = TextAlignment.Left,
                sortedAscending = true,
                sortingArrowAlignment = TextAlignment.Center,
                width = 90f,
                minWidth = 30f,
                maxWidth = s_DontCareWidth,
                autoResize = true,
                allowToggleVisibility = true,
                propertyName = "m_Type",
                dependencyIndices = null,
                drawDelegate = SerializedPropertyTreeView.DefaultDelegates.s_DrawDefault,
                compareDelegate = SerializedPropertyTreeView.DefaultDelegates.s_CompareEnum,
                filter = new LightTableFilters.LightTypeEnum()
            };
            columnArray1[2] = column;
            column = new SerializedPropertyTreeView.Column {
                headerContent = new GUIContent("Mode"),
                headerTextAlignment = TextAlignment.Left,
                sortedAscending = true,
                sortingArrowAlignment = TextAlignment.Center,
                width = 90f,
                minWidth = 90f,
                maxWidth = s_DontCareWidth,
                autoResize = false,
                allowToggleVisibility = true,
                propertyName = "m_Lightmapping"
            };
            column.dependencyIndices = new int[] { 2 };
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = delegate (Rect r, SerializedProperty prop, SerializedProperty[] dep) {
                    LightModeUtil.Get().DrawElement(r, prop, dep[0]);
                };
            }
            column.drawDelegate = <>f__am$cache0;
            column.compareDelegate = SerializedPropertyTreeView.DefaultDelegates.s_CompareEnum;
            column.filter = new LightTableFilters.LightModeEnum();
            columnArray1[3] = column;
            column = new SerializedPropertyTreeView.Column {
                headerContent = new GUIContent("Color"),
                headerTextAlignment = TextAlignment.Left,
                sortedAscending = true,
                sortingArrowAlignment = TextAlignment.Center,
                width = 50f,
                minWidth = 50f,
                maxWidth = s_DontCareWidth,
                autoResize = true,
                allowToggleVisibility = true,
                propertyName = "m_Color",
                dependencyIndices = null,
                drawDelegate = SerializedPropertyTreeView.DefaultDelegates.s_DrawDefault,
                compareDelegate = SerializedPropertyTreeView.DefaultDelegates.s_CompareColor,
                filter = SerializedPropertyFilters.s_FilterNone
            };
            columnArray1[4] = column;
            column = new SerializedPropertyTreeView.Column {
                headerContent = new GUIContent("Intensity"),
                headerTextAlignment = TextAlignment.Left,
                sortedAscending = true,
                sortingArrowAlignment = TextAlignment.Center,
                width = 60f,
                minWidth = 30f,
                maxWidth = s_DontCareWidth,
                autoResize = true,
                allowToggleVisibility = true,
                propertyName = "m_Intensity",
                dependencyIndices = null,
                drawDelegate = SerializedPropertyTreeView.DefaultDelegates.s_DrawDefault,
                compareDelegate = SerializedPropertyTreeView.DefaultDelegates.s_CompareFloat,
                filter = new SerializedPropertyFilters.FloatValue()
            };
            columnArray1[5] = column;
            column = new SerializedPropertyTreeView.Column {
                headerContent = new GUIContent("Indirect Multiplier"),
                headerTextAlignment = TextAlignment.Left,
                sortedAscending = true,
                sortingArrowAlignment = TextAlignment.Center,
                width = 120f,
                minWidth = 30f,
                maxWidth = s_DontCareWidth,
                autoResize = true,
                allowToggleVisibility = true,
                propertyName = "m_BounceIntensity",
                dependencyIndices = null,
                drawDelegate = SerializedPropertyTreeView.DefaultDelegates.s_DrawDefault,
                compareDelegate = SerializedPropertyTreeView.DefaultDelegates.s_CompareFloat,
                filter = new SerializedPropertyFilters.FloatValue()
            };
            columnArray1[6] = column;
            column = new SerializedPropertyTreeView.Column {
                headerContent = new GUIContent("Shadow Type"),
                headerTextAlignment = TextAlignment.Left,
                sortedAscending = true,
                sortingArrowAlignment = TextAlignment.Center,
                width = 100f,
                minWidth = 40f,
                maxWidth = s_DontCareWidth,
                autoResize = true,
                allowToggleVisibility = true,
                propertyName = "m_Shadows.m_Type",
                dependencyIndices = null,
                drawDelegate = SerializedPropertyTreeView.DefaultDelegates.s_DrawDefault,
                compareDelegate = SerializedPropertyTreeView.DefaultDelegates.s_CompareEnum,
                filter = new LightTableFilters.ShadowTypeEnum()
            };
            columnArray1[7] = column;
            SerializedPropertyTreeView.Column[] columns = columnArray1;
            return FinalizeColumns(columns, out propNames);
        }

        public static SerializedPropertyTreeView.Column[] CreateLightProbeColumns(out string[] propNames)
        {
            SerializedPropertyTreeView.Column[] columnArray1 = new SerializedPropertyTreeView.Column[2];
            SerializedPropertyTreeView.Column column = new SerializedPropertyTreeView.Column {
                headerContent = new GUIContent("Name"),
                headerTextAlignment = TextAlignment.Left,
                sortedAscending = true,
                sortingArrowAlignment = TextAlignment.Center,
                width = 150f,
                minWidth = 30f,
                maxWidth = s_DontCareWidth,
                autoResize = false,
                allowToggleVisibility = true,
                propertyName = null,
                dependencyIndices = null,
                drawDelegate = SerializedPropertyTreeView.DefaultDelegates.s_DrawName,
                compareDelegate = SerializedPropertyTreeView.DefaultDelegates.s_CompareName,
                filter = new SerializedPropertyFilters.Name()
            };
            columnArray1[0] = column;
            column = new SerializedPropertyTreeView.Column {
                headerContent = new GUIContent("On"),
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
                drawDelegate = SerializedPropertyTreeView.DefaultDelegates.s_DrawCheckbox,
                compareDelegate = SerializedPropertyTreeView.DefaultDelegates.s_CompareCheckbox,
                filter = new SerializedPropertyFilters.Checkbox()
            };
            columnArray1[1] = column;
            SerializedPropertyTreeView.Column[] columns = columnArray1;
            return FinalizeColumns(columns, out propNames);
        }

        public static SerializedPropertyTreeView.Column[] CreateReflectionColumns(out string[] propNames)
        {
            SerializedPropertyTreeView.Column[] columnArray1 = new SerializedPropertyTreeView.Column[8];
            SerializedPropertyTreeView.Column column = new SerializedPropertyTreeView.Column {
                headerContent = new GUIContent("Name"),
                headerTextAlignment = TextAlignment.Left,
                sortedAscending = true,
                sortingArrowAlignment = TextAlignment.Center,
                width = 150f,
                minWidth = 30f,
                maxWidth = s_DontCareWidth,
                autoResize = false,
                allowToggleVisibility = true,
                propertyName = null,
                dependencyIndices = null,
                drawDelegate = SerializedPropertyTreeView.DefaultDelegates.s_DrawName,
                compareDelegate = SerializedPropertyTreeView.DefaultDelegates.s_CompareName,
                filter = new SerializedPropertyFilters.Name()
            };
            columnArray1[0] = column;
            column = new SerializedPropertyTreeView.Column {
                headerContent = new GUIContent("On"),
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
                drawDelegate = SerializedPropertyTreeView.DefaultDelegates.s_DrawCheckbox,
                compareDelegate = SerializedPropertyTreeView.DefaultDelegates.s_CompareCheckbox,
                filter = new SerializedPropertyFilters.Checkbox()
            };
            columnArray1[1] = column;
            column = new SerializedPropertyTreeView.Column {
                headerContent = new GUIContent("Mode"),
                headerTextAlignment = TextAlignment.Left,
                sortedAscending = true,
                sortingArrowAlignment = TextAlignment.Center,
                width = 70f,
                minWidth = 40f,
                maxWidth = s_DontCareWidth,
                autoResize = true,
                allowToggleVisibility = true,
                propertyName = "m_Mode",
                dependencyIndices = null
            };
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = delegate (Rect r, SerializedProperty prop, SerializedProperty[] dep) {
                    EditorGUI.IntPopup(r, prop, ReflectionProbeEditor.Styles.reflectionProbeMode, ReflectionProbeEditor.Styles.reflectionProbeModeValues, GUIContent.none);
                };
            }
            column.drawDelegate = <>f__am$cache1;
            column.compareDelegate = SerializedPropertyTreeView.DefaultDelegates.s_CompareInt;
            column.filter = new LightTableFilters.ReflProbeModeEnum();
            columnArray1[2] = column;
            column = new SerializedPropertyTreeView.Column {
                headerContent = new GUIContent("Projection"),
                headerTextAlignment = TextAlignment.Left,
                sortedAscending = true,
                sortingArrowAlignment = TextAlignment.Center,
                width = 80f,
                minWidth = 40f,
                maxWidth = s_DontCareWidth,
                autoResize = true,
                allowToggleVisibility = true,
                propertyName = "m_BoxProjection",
                dependencyIndices = null
            };
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = delegate (Rect r, SerializedProperty prop, SerializedProperty[] dep) {
                    int[] numArray1 = new int[2];
                    numArray1[1] = 1;
                    int[] optionValues = numArray1;
                    prop.boolValue = EditorGUI.IntPopup(r, !prop.boolValue ? 0 : 1, s_ProjectionStrings, optionValues) == 1;
                };
            }
            column.drawDelegate = <>f__am$cache2;
            column.compareDelegate = SerializedPropertyTreeView.DefaultDelegates.s_CompareCheckbox;
            column.filter = new LightTableFilters.ReflProbeProjEnum();
            columnArray1[3] = column;
            column = new SerializedPropertyTreeView.Column {
                headerContent = new GUIContent("HDR"),
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
                drawDelegate = SerializedPropertyTreeView.DefaultDelegates.s_DrawCheckbox,
                compareDelegate = SerializedPropertyTreeView.DefaultDelegates.s_CompareCheckbox,
                filter = new SerializedPropertyFilters.Checkbox()
            };
            columnArray1[4] = column;
            column = new SerializedPropertyTreeView.Column {
                headerContent = new GUIContent("Shadow Distance"),
                headerTextAlignment = TextAlignment.Left,
                sortedAscending = true,
                sortingArrowAlignment = TextAlignment.Center,
                width = 130f,
                minWidth = 50f,
                maxWidth = s_DontCareWidth,
                autoResize = true,
                allowToggleVisibility = true,
                propertyName = "m_ShadowDistance",
                dependencyIndices = null,
                drawDelegate = SerializedPropertyTreeView.DefaultDelegates.s_DrawDefault,
                compareDelegate = SerializedPropertyTreeView.DefaultDelegates.s_CompareFloat,
                filter = new SerializedPropertyFilters.FloatValue()
            };
            columnArray1[5] = column;
            column = new SerializedPropertyTreeView.Column {
                headerContent = new GUIContent("Near Plane"),
                headerTextAlignment = TextAlignment.Left,
                sortedAscending = true,
                sortingArrowAlignment = TextAlignment.Center,
                width = 80f,
                minWidth = 50f,
                maxWidth = s_DontCareWidth,
                autoResize = true,
                allowToggleVisibility = true,
                propertyName = "m_NearClip",
                dependencyIndices = null,
                drawDelegate = SerializedPropertyTreeView.DefaultDelegates.s_DrawDefault,
                compareDelegate = SerializedPropertyTreeView.DefaultDelegates.s_CompareFloat,
                filter = new SerializedPropertyFilters.FloatValue()
            };
            columnArray1[6] = column;
            column = new SerializedPropertyTreeView.Column {
                headerContent = new GUIContent("Far Plane"),
                headerTextAlignment = TextAlignment.Left,
                sortedAscending = true,
                sortingArrowAlignment = TextAlignment.Center,
                width = 80f,
                minWidth = 50f,
                maxWidth = s_DontCareWidth,
                autoResize = true,
                allowToggleVisibility = true,
                propertyName = "m_FarClip",
                dependencyIndices = null,
                drawDelegate = SerializedPropertyTreeView.DefaultDelegates.s_DrawDefault,
                compareDelegate = SerializedPropertyTreeView.DefaultDelegates.s_CompareFloat,
                filter = new SerializedPropertyFilters.FloatValue()
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
    }
}

