namespace UnityEditorInternal
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEngine;

    internal class ProfilerDetailedObjectsView
    {
        private ProfilerProperty m_CachedProfilerProperty;
        private CachedProfilerPropertyConfig m_CachedProfilerPropertyConfig;
        private ProfilerHierarchyGUI m_MainProfilerHierarchyGUI;
        private ProfilerHierarchyGUI m_ProfilerHierarchyGUI;

        public ProfilerDetailedObjectsView(ProfilerHierarchyGUI profilerHierarchyGUI, ProfilerHierarchyGUI mainProfilerHierarchyGUI)
        {
            this.m_ProfilerHierarchyGUI = profilerHierarchyGUI;
            this.m_MainProfilerHierarchyGUI = mainProfilerHierarchyGUI;
        }

        public void ClearCache()
        {
            if (this.m_CachedProfilerProperty != null)
            {
                this.m_CachedProfilerProperty.Cleanup();
                this.m_CachedProfilerProperty = null;
            }
            this.m_CachedProfilerPropertyConfig.frameIndex = -1;
        }

        public void DoGUI(GUIStyle headerStyle, int frameIndex, ProfilerViewType viewType)
        {
            ProfilerProperty detailedProperty = this.GetDetailedProperty(frameIndex, viewType);
            if (detailedProperty != null)
            {
                this.m_ProfilerHierarchyGUI.DoGUI(detailedProperty, string.Empty, false);
            }
            else
            {
                this.DrawEmptyPane(headerStyle);
            }
        }

        private void DrawEmptyPane(GUIStyle headerStyle)
        {
            GUILayout.Box(string.Empty, headerStyle, new GUILayoutOption[0]);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            GUILayout.Label("Select Line for per-object breakdown", EditorStyles.wordWrappedLabel, new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private ProfilerProperty GetDetailedProperty(int frameIndex, ProfilerViewType viewType)
        {
            if (((this.m_CachedProfilerPropertyConfig.frameIndex == frameIndex) && (this.m_CachedProfilerPropertyConfig.sortType == this.m_ProfilerHierarchyGUI.sortType)) && ((this.m_CachedProfilerPropertyConfig.viewType == viewType) && (this.m_CachedProfilerPropertyConfig.propertyPath == ProfilerDriver.selectedPropertyPath)))
            {
                return this.m_CachedProfilerProperty;
            }
            string selectedPropertyPath = ProfilerDriver.selectedPropertyPath;
            ProfilerProperty detailedProperty = this.m_MainProfilerHierarchyGUI.GetDetailedProperty();
            if (this.m_CachedProfilerProperty != null)
            {
                this.m_CachedProfilerProperty.Cleanup();
            }
            this.m_CachedProfilerPropertyConfig.frameIndex = frameIndex;
            this.m_CachedProfilerPropertyConfig.sortType = this.m_ProfilerHierarchyGUI.sortType;
            this.m_CachedProfilerPropertyConfig.viewType = viewType;
            this.m_CachedProfilerPropertyConfig.propertyPath = selectedPropertyPath;
            this.m_CachedProfilerProperty = detailedProperty;
            return detailedProperty;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct CachedProfilerPropertyConfig
        {
            public string propertyPath;
            public int frameIndex;
            public ProfilerColumn sortType;
            public ProfilerViewType viewType;
        }
    }
}

