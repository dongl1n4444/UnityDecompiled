namespace UnityEngine.VR.WSA
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public sealed class SpatialMappingContext
    {
        private static readonly SpatialMappingContext instance = new SpatialMappingContext();
        private const int kIdealInFlightSurfaceCount = 2;
        private List<SMComponentRecord> m_Components = new List<SMComponentRecord>();
        private SMBakeRequest[] m_InFlightRequests = new SMBakeRequest[2];
        private int m_InFlightSurfaces = 0;
        private int m_NextIndex = 0;

        private SpatialMappingContext()
        {
        }

        public void ComponentHasDataRequests()
        {
            this.RequestMeshPriorityFromComponents();
        }

        public void DeregisterComponent(SpatialMappingBase smComponent)
        {
            <DeregisterComponent>c__AnonStorey1 storey = new <DeregisterComponent>c__AnonStorey1 {
                smComponent = smComponent
            };
            if (this.m_Components.RemoveAll(new Predicate<SMComponentRecord>(storey.<>m__0)) == 0)
            {
                throw new ArgumentException("DeregisterComponent for a component not registered!");
            }
        }

        private int GetInFlightIndexFromSD(SurfaceData sd)
        {
            for (int i = 0; i < this.m_InFlightRequests.Length; i++)
            {
                SMBakeRequest request = this.m_InFlightRequests[i];
                if (((request.m_RequestData.id.handle == sd.id.handle) && (request.m_RequestData.trianglesPerCubicMeter == sd.trianglesPerCubicMeter)) && (request.m_RequestData.bakeCollider == sd.bakeCollider))
                {
                    return i;
                }
            }
            return -1;
        }

        private SpatialMappingBase GetSMComponentFromInFlightIndex(int inFlightIndex)
        {
            if (inFlightIndex < 0)
            {
                return null;
            }
            if (((this.m_InFlightRequests == null) || (inFlightIndex >= this.m_InFlightRequests.Length)) || this.m_InFlightRequests[inFlightIndex].IsClear())
            {
                return null;
            }
            return this.m_InFlightRequests[inFlightIndex].m_Requester.m_Component;
        }

        public void OnSurfaceDataReady(SurfaceData sd, bool outputWritten, float elapsedBakeTimeSeconds)
        {
            int inFlightIndexFromSD = this.GetInFlightIndexFromSD(sd);
            this.PropagateDataReadyEventToComponents(sd, outputWritten, elapsedBakeTimeSeconds, inFlightIndexFromSD);
            this.UpdateInFlightRecords(inFlightIndexFromSD, elapsedBakeTimeSeconds);
            this.RequestMeshPriorityFromComponents();
        }

        private void PropagateDataReadyEventToComponents(SurfaceData sd, bool outputWritten, float elapsedBakeTimeSeconds, int inFlightIndex)
        {
            SpatialMappingBase.LODType lODFromTPCM = SpatialMappingBase.GetLODFromTPCM((double) sd.trianglesPerCubicMeter);
            SpatialMappingBase sMComponentFromInFlightIndex = this.GetSMComponentFromInFlightIndex(inFlightIndex);
            if (outputWritten)
            {
                foreach (SMComponentRecord record in this.m_Components)
                {
                    if ((record.m_Component.lodType == lODFromTPCM) && (record.m_Component.bakePhysics == sd.bakeCollider))
                    {
                        record.m_OnDataReady(sMComponentFromInFlightIndex, sd, outputWritten, elapsedBakeTimeSeconds);
                    }
                }
            }
            else if (inFlightIndex != -1)
            {
                this.m_InFlightRequests[inFlightIndex].m_Requester.m_OnDataReady(sMComponentFromInFlightIndex, sd, outputWritten, elapsedBakeTimeSeconds);
            }
            else
            {
                Debug.LogError(string.Format("SpatialMappingContext unable to notify a component about a failure to cook surface {0}!", sd.id.handle));
            }
        }

        public void RegisterComponent(SpatialMappingBase smComponent, SpatialMappingBase.SurfaceDataReadyCallback onDataReady, GetHighestPriorityCallback getHighestPri, SurfaceObserver observer)
        {
            SMComponentRecord record2;
            <RegisterComponent>c__AnonStorey0 storey = new <RegisterComponent>c__AnonStorey0 {
                smComponent = smComponent
            };
            if (storey.smComponent == null)
            {
                throw new ArgumentNullException("smComponent");
            }
            if (onDataReady == null)
            {
                throw new ArgumentNullException("onDataReady");
            }
            if (getHighestPri == null)
            {
                throw new ArgumentNullException("getHighestPri");
            }
            if (observer == null)
            {
                throw new ArgumentNullException("observer");
            }
            if (this.m_Components.Find(new Predicate<SMComponentRecord>(storey.<>m__0)).m_Component != null)
            {
                throw new ArgumentException("RegisterComponent on a component already registered!");
            }
            record2.m_Component = storey.smComponent;
            record2.m_OnDataReady = onDataReady;
            record2.m_GetHighestPri = getHighestPri;
            record2.m_SurfaceObserver = observer;
            this.m_Components.Add(record2);
        }

        private void RequestMeshPriorityFromComponents()
        {
            if (this.m_InFlightSurfaces < 2)
            {
                for (int i = 0; i < this.m_Components.Count; i++)
                {
                    SurfaceData data;
                    SMComponentRecord item = this.m_Components[i];
                    if (item.m_GetHighestPri(out data))
                    {
                        if ((this.m_NextIndex == -1) || !this.m_InFlightRequests[this.m_NextIndex].IsClear())
                        {
                            Debug.LogError(string.Format("SMContext:  next index {0} may not be clear!", this.m_NextIndex));
                        }
                        else if (item.m_SurfaceObserver.RequestMeshAsync(data, new SurfaceObserver.SurfaceDataReadyDelegate(this.OnSurfaceDataReady)))
                        {
                            this.m_InFlightRequests[this.m_NextIndex].m_RequestData = data;
                            this.m_InFlightRequests[this.m_NextIndex].m_Requester = item;
                            this.m_InFlightSurfaces++;
                            this.m_NextIndex = (this.m_NextIndex != 1) ? 1 : 0;
                            this.m_Components.RemoveAt(i);
                            this.m_Components.Add(item);
                        }
                        else
                        {
                            Debug.LogError("SMContext:  unexpected failure requesting mesh bake!");
                        }
                        break;
                    }
                }
            }
        }

        private void UpdateInFlightRecords(int inFlightIndex, float elapsedBakeTimeSeconds)
        {
            if ((inFlightIndex == 0) || (inFlightIndex == 1))
            {
                if (this.m_InFlightSurfaces <= 0)
                {
                    Debug.LogError("SMContext:  unexpectedly got a data ready event with too few in flight surfaces!");
                }
                else
                {
                    this.m_InFlightSurfaces--;
                }
                this.m_InFlightRequests[inFlightIndex].Clear();
                if (!this.m_InFlightRequests[inFlightIndex].IsClear())
                {
                }
                this.m_NextIndex = inFlightIndex;
            }
            else
            {
                Debug.LogError(string.Format("SMContext:  unable to update in flight record for an invalid index {0}!", inFlightIndex));
            }
        }

        public static SpatialMappingContext Instance
        {
            get
            {
                return instance;
            }
        }

        [CompilerGenerated]
        private sealed class <DeregisterComponent>c__AnonStorey1
        {
            internal SpatialMappingBase smComponent;

            internal bool <>m__0(SpatialMappingContext.SMComponentRecord record)
            {
                return (record.m_Component == this.smComponent);
            }
        }

        [CompilerGenerated]
        private sealed class <RegisterComponent>c__AnonStorey0
        {
            internal SpatialMappingBase smComponent;

            internal bool <>m__0(SpatialMappingContext.SMComponentRecord record)
            {
                return (record.m_Component == this.smComponent);
            }
        }

        public delegate bool GetHighestPriorityCallback(out SurfaceData dataRequest);

        [StructLayout(LayoutKind.Sequential)]
        private struct SMBakeRequest
        {
            public SurfaceData m_RequestData;
            public SpatialMappingContext.SMComponentRecord m_Requester;
            public void Clear()
            {
                this.m_RequestData.id.handle = 0;
                this.m_Requester.Clear();
            }

            public bool IsClear()
            {
                return ((this.m_RequestData.id.handle == 0) && this.m_Requester.IsClear());
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SMComponentRecord
        {
            public SpatialMappingBase m_Component;
            public SpatialMappingBase.SurfaceDataReadyCallback m_OnDataReady;
            public SpatialMappingContext.GetHighestPriorityCallback m_GetHighestPri;
            public SurfaceObserver m_SurfaceObserver;
            private SMComponentRecord(SpatialMappingBase comp, SpatialMappingBase.SurfaceDataReadyCallback onDataReady, SpatialMappingContext.GetHighestPriorityCallback getHighestPri, SurfaceObserver observer)
            {
                this.m_Component = comp;
                this.m_OnDataReady = onDataReady;
                this.m_GetHighestPri = getHighestPri;
                this.m_SurfaceObserver = observer;
            }

            public void Clear()
            {
                this.m_Component = null;
                this.m_OnDataReady = null;
                this.m_GetHighestPri = null;
                this.m_SurfaceObserver = null;
            }

            public bool IsClear()
            {
                return ((((this.m_Component == null) && (this.m_OnDataReady == null)) && (this.m_GetHighestPri == null)) && (this.m_SurfaceObserver == null));
            }
        }
    }
}

