namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal interface ISpriteEditorModule
    {
        bool CanBeActivated();
        void DoTextureGUI();
        void DrawToolbarGUI(Rect drawArea);
        void OnModuleActivate();
        void OnModuleDeactivate();
        void OnPostGUI();

        string moduleName { get; }
    }
}

