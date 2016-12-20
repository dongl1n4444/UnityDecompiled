namespace UnityEditor
{
    using System;

    /// <summary>
    /// <para>Material generation options for ModelImporter.</para>
    /// </summary>
    [Obsolete("Use ModelImporterMaterialName, ModelImporter.materialName and ModelImporter.importMaterials instead")]
    public enum ModelImporterGenerateMaterials
    {
        /// <summary>
        /// <para>Do not generate materials.</para>
        /// </summary>
        [Obsolete("Use ModelImporter.importMaterials=false instead")]
        None = 0,
        /// <summary>
        /// <para>Generate a material for each material in the source asset.</para>
        /// </summary>
        [Obsolete("Use ModelImporter.importMaterials=true and ModelImporter.materialName=ModelImporterMaterialName.BasedOnModelNameAndMaterialName instead")]
        PerSourceMaterial = 2,
        /// <summary>
        /// <para>Generate a material for each texture used.</para>
        /// </summary>
        [Obsolete("Use ModelImporter.importMaterials=true and ModelImporter.materialName=ModelImporterMaterialName.BasedOnTextureName instead")]
        PerTexture = 1
    }
}

