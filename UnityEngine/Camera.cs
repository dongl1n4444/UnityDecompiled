namespace UnityEngine
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Internal;
    using UnityEngine.Rendering;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>A Camera is a device through which the player views the world.</para>
    /// </summary>
    [UsedByNativeCode]
    public sealed class Camera : Behaviour
    {
        /// <summary>
        /// <para>Event that is fired after any camera finishes rendering.</para>
        /// </summary>
        public static CameraCallback onPostRender;
        /// <summary>
        /// <para>Event that is fired before any camera starts culling.</para>
        /// </summary>
        public static CameraCallback onPreCull;
        /// <summary>
        /// <para>Event that is fired before any camera starts rendering.</para>
        /// </summary>
        public static CameraCallback onPreRender;

        /// <summary>
        /// <para>Add a command buffer to be executed at a specified place.</para>
        /// </summary>
        /// <param name="evt">When to execute the command buffer during rendering.</param>
        /// <param name="buffer">The buffer to execute.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void AddCommandBuffer(CameraEvent evt, CommandBuffer buffer);
        public void CalculateFrustumCorners(Rect viewport, float z, MonoOrStereoscopicEye eye, Vector3[] outCorners)
        {
            if (outCorners == null)
            {
                throw new ArgumentNullException("outCorners");
            }
            if (outCorners.Length < 4)
            {
                throw new ArgumentException("outCorners minimum size is 4", "outCorners");
            }
            this.CalculateFrustumCornersInternal(viewport, z, eye, outCorners);
        }

        private void CalculateFrustumCornersInternal(Rect viewport, float z, MonoOrStereoscopicEye eye, Vector3[] outCorners)
        {
            INTERNAL_CALL_CalculateFrustumCornersInternal(this, ref viewport, z, eye, outCorners);
        }

        /// <summary>
        /// <para>Calculates and returns oblique near-plane projection matrix.</para>
        /// </summary>
        /// <param name="clipPlane">Vector4 that describes a clip plane.</param>
        /// <returns>
        /// <para>Oblique near-plane projection matrix.</para>
        /// </returns>
        public Matrix4x4 CalculateObliqueMatrix(Vector4 clipPlane)
        {
            Matrix4x4 matrixx;
            INTERNAL_CALL_CalculateObliqueMatrix(this, ref clipPlane, out matrixx);
            return matrixx;
        }

        /// <summary>
        /// <para>Makes this camera's settings match other camera.</para>
        /// </summary>
        /// <param name="other"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void CopyFrom(Camera other);
        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Camera.DoClear has been deprecated (UnityUpgradable).", true)]
        public void DoClear()
        {
        }

        [RequiredByNativeCode]
        private static void FireOnPostRender(Camera cam)
        {
            if (onPostRender != null)
            {
                onPostRender(cam);
            }
        }

        [RequiredByNativeCode]
        private static void FireOnPreCull(Camera cam)
        {
            if (onPreCull != null)
            {
                onPreCull(cam);
            }
        }

        [RequiredByNativeCode]
        private static void FireOnPreRender(Camera cam)
        {
            if (onPreRender != null)
            {
                onPreRender(cam);
            }
        }

        /// <summary>
        /// <para>Fills an array of Camera with the current cameras in the scene, without allocating a new array.</para>
        /// </summary>
        /// <param name="cameras">An array to be filled up with cameras currently in the scene.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern int GetAllCameras(Camera[] cameras);
        /// <summary>
        /// <para>Get command buffers to be executed at a specified place.</para>
        /// </summary>
        /// <param name="evt">When to execute the command buffer during rendering.</param>
        /// <returns>
        /// <para>Array of command buffers.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern CommandBuffer[] GetCommandBuffers(CameraEvent evt);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern string[] GetHDRWarnings();
        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Property GetScreenHeight() has been deprecated. Use Screen.height instead (UnityUpgradable) -> Screen.height", true)]
        public float GetScreenHeight()
        {
            return 0f;
        }

        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Property GetScreenWidth() has been deprecated. Use Screen.width instead (UnityUpgradable) -> Screen.width", true)]
        public float GetScreenWidth()
        {
            return 0f;
        }

        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("GetStereoProjectionMatrices is deprecated. Use GetStereoProjectionMatrix(StereoscopicEye eye) instead.")]
        public extern Matrix4x4[] GetStereoProjectionMatrices();
        public Matrix4x4 GetStereoProjectionMatrix(StereoscopicEye eye)
        {
            Matrix4x4 matrixx;
            INTERNAL_CALL_GetStereoProjectionMatrix(this, eye, out matrixx);
            return matrixx;
        }

        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("GetStereoViewMatrices is deprecated. Use GetStereoViewMatrix(StereoscopicEye eye) instead.")]
        public extern Matrix4x4[] GetStereoViewMatrices();
        public Matrix4x4 GetStereoViewMatrix(StereoscopicEye eye)
        {
            Matrix4x4 matrixx;
            INTERNAL_CALL_GetStereoViewMatrix(this, eye, out matrixx);
            return matrixx;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_CalculateFrustumCornersInternal(Camera self, ref Rect viewport, float z, MonoOrStereoscopicEye eye, Vector3[] outCorners);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_CalculateObliqueMatrix(Camera self, ref Vector4 clipPlane, out Matrix4x4 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_GetStereoProjectionMatrix(Camera self, StereoscopicEye eye, out Matrix4x4 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_GetStereoViewMatrix(Camera self, StereoscopicEye eye, out Matrix4x4 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern GameObject INTERNAL_CALL_RaycastTry(Camera self, ref Ray ray, float distance, int layerMask, QueryTriggerInteraction queryTriggerInteraction);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern GameObject INTERNAL_CALL_RaycastTry2D(Camera self, ref Ray ray, float distance, int layerMask);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_ResetAspect(Camera self);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_ResetCullingMatrix(Camera self);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_ResetFieldOfView(Camera self);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_ResetProjectionMatrix(Camera self);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_ResetReplacementShader(Camera self);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_ResetWorldToCameraMatrix(Camera self);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_ScreenPointToRay(Camera self, ref Vector3 position, out Ray value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_ScreenToViewportPoint(Camera self, ref Vector3 position, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_ScreenToWorldPoint(Camera self, ref Vector3 position, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_SetStereoProjectionMatrices(Camera self, ref Matrix4x4 leftMatrix, ref Matrix4x4 rightMatrix);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_SetStereoProjectionMatrix(Camera self, StereoscopicEye eye, ref Matrix4x4 matrix);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_SetStereoViewMatrices(Camera self, ref Matrix4x4 leftMatrix, ref Matrix4x4 rightMatrix);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_SetStereoViewMatrix(Camera self, StereoscopicEye eye, ref Matrix4x4 matrix);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_ViewportPointToRay(Camera self, ref Vector3 position, out Ray value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_ViewportToScreenPoint(Camera self, ref Vector3 position, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_ViewportToWorldPoint(Camera self, ref Vector3 position, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_WorldToScreenPoint(Camera self, ref Vector3 position, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_WorldToViewportPoint(Camera self, ref Vector3 position, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_backgroundColor(out Color value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_cameraToWorldMatrix(out Matrix4x4 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_cullingMatrix(out Matrix4x4 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_nonJitteredProjectionMatrix(out Matrix4x4 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_pixelRect(out Rect value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_projectionMatrix(out Matrix4x4 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_rect(out Rect value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_velocity(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_worldToCameraMatrix(out Matrix4x4 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern bool Internal_RenderToCubemapRT(RenderTexture cubemap, int faceMask);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern bool Internal_RenderToCubemapTexture(Cubemap cubemap, int faceMask);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_backgroundColor(ref Color value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_cullingMatrix(ref Matrix4x4 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_nonJitteredProjectionMatrix(ref Matrix4x4 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_pixelRect(ref Rect value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_projectionMatrix(ref Matrix4x4 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_rect(ref Rect value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_worldToCameraMatrix(ref Matrix4x4 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern bool IsFiltered(GameObject go);
        internal void OnlyUsedForTesting1()
        {
        }

        internal void OnlyUsedForTesting2()
        {
        }

        [ExcludeFromDocs]
        internal GameObject RaycastTry(Ray ray, float distance, int layerMask)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            return INTERNAL_CALL_RaycastTry(this, ref ray, distance, layerMask, useGlobal);
        }

        internal GameObject RaycastTry(Ray ray, float distance, int layerMask, [UnityEngine.Internal.DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
        {
            return INTERNAL_CALL_RaycastTry(this, ref ray, distance, layerMask, queryTriggerInteraction);
        }

        internal GameObject RaycastTry2D(Ray ray, float distance, int layerMask)
        {
            return INTERNAL_CALL_RaycastTry2D(this, ref ray, distance, layerMask);
        }

        /// <summary>
        /// <para>Remove all command buffers set on this camera.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void RemoveAllCommandBuffers();
        /// <summary>
        /// <para>Remove command buffer from execution at a specified place.</para>
        /// </summary>
        /// <param name="evt">When to execute the command buffer during rendering.</param>
        /// <param name="buffer">The buffer to execute.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void RemoveCommandBuffer(CameraEvent evt, CommandBuffer buffer);
        /// <summary>
        /// <para>Remove command buffers from execution at a specified place.</para>
        /// </summary>
        /// <param name="evt">When to execute the command buffer during rendering.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void RemoveCommandBuffers(CameraEvent evt);
        /// <summary>
        /// <para>Render the camera manually.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void Render();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void RenderDontRestore();
        [ExcludeFromDocs]
        public bool RenderToCubemap(Cubemap cubemap)
        {
            int faceMask = 0x3f;
            return this.RenderToCubemap(cubemap, faceMask);
        }

        [ExcludeFromDocs]
        public bool RenderToCubemap(RenderTexture cubemap)
        {
            int faceMask = 0x3f;
            return this.RenderToCubemap(cubemap, faceMask);
        }

        /// <summary>
        /// <para>Render into a static cubemap from this camera.</para>
        /// </summary>
        /// <param name="cubemap">The cube map to render to.</param>
        /// <param name="faceMask">A bitmask which determines which of the six faces are rendered to.</param>
        /// <returns>
        /// <para>False is rendering fails, else true.</para>
        /// </returns>
        public bool RenderToCubemap(Cubemap cubemap, [UnityEngine.Internal.DefaultValue("63")] int faceMask)
        {
            return this.Internal_RenderToCubemapTexture(cubemap, faceMask);
        }

        /// <summary>
        /// <para>Render into a cubemap from this camera.</para>
        /// </summary>
        /// <param name="faceMask">A bitfield indicating which cubemap faces should be rendered into.</param>
        /// <param name="cubemap">The texture to render to.</param>
        /// <returns>
        /// <para>False is rendering fails, else true.</para>
        /// </returns>
        public bool RenderToCubemap(RenderTexture cubemap, [UnityEngine.Internal.DefaultValue("63")] int faceMask)
        {
            return this.Internal_RenderToCubemapRT(cubemap, faceMask);
        }

        /// <summary>
        /// <para>Render the camera with shader replacement.</para>
        /// </summary>
        /// <param name="shader"></param>
        /// <param name="replacementTag"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void RenderWithShader(Shader shader, string replacementTag);
        /// <summary>
        /// <para>Revert the aspect ratio to the screen's aspect ratio.</para>
        /// </summary>
        public void ResetAspect()
        {
            INTERNAL_CALL_ResetAspect(this);
        }

        /// <summary>
        /// <para>Make culling queries reflect the camera's built in parameters.</para>
        /// </summary>
        public void ResetCullingMatrix()
        {
            INTERNAL_CALL_ResetCullingMatrix(this);
        }

        /// <summary>
        /// <para>Reset to the default field of view.</para>
        /// </summary>
        public void ResetFieldOfView()
        {
            INTERNAL_CALL_ResetFieldOfView(this);
        }

        /// <summary>
        /// <para>Make the projection reflect normal camera's parameters.</para>
        /// </summary>
        public void ResetProjectionMatrix()
        {
            INTERNAL_CALL_ResetProjectionMatrix(this);
        }

        /// <summary>
        /// <para>Remove shader replacement from camera.</para>
        /// </summary>
        public void ResetReplacementShader()
        {
            INTERNAL_CALL_ResetReplacementShader(this);
        }

        /// <summary>
        /// <para>Use the default projection matrix for both stereo eye. Only work in 3D flat panel display.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void ResetStereoProjectionMatrices();
        /// <summary>
        /// <para>Use the default view matrix for both stereo eye. Only work in 3D flat panel display.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void ResetStereoViewMatrices();
        /// <summary>
        /// <para>Make the rendering position reflect the camera's position in the scene.</para>
        /// </summary>
        public void ResetWorldToCameraMatrix()
        {
            INTERNAL_CALL_ResetWorldToCameraMatrix(this);
        }

        /// <summary>
        /// <para>Returns a ray going from camera through a screen point.</para>
        /// </summary>
        /// <param name="position"></param>
        public Ray ScreenPointToRay(Vector3 position)
        {
            Ray ray;
            INTERNAL_CALL_ScreenPointToRay(this, ref position, out ray);
            return ray;
        }

        /// <summary>
        /// <para>Transforms position from screen space into viewport space.</para>
        /// </summary>
        /// <param name="position"></param>
        public Vector3 ScreenToViewportPoint(Vector3 position)
        {
            Vector3 vector;
            INTERNAL_CALL_ScreenToViewportPoint(this, ref position, out vector);
            return vector;
        }

        /// <summary>
        /// <para>Transforms position from screen space into world space.</para>
        /// </summary>
        /// <param name="position"></param>
        public Vector3 ScreenToWorldPoint(Vector3 position)
        {
            Vector3 vector;
            INTERNAL_CALL_ScreenToWorldPoint(this, ref position, out vector);
            return vector;
        }

        /// <summary>
        /// <para>Make the camera render with shader replacement.</para>
        /// </summary>
        /// <param name="shader"></param>
        /// <param name="replacementTag"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void SetReplacementShader(Shader shader, string replacementTag);
        /// <summary>
        /// <para>Set custom projection matrices for both eyes.</para>
        /// </summary>
        /// <param name="leftMatrix">Projection matrix for the stereo left eye.</param>
        /// <param name="rightMatrix">Projection matrix for the stereo right eye.</param>
        [Obsolete("SetStereoProjectionMatrices is deprecated. Use SetStereoProjectionMatrix(StereoscopicEye eye) instead.")]
        public void SetStereoProjectionMatrices(Matrix4x4 leftMatrix, Matrix4x4 rightMatrix)
        {
            INTERNAL_CALL_SetStereoProjectionMatrices(this, ref leftMatrix, ref rightMatrix);
        }

        public void SetStereoProjectionMatrix(StereoscopicEye eye, Matrix4x4 matrix)
        {
            INTERNAL_CALL_SetStereoProjectionMatrix(this, eye, ref matrix);
        }

        /// <summary>
        /// <para>Set custom view matrices for both eyes.</para>
        /// </summary>
        /// <param name="leftMatrix">View matrix for the stereo left eye.</param>
        /// <param name="rightMatrix">View matrix for the stereo right eye.</param>
        [Obsolete("SetStereoViewMatrices is deprecated. Use SetStereoViewMatrix(StereoscopicEye eye) instead.")]
        public void SetStereoViewMatrices(Matrix4x4 leftMatrix, Matrix4x4 rightMatrix)
        {
            INTERNAL_CALL_SetStereoViewMatrices(this, ref leftMatrix, ref rightMatrix);
        }

        public void SetStereoViewMatrix(StereoscopicEye eye, Matrix4x4 matrix)
        {
            INTERNAL_CALL_SetStereoViewMatrix(this, eye, ref matrix);
        }

        /// <summary>
        /// <para>Sets the Camera to render to the chosen buffers of one or more RenderTextures.</para>
        /// </summary>
        /// <param name="colorBuffer">The RenderBuffer(s) to which color information will be rendered.</param>
        /// <param name="depthBuffer">The RenderBuffer to which depth information will be rendered.</param>
        public void SetTargetBuffers(RenderBuffer colorBuffer, RenderBuffer depthBuffer)
        {
            this.SetTargetBuffersImpl(out colorBuffer, out depthBuffer);
        }

        /// <summary>
        /// <para>Sets the Camera to render to the chosen buffers of one or more RenderTextures.</para>
        /// </summary>
        /// <param name="colorBuffer">The RenderBuffer(s) to which color information will be rendered.</param>
        /// <param name="depthBuffer">The RenderBuffer to which depth information will be rendered.</param>
        public void SetTargetBuffers(RenderBuffer[] colorBuffer, RenderBuffer depthBuffer)
        {
            this.SetTargetBuffersMRTImpl(colorBuffer, out depthBuffer);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void SetTargetBuffersImpl(out RenderBuffer color, out RenderBuffer depth);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void SetTargetBuffersMRTImpl(RenderBuffer[] color, out RenderBuffer depth);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void SetupCurrent(Camera cur);
        /// <summary>
        /// <para>Returns a ray going from camera through a viewport point.</para>
        /// </summary>
        /// <param name="position"></param>
        public Ray ViewportPointToRay(Vector3 position)
        {
            Ray ray;
            INTERNAL_CALL_ViewportPointToRay(this, ref position, out ray);
            return ray;
        }

        /// <summary>
        /// <para>Transforms position from viewport space into screen space.</para>
        /// </summary>
        /// <param name="position"></param>
        public Vector3 ViewportToScreenPoint(Vector3 position)
        {
            Vector3 vector;
            INTERNAL_CALL_ViewportToScreenPoint(this, ref position, out vector);
            return vector;
        }

        /// <summary>
        /// <para>Transforms position from viewport space into world space.</para>
        /// </summary>
        /// <param name="position">The 3d vector in Viewport space.</param>
        /// <returns>
        /// <para>The 3d vector in World space.</para>
        /// </returns>
        public Vector3 ViewportToWorldPoint(Vector3 position)
        {
            Vector3 vector;
            INTERNAL_CALL_ViewportToWorldPoint(this, ref position, out vector);
            return vector;
        }

        /// <summary>
        /// <para>Transforms position from world space into screen space.</para>
        /// </summary>
        /// <param name="position"></param>
        public Vector3 WorldToScreenPoint(Vector3 position)
        {
            Vector3 vector;
            INTERNAL_CALL_WorldToScreenPoint(this, ref position, out vector);
            return vector;
        }

        /// <summary>
        /// <para>Transforms position from world space into viewport space.</para>
        /// </summary>
        /// <param name="position"></param>
        public Vector3 WorldToViewportPoint(Vector3 position)
        {
            Vector3 vector;
            INTERNAL_CALL_WorldToViewportPoint(this, ref position, out vector);
            return vector;
        }

        /// <summary>
        /// <para>The rendering path that is currently being used (Read Only).</para>
        /// </summary>
        public RenderingPath actualRenderingPath { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Returns all enabled cameras in the scene.</para>
        /// </summary>
        public static Camera[] allCameras { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>The number of cameras in the current scene.</para>
        /// </summary>
        public static int allCamerasCount { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>The aspect ratio (width divided by height).</para>
        /// </summary>
        public float aspect { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The color with which the screen will be cleared.</para>
        /// </summary>
        public Color backgroundColor
        {
            get
            {
                Color color;
                this.INTERNAL_get_backgroundColor(out color);
                return color;
            }
            set
            {
                this.INTERNAL_set_backgroundColor(ref value);
            }
        }

        /// <summary>
        /// <para>Matrix that transforms from camera space to world space (Read Only).</para>
        /// </summary>
        public Matrix4x4 cameraToWorldMatrix
        {
            get
            {
                Matrix4x4 matrixx;
                this.INTERNAL_get_cameraToWorldMatrix(out matrixx);
                return matrixx;
            }
        }

        /// <summary>
        /// <para>Identifies what kind of camera this is.</para>
        /// </summary>
        public CameraType cameraType { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>How the camera clears the background.</para>
        /// </summary>
        public CameraClearFlags clearFlags { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Should the camera clear the stencil buffer after the deferred light pass?</para>
        /// </summary>
        public bool clearStencilAfterLightingPass { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Number of command buffers set up on this camera (Read Only).</para>
        /// </summary>
        public int commandBufferCount { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>This is used to render parts of the scene selectively.</para>
        /// </summary>
        public int cullingMask { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Sets a custom matrix for the camera to use for all culling queries.</para>
        /// </summary>
        public Matrix4x4 cullingMatrix
        {
            get
            {
                Matrix4x4 matrixx;
                this.INTERNAL_get_cullingMatrix(out matrixx);
                return matrixx;
            }
            set
            {
                this.INTERNAL_set_cullingMatrix(ref value);
            }
        }

        /// <summary>
        /// <para>The camera we are currently rendering with, for low-level render control only (Read Only).</para>
        /// </summary>
        public static Camera current { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Camera's depth in the camera rendering order.</para>
        /// </summary>
        public float depth { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>How and if camera generates a depth texture.</para>
        /// </summary>
        public DepthTextureMode depthTextureMode { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Mask to select which layers can trigger events on the camera.</para>
        /// </summary>
        public int eventMask { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        [Obsolete("use Camera.farClipPlane instead.")]
        public float far
        {
            get
            {
                return this.farClipPlane;
            }
            set
            {
                this.farClipPlane = value;
            }
        }

        /// <summary>
        /// <para>The far clipping plane distance.</para>
        /// </summary>
        public float farClipPlane { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The field of view of the camera in degrees.</para>
        /// </summary>
        public float fieldOfView { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        [Obsolete("use Camera.fieldOfView instead.")]
        public float fov
        {
            get
            {
                return this.fieldOfView;
            }
            set
            {
                this.fieldOfView = value;
            }
        }

        /// <summary>
        /// <para>High dynamic range rendering.</para>
        /// </summary>
        public bool hdr { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Property isOrthoGraphic has been deprecated. Use orthographic (UnityUpgradable) -> orthographic", true)]
        public bool isOrthoGraphic
        {
            get
            {
                return false;
            }
            set
            {
            }
        }

        /// <summary>
        /// <para>Per-layer culling distances.</para>
        /// </summary>
        public float[] layerCullDistances { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>How to perform per-layer culling for a Camera.</para>
        /// </summary>
        public bool layerCullSpherical { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The first enabled camera tagged "MainCamera" (Read Only).</para>
        /// </summary>
        public static Camera main { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Property mainCamera has been deprecated. Use Camera.main instead (UnityUpgradable) -> main", true)]
        public static Camera mainCamera
        {
            get
            {
                return null;
            }
        }

        [Obsolete("use Camera.nearClipPlane instead.")]
        public float near
        {
            get
            {
                return this.nearClipPlane;
            }
            set
            {
                this.nearClipPlane = value;
            }
        }

        /// <summary>
        /// <para>The near clipping plane distance.</para>
        /// </summary>
        public float nearClipPlane { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Get or set the raw projection matrix with no camera offset (no jittering).</para>
        /// </summary>
        public Matrix4x4 nonJitteredProjectionMatrix
        {
            get
            {
                Matrix4x4 matrixx;
                this.INTERNAL_get_nonJitteredProjectionMatrix(out matrixx);
                return matrixx;
            }
            set
            {
                this.INTERNAL_set_nonJitteredProjectionMatrix(ref value);
            }
        }

        /// <summary>
        /// <para>Opaque object sorting mode.</para>
        /// </summary>
        public OpaqueSortMode opaqueSortMode { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Is the camera orthographic (true) or perspective (false)?</para>
        /// </summary>
        public bool orthographic { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Camera's half-size when in orthographic mode.</para>
        /// </summary>
        public float orthographicSize { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>How tall is the camera in pixels (Read Only).</para>
        /// </summary>
        public int pixelHeight { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Where on the screen is the camera rendered in pixel coordinates.</para>
        /// </summary>
        public Rect pixelRect
        {
            get
            {
                Rect rect;
                this.INTERNAL_get_pixelRect(out rect);
                return rect;
            }
            set
            {
                this.INTERNAL_set_pixelRect(ref value);
            }
        }

        /// <summary>
        /// <para>How wide is the camera in pixels (Read Only).</para>
        /// </summary>
        public int pixelWidth { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        internal static int PreviewCullingLayer { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Set a custom projection matrix.</para>
        /// </summary>
        public Matrix4x4 projectionMatrix
        {
            get
            {
                Matrix4x4 matrixx;
                this.INTERNAL_get_projectionMatrix(out matrixx);
                return matrixx;
            }
            set
            {
                this.INTERNAL_set_projectionMatrix(ref value);
            }
        }

        /// <summary>
        /// <para>Where on the screen is the camera rendered in normalized coordinates.</para>
        /// </summary>
        public Rect rect
        {
            get
            {
                Rect rect;
                this.INTERNAL_get_rect(out rect);
                return rect;
            }
            set
            {
                this.INTERNAL_set_rect(ref value);
            }
        }

        /// <summary>
        /// <para>The rendering path that should be used, if possible.</para>
        /// </summary>
        public RenderingPath renderingPath { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Returns the eye that is currently rendering.
        /// If called when stereo is not enabled it will return Camera.MonoOrStereoscopicEye.Mono.
        /// 
        /// If called during a camera rendering callback such as OnRenderImage it will return the currently rendering eye.
        /// 
        /// If called outside of a rendering callback and stereo is enabled, it will return the default eye which is Camera.MonoOrStereoscopicEye.Left.</para>
        /// </summary>
        public MonoOrStereoscopicEye stereoActiveEye { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Distance to a point where virtual eyes converge.</para>
        /// </summary>
        public float stereoConvergence { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Stereoscopic rendering.</para>
        /// </summary>
        public bool stereoEnabled { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Render only once and use resulting image for both eyes.</para>
        /// </summary>
        public bool stereoMirrorMode { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Distance between the virtual eyes.</para>
        /// </summary>
        public float stereoSeparation { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Defines which eye of a VR display the Camera renders into.</para>
        /// </summary>
        public StereoTargetEyeMask stereoTargetEye { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Set the target display for this Camera.</para>
        /// </summary>
        public int targetDisplay { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Destination render texture.</para>
        /// </summary>
        public RenderTexture targetTexture { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Transparent object sorting mode.</para>
        /// </summary>
        public TransparencySortMode transparencySortMode { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Should the jittered matrix be used for transparency rendering?</para>
        /// </summary>
        public bool useJitteredProjectionMatrixForTransparentRendering { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Whether or not the Camera will use occlusion culling during rendering.</para>
        /// </summary>
        public bool useOcclusionCulling { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Get the world-space speed of the camera (Read Only).</para>
        /// </summary>
        public Vector3 velocity
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_velocity(out vector);
                return vector;
            }
        }

        /// <summary>
        /// <para>Matrix that transforms from world to camera space.</para>
        /// </summary>
        public Matrix4x4 worldToCameraMatrix
        {
            get
            {
                Matrix4x4 matrixx;
                this.INTERNAL_get_worldToCameraMatrix(out matrixx);
                return matrixx;
            }
            set
            {
                this.INTERNAL_set_worldToCameraMatrix(ref value);
            }
        }

        /// <summary>
        /// <para>Delegate type for camera callbacks.</para>
        /// </summary>
        /// <param name="cam"></param>
        public delegate void CameraCallback(Camera cam);

        /// <summary>
        /// <para>A Camera eye corresponding to the left or right human eye for stereoscopic rendering, or neither for non-stereoscopic rendering.
        /// 
        /// A single Camera can render both left and right views in a single frame. Therefore, this enum describes which eye the Camera is currently rendering when returned by Camera.stereoActiveEye during a rendering callback (such as Camera.OnRenderImage), or which eye to act on when passed into a function.
        /// 
        /// The default value is Camera.MonoOrStereoscopicEye.Left, so Camera.MonoOrStereoscopicEye.Left may be returned by some methods or properties when called outside of rendering if stereoscopic rendering is enabled.</para>
        /// </summary>
        public enum MonoOrStereoscopicEye
        {
            Left,
            Right,
            Mono
        }

        /// <summary>
        /// <para>Enum used to specify either the left or the right eye of a stereoscopic camera.</para>
        /// </summary>
        public enum StereoscopicEye
        {
            Left,
            Right
        }
    }
}

