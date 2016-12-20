namespace Unity.PackageManager.Ivy
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class IvyArtifact
    {
        private List<string> configurations;
        public string Extension;
        public string Guid;
        public string Name;
        public Uri PublicUrl;
        public ArtifactType Type;
        public Uri Url;

        public IvyArtifact()
        {
        }

        public IvyArtifact(string filename) : this(filename, ArtifactType.None)
        {
        }

        public IvyArtifact(string filename, ArtifactType type)
        {
            this.Name = Path.GetFileNameWithoutExtension(filename);
            this.Extension = Path.GetExtension(filename);
            this.Type = type;
        }

        public IvyArtifact Clone()
        {
            return Cloner.CloneObject<IvyArtifact>(this);
        }

        public string WriteToDisk(System.Guid jobId, string basePath, byte[] bytes)
        {
            string path = Path.Combine(basePath, jobId.ToString());
            Directory.CreateDirectory(path);
            File.WriteAllBytes(Path.Combine(path, this.Filename), bytes);
            return Path.Combine(path, this.Filename);
        }

        public List<string> Configurations
        {
            get
            {
                if (this.configurations == null)
                {
                    this.configurations = new List<string>();
                }
                return this.configurations;
            }
        }

        public string Filename
        {
            get
            {
                return (!string.IsNullOrEmpty(this.Extension) ? string.Format("{0}.{1}", this.Name, this.Extension) : this.Name);
            }
        }

        public string MD5Filename
        {
            get
            {
                return string.Format("{0}.md5", this.Name);
            }
        }

        public Uri MD5Uri
        {
            get
            {
                if (this.Url == null)
                {
                    return null;
                }
                return new Uri(this.Url.ToString().Replace(this.Filename, this.MD5Filename));
            }
        }
    }
}

