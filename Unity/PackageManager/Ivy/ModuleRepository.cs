namespace Unity.PackageManager.Ivy
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Unity.DataContract;

    public class ModuleRepository
    {
        private List<IvyModule> modules;

        public ModuleRepository Clone()
        {
            return Cloner.CloneObject<ModuleRepository>(this);
        }

        public static ModuleRepository FromIvyFile(string fullpath)
        {
            return IvyParser.ParseFile<ModuleRepository>(fullpath);
        }

        public IvyModule GetPackage(PackageType type)
        {
            foreach (IvyModule module in this.modules)
            {
                if (module.Info.Type == type)
                {
                    return module;
                }
            }
            return null;
        }

        public IvyModule GetPackage(string org, string name, string version)
        {
            foreach (IvyModule module in this.modules)
            {
                if (module.Info.FullName == string.Format("{0}.{1}.{2}", org, name, version))
                {
                    return module;
                }
            }
            return null;
        }

        public override string ToString()
        {
            return IvyParser.Serialize(this);
        }

        public string WriteIvyFile(string outputPath)
        {
            return this.WriteIvyFile(outputPath, null);
        }

        public string WriteIvyFile(string outputPath, string filename)
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
            string str2 = IvyParser.Serialize(this);
            using (StreamWriter writer = File.CreateText(path))
            {
                writer.Write(str2);
            }
            return path;
        }

        public List<IvyModule> Modules
        {
            get
            {
                if (this.modules == null)
                {
                    this.modules = new List<IvyModule>();
                }
                return this.modules;
            }
        }
    }
}

