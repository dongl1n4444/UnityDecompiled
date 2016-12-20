namespace UnityEditor
{
    using System;

    internal interface IGameViewSizeMenuUser
    {
        void SizeSelectionCallback(int indexClicked, object objectSelected);

        bool forceLowResolutionAspectRatios { get; }

        bool lowResolutionForAspectRatios { get; set; }

        bool showLowResolutionToggle { get; }
    }
}

