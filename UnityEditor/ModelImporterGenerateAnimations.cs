namespace UnityEditor
{
    using System;

    /// <summary>
    /// <para>Animation generation options for ModelImporter. These options relate to the legacy Animation system, they should only be used when ModelImporter.animationType==ModelImporterAnimationType.Legacy.</para>
    /// </summary>
    public enum ModelImporterGenerateAnimations
    {
        None,
        InOriginalRoots,
        InNodes,
        InRoot,
        GenerateAnimations
    }
}

