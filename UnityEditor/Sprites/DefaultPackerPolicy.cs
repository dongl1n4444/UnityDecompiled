namespace UnityEditor.Sprites
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEngine;

    internal class DefaultPackerPolicy : IPackerPolicy
    {
        [CompilerGenerated]
        private static Func<Object, Sprite> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<Sprite, bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<Entry, string> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<Entry, Entry> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<Entry, AtlasSettings> <>f__am$cache4;
        [CompilerGenerated]
        private static Func<Entry, Entry> <>f__am$cache5;
        private const uint kDefaultPaddingPower = 3;

        private SpritePackingMode GetPackingMode(string packingTag, SpriteMeshType meshType)
        {
            if ((meshType == SpriteMeshType.Tight) && (this.IsTagPrefixed(packingTag) == this.AllowTightWhenTagged))
            {
                return SpritePackingMode.Tight;
            }
            return SpritePackingMode.Rectangle;
        }

        public virtual int GetVersion()
        {
            return 1;
        }

        protected bool HasPlatformEnabledAlphaSplittingForCompression(string targetName, TextureImporter ti)
        {
            TextureImporterPlatformSettings platformTextureSettings = ti.GetPlatformTextureSettings(targetName);
            return (platformTextureSettings.overridden && platformTextureSettings.allowsAlphaSplitting);
        }

        protected bool IsTagPrefixed(string packingTag)
        {
            packingTag = packingTag.Trim();
            if (packingTag.Length < this.TagPrefix.Length)
            {
                return false;
            }
            return (packingTag.Substring(0, this.TagPrefix.Length) == this.TagPrefix);
        }

        public void OnGroupAtlases(BuildTarget target, PackerJob job, int[] textureImporterInstanceIDs)
        {
            List<Entry> list = new List<Entry>();
            string targetName = "";
            if (target != BuildTarget.NoTarget)
            {
                targetName = BuildPipeline.GetBuildTargetName(target);
            }
            foreach (int num in textureImporterInstanceIDs)
            {
                TextureFormat format;
                ColorSpace space;
                int num3;
                TextureImporter ti = EditorUtility.InstanceIDToObject(num) as TextureImporter;
                ti.ReadTextureImportInstructions(target, out format, out space, out num3);
                TextureImporterSettings dest = new TextureImporterSettings();
                ti.ReadTextureSettings(dest);
                bool flag = (targetName != "") && this.HasPlatformEnabledAlphaSplittingForCompression(targetName, ti);
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = new Func<Object, Sprite>(null, (IntPtr) <OnGroupAtlases>m__0);
                }
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = new Func<Sprite, bool>(null, (IntPtr) <OnGroupAtlases>m__1);
                }
                Sprite[] spriteArray = Enumerable.ToArray<Sprite>(Enumerable.Where<Sprite>(Enumerable.Select<Object, Sprite>(AssetDatabase.LoadAllAssetRepresentationsAtPath(ti.assetPath), <>f__am$cache0), <>f__am$cache1));
                foreach (Sprite sprite in spriteArray)
                {
                    Entry item = new Entry {
                        sprite = sprite,
                        settings = { 
                            format = format,
                            colorSpace = space,
                            compressionQuality = !TextureUtil.IsCompressedTextureFormat(format) ? 0 : num3,
                            filterMode = !Enum.IsDefined(typeof(FilterMode), ti.filterMode) ? FilterMode.Bilinear : ti.filterMode,
                            maxWidth = 0x800,
                            maxHeight = 0x800,
                            generateMipMaps = ti.mipmapEnabled,
                            enableRotation = this.AllowRotationFlipping,
                            allowsAlphaSplitting = TextureImporter.IsTextureFormatETC1Compression(format) && flag
                        }
                    };
                    if (ti.mipmapEnabled)
                    {
                        item.settings.paddingPower = 3;
                    }
                    else
                    {
                        item.settings.paddingPower = (uint) EditorSettings.spritePackerPaddingPower;
                    }
                    item.atlasName = this.ParseAtlasName(ti.spritePackingTag);
                    item.packingMode = this.GetPackingMode(ti.spritePackingTag, dest.spriteMeshType);
                    item.anisoLevel = ti.anisoLevel;
                    list.Add(item);
                }
                Resources.UnloadAsset(ti);
            }
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = new Func<Entry, string>(null, (IntPtr) <OnGroupAtlases>m__2);
            }
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = new Func<Entry, Entry>(null, (IntPtr) <OnGroupAtlases>m__3);
            }
            IEnumerable<IGrouping<string, Entry>> enumerable = Enumerable.GroupBy<Entry, string, Entry>(list, <>f__am$cache2, <>f__am$cache3);
            foreach (IGrouping<string, Entry> grouping in enumerable)
            {
                int num5 = 0;
                if (<>f__am$cache4 == null)
                {
                    <>f__am$cache4 = new Func<Entry, AtlasSettings>(null, (IntPtr) <OnGroupAtlases>m__4);
                }
                if (<>f__am$cache5 == null)
                {
                    <>f__am$cache5 = new Func<Entry, Entry>(null, (IntPtr) <OnGroupAtlases>m__5);
                }
                IEnumerable<IGrouping<AtlasSettings, Entry>> source = Enumerable.GroupBy<Entry, AtlasSettings, Entry>(grouping, <>f__am$cache4, <>f__am$cache5);
                foreach (IGrouping<AtlasSettings, Entry> grouping2 in source)
                {
                    string key = grouping.Key;
                    if (Enumerable.Count<IGrouping<AtlasSettings, Entry>>(source) > 1)
                    {
                        key = key + string.Format(" (Group {0})", num5);
                    }
                    AtlasSettings settings = grouping2.Key;
                    settings.anisoLevel = 1;
                    if (settings.generateMipMaps)
                    {
                        foreach (Entry entry2 in grouping2)
                        {
                            if (entry2.anisoLevel > settings.anisoLevel)
                            {
                                settings.anisoLevel = entry2.anisoLevel;
                            }
                        }
                    }
                    job.AddAtlas(key, settings);
                    foreach (Entry entry3 in grouping2)
                    {
                        job.AssignToAtlas(key, entry3.sprite, entry3.packingMode, SpritePackingRotation.None);
                    }
                    num5++;
                }
            }
        }

        private string ParseAtlasName(string packingTag)
        {
            string str = packingTag.Trim();
            if (this.IsTagPrefixed(str))
            {
                str = str.Substring(this.TagPrefix.Length).Trim();
            }
            return ((str.Length != 0) ? str : "(unnamed)");
        }

        protected virtual bool AllowRotationFlipping
        {
            get
            {
                return false;
            }
        }

        protected virtual bool AllowTightWhenTagged
        {
            get
            {
                return true;
            }
        }

        protected virtual string TagPrefix
        {
            get
            {
                return "[TIGHT]";
            }
        }

        protected class Entry
        {
            public int anisoLevel;
            public string atlasName;
            public SpritePackingMode packingMode;
            public AtlasSettings settings;
            public Sprite sprite;
        }
    }
}

