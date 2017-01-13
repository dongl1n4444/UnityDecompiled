namespace UnityEngine.VR.WSA
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    /// <summary>
    /// <para>The base class for all spatial mapping components.</para>
    /// </summary>
    public abstract class SpatialMappingBase : MonoBehaviour
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Bounds <bounds>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Vector3 <lastUpdatedObserverPosition>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private float <nextSurfaceChangeUpdateTime>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int <observerId>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Camera <selectedCamera>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Dictionary<int, Surface> <surfaceObjects>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private SurfaceObserver <surfaceObserver>k__BackingField;
        private SurfaceData bestSurfaceDataNull = new SurfaceData();
        protected bool m_BakePhysics = false;
        [SerializeField]
        private bool m_FreezeUpdates = false;
        [SerializeField]
        private Vector3 m_HalfBoxExtents = ((Vector3) (Vector3.one * 4f));
        [SerializeField]
        private LODType m_LodType = LODType.Medium;
        [SerializeField]
        private int m_NumUpdatesBeforeRemoval = 10;
        private Dictionary<int, Surface> m_PendingSurfacesForEviction = new Dictionary<int, Surface>();
        [SerializeField]
        private float m_SecondsBetweenUpdates = 2.5f;
        [SerializeField]
        private float m_SphereRadius = 2f;
        [SerializeField]
        private GameObject m_SurfaceParent;
        protected bool m_SurfaceParentWasDynamicallyCreated = false;
        private List<int> m_SurfacesToRemoveFromDict = new List<int>();
        [SerializeField]
        private VolumeType m_VolumeType = VolumeType.AxisAlignedBox;
        private static readonly float s_EvictionUpdateTickThresholdSqr = 100f;
        private static readonly int[] s_LodToPcm = new int[] { 0x7d0, 750, 200 };
        private static readonly float s_MovementUpdateThresholdSqr = 0.0001f;
        private static int s_ObserverIdCounter = 0;

        protected SpatialMappingBase()
        {
        }

        protected virtual void AddRequiredComponentsForBaking(Surface surface)
        {
            if (this.surfaceParent == null)
            {
                this.surfaceParent = new GameObject($"Surface Parent{this.observerId}");
                this.surfaceParentWasDynamicallyCreated = true;
            }
            if (surface.gameObject == null)
            {
                surface.gameObject = new GameObject($"spatial-mapping-surface{this.observerId}_{surface.surfaceId.handle}");
                surface.gameObject.transform.parent = this.surfaceParent.transform;
            }
            if (surface.meshFilter == null)
            {
                surface.meshFilter = surface.gameObject.GetComponent<MeshFilter>();
                if (surface.meshFilter == null)
                {
                    surface.meshFilter = surface.gameObject.AddComponent<MeshFilter>();
                }
            }
            SurfaceData surfaceData = surface.surfaceData;
            surfaceData.outputMesh = surface.meshFilter;
            if (surface.worldAnchor == null)
            {
                surface.worldAnchor = surface.gameObject.GetComponent<WorldAnchor>();
                if (surface.worldAnchor == null)
                {
                    surface.worldAnchor = surface.gameObject.AddComponent<WorldAnchor>();
                }
            }
            surfaceData.outputAnchor = surface.worldAnchor;
            surface.surfaceData = surfaceData;
        }

        protected virtual void Awake()
        {
        }

        protected bool BoundsContains(Vector3 position)
        {
            if (this.volumeType == VolumeType.Sphere)
            {
                if (Vector3.SqrMagnitude(position - base.transform.position) <= (this.sphereRadius * this.sphereRadius))
                {
                    return true;
                }
            }
            else if (this.volumeType == VolumeType.AxisAlignedBox)
            {
                return this.bounds.Contains(position);
            }
            return false;
        }

        protected void CloneBakedComponents(SurfaceData bakedData, Surface target)
        {
            if ((target != null) && ((bakedData.outputMesh != null) && (target.meshFilter != null)))
            {
                UnityEngine.Object.Destroy(target.meshFilter.mesh);
                target.meshFilter.mesh = bakedData.outputMesh.sharedMesh;
            }
        }

        private Surface CreateSurface(SurfaceId surfaceId) => 
            new Surface { 
                surfaceId = surfaceId,
                awaitingBake = false
            };

        protected virtual void DestroySurface(Surface surface)
        {
            surface.remainingUpdatesToLive = -1;
            if ((surface.meshFilter != null) && (surface.meshFilter.mesh != null))
            {
                UnityEngine.Object.Destroy(surface.meshFilter.mesh);
            }
            UnityEngine.Object.Destroy(surface.gameObject);
            surface.gameObject = null;
        }

        protected void ForEachSurfaceInCache(Action<Surface> callback)
        {
            if ((callback != null) && ((this.surfaceObjects != null) && (this.surfaceObjects.Count != 0)))
            {
                foreach (KeyValuePair<int, Surface> pair in this.surfaceObjects)
                {
                    callback(pair.Value);
                }
                foreach (KeyValuePair<int, Surface> pair2 in this.pendingSurfacesForEviction)
                {
                    if (this.ShouldRemainActiveWhileBeingRemoved(pair2.Value))
                    {
                        callback(pair2.Value);
                    }
                }
            }
        }

        public static LODType GetLODFromTPCM(double trianglesPerCubicMeter)
        {
            if (trianglesPerCubicMeter >= 1999.0)
            {
                return LODType.High;
            }
            if ((trianglesPerCubicMeter >= 749.0) && (trianglesPerCubicMeter <= 751.0))
            {
                return LODType.Medium;
            }
            return LODType.Low;
        }

        private void OnAddOrUpdateSurface(SurfaceId surfaceId, DateTime updateTime, bool bakePhysics)
        {
            Surface surface = null;
            if (this.pendingSurfacesForEviction.ContainsKey(surfaceId.handle))
            {
                this.surfaceObjects[surfaceId.handle] = this.pendingSurfacesForEviction[surfaceId.handle];
                this.pendingSurfacesForEviction.Remove(surfaceId.handle);
            }
            else if (!this.surfaceObjects.ContainsKey(surfaceId.handle))
            {
                surface = this.CreateSurface(surfaceId);
                surface.surfaceData = new SurfaceData();
                this.surfaceObjects.Add(surfaceId.handle, surface);
            }
            if (surface == null)
            {
                surface = this.surfaceObjects[surfaceId.handle];
            }
            SurfaceData surfaceData = surface.surfaceData;
            surfaceData.id = surfaceId;
            surfaceData.bakeCollider = bakePhysics;
            surfaceData.trianglesPerCubicMeter = lodToPcm[(int) this.lodType];
            surface.surfaceData = surfaceData;
            surface.awaitingBake = true;
            surface.updateTime = updateTime;
            this.AddRequiredComponentsForBaking(surface);
        }

        protected abstract void OnBeginSurfaceEviction(bool shouldBeActiveWhileRemoved, Surface surface);
        protected virtual void OnDestroy()
        {
            SpatialMappingContext.Instance.DeregisterComponent(this);
            if ((this.surfaceObjects != null) && (this.surfaceObjects.Count > 0))
            {
                foreach (KeyValuePair<int, Surface> pair in this.surfaceObjects)
                {
                    this.DestroySurface(pair.Value);
                }
                this.surfaceObjects.Clear();
            }
            if ((this.pendingSurfacesForEviction != null) && (this.pendingSurfacesForEviction.Count > 0))
            {
                foreach (KeyValuePair<int, Surface> pair2 in this.pendingSurfacesForEviction)
                {
                    if (pair2.Value.gameObject != null)
                    {
                        this.DestroySurface(pair2.Value);
                    }
                }
                this.pendingSurfacesForEviction.Clear();
            }
            if (this.surfaceParentWasDynamicallyCreated)
            {
                UnityEngine.Object.Destroy(this.surfaceParent);
                this.surfaceParent = null;
            }
            this.surfaceObserver.Dispose();
            this.surfaceObserver = null;
        }

        protected virtual void OnDidApplyAnimationProperties()
        {
            this.OnResetProperties();
        }

        protected virtual void OnDisable()
        {
            if ((this.surfaceObjects != null) && (this.surfaceObjects.Count > 0))
            {
                foreach (KeyValuePair<int, Surface> pair in this.surfaceObjects)
                {
                    if (pair.Value.gameObject != null)
                    {
                        pair.Value.gameObject.SetActive(false);
                    }
                }
            }
            if ((this.pendingSurfacesForEviction != null) && (this.pendingSurfacesForEviction.Count > 0))
            {
                foreach (KeyValuePair<int, Surface> pair2 in this.pendingSurfacesForEviction)
                {
                    if (pair2.Value.gameObject != null)
                    {
                        pair2.Value.gameObject.SetActive(false);
                    }
                }
            }
        }

        protected virtual void OnEnable()
        {
            if ((this.surfaceObjects != null) && (this.surfaceObjects.Count > 0))
            {
                foreach (KeyValuePair<int, Surface> pair in this.surfaceObjects)
                {
                    if (pair.Value.gameObject != null)
                    {
                        pair.Value.gameObject.SetActive(true);
                    }
                }
            }
            if ((this.pendingSurfacesForEviction != null) && (this.pendingSurfacesForEviction.Count > 0))
            {
                foreach (KeyValuePair<int, Surface> pair2 in this.pendingSurfacesForEviction)
                {
                    if (pair2.Value.gameObject == null)
                    {
                        Debug.LogWarning($"Can not activate the surface id "{pair2.Key}" because its GameObject is null.");
                    }
                    else
                    {
                        pair2.Value.gameObject.SetActive(true);
                    }
                }
            }
        }

        protected void OnRemoveSurface(SurfaceId surfaceId)
        {
            if ((this.surfaceObjects != null) && (this.surfaceObjects.Count != 0))
            {
                Surface surface;
                if (!this.surfaceObjects.TryGetValue(surfaceId.handle, out surface))
                {
                    Debug.LogWarning($"Can not remove the surface id "{surfaceId.handle}" because it is not an active surface.");
                }
                else
                {
                    this.surfaceObjects.Remove(surfaceId.handle);
                    if (this.numUpdatesBeforeRemoval < 1)
                    {
                        this.DestroySurface(surface);
                    }
                    else
                    {
                        this.OnBeginSurfaceEviction(this.ShouldRemainActiveWhileBeingRemoved(surface), surface);
                        surface.remainingUpdatesToLive = this.numUpdatesBeforeRemoval + 1;
                        this.pendingSurfacesForEviction.Add(surfaceId.handle, surface);
                    }
                }
            }
        }

        protected virtual void OnResetProperties()
        {
        }

        private void OnSurfaceChanged(SurfaceId surfaceId, SurfaceChange changeType, Bounds bounds, DateTime updateTime)
        {
            switch (changeType)
            {
                case SurfaceChange.Added:
                case SurfaceChange.Updated:
                    this.OnAddOrUpdateSurface(surfaceId, updateTime, this.bakePhysics);
                    break;

                case SurfaceChange.Removed:
                    this.OnRemoveSurface(surfaceId);
                    break;
            }
        }

        /// <summary>
        /// <para>This method will be called by the system when the surface data has been generated.</para>
        /// </summary>
        /// <param name="requester"></param>
        /// <param name="bakedData"></param>
        /// <param name="outputWritten"></param>
        /// <param name="elapsedBakeTimeSeconds"></param>
        protected abstract void OnSurfaceDataReady(SpatialMappingBase requester, SurfaceData bakedData, bool outputWritten, float elapsedBakeTimeSeconds);
        protected virtual void OnValidate()
        {
            this.OnResetProperties();
        }

        protected void ProcessEvictedObjects()
        {
            if ((this.pendingSurfacesForEviction != null) && (this.pendingSurfacesForEviction.Count != 0))
            {
                this.surfacesToRemoveFromDict.Clear();
                foreach (KeyValuePair<int, Surface> pair in this.pendingSurfacesForEviction)
                {
                    if (pair.Value.gameObject == null)
                    {
                        this.surfacesToRemoveFromDict.Add(pair.Key);
                    }
                    else
                    {
                        Surface surface = pair.Value;
                        Vector3 position = surface.gameObject.transform.position;
                        if (!this.BoundsContains(position) || (Vector3.SqrMagnitude(position - base.transform.position) <= s_EvictionUpdateTickThresholdSqr))
                        {
                            int num;
                            surface.remainingUpdatesToLive = (num = surface.remainingUpdatesToLive) - 1;
                            if (num <= 0)
                            {
                                this.DestroySurface(surface);
                                this.surfacesToRemoveFromDict.Add(pair.Key);
                            }
                        }
                    }
                }
                for (int i = 0; i < this.surfacesToRemoveFromDict.Count; i++)
                {
                    this.pendingSurfacesForEviction.Remove(this.surfacesToRemoveFromDict[i]);
                }
            }
        }

        protected virtual void Reset()
        {
            this.OnResetProperties();
        }

        protected bool ShouldRemainActiveWhileBeingRemoved(Surface surface)
        {
            if (surface.gameObject == null)
            {
                return false;
            }
            GameObject gameObject = this.selectedCamera.gameObject;
            bool flag2 = surface.gameObject == gameObject;
            for (Transform transform = surface.gameObject.transform.parent; !flag2 && (transform != null); transform = transform.parent)
            {
                if (transform.gameObject == gameObject)
                {
                    flag2 = true;
                    break;
                }
            }
            if (flag2)
            {
                return false;
            }
            if (this.BoundsContains(surface.gameObject.transform.position))
            {
                return false;
            }
            return true;
        }

        protected virtual void Start()
        {
            this.observerId = s_ObserverIdCounter++;
            this.surfaceObjects = new Dictionary<int, Surface>();
            this.selectedCamera = Camera.main;
            this.nextSurfaceChangeUpdateTime = float.MinValue;
            this.surfaceObserver = new SurfaceObserver();
            SpatialMappingContext.Instance.RegisterComponent(this, new SurfaceDataReadyCallback(this.OnSurfaceDataReady), new SpatialMappingContext.GetHighestPriorityCallback(this.TryGetHighestPriorityRequest), this.surfaceObserver);
            this.bounds = new Bounds(base.transform.position, this.halfBoxExtents);
            this.UpdatePosition();
        }

        protected virtual bool TryGetHighestPriorityRequest(out SurfaceData bestSurfaceData)
        {
            bestSurfaceData = this.bestSurfaceDataNull;
            if ((this.surfaceObjects == null) || (this.surfaceObjects.Count == 0))
            {
                return false;
            }
            Surface surface = null;
            foreach (KeyValuePair<int, Surface> pair in this.surfaceObjects)
            {
                if (pair.Value.awaitingBake)
                {
                    if (surface == null)
                    {
                        surface = pair.Value;
                    }
                    else if (pair.Value.updateTime < surface.updateTime)
                    {
                        surface = pair.Value;
                    }
                }
            }
            if (surface == null)
            {
                return false;
            }
            this.AddRequiredComponentsForBaking(surface);
            this.UpdateSurfaceData(surface);
            bestSurfaceData = surface.surfaceData;
            return true;
        }

        protected virtual void Update()
        {
            if (Vector3.SqrMagnitude(this.lastUpdatedObserverPosition - base.transform.position) > s_MovementUpdateThresholdSqr)
            {
                this.UpdatePosition();
            }
            if (!this.freezeUpdates && (Time.time >= this.nextSurfaceChangeUpdateTime))
            {
                this.surfaceObserver.Update(new SurfaceObserver.SurfaceChangedDelegate(this.OnSurfaceChanged));
                this.ProcessEvictedObjects();
                this.nextSurfaceChangeUpdateTime = Time.time + this.secondsBetweenUpdates;
                SpatialMappingContext.Instance.ComponentHasDataRequests();
            }
        }

        protected void UpdatePosition()
        {
            if (this.volumeType == VolumeType.Sphere)
            {
                this.surfaceObserver.SetVolumeAsSphere(base.transform.position, this.sphereRadius);
            }
            else if (this.volumeType == VolumeType.AxisAlignedBox)
            {
                this.surfaceObserver.SetVolumeAsAxisAlignedBox(base.transform.position, this.halfBoxExtents);
                Bounds bounds = this.bounds;
                bounds.center = base.transform.position;
                bounds.extents = this.halfBoxExtents;
                this.bounds = bounds;
            }
            this.lastUpdatedObserverPosition = base.transform.position;
        }

        protected virtual void UpdateSurfaceData(Surface surface)
        {
            SurfaceData surfaceData = surface.surfaceData;
            surfaceData.id = surface.surfaceId;
            surfaceData.trianglesPerCubicMeter = lodToPcm[(int) this.lodType];
            surfaceData.bakeCollider = false;
            surfaceData.outputMesh = surface.meshFilter;
            surface.surfaceData = surfaceData;
        }

        /// <summary>
        /// <para>This property specifies whether or not collision data will be generated when computing the surface mesh's triangulation. (Read Only)</para>
        /// </summary>
        public bool bakePhysics
        {
            get => 
                this.m_BakePhysics;
            protected set
            {
                this.m_BakePhysics = value;
            }
        }

        protected Bounds bounds { get; set; }

        /// <summary>
        /// <para>Specifies if the component should listen and respond to spatial surface changes.</para>
        /// </summary>
        public bool freezeUpdates
        {
            get => 
                this.m_FreezeUpdates;
            set
            {
                this.m_FreezeUpdates = value;
            }
        }

        /// <summary>
        /// <para>The half extents of the bounding box from its center.</para>
        /// </summary>
        public Vector3 halfBoxExtents
        {
            get => 
                this.m_HalfBoxExtents;
            set
            {
                this.m_HalfBoxExtents = value;
            }
        }

        protected Vector3 lastUpdatedObserverPosition { get; set; }

        protected static int[] lodToPcm =>
            s_LodToPcm;

        /// <summary>
        /// <para>The level of detail that should be used when generating surface meshes.</para>
        /// </summary>
        public LODType lodType
        {
            get => 
                this.m_LodType;
            set
            {
                this.m_LodType = value;
            }
        }

        protected float nextSurfaceChangeUpdateTime { get; set; }

        /// <summary>
        /// <para>The number of frames to keep a surface mesh alive for before destroying it after the system has determined that its real world counterpart has been removed.</para>
        /// </summary>
        public int numUpdatesBeforeRemoval
        {
            get => 
                this.m_NumUpdatesBeforeRemoval;
            set
            {
                this.m_NumUpdatesBeforeRemoval = value;
            }
        }

        protected int observerId { get; set; }

        protected Dictionary<int, Surface> pendingSurfacesForEviction
        {
            get => 
                this.m_PendingSurfacesForEviction;
            set
            {
                this.m_PendingSurfacesForEviction = value;
            }
        }

        /// <summary>
        /// <para>The time in seconds between system queries for changes in physical space.</para>
        /// </summary>
        public float secondsBetweenUpdates
        {
            get => 
                this.m_SecondsBetweenUpdates;
            set
            {
                this.m_SecondsBetweenUpdates = value;
            }
        }

        protected Camera selectedCamera { get; set; }

        /// <summary>
        /// <para>The radius of the bounding sphere volume.</para>
        /// </summary>
        public float sphereRadius
        {
            get => 
                this.m_SphereRadius;
            set
            {
                this.m_SphereRadius = value;
            }
        }

        protected Dictionary<int, Surface> surfaceObjects { get; set; }

        protected SurfaceObserver surfaceObserver { get; set; }

        /// <summary>
        /// <para>The GameObject that should be the parent of all the component's surface mesh game objects.</para>
        /// </summary>
        public GameObject surfaceParent
        {
            get => 
                this.m_SurfaceParent;
            set
            {
                this.m_SurfaceParent = value;
            }
        }

        protected bool surfaceParentWasDynamicallyCreated
        {
            get => 
                this.m_SurfaceParentWasDynamicallyCreated;
            set
            {
                this.m_SurfaceParentWasDynamicallyCreated = value;
            }
        }

        protected List<int> surfacesToRemoveFromDict
        {
            get => 
                this.m_SurfacesToRemoveFromDict;
            set
            {
                this.m_SurfacesToRemoveFromDict = value;
            }
        }

        /// <summary>
        /// <para>The surface observer volume to use when querying the system for changes in physical space.</para>
        /// </summary>
        public VolumeType volumeType
        {
            get => 
                this.m_VolumeType;
            set
            {
                this.m_VolumeType = value;
            }
        }

        /// <summary>
        /// <para>The number of triangles per cubic meter or level of detail that should be used when creating a surface mesh.</para>
        /// </summary>
        public enum LODType
        {
            High,
            Medium,
            Low
        }

        public class Surface
        {
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private bool <awaitingBake>k__BackingField;
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private GameObject <gameObject>k__BackingField;
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private MeshCollider <meshCollider>k__BackingField;
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private MeshFilter <meshFilter>k__BackingField;
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private MeshRenderer <meshRenderer>k__BackingField;
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private int <remainingUpdatesToLive>k__BackingField;
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private SurfaceData <surfaceData>k__BackingField;
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private SurfaceId <surfaceId>k__BackingField;
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private DateTime <updateTime>k__BackingField;
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private WorldAnchor <worldAnchor>k__BackingField;

            public bool awaitingBake { get; set; }

            public GameObject gameObject { get; set; }

            public MeshCollider meshCollider { get; set; }

            public MeshFilter meshFilter { get; set; }

            public MeshRenderer meshRenderer { get; set; }

            public int remainingUpdatesToLive { get; set; }

            public SurfaceData surfaceData { get; set; }

            public SurfaceId surfaceId { get; set; }

            public DateTime updateTime { get; set; }

            public WorldAnchor worldAnchor { get; set; }
        }

        public delegate void SurfaceDataReadyCallback(SpatialMappingBase requester, SurfaceData bakedData, bool outputWritten, float elapsedBakeTimeSeconds);

        /// <summary>
        /// <para>The surface observer volume type to use when querying the system for changes in physical space.</para>
        /// </summary>
        public enum VolumeType
        {
            Sphere,
            AxisAlignedBox
        }
    }
}

