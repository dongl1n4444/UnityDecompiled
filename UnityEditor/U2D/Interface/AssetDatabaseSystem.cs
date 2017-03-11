namespace UnityEditor.U2D.Interface
{
    using System;
    using UnityEditor;
    using UnityEngine;

    internal class AssetDatabaseSystem : IAssetDatabase
    {
        public ITextureImporter GetAssetImporterFromPath(string path)
        {
            AssetImporter atPath = AssetImporter.GetAtPath(path) as UnityEditor.TextureImporter;
            return ((atPath != null) ? new UnityEditor.U2D.Interface.TextureImporter((UnityEditor.TextureImporter) atPath) : null);
        }

        public string GetAssetPath(UnityEngine.Object o) => 
            AssetDatabase.GetAssetPath(o);
    }
}

