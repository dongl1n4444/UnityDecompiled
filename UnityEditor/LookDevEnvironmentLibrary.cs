namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class LookDevEnvironmentLibrary : ScriptableObject, ISerializationCallbackReceiver
    {
        private bool m_Dirty = false;
        [SerializeField]
        private List<CubemapInfo> m_HDRIList = new List<CubemapInfo>();
        private LookDevView m_LookDevView = null;
        [SerializeField]
        private List<CubemapInfo> m_SerialShadowMapHDRIList = new List<CubemapInfo>();

        public void CleanupDeletedHDRI()
        {
            while (this.RemoveHDRI(null))
            {
            }
        }

        private ShadowInfo GetCurrentShadowInfo()
        {
            return this.m_HDRIList[this.m_LookDevView.config.lookDevContexts[(int) this.m_LookDevView.config.currentEditionContext].currentHDRIIndex].shadowInfo;
        }

        public void InsertHDRI(Cubemap cubemap)
        {
            this.InsertHDRI(cubemap, -1);
        }

        public void InsertHDRI(Cubemap cubemap, int insertionIndex)
        {
            <InsertHDRI>c__AnonStorey0 storey = new <InsertHDRI>c__AnonStorey0 {
                cubemap = cubemap
            };
            Undo.RecordObject(this.m_LookDevView.envLibrary, "Insert HDRI");
            Undo.RecordObject(this.m_LookDevView.config, "Insert HDRI");
            storey.cubemap0 = null;
            storey.cubemap1 = null;
            if (storey.cubemap == LookDevResources.m_DefaultHDRI)
            {
                storey.cubemap0 = LookDevResources.m_DefaultHDRI;
                storey.cubemap1 = LookDevResources.m_DefaultHDRI;
            }
            else
            {
                storey.cubemap0 = this.m_HDRIList[this.m_LookDevView.config.lookDevContexts[0].currentHDRIIndex].cubemap;
                storey.cubemap1 = this.m_HDRIList[this.m_LookDevView.config.lookDevContexts[1].currentHDRIIndex].cubemap;
            }
            int index = this.m_HDRIList.FindIndex(new Predicate<CubemapInfo>(storey.<>m__0));
            if (index == -1)
            {
                this.m_Dirty = true;
                CubemapInfo newCubemapShadowInfo = null;
                for (int i = 0; i < this.m_HDRIList.Count; i++)
                {
                    if (this.m_HDRIList[i].cubemapShadowInfo.cubemap == storey.cubemap)
                    {
                        newCubemapShadowInfo = this.m_HDRIList[i].cubemapShadowInfo;
                        newCubemapShadowInfo.SetCubemapShadowInfo(newCubemapShadowInfo);
                        break;
                    }
                }
                if (newCubemapShadowInfo == null)
                {
                    newCubemapShadowInfo = new CubemapInfo {
                        cubemap = storey.cubemap
                    };
                    newCubemapShadowInfo.ambientProbe.Clear();
                    newCubemapShadowInfo.alreadyComputed = false;
                    newCubemapShadowInfo.SetCubemapShadowInfo(newCubemapShadowInfo);
                }
                int count = this.m_HDRIList.Count;
                this.m_HDRIList.Insert((insertionIndex != -1) ? insertionIndex : count, newCubemapShadowInfo);
                if (newCubemapShadowInfo.cubemap != LookDevResources.m_DefaultHDRI)
                {
                    LookDevResources.UpdateShadowInfoWithBrightestSpot(newCubemapShadowInfo);
                }
            }
            if (((index != insertionIndex) && (index != -1)) && (insertionIndex != -1))
            {
                CubemapInfo item = this.m_HDRIList[index];
                this.m_HDRIList.RemoveAt(index);
                this.m_HDRIList.Insert((index <= insertionIndex) ? (insertionIndex - 1) : insertionIndex, item);
            }
            this.m_LookDevView.config.lookDevContexts[0].UpdateProperty(LookDevProperty.HDRI, this.m_HDRIList.FindIndex(new Predicate<CubemapInfo>(storey.<>m__1)));
            this.m_LookDevView.config.lookDevContexts[1].UpdateProperty(LookDevProperty.HDRI, this.m_HDRIList.FindIndex(new Predicate<CubemapInfo>(storey.<>m__2)));
            this.m_LookDevView.Repaint();
        }

        public void OnAfterDeserialize()
        {
            for (int i = 0; i < this.m_HDRIList.Count; i++)
            {
                if (this.m_HDRIList[i].serialIndexMain != -1)
                {
                    this.m_HDRIList[i].cubemapShadowInfo = this.m_HDRIList[this.hdriList[i].serialIndexMain];
                }
                else
                {
                    this.m_HDRIList[i].cubemapShadowInfo = this.m_SerialShadowMapHDRIList[this.m_HDRIList[i].serialIndexShadow];
                }
            }
        }

        public void OnBeforeSerialize()
        {
            this.m_SerialShadowMapHDRIList.Clear();
            for (int i = 0; i < this.m_HDRIList.Count; i++)
            {
                <OnBeforeSerialize>c__AnonStorey3 storey = new <OnBeforeSerialize>c__AnonStorey3 {
                    shadowCubemapInfo = this.m_HDRIList[i].cubemapShadowInfo
                };
                this.m_HDRIList[i].serialIndexMain = this.m_HDRIList.FindIndex(new Predicate<CubemapInfo>(storey.<>m__0));
                if (this.m_HDRIList[i].serialIndexMain == -1)
                {
                    this.m_HDRIList[i].serialIndexShadow = this.m_SerialShadowMapHDRIList.FindIndex(new Predicate<CubemapInfo>(storey.<>m__1));
                    if (this.m_HDRIList[i].serialIndexShadow == -1)
                    {
                        this.m_SerialShadowMapHDRIList.Add(storey.shadowCubemapInfo);
                        this.m_HDRIList[i].serialIndexShadow = this.m_SerialShadowMapHDRIList.Count - 1;
                    }
                }
            }
        }

        public bool RemoveHDRI(Cubemap cubemap)
        {
            <RemoveHDRI>c__AnonStorey1 storey = new <RemoveHDRI>c__AnonStorey1 {
                cubemap = cubemap
            };
            if (storey.cubemap != null)
            {
                Undo.RecordObject(this.m_LookDevView.envLibrary, "Remove HDRI");
                Undo.RecordObject(this.m_LookDevView.config, "Remove HDRI");
            }
            if (storey.cubemap == LookDevResources.m_DefaultHDRI)
            {
                Debug.LogWarning("Cannot remove default HDRI from the library");
                return false;
            }
            int index = this.m_HDRIList.FindIndex(new Predicate<CubemapInfo>(storey.<>m__0));
            if (index != -1)
            {
                <RemoveHDRI>c__AnonStorey2 storey2 = new <RemoveHDRI>c__AnonStorey2 {
                    cubemap0 = this.m_HDRIList[this.m_LookDevView.config.lookDevContexts[0].currentHDRIIndex].cubemap,
                    cubemap1 = this.m_HDRIList[this.m_LookDevView.config.lookDevContexts[1].currentHDRIIndex].cubemap
                };
                this.m_HDRIList.RemoveAt(index);
                int num2 = (this.m_HDRIList.Count != 0) ? 0 : -1;
                this.m_LookDevView.config.lookDevContexts[0].UpdateProperty(LookDevProperty.HDRI, (storey2.cubemap0 != storey.cubemap) ? this.m_HDRIList.FindIndex(new Predicate<CubemapInfo>(storey2.<>m__0)) : num2);
                this.m_LookDevView.config.lookDevContexts[1].UpdateProperty(LookDevProperty.HDRI, (storey2.cubemap1 != storey.cubemap) ? this.m_HDRIList.FindIndex(new Predicate<CubemapInfo>(storey2.<>m__1)) : num2);
                this.m_LookDevView.Repaint();
                this.m_Dirty = true;
                return true;
            }
            return false;
        }

        public void SetLookDevView(LookDevView lookDevView)
        {
            this.m_LookDevView = lookDevView;
        }

        public bool dirty
        {
            get
            {
                return this.m_Dirty;
            }
            set
            {
                this.m_Dirty = value;
            }
        }

        public int hdriCount
        {
            get
            {
                return this.hdriList.Count;
            }
        }

        public List<CubemapInfo> hdriList
        {
            get
            {
                return this.m_HDRIList;
            }
        }

        [CompilerGenerated]
        private sealed class <InsertHDRI>c__AnonStorey0
        {
            internal Cubemap cubemap;
            internal Cubemap cubemap0;
            internal Cubemap cubemap1;

            internal bool <>m__0(CubemapInfo x)
            {
                return (x.cubemap == this.cubemap);
            }

            internal bool <>m__1(CubemapInfo x)
            {
                return (x.cubemap == this.cubemap0);
            }

            internal bool <>m__2(CubemapInfo x)
            {
                return (x.cubemap == this.cubemap1);
            }
        }

        [CompilerGenerated]
        private sealed class <OnBeforeSerialize>c__AnonStorey3
        {
            internal CubemapInfo shadowCubemapInfo;

            internal bool <>m__0(CubemapInfo x)
            {
                return (x == this.shadowCubemapInfo);
            }

            internal bool <>m__1(CubemapInfo x)
            {
                return (x == this.shadowCubemapInfo);
            }
        }

        [CompilerGenerated]
        private sealed class <RemoveHDRI>c__AnonStorey1
        {
            internal Cubemap cubemap;

            internal bool <>m__0(CubemapInfo x)
            {
                return (x.cubemap == this.cubemap);
            }
        }

        [CompilerGenerated]
        private sealed class <RemoveHDRI>c__AnonStorey2
        {
            internal Cubemap cubemap0;
            internal Cubemap cubemap1;

            internal bool <>m__0(CubemapInfo x)
            {
                return (x.cubemap == this.cubemap0);
            }

            internal bool <>m__1(CubemapInfo x)
            {
                return (x.cubemap == this.cubemap1);
            }
        }
    }
}

