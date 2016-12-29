namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    /// <summary>
    /// <para>Stores platform specifics settings of a TextureImporter.</para>
    /// </summary>
    [Serializable]
    public sealed class TextureImporterPlatformSettings
    {
        [SerializeField]
        private int m_AllowsAlphaSplitting = 0;
        [SerializeField]
        private int m_CompressionQuality = 50;
        [SerializeField]
        private int m_CrunchedCompression = 0;
        [SerializeField]
        private int m_MaxTextureSize = 0x800;
        [SerializeField]
        private string m_Name = TextureImporterInspector.s_DefaultPlatformName;
        [SerializeField]
        private int m_Overridden = 0;
        [SerializeField]
        private int m_TextureCompression = 1;
        [SerializeField]
        private int m_TextureFormat = -1;

        /// <summary>
        /// <para>Copy parameters into another TextureImporterPlatformSettings object.</para>
        /// </summary>
        /// <param name="target">TextureImporterPlatformSettings object to copy settings to.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void CopyTo(TextureImporterPlatformSettings target);

        /// <summary>
        /// <para>Allows Alpha splitting on the imported texture when needed (for example ETC1 compression for textures with transparency).</para>
        /// </summary>
        public bool allowsAlphaSplitting
        {
            get => 
                (this.m_AllowsAlphaSplitting != 0);
            set
            {
                this.m_AllowsAlphaSplitting = !value ? 0 : 1;
            }
        }

        /// <summary>
        /// <para>Quality of texture compression in the range [0..100].</para>
        /// </summary>
        public int compressionQuality
        {
            get => 
                this.m_CompressionQuality;
            set
            {
                this.m_CompressionQuality = value;
            }
        }

        /// <summary>
        /// <para>Use crunch compression when available.</para>
        /// </summary>
        public bool crunchedCompression
        {
            get => 
                (this.m_CrunchedCompression != 0);
            set
            {
                this.m_CrunchedCompression = !value ? 0 : 1;
            }
        }

        /// <summary>
        /// <para>Format of imported texture.</para>
        /// </summary>
        public TextureImporterFormat format
        {
            get => 
                ((TextureImporterFormat) this.m_TextureFormat);
            set
            {
                this.m_TextureFormat = (int) value;
            }
        }

        /// <summary>
        /// <para>Maximum texture size.</para>
        /// </summary>
        public int maxTextureSize
        {
            get => 
                this.m_MaxTextureSize;
            set
            {
                this.m_MaxTextureSize = value;
            }
        }

        /// <summary>
        /// <para>Name of the build target.</para>
        /// </summary>
        public string name
        {
            get => 
                this.m_Name;
            set
            {
                this.m_Name = value;
            }
        }

        /// <summary>
        /// <para>Set to true in order to override the Default platform parameters by those provided in the TextureImporterPlatformSettings structure.</para>
        /// </summary>
        public bool overridden
        {
            get => 
                (this.m_Overridden != 0);
            set
            {
                this.m_Overridden = !value ? 0 : 1;
            }
        }

        /// <summary>
        /// <para>Compression of imported texture.</para>
        /// </summary>
        public TextureImporterCompression textureCompression
        {
            get => 
                ((TextureImporterCompression) this.m_TextureCompression);
            set
            {
                this.m_TextureCompression = (int) value;
            }
        }
    }
}

