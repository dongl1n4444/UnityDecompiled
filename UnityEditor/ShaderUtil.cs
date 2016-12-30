namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Utility functions to assist with working with shaders from the editor.</para>
    /// </summary>
    public sealed class ShaderUtil
    {
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern bool AddNewShaderToCollection(Shader shader, ShaderVariantCollection collection);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void ApplyMaterialPropertyBlockToMaterialProperty(MaterialPropertyBlock propertyBlock, MaterialProperty materialProperty);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void ApplyMaterialPropertyToMaterialPropertyBlock(MaterialProperty materialProperty, int propertyMask, MaterialPropertyBlock propertyBlock);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void ApplyProperty(MaterialProperty prop, int propertyMask, string undoName);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void CalculateFogStrippingFromCurrentScene();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void CalculateLightmapStrippingFromCurrentScene();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void ClearCurrentShaderVariantCollection();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern Shader CreateShaderAsset(string source);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern bool DoesIgnoreProjector(Shader s);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void FetchCachedErrors(Shader s);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern int GetAvailableShaderCompilerPlatforms();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern int GetComboCount(Shader s, bool usedBySceneOnly);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern int GetComputeShaderErrorCount(ComputeShader s);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern ShaderError[] GetComputeShaderErrors(ComputeShader s);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern int GetComputeShaderPlatformCount(ComputeShader s);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern int GetComputeShaderPlatformKernelCount(ComputeShader s, int platformIndex);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern string GetComputeShaderPlatformKernelName(ComputeShader s, int platformIndex, int kernelIndex);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern GraphicsDeviceType GetComputeShaderPlatformType(ComputeShader s, int platformIndex);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern int GetCurrentShaderVariantCollectionShaderCount();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern int GetCurrentShaderVariantCollectionVariantCount();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern string GetDependency(Shader s, string name);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern int GetLOD(Shader s);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern MaterialProperty[] GetMaterialProperties(Object[] mats);
        internal static MaterialProperty GetMaterialProperty(Object[] mats, int propertyIndex) => 
            GetMaterialProperty_Index(mats, propertyIndex);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern MaterialProperty GetMaterialProperty(Object[] mats, string name);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern MaterialProperty GetMaterialProperty_Index(Object[] mats, int propertyIndex);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern int GetMaterialRawRenderQueue(Material mat);
        /// <summary>
        /// <para>Get the number of properties in Shader s.</para>
        /// </summary>
        /// <param name="s">The shader to check against.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern int GetPropertyCount(Shader s);
        /// <summary>
        /// <para>Get the description of the shader propery at index propertyIdx of Shader s.</para>
        /// </summary>
        /// <param name="s">The shader to check against.</param>
        /// <param name="propertyIdx">The property index to use.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern string GetPropertyDescription(Shader s, int propertyIdx);
        /// <summary>
        /// <para>Get the name of the shader propery at index propertyIdx of Shader s.</para>
        /// </summary>
        /// <param name="s">The shader to check against.</param>
        /// <param name="propertyIdx">The property index to use.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern string GetPropertyName(Shader s, int propertyIdx);
        /// <summary>
        /// <para>Get the ShaderProperyType of the shader propery at index propertyIdx of Shader s.</para>
        /// </summary>
        /// <param name="s">The shader to check against.</param>
        /// <param name="propertyIdx">The property index to use.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern ShaderPropertyType GetPropertyType(Shader s, int propertyIdx);
        /// <summary>
        /// <para>Get Limits for a range property at index propertyIdx of Shader s.</para>
        /// </summary>
        /// <param name="defminmax">Which value to get: 0 = default, 1 = min, 2 = max.</param>
        /// <param name="s">The shader to check against.</param>
        /// <param name="propertyIdx">The property index to use.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern float GetRangeLimits(Shader s, int propertyIdx, int defminmax);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern int GetRenderQueue(Shader s);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern int GetShaderErrorCount(Shader s);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern ShaderError[] GetShaderErrors(Shader s);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern string[] GetShaderPropertyAttributes(Shader s, string name);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void GetShaderVariantEntries(Shader shader, ShaderVariantCollection skipAlreadyInCollection, out int[] types, out string[] keywords);
        /// <summary>
        /// <para>Gets texture dimension of a shader property.</para>
        /// </summary>
        /// <param name="s">The shader to get the property from.</param>
        /// <param name="propertyIdx">The property index to use.</param>
        /// <returns>
        /// <para>Texture dimension.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern TextureDimension GetTexDim(Shader s, int propertyIdx);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern int GetTextureBindingIndex(Shader s, int texturePropertyID);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern bool HasFixedFunctionShaders(Shader s);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern bool HasInstancing(Shader s);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern bool HasShaderSnippets(Shader s);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern bool HasShadowCasterPass(Shader s);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern bool HasSurfaceShaders(Shader s);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern bool HasTangentChannel(Shader s);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_get_rawScissorRect(out Rect value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_get_rawViewportRect(out Rect value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_set_rawScissorRect(ref Rect value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_set_rawViewportRect(ref Rect value);
        /// <summary>
        /// <para>Is the shader propery at index propertyIdx of Shader s hidden?</para>
        /// </summary>
        /// <param name="s">The shader to check against.</param>
        /// <param name="propertyIdx">The property index to use.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool IsShaderPropertyHidden(Shader s, int propertyIdx);
        internal static bool MaterialsUseInstancingShader(SerializedProperty materialsArray)
        {
            if (!materialsArray.hasMultipleDifferentValues)
            {
                for (int i = 0; i < materialsArray.arraySize; i++)
                {
                    Material objectReferenceValue = materialsArray.GetArrayElementAtIndex(i).objectReferenceValue as Material;
                    if (((objectReferenceValue != null) && (objectReferenceValue.shader != null)) && HasInstancing(objectReferenceValue.shader))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void OpenCompiledComputeShader(ComputeShader shader, bool allVariantsAndPlatforms);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void OpenCompiledShader(Shader shader, int mode, int customPlatformsMask, bool includeAllVariants);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void OpenGeneratedFixedFunctionShader(Shader shader);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void OpenParsedSurfaceShader(Shader shader);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void OpenShaderCombinations(Shader shader, bool usedBySceneOnly);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void OpenShaderSnippets(Shader shader);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void OpenSystemShaderIncludeError(string includeName, int line);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void RecreateGfxDevice();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void RecreateSkinnedMeshResources();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void ReloadAllShaders();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void SaveCurrentShaderVariantCollection(string path);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void UpdateShaderAsset(Shader shader, string source);

        internal static bool hardwareSupportsFullNPOT { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Does the current hardware support render textues.</para>
        /// </summary>
        public static bool hardwareSupportsRectRenderTexture { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        internal static Rect rawScissorRect
        {
            get
            {
                Rect rect;
                INTERNAL_get_rawScissorRect(out rect);
                return rect;
            }
            set
            {
                INTERNAL_set_rawScissorRect(ref value);
            }
        }

        internal static Rect rawViewportRect
        {
            get
            {
                Rect rect;
                INTERNAL_get_rawViewportRect(out rect);
                return rect;
            }
            set
            {
                INTERNAL_set_rawViewportRect(ref value);
            }
        }

        internal enum ShaderCompilerPlatformType
        {
            OpenGL,
            D3D9,
            Xbox360,
            PS3,
            D3D11,
            OpenGLES20,
            OpenGLES20Desktop,
            Flash,
            D3D11_9x,
            OpenGLES30,
            PSVita,
            PS4,
            XboxOne,
            PSM,
            Metal,
            OpenGLCore,
            N3DS,
            WiiU,
            Vulkan,
            Switch,
            Count
        }

        [Obsolete("Use UnityEngine.Rendering.TextureDimension instead.")]
        public enum ShaderPropertyTexDim
        {
            TexDim2D = 2,
            TexDim3D = 3,
            TexDimAny = 6,
            TexDimCUBE = 4,
            TexDimNone = 0
        }

        /// <summary>
        /// <para>Type of a given texture property.</para>
        /// </summary>
        public enum ShaderPropertyType
        {
            Color,
            Vector,
            Float,
            Range,
            TexEnv
        }
    }
}

