namespace UnityEditor
{
    using System;

    /// <summary>
    /// <para>Vertex tangent generation options for ModelImporter.</para>
    /// </summary>
    public enum ModelImporterTangents
    {
        Import,
        CalculateLegacy,
        None,
        CalculateMikk,
        CalculateLegacyWithSplitTangents
    }
}

