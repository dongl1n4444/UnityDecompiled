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

        public override string ToString()
        {
            object[] args = new object[] { this.m_Type, this.m_NameID, this.m_InstanceID };
            return UnityString.Format("Type {0} NameID {1} InstanceID {2}", args);
        }

        public override int GetHashCode() => 
            ((((this.m_Type.GetHashCode() * 0x17) + this.m_NameID.GetHashCode()) * 0x17) + this.m_InstanceID.GetHashCode());

        public override bool Equals(object obj)
        {
            if (!(obj is RenderTargetIdentifier))
            {
                return false;
            }
            RenderTargetIdentifier identifier = (RenderTargetIdentifier) obj;
            return (((this.m_Type == identifier.m_Type) && (this.m_NameID == identifier.m_NameID)) && (this.m_InstanceID == identifier.m_InstanceID));
        }

        public bool Equals(RenderTargetIdentifier rhs) => 
            (((this.m_Type == rhs.m_Type) && (this.m_NameID == rhs.m_NameID)) && (this.m_InstanceID == rhs.m_InstanceID));

        public static bool operator ==(RenderTargetIdentifier lhs, RenderTargetIdentifier rhs) => 
            (((lhs.m_Type == rhs.m_Type) && (lhs.m_NameID == rhs.m_NameID)) && (lhs.m_InstanceID == rhs.m_InstanceID));

        public static bool operator !=(RenderTargetIdentifier lhs, RenderTargetIdentifier rhs) => 
            (((lhs.m_Type != rhs.m_Type) || (lhs.m_NameID != rhs.m_NameID)) || (lhs.m_InstanceID != rhs.m_InstanceID));
    }
}

