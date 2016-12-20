namespace UnityEditor
{
    using System;

    internal interface IBaseInspectView
    {
        void DrawInstructionList();
        void DrawSelectedInstructionDetails();
        void SelectRow(int index);
        void ShowOverlay();
        void Unselect();
        void UpdateInstructions();
    }
}

