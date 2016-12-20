namespace UnityEditor
{
    using System;

    /// <summary>
    /// <para>SelectionMode can be used to tweak the selection returned by Selection.GetTransforms.</para>
    /// </summary>
    public enum SelectionMode
    {
        /// <summary>
        /// <para>Only return objects that are assets in the Asset directory.</para>
        /// </summary>
        Assets = 0x10,
        /// <summary>
        /// <para>Return the selection and all child transforms of the selection.</para>
        /// </summary>
        Deep = 2,
        /// <summary>
        /// <para>If the selection contains folders, also include all assets and subfolders within that folder in the file hierarchy.</para>
        /// </summary>
        DeepAssets = 0x20,
        /// <summary>
        /// <para>Excludes any objects which shall not be modified.</para>
        /// </summary>
        Editable = 8,
        /// <summary>
        /// <para>Excludes any prefabs from the selection.</para>
        /// </summary>
        ExcludePrefab = 4,
        OnlyUserModifiable = 8,
        /// <summary>
        /// <para>Only return the topmost selected transform. A selected child of another selected transform will be filtered out.</para>
        /// </summary>
        TopLevel = 1,
        /// <summary>
        /// <para>Return the whole selection.</para>
        /// </summary>
        Unfiltered = 0
    }
}

