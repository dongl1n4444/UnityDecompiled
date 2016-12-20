namespace Unity.PackageManager.Ivy
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Unity.DataContract;

    public class IvyModule
    {
        [CompilerGenerated]
        private static Func<IvyArtifact, string> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<IvyArtifact, PackageFileData> <>f__am$cache1;
        private IvyArtifacts artifacts;
        public string BasePath;
        private IvyDependencies dependencies;
        private IvyInfo info;
        public string IvyFile;
        public bool Loaded;
        public bool Selected;
        public string Timestamp;

        public IvyModule Clone()
        {
            return Cloner.CloneObject<IvyModule>(this);
        }

        public override bool Equals(object other)
        {
            if (other is PackageInfo)
            {
                return (this == (other as PackageInfo));
            }
            return (this == (other as IvyModule));
        }

        public static IvyModule FromIvyFile(string fullpath)
        {
            return IvyParser.ParseFile<IvyModule>(fullpath);
        }

        public static IvyModule FromPackageInfo(PackageInfo package)
        {
            <FromPackageInfo>c__AnonStorey0 storey = new <FromPackageInfo>c__AnonStorey0 {
                package = package
            };
            IvyModule module = new IvyModule {
                Info = { 
                    Organisation = storey.package.organisation,
                    UnityVersion = storey.package.unityVersion,
                    Module = storey.package.name,
                    Version = storey.package.version,
                    Type = storey.package.type
                },
                BasePath = storey.package.basePath,
                Loaded = storey.package.loaded
            };
            IvyArtifact item = new IvyArtifact {
                Name = module.Info.FullName,
                Type = ArtifactType.Ivy,
                Extension = "xml"
            };
            module.Artifacts.Add(item);
            if (storey.package.files != null)
            {
                module.Artifacts.AddRange(Enumerable.Select<string, IvyArtifact>(storey.package.files.Keys, new Func<string, IvyArtifact>(storey, (IntPtr) this.<>m__0)));
            }
            return module;
        }

        public IvyArtifact GetArtifact(string filename)
        {
            <GetArtifact>c__AnonStorey2 storey = new <GetArtifact>c__AnonStorey2 {
                filename = filename
            };
            return Enumerable.FirstOrDefault<IvyArtifact>(this.Artifacts, new Func<IvyArtifact, bool>(storey, (IntPtr) this.<>m__0));
        }

        public IvyArtifact GetArtifact(ArtifactType type)
        {
            <GetArtifact>c__AnonStorey1 storey = new <GetArtifact>c__AnonStorey1 {
                type = type
            };
            return Enumerable.FirstOrDefault<IvyArtifact>(this.Artifacts, new Func<IvyArtifact, bool>(storey, (IntPtr) this.<>m__0));
        }

        public override int GetHashCode()
        {
            int num = 0x11;
            num = (num * 0x17) + this.Info.Organisation.GetHashCode();
            num = (num * 0x17) + this.Info.Module.GetHashCode();
            num = (num * 0x17) + this.Info.Type.GetHashCode();
            num = (num * 0x17) + this.Info.Version.GetHashCode();
            return ((num * 0x17) + this.Info.UnityVersion.GetHashCode());
        }

        public IvyRepository GetRepository(string name)
        {
            <GetRepository>c__AnonStorey3 storey = new <GetRepository>c__AnonStorey3 {
                name = name
            };
            return Enumerable.FirstOrDefault<IvyRepository>(this.Info.repositories, new Func<IvyRepository, bool>(storey, (IntPtr) this.<>m__0));
        }

        public static bool operator ==(IvyModule a, object z)
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

        public static bool operator !=(IvyModule a, object z)
        {
            return (a != z);
        }

        public PackageInfo ToPackageInfo()
        {
            PackageInfo info2 = new PackageInfo {
                unityVersion = this.Info.UnityVersion,
                name = this.Info.Module,
                organisation = this.Info.Organisation,
                version = this.Info.Version,
                type = this.Info.Type,
                basePath = this.BasePath,
                description = this.Info.Description,
                loaded = this.Loaded
            };
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<IvyArtifact, string>(null, (IntPtr) <ToPackageInfo>m__0);
            }
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = new Func<IvyArtifact, PackageFileData>(null, (IntPtr) <ToPackageInfo>m__1);
            }
            info2.files = Enumerable.ToDictionary<IvyArtifact, string, PackageFileData>(this.Artifacts, <>f__am$cache0, <>f__am$cache1);
            PackageInfo info = info2;
            IvyArtifact artifact = this.GetArtifact(ArtifactType.ReleaseNotes);
            if ((artifact != null) && (this.BasePath != null))
            {
                string path = Path.Combine(this.BasePath, artifact.Filename);
                if (File.Exists(path))
                {
                    info.releaseNotes = File.ReadAllText(path);
                }
            }
            return info;
        }

        public override string ToString()
        {
            return IvyParser.Serialize(this);
        }

        public string WriteIvyFile()
        {
            if (this.BasePath == null)
            {
                throw new InvalidOperationException("Can't save IvyModule without path information");
            }
            return this.WriteIvyFile(this.BasePath, this.IvyFile, true);
        }

        public string WriteIvyFile(string outputPath)
        {
            return this.WriteIvyFile(outputPath, this.IvyFile, false);
        }

        public string WriteIvyFile(string outputPath, string filename)
        {
            if (filename == null)
            {
            }
            return this.WriteIvyFile(outputPath, this.IvyFile, false);
        }

        private string WriteIvyFile(string outputPath, string filename, bool savePath)
        {
            if (filename == null)
            {
                filename = "ivy.xml";
            }
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }
            string path = Path.Combine(outputPath, filename);
            if (!savePath)
            {
                this.BasePath = (string) (this.IvyFile = null);
            }
            string str3 = IvyParser.Serialize(this);
            using (StreamWriter writer = File.CreateText(path))
            {
                writer.Write(str3);
            }
            this.BasePath = outputPath;
            this.IvyFile = filename;
            return path;
        }

        public IvyArtifacts Artifacts
        {
            get
            {
                if (this.artifacts == null)
                {
                    this.artifacts = new IvyArtifacts();
                }
                return this.artifacts;
            }
        }

        public IvyDependencies Dependencies
        {
            get
            {
                if (this.dependencies == null)
                {
                    this.dependencies = new IvyDependencies();
                }
                return this.dependencies;
            }
        }

        public IvyInfo Info
        {
            get
            {
                if (this.info == null)
                {
                    this.info = new IvyInfo();
                }
                return this.info;
            }
            set
            {
                this.info = value;
            }
        }

        public string Name
        {
            get
            {
                return string.Format("{0}.{1}", this.Info.Organisation, this.Info.Module);
            }
        }

        public bool Public
        {
            get
            {
                return this.Info.Published;
            }
            set
            {
                this.Info.Published = value;
            }
        }

        public PackageVersion UnityVersion
        {
            get
            {
                return this.Info.UnityVersion;
            }
        }

        public PackageVersion Version
        {
            get
            {
                return this.Info.Version;
            }
        }

        [CompilerGenerated]
        private sealed class <FromPackageInfo>c__AnonStorey0
        {
            internal PackageInfo package;

            internal IvyArtifact <>m__0(string f)
            {
                return new IvyArtifact(f) { 
                    Type = (ArtifactType) this.package.files[f].type,
                    Url = new Uri(this.package.files[f].url)
                };
            }
        }

        [CompilerGenerated]
        private sealed class <GetArtifact>c__AnonStorey1
        {
            internal ArtifactType type;

            internal bool <>m__0(IvyArtifact x)
            {
                return (x.Type == this.type);
            }
        }

        [CompilerGenerated]
        private sealed class <GetArtifact>c__AnonStorey2
        {
            internal string filename;

            internal bool <>m__0(IvyArtifact x)
            {
                return (x.Filename == this.filename);
            }
        }

        [CompilerGenerated]
        private sealed class <GetRepository>c__AnonStorey3
        {
            internal string name;

            internal bool <>m__0(IvyRepository x)
            {
                return (x.Name == this.name);
            }
        }
    }
}

