namespace UnityEngine.Rendering
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    /// <summary>
    /// <para>Identifies a RenderTexture for a Rendering.CommandBuffer.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RenderTargetIdentifier
    {
        private BuiltinRenderTextureType m_Type;
        private int m_NameID;
        private int m_InstanceID;
        /// <summary>
        /// <para>Creates a render target identifier.</para>
        /// </summary>
        /// <param name="type">Built-in temporary render texture type.</param>
        /// <param name="name">Temporary render texture name.</param>
        /// <param name="nameID">Temporary render texture name (as integer, see Shader.PropertyToID).</param>
        /// <param name="tex">RenderTexture or Texture object to use.</param>
        public RenderTargetIdentifier(BuiltinRenderTextureType type)
        {
            this.m_Type = type;
            this.m_NameID = -1;
            this.m_InstanceID = 0;
        }

        /// <summary>
        /// <para>Creates a render target identifier.</para>
        /// </summary>
        /// <param name="type">Built-in temporary render texture type.</param>
        /// <param name="name">Temporary render texture name.</param>
        /// <param name="nameID">Temporary render texture name (as integer, see Shader.PropertyToID).</param>
        /// <param name="tex">RenderTexture or Texture object to use.</param>
        public RenderTargetIdentifier(string name)
        {
            this.m_Type = BuiltinRenderTextureType.None;
            this.m_NameID = Shader.PropertyToID(name);
            this.m_InstanceID = 0;
        }

        /// <summary>
        /// <para>Creates a render target identifier.</para>
        /// </summary>
        /// <param name="type">Built-in temporary render texture type.</param>
        /// <param name="name">Temporary render texture name.</param>
        /// <param name="nameID">Temporary render texture name (as integer, see Shader.PropertyToID).</param>
        /// <param name="tex">RenderTexture or Texture object to use.</param>
        public RenderTargetIdentifier(int nameID)
        {
            this.m_Type = BuiltinRenderTextureType.None;
            this.m_NameID = nameID;
            this.m_InstanceID = 0;
        }

        /// <summary>
        /// <para>Creates a render target identifier.</para>
        /// </summary>
        /// <param name="type">Built-in temporary render texture type.</param>
        /// <param name="name">Temporary render texture name.</param>
        /// <param name="nameID">Temporary render texture name (as integer, see Shader.PropertyToID).</param>
        /// <param name="tex">RenderTexture or Texture object to use.</param>
        public RenderTargetIdentifier(Texture tex)
        {
            this.m_Type = ((tex != null) && !(tex is RenderTexture)) ? BuiltinRenderTextureType.BindableTexture : BuiltinRenderTextureType.None;
            this.m_NameID = -1;
            this.m_InstanceID = (tex == null) ? 0 : tex.GetInstanceID();
        }

        public static implicit operator RenderTargetIdentifier(BuiltinRenderTextureType type) => 
            new RenderTargetIdentifier(type);

        public static implicit operator RenderTargetIdentifier(string name) => 
            new RenderTargetIdentifier(name);

        public static implicit operator RenderTargetIdentifier(int nameID) => 
            new RenderTargetIdentifier(nameID);

        public static implicit operator RenderTargetIdentifier(Texture tex) => 
            new RenderTargetIdentifier(tex);
    }
}

