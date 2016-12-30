namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    [EditorWindowTitle(title="Light Explorer", icon="Lighting")]
    internal class LightingExplorerWindow : EditorWindow
    {
        [CompilerGenerated]
        private static SerializedPropertyDataStore.GatherDelegate <>f__am$cache0;
        [CompilerGenerated]
        private static SerializedPropertyDataStore.GatherDelegate <>f__am$cache1;
        [CompilerGenerated]
        private static SerializedPropertyDataStore.GatherDelegate <>f__am$cache2;
        [CompilerGenerated]
        private static SerializedPropertyTable.HeaderDelegate <>f__mg$cache0;
        [CompilerGenerated]
        private static SerializedPropertyTable.HeaderDelegate <>f__mg$cache1;
        [CompilerGenerated]
        private static SerializedPropertyTable.HeaderDelegate <>f__mg$cache2;
        private TabType m_SelectedTab = TabType.Lights;
        private List<LightingExplorerWindowTab> m_TableTabs;
        private float m_ToolbarPadding = -1f;

        [MenuItem("Window/Lighting/Light Explorer", false, 0x833)]
        private static void CreateLightingExplorerWindow()
        {
            EditorWindow.GetWindow<LightingExplorerWindow>().minSize = new Vector2(300f, 250f);
        }

        private void OnDisable()
        {
            if (this.m_TableTabs != null)
            {
                for (int i = 0; i < this.m_TableTabs.Count; i++)
                {
                    this.m_TableTabs[i].OnDisable();
                }
            }
            EditorApplication.searchChanged = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.searchChanged, new EditorApplication.CallbackFunction(this.Repaint));
        }

        private void OnEnable()
        {
            base.titleContent = base.GetLocalizedTitleContent();
            if ((this.m_TableTabs == null) || (this.m_TableTabs.Count != 4))
            {
                List<LightingExplorerWindowTab> list = new List<LightingExplorerWindowTab>();
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = () => Object.FindObjectsOfType<Light>();
                }
                if (<>f__mg$cache0 == null)
                {
                    <>f__mg$cache0 = new SerializedPropertyTable.HeaderDelegate(LightTableColumns.CreateLightColumns);
                }
                list.Add(new LightingExplorerWindowTab(new SerializedPropertyTable("LightTable", <>f__am$cache0, <>f__mg$cache0)));
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = () => Object.FindObjectsOfType<ReflectionProbe>();
                }
                if (<>f__mg$cache1 == null)
                {
                    <>f__mg$cache1 = new SerializedPropertyTable.HeaderDelegate(LightTableColumns.CreateReflectionColumns);
                }
                list.Add(new LightingExplorerWindowTab(new SerializedPropertyTable("ReflectionTable", <>f__am$cache1, <>f__mg$cache1)));
                if (<>f__am$cache2 == null)
                {
                    <>f__am$cache2 = () => Object.FindObjectsOfType<LightProbeGroup>();
                }
                if (<>f__mg$cache2 == null)
                {
                    <>f__mg$cache2 = new SerializedPropertyTable.HeaderDelegate(LightTableColumns.CreateLightProbeColumns);
                }
                list.Add(new LightingExplorerWindowTab(new SerializedPropertyTable("LightProbeTable", <>f__am$cache2, <>f__mg$cache2)));
                list.Add(new LightingExplorerWindowTab(new EmissionTable()));
                this.m_TableTabs = list;
            }
            for (int i = 0; i < this.m_TableTabs.Count; i++)
            {
                this.m_TableTabs[i].OnEnable();
            }
            EditorApplication.searchChanged = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.searchChanged, new EditorApplication.CallbackFunction(this.Repaint));
            base.Repaint();
        }

        private void OnGUI()
        {
            EditorGUIUtility.labelWidth = 130f;
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Space(this.toolbarPadding);
            float width = base.position.width - (this.toolbarPadding * 2f);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(width) };
            this.m_SelectedTab = (TabType) GUILayout.Toolbar((int) this.m_SelectedTab, Styles.TabTypes, "LargeButton", options);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
            if (((this.m_TableTabs != null) && (this.m_SelectedTab >= TabType.Lights)) && (this.m_SelectedTab < this.m_TableTabs.Count))
            {
                this.m_TableTabs[(int) this.m_SelectedTab].OnGUI();
            }
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        private void OnHierarchyChange()
        {
            if (this.m_TableTabs != null)
            {
                for (int i = 0; i < this.m_TableTabs.Count; i++)
                {
                    this.m_TableTabs[i].OnHierarchyChange();
                }
            }
        }

        private void OnInspectorUpdate()
        {
            if (((this.m_TableTabs != null) && (this.m_SelectedTab >= TabType.Lights)) && (this.m_SelectedTab < this.m_TableTabs.Count))
            {
                this.m_TableTabs[(int) this.m_SelectedTab].OnInspectorUpdate();
            }
        }

        private void OnSelectionChange()
        {
            if (this.m_TableTabs != null)
            {
                for (int i = 0; i < this.m_TableTabs.Count; i++)
                {
                    this.m_TableTabs[i].OnSelectionChange();
                }
            }
            base.Repaint();
        }

        private float toolbarPadding
        {
            get
            {
                if (this.m_ToolbarPadding == -1f)
                {
                    Vector2 vector = EditorStyles.iconButton.CalcSize(EditorGUI.GUIContents.helpIcon);
                    this.m_ToolbarPadding = (vector.x * 2f) + 6f;
                }
                return this.m_ToolbarPadding;
            }
        }

        private static class Styles
        {
            public static readonly GUIContent[] TabTypes = new GUIContent[] { EditorGUIUtility.TextContent("Lights"), EditorGUIUtility.TextContent("Reflection Probes"), EditorGUIUtility.TextContent("Light Probes"), EditorGUIUtility.TextContent("Static Emissives") };
        }

        private enum TabType
        {
            Lights,
            Reflections,
            LightProbes,
            Emissives,
            Count
        }
    }
}

