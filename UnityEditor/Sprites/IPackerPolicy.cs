namespace UnityEditor.Sprites
{
    using System;
    using UnityEditor;

    public interface IPackerPolicy
    {
        /// <summary>
        /// <para>Return the version of your policy. Sprite Packer needs to know if atlas grouping logic changed.</para>
        /// </summary>
        int GetVersion();
        /// <summary>
        /// <para>Implement custom atlas grouping here.</para>
        /// </summary>
        /// <param name="target"></param>
        /// <param name="job"></param>
        /// <param name="textureImporterInstanceIDs"></param>
        void OnGroupAtlases(BuildTarget target, PackerJob job, int[] textureImporterInstanceIDs);
    }
}

