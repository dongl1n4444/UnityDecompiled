namespace UnityEditor
{
    using System;

    /// <summary>
    /// <para>Tangent space generation options for ModelImporter.</para>
    /// </summary>
    public enum ModelImporterTangentSpaceMode
    {
        /// <summary>
        /// <para>Calculate tangents.</para>
        /// </summary>
        [Obsolete("Use ModelImporterNormals.Calculate instead")]
        Calculate = 1,
        /// <summary>
        /// <para>Import normals/tangents from file.</para>
        /// </summary>
        [Obsolete("Use ModelImporterNormals.Import instead")]
        Import = 0,
        /// <summary>
        /// <para>Strip normals/tangents.</para>
        /// </summary>
        [Obsolete("Use ModelImporterNormals.None instead")]
        None = 2
    }
}

