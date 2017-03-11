namespace UnityEditorInternal
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEngine;

    internal abstract class ProfilerDetailedView
    {
        protected ProfilerProperty m_CachedProfilerProperty;
        protected CachedProfilerPropertyConfig m_CachedProfilerPropertyConfig;
        protected readonly ProfilerHierarchyGUI m_MainProfilerHierarchyGUI;

        protected ProfilerDetailedView(ProfilerHierarchyGUI mainProfilerHierarchyGUI)
        {
            this.m_MainProfilerHierarchyGUI = mainProfilerHierarchyGUI;
        }

        protected void DrawEmptyPane(GUIStyle headerStyle)
        {
            GUILayout.Box(Styles.emptyText, headerStyle, new GUILayoutOption[0]);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            GUILayout.Label(Styles.selectLineText, EditorStyles.wordWrappedLabel, new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        public void ResetCachedProfilerProperty()
        {
            if (this.m_CachedProfilerProperty != null)
            {
                this.m_CachedProfilerProperty.Cleanup();
                this.m_CachedProfilerProperty = null;
            }
            this.m_CachedProfilerPropertyConfig.frameIndex = -1;
        }

        [StructLayout(LayoutKind.Sequential)]
        protected struct CachedProfilerPropertyConfig
        {
            public string propertyPath;
            public int frameIndex;
            public ProfilerColumn sortType;
            public ProfilerViewType viewType;
            public bool EqualsTo(int frameIndex, ProfilerViewType viewType, ProfilerColumn sortType) => 
                ((((this.frameIndex == frameIndex) && (this.sortType == sortType)) && (this.viewType == viewType)) && (this.propertyPath == ProfilerDriver.selectedPropertyPath));

            public void Set(int frameIndex, ProfilerViewType viewType, ProfilerColumn sortType)
            {
                this.frameIndex = frameIndex;
                this.sortType = sortType;
                this.viewType = viewType;
                this.propertyPath = ProfilerDriver.selectedPropertyPath;
            }
        }

        private static class Styles
        {
            public static GUIContent emptyText = new GUIContent("");
            public static GUIContent selectLineText = new GUIContent("Select Line for the detailed information");
        }
    }
}

