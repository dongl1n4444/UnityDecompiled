namespace UnityEngine.U2D.Interface
{
    using System;
    using UnityEngine;

    internal class Texture2D : ITexture2D
    {
        private UnityEngine.Texture2D m_Texture;

        public Texture2D(UnityEngine.Texture2D texture)
        {
            this.m_Texture = texture;
        }

        public override void Apply()
        {
            this.m_Texture.Apply();
        }

        public override bool Equals(object other)
        {
            UnityEngine.U2D.Interface.Texture2D objA = other as UnityEngine.U2D.Interface.Texture2D;
            if (object.ReferenceEquals(objA, null))
            {
                return (this.m_Texture == null);
            }
            return (this.m_Texture == objA.m_Texture);
        }

        public override int GetHashCode() => 
            this.m_Texture.GetHashCode();

        public override Color32[] GetPixels32() => 
            this.m_Texture.GetPixels32();

        public override void SetPixels(Color[] c)
        {
            this.m_Texture.SetPixels(c);
        }

        protected override UnityEngine.Object ToUnityObject() => 
            this.m_Texture;

        protected override UnityEngine.Texture2D ToUnityTexture() => 
            this.m_Texture;

        public override FilterMode filterMode
        {
            get => 
                this.m_Texture.filterMode;
            set
            {
                this.m_Texture.filterMode = value;
            }
        }

        public override TextureFormat format =>
            this.m_Texture.format;

        public override int height =>
            this.m_Texture.height;

        public override float mipMapBias =>
            this.m_Texture.mipMapBias;

        public override string name =>
            this.m_Texture.name;

        public override int width =>
            this.m_Texture.width;
    }
}

