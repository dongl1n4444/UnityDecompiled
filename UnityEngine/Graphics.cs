namespace UnityEngine
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Internal;
    using UnityEngine.Rendering;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Raw interface to Unity's drawing functions.</para>
    /// </summary>
    public sealed class Graphics
    {
        internal static readonly int kMaxDrawMeshInstanceCount = Internal_GetMaxDrawMeshInstanceCount();

        [ExcludeFromDocs]
        public static void Blit(Texture source, Material mat)
        {
            int pass = -1;
            Blit(source, mat, pass);
        }

        /// <summary>
        /// <para>Copies source texture into destination render texture with a shader.</para>
        /// </summary>
        /// <param name="source">Source texture.</param>
        /// <param name="dest">Destination RenderTexture, or null to blit directly to screen.</param>
        /// <param name="mat">Material to use. Material's shader could do some post-processing effect, for example.</param>
        /// <param name="pass">If -1 (default), draws all passes in the material. Otherwise, draws given pass only.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void Blit(Texture source, RenderTexture dest);
        /// <summary>
        /// <para>Copies source texture into destination render texture with a shader.</para>
        /// </summary>
        /// <param name="source">Source texture.</param>
        /// <param name="dest">Destination RenderTexture, or null to blit directly to screen.</param>
        /// <param name="mat">Material to use. Material's shader could do some post-processing effect, for example.</param>
        /// <param name="pass">If -1 (default), draws all passes in the material. Otherwise, draws given pass only.</param>
        public static void Blit(Texture source, Material mat, [UnityEngine.Internal.DefaultValue("-1")] int pass)
        {
            Internal_BlitMaterial(source, null, mat, pass, false);
        }

        [ExcludeFromDocs]
        public static void Blit(Texture source, RenderTexture dest, Material mat)
        {
            int pass = -1;
            Blit(source, dest, mat, pass);
        }

        /// <summary>
        /// <para>Copies source texture into destination render texture with a shader.</para>
        /// </summary>
        /// <param name="source">Source texture.</param>
        /// <param name="dest">Destination RenderTexture, or null to blit directly to screen.</param>
        /// <param name="mat">Material to use. Material's shader could do some post-processing effect, for example.</param>
        /// <param name="pass">If -1 (default), draws all passes in the material. Otherwise, draws given pass only.</param>
        public static void Blit(Texture source, RenderTexture dest, Material mat, [UnityEngine.Internal.DefaultValue("-1")] int pass)
        {
            Internal_BlitMaterial(source, dest, mat, pass, true);
        }

        /// <summary>
        /// <para>Copies source texture into destination, for multi-tap shader.</para>
        /// </summary>
        /// <param name="source">Source texture.</param>
        /// <param name="dest">Destination RenderTexture, or null to blit directly to screen.</param>
        /// <param name="mat">Material to use for copying. Material's shader should do some post-processing effect.</param>
        /// <param name="offsets">Variable number of filtering offsets. Offsets are given in pixels.</param>
        public static void BlitMultiTap(Texture source, RenderTexture dest, Material mat, params Vector2[] offsets)
        {
            Internal_BlitMultiTap(source, dest, mat, offsets);
        }

        internal static void CheckLoadActionValid(RenderBufferLoadAction load, string bufferType)
        {
            if ((load != RenderBufferLoadAction.Load) && (load != RenderBufferLoadAction.DontCare))
            {
                object[] args = new object[] { bufferType };
                throw new ArgumentException(UnityString.Format("Bad {0} LoadAction provided.", args));
            }
        }

        internal static void CheckStoreActionValid(RenderBufferStoreAction store, string bufferType)
        {
            if ((store != RenderBufferStoreAction.Store) && (store != RenderBufferStoreAction.DontCare))
            {
                object[] args = new object[] { bufferType };
                throw new ArgumentException(UnityString.Format("Bad {0} StoreAction provided.", args));
            }
        }

        /// <summary>
        /// <para>Clear random write targets for level pixel shaders.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void ClearRandomWriteTargets();
        /// <summary>
        /// <para>This function provides an efficient way to convert between textures of different formats and dimensions.
        /// The destination texture format should be uncompressed and correspond to a supported RenderTextureFormat.</para>
        /// </summary>
        /// <param name="src">Source texture.</param>
        /// <param name="dst">Destination texture.</param>
        /// <param name="srcElement">Source element (e.g. cubemap face).  Set this to 0 for 2d source textures.</param>
        /// <param name="dstElement">Destination element (e.g. cubemap face or texture array element).</param>
        public static bool ConvertTexture(Texture src, Texture dst) => 
            ConvertTexture_Full(src, dst);

        /// <summary>
        /// <para>This function provides an efficient way to convert between textures of different formats and dimensions.
        /// The destination texture format should be uncompressed and correspond to a supported RenderTextureFormat.</para>
        /// </summary>
        /// <param name="src">Source texture.</param>
        /// <param name="dst">Destination texture.</param>
        /// <param name="srcElement">Source element (e.g. cubemap face).  Set this to 0 for 2d source textures.</param>
        /// <param name="dstElement">Destination element (e.g. cubemap face or texture array element).</param>
        public static bool ConvertTexture(Texture src, int srcElement, Texture dst, int dstElement) => 
            ConvertTexture_Slice(src, srcElement, dst, dstElement);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool ConvertTexture_Full(Texture src, Texture dst);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool ConvertTexture_Slice(Texture src, int srcElement, Texture dst, int dstElement);
        /// <summary>
        /// <para>Copy texture contents.</para>
        /// </summary>
        /// <param name="src">Source texture.</param>
        /// <param name="dst">Destination texture.</param>
        /// <param name="srcElement">Source texture element (cubemap face, texture array layer or 3D texture depth slice).</param>
        /// <param name="srcMip">Source texture mipmap level.</param>
        /// <param name="dstElement">Destination texture element (cubemap face, texture array layer or 3D texture depth slice).</param>
        /// <param name="dstMip">Destination texture mipmap level.</param>
        /// <param name="srcX">X coordinate of source texture region to copy (left side is zero).</param>
        /// <param name="srcY">Y coordinate of source texture region to copy (bottom is zero).</param>
        /// <param name="srcWidth">Width of source texture region to copy.</param>
        /// <param name="srcHeight">Height of source texture region to copy.</param>
        /// <param name="dstX">X coordinate of where to copy region in destination texture (left side is zero).</param>
        /// <param name="dstY">Y coordinate of where to copy region in destination texture (bottom is zero).</param>
        public static void CopyTexture(Texture src, Texture dst)
        {
            CopyTexture_Full(src, dst);
        }

        public static void CopyTexture(Texture src, int srcElement, Texture dst, int dstElement)
        {
            CopyTexture_Slice_AllMips(src, srcElement, dst, dstElement);
        }

        /// <summary>
        /// <para>Copy texture contents.</para>
        /// </summary>
        /// <param name="src">Source texture.</param>
        /// <param name="dst">Destination texture.</param>
        /// <param name="srcElement">Source texture element (cubemap face, texture array layer or 3D texture depth slice).</param>
        /// <param name="srcMip">Source texture mipmap level.</param>
        /// <param name="dstElement">Destination texture element (cubemap face, texture array layer or 3D texture depth slice).</param>
        /// <param name="dstMip">Destination texture mipmap level.</param>
        /// <param name="srcX">X coordinate of source texture region to copy (left side is zero).</param>
        /// <param name="srcY">Y coordinate of source texture region to copy (bottom is zero).</param>
        /// <param name="srcWidth">Width of source texture region to copy.</param>
        /// <param name="srcHeight">Height of source texture region to copy.</param>
        /// <param name="dstX">X coordinate of where to copy region in destination texture (left side is zero).</param>
        /// <param name="dstY">Y coordinate of where to copy region in destination texture (bottom is zero).</param>
        public static void CopyTexture(Texture src, int srcElement, int srcMip, Texture dst, int dstElement, int dstMip)
        {
            CopyTexture_Slice(src, srcElement, srcMip, dst, dstElement, dstMip);
        }

        /// <summary>
        /// <para>Copy texture contents.</para>
        /// </summary>
        /// <param name="src">Source texture.</param>
        /// <param name="dst">Destination texture.</param>
        /// <param name="srcElement">Source texture element (cubemap face, texture array layer or 3D texture depth slice).</param>
        /// <param name="srcMip">Source texture mipmap level.</param>
        /// <param name="dstElement">Destination texture element (cubemap face, texture array layer or 3D texture depth slice).</param>
        /// <param name="dstMip">Destination texture mipmap level.</param>
        /// <param name="srcX">X coordinate of source texture region to copy (left side is zero).</param>
        /// <param name="srcY">Y coordinate of source texture region to copy (bottom is zero).</param>
        /// <param name="srcWidth">Width of source texture region to copy.</param>
        /// <param name="srcHeight">Height of source texture region to copy.</param>
        /// <param name="dstX">X coordinate of where to copy region in destination texture (left side is zero).</param>
        /// <param name="dstY">Y coordinate of where to copy region in destination texture (bottom is zero).</param>
        public static void CopyTexture(Texture src, int srcElement, int srcMip, int srcX, int srcY, int srcWidth, int srcHeight, Texture dst, int dstElement, int dstMip, int dstX, int dstY)
        {
            CopyTexture_Region(src, srcElement, srcMip, srcX, srcY, srcWidth, srcHeight, dst, dstElement, dstMip, dstX, dstY);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void CopyTexture_Full(Texture src, Texture dst);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void CopyTexture_Region(Texture src, int srcElement, int srcMip, int srcX, int srcY, int srcWidth, int srcHeight, Texture dst, int dstElement, int dstMip, int dstX, int dstY);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void CopyTexture_Slice(Texture src, int srcElement, int srcMip, Texture dst, int dstElement, int dstMip);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void CopyTexture_Slice_AllMips(Texture src, int srcElement, Texture dst, int dstElement);
        /// <summary>
        /// <para>Draw a mesh.</para>
        /// </summary>
        /// <param name="mesh">The Mesh to draw.</param>
        /// <param name="position">Position of the mesh.</param>
        /// <param name="rotation">Rotation of the mesh.</param>
        /// <param name="matrix">Transformation matrix of the mesh (combines position, rotation and other transformations).</param>
        /// <param name="material">Material to use.</param>
        /// <param name="layer"> to use.</param>
        /// <param name="camera">If null (default), the mesh will be drawn in all cameras. Otherwise it will be rendered in the given camera only.</param>
        /// <param name="submeshIndex">Which subset of the mesh to draw. This applies only to meshes that are composed of several materials.</param>
        /// <param name="properties">Additional material properties to apply onto material just before this mesh will be drawn. See MaterialPropertyBlock.</param>
        /// <param name="castShadows">Should the mesh cast shadows?</param>
        /// <param name="receiveShadows">Should the mesh receive shadows?</param>
        /// <param name="useLightProbes">Should the mesh use light probes?</param>
        /// <param name="probeAnchor">If used, the mesh will use this Transform's position to sample light probes and find the matching reflection probe.</param>
        /// <param name="materialIndex"></param>
        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Method DrawMesh has been deprecated. Use Graphics.DrawMeshNow instead (UnityUpgradable) -> DrawMeshNow(*)", true)]
        public static void DrawMesh(Mesh mesh, Matrix4x4 matrix)
        {
        }

        /// <summary>
        /// <para>Draw a mesh.</para>
        /// </summary>
        /// <param name="mesh">The Mesh to draw.</param>
        /// <param name="position">Position of the mesh.</param>
        /// <param name="rotation">Rotation of the mesh.</param>
        /// <param name="matrix">Transformation matrix of the mesh (combines position, rotation and other transformations).</param>
        /// <param name="material">Material to use.</param>
        /// <param name="layer"> to use.</param>
        /// <param name="camera">If null (default), the mesh will be drawn in all cameras. Otherwise it will be rendered in the given camera only.</param>
        /// <param name="submeshIndex">Which subset of the mesh to draw. This applies only to meshes that are composed of several materials.</param>
        /// <param name="properties">Additional material properties to apply onto material just before this mesh will be drawn. See MaterialPropertyBlock.</param>
        /// <param name="castShadows">Should the mesh cast shadows?</param>
        /// <param name="receiveShadows">Should the mesh receive shadows?</param>
        /// <param name="useLightProbes">Should the mesh use light probes?</param>
        /// <param name="probeAnchor">If used, the mesh will use this Transform's position to sample light probes and find the matching reflection probe.</param>
        /// <param name="materialIndex"></param>
        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Method DrawMesh has been deprecated. Use Graphics.DrawMeshNow instead (UnityUpgradable) -> DrawMeshNow(*)", true)]
        public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, int materialIndex)
        {
        }

        /// <summary>
        /// <para>Draw a mesh.</para>
        /// </summary>
        /// <param name="mesh">The Mesh to draw.</param>
        /// <param name="position">Position of the mesh.</param>
        /// <param name="rotation">Rotation of the mesh.</param>
        /// <param name="matrix">Transformation matrix of the mesh (combines position, rotation and other transformations).</param>
        /// <param name="material">Material to use.</param>
        /// <param name="layer"> to use.</param>
        /// <param name="camera">If null (default), the mesh will be drawn in all cameras. Otherwise it will be rendered in the given camera only.</param>
        /// <param name="submeshIndex">Which subset of the mesh to draw. This applies only to meshes that are composed of several materials.</param>
        /// <param name="properties">Additional material properties to apply onto material just before this mesh will be drawn. See MaterialPropertyBlock.</param>
        /// <param name="castShadows">Should the mesh cast shadows?</param>
        /// <param name="receiveShadows">Should the mesh receive shadows?</param>
        /// <param name="useLightProbes">Should the mesh use light probes?</param>
        /// <param name="probeAnchor">If used, the mesh will use this Transform's position to sample light probes and find the matching reflection probe.</param>
        /// <param name="materialIndex"></param>
        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Method DrawMesh has been deprecated. Use Graphics.DrawMeshNow instead (UnityUpgradable) -> DrawMeshNow(*)", true)]
        public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation)
        {
        }

        [ExcludeFromDocs]
        public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer)
        {
            bool useLightProbes = true;
            bool receiveShadows = true;
            bool castShadows = true;
            MaterialPropertyBlock properties = null;
            int submeshIndex = 0;
            Camera camera = null;
            DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, useLightProbes);
        }

        /// <summary>
        /// <para>Draw a mesh.</para>
        /// </summary>
        /// <param name="mesh">The Mesh to draw.</param>
        /// <param name="position">Position of the mesh.</param>
        /// <param name="rotation">Rotation of the mesh.</param>
        /// <param name="matrix">Transformation matrix of the mesh (combines position, rotation and other transformations).</param>
        /// <param name="material">Material to use.</param>
        /// <param name="layer"> to use.</param>
        /// <param name="camera">If null (default), the mesh will be drawn in all cameras. Otherwise it will be rendered in the given camera only.</param>
        /// <param name="submeshIndex">Which subset of the mesh to draw. This applies only to meshes that are composed of several materials.</param>
        /// <param name="properties">Additional material properties to apply onto material just before this mesh will be drawn. See MaterialPropertyBlock.</param>
        /// <param name="castShadows">Should the mesh cast shadows?</param>
        /// <param name="receiveShadows">Should the mesh receive shadows?</param>
        /// <param name="useLightProbes">Should the mesh use light probes?</param>
        /// <param name="probeAnchor">If used, the mesh will use this Transform's position to sample light probes and find the matching reflection probe.</param>
        /// <param name="materialIndex"></param>
        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Method DrawMesh has been deprecated. Use Graphics.DrawMeshNow instead (UnityUpgradable) -> DrawMeshNow(*)", true)]
        public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, int materialIndex)
        {
        }

        [ExcludeFromDocs]
        public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera)
        {
            bool useLightProbes = true;
            bool receiveShadows = true;
            bool castShadows = true;
            MaterialPropertyBlock properties = null;
            int submeshIndex = 0;
            DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, useLightProbes);
        }

        [ExcludeFromDocs]
        public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer)
        {
            bool useLightProbes = true;
            bool receiveShadows = true;
            bool castShadows = true;
            MaterialPropertyBlock properties = null;
            int submeshIndex = 0;
            Camera camera = null;
            DrawMesh(mesh, position, rotation, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, useLightProbes);
        }

        [ExcludeFromDocs]
        public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex)
        {
            bool useLightProbes = true;
            bool receiveShadows = true;
            bool castShadows = true;
            MaterialPropertyBlock properties = null;
            DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, useLightProbes);
        }

        [ExcludeFromDocs]
        public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera)
        {
            bool useLightProbes = true;
            bool receiveShadows = true;
            bool castShadows = true;
            MaterialPropertyBlock properties = null;
            int submeshIndex = 0;
            DrawMesh(mesh, position, rotation, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, useLightProbes);
        }

        [ExcludeFromDocs]
        public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties)
        {
            bool useLightProbes = true;
            bool receiveShadows = true;
            bool castShadows = true;
            DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, useLightProbes);
        }

        [ExcludeFromDocs]
        public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex)
        {
            bool useLightProbes = true;
            bool receiveShadows = true;
            bool castShadows = true;
            MaterialPropertyBlock properties = null;
            DrawMesh(mesh, position, rotation, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, useLightProbes);
        }

        [ExcludeFromDocs]
        public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, bool castShadows)
        {
            bool useLightProbes = true;
            bool receiveShadows = true;
            DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, useLightProbes);
        }

        [ExcludeFromDocs]
        public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows)
        {
            bool useLightProbes = true;
            Transform probeAnchor = null;
            bool receiveShadows = true;
            DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, probeAnchor, useLightProbes);
        }

        [ExcludeFromDocs]
        public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties)
        {
            bool useLightProbes = true;
            bool receiveShadows = true;
            bool castShadows = true;
            DrawMesh(mesh, position, rotation, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, useLightProbes);
        }

        /// <summary>
        /// <para>Draw a mesh.</para>
        /// </summary>
        /// <param name="mesh">The Mesh to draw.</param>
        /// <param name="position">Position of the mesh.</param>
        /// <param name="rotation">Rotation of the mesh.</param>
        /// <param name="matrix">Transformation matrix of the mesh (combines position, rotation and other transformations).</param>
        /// <param name="material">Material to use.</param>
        /// <param name="layer"> to use.</param>
        /// <param name="camera">If null (default), the mesh will be drawn in all cameras. Otherwise it will be rendered in the given camera only.</param>
        /// <param name="submeshIndex">Which subset of the mesh to draw. This applies only to meshes that are composed of several materials.</param>
        /// <param name="properties">Additional material properties to apply onto material just before this mesh will be drawn. See MaterialPropertyBlock.</param>
        /// <param name="castShadows">Should the mesh cast shadows?</param>
        /// <param name="receiveShadows">Should the mesh receive shadows?</param>
        /// <param name="useLightProbes">Should the mesh use light probes?</param>
        /// <param name="probeAnchor">If used, the mesh will use this Transform's position to sample light probes and find the matching reflection probe.</param>
        /// <param name="materialIndex"></param>
        [ExcludeFromDocs]
        public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, bool castShadows, bool receiveShadows)
        {
            bool useLightProbes = true;
            DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, useLightProbes);
        }

        /// <summary>
        /// <para>Draw a mesh.</para>
        /// </summary>
        /// <param name="mesh">The Mesh to draw.</param>
        /// <param name="position">Position of the mesh.</param>
        /// <param name="rotation">Rotation of the mesh.</param>
        /// <param name="matrix">Transformation matrix of the mesh (combines position, rotation and other transformations).</param>
        /// <param name="material">Material to use.</param>
        /// <param name="layer"> to use.</param>
        /// <param name="camera">If null (default), the mesh will be drawn in all cameras. Otherwise it will be rendered in the given camera only.</param>
        /// <param name="submeshIndex">Which subset of the mesh to draw. This applies only to meshes that are composed of several materials.</param>
        /// <param name="properties">Additional material properties to apply onto material just before this mesh will be drawn. See MaterialPropertyBlock.</param>
        /// <param name="castShadows">Should the mesh cast shadows?</param>
        /// <param name="receiveShadows">Should the mesh receive shadows?</param>
        /// <param name="useLightProbes">Should the mesh use light probes?</param>
        /// <param name="probeAnchor">If used, the mesh will use this Transform's position to sample light probes and find the matching reflection probe.</param>
        /// <param name="materialIndex"></param>
        [ExcludeFromDocs]
        public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows)
        {
            bool useLightProbes = true;
            Transform probeAnchor = null;
            DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, probeAnchor, useLightProbes);
        }

        [ExcludeFromDocs]
        public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, bool castShadows)
        {
            bool useLightProbes = true;
            bool receiveShadows = true;
            DrawMesh(mesh, position, rotation, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, useLightProbes);
        }

        [ExcludeFromDocs]
        public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows)
        {
            bool useLightProbes = true;
            Transform probeAnchor = null;
            bool receiveShadows = true;
            DrawMesh(mesh, position, rotation, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, probeAnchor, useLightProbes);
        }

        /// <summary>
        /// <para>Draw a mesh.</para>
        /// </summary>
        /// <param name="mesh">The Mesh to draw.</param>
        /// <param name="position">Position of the mesh.</param>
        /// <param name="rotation">Rotation of the mesh.</param>
        /// <param name="matrix">Transformation matrix of the mesh (combines position, rotation and other transformations).</param>
        /// <param name="material">Material to use.</param>
        /// <param name="layer"> to use.</param>
        /// <param name="camera">If null (default), the mesh will be drawn in all cameras. Otherwise it will be rendered in the given camera only.</param>
        /// <param name="submeshIndex">Which subset of the mesh to draw. This applies only to meshes that are composed of several materials.</param>
        /// <param name="properties">Additional material properties to apply onto material just before this mesh will be drawn. See MaterialPropertyBlock.</param>
        /// <param name="castShadows">Should the mesh cast shadows?</param>
        /// <param name="receiveShadows">Should the mesh receive shadows?</param>
        /// <param name="useLightProbes">Should the mesh use light probes?</param>
        /// <param name="probeAnchor">If used, the mesh will use this Transform's position to sample light probes and find the matching reflection probe.</param>
        /// <param name="materialIndex"></param>
        public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, [UnityEngine.Internal.DefaultValue("null")] Camera camera, [UnityEngine.Internal.DefaultValue("0")] int submeshIndex, [UnityEngine.Internal.DefaultValue("null")] MaterialPropertyBlock properties, [UnityEngine.Internal.DefaultValue("true")] bool castShadows, [UnityEngine.Internal.DefaultValue("true")] bool receiveShadows, [UnityEngine.Internal.DefaultValue("true")] bool useLightProbes)
        {
            DrawMeshImpl(mesh, matrix, material, layer, camera, submeshIndex, properties, !castShadows ? ShadowCastingMode.Off : ShadowCastingMode.On, receiveShadows, null, useLightProbes);
        }

        [ExcludeFromDocs]
        public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, Transform probeAnchor)
        {
            bool useLightProbes = true;
            DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, probeAnchor, useLightProbes);
        }

        /// <summary>
        /// <para>Draw a mesh.</para>
        /// </summary>
        /// <param name="mesh">The Mesh to draw.</param>
        /// <param name="position">Position of the mesh.</param>
        /// <param name="rotation">Rotation of the mesh.</param>
        /// <param name="matrix">Transformation matrix of the mesh (combines position, rotation and other transformations).</param>
        /// <param name="material">Material to use.</param>
        /// <param name="layer"> to use.</param>
        /// <param name="camera">If null (default), the mesh will be drawn in all cameras. Otherwise it will be rendered in the given camera only.</param>
        /// <param name="submeshIndex">Which subset of the mesh to draw. This applies only to meshes that are composed of several materials.</param>
        /// <param name="properties">Additional material properties to apply onto material just before this mesh will be drawn. See MaterialPropertyBlock.</param>
        /// <param name="castShadows">Should the mesh cast shadows?</param>
        /// <param name="receiveShadows">Should the mesh receive shadows?</param>
        /// <param name="useLightProbes">Should the mesh use light probes?</param>
        /// <param name="probeAnchor">If used, the mesh will use this Transform's position to sample light probes and find the matching reflection probe.</param>
        /// <param name="materialIndex"></param>
        [ExcludeFromDocs]
        public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, bool castShadows, bool receiveShadows)
        {
            bool useLightProbes = true;
            DrawMesh(mesh, position, rotation, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, useLightProbes);
        }

        [ExcludeFromDocs]
        public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows)
        {
            bool useLightProbes = true;
            Transform probeAnchor = null;
            DrawMesh(mesh, position, rotation, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, probeAnchor, useLightProbes);
        }

        /// <summary>
        /// <para>Draw a mesh.</para>
        /// </summary>
        /// <param name="mesh">The Mesh to draw.</param>
        /// <param name="position">Position of the mesh.</param>
        /// <param name="rotation">Rotation of the mesh.</param>
        /// <param name="matrix">Transformation matrix of the mesh (combines position, rotation and other transformations).</param>
        /// <param name="material">Material to use.</param>
        /// <param name="layer"> to use.</param>
        /// <param name="camera">If null (default), the mesh will be drawn in all cameras. Otherwise it will be rendered in the given camera only.</param>
        /// <param name="submeshIndex">Which subset of the mesh to draw. This applies only to meshes that are composed of several materials.</param>
        /// <param name="properties">Additional material properties to apply onto material just before this mesh will be drawn. See MaterialPropertyBlock.</param>
        /// <param name="castShadows">Should the mesh cast shadows?</param>
        /// <param name="receiveShadows">Should the mesh receive shadows?</param>
        /// <param name="useLightProbes">Should the mesh use light probes?</param>
        /// <param name="probeAnchor">If used, the mesh will use this Transform's position to sample light probes and find the matching reflection probe.</param>
        /// <param name="materialIndex"></param>
        public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows, [UnityEngine.Internal.DefaultValue("true")] bool receiveShadows, [UnityEngine.Internal.DefaultValue("null")] Transform probeAnchor, [UnityEngine.Internal.DefaultValue("true")] bool useLightProbes)
        {
            DrawMeshImpl(mesh, matrix, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, probeAnchor, useLightProbes);
        }

        /// <summary>
        /// <para>Draw a mesh.</para>
        /// </summary>
        /// <param name="mesh">The Mesh to draw.</param>
        /// <param name="position">Position of the mesh.</param>
        /// <param name="rotation">Rotation of the mesh.</param>
        /// <param name="matrix">Transformation matrix of the mesh (combines position, rotation and other transformations).</param>
        /// <param name="material">Material to use.</param>
        /// <param name="layer"> to use.</param>
        /// <param name="camera">If null (default), the mesh will be drawn in all cameras. Otherwise it will be rendered in the given camera only.</param>
        /// <param name="submeshIndex">Which subset of the mesh to draw. This applies only to meshes that are composed of several materials.</param>
        /// <param name="properties">Additional material properties to apply onto material just before this mesh will be drawn. See MaterialPropertyBlock.</param>
        /// <param name="castShadows">Should the mesh cast shadows?</param>
        /// <param name="receiveShadows">Should the mesh receive shadows?</param>
        /// <param name="useLightProbes">Should the mesh use light probes?</param>
        /// <param name="probeAnchor">If used, the mesh will use this Transform's position to sample light probes and find the matching reflection probe.</param>
        /// <param name="materialIndex"></param>
        public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, [UnityEngine.Internal.DefaultValue("null")] Camera camera, [UnityEngine.Internal.DefaultValue("0")] int submeshIndex, [UnityEngine.Internal.DefaultValue("null")] MaterialPropertyBlock properties, [UnityEngine.Internal.DefaultValue("true")] bool castShadows, [UnityEngine.Internal.DefaultValue("true")] bool receiveShadows, [UnityEngine.Internal.DefaultValue("true")] bool useLightProbes)
        {
            DrawMesh(mesh, position, rotation, material, layer, camera, submeshIndex, properties, !castShadows ? ShadowCastingMode.Off : ShadowCastingMode.On, receiveShadows, null, useLightProbes);
        }

        /// <summary>
        /// <para>Draw a mesh.</para>
        /// </summary>
        /// <param name="mesh">The Mesh to draw.</param>
        /// <param name="position">Position of the mesh.</param>
        /// <param name="rotation">Rotation of the mesh.</param>
        /// <param name="matrix">Transformation matrix of the mesh (combines position, rotation and other transformations).</param>
        /// <param name="material">Material to use.</param>
        /// <param name="layer"> to use.</param>
        /// <param name="camera">If null (default), the mesh will be drawn in all cameras. Otherwise it will be rendered in the given camera only.</param>
        /// <param name="submeshIndex">Which subset of the mesh to draw. This applies only to meshes that are composed of several materials.</param>
        /// <param name="properties">Additional material properties to apply onto material just before this mesh will be drawn. See MaterialPropertyBlock.</param>
        /// <param name="castShadows">Should the mesh cast shadows?</param>
        /// <param name="receiveShadows">Should the mesh receive shadows?</param>
        /// <param name="useLightProbes">Should the mesh use light probes?</param>
        /// <param name="probeAnchor">If used, the mesh will use this Transform's position to sample light probes and find the matching reflection probe.</param>
        /// <param name="materialIndex"></param>
        [ExcludeFromDocs]
        public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, Transform probeAnchor)
        {
            bool useLightProbes = true;
            DrawMesh(mesh, position, rotation, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, probeAnchor, useLightProbes);
        }

        /// <summary>
        /// <para>Draw a mesh.</para>
        /// </summary>
        /// <param name="mesh">The Mesh to draw.</param>
        /// <param name="position">Position of the mesh.</param>
        /// <param name="rotation">Rotation of the mesh.</param>
        /// <param name="matrix">Transformation matrix of the mesh (combines position, rotation and other transformations).</param>
        /// <param name="material">Material to use.</param>
        /// <param name="layer"> to use.</param>
        /// <param name="camera">If null (default), the mesh will be drawn in all cameras. Otherwise it will be rendered in the given camera only.</param>
        /// <param name="submeshIndex">Which subset of the mesh to draw. This applies only to meshes that are composed of several materials.</param>
        /// <param name="properties">Additional material properties to apply onto material just before this mesh will be drawn. See MaterialPropertyBlock.</param>
        /// <param name="castShadows">Should the mesh cast shadows?</param>
        /// <param name="receiveShadows">Should the mesh receive shadows?</param>
        /// <param name="useLightProbes">Should the mesh use light probes?</param>
        /// <param name="probeAnchor">If used, the mesh will use this Transform's position to sample light probes and find the matching reflection probe.</param>
        /// <param name="materialIndex"></param>
        public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows, [UnityEngine.Internal.DefaultValue("true")] bool receiveShadows, [UnityEngine.Internal.DefaultValue("null")] Transform probeAnchor, [UnityEngine.Internal.DefaultValue("true")] bool useLightProbes)
        {
            DrawMeshImpl(mesh, Matrix4x4.TRS(position, rotation, Vector3.one), material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, probeAnchor, useLightProbes);
        }

        private static void DrawMeshImpl(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, Transform probeAnchor, bool useLightProbes)
        {
            Internal_DrawMeshMatrixArguments arguments = new Internal_DrawMeshMatrixArguments {
                layer = layer,
                submeshIndex = submeshIndex,
                matrix = matrix,
                castShadows = (int) castShadows,
                receiveShadows = !receiveShadows ? 0 : 1,
                reflectionProbeAnchorInstanceID = (probeAnchor == null) ? 0 : probeAnchor.GetInstanceID(),
                useLightProbes = useLightProbes
            };
            Internal_DrawMeshMatrix(ref arguments, properties, material, mesh, camera);
        }

        [ExcludeFromDocs]
        public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, List<Matrix4x4> matrices)
        {
            Camera camera = null;
            int layer = 0;
            bool receiveShadows = true;
            ShadowCastingMode on = ShadowCastingMode.On;
            MaterialPropertyBlock properties = null;
            DrawMeshInstanced(mesh, submeshIndex, material, matrices, properties, on, receiveShadows, layer, camera);
        }

        [ExcludeFromDocs]
        public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, Matrix4x4[] matrices)
        {
            Camera camera = null;
            int layer = 0;
            bool receiveShadows = true;
            ShadowCastingMode on = ShadowCastingMode.On;
            MaterialPropertyBlock properties = null;
            int length = matrices.Length;
            DrawMeshInstanced(mesh, submeshIndex, material, matrices, length, properties, on, receiveShadows, layer, camera);
        }

        [ExcludeFromDocs]
        public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, List<Matrix4x4> matrices, MaterialPropertyBlock properties)
        {
            Camera camera = null;
            int layer = 0;
            bool receiveShadows = true;
            ShadowCastingMode on = ShadowCastingMode.On;
            DrawMeshInstanced(mesh, submeshIndex, material, matrices, properties, on, receiveShadows, layer, camera);
        }

        [ExcludeFromDocs]
        public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, Matrix4x4[] matrices, int count)
        {
            Camera camera = null;
            int layer = 0;
            bool receiveShadows = true;
            ShadowCastingMode on = ShadowCastingMode.On;
            MaterialPropertyBlock properties = null;
            DrawMeshInstanced(mesh, submeshIndex, material, matrices, count, properties, on, receiveShadows, layer, camera);
        }

        [ExcludeFromDocs]
        public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, List<Matrix4x4> matrices, MaterialPropertyBlock properties, ShadowCastingMode castShadows)
        {
            Camera camera = null;
            int layer = 0;
            bool receiveShadows = true;
            DrawMeshInstanced(mesh, submeshIndex, material, matrices, properties, castShadows, receiveShadows, layer, camera);
        }

        [ExcludeFromDocs]
        public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, Matrix4x4[] matrices, int count, MaterialPropertyBlock properties)
        {
            Camera camera = null;
            int layer = 0;
            bool receiveShadows = true;
            ShadowCastingMode on = ShadowCastingMode.On;
            DrawMeshInstanced(mesh, submeshIndex, material, matrices, count, properties, on, receiveShadows, layer, camera);
        }

        [ExcludeFromDocs]
        public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, List<Matrix4x4> matrices, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows)
        {
            Camera camera = null;
            int layer = 0;
            DrawMeshInstanced(mesh, submeshIndex, material, matrices, properties, castShadows, receiveShadows, layer, camera);
        }

        [ExcludeFromDocs]
        public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, Matrix4x4[] matrices, int count, MaterialPropertyBlock properties, ShadowCastingMode castShadows)
        {
            Camera camera = null;
            int layer = 0;
            bool receiveShadows = true;
            DrawMeshInstanced(mesh, submeshIndex, material, matrices, count, properties, castShadows, receiveShadows, layer, camera);
        }

        [ExcludeFromDocs]
        public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, List<Matrix4x4> matrices, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, int layer)
        {
            Camera camera = null;
            DrawMeshInstanced(mesh, submeshIndex, material, matrices, properties, castShadows, receiveShadows, layer, camera);
        }

        [ExcludeFromDocs]
        public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, Matrix4x4[] matrices, int count, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows)
        {
            Camera camera = null;
            int layer = 0;
            DrawMeshInstanced(mesh, submeshIndex, material, matrices, count, properties, castShadows, receiveShadows, layer, camera);
        }

        public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, List<Matrix4x4> matrices, [UnityEngine.Internal.DefaultValue("null")] MaterialPropertyBlock properties, [UnityEngine.Internal.DefaultValue("ShadowCastingMode.On")] ShadowCastingMode castShadows, [UnityEngine.Internal.DefaultValue("true")] bool receiveShadows, [UnityEngine.Internal.DefaultValue("0")] int layer, [UnityEngine.Internal.DefaultValue("null")] Camera camera)
        {
            DrawMeshInstancedImpl(mesh, submeshIndex, material, matrices, properties, castShadows, receiveShadows, layer, camera);
        }

        [ExcludeFromDocs]
        public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, Matrix4x4[] matrices, int count, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, int layer)
        {
            Camera camera = null;
            DrawMeshInstanced(mesh, submeshIndex, material, matrices, count, properties, castShadows, receiveShadows, layer, camera);
        }

        /// <summary>
        /// <para>Draw the same mesh multiple times using GPU instancing.</para>
        /// </summary>
        /// <param name="mesh">The Mesh to draw.</param>
        /// <param name="submeshIndex">Which subset of the mesh to draw. This applies only to meshes that are composed of several materials.</param>
        /// <param name="material">Material to use.</param>
        /// <param name="matrices">The array of object transformation matrices.</param>
        /// <param name="count">The number of instances to be drawn.</param>
        /// <param name="properties">Additional material properties to apply. See MaterialPropertyBlock.</param>
        /// <param name="castShadows">Should the mesh cast shadows?</param>
        /// <param name="receiveShadows">Should the mesh receive shadows?</param>
        /// <param name="layer"> to use.</param>
        /// <param name="camera">If null (default), the mesh will be drawn in all cameras. Otherwise it will be drawn in the given camera only.</param>
        public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, Matrix4x4[] matrices, [UnityEngine.Internal.DefaultValue("matrices.Length")] int count, [UnityEngine.Internal.DefaultValue("null")] MaterialPropertyBlock properties, [UnityEngine.Internal.DefaultValue("ShadowCastingMode.On")] ShadowCastingMode castShadows, [UnityEngine.Internal.DefaultValue("true")] bool receiveShadows, [UnityEngine.Internal.DefaultValue("0")] int layer, [UnityEngine.Internal.DefaultValue("null")] Camera camera)
        {
            DrawMeshInstancedImpl(mesh, submeshIndex, material, matrices, count, properties, castShadows, receiveShadows, layer, camera);
        }

        private static void DrawMeshInstancedImpl(Mesh mesh, int submeshIndex, Material material, List<Matrix4x4> matrices, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, int layer, Camera camera)
        {
            if (matrices == null)
            {
                throw new ArgumentNullException("matrices");
            }
            if (matrices.Count > kMaxDrawMeshInstanceCount)
            {
                throw new ArgumentOutOfRangeException("matrices", $"Matrix list count must be in the range of 0 to {kMaxDrawMeshInstanceCount}.");
            }
            DrawMeshInstancedImpl(mesh, submeshIndex, material, (Matrix4x4[]) ExtractArrayFromList(matrices), matrices.Count, properties, castShadows, receiveShadows, layer, camera);
        }

        private static void DrawMeshInstancedImpl(Mesh mesh, int submeshIndex, Material material, Matrix4x4[] matrices, int count, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, int layer, Camera camera)
        {
            if (!SystemInfo.supportsInstancing)
            {
                throw new InvalidOperationException("Instancing is not supported.");
            }
            if (mesh == null)
            {
                throw new ArgumentNullException("mesh");
            }
            if ((submeshIndex < 0) || (submeshIndex >= mesh.subMeshCount))
            {
                throw new ArgumentOutOfRangeException("submeshIndex", "submeshIndex out of range.");
            }
            if (material == null)
            {
                throw new ArgumentNullException("material");
            }
            if (!material.enableInstancing)
            {
                throw new InvalidOperationException("Material needs to enable instancing for use with DrawMeshInstanced.");
            }
            if (matrices == null)
            {
                throw new ArgumentNullException("matrices");
            }
            if ((count < 0) || (count > Mathf.Min(kMaxDrawMeshInstanceCount, matrices.Length)))
            {
                throw new ArgumentOutOfRangeException("count", $"Count must be in the range of 0 to {Mathf.Min(kMaxDrawMeshInstanceCount, matrices.Length)}.");
            }
            if (count > 0)
            {
                Internal_DrawMeshInstanced(mesh, submeshIndex, material, matrices, count, properties, castShadows, receiveShadows, layer, camera);
            }
        }

        [ExcludeFromDocs]
        public static void DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, Bounds bounds, ComputeBuffer bufferWithArgs)
        {
            Camera camera = null;
            int layer = 0;
            bool receiveShadows = true;
            ShadowCastingMode on = ShadowCastingMode.On;
            MaterialPropertyBlock properties = null;
            int argsOffset = 0;
            DrawMeshInstancedIndirect(mesh, submeshIndex, material, bounds, bufferWithArgs, argsOffset, properties, on, receiveShadows, layer, camera);
        }

        [ExcludeFromDocs]
        public static void DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, Bounds bounds, ComputeBuffer bufferWithArgs, int argsOffset)
        {
            Camera camera = null;
            int layer = 0;
            bool receiveShadows = true;
            ShadowCastingMode on = ShadowCastingMode.On;
            MaterialPropertyBlock properties = null;
            DrawMeshInstancedIndirect(mesh, submeshIndex, material, bounds, bufferWithArgs, argsOffset, properties, on, receiveShadows, layer, camera);
        }

        [ExcludeFromDocs]
        public static void DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, Bounds bounds, ComputeBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties)
        {
            Camera camera = null;
            int layer = 0;
            bool receiveShadows = true;
            ShadowCastingMode on = ShadowCastingMode.On;
            DrawMeshInstancedIndirect(mesh, submeshIndex, material, bounds, bufferWithArgs, argsOffset, properties, on, receiveShadows, layer, camera);
        }

        [ExcludeFromDocs]
        public static void DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, Bounds bounds, ComputeBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties, ShadowCastingMode castShadows)
        {
            Camera camera = null;
            int layer = 0;
            bool receiveShadows = true;
            DrawMeshInstancedIndirect(mesh, submeshIndex, material, bounds, bufferWithArgs, argsOffset, properties, castShadows, receiveShadows, layer, camera);
        }

        [ExcludeFromDocs]
        public static void DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, Bounds bounds, ComputeBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows)
        {
            Camera camera = null;
            int layer = 0;
            DrawMeshInstancedIndirect(mesh, submeshIndex, material, bounds, bufferWithArgs, argsOffset, properties, castShadows, receiveShadows, layer, camera);
        }

        [ExcludeFromDocs]
        public static void DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, Bounds bounds, ComputeBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, int layer)
        {
            Camera camera = null;
            DrawMeshInstancedIndirect(mesh, submeshIndex, material, bounds, bufferWithArgs, argsOffset, properties, castShadows, receiveShadows, layer, camera);
        }

        /// <summary>
        /// <para>Draw the same mesh multiple times using GPU instancing.</para>
        /// </summary>
        /// <param name="mesh">The Mesh to draw.</param>
        /// <param name="submeshIndex">Which subset of the mesh to draw. This applies only to meshes that are composed of several materials.</param>
        /// <param name="material">Material to use.</param>
        /// <param name="bounds">The bounding volume surrounding the instances you intend to draw.</param>
        /// <param name="bufferWithArgs">The GPU buffer containing the arguments for how many instances of this mesh to draw.</param>
        /// <param name="argsOffset">The byte offset into the buffer, where the draw arguments start.</param>
        /// <param name="properties">Additional material properties to apply. See MaterialPropertyBlock.</param>
        /// <param name="castShadows">Should the mesh cast shadows?</param>
        /// <param name="receiveShadows">Should the mesh receive shadows?</param>
        /// <param name="layer"> to use.</param>
        /// <param name="camera">If null (default), the mesh will be drawn in all cameras. Otherwise it will be drawn in the given camera only.</param>
        public static void DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, Bounds bounds, ComputeBuffer bufferWithArgs, [UnityEngine.Internal.DefaultValue("0")] int argsOffset, [UnityEngine.Internal.DefaultValue("null")] MaterialPropertyBlock properties, [UnityEngine.Internal.DefaultValue("ShadowCastingMode.On")] ShadowCastingMode castShadows, [UnityEngine.Internal.DefaultValue("true")] bool receiveShadows, [UnityEngine.Internal.DefaultValue("0")] int layer, [UnityEngine.Internal.DefaultValue("null")] Camera camera)
        {
            DrawMeshInstancedIndirectImpl(mesh, submeshIndex, material, bounds, bufferWithArgs, argsOffset, properties, castShadows, receiveShadows, layer, camera);
        }

        private static void DrawMeshInstancedIndirectImpl(Mesh mesh, int submeshIndex, Material material, Bounds bounds, ComputeBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, int layer, Camera camera)
        {
            if (!SystemInfo.supportsInstancing)
            {
                throw new InvalidOperationException("Instancing is not supported.");
            }
            if (mesh == null)
            {
                throw new ArgumentNullException("mesh");
            }
            if ((submeshIndex < 0) || (submeshIndex >= mesh.subMeshCount))
            {
                throw new ArgumentOutOfRangeException("submeshIndex", "submeshIndex out of range.");
            }
            if (material == null)
            {
                throw new ArgumentNullException("material");
            }
            if (bufferWithArgs == null)
            {
                throw new ArgumentNullException("bufferWithArgs");
            }
            Internal_DrawMeshInstancedIndirect(mesh, submeshIndex, material, bounds, bufferWithArgs, argsOffset, properties, castShadows, receiveShadows, layer, camera);
        }

        /// <summary>
        /// <para>Draw a mesh immediately.</para>
        /// </summary>
        /// <param name="mesh">The Mesh to draw.</param>
        /// <param name="position">Position of the mesh.</param>
        /// <param name="rotation">Rotation of the mesh.</param>
        /// <param name="matrix">Transformation matrix of the mesh (combines position, rotation and other transformations). Note that the mesh will not be displayed correctly if matrix has negative scale.</param>
        /// <param name="materialIndex">Subset of the mesh to draw.</param>
        public static void DrawMeshNow(Mesh mesh, Matrix4x4 matrix)
        {
            DrawMeshNow(mesh, matrix, -1);
        }

        /// <summary>
        /// <para>Draw a mesh immediately.</para>
        /// </summary>
        /// <param name="mesh">The Mesh to draw.</param>
        /// <param name="position">Position of the mesh.</param>
        /// <param name="rotation">Rotation of the mesh.</param>
        /// <param name="matrix">Transformation matrix of the mesh (combines position, rotation and other transformations). Note that the mesh will not be displayed correctly if matrix has negative scale.</param>
        /// <param name="materialIndex">Subset of the mesh to draw.</param>
        public static void DrawMeshNow(Mesh mesh, Matrix4x4 matrix, int materialIndex)
        {
            Internal_DrawMeshNow2(mesh, materialIndex, matrix);
        }

        /// <summary>
        /// <para>Draw a mesh immediately.</para>
        /// </summary>
        /// <param name="mesh">The Mesh to draw.</param>
        /// <param name="position">Position of the mesh.</param>
        /// <param name="rotation">Rotation of the mesh.</param>
        /// <param name="matrix">Transformation matrix of the mesh (combines position, rotation and other transformations). Note that the mesh will not be displayed correctly if matrix has negative scale.</param>
        /// <param name="materialIndex">Subset of the mesh to draw.</param>
        public static void DrawMeshNow(Mesh mesh, Vector3 position, Quaternion rotation)
        {
            DrawMeshNow(mesh, position, rotation, -1);
        }

        /// <summary>
        /// <para>Draw a mesh immediately.</para>
        /// </summary>
        /// <param name="mesh">The Mesh to draw.</param>
        /// <param name="position">Position of the mesh.</param>
        /// <param name="rotation">Rotation of the mesh.</param>
        /// <param name="matrix">Transformation matrix of the mesh (combines position, rotation and other transformations). Note that the mesh will not be displayed correctly if matrix has negative scale.</param>
        /// <param name="materialIndex">Subset of the mesh to draw.</param>
        public static void DrawMeshNow(Mesh mesh, Vector3 position, Quaternion rotation, int materialIndex)
        {
            Internal_DrawMeshNow1(mesh, materialIndex, position, rotation);
        }

        [ExcludeFromDocs]
        public static void DrawProcedural(MeshTopology topology, int vertexCount)
        {
            int instanceCount = 1;
            DrawProcedural(topology, vertexCount, instanceCount);
        }

        /// <summary>
        /// <para>Draws a fully procedural geometry on the GPU.</para>
        /// </summary>
        /// <param name="topology"></param>
        /// <param name="vertexCount"></param>
        /// <param name="instanceCount"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void DrawProcedural(MeshTopology topology, int vertexCount, [UnityEngine.Internal.DefaultValue("1")] int instanceCount);
        [ExcludeFromDocs]
        public static void DrawProceduralIndirect(MeshTopology topology, ComputeBuffer bufferWithArgs)
        {
            int argsOffset = 0;
            DrawProceduralIndirect(topology, bufferWithArgs, argsOffset);
        }

        /// <summary>
        /// <para>Draws a fully procedural geometry on the GPU.</para>
        /// </summary>
        /// <param name="topology">Topology of the procedural geometry.</param>
        /// <param name="bufferWithArgs">Buffer with draw arguments.</param>
        /// <param name="argsOffset">Byte offset where in the buffer the draw arguments are.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void DrawProceduralIndirect(MeshTopology topology, ComputeBuffer bufferWithArgs, [UnityEngine.Internal.DefaultValue("0")] int argsOffset);
        [ExcludeFromDocs]
        public static void DrawTexture(Rect screenRect, Texture texture)
        {
            int pass = -1;
            Material mat = null;
            DrawTexture(screenRect, texture, mat, pass);
        }

        /// <summary>
        /// <para>Draw a texture in screen coordinates.</para>
        /// </summary>
        /// <param name="screenRect">Rectangle on the screen to use for the texture. In pixel coordinates with (0,0) in the upper-left corner.</param>
        /// <param name="texture">Texture to draw.</param>
        /// <param name="sourceRect">Region of the texture to use. In normalized coordinates with (0,0) in the bottom-left corner.</param>
        /// <param name="leftBorder">Number of pixels from the left that are not affected by scale.</param>
        /// <param name="rightBorder">Number of pixels from the right that are not affected by scale.</param>
        /// <param name="topBorder">Number of pixels from the top that are not affected by scale.</param>
        /// <param name="bottomBorder">Number of pixels from the bottom that are not affected by scale.</param>
        /// <param name="color">Color that modulates the output. The neutral value is (0.5, 0.5, 0.5, 0.5). Set as vertex color for the shader.</param>
        /// <param name="mat">Custom Material that can be used to draw the texture. If null is passed, a default material with the Internal-GUITexture.shader is used.</param>
        /// <param name="pass">If -1 (default), draws all passes in the material. Otherwise, draws given pass only.</param>
        [ExcludeFromDocs]
        public static void DrawTexture(Rect screenRect, Texture texture, Material mat)
        {
            int pass = -1;
            DrawTexture(screenRect, texture, mat, pass);
        }

        public static void DrawTexture(Rect screenRect, Texture texture, [UnityEngine.Internal.DefaultValue("null")] Material mat, [UnityEngine.Internal.DefaultValue("-1")] int pass)
        {
            DrawTexture(screenRect, texture, 0, 0, 0, 0, mat, pass);
        }

        [ExcludeFromDocs]
        public static void DrawTexture(Rect screenRect, Texture texture, int leftBorder, int rightBorder, int topBorder, int bottomBorder)
        {
            int pass = -1;
            Material mat = null;
            DrawTexture(screenRect, texture, leftBorder, rightBorder, topBorder, bottomBorder, mat, pass);
        }

        /// <summary>
        /// <para>Draw a texture in screen coordinates.</para>
        /// </summary>
        /// <param name="screenRect">Rectangle on the screen to use for the texture. In pixel coordinates with (0,0) in the upper-left corner.</param>
        /// <param name="texture">Texture to draw.</param>
        /// <param name="sourceRect">Region of the texture to use. In normalized coordinates with (0,0) in the bottom-left corner.</param>
        /// <param name="leftBorder">Number of pixels from the left that are not affected by scale.</param>
        /// <param name="rightBorder">Number of pixels from the right that are not affected by scale.</param>
        /// <param name="topBorder">Number of pixels from the top that are not affected by scale.</param>
        /// <param name="bottomBorder">Number of pixels from the bottom that are not affected by scale.</param>
        /// <param name="color">Color that modulates the output. The neutral value is (0.5, 0.5, 0.5, 0.5). Set as vertex color for the shader.</param>
        /// <param name="mat">Custom Material that can be used to draw the texture. If null is passed, a default material with the Internal-GUITexture.shader is used.</param>
        /// <param name="pass">If -1 (default), draws all passes in the material. Otherwise, draws given pass only.</param>
        [ExcludeFromDocs]
        public static void DrawTexture(Rect screenRect, Texture texture, int leftBorder, int rightBorder, int topBorder, int bottomBorder, Material mat)
        {
            int pass = -1;
            DrawTexture(screenRect, texture, leftBorder, rightBorder, topBorder, bottomBorder, mat, pass);
        }

        [ExcludeFromDocs]
        public static void DrawTexture(Rect screenRect, Texture texture, Rect sourceRect, int leftBorder, int rightBorder, int topBorder, int bottomBorder)
        {
            int pass = -1;
            Material mat = null;
            DrawTexture(screenRect, texture, sourceRect, leftBorder, rightBorder, topBorder, bottomBorder, mat, pass);
        }

        public static void DrawTexture(Rect screenRect, Texture texture, int leftBorder, int rightBorder, int topBorder, int bottomBorder, [UnityEngine.Internal.DefaultValue("null")] Material mat, [UnityEngine.Internal.DefaultValue("-1")] int pass)
        {
            DrawTexture(screenRect, texture, new Rect(0f, 0f, 1f, 1f), leftBorder, rightBorder, topBorder, bottomBorder, mat, pass);
        }

        [ExcludeFromDocs]
        public static void DrawTexture(Rect screenRect, Texture texture, Rect sourceRect, int leftBorder, int rightBorder, int topBorder, int bottomBorder, Color color)
        {
            int pass = -1;
            Material mat = null;
            DrawTexture(screenRect, texture, sourceRect, leftBorder, rightBorder, topBorder, bottomBorder, color, mat, pass);
        }

        /// <summary>
        /// <para>Draw a texture in screen coordinates.</para>
        /// </summary>
        /// <param name="screenRect">Rectangle on the screen to use for the texture. In pixel coordinates with (0,0) in the upper-left corner.</param>
        /// <param name="texture">Texture to draw.</param>
        /// <param name="sourceRect">Region of the texture to use. In normalized coordinates with (0,0) in the bottom-left corner.</param>
        /// <param name="leftBorder">Number of pixels from the left that are not affected by scale.</param>
        /// <param name="rightBorder">Number of pixels from the right that are not affected by scale.</param>
        /// <param name="topBorder">Number of pixels from the top that are not affected by scale.</param>
        /// <param name="bottomBorder">Number of pixels from the bottom that are not affected by scale.</param>
        /// <param name="color">Color that modulates the output. The neutral value is (0.5, 0.5, 0.5, 0.5). Set as vertex color for the shader.</param>
        /// <param name="mat">Custom Material that can be used to draw the texture. If null is passed, a default material with the Internal-GUITexture.shader is used.</param>
        /// <param name="pass">If -1 (default), draws all passes in the material. Otherwise, draws given pass only.</param>
        [ExcludeFromDocs]
        public static void DrawTexture(Rect screenRect, Texture texture, Rect sourceRect, int leftBorder, int rightBorder, int topBorder, int bottomBorder, Material mat)
        {
            int pass = -1;
            DrawTexture(screenRect, texture, sourceRect, leftBorder, rightBorder, topBorder, bottomBorder, mat, pass);
        }

        /// <summary>
        /// <para>Draw a texture in screen coordinates.</para>
        /// </summary>
        /// <param name="screenRect">Rectangle on the screen to use for the texture. In pixel coordinates with (0,0) in the upper-left corner.</param>
        /// <param name="texture">Texture to draw.</param>
        /// <param name="sourceRect">Region of the texture to use. In normalized coordinates with (0,0) in the bottom-left corner.</param>
        /// <param name="leftBorder">Number of pixels from the left that are not affected by scale.</param>
        /// <param name="rightBorder">Number of pixels from the right that are not affected by scale.</param>
        /// <param name="topBorder">Number of pixels from the top that are not affected by scale.</param>
        /// <param name="bottomBorder">Number of pixels from the bottom that are not affected by scale.</param>
        /// <param name="color">Color that modulates the output. The neutral value is (0.5, 0.5, 0.5, 0.5). Set as vertex color for the shader.</param>
        /// <param name="mat">Custom Material that can be used to draw the texture. If null is passed, a default material with the Internal-GUITexture.shader is used.</param>
        /// <param name="pass">If -1 (default), draws all passes in the material. Otherwise, draws given pass only.</param>
        [ExcludeFromDocs]
        public static void DrawTexture(Rect screenRect, Texture texture, Rect sourceRect, int leftBorder, int rightBorder, int topBorder, int bottomBorder, Color color, Material mat)
        {
            int pass = -1;
            DrawTexture(screenRect, texture, sourceRect, leftBorder, rightBorder, topBorder, bottomBorder, color, mat, pass);
        }

        public static void DrawTexture(Rect screenRect, Texture texture, Rect sourceRect, int leftBorder, int rightBorder, int topBorder, int bottomBorder, [UnityEngine.Internal.DefaultValue("null")] Material mat, [UnityEngine.Internal.DefaultValue("-1")] int pass)
        {
            Color32 color = new Color32(0x80, 0x80, 0x80, 0x80);
            DrawTextureImpl(screenRect, texture, sourceRect, leftBorder, rightBorder, topBorder, bottomBorder, (Color) color, mat, pass);
        }

        public static void DrawTexture(Rect screenRect, Texture texture, Rect sourceRect, int leftBorder, int rightBorder, int topBorder, int bottomBorder, Color color, [UnityEngine.Internal.DefaultValue("null")] Material mat, [UnityEngine.Internal.DefaultValue("-1")] int pass)
        {
            DrawTextureImpl(screenRect, texture, sourceRect, leftBorder, rightBorder, topBorder, bottomBorder, color, mat, pass);
        }

        private static void DrawTextureImpl(Rect screenRect, Texture texture, Rect sourceRect, int leftBorder, int rightBorder, int topBorder, int bottomBorder, Color color, Material mat, int pass)
        {
            Internal_DrawTextureArguments args = new Internal_DrawTextureArguments {
                screenRect = screenRect,
                sourceRect = sourceRect,
                leftBorder = leftBorder,
                rightBorder = rightBorder,
                topBorder = topBorder,
                bottomBorder = bottomBorder,
                color = color,
                pass = pass,
                texture = texture,
                mat = mat
            };
            Internal_DrawTexture(ref args);
        }

        /// <summary>
        /// <para>Execute a command buffer.</para>
        /// </summary>
        /// <param name="buffer">The buffer to execute.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void ExecuteCommandBuffer(CommandBuffer buffer);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern Array ExtractArrayFromList(object list);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void GetActiveColorBuffer(out RenderBuffer res);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void GetActiveDepthBuffer(out RenderBuffer res);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void Internal_BlitMaterial(Texture source, RenderTexture dest, Material mat, int pass, bool setRT);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void Internal_BlitMultiTap(Texture source, RenderTexture dest, Material mat, Vector2[] offsets);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_Internal_DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, ref Bounds bounds, ComputeBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, int layer, Camera camera);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_Internal_DrawMeshNow1(Mesh mesh, int subsetIndex, ref Vector3 position, ref Quaternion rotation);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_Internal_DrawMeshNow2(Mesh mesh, int subsetIndex, ref Matrix4x4 matrix);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void Internal_DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, Matrix4x4[] matrices, int count, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, int layer, Camera camera);
        private static void Internal_DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, Bounds bounds, ComputeBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, int layer, Camera camera)
        {
            INTERNAL_CALL_Internal_DrawMeshInstancedIndirect(mesh, submeshIndex, material, ref bounds, bufferWithArgs, argsOffset, properties, castShadows, receiveShadows, layer, camera);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void Internal_DrawMeshMatrix(ref Internal_DrawMeshMatrixArguments arguments, MaterialPropertyBlock properties, Material material, Mesh mesh, Camera camera);
        private static void Internal_DrawMeshNow1(Mesh mesh, int subsetIndex, Vector3 position, Quaternion rotation)
        {
            INTERNAL_CALL_Internal_DrawMeshNow1(mesh, subsetIndex, ref position, ref rotation);
        }

        private static void Internal_DrawMeshNow2(Mesh mesh, int subsetIndex, Matrix4x4 matrix)
        {
            INTERNAL_CALL_Internal_DrawMeshNow2(mesh, subsetIndex, ref matrix);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void Internal_DrawTexture(ref Internal_DrawTextureArguments args);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern int Internal_GetMaxDrawMeshInstanceCount();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void Internal_SetMRTFullSetup(RenderBuffer[] colorSA, out RenderBuffer depth, int mip, CubemapFace face, int depthSlice, RenderBufferLoadAction[] colorLoadSA, RenderBufferStoreAction[] colorStoreSA, RenderBufferLoadAction depthLoad, RenderBufferStoreAction depthStore);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void Internal_SetMRTSimple(RenderBuffer[] colorSA, out RenderBuffer depth, int mip, CubemapFace face, int depthSlice);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void Internal_SetNullRT();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void Internal_SetRandomWriteTargetBuffer(int index, ComputeBuffer uav, bool preserveCounterValue);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void Internal_SetRandomWriteTargetRT(int index, RenderTexture uav);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void Internal_SetRTSimple(out RenderBuffer color, out RenderBuffer depth, int mip, CubemapFace face, int depthSlice);
        [ExcludeFromDocs]
        public static void SetRandomWriteTarget(int index, ComputeBuffer uav)
        {
            bool preserveCounterValue = false;
            SetRandomWriteTarget(index, uav, preserveCounterValue);
        }

        /// <summary>
        /// <para>Set random write target for level pixel shaders.</para>
        /// </summary>
        /// <param name="index">Index of the random write target in the shader.</param>
        /// <param name="uav">RenderTexture to set as write target.</param>
        /// <param name="preserveCounterValue">Whether to leave the append/consume counter value unchanged.</param>
        public static void SetRandomWriteTarget(int index, RenderTexture uav)
        {
            Internal_SetRandomWriteTargetRT(index, uav);
        }

        /// <summary>
        /// <para>Set random write target for level pixel shaders.</para>
        /// </summary>
        /// <param name="index">Index of the random write target in the shader.</param>
        /// <param name="uav">RenderTexture to set as write target.</param>
        /// <param name="preserveCounterValue">Whether to leave the append/consume counter value unchanged.</param>
        public static void SetRandomWriteTarget(int index, ComputeBuffer uav, [UnityEngine.Internal.DefaultValue("false")] bool preserveCounterValue)
        {
            if (uav == null)
            {
                throw new ArgumentNullException("uav");
            }
            if (uav.m_Ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException("uav");
            }
            Internal_SetRandomWriteTargetBuffer(index, uav, preserveCounterValue);
        }

        /// <summary>
        /// <para>Sets current render target.</para>
        /// </summary>
        /// <param name="rt">RenderTexture to set as active render target.</param>
        /// <param name="mipLevel">Mipmap level to render into (use 0 if not mipmapped).</param>
        /// <param name="face">Cubemap face to render into (use Unknown if not a cubemap).</param>
        /// <param name="depthSlice">Depth slice to render into (use 0 if not a 3D or 2DArray render target).</param>
        /// <param name="colorBuffer">Color buffer to render into.</param>
        /// <param name="depthBuffer">Depth buffer to render into.</param>
        /// <param name="colorBuffers">
        /// Color buffers to render into (for multiple render target effects).</param>
        /// <param name="setup">Full render target setup information.</param>
        public static void SetRenderTarget(RenderTargetSetup setup)
        {
            SetRenderTargetImpl(setup);
        }

        /// <summary>
        /// <para>Sets current render target.</para>
        /// </summary>
        /// <param name="rt">RenderTexture to set as active render target.</param>
        /// <param name="mipLevel">Mipmap level to render into (use 0 if not mipmapped).</param>
        /// <param name="face">Cubemap face to render into (use Unknown if not a cubemap).</param>
        /// <param name="depthSlice">Depth slice to render into (use 0 if not a 3D or 2DArray render target).</param>
        /// <param name="colorBuffer">Color buffer to render into.</param>
        /// <param name="depthBuffer">Depth buffer to render into.</param>
        /// <param name="colorBuffers">
        /// Color buffers to render into (for multiple render target effects).</param>
        /// <param name="setup">Full render target setup information.</param>
        public static void SetRenderTarget(RenderTexture rt)
        {
            SetRenderTargetImpl(rt, 0, CubemapFace.Unknown, 0);
        }

        /// <summary>
        /// <para>Sets current render target.</para>
        /// </summary>
        /// <param name="rt">RenderTexture to set as active render target.</param>
        /// <param name="mipLevel">Mipmap level to render into (use 0 if not mipmapped).</param>
        /// <param name="face">Cubemap face to render into (use Unknown if not a cubemap).</param>
        /// <param name="depthSlice">Depth slice to render into (use 0 if not a 3D or 2DArray render target).</param>
        /// <param name="colorBuffer">Color buffer to render into.</param>
        /// <param name="depthBuffer">Depth buffer to render into.</param>
        /// <param name="colorBuffers">
        /// Color buffers to render into (for multiple render target effects).</param>
        /// <param name="setup">Full render target setup information.</param>
        public static void SetRenderTarget(RenderBuffer colorBuffer, RenderBuffer depthBuffer)
        {
            SetRenderTargetImpl(colorBuffer, depthBuffer, 0, CubemapFace.Unknown, 0);
        }

        /// <summary>
        /// <para>Sets current render target.</para>
        /// </summary>
        /// <param name="rt">RenderTexture to set as active render target.</param>
        /// <param name="mipLevel">Mipmap level to render into (use 0 if not mipmapped).</param>
        /// <param name="face">Cubemap face to render into (use Unknown if not a cubemap).</param>
        /// <param name="depthSlice">Depth slice to render into (use 0 if not a 3D or 2DArray render target).</param>
        /// <param name="colorBuffer">Color buffer to render into.</param>
        /// <param name="depthBuffer">Depth buffer to render into.</param>
        /// <param name="colorBuffers">
        /// Color buffers to render into (for multiple render target effects).</param>
        /// <param name="setup">Full render target setup information.</param>
        public static void SetRenderTarget(RenderTexture rt, int mipLevel)
        {
            SetRenderTargetImpl(rt, mipLevel, CubemapFace.Unknown, 0);
        }

        /// <summary>
        /// <para>Sets current render target.</para>
        /// </summary>
        /// <param name="rt">RenderTexture to set as active render target.</param>
        /// <param name="mipLevel">Mipmap level to render into (use 0 if not mipmapped).</param>
        /// <param name="face">Cubemap face to render into (use Unknown if not a cubemap).</param>
        /// <param name="depthSlice">Depth slice to render into (use 0 if not a 3D or 2DArray render target).</param>
        /// <param name="colorBuffer">Color buffer to render into.</param>
        /// <param name="depthBuffer">Depth buffer to render into.</param>
        /// <param name="colorBuffers">
        /// Color buffers to render into (for multiple render target effects).</param>
        /// <param name="setup">Full render target setup information.</param>
        public static void SetRenderTarget(RenderBuffer[] colorBuffers, RenderBuffer depthBuffer)
        {
            SetRenderTargetImpl(colorBuffers, depthBuffer, 0, CubemapFace.Unknown, 0);
        }

        /// <summary>
        /// <para>Sets current render target.</para>
        /// </summary>
        /// <param name="rt">RenderTexture to set as active render target.</param>
        /// <param name="mipLevel">Mipmap level to render into (use 0 if not mipmapped).</param>
        /// <param name="face">Cubemap face to render into (use Unknown if not a cubemap).</param>
        /// <param name="depthSlice">Depth slice to render into (use 0 if not a 3D or 2DArray render target).</param>
        /// <param name="colorBuffer">Color buffer to render into.</param>
        /// <param name="depthBuffer">Depth buffer to render into.</param>
        /// <param name="colorBuffers">
        /// Color buffers to render into (for multiple render target effects).</param>
        /// <param name="setup">Full render target setup information.</param>
        public static void SetRenderTarget(RenderBuffer colorBuffer, RenderBuffer depthBuffer, int mipLevel)
        {
            SetRenderTargetImpl(colorBuffer, depthBuffer, mipLevel, CubemapFace.Unknown, 0);
        }

        /// <summary>
        /// <para>Sets current render target.</para>
        /// </summary>
        /// <param name="rt">RenderTexture to set as active render target.</param>
        /// <param name="mipLevel">Mipmap level to render into (use 0 if not mipmapped).</param>
        /// <param name="face">Cubemap face to render into (use Unknown if not a cubemap).</param>
        /// <param name="depthSlice">Depth slice to render into (use 0 if not a 3D or 2DArray render target).</param>
        /// <param name="colorBuffer">Color buffer to render into.</param>
        /// <param name="depthBuffer">Depth buffer to render into.</param>
        /// <param name="colorBuffers">
        /// Color buffers to render into (for multiple render target effects).</param>
        /// <param name="setup">Full render target setup information.</param>
        public static void SetRenderTarget(RenderTexture rt, int mipLevel, CubemapFace face)
        {
            SetRenderTargetImpl(rt, mipLevel, face, 0);
        }

        /// <summary>
        /// <para>Sets current render target.</para>
        /// </summary>
        /// <param name="rt">RenderTexture to set as active render target.</param>
        /// <param name="mipLevel">Mipmap level to render into (use 0 if not mipmapped).</param>
        /// <param name="face">Cubemap face to render into (use Unknown if not a cubemap).</param>
        /// <param name="depthSlice">Depth slice to render into (use 0 if not a 3D or 2DArray render target).</param>
        /// <param name="colorBuffer">Color buffer to render into.</param>
        /// <param name="depthBuffer">Depth buffer to render into.</param>
        /// <param name="colorBuffers">
        /// Color buffers to render into (for multiple render target effects).</param>
        /// <param name="setup">Full render target setup information.</param>
        public static void SetRenderTarget(RenderBuffer colorBuffer, RenderBuffer depthBuffer, int mipLevel, CubemapFace face)
        {
            SetRenderTargetImpl(colorBuffer, depthBuffer, mipLevel, face, 0);
        }

        /// <summary>
        /// <para>Sets current render target.</para>
        /// </summary>
        /// <param name="rt">RenderTexture to set as active render target.</param>
        /// <param name="mipLevel">Mipmap level to render into (use 0 if not mipmapped).</param>
        /// <param name="face">Cubemap face to render into (use Unknown if not a cubemap).</param>
        /// <param name="depthSlice">Depth slice to render into (use 0 if not a 3D or 2DArray render target).</param>
        /// <param name="colorBuffer">Color buffer to render into.</param>
        /// <param name="depthBuffer">Depth buffer to render into.</param>
        /// <param name="colorBuffers">
        /// Color buffers to render into (for multiple render target effects).</param>
        /// <param name="setup">Full render target setup information.</param>
        public static void SetRenderTarget(RenderTexture rt, int mipLevel, CubemapFace face, int depthSlice)
        {
            SetRenderTargetImpl(rt, mipLevel, face, depthSlice);
        }

        /// <summary>
        /// <para>Sets current render target.</para>
        /// </summary>
        /// <param name="rt">RenderTexture to set as active render target.</param>
        /// <param name="mipLevel">Mipmap level to render into (use 0 if not mipmapped).</param>
        /// <param name="face">Cubemap face to render into (use Unknown if not a cubemap).</param>
        /// <param name="depthSlice">Depth slice to render into (use 0 if not a 3D or 2DArray render target).</param>
        /// <param name="colorBuffer">Color buffer to render into.</param>
        /// <param name="depthBuffer">Depth buffer to render into.</param>
        /// <param name="colorBuffers">
        /// Color buffers to render into (for multiple render target effects).</param>
        /// <param name="setup">Full render target setup information.</param>
        public static void SetRenderTarget(RenderBuffer colorBuffer, RenderBuffer depthBuffer, int mipLevel, CubemapFace face, int depthSlice)
        {
            SetRenderTargetImpl(colorBuffer, depthBuffer, mipLevel, face, depthSlice);
        }

        internal static void SetRenderTargetImpl(RenderTargetSetup setup)
        {
            if (setup.color.Length == 0)
            {
                throw new ArgumentException("Invalid color buffer count for SetRenderTarget");
            }
            if (setup.color.Length != setup.colorLoad.Length)
            {
                throw new ArgumentException("Color LoadAction and Buffer arrays have different sizes");
            }
            if (setup.color.Length != setup.colorStore.Length)
            {
                throw new ArgumentException("Color StoreAction and Buffer arrays have different sizes");
            }
            foreach (RenderBufferLoadAction action in setup.colorLoad)
            {
                CheckLoadActionValid(action, "Color");
            }
            foreach (RenderBufferStoreAction action2 in setup.colorStore)
            {
                CheckStoreActionValid(action2, "Color");
            }
            CheckLoadActionValid(setup.depthLoad, "Depth");
            CheckStoreActionValid(setup.depthStore, "Depth");
            if ((setup.cubemapFace < CubemapFace.Unknown) || (setup.cubemapFace > CubemapFace.NegativeZ))
            {
                throw new ArgumentException("Bad CubemapFace provided");
            }
            Internal_SetMRTFullSetup(setup.color, out setup.depth, setup.mipLevel, setup.cubemapFace, setup.depthSlice, setup.colorLoad, setup.colorStore, setup.depthLoad, setup.depthStore);
        }

        internal static void SetRenderTargetImpl(RenderTexture rt, int mipLevel, CubemapFace face, int depthSlice)
        {
            if (rt != null)
            {
                SetRenderTargetImpl(rt.colorBuffer, rt.depthBuffer, mipLevel, face, depthSlice);
            }
            else
            {
                Internal_SetNullRT();
            }
        }

        internal static void SetRenderTargetImpl(RenderBuffer colorBuffer, RenderBuffer depthBuffer, int mipLevel, CubemapFace face, int depthSlice)
        {
            RenderBuffer color = colorBuffer;
            RenderBuffer depth = depthBuffer;
            Internal_SetRTSimple(out color, out depth, mipLevel, face, depthSlice);
        }

        internal static void SetRenderTargetImpl(RenderBuffer[] colorBuffers, RenderBuffer depthBuffer, int mipLevel, CubemapFace face, int depthSlice)
        {
            RenderBuffer depth = depthBuffer;
            Internal_SetMRTSimple(colorBuffers, out depth, mipLevel, face, depthSlice);
        }

        /// <summary>
        /// <para>Currently active color buffer (Read Only).</para>
        /// </summary>
        public static RenderBuffer activeColorBuffer
        {
            get
            {
                RenderBuffer buffer;
                GetActiveColorBuffer(out buffer);
                return buffer;
            }
        }

        /// <summary>
        /// <para>Currently active depth/stencil buffer (Read Only).</para>
        /// </summary>
        public static RenderBuffer activeDepthBuffer
        {
            get
            {
                RenderBuffer buffer;
                GetActiveDepthBuffer(out buffer);
                return buffer;
            }
        }

        /// <summary>
        /// <para>Graphics Tier classification for current device.
        /// Changing this value affects any subsequently loaded shaders. Initially this value is auto-detected from the hardware in use.</para>
        /// </summary>
        public static GraphicsTier activeTier { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Property deviceName has been deprecated. Use SystemInfo.graphicsDeviceName instead (UnityUpgradable) -> UnityEngine.SystemInfo.graphicsDeviceName", true)]
        public static string deviceName =>
            SystemInfo.graphicsDeviceName;

        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Property deviceVendor has been deprecated. Use SystemInfo.graphicsDeviceVendor instead (UnityUpgradable) -> UnityEngine.SystemInfo.graphicsDeviceVendor", true)]
        public static string deviceVendor =>
            SystemInfo.graphicsDeviceVendor;

        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Property deviceVersion has been deprecated. Use SystemInfo.graphicsDeviceVersion instead (UnityUpgradable) -> UnityEngine.SystemInfo.graphicsDeviceVersion", true)]
        public static string deviceVersion =>
            SystemInfo.graphicsDeviceVersion;
    }
}

