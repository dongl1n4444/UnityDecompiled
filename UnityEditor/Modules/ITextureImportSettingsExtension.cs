namespace UnityEditor.Modules
{
    using System;
    using UnityEditor;

    internal interface ITextureImportSettingsExtension
    {
        void ShowImportSettings(Editor baseEditor, TextureImportPlatformSettings platformSettings);
    }
}

