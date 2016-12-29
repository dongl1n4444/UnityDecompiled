namespace UnityEngine.VR.WSA
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Rendering;

    /// <summary>
    /// <para>Creates and manages the GameObjects that represent spatial surfaces.  A MeshFilter, MeshRenderer, and material will automatically be added to these GameObjects so that the spatial surface can be visualized.</para>
    /// </summary>
    [AddComponentMenu("AR/Spatial Mapping Renderer", 12)]
    public class SpatialMappingRenderer : SpatialMappingBase
    {
        [SerializeField]
        private RenderState m_CurrentRenderState = RenderState.Occlusion;
        [SerializeField]
        private Material m_OcclusionMaterial;
        [SerializeField]
        private Material m_VisualMaterial;

        protected void ApplyPropertiesToCachedSurfaces()
        {
            if ((base.surfaceObjects != null) && (base.surfaceObjects.Count != 0))
            {
                foreach (KeyValuePair<int, SpatialMappingBase.Surface> pair in base.surfaceObjects)
                {
                    if ((pair.Value.gameObject != null) && (pair.Value.meshRenderer != null))
                    {
                        this.ApplyRenderSettings(pair.Value.meshRenderer);
                    }
                }
                foreach (KeyValuePair<int, SpatialMappingBase.Surface> pair2 in base.pendingSurfacesForEviction)
                {
                    if (pair2.Value.meshRenderer != null)
                    {
                        this.ApplyRenderSettings(pair2.Value.meshRenderer);
                        if (base.ShouldRemainActiveWhileBeingRemoved(pair2.Value))
                        {
                            pair2.Value.meshRenderer.enabled = this.renderState != RenderState.None;
                        }
                    }
                }
            }
        }

        private void ApplyRenderSettings(MeshRenderer meshRenderer)
        {
            if (meshRenderer != null)
            {
                switch (this.renderState)
                {
                    case RenderState.Occlusion:
                        meshRenderer.sharedMaterial = this.occlusionMaterial;
                        meshRenderer.enabled = true;
                        break;

                    case RenderState.Visualization:
                        meshRenderer.sharedMaterial = this.visualMaterial;
                        meshRenderer.enabled = true;
                        break;

                    case RenderState.None:
                        meshRenderer.enabled = false;
                        break;
                }
            }
        }

        protected override void DestroySurface(SpatialMappingBase.Surface surface)
        {
            if (surface.meshRenderer != null)
            {
                surface.meshRenderer.sharedMaterial = null;
                surface.meshRenderer.enabled = false;
                UnityEngine.Object.Destroy(surface.meshRenderer);
                surface.meshRenderer = null;
            }
            base.DestroySurface(surface);
        }

        protected override void OnBeginSurfaceEviction(bool shouldBeActiveWhileRemoved, SpatialMappingBase.Surface surface)
        {
            if ((surface.gameObject != null) && (surface.meshRenderer != null))
            {
                surface.meshRenderer.enabled = shouldBeActiveWhileRemoved;
            }
        }

        protected override void OnDestroy()
        {
            this.occlusionMaterial = null;
            this.visualMaterial = null;
            base.OnDestroy();
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
                        Debug.LogError($"A SpatialMappingRenderer component can not apply baked data to a surface with id "{bakedData.id.handle}" because its GameObject is null.");
                    }
                    else
                    {
                        if (requester != this)
                        {
                            base.CloneBakedComponents(bakedData, surface);
                        }
                        if (surface.meshRenderer == null)
                        {
                            surface.meshRenderer = surface.gameObject.GetComponent<MeshRenderer>();
                            if (surface.meshRenderer == null)
                            {
                                surface.meshRenderer = surface.gameObject.AddComponent<MeshRenderer>();
                            }
                            surface.meshRenderer.receiveShadows = false;
                            surface.meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
                        }
                        this.ApplyRenderSettings(surface.meshRenderer);
                    }
                }
            }
        }

        protected override void Reset()
        {
            base.Reset();
        }

        /// <summary>
        /// <para>A Material applied to all surface meshes to enable occlusion of holograms by real world objects.</para>
        /// </summary>
        public Material occlusionMaterial
        {
            get => 
                this.m_OcclusionMaterial;
            set
            {
                this.m_OcclusionMaterial = value;
            }
        }

        /// <summary>
        /// <para>Controls which Material will be used when rendering a surface mesh.</para>
        /// </summary>
        public RenderState renderState
        {
            get => 
                this.m_CurrentRenderState;
            set
            {
                this.m_CurrentRenderState = value;
                this.ApplyPropertiesToCachedSurfaces();
            }
        }

        /// <summary>
        /// <para>A material that can be used to visualize the spatial surface.</para>
        /// </summary>
        public Material visualMaterial
        {
            get => 
                this.m_VisualMaterial;
            set
            {
                this.m_VisualMaterial = value;
            }
        }

        /// <summary>
        /// <para>Specifies how to render the spatial surfaces associated with the SpatialMappingRenderer component instance.</para>
        /// </summary>
        public enum RenderState
        {
            None,
            Occlusion,
            Visualization
        }
    }
}

