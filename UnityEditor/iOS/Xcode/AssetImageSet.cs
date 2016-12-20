namespace UnityEditor.iOS.Xcode
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    internal class AssetImageSet : AssetCatalogItemWithVariants
    {
        internal AssetImageSet(string assetCatalogPath, string name, string authorId) : base(name, authorId)
        {
            base.m_Path = Path.Combine(assetCatalogPath, name + ".imageset");
        }

        public void AddVariant(DeviceRequirement requirement, string path)
        {
            base.AddVariant(new ImageSetVariant(requirement, path));
        }

        public void AddVariant(DeviceRequirement requirement, string path, ImageAlignment alignment, ImageResizing resizing)
        {
            ImageSetVariant newItem = new ImageSetVariant(requirement, path) {
                alignment = alignment,
                resizing = resizing
            };
            base.AddVariant(newItem);
        }

        private static string GetCenterResizeMode(ImageResizing.ResizeMode mode)
        {
            if (mode != ImageResizing.ResizeMode.Stretch)
            {
                if (mode == ImageResizing.ResizeMode.Tile)
                {
                    return "tile";
                }
            }
            else
            {
                return "stretch";
            }
            return "";
        }

        private static string GetSlicingMode(ImageResizing.SlicingType mode)
        {
            if (mode != ImageResizing.SlicingType.Horizontal)
            {
                if (mode == ImageResizing.SlicingType.Vertical)
                {
                    return "3-part-vertical";
                }
                if (mode == ImageResizing.SlicingType.HorizontalAndVertical)
                {
                    return "9-part";
                }
            }
            else
            {
                return "3-part-horizontal";
            }
            return "";
        }

        public override void Write(List<string> warnings)
        {
            Directory.CreateDirectory(base.m_Path);
            JsonDocument doc = new JsonDocument();
            JsonElementDict info = base.WriteInfoToJson(doc);
            base.WriteODRTagsToJson(info);
            JsonElementArray array = doc.root.CreateArray("images");
            foreach (ImageSetVariant variant in base.m_Variants)
            {
                string fileName = Path.GetFileName(variant.path);
                if (!File.Exists(variant.path))
                {
                    if (warnings != null)
                    {
                        warnings.Add("File not found: " + variant.path);
                    }
                }
                else
                {
                    File.Copy(variant.path, Path.Combine(base.m_Path, fileName));
                }
                JsonElementDict item = array.AddDict();
                item.SetString("filename", fileName);
                base.WriteRequirementsToJson(item, variant.requirement);
                if (variant.alignment != null)
                {
                    this.WriteAlignmentToJson(item, variant.alignment);
                }
                if (variant.resizing != null)
                {
                    this.WriteResizingToJson(item, variant.resizing);
                }
            }
            doc.WriteToFile(Path.Combine(base.m_Path, "Contents.json"));
        }

        private void WriteAlignmentToJson(JsonElementDict item, ImageAlignment alignment)
        {
            JsonElementDict dict = item.CreateDict("alignment-insets");
            dict.SetInteger("top", alignment.top);
            dict.SetInteger("bottom", alignment.bottom);
            dict.SetInteger("left", alignment.left);
            dict.SetInteger("right", alignment.right);
        }

        private void WriteResizingToJson(JsonElementDict item, ImageResizing resizing)
        {
            JsonElementDict dict = item.CreateDict("resizing");
            dict.SetString("mode", GetSlicingMode(resizing.type));
            JsonElementDict dict2 = dict.CreateDict("center");
            dict2.SetString("mode", GetCenterResizeMode(resizing.centerResizeMode));
            dict2.SetInteger("width", resizing.centerWidth);
            dict2.SetInteger("height", resizing.centerHeight);
            JsonElementDict dict3 = dict.CreateDict("cap-insets");
            dict3.SetInteger("top", resizing.top);
            dict3.SetInteger("bottom", resizing.bottom);
            dict3.SetInteger("left", resizing.left);
            dict3.SetInteger("right", resizing.right);
        }

        private class ImageSetVariant : AssetCatalogItemWithVariants.VariantData
        {
            public ImageAlignment alignment;
            public ImageResizing resizing;

            public ImageSetVariant(DeviceRequirement requirement, string path) : base(requirement, path)
            {
                this.alignment = null;
                this.resizing = null;
            }
        }
    }
}

