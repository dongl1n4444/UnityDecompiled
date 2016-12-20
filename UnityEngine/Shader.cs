namespace UnityEngine
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Rendering;

    /// <summary>
    /// <para>Shader scripts used for all rendering.</para>
    /// </summary>
    public sealed class Shader : UnityEngine.Object
    {
        /// <summary>
        /// <para>Unset a global shader keyword.</para>
        /// </summary>
        /// <param name="keyword"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void DisableKeyword(string keyword);
        /// <summary>
        /// <para>Set a global shader keyword.</para>
        /// </summary>
        /// <param name="keyword"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void EnableKeyword(string keyword);
        /// <summary>
        /// <para>Finds a shader with the given name.</para>
        /// </summary>
        /// <param name="name"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern Shader Find(string name);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern Shader FindBuiltin(string name);
        /// <summary>
        /// <para>Gets a global color property for all shaders previously set using SetGlobalColor.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameID"></param>
        public static Color GetGlobalColor(int nameID)
        {
            Color color;
            INTERNAL_CALL_GetGlobalColor(nameID, out color);
            return color;
        }

        /// <summary>
        /// <para>Gets a global color property for all shaders previously set using SetGlobalColor.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameID"></param>
        public static Color GetGlobalColor(string name)
        {
            return GetGlobalColor(PropertyToID(name));
        }

        /// <summary>
        /// <para>Gets a global float property for all shaders previously set using SetGlobalFloat.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameID"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern float GetGlobalFloat(int nameID);
        /// <summary>
        /// <para>Gets a global float property for all shaders previously set using SetGlobalFloat.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameID"></param>
        public static float GetGlobalFloat(string name)
        {
            return GetGlobalFloat(PropertyToID(name));
        }

        /// <summary>
        /// <para>Gets a global float array for all shaders previously set using SetGlobalFloatArray.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameID"></param>
        public static float[] GetGlobalFloatArray(int nameID)
        {
            return GetGlobalFloatArrayImpl(nameID);
        }

        /// <summary>
        /// <para>Gets a global float array for all shaders previously set using SetGlobalFloatArray.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameID"></param>
        public static float[] GetGlobalFloatArray(string name)
        {
            return GetGlobalFloatArray(PropertyToID(name));
        }

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

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern float[] GetGlobalFloatArrayImpl(int nameID);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void GetGlobalFloatArrayImplList(int nameID, object list);
        /// <summary>
        /// <para>Gets a global int property for all shaders previously set using SetGlobalInt.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameID"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern int GetGlobalInt(int nameID);
        /// <summary>
        /// <para>Gets a global int property for all shaders previously set using SetGlobalInt.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameID"></param>
        public static int GetGlobalInt(string name)
        {
            return GetGlobalInt(PropertyToID(name));
        }

        /// <summary>
        /// <para>Gets a global matrix property for all shaders previously set using SetGlobalMatrix.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameID"></param>
        public static Matrix4x4 GetGlobalMatrix(int nameID)
        {
            Matrix4x4 matrixx;
            INTERNAL_CALL_GetGlobalMatrix(nameID, out matrixx);
            return matrixx;
        }

        /// <summary>
        /// <para>Gets a global matrix property for all shaders previously set using SetGlobalMatrix.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameID"></param>
        public static Matrix4x4 GetGlobalMatrix(string name)
        {
            return GetGlobalMatrix(PropertyToID(name));
        }

        /// <summary>
        /// <para>Gets a global matrix array for all shaders previously set using SetGlobalMatrixArray.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameID"></param>
        public static Matrix4x4[] GetGlobalMatrixArray(int nameID)
        {
            return GetGlobalMatrixArrayImpl(nameID);
        }

        /// <summary>
        /// <para>Gets a global matrix array for all shaders previously set using SetGlobalMatrixArray.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameID"></param>
        public static Matrix4x4[] GetGlobalMatrixArray(string name)
        {
            return GetGlobalMatrixArray(PropertyToID(name));
        }

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

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern Matrix4x4[] GetGlobalMatrixArrayImpl(int nameID);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void GetGlobalMatrixArrayImplList(int nameID, object list);
        /// <summary>
        /// <para>Gets a global texture property for all shaders previously set using SetGlobalTexture.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameID"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern Texture GetGlobalTexture(int nameID);
        /// <summary>
        /// <para>Gets a global texture property for all shaders previously set using SetGlobalTexture.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameID"></param>
        public static Texture GetGlobalTexture(string name)
        {
            return GetGlobalTexture(PropertyToID(name));
        }

        /// <summary>
        /// <para>Gets a global vector property for all shaders previously set using SetGlobalVector.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameID"></param>
        public static Vector4 GetGlobalVector(int nameID)
        {
            Vector4 vector;
            INTERNAL_CALL_GetGlobalVector(nameID, out vector);
            return vector;
        }

        /// <summary>
        /// <para>Gets a global vector property for all shaders previously set using SetGlobalVector.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameID"></param>
        public static Vector4 GetGlobalVector(string name)
        {
            return GetGlobalVector(PropertyToID(name));
        }

        /// <summary>
        /// <para>Gets a global vector array for all shaders previously set using SetGlobalVectorArray.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameID"></param>
        public static Vector4[] GetGlobalVectorArray(int nameID)
        {
            return GetGlobalVectorArrayImpl(nameID);
        }

        /// <summary>
        /// <para>Gets a global vector array for all shaders previously set using SetGlobalVectorArray.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameID"></param>
        public static Vector4[] GetGlobalVectorArray(string name)
        {
            return GetGlobalVectorArray(PropertyToID(name));
        }

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

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern Vector4[] GetGlobalVectorArrayImpl(int nameID);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void GetGlobalVectorArrayImplList(int nameID, object list);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_GetGlobalColor(int nameID, out Color value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_GetGlobalMatrix(int nameID, out Matrix4x4 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_GetGlobalVector(int nameID, out Vector4 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_SetGlobalMatrix(int nameID, ref Matrix4x4 mat);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_SetGlobalVector(int nameID, ref Vector4 vec);
        /// <summary>
        /// <para>Is global shader keyword enabled?</para>
        /// </summary>
        /// <param name="keyword"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool IsKeywordEnabled(string keyword);
        /// <summary>
        /// <para>Gets unique identifier for a shader property name.</para>
        /// </summary>
        /// <param name="name">Shader property name.</param>
        /// <returns>
        /// <para>Unique integer for the name.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern int PropertyToID(string name);
        /// <summary>
        /// <para>Sets a global compute buffer property for all shaders.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="buffer"></param>
        /// <param name="nameID"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
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
        /// <param name="propertyName"></param>
        /// <param name="color"></param>
        /// <param name="nameID"></param>
        public static void SetGlobalColor(int nameID, Color color)
        {
            SetGlobalVector(nameID, (Vector4) color);
        }

        /// <summary>
        /// <para>Sets a global color property for all shaders.</para>
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="color"></param>
        /// <param name="nameID"></param>
        public static void SetGlobalColor(string propertyName, Color color)
        {
            SetGlobalColor(PropertyToID(propertyName), color);
        }

        /// <summary>
        /// <para>Sets a global float property for all shaders.</para>
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <param name="nameID"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void SetGlobalFloat(int nameID, float value);
        /// <summary>
        /// <para>Sets a global float property for all shaders.</para>
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <param name="nameID"></param>
        public static void SetGlobalFloat(string propertyName, float value)
        {
            SetGlobalFloat(PropertyToID(propertyName), value);
        }

        public static void SetGlobalFloatArray(int nameID, List<float> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            if (values.Count == 0)
            {
                throw new ArgumentException("Zero-sized array is not allowed.");
            }
            SetGlobalFloatArrayImplList(nameID, values);
        }

        /// <summary>
        /// <para>Sets a global float array property for all shaders.</para>
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="values"></param>
        /// <param name="nameID"></param>
        /// <param name="name"></param>
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
        /// <param name="propertyName"></param>
        /// <param name="values"></param>
        /// <param name="nameID"></param>
        /// <param name="name"></param>
        public static void SetGlobalFloatArray(string propertyName, float[] values)
        {
            SetGlobalFloatArray(PropertyToID(propertyName), values);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void SetGlobalFloatArrayImpl(int nameID, float[] values);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void SetGlobalFloatArrayImplList(int nameID, object values);
        /// <summary>
        /// <para>Sets a global int property for all shaders.</para>
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <param name="nameID"></param>
        public static void SetGlobalInt(int nameID, int value)
        {
            SetGlobalFloat(nameID, (float) value);
        }

        /// <summary>
        /// <para>Sets a global int property for all shaders.</para>
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <param name="nameID"></param>
        public static void SetGlobalInt(string propertyName, int value)
        {
            SetGlobalFloat(propertyName, (float) value);
        }

        /// <summary>
        /// <para>Sets a global matrix property for all shaders.</para>
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="mat"></param>
        /// <param name="nameID"></param>
        public static void SetGlobalMatrix(int nameID, Matrix4x4 mat)
        {
            INTERNAL_CALL_SetGlobalMatrix(nameID, ref mat);
        }

        /// <summary>
        /// <para>Sets a global matrix property for all shaders.</para>
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="mat"></param>
        /// <param name="nameID"></param>
        public static void SetGlobalMatrix(string propertyName, Matrix4x4 mat)
        {
            SetGlobalMatrix(PropertyToID(propertyName), mat);
        }

        public static void SetGlobalMatrixArray(int nameID, List<Matrix4x4> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            if (values.Count == 0)
            {
                throw new ArgumentException("Zero-sized array is not allowed.");
            }
            SetGlobalMatrixArrayImplList(nameID, values);
        }

        /// <summary>
        /// <para>Sets a global matrix array property for all shaders.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="values"></param>
        /// <param name="nameID"></param>
        /// <param name="propertyName"></param>
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
        /// <param name="propertyName"></param>
        public static void SetGlobalMatrixArray(string propertyName, Matrix4x4[] values)
        {
            SetGlobalMatrixArray(PropertyToID(propertyName), values);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void SetGlobalMatrixArrayImpl(int nameID, Matrix4x4[] values);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void SetGlobalMatrixArrayImplList(int nameID, object values);
        [Obsolete("SetGlobalTexGenMode is not supported anymore. Use programmable shaders to achieve the same effect.", true)]
        public static void SetGlobalTexGenMode(string propertyName, TexGenMode mode)
        {
        }

        /// <summary>
        /// <para>Sets a global texture property for all shaders.</para>
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="tex"></param>
        /// <param name="nameID"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void SetGlobalTexture(int nameID, Texture tex);
        /// <summary>
        /// <para>Sets a global texture property for all shaders.</para>
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="tex"></param>
        /// <param name="nameID"></param>
        public static void SetGlobalTexture(string propertyName, Texture tex)
        {
            SetGlobalTexture(PropertyToID(propertyName), tex);
        }

        [Obsolete("SetGlobalTextureMatrixName is not supported anymore. Use programmable shaders to achieve the same effect.", true)]
        public static void SetGlobalTextureMatrixName(string propertyName, string matrixName)
        {
        }

        /// <summary>
        /// <para>Sets a global vector property for all shaders.</para>
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="vec"></param>
        /// <param name="nameID"></param>
        public static void SetGlobalVector(int nameID, Vector4 vec)
        {
            INTERNAL_CALL_SetGlobalVector(nameID, ref vec);
        }

        /// <summary>
        /// <para>Sets a global vector property for all shaders.</para>
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="vec"></param>
        /// <param name="nameID"></param>
        public static void SetGlobalVector(string propertyName, Vector4 vec)
        {
            SetGlobalVector(PropertyToID(propertyName), vec);
        }

        public static void SetGlobalVectorArray(int nameID, List<Vector4> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            if (values.Count == 0)
            {
                throw new ArgumentException("Zero-sized array is not allowed.");
            }
            SetGlobalVectorArrayImplList(nameID, values);
        }

        /// <summary>
        /// <para>Sets a global vector array property for all shaders.</para>
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="values"></param>
        /// <param name="nameID"></param>
        /// <param name="name"></param>
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
        /// <param name="propertyName"></param>
        /// <param name="values"></param>
        /// <param name="nameID"></param>
        /// <param name="name"></param>
        public static void SetGlobalVectorArray(string propertyName, Vector4[] values)
        {
            SetGlobalVectorArray(PropertyToID(propertyName), values);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void SetGlobalVectorArrayImpl(int nameID, Vector4[] values);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void SetGlobalVectorArrayImplList(int nameID, object values);
        /// <summary>
        /// <para>Fully load all shaders to prevent future performance hiccups.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void WarmupAllShaders();

        internal string customEditor { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        internal DisableBatchingType disableBatching { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Shader LOD level for all shaders.</para>
        /// </summary>
        public static int globalMaximumLOD { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Shader hardware tier classification for current device.</para>
        /// </summary>
        [Obsolete("Use Graphics.activeTier instead (UnityUpgradable) -> UnityEngine.Graphics.activeTier", false)]
        public static ShaderHardwareTier globalShaderHardwareTier
        {
            get
            {
                return (ShaderHardwareTier) Graphics.activeTier;
            }
            set
            {
                Graphics.activeTier = (GraphicsTier) value;
            }
        }

        /// <summary>
        /// <para>Can this shader run on the end-users graphics card? (Read Only)</para>
        /// </summary>
        public bool isSupported { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Shader LOD level for this shader.</para>
        /// </summary>
        public int maximumLOD { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Render queue of this shader. (Read Only)</para>
        /// </summary>
        public int renderQueue { [MethodImpl(MethodImplOptions.InternalCall)] get; }
    }
}

