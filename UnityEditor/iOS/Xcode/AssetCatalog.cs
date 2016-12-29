namespace UnityEditor.iOS.Xcode
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    internal class AssetCatalog
    {
        private AssetFolder m_Root;

        public AssetCatalog(string path, string authorId)
        {
            if (Path.GetExtension(path) != ".xcassets")
            {
                throw new Exception("Asset catalogs must have xcassets extension");
            }
            this.m_Root = new AssetFolder(path, null, authorId);
        }

        public AssetBrandAssetGroup OpenBrandAssetGroup(string relativePath) => 
            this.OpenFolderForResource(relativePath).OpenBrandAssetGroup(Path.GetFileName(relativePath));

        public AssetDataSet OpenDataSet(string relativePath) => 
            this.OpenFolderForResource(relativePath).OpenDataSet(Path.GetFileName(relativePath));

        public AssetFolder OpenFolder(string relativePath)
        {
            if (relativePath == null)
            {
                return this.root;
            }
            string[] strArray = PBXPath.Split(relativePath);
            if (strArray.Length == 0)
            {
                return this.root;
            }
            AssetFolder root = this.root;
            foreach (string str in strArray)
            {
                root = root.OpenFolder(str);
            }
            return root;
        }

        private AssetFolder OpenFolderForResource(string relativePath)
        {
            List<string> list = PBXPath.Split(relativePath).ToList<string>();
            list.RemoveAt(list.Count - 1);
            AssetFolder root = this.root;
            foreach (string str in list)
            {
                root = root.OpenFolder(str);
            }
            return root;
        }

        public AssetImageSet OpenImageSet(string relativePath) => 
            this.OpenFolderForResource(relativePath).OpenImageSet(Path.GetFileName(relativePath));

        public AssetImageStack OpenImageStack(string relativePath) => 
            this.OpenFolderForResource(relativePath).OpenImageStack(Path.GetFileName(relativePath));

        public AssetFolder OpenNamespacedFolder(string relativeBasePath, string namespacePath)
        {
            AssetFolder folder = this.OpenFolder(relativeBasePath);
            string[] strArray = PBXPath.Split(namespacePath);
            foreach (string str in strArray)
            {
                folder = folder.OpenFolder(str);
                folder.providesNamespace = true;
            }
            return folder;
        }

        public void Write()
        {
            this.Write(null);
        }

        public void Write(List<string> warnings)
        {
            this.m_Root.Write(warnings);
        }

        public string path =>
            this.m_Root.path;

        public AssetFolder root =>
            this.m_Root;
    }
}

