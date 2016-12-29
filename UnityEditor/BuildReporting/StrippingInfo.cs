namespace UnityEditor.BuildReporting
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    internal class StrippingInfo : ScriptableObject, ISerializationCallbackReceiver
    {
        public Dictionary<string, HashSet<string>> dependencies = new Dictionary<string, HashSet<string>>();
        public Dictionary<string, string> icons = new Dictionary<string, string>();
        public List<string> modules = new List<string>();
        public const string RequiredByScripts = "Required by Scripts";
        public List<SerializedDependency> serializedDependencies;
        public List<int> serializedSizes = new List<int>();
        public Dictionary<string, int> sizes = new Dictionary<string, int>();
        public int totalSize = 0;

        public void AddModule(string module)
        {
            if (!this.modules.Contains(module))
            {
                this.modules.Add(module);
            }
            if (!this.sizes.ContainsKey(module))
            {
                this.sizes[module] = 0;
            }
            if (!this.icons.ContainsKey(module))
            {
                this.SetIcon(module, "class/DefaultAsset");
            }
        }

        public void AddModuleSize(string module, int size)
        {
            if (this.modules.Contains(module))
            {
                this.sizes[module] = size;
            }
        }

        public static StrippingInfo GetBuildReportData(BuildReport report)
        {
            if (report == null)
            {
                return null;
            }
            StrippingInfo[] appendices = (StrippingInfo[]) report.GetAppendices(typeof(StrippingInfo));
            if (appendices.Length > 0)
            {
                return appendices[0];
            }
            StrippingInfo info2 = ScriptableObject.CreateInstance<StrippingInfo>();
            report.AddAppendix(info2);
            return info2;
        }

        public static string ModuleName(string module) => 
            (module + " Module");

        public void OnAfterDeserialize()
        {
            this.dependencies = new Dictionary<string, HashSet<string>>();
            this.icons = new Dictionary<string, string>();
            for (int i = 0; i < this.serializedDependencies.Count; i++)
            {
                HashSet<string> set = new HashSet<string>();
                SerializedDependency dependency = this.serializedDependencies[i];
                foreach (string str in dependency.value)
                {
                    set.Add(str);
                }
                SerializedDependency dependency2 = this.serializedDependencies[i];
                this.dependencies.Add(dependency2.key, set);
                SerializedDependency dependency3 = this.serializedDependencies[i];
                SerializedDependency dependency4 = this.serializedDependencies[i];
                this.icons[dependency3.key] = dependency4.icon;
            }
            this.sizes = new Dictionary<string, int>();
            for (int j = 0; j < this.serializedSizes.Count; j++)
            {
                this.sizes[this.modules[j]] = this.serializedSizes[j];
            }
        }

        public void OnBeforeSerialize()
        {
            this.serializedDependencies = new List<SerializedDependency>();
            foreach (KeyValuePair<string, HashSet<string>> pair in this.dependencies)
            {
                SerializedDependency dependency;
                List<string> list = new List<string>();
                foreach (string str in pair.Value)
                {
                    list.Add(str);
                }
                dependency.key = pair.Key;
                dependency.value = list;
                dependency.icon = !this.icons.ContainsKey(pair.Key) ? "class/DefaultAsset" : this.icons[pair.Key];
                this.serializedDependencies.Add(dependency);
            }
            this.serializedSizes = new List<int>();
            foreach (string str2 in this.modules)
            {
                this.serializedSizes.Add(!this.sizes.ContainsKey(str2) ? 0 : this.sizes[str2]);
            }
        }

        private void OnEnable()
        {
            this.SetIcon("Required by Scripts", "class/MonoScript");
            this.SetIcon(ModuleName("AI"), "class/NavMeshAgent");
            this.SetIcon(ModuleName("Animation"), "class/Animation");
            this.SetIcon(ModuleName("Audio"), "class/AudioSource");
            this.SetIcon(ModuleName("Core"), "class/GameManager");
            this.SetIcon(ModuleName("IMGUI"), "class/GUILayer");
            this.SetIcon(ModuleName("ParticleSystem"), "class/ParticleSystem");
            this.SetIcon(ModuleName("ParticlesLegacy"), "class/EllipsoidParticleEmitter");
            this.SetIcon(ModuleName("Physics"), "class/PhysicMaterial");
            this.SetIcon(ModuleName("Physics2D"), "class/PhysicsMaterial2D");
            this.SetIcon(ModuleName("TextRendering"), "class/Font");
            this.SetIcon(ModuleName("UI"), "class/CanvasGroup");
            this.SetIcon(ModuleName("Umbra"), "class/OcclusionCullingSettings");
            this.SetIcon(ModuleName("UNET"), "class/NetworkTransform");
            this.SetIcon(ModuleName("Vehicles"), "class/WheelCollider");
            this.SetIcon(ModuleName("Cloth"), "class/Cloth");
        }

        public void RegisterDependency(string obj, string depends)
        {
            if (!this.dependencies.ContainsKey(obj))
            {
                this.dependencies[obj] = new HashSet<string>();
            }
            this.dependencies[obj].Add(depends);
            if (!this.icons.ContainsKey(depends))
            {
                this.SetIcon(depends, "class/" + depends);
            }
        }

        public void SetIcon(string dependency, string icon)
        {
            this.icons[dependency] = icon;
            if (!this.dependencies.ContainsKey(dependency))
            {
                this.dependencies[dependency] = new HashSet<string>();
            }
        }

        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct SerializedDependency
        {
            [SerializeField]
            public string key;
            [SerializeField]
            public List<string> value;
            [SerializeField]
            public string icon;
        }
    }
}

