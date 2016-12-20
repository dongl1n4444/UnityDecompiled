namespace UnityEditor
{
    using System;

    /// <summary>
    /// <para>Options for removing assets</para>
    /// </summary>
    public enum RemoveAssetOptions
    {
        /// <summary>
        /// <para>Delete the asset without moving it to the trash.</para>
        /// </summary>
        DeleteAssets = 2,
        /// <summary>
        /// <para>The asset should be moved to trash.</para>
        /// </summary>
        MoveAssetToTrash = 0
    }
}

