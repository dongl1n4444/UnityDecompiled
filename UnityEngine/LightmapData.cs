namespace UnityEngine
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Data of a lightmap.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), UsedByNativeCode]
    public sealed class LightmapData
    {
        internal Texture2D m_Light;
        internal Texture2D m_Dir;
        internal Texture2D m_ShadowMask;
        [Obsolete("Use lightmapColor property (UnityUpgradable) -> lightmapColor")]
        public Texture2D lightmapLight
        {
            get => 
                this.m_Light;
            set
            {
                this.m_Light = value;
            }
        }
        /// <summary>
        /// <para>Lightmap storing color of incoming light.</para>
        /// </summary>
        public Texture2D lightmapColor
        {
            get => 
                this.m_Light;
            set
            {
                this.m_Light = value;
            }
        }
        /// <summary>
        /// <para>Lightmap storing dominant direction of incoming light.</para>
        /// </summary>
        public Texture2D lightmapDir
        {
            get => 
                this.m_Dir;
            set
            {
                this.m_Dir = value;
            }
        }
        /// <summary>
        /// <para>Texture storing occlusion mask per light (ShadowMask, up to four lights).</para>
        /// </summary>
        public Texture2D shadowMask
        {
            get => 
                this.m_ShadowMask;
            set
            {
                this.m_ShadowMask = value;
            }
        }
        [Obsolete("Property LightmapData.lightmap has been deprecated. Use LightmapData.lightmapColor instead (UnityUpgradable) -> lightmapColor", true), EditorBrowsable(EditorBrowsableState.Never)]
        public Texture2D lightmap
        {
            get => 
                null;
            set
            {
            }
        }
        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Property LightmapData.lightmapFar has been deprecated. Use LightmapData.lightmapColor instead (UnityUpgradable) -> lightmapColor", true)]
        public Texture2D lightmapFar
        {
            get => 
                null;
            set
            {
            }
        }
        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Property LightmapData.lightmapNear has been deprecated. Use LightmapData.lightmapDir instead (UnityUpgradable) -> lightmapDir", true)]
        public Texture2D lightmapNear
        {
            get => 
                null;
            set
            {
            }
        }
    }
}

