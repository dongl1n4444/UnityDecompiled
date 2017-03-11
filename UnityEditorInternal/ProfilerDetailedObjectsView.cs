namespace UnityEditorInternal
{
    using System;
    using UnityEngine;

    internal class ProfilerDetailedObjectsView : ProfilerDetailedView
    {
        private ProfilerHierarchyGUI m_ProfilerHierarchyGUI;

        public ProfilerDetailedObjectsView(ProfilerHierarchyGUI profilerHierarchyGUI, ProfilerHierarchyGUI mainProfilerHierarchyGUI) : base(mainProfilerHierarchyGUI)
        {
            this.m_ProfilerHierarchyGUI = profilerHierarchyGUI;
        }

        public void DoGUI(GUIStyle headerStyle, int frameIndex, ProfilerViewType viewType)
        {
            ProfilerProperty property = this.GetDetailedProperty(frameIndex, viewType, this.m_ProfilerHierarchyGUI.sortType);
            if (property != null)
            {
                this.m_ProfilerHierarchyGUI.DoGUI(property, string.Empty, false);
            }
            else
            {
                base.DrawEmptyPane(headerStyle);
            }
        }

        private ProfilerProperty GetDetailedProperty(int frameIndex, ProfilerViewType viewType, ProfilerColumn sortType)
        {
            if (this.m_CachedProfilerPropertyConfig.EqualsTo(frameIndex, viewType, sortType))
            {
                return base.m_CachedProfilerProperty;
            }
            ProfilerProperty detailedProperty = base.m_MainProfilerHierarchyGUI.GetDetailedProperty();
            if (base.m_CachedProfilerProperty != null)
            {
                base.m_CachedProfilerProperty.Cleanup();
            }
            this.m_CachedProfilerPropertyConfig.Set(frameIndex, viewType, sortType);
            base.m_CachedProfilerProperty = detailedProperty;
            return detailedProperty;
        }
    }
}

