namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
        private static Func<MeshRenderer, bool> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<MeshRenderer, IEnumerable<Material>> <>f__am$cache4;
        [CompilerGenerated]
        private static Func<Material, bool> <>f__am$cache5;
        [CompilerGenerated]
        private static Func<Material, int> <>f__am$cache6;
        [CompilerGenerated]
        private static SerializedPropertyDataStore.GatherDelegate <>f__am$cache7;
        [CompilerGenerated]
        private static Func<MeshRenderer, bool> <>f__am$cache8;
        [CompilerGenerated]
        private static Func<MeshRenderer, IEnumerable<Material>> <>f__am$cache9;
        [CompilerGenerated]
        private static Func<Material, bool> <>f__am$cacheA;
        [CompilerGenerated]
        private static SerializedPropertyTable.HeaderDelegate <>f__mg$cache0;
        [CompilerGenerated]
        private static SerializedPropertyTable.HeaderDelegate <>f__mg$cache1;
        [CompilerGenerated]
        private static SerializedPropertyTable.HeaderDelegate <>f__mg$cache2;
        [CompilerGenerated]
        private static SerializedPropertyTable.HeaderDelegate <>f__mg$cache3;
        private TabType m_SelectedTab = TabType.Lights;
        private List<LightingExplorerWindowTab> m_TableTabs;
        private float m_ToolbarPadding = -1f;

        [UnityEditor.MenuItem("Window/Lighting/Light Explorer", false, 0x833)]
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
                    <>f__am$cache0 = () => UnityEngine.Object.FindObjectsOfType<Light>();
                }
                if (<>f__mg$cache0 == null)
                {
                    <>f__mg$cache0 = new SerializedPropertyTable.HeaderDelegate(LightTableColumns.CreateLightColumns);
                }
                list.Add(new LightingExplorerWindowTab(new SerializedPropertyTable("LightTable", <>f__am$cache0, <>f__mg$cache0)));
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = () => UnityEngine.Object.FindObjectsOfType<UnityEngine.ReflectionProbe>();
                }
                if (<>f__mg$cache1 == null)
                {
                    <>f__mg$cache1 = new SerializedPropertyTable.HeaderDelegate(LightTableColumns.CreateReflectionColumns);
                }
                list.Add(new LightingExplorerWindowTab(new SerializedPropertyTable("ReflectionTable", <>f__am$cache1, <>f__mg$cache1)));
                if (<>f__am$cache2 == null)
                {
                    <>f__am$cache2 = () => UnityEngine.Object.FindObjectsOfType<LightProbeGroup>();
                }
                if (<>f__mg$cache2 == null)
                {
                    <>f__mg$cache2 = new SerializedPropertyTable.HeaderDelegate(LightTableColumns.CreateLightProbeColumns);
                }
                list.Add(new LightingExplorerWindowTab(new SerializedPropertyTable("LightProbeTable", <>f__am$cache2, <>f__mg$cache2)));
                if (<>f__mg$cache3 == null)
                {
                    <>f__mg$cache3 = new SerializedPropertyTable.HeaderDelegate(LightTableColumns.CreateEmissivesColumns);
                }
                list.Add(new LightingExplorerWindowTab(new SerializedPropertyTable("EmissiveMaterialTable", this.StaticEmissivesGatherDelegate(), <>f__mg$cache3)));
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
                    if (i == (this.m_TableTabs.Count - 1))
                    {
                        if (<>f__am$cache3 == null)
                        {
                            <>f__am$cache3 = mr => Selection.instanceIDs.Contains<int>(mr.gameObject.GetInstanceID());
                        }
                        if (<>f__am$cache4 == null)
                        {
                            <>f__am$cache4 = meshRenderer => meshRenderer.sharedMaterials;
                        }
                        if (<>f__am$cache5 == null)
                        {
                            <>f__am$cache5 = m => (m != null) && ((m.globalIlluminationFlags & MaterialGlobalIlluminationFlags.AnyEmissive) != MaterialGlobalIlluminationFlags.None);
                        }
                        if (<>f__am$cache6 == null)
                        {
                            <>f__am$cache6 = m => m.GetInstanceID();
                        }
                        int[] instanceIDs = Enumerable.Select<Material, int>(Enumerable.Where<Material>(Enumerable.SelectMany<MeshRenderer, Material>(Enumerable.Where<MeshRenderer>(UnityEngine.Object.FindObjectsOfType<MeshRenderer>(), <>f__am$cache3), <>f__am$cache4), <>f__am$cache5), <>f__am$cache6).Union<int>(Selection.instanceIDs).Distinct<int>().ToArray<int>();
                        this.m_TableTabs[i].OnSelectionChange(instanceIDs);
                    }
                    else
                    {
                        this.m_TableTabs[i].OnSelectionChange();
                    }
                }
            }
            base.Repaint();
        }

        private SerializedPropertyDataStore.GatherDelegate StaticEmissivesGatherDelegate()
        {
            if (<>f__am$cache7 == null)
            {
                <>f__am$cache7 = delegate {
                    if (<>f__am$cache8 == null)
                    {
                        <>f__am$cache8 = mr => GameObjectUtility.AreStaticEditorFlagsSet(mr.gameObject, StaticEditorFlags.LightmapStatic);
                    }
                    if (<>f__am$cache9 == null)
                    {
                        <>f__am$cache9 = meshRenderer => meshRenderer.sharedMaterials;
                    }
                    if (<>f__am$cacheA == null)
                    {
                        <>f__am$cacheA = m => ((m != null) && ((m.globalIlluminationFlags & MaterialGlobalIlluminationFlags.AnyEmissive) != MaterialGlobalIlluminationFlags.None)) && m.HasProperty("_EmissionColor");
                    }
                    return Enumerable.Where<Material>(Enumerable.SelectMany<MeshRenderer, Material>(Enumerable.Where<MeshRenderer>(UnityEngine.Object.FindObjectsOfType<MeshRenderer>(), <>f__am$cache8), <>f__am$cache9), <>f__am$cacheA).Distinct<Material>().ToArray<Material>();
                };
            }
            return <>f__am$cache7;
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

