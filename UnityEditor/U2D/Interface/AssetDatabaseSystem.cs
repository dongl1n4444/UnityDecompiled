namespace UnityEditor.U2D.Interface
{
    using System;
    using UnityEditor;
    using UnityEngine;

    internal class AssetDatabaseSystem : IAssetDatabase
    {
        public ITextureImporter GetAssetImporterFromPath(string path)
        {
            AssetImporter atPath = AssetImporter.GetAtPath(path);
            if (!(atPath is TextureImporter))
            {
                throw new NotSupportedException("Current implementation only supports TextureImporter.");
            }
            return new TextureImporter((TextureImporter) atPath);
        }

        public string GetAssetPath(Object o) => 
            AssetDatabase.GetAssetPath(o);
    }
}

