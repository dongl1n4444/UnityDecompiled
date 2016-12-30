namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Profiling;

    internal class EmissionTreeDataStore
    {
        [CompilerGenerated]
        private static Func<MeshRenderer, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<Material, bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<Material, bool> <>f__am$cache2;
        private static MaterialGlobalIlluminationFlags kErrorMask = (MaterialGlobalIlluminationFlags.EmissiveIsBlack | MaterialGlobalIlluminationFlags.BakedEmissive);
        private List<Data> m_Elements;

        public EmissionTreeDataStore()
        {
            this.Repopulate();
        }

        public List<Data> GetElements() => 
            this.m_Elements;

        public void Repopulate()
        {
            Profiler.BeginSample("EmissionTreeDataStore.Repopulate");
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = delegate (MeshRenderer mr) {
                    if (!GameObjectUtility.AreStaticEditorFlagsSet(mr.gameObject, StaticEditorFlags.LightmapStatic))
                    {
                        return false;
                    }
                    if (<>f__am$cache2 == null)
                    {
                        <>f__am$cache2 = m => (m != null) && ((m.globalIlluminationFlags & MaterialGlobalIlluminationFlags.AnyEmissive) != MaterialGlobalIlluminationFlags.None);
                    }
                    return Enumerable.Any<Material>(mr.sharedMaterials, <>f__am$cache2);
                };
            }
            IEnumerable<MeshRenderer> enumerable = Enumerable.Where<MeshRenderer>(Object.FindObjectsOfType<MeshRenderer>(), <>f__am$cache0);
            this.m_Elements = new List<Data>(50);
            foreach (MeshRenderer renderer in enumerable)
            {
                Data item = new Data {
                    go = renderer.gameObject
                };
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = m => (m != null) && ((m.globalIlluminationFlags & MaterialGlobalIlluminationFlags.AnyEmissive) != MaterialGlobalIlluminationFlags.None);
                }
                IEnumerable<Material> enumerable2 = Enumerable.Where<Material>(renderer.sharedMaterials, <>f__am$cache1);
                foreach (Material material in enumerable2)
                {
                    item.mat = material;
                    item.isBlack = material.globalIlluminationFlags == kErrorMask;
                    item.isBaked = (material.globalIlluminationFlags & MaterialGlobalIlluminationFlags.BakedEmissive) != MaterialGlobalIlluminationFlags.None;
                    this.m_Elements.Add(item);
                }
            }
            Profiler.EndSample();
        }

        public void Update()
        {
            for (int i = 0; i < this.m_Elements.Count; i++)
            {
                Data data = this.m_Elements[i];
                if (data.mat != null)
                {
                    data.isBlack = data.mat.globalIlluminationFlags == kErrorMask;
                    data.isBaked = (data.mat.globalIlluminationFlags & MaterialGlobalIlluminationFlags.BakedEmissive) != MaterialGlobalIlluminationFlags.None;
                    this.m_Elements[i] = data;
                }
            }
        }

        internal class Data
        {
            public GameObject go;
            public bool isBaked;
            public bool isBlack;
            public Material mat;

            public int GIModeId() => 
                ((((this.isBaked == null) ? 0 : 1) << 1) | (!this.isBlack ? 0 : 1));

            public string GIModeName() => 
                (!this.isBaked ? "Realtime" : "Baked");

            public bool IsBlack() => 
                this.isBlack;

            public int MaterialId() => 
                this.mat.GetInstanceID();

            public string MaterialName() => 
                this.mat.name;

            public int ObjectId() => 
                this.go.GetInstanceID();

            public string ObjectName() => 
                this.go.name;
        }
    }
}

