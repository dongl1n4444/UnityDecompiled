namespace UnityEngine.VR.WSA
{
    using System;
    using UnityEngine;

    /// <summary>
    /// <para>Creates and manages the GameObjects that represent spatial surfaces.  A MeshFilter and MeshCollider will automatically be added to these game objects so that holograms can interact with physical surfaces.</para>
    /// </summary>
    [AddComponentMenu("AR/Spatial Mapping Collider", 12)]
    public class SpatialMappingCollider : SpatialMappingBase
    {
        [SerializeField]
        private bool m_EnableCollisions = true;
        [SerializeField]
        private int m_Layer = 0;
        [SerializeField]
        private PhysicMaterial m_Material;

        protected override void AddRequiredComponentsForBaking(SpatialMappingBase.Surface surface)
        {
            base.AddRequiredComponentsForBaking(surface);
            if (surface.meshCollider == null)
            {
                surface.meshCollider = surface.gameObject.AddComponent<MeshCollider>();
            }
            SurfaceData surfaceData = surface.surfaceData;
            surfaceData.outputCollider = surface.meshCollider;
            surface.surfaceData = surfaceData;
        }

        protected void ApplyPropertiesToCachedSurfaces()
        {
            if (this.material != null)
            {
                base.ForEachSurfaceInCache(delegate (SpatialMappingBase.Surface surface) {
                    if (surface.meshCollider != null)
                    {
                        if ((surface.gameObject != null) && (surface.gameObject.layer != this.layer))
                        {
                            surface.gameObject.layer = this.layer;
                        }
                        if (surface.meshCollider.material != this.material)
                        {
                            surface.meshCollider.material = this.material;
                        }
                        if (surface.meshCollider.enabled != this.enableCollisions)
                        {
                            surface.meshCollider.enabled = this.enableCollisions;
                        }
                    }
                });
            }
        }

        protected override void Awake()
        {
            base.bakePhysics = true;
        }

        protected override void OnBeginSurfaceEviction(bool shouldBeActiveWhileRemoved, SpatialMappingBase.Surface surfaceData)
        {
            if ((surfaceData.gameObject != null) && (surfaceData.meshCollider != null))
            {
                surfaceData.meshCollider.enabled = shouldBeActiveWhileRemoved;
            }
        }

        protected override void OnResetProperties()
        {
            base.OnResetProperties();
            this.ApplyPropertiesToCachedSurfaces();
        }

        protected override void OnSurfaceDataReady(SpatialMappingBase requester, SurfaceData bakedData, bool outputWritten, float elapsedBakeTimeSeconds)
        {
            SpatialMappingBase.Surface surface;
            if (base.surfaceObjects.TryGetValue(bakedData.id.handle, out surface))
            {
                surface.awaitingBake = false;
                if (outputWritten)
                {
                    if (surface.gameObject == null)
                    {
                        Debug.LogError($"A SpatialMappingCollider component can not apply baked data to the surface with id "{bakedData.id.handle}" because its GameObject is null.");
                    }
                    else if (bakedData.outputCollider != null)
                    {
                        if (requester != this)
                        {
                            base.CloneBakedComponents(bakedData, surface);
                        }
                        bakedData.outputCollider.gameObject.layer = this.layer;
                        if (this.material != null)
                        {
                            bakedData.outputCollider.material = this.material;
                        }
                    }
                }
            }
        }

        protected override void UpdateSurfaceData(SpatialMappingBase.Surface surface)
        {
            base.UpdateSurfaceData(surface);
            SurfaceData surfaceData = surface.surfaceData;
            surfaceData.bakeCollider = base.bakePhysics;
            surfaceData.outputCollider = surface.meshCollider;
            surface.surfaceData = surfaceData;
        }

        /// <summary>
        /// <para>Enables/Disables MeshCollider components on all spatial surfaces associated with the SpatialMappingCollider component instance.</para>
        /// </summary>
        public bool enableCollisions
        {
            get => 
                this.m_EnableCollisions;
            set
            {
                this.m_EnableCollisions = value;
                this.ApplyPropertiesToCachedSurfaces();
            }
        }

        /// <summary>
        /// <para>Sets the layer on all spatial surfaces associated with the SpatialMappingCollider component instance.</para>
        /// </summary>
        public int layer
        {
            get => 
                this.m_Layer;
            set
            {
                this.m_Layer = value;
                this.ApplyPropertiesToCachedSurfaces();
            }
        }

        /// <summary>
        /// <para>Sets the PhysicMaterial on all spatial surfaces associated with the SpatialMappingCollider component instance.</para>
        /// </summary>
        public PhysicMaterial material
        {
            get => 
                this.m_Material;
            set
            {
                this.m_Material = value;
                this.ApplyPropertiesToCachedSurfaces();
            }
        }
    }
}

