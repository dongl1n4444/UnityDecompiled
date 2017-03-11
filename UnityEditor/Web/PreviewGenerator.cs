namespace UnityEditor.Web
{
    using System;
    using UnityEditor;
    using UnityEngine;

    internal class PreviewGenerator
    {
        private const string kPreviewBuildFolder = "builds";
        protected static PreviewGenerator s_Instance = null;

        public byte[] GeneratePreview(string assetPath, int width, int height)
        {
            UnityEngine.Object targetObject = AssetDatabase.LoadMainAssetAtPath(assetPath);
            if (targetObject == null)
            {
                return null;
            }
            Editor editor = Editor.CreateEditor(targetObject);
            if (editor == null)
            {
                return null;
            }
            Texture2D tex = editor.RenderStaticPreview(assetPath, null, width, height);
            if (tex == null)
            {
                UnityEngine.Object.DestroyImmediate(editor);
                return null;
            }
            byte[] buffer2 = tex.EncodeToPNG();
            UnityEngine.Object.DestroyImmediate(tex);
            UnityEngine.Object.DestroyImmediate(editor);
            return buffer2;
        }

        public static PreviewGenerator GetInstance()
        {
            if (s_Instance == null)
            {
                return new PreviewGenerator();
            }
            return s_Instance;
        }
    }
}

