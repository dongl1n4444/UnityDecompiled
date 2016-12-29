namespace UnityEditor.WebGL
{
    using System;
    using System.IO;
    using UnityEditor;
    using UnityEditor.WebGL.Emscripten;
    using UnityEngine;

    internal class WebGLTemplateManager : WebTemplateManagerBase
    {
        private const string kWebTemplateDefaultIconResource = "BuildSettings.WebGL.Small";

        public override string builtinTemplatesFolder =>
            Path.Combine(EmscriptenPaths.buildToolsDir, "WebGLTemplates");

        public override string customTemplatesFolder =>
            Path.Combine(Application.dataPath, "WebGLTemplates");

        public override Texture2D defaultIcon =>
            ((Texture2D) EditorGUIUtility.IconContent("BuildSettings.WebGL.Small").image);
    }
}

