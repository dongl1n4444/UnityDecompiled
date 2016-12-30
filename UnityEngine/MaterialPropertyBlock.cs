namespace UnityEngine
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>A block of material values to apply.</para>
    /// </summary>
    public sealed class MaterialPropertyBlock
    {
        internal IntPtr m_Ptr;

        public MaterialPropertyBlock()
        {
            this.InitBlock();
        }

        [Obsolete("Use SetColor instead (UnityUpgradable) -> SetColor(*)", false)]
        public void AddColor(int nameID, Color value)
        {
            this.SetColor(nameID, value);
        }

        [Obsolete("Use SetColor instead (UnityUpgradable) -> SetColor(*)", false)]
        public void AddColor(string name, Color value)
        {
            this.SetColor(Shader.PropertyToID(name), value);
        }

        [Obsolete("Use SetFloat instead (UnityUpgradable) -> SetFloat(*)", false)]
        public void AddFloat(int nameID, float value)
        {
            this.SetFloat(nameID, value);
        }

        [Obsolete("Use SetFloat instead (UnityUpgradable) -> SetFloat(*)", false)]
        public void AddFloat(string name, float value)
        {
            this.SetFloat(Shader.PropertyToID(name), value);
        }

        [Obsolete("Use SetMatrix instead (UnityUpgradable) -> SetMatrix(*)", false)]
        public void AddMatrix(int nameID, Matrix4x4 value)
        {
            this.SetMatrix(nameID, value);
        }

        [Obsolete("Use SetMatrix instead (UnityUpgradable) -> SetMatrix(*)", false)]
        public void AddMatrix(string name, Matrix4x4 value)
        {
            this.SetMatrix(Shader.PropertyToID(name), value);
        }

        [Obsolete("Use SetTexture instead (UnityUpgradable) -> SetTexture(*)", false)]
        public void AddTexture(int nameID, Texture value)
        {
            this.SetTexture(nameID, value);
        }

        [Obsolete("Use SetTexture instead (UnityUpgradable) -> SetTexture(*)", false)]
        public void AddTexture(string name, Texture value)
        {
            this.SetTexture(Shader.PropertyToID(name), value);
        }

        [Obsolete("Use SetVector instead (UnityUpgradable) -> SetVector(*)", false)]
        public void AddVector(int nameID, Vector4 value)
        {
            this.SetVector(nameID, value);
        }

        [Obsolete("Use SetVector instead (UnityUpgradable) -> SetVector(*)", false)]
        public void AddVector(string name, Vector4 value)
        {
            this.SetVector(Shader.PropertyToID(name), value);
        }

        /// <summary>
        /// <para>Clear material property values.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void Clear();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator, ThreadAndSerializationSafe]
        internal extern void DestroyBlock();
        ~MaterialPropertyBlock()
        {
            this.DestroyBlock();
        }

        /// <summary>
        /// <para>Get a float from the property block.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameID"></param>
        public float GetFloat(int nameID) => 
            this.GetFloatImpl(nameID);

        /// <summary>
        /// <para>Get a float from the property block.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameID"></param>
        public float GetFloat(string name) => 
            this.GetFloat(Shader.PropertyToID(name));

        /// <summary>
        /// <para>Get a float array from the property block.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameID"></param>
        public float[] GetFloatArray(int nameID) => 
            this.GetFloatArrayImpl(nameID);

        /// <summary>
        /// <para>Get a float array from the property block.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameID"></param>
        public float[] GetFloatArray(string name) => 
            this.GetFloatArray(Shader.PropertyToID(name));

        public void GetFloatArray(int nameID, List<float> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            this.GetFloatArrayImplList(nameID, values);
        }

        public void GetFloatArray(string name, List<float> values)
        {
            this.GetFloatArray(Shader.PropertyToID(name), values);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern float[] GetFloatArrayImpl(int nameID);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void GetFloatArrayImplList(int nameID, object list);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern float GetFloatImpl(int nameID);
        /// <summary>
        /// <para>Get a matrix from the property block.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameID"></param>
        public Matrix4x4 GetMatrix(int nameID) => 
            this.GetMatrixImpl(nameID);

        /// <summary>
        /// <para>Get a matrix from the property block.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameID"></param>
        public Matrix4x4 GetMatrix(string name) => 
            this.GetMatrix(Shader.PropertyToID(name));

        /// <summary>
        /// <para>Get a matrix array from the property block.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameID"></param>
        public Matrix4x4[] GetMatrixArray(int nameID) => 
            this.GetMatrixArrayImpl(nameID);

        /// <summary>
        /// <para>Get a matrix array from the property block.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameID"></param>
        public Matrix4x4[] GetMatrixArray(string name) => 
            this.GetMatrixArray(Shader.PropertyToID(name));

        public void GetMatrixArray(int nameID, List<Matrix4x4> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            this.GetMatrixArrayImplList(nameID, values);
        }

        public void GetMatrixArray(string name, List<Matrix4x4> values)
        {
            this.GetMatrixArray(Shader.PropertyToID(name), values);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern Matrix4x4[] GetMatrixArrayImpl(int nameID);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void GetMatrixArrayImplList(int nameID, object list);
        private Matrix4x4 GetMatrixImpl(int nameID)
        {
            Matrix4x4 matrixx;
            INTERNAL_CALL_GetMatrixImpl(this, nameID, out matrixx);
            return matrixx;
        }

        /// <summary>
        /// <para>Get a texture from the property block.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameID"></param>
        public Texture GetTexture(int nameID) => 
            this.GetTextureImpl(nameID);

        /// <summary>
        /// <para>Get a texture from the property block.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameID"></param>
        public Texture GetTexture(string name) => 
            this.GetTexture(Shader.PropertyToID(name));

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern Texture GetTextureImpl(int nameID);
        /// <summary>
        /// <para>Get a vector from the property block.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameID"></param>
        public Vector4 GetVector(int nameID) => 
            this.GetVectorImpl(nameID);

        /// <summary>
        /// <para>Get a vector from the property block.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameID"></param>
        public Vector4 GetVector(string name) => 
            this.GetVector(Shader.PropertyToID(name));

        /// <summary>
        /// <para>Get a vector array from the property block.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameID"></param>
        public Vector4[] GetVectorArray(int nameID) => 
            this.GetVectorArrayImpl(nameID);

        /// <summary>
        /// <para>Get a vector array from the property block.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameID"></param>
        public Vector4[] GetVectorArray(string name) => 
            this.GetVectorArray(Shader.PropertyToID(name));

        public void GetVectorArray(int nameID, List<Vector4> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            this.GetVectorArrayImplList(nameID, values);
        }

        public void GetVectorArray(string name, List<Vector4> values)
        {
            this.GetVectorArray(Shader.PropertyToID(name), values);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern Vector4[] GetVectorArrayImpl(int nameID);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void GetVectorArrayImplList(int nameID, object list);
        private Vector4 GetVectorImpl(int nameID)
        {
            Vector4 vector;
            INTERNAL_CALL_GetVectorImpl(this, nameID, out vector);
            return vector;
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern void InitBlock();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetMatrixImpl(MaterialPropertyBlock self, int nameID, out Matrix4x4 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetVectorImpl(MaterialPropertyBlock self, int nameID, out Vector4 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_SetColorImpl(MaterialPropertyBlock self, int nameID, ref Color value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_SetMatrixImpl(MaterialPropertyBlock self, int nameID, ref Matrix4x4 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_SetVectorImpl(MaterialPropertyBlock self, int nameID, ref Vector4 value);
        /// <summary>
        /// <para>Set a ComputeBuffer property.</para>
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
        /// <param name="value">The ComputeBuffer to set.</param>
        public void SetBuffer(int nameID, ComputeBuffer value)
        {
            this.SetBufferImpl(nameID, value);
        }

        /// <summary>
        /// <para>Set a ComputeBuffer property.</para>
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
        /// <param name="value">The ComputeBuffer to set.</param>
        public void SetBuffer(string name, ComputeBuffer value)
        {
            this.SetBuffer(Shader.PropertyToID(name), value);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void SetBufferImpl(int nameID, ComputeBuffer value);
        /// <summary>
        /// <para>Set a color property.</para>
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
        /// <param name="value">The Color value to set.</param>
        public void SetColor(int nameID, Color value)
        {
            this.SetColorImpl(nameID, value);
        }

        /// <summary>
        /// <para>Set a color property.</para>
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
        /// <param name="value">The Color value to set.</param>
        public void SetColor(string name, Color value)
        {
            this.SetColor(Shader.PropertyToID(name), value);
        }

        private void SetColorImpl(int nameID, Color value)
        {
            INTERNAL_CALL_SetColorImpl(this, nameID, ref value);
        }

        /// <summary>
        /// <para>Set a float property.</para>
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
        /// <param name="value">The float value to set.</param>
        public void SetFloat(int nameID, float value)
        {
            this.SetFloatImpl(nameID, value);
        }

        /// <summary>
        /// <para>Set a float property.</para>
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
        /// <param name="value">The float value to set.</param>
        public void SetFloat(string name, float value)
        {
            this.SetFloat(Shader.PropertyToID(name), value);
        }

        public void SetFloatArray(int nameID, List<float> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            if (values.Count == 0)
            {
                throw new ArgumentException("Zero-sized array is not allowed.");
            }
            this.SetFloatArrayImplList(nameID, values);
        }

        /// <summary>
        /// <para>Set a float array property.</para>
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
        /// <param name="values">The array to set.</param>
        public void SetFloatArray(int nameID, float[] values)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            if (values.Length == 0)
            {
                throw new ArgumentException("Zero-sized array is not allowed.");
            }
            this.SetFloatArrayImpl(nameID, values);
        }

        public void SetFloatArray(string name, List<float> values)
        {
            this.SetFloatArray(Shader.PropertyToID(name), values);
        }

        /// <summary>
        /// <para>Set a float array property.</para>
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
        /// <param name="values">The array to set.</param>
        public void SetFloatArray(string name, float[] values)
        {
            this.SetFloatArray(Shader.PropertyToID(name), values);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void SetFloatArrayImpl(int nameID, float[] values);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void SetFloatArrayImplList(int nameID, object list);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void SetFloatImpl(int nameID, float value);
        /// <summary>
        /// <para>Set a matrix property.</para>
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
        /// <param name="value">The matrix value to set.</param>
        public void SetMatrix(int nameID, Matrix4x4 value)
        {
            this.SetMatrixImpl(nameID, value);
        }

        /// <summary>
        /// <para>Set a matrix property.</para>
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
        /// <param name="value">The matrix value to set.</param>
        public void SetMatrix(string name, Matrix4x4 value)
        {
            this.SetMatrix(Shader.PropertyToID(name), value);
        }

        public void SetMatrixArray(int nameID, List<Matrix4x4> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            if (values.Count == 0)
            {
                throw new ArgumentException("Zero-sized array is not allowed.");
            }
            this.SetMatrixArrayImplList(nameID, values);
        }

        /// <summary>
        /// <para>Set a matrix array property.</para>
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="values">The name ID of the property retrieved by Shader.PropertyToID.</param>
        /// <param name="nameID">The array to set.</param>
        public void SetMatrixArray(int nameID, Matrix4x4[] values)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            if (values.Length == 0)
            {
                throw new ArgumentException("Zero-sized array is not allowed.");
            }
            this.SetMatrixArrayImpl(nameID, values);
        }

        public void SetMatrixArray(string name, List<Matrix4x4> values)
        {
            this.SetMatrixArray(Shader.PropertyToID(name), values);
        }

        /// <summary>
        /// <para>Set a matrix array property.</para>
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="values">The name ID of the property retrieved by Shader.PropertyToID.</param>
        /// <param name="nameID">The array to set.</param>
        public void SetMatrixArray(string name, Matrix4x4[] values)
        {
            this.SetMatrixArray(Shader.PropertyToID(name), values);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void SetMatrixArrayImpl(int nameID, Matrix4x4[] values);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void SetMatrixArrayImplList(int nameID, object list);
        private void SetMatrixImpl(int nameID, Matrix4x4 value)
        {
            INTERNAL_CALL_SetMatrixImpl(this, nameID, ref value);
        }

        /// <summary>
        /// <para>Set a texture property.</para>
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
        /// <param name="value">The Texture to set.</param>
        public void SetTexture(int nameID, Texture value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            this.SetTextureImpl(nameID, value);
        }

        /// <summary>
        /// <para>Set a texture property.</para>
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
        /// <param name="value">The Texture to set.</param>
        public void SetTexture(string name, Texture value)
        {
            this.SetTexture(Shader.PropertyToID(name), value);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void SetTextureImpl(int nameID, Texture value);
        /// <summary>
        /// <para>Set a vector property.</para>
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
        /// <param name="value">The Vector4 value to set.</param>
        public void SetVector(int nameID, Vector4 value)
        {
            this.SetVectorImpl(nameID, value);
        }

        /// <summary>
        /// <para>Set a vector property.</para>
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
        /// <param name="value">The Vector4 value to set.</param>
        public void SetVector(string name, Vector4 value)
        {
            this.SetVector(Shader.PropertyToID(name), value);
        }

        public void SetVectorArray(int nameID, List<Vector4> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            if (values.Count == 0)
            {
                throw new ArgumentException("Zero-sized array is not allowed.");
            }
            this.SetVectorArrayImplList(nameID, values);
        }

        /// <summary>
        /// <para>Set a vector array property.</para>
        /// </summary>
        /// <param name="nameID">The name of the property.</param>
        /// <param name="values">The name ID of the property retrieved by Shader.PropertyToID.</param>
        /// <param name="name">The array to set.</param>
        public void SetVectorArray(int nameID, Vector4[] values)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            if (values.Length == 0)
            {
                throw new ArgumentException("Zero-sized array is not allowed.");
            }
            this.SetVectorArrayImpl(nameID, values);
        }

        public void SetVectorArray(string name, List<Vector4> values)
        {
            this.SetVectorArray(Shader.PropertyToID(name), values);
        }

        /// <summary>
        /// <para>Set a vector array property.</para>
        /// </summary>
        /// <param name="nameID">The name of the property.</param>
        /// <param name="values">The name ID of the property retrieved by Shader.PropertyToID.</param>
        /// <param name="name">The array to set.</param>
        public void SetVectorArray(string name, Vector4[] values)
        {
            this.SetVectorArray(Shader.PropertyToID(name), values);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void SetVectorArrayImpl(int nameID, Vector4[] values);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void SetVectorArrayImplList(int nameID, object list);
        private void SetVectorImpl(int nameID, Vector4 value)
        {
            INTERNAL_CALL_SetVectorImpl(this, nameID, ref value);
        }

        /// <summary>
        /// <para>Is the material property block empty? (Read Only)</para>
        /// </summary>
        public bool isEmpty { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }
    }
}

