namespace UnityEngine.U2D.Interface
{
    using System;
    using UnityEngine;

    internal abstract class ITexture2D
    {
        protected ITexture2D()
        {
        }

        public abstract void Apply();
        public override bool Equals(object other)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public abstract Color32[] GetPixels32();
        public static bool operator ==(ITexture2D t1, ITexture2D t2)
        {
            if (object.ReferenceEquals(t1, null))
            {
                return (object.ReferenceEquals(t2, null) || (t2 == null));
            }
            return t1.Equals(t2);
        }

        public static implicit operator UnityEngine.Object(ITexture2D t) => 
            (!object.ReferenceEquals(t, null) ? t.ToUnityObject() : null);

        public static implicit operator UnityEngine.Texture2D(ITexture2D t) => 
            (!object.ReferenceEquals(t, null) ? t.ToUnityTexture() : null);

        public static bool operator !=(ITexture2D t1, ITexture2D t2)
        {
            if (object.ReferenceEquals(t1, null))
            {
                return (!object.ReferenceEquals(t2, null) && (t2 != null));
            }
            return !t1.Equals(t2);
        }

        public abstract void SetPixels(Color[] c);
        protected abstract UnityEngine.Object ToUnityObject();
        protected abstract UnityEngine.Texture2D ToUnityTexture();

        public abstract FilterMode filterMode { get; set; }

        public abstract TextureFormat format { get; }

        public abstract int height { get; }

        public abstract float mipMapBias { get; }

        public abstract string name { get; }

        public abstract int width { get; }
    }
}

