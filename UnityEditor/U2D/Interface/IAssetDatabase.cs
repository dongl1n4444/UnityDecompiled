namespace UnityEditor.U2D.Interface
{
    using System;
    using UnityEngine;

    internal interface IAssetDatabase
    {
        ITextureImporter GetAssetImporterFromPath(string path);
        string GetAssetPath(Object o);
    }
}

