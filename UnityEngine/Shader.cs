namespace UnityEngine
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Rendering;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Shader scripts used for all rendering.</para>
    /// </summary>
    public sealed class Shader : UnityEngine.Object
    {
        /// <summary>
        /// <para>Unset a global shader keyword.</para>
        /// </summary>
        /// <param name="keyword"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void DisableKeyword(string keyword);
        /// <summary>
        /// <para>Set a global shader keyword.</para>
        /// </summary>
        /// <param name="keyword"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void EnableKeyword(string keyword);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern Array ExtractArrayFromList(object list);
        /// <summary>
        /// <para>Finds a shader with the given name.</para>
        /// </summary>
        /// <param name="name"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern Shader Find(string name);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern Shader FindBuiltin(string name);
        /// <summary>
        /// <para>Gets a global color property for all shaders previously set using SetGlobalColor.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameID"></param>
        public static Color GetGlobalColor(int nameID) => 
            GetGlobalColorImpl(nameID);

        /// <summary>
        /// <para>Gets a global color property for all shaders previously set using SetGlobalColor.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameID"></param>
        public static Color GetGlobalColor(string name) => 
            GetGlobalColor(PropertyToID(name));

        private static Color GetGlobalColorImpl(int nameID)
        {
            Color color;
            INTERNAL_CALL_GetGlobalColorImpl(nameID, out color);
            return color;
        }

        /// <summary>
        /// <para>Gets a global float property for all shaders previously set using SetGlobalFloat.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameID"></param>
        public static float GetGlobalFloat(int nameID) => 
            GetGlobalFloatImpl(nameID);

        /// <summary>
        /// <para>Gets a global float property for all shaders previously set using SetGlobalFloat.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameID"></param>
        public static float GetGlobalFloat(string name) => 
            GetGlobalFloat(PropertyToID(name));

        /// <summary>
        /// <para>Gets a global float array for all shaders previously set using SetGlobalFloatArray.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameID"></param>
        public static float[] GetGlobalFloatArray(int nameID) => 
            GetGlobalFloatArrayImpl(nameID);

        /// <summary>
        /// <para>Gets a global float array for all shaders previously set using SetGlobalFloatArray.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameID"></param>
        public static float[] GetGlobalFloatArray(string name) => 
            GetGlobalFloatArray(PropertyToID(name));

        public static void GetGlobalFloatArray(int nameID, List<float> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            GetGlobalFloatArrayImplList(nameID, values);
        }

        public static void GetGlobalFloatArray(string name, List<float> values)
        {
            GetGlobalFloatArray(PropertyToID(name), values);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern float[] GetGlobalFloatArrayImpl(int nameID);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void GetGlobalFloatArrayImplList(int nameID, object list);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern float GetGlobalFloatImpl(int nameID);
        /// <summary>
        /// <para>Gets a global int property for all shaders previously set using SetGlobalInt.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameID"></param>
        public static int GetGlobalInt(int nameID) => 
            GetGlobalIntImpl(nameID);

        /// <summary>
        /// <para>Gets a global int property for all shaders previously set using SetGlobalInt.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameID"></param>
        public static int GetGlobalInt(string name) => 
            GetGlobalInt(PropertyToID(name));

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern int GetGlobalIntImpl(int nameID);
        /// <summary>
        /// <para>Gets a global matrix property for all shaders previously set using SetGlobalMatrix.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameID"></param>
        public static Matrix4x4 GetGlobalMatrix(int nameID) => 
            GetGlobalMatrixImpl(nameID);

        /// <summary>
        /// <para>Gets a global matrix property for all shaders previously set using SetGlobalMatrix.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameID"></param>
        public static Matrix4x4 GetGlobalMatrix(string name) => 
            GetGlobalMatrix(PropertyToID(name));

        /// <summary>
        /// <para>Gets a global matrix array for all shaders previously set using SetGlobalMatrixArray.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameID"></param>
        public static Matrix4x4[] GetGlobalMatrixArray(int nameID) => 
            GetGlobalMatrixArrayImpl(nameID);

        /// <summary>
        /// <para>Gets a global matrix array for all shaders previously set using SetGlobalMatrixArray.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameID"></param>
        public static Matrix4x4[] GetGlobalMatrixArray(string name) => 
            GetGlobalMatrixArray(PropertyToID(name));

        public static void GetGlobalMatrixArray(int nameID, List<Matrix4x4> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            GetGlobalMatrixArrayImplList(nameID, values);
        }

        public static void GetGlobalMatrixArray(string name, List<Matrix4x4> values)
        {
            GetGlobalMatrixArray(PropertyToID(name), values);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern Matrix4x4[] GetGlobalMatrixArrayImpl(int nameID);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void GetGlobalMatrixArrayImplList(int nameID, object list);
        private static Matrix4x4 GetGlobalMatrixImpl(int nameID)
        {
            Matrix4x4 matrixx;
            INTERNAL_CALL_GetGlobalMatrixImpl(nameID, out matrixx);
            return matrixx;
        }

        /// <summary>
        /// <para>Gets a global texture property for all shaders previously set using SetGlobalTexture.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameID"></param>
        public static Texture GetGlobalTexture(int nameID) => 
            GetGlobalTextureImpl(nameID);

        /// <summary>
        /// <para>Gets a global texture property for all shaders previously set using SetGlobalTexture.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameID"></param>
        public static Texture GetGlobalTexture(string name) => 
            GetGlobalTexture(PropertyToID(name));

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern Texture GetGlobalTextureImpl(int nameID);
        /// <summary>
        /// <para>Gets a global vector property for all shaders previously set using SetGlobalVector.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameID"></param>
        public static Vector4 GetGlobalVector(int nameID) => 
            GetGlobalVectorImpl(nameID);

        /// <summary>
        /// <para>Gets a global vector property for all shaders previously set using SetGlobalVector.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameID"></param>
        public static Vector4 GetGlobalVector(string name) => 
            GetGlobalVector(PropertyToID(name));

        /// <summary>
        /// <para>Gets a global vector array for all shaders previously set using SetGlobalVectorArray.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameID"></param>
        public static Vector4[] GetGlobalVectorArray(int nameID) => 
            GetGlobalVectorArrayImpl(nameID);

        /// <summary>
        /// <para>Gets a global vector array for all shaders previously set using SetGlobalVectorArray.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameID"></param>
        public static Vector4[] GetGlobalVectorArray(string name) => 
            GetGlobalVectorArray(PropertyToID(name));

        public static void GetGlobalVectorArray(int nameID, List<Vector4> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            GetGlobalVectorArrayImplList(nameID, values);
        }

        public static void GetGlobalVectorArray(string name, List<Vector4> values)
        {
            GetGlobalVectorArray(PropertyToID(name), values);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern Vector4[] GetGlobalVectorArrayImpl(int nameID);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void GetGlobalVectorArrayImplList(int nameID, object list);
        private static Vector4 GetGlobalVectorImpl(int nameID)
        {
            Vector4 vector;
            INTERNAL_CALL_GetGlobalVectorImpl(nameID, out vector);
            return vector;
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern string IDToProperty(int id);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetGlobalColorImpl(int nameID, out Color value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetGlobalMatrixImpl(int nameID, out Matrix4x4 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetGlobalVectorImpl(int nameID, out Vector4 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_SetGlobalColorImpl(int nameID, ref Color value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_SetGlobalMatrixImpl(int nameID, ref Matrix4x4 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_SetGlobalVectorImpl(int nameID, ref Vector4 value);
        /// <summary>
        /// <para>Is global shader keyword enabled?</para>
        /// </summary>
        /// <param name="keyword"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool IsKeywordEnabled(string keyword);
        /// <summary>
        /// <para>Gets unique identifier for a shader property name.</para>
        /// </summary>
        /// <param name="name">Shader property name.</param>
        /// <returns>
        /// <para>Unique integer for the name.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern int PropertyToID(string name);
        /// <summary>
        /// <para>Sets a global compute buffer property for all shaders.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="buffer"></param>
        /// <param name="nameID"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void SetGlobalBuffer(int nameID, ComputeBuffer buffer);
        /// <summary>
        /// <para>Sets a global compute buffer property for all shaders.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="buffer"></param>
        /// <param name="nameID"></param>
        public static void SetGlobalBuffer(string name, ComputeBuffer buffer)
        {
            SetGlobalBuffer(PropertyToID(name), buffer);
        }

        /// <summary>
        /// <para>Sets a global color property for all shaders.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="nameID"></param>
        public static void SetGlobalColor(int nameID, Color value)
        {
            SetGlobalColorImpl(nameID, value);
        }

        /// <summary>
        /// <para>Sets a global color property for all shaders.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="nameID"></param>
        public static void SetGlobalColor(string name, Color value)
        {
            SetGlobalColor(PropertyToID(name), value);
        }

        private static void SetGlobalColorImpl(int nameID, Color value)
        {
            INTERNAL_CALL_SetGlobalColorImpl(nameID, ref value);
        }

        /// <summary>
        /// <para>Sets a global float property for all shaders.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="nameID"></param>
        public static void SetGlobalFloat(int nameID, float value)
        {
            SetGlobalFloatImpl(nameID, value);
        }

        /// <summary>
        /// <para>Sets a global float property for all shaders.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="nameID"></param>
        public static void SetGlobalFloat(string name, float value)
        {
            SetGlobalFloat(PropertyToID(name), value);
        }

        public static void SetGlobalFloatArray(int nameID, List<float> values)
        {
            SetGlobalFloatArray(nameID, (float[]) ExtractArrayFromList(values));
        }

        /// <summary>
        /// <para>Sets a global float array property for all shaders.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="values"></param>
        /// <param name="nameID"></param>
        public static void SetGlobalFloatArray(int nameID, float[] values)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            if (values.Length == 0)
            {
                throw new ArgumentException("Zero-sized array is not allowed.");
            }
            SetGlobalFloatArrayImpl(nameID, values);
        }

        public static void SetGlobalFloatArray(string name, List<float> values)
        {
            SetGlobalFloatArray(PropertyToID(name), values);
        }

        /// <summary>
        /// <para>Sets a global float array property for all shaders.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="values"></param>
        /// <param name="nameID"></param>
        public static void SetGlobalFloatArray(string name, float[] values)
        {
            SetGlobalFloatArray(PropertyToID(name), values);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void SetGlobalFloatArrayImpl(int nameID, float[] values);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void SetGlobalFloatImpl(int nameID, float value);
        /// <summary>
        /// <para>Sets a global int property for all shaders.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="nameID"></param>
        public static void SetGlobalInt(int nameID, int value)
        {
            SetGlobalIntImpl(nameID, value);
        }

        /// <summary>
        /// <para>Sets a global int property for all shaders.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="nameID"></param>
        public static void SetGlobalInt(string name, int value)
        {
            SetGlobalInt(PropertyToID(name), value);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void SetGlobalIntImpl(int nameID, int value);
        /// <summary>
        /// <para>Sets a global matrix property for all shaders.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="nameID"></param>
        public static void SetGlobalMatrix(int nameID, Matrix4x4 value)
        {
            SetGlobalMatrixImpl(nameID, value);
        }

        /// <summary>
        /// <para>Sets a global matrix property for all shaders.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="nameID"></param>
        public static void SetGlobalMatrix(string name, Matrix4x4 value)
        {
            SetGlobalMatrix(PropertyToID(name), value);
        }

        public static void SetGlobalMatrixArray(int nameID, List<Matrix4x4> values)
        {
            SetGlobalMatrixArray(nameID, (Matrix4x4[]) ExtractArrayFromList(values));
        }

        /// <summary>
        /// <para>Sets a global matrix array property for all shaders.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="values"></param>
        /// <param name="nameID"></param>
        public static void SetGlobalMatrixArray(int nameID, Matrix4x4[] values)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            if (values.Length == 0)
            {
                throw new ArgumentException("Zero-sized array is not allowed.");
            }
            SetGlobalMatrixArrayImpl(nameID, values);
        }

        public static void SetGlobalMatrixArray(string name, List<Matrix4x4> values)
        {
            SetGlobalMatrixArray(PropertyToID(name), values);
        }

        /// <summary>
        /// <para>Sets a global matrix array property for all shaders.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="values"></param>
        /// <param name="nameID"></param>
        public static void SetGlobalMatrixArray(string name, Matrix4x4[] values)
        {
            SetGlobalMatrixArray(PropertyToID(name), values);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void SetGlobalMatrixArrayImpl(int nameID, Matrix4x4[] values);
        private static void SetGlobalMatrixImpl(int nameID, Matrix4x4 value)
        {
            INTERNAL_CALL_SetGlobalMatrixImpl(nameID, ref value);
        }

        [Obsolete("SetGlobalTexGenMode is not supported anymore. Use programmable shaders to achieve the same effect.", true)]
        public static void SetGlobalTexGenMode(string propertyName, TexGenMode mode)
        {
        }

        /// <summary>
        /// <para>Sets a global texture property for all shaders.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="nameID"></param>
        public static void SetGlobalTexture(int nameID, Texture value)
        {
            SetGlobalTextureImpl(nameID, value);
        }

        /// <summary>
        /// <para>Sets a global texture property for all shaders.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="nameID"></param>
        public static void SetGlobalTexture(string name, Texture value)
        {
            SetGlobalTexture(PropertyToID(name), value);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void SetGlobalTextureImpl(int nameID, Texture value);
        [Obsolete("SetGlobalTextureMatrixName is not supported anymore. Use programmable shaders to achieve the same effect.", true)]
        public static void SetGlobalTextureMatrixName(string propertyName, string matrixName)
        {
        }

        /// <summary>
        /// <para>Sets a global vector property for all shaders.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="nameID"></param>
        public static void SetGlobalVector(int nameID, Vector4 value)
        {
            SetGlobalVectorImpl(nameID, value);
        }

        /// <summary>
        /// <para>Sets a global vector property for all shaders.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="nameID"></param>
        public static void SetGlobalVector(string name, Vector4 value)
        {
            SetGlobalVector(PropertyToID(name), value);
        }

        public static void SetGlobalVectorArray(int nameID, List<Vector4> values)
        {
            SetGlobalVectorArray(nameID, (Vector4[]) ExtractArrayFromList(values));
        }

        /// <summary>
        /// <para>Sets a global vector array property for all shaders.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="values"></param>
        /// <param name="nameID"></param>
        public static void SetGlobalVectorArray(int nameID, Vector4[] values)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            if (values.Length == 0)
            {
                throw new ArgumentException("Zero-sized array is not allowed.");
            }
            SetGlobalVectorArrayImpl(nameID, values);
        }

        public static void SetGlobalVectorArray(string name, List<Vector4> values)
        {
            SetGlobalVectorArray(PropertyToID(name), values);
        }

        /// <summary>
        /// <para>Sets a global vector array property for all shaders.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="values"></param>
        /// <param name="nameID"></param>
        public static void SetGlobalVectorArray(string name, Vector4[] values)
        {
            SetGlobalVectorArray(PropertyToID(name), values);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void SetGlobalVectorArrayImpl(int nameID, Vector4[] values);
        private static void SetGlobalVectorImpl(int nameID, Vector4 value)
        {
            INTERNAL_CALL_SetGlobalVectorImpl(nameID, ref value);
        }

        /// <summary>
        /// <para>Fully load all shaders to prevent future performance hiccups.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void WarmupAllShaders();

        internal string customEditor { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        internal DisableBatchingType disableBatching { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Shader LOD level for all shaders.</para>
        /// </summary>
        public static int globalMaximumLOD { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Render pipeline currently in use.</para>
        /// </summary>
        public static string globalRenderPipeline { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Shader hardware tier classification for current device.</para>
        /// </summary>
        [Obsolete("Use Graphics.activeTier instead (UnityUpgradable) -> UnityEngine.Graphics.activeTier", false)]
        public static ShaderHardwareTier globalShaderHardwareTier
        {
            get => 
                ((ShaderHardwareTier) Graphics.activeTier);
            set
            {
                Graphics.activeTier = (GraphicsTier) value;
            }
        }

        /// <summary>
        /// <para>Can this shader run on the end-users graphics card? (Read Only)</para>
        /// </summary>
        public bool isSupported { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Shader LOD level for this shader.</para>
        /// </summary>
        public int maximumLOD { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Render queue of this shader. (Read Only)</para>
        /// </summary>
        public int renderQueue { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }
    }
}

