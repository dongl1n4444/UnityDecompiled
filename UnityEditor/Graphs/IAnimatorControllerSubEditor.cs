namespace UnityEditor.Graphs
{
    using System;
    using UnityEditor;
    using UnityEngine;

    internal interface IAnimatorControllerSubEditor
    {
        void GrabKeyboardFocus();
        bool HasKeyboardControl();
        void Init(IAnimatorControllerEditor host);
        void OnDestroy();
        void OnDisable();
        void OnEnable();
        void OnEvent();
        void OnFocus();
        void OnGUI(Rect rect);
        void OnLostFocus();
        void OnToolbarGUI();
        void ReleaseKeyboardFocus();
        void ResetUI();

        RenameOverlay renameOverlay { get; }
    }
}

