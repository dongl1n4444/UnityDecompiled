namespace UnityEditorInternal
{
    using System;

    internal interface IProfilerWindowController
    {
        void ClearSelectedPropertyPath();
        int GetActiveVisibleFrameIndex();
        ProfilerProperty GetRootProfilerProperty(ProfilerColumn sortType);
        string GetSearch();
        bool IsSearching();
        void Repaint();
        void SetSearch(string searchString);
        void SetSelectedPropertyPath(string path);
    }
}

