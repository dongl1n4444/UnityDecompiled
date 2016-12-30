namespace UnityEditor.U2D.Interface
{
    using System;
    using UnityEditor;
    using UnityEngine;

    internal abstract class ITextureImporter
    {
        protected ITextureImporter()
        {
        }

        public override bool Equals(object other)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public abstract void GetWidthAndHeight(ref int width, ref int height);
        public static bool operator ==(ITextureImporter t1, ITextureImporter t2)
        {
            if (object.ReferenceEquals(t1, null))
            {
                return (object.ReferenceEquals(t2, null) || (t2 == null));
            }
            return t1.Equals(t2);
        }

        public static bool operator !=(ITextureImporter t1, ITextureImporter t2)
        {
            if (object.ReferenceEquals(t1, null))
            {
                return (!object.ReferenceEquals(t2, null) && (t2 != null));
            }
            return !t1.Equals(t2);
        }

        public abstract string assetPath { get; }

        public abstract Vector4 spriteBorder { get; }

        public abstract SpriteImportMode spriteImportMode { get; }

        public abstract Vector2 spritePivot { get; }
    }
}

