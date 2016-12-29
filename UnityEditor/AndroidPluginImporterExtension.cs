namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;
    using UnityEditor.Modules;
    using UnityEngine;

    internal class AndroidPluginImporterExtension : DefaultPluginImporterExtension
    {
        internal static readonly string cpuTag = "CPU";

        public AndroidPluginImporterExtension() : base(GetProperties())
        {
        }

        public override bool CheckFileCollisions(string buildTargetName)
        {
            bool flag = false;
            Dictionary<string, List<PluginInfo>> dictionary = new Dictionary<string, List<PluginInfo>>();
            foreach (PluginImporter importer in PluginImporter.GetImporters(BuildTarget.Android))
            {
                List<PluginInfo> list;
                PluginInfo item = new PluginInfo(buildTargetName, importer);
                if (dictionary.TryGetValue(item.assetName, out list))
                {
                    foreach (PluginInfo info2 in list)
                    {
                        if (item.ConflictsWith(info2))
                        {
                            Debug.LogError($"Found plugins with same names and architectures, {item.assetPath} ({item.cpuType}) and {info2.assetPath} ({info2.cpuType}). Assign different architectures or delete the duplicate.");
                            flag = true;
                        }
                    }
                }
                else
                {
                    dictionary[item.assetName] = list = new List<PluginInfo>();
                }
                list.Add(item);
            }
            return flag;
        }

        private static DefaultPluginImporterExtension.Property[] GetProperties()
        {
            string buildTargetName = BuildPipeline.GetBuildTargetName(BuildTarget.Android);
            return new DefaultPluginImporterExtension.Property[] { new DefaultPluginImporterExtension.Property("CPU", cpuTag, AndroidPluginCPUArchitecture.ARMv7, buildTargetName) };
        }

        public override void OnPlatformSettingsGUI(PluginImporterInspector inspector)
        {
            if (!base.propertiesRefreshed)
            {
                this.RefreshProperties(inspector);
            }
            EditorGUI.BeginChangeCheck();
            foreach (DefaultPluginImporterExtension.Property property in base.properties)
            {
                if ((property.key != cpuTag) || (Path.GetExtension(inspector.importer.assetPath) == ".so"))
                {
                    property.OnGUI(inspector);
                }
            }
            if (EditorGUI.EndChangeCheck())
            {
                base.hasModified = true;
            }
        }

        internal enum AndroidPluginCPUArchitecture
        {
            ARMv7,
            x86
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct PluginInfo
        {
            public readonly string assetName;
            public readonly string assetPath;
            public readonly string cpuType;
            public PluginInfo(string buildTargetName, PluginImporter plugin)
            {
                this.assetPath = plugin.assetPath;
                this.assetName = Path.GetFileName(plugin.assetPath);
                this.cpuType = plugin.GetPlatformData(buildTargetName, AndroidPluginImporterExtension.cpuTag);
            }

            public bool ConflictsWith(AndroidPluginImporterExtension.PluginInfo other) => 
                ((this.assetName == other.assetName) && (((this.cpuType == "AnyCPU") || (other.cpuType == "AnyCPU")) || (other.cpuType == this.cpuType)));
        }
    }
}

