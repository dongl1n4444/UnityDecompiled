namespace UnityEditor
{
    using System;

    /// <summary>
    /// <para>Texture importer lets you modify Texture2D import settings for DDS textures from editor scripts.</para>
    /// </summary>
    [Obsolete("DDSImporter is obsolete. Use IHVImageFormatImporter instead (UnityUpgradable) -> IHVImageFormatImporter", true)]
    public class DDSImporter : AssetImporter
    {
        /// <summary>
        /// <para>Is texture data readable from scripts.</para>
        /// </summary>
        public bool isReadable
        {
            get => 
                false;
            set
            {
            }
        }
    }
}

