namespace Unity.DataContract
{
    using System;

    public class PackageFileData
    {
        public string guid;
        public PackageFileType type;
        public string url;

        public PackageFileData()
        {
        }

        public PackageFileData(PackageFileType type, string url)
        {
            this.type = type;
            this.url = url;
        }

        public PackageFileData(PackageFileType type, string url, string guid) : this(type, url)
        {
            this.guid = guid;
        }
    }
}

