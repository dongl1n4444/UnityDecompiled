namespace UnityEditor.U2D.Interface
{
    using System;
    using UnityEditor;
    using UnityEngine;

    internal class TextureImporter : ITextureImporter
    {
        protected AssetImporter m_AssetImporter;

        public TextureImporter(UnityEditor.TextureImporter textureImporter)
        {
            this.m_AssetImporter = textureImporter;
        }

        public override bool Equals(object other)
        {
            UnityEditor.U2D.Interface.TextureImporter objA = other as UnityEditor.U2D.Interface.TextureImporter;
            if (object.ReferenceEquals(objA, null))
            {
                return (this.m_AssetImporter == null);
            }
            return (this.m_AssetImporter == objA.m_AssetImporter);
        }

        public override int GetHashCode() => 
            this.m_AssetImporter.GetHashCode();

        public override void GetWidthAndHeight(ref int width, ref int height)
        {
            ((UnityEditor.TextureImporter) this.m_AssetImporter).GetWidthAndHeight(ref width, ref height);
        }

        public override string assetPath =>
            this.m_AssetImporter.assetPath;

        public override Vector4 spriteBorder =>
            ((UnityEditor.TextureImporter) this.m_AssetImporter).spriteBorder;

        public override SpriteImportMode spriteImportMode =>
            ((UnityEditor.TextureImporter) this.m_AssetImporter).spriteImportMode;

        public override Vector2 spritePivot =>
            ((UnityEditor.TextureImporter) this.m_AssetImporter).spritePivot;
    }
}

