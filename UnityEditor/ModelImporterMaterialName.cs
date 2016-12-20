namespace UnityEditor
{
    using System;

    /// <summary>
    /// <para>Material naming options for ModelImporter.</para>
    /// </summary>
    public enum ModelImporterMaterialName
    {
        /// <summary>
        /// <para>Use a material name of the form &lt;materialName&gt;.mat.</para>
        /// </summary>
        BasedOnMaterialName = 1,
        /// <summary>
        /// <para>Use material names in the form &lt;modelFileName&gt;-&lt;materialName&gt;.mat.</para>
        /// </summary>
        BasedOnModelNameAndMaterialName = 2,
        /// <summary>
        /// <para>Use material names in the form &lt;textureName&gt;.mat.</para>
        /// </summary>
        BasedOnTextureName = 0,
        /// <summary>
        /// <para>&lt;textureName&gt;.mat or &lt;modelFileName&gt;-&lt;materialName&gt;.mat material name.</para>
        /// </summary>
        [Obsolete("You should use ModelImporterMaterialName.BasedOnTextureName instead, because it it less complicated and behaves in more consistent way.")]
        BasedOnTextureName_Or_ModelNameAndMaterialName = 3
    }
}

