namespace Unity.DataContract
{
    using System;
    using System.Collections.Generic;

    public class PackageInfo
    {
        public string basePath;
        public string description;
        public bool loaded;
        private Dictionary<string, PackageFileData> m_FileDict;
        public string name;
        public string organisation;
        public string releaseNotes;
        public PackageType type;
        public PackageVersion unityVersion;
        public PackageVersion version;

        public override bool Equals(object other)
        {
            return (this == (other as PackageInfo));
        }

        public override int GetHashCode()
        {
            int num = 0x11;
            num = (num * 0x17) + this.organisation.GetHashCode();
            num = (num * 0x17) + this.name.GetHashCode();
            num = (num * 0x17) + this.type.GetHashCode();
            num = (num * 0x17) + this.version.GetHashCode();
            return ((num * 0x17) + this.unityVersion.GetHashCode());
        }

        public static bool operator ==(PackageInfo a, PackageInfo z)
        {
            if ((a == null) && (z == null))
            {
                return true;
            }
            if ((a == null) || (z == null))
            {
                return false;
            }
            return (a.GetHashCode() == z.GetHashCode());
        }

        public static bool operator !=(PackageInfo a, PackageInfo z)
        {
            return !(a == z);
        }

        public override string ToString()
        {
            return string.Format("{0} {1} ({2}) v{3} for Unity v{4}", new object[] { this.organisation, this.name, this.type, (this.version == null) ? null : this.version.text, (this.unityVersion == null) ? null : this.basePath });
        }

        public Dictionary<string, PackageFileData> files
        {
            get
            {
                return this.m_FileDict;
            }
            set
            {
                this.m_FileDict = value;
            }
        }

        public string packageName
        {
            get
            {
                return string.Format("{0}.{1}", this.organisation, this.name);
            }
        }
    }
}

