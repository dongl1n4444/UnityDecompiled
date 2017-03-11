namespace UnityEditor.Collaboration
{
    using System;
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    internal static class TextureUtility
    {
        public static Texture2D LoadTextureFromApplicationContents(string path)
        {
            Texture2D tex = new Texture2D(2, 2);
            path = Path.Combine(Path.Combine(Path.Combine(Path.Combine(EditorApplication.applicationContentsPath, "Resources"), "Collab"), "overlays"), path);
            try
            {
                FileStream stream = File.OpenRead(path);
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, (int) stream.Length);
                if (!tex.LoadImage(buffer))
                {
                    return null;
                }
            }
            catch (Exception)
            {
                Debug.LogWarning("Collab Overlay Texture load fail, path: " + path);
                return null;
            }
            return tex;
        }
    }
}

