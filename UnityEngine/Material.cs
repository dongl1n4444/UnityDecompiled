namespace UnityEngine
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Internal;

    /// <summary>
    /// <para>The material class.</para>
    /// </summary>
    public class Material : UnityEngine.Object
    {
        /// <summary>
        /// <para></para>
        /// </summary>
        /// <param name="contents"></param>
        [Obsolete("Creating materials from shader source string is no longer supported. Use Shader assets instead.")]
        public Material(string contents)
        {
            Internal_CreateWithString(this, contents);
        }

        /// <summary>
        /// <para>Create a temporary Material.</para>
        /// </summary>
        /// <param name="shader">Create a material with a given Shader.</param>
        /// <param name="source">Create a material by copying all properties from another material.</param>
        public Material(Material source)
        {
            Internal_CreateWithMaterial(this, source);
        }

        /// <summary>
        /// <para>Create a temporary Material.</para>
        /// </summary>
        /// <param name="shader">Create a material with a given Shader.</param>
        /// <param name="source">Create a material by copying all properties from another material.</param>
        public Material(Shader shader)
        {
            Internal_CreateWithShader(this, shader);
        }

        /// <summary>
        /// <para>Copy properties from other material into this material.</para>
        /// </summary>
        /// <param name="mat"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void CopyPropertiesFromMaterial(Material mat);
        [Obsolete("Creating materials from shader source string will be removed in the future. Use Shader assets instead.")]
        public static Material Create(string scriptContents)
        {
            return new Material(scriptContents);
        }

        /// <summary>
        /// <para>Unset a shader keyword.</para>
        /// </summary>
        /// <param name="keyword"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void DisableKeyword(string keyword);
        /// <summary>
        /// <para>Set a shader keyword that is enabled by this material.</para>
        /// </summary>
        /// <param name="keyword"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void EnableKeyword(string keyword);
        /// <summary>
        /// <para>Returns the index of the pass passName.</para>
        /// </summary>
        /// <param name="passName"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern int FindPass(string passName);
        /// <summary>
        /// <para>Get a named color value.</para>
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
        public Color GetColor(int nameID)
        {
            Color color;
            INTERNAL_CALL_GetColor(this, nameID, out color);
            return color;
        }

        /// <summary>
        /// <para>Get a named color value.</para>
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
        public Color GetColor(string propertyName)
        {
            return this.GetColor(Shader.PropertyToID(propertyName));
        }

        /// <summary>
        /// <para>Get a named color array.</para>
        /// </summary>
        /// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
        /// <param name="name">The name of the property.</param>
        public Color[] GetColorArray(int nameID)
        {
            return this.GetColorArrayImpl(nameID);
        }

        /// <summary>
        /// <para>Get a named color array.</para>
        /// </summary>
        /// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
        /// <param name="name">The name of the property.</param>
        public Color[] GetColorArray(string name)
        {
            return this.GetColorArray(Shader.PropertyToID(name));
        }

        public void GetColorArray(int nameID, List<Color> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            this.GetColorArrayImplList(nameID, values);
        }

        public void GetColorArray(string name, List<Color> values)
        {
            this.GetColorArray(Shader.PropertyToID(name), values);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern Color[] GetColorArrayImpl(int nameID);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void GetColorArrayImplList(int nameID, object list);
        /// <summary>
        /// <para>Get a named float value.</para>
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern float GetFloat(int nameID);
        /// <summary>
        /// <para>Get a named float value.</para>
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
        public float GetFloat(string propertyName)
        {
            return this.GetFloat(Shader.PropertyToID(propertyName));
        }

        /// <summary>
        /// <para>Get a named float array.</para>
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
        public float[] GetFloatArray(int nameID)
        {
            return this.GetFloatArrayImpl(nameID);
        }

        /// <summary>
        /// <para>Get a named float array.</para>
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
        public float[] GetFloatArray(string name)
        {
            return this.GetFloatArray(Shader.PropertyToID(name));
        }

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

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern float[] GetFloatArrayImpl(int nameID);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void GetFloatArrayImplList(int nameID, object list);
        /// <summary>
        /// <para>Get a named integer value.</para>
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
        public int GetInt(int nameID)
        {
            return (int) this.GetFloat(nameID);
        }

        /// <summary>
        /// <para>Get a named integer value.</para>
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
        public int GetInt(string propertyName)
        {
            return (int) this.GetFloat(propertyName);
        }

        /// <summary>
        /// <para>Get a named matrix value from the shader.</para>
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
        public Matrix4x4 GetMatrix(int nameID)
        {
            Matrix4x4 matrixx;
            INTERNAL_CALL_GetMatrix(this, nameID, out matrixx);
            return matrixx;
        }

        /// <summary>
        /// <para>Get a named matrix value from the shader.</para>
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
        public Matrix4x4 GetMatrix(string propertyName)
        {
            return this.GetMatrix(Shader.PropertyToID(propertyName));
        }

        /// <summary>
        /// <para>Get a named matrix array.</para>
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
        public Matrix4x4[] GetMatrixArray(int nameID)
        {
            return this.GetMatrixArrayImpl(nameID);
        }

        /// <summary>
        /// <para>Get a named matrix array.</para>
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
        public Matrix4x4[] GetMatrixArray(string name)
        {
            return this.GetMatrixArray(Shader.PropertyToID(name));
        }

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

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern Matrix4x4[] GetMatrixArrayImpl(int nameID);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void GetMatrixArrayImplList(int nameID, object list);
        /// <summary>
        /// <para>Returns the name of the shader pass at index pass.</para>
        /// </summary>
        /// <param name="pass"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern string GetPassName(int pass);
        /// <summary>
        /// <para>Get the value of material's shader tag.</para>
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="searchFallbacks"></param>
        /// <param name="defaultValue"></param>
        [ExcludeFromDocs]
        public string GetTag(string tag, bool searchFallbacks)
        {
            string defaultValue = "";
            return this.GetTag(tag, searchFallbacks, defaultValue);
        }

        /// <summary>
        /// <para>Get the value of material's shader tag.</para>
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="searchFallbacks"></param>
        /// <param name="defaultValue"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern string GetTag(string tag, bool searchFallbacks, [DefaultValue("\"\"")] string defaultValue);
        /// <summary>
        /// <para>Get a named texture.</para>
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern Texture GetTexture(int nameID);
        /// <summary>
        /// <para>Get a named texture.</para>
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
        public Texture GetTexture(string propertyName)
        {
            return this.GetTexture(Shader.PropertyToID(propertyName));
        }

        /// <summary>
        /// <para>Gets the placement offset of texture propertyName.</para>
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        public Vector2 GetTextureOffset(string propertyName)
        {
            Vector4 vector;
            Internal_GetTextureScaleAndOffset(this, propertyName, out vector);
            return new Vector2(vector.z, vector.w);
        }

        /// <summary>
        /// <para>Gets the placement scale of texture propertyName.</para>
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        public Vector2 GetTextureScale(string propertyName)
        {
            Vector4 vector;
            Internal_GetTextureScaleAndOffset(this, propertyName, out vector);
            return new Vector2(vector.x, vector.y);
        }

        /// <summary>
        /// <para>Get a named vector value.</para>
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
        public Vector4 GetVector(int nameID)
        {
            Color color = this.GetColor(nameID);
            return new Vector4(color.r, color.g, color.b, color.a);
        }

        /// <summary>
        /// <para>Get a named vector value.</para>
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
        public Vector4 GetVector(string propertyName)
        {
            Color color = this.GetColor(propertyName);
            return new Vector4(color.r, color.g, color.b, color.a);
        }

        /// <summary>
        /// <para>Get a named vector array.</para>
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
        public Vector4[] GetVectorArray(int nameID)
        {
            return this.GetVectorArrayImpl(nameID);
        }

        /// <summary>
        /// <para>Get a named vector array.</para>
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="nameID">The name ID of the property retrieved by Shader.PropertyToID.</param>
        public Vector4[] GetVectorArray(string name)
        {
            return this.GetVectorArray(Shader.PropertyToID(name));
        }

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

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern Vector4[] GetVectorArrayImpl(int nameID);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void GetVectorArrayImplList(int nameID, object list);
        /// <summary>
        /// <para>Checks if material's shader has a property of a given name.</para>
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="nameID"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern bool HasProperty(int nameID);
        /// <summary>
        /// <para>Checks if material's shader has a property of a given name.</para>
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="nameID"></param>
        public bool HasProperty(string propertyName)
        {
            return this.HasProperty(Shader.PropertyToID(propertyName));
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_GetColor(Material self, int nameID, out Color value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_GetMatrix(Material self, int nameID, out Matrix4x4 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_SetColor(Material self, int nameID, ref Color color);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_SetMatrix(Material self, int nameID, ref Matrix4x4 matrix);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_SetTextureOffset(Material self, string propertyName, ref Vector2 offset);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_SetTextureScale(Material self, string propertyName, ref Vector2 scale);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void Internal_CreateWithMaterial([Writable] Material mono, Material source);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void Internal_CreateWithShader([Writable] Material mono, Shader shader);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void Internal_CreateWithString([Writable] Material mono, string contents);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void Internal_GetTextureScaleAndOffset(Material mat, string name, out Vector4 output);
        /// <summary>
        /// <para>Is the shader keyword enabled on this material?</para>
        /// </summary>
        /// <param name="keyword"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern bool IsKeywordEnabled(string keyword);
        /// <summary>
        /// <para>Interpolate properties between two materials.</para>
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="t"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void Lerp(Material start, Material end, float t);
        /// <summary>
        /// <para>Set a named ComputeBuffer value.</para>
        /// </summary>
        /// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
        /// <param name="name">Property name.</param>
        /// <param name="value">ComputeBuffer value to set.</param>
        public void SetBuffer(int nameID, ComputeBuffer value)
        {
            this.SetBufferImpl(nameID, value);
        }

        /// <summary>
        /// <para>Set a named ComputeBuffer value.</para>
        /// </summary>
        /// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
        /// <param name="name">Property name.</param>
        /// <param name="value">ComputeBuffer value to set.</param>
        public void SetBuffer(string name, ComputeBuffer value)
        {
            this.SetBuffer(Shader.PropertyToID(name), value);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void SetBufferImpl(int nameID, ComputeBuffer value);
        /// <summary>
        /// <para>Set a named color value.</para>
        /// </summary>
        /// <param name="propertyName">Property name, e.g. "_Color".</param>
        /// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
        /// <param name="color">Color value to set.</param>
        public void SetColor(int nameID, Color color)
        {
            INTERNAL_CALL_SetColor(this, nameID, ref color);
        }

        /// <summary>
        /// <para>Set a named color value.</para>
        /// </summary>
        /// <param name="propertyName">Property name, e.g. "_Color".</param>
        /// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
        /// <param name="color">Color value to set.</param>
        public void SetColor(string propertyName, Color color)
        {
            this.SetColor(Shader.PropertyToID(propertyName), color);
        }

        public void SetColorArray(int nameID, List<Color> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            if (values.Count == 0)
            {
                throw new ArgumentException("Zero-sized array is not allowed.");
            }
            this.SetColorArrayImplList(nameID, values);
        }

        /// <summary>
        /// <para>Set a color array property.</para>
        /// </summary>
        /// <param name="name">Property name.</param>
        /// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
        /// <param name="values">Array of values to set.</param>
        public void SetColorArray(int nameID, Color[] values)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            if (values.Length == 0)
            {
                throw new ArgumentException("Zero-sized array is not allowed.");
            }
            this.SetColorArrayImpl(nameID, values);
        }

        public void SetColorArray(string name, List<Color> values)
        {
            this.SetColorArray(Shader.PropertyToID(name), values);
        }

        /// <summary>
        /// <para>Set a color array property.</para>
        /// </summary>
        /// <param name="name">Property name.</param>
        /// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
        /// <param name="values">Array of values to set.</param>
        public void SetColorArray(string name, Color[] values)
        {
            this.SetColorArray(Shader.PropertyToID(name), values);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void SetColorArrayImpl(int nameID, Color[] values);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void SetColorArrayImplList(int nameID, object values);
        /// <summary>
        /// <para>Set a named float value.</para>
        /// </summary>
        /// <param name="propertyName">Property name, e.g. "_Glossiness".</param>
        /// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
        /// <param name="value">Float value to set.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void SetFloat(int nameID, float value);
        /// <summary>
        /// <para>Set a named float value.</para>
        /// </summary>
        /// <param name="propertyName">Property name, e.g. "_Glossiness".</param>
        /// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
        /// <param name="value">Float value to set.</param>
        public void SetFloat(string propertyName, float value)
        {
            this.SetFloat(Shader.PropertyToID(propertyName), value);
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
        /// <param name="name">Property name.</param>
        /// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
        /// <param name="values">Array of values to set.</param>
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
        /// <param name="name">Property name.</param>
        /// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
        /// <param name="values">Array of values to set.</param>
        public void SetFloatArray(string name, float[] values)
        {
            this.SetFloatArray(Shader.PropertyToID(name), values);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void SetFloatArrayImpl(int nameID, float[] values);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void SetFloatArrayImplList(int nameID, object values);
        /// <summary>
        /// <para>Set a named integer value.</para>
        /// </summary>
        /// <param name="propertyName">Property name, e.g. "_SrcBlend".</param>
        /// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
        /// <param name="value">Integer value to set.</param>
        public void SetInt(int nameID, int value)
        {
            this.SetFloat(nameID, (float) value);
        }

        /// <summary>
        /// <para>Set a named integer value.</para>
        /// </summary>
        /// <param name="propertyName">Property name, e.g. "_SrcBlend".</param>
        /// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
        /// <param name="value">Integer value to set.</param>
        public void SetInt(string propertyName, int value)
        {
            this.SetFloat(propertyName, (float) value);
        }

        /// <summary>
        /// <para>Set a named matrix for the shader.</para>
        /// </summary>
        /// <param name="propertyName">Property name, e.g. "_CubemapRotation".</param>
        /// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
        /// <param name="matrix">Matrix value to set.</param>
        public void SetMatrix(int nameID, Matrix4x4 matrix)
        {
            INTERNAL_CALL_SetMatrix(this, nameID, ref matrix);
        }

        /// <summary>
        /// <para>Set a named matrix for the shader.</para>
        /// </summary>
        /// <param name="propertyName">Property name, e.g. "_CubemapRotation".</param>
        /// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
        /// <param name="matrix">Matrix value to set.</param>
        public void SetMatrix(string propertyName, Matrix4x4 matrix)
        {
            this.SetMatrix(Shader.PropertyToID(propertyName), matrix);
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
        /// <param name="name">Property name.</param>
        /// <param name="values">Array of values to set.</param>
        /// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
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
        /// <param name="name">Property name.</param>
        /// <param name="values">Array of values to set.</param>
        /// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
        public void SetMatrixArray(string name, Matrix4x4[] values)
        {
            this.SetMatrixArray(Shader.PropertyToID(name), values);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void SetMatrixArrayImpl(int nameID, Matrix4x4[] values);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void SetMatrixArrayImplList(int nameID, object values);
        /// <summary>
        /// <para>Sets an override tag/value on the material.</para>
        /// </summary>
        /// <param name="tag">Name of the tag to set.</param>
        /// <param name="val">Name of the value to set. Empty string to clear the override flag.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void SetOverrideTag(string tag, string val);
        /// <summary>
        /// <para>Activate the given pass for rendering.</para>
        /// </summary>
        /// <param name="pass">Shader pass number to setup.</param>
        /// <returns>
        /// <para>If false is returned, no rendering should be done.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern bool SetPass(int pass);
        /// <summary>
        /// <para>Set a named texture.</para>
        /// </summary>
        /// <param name="propertyName">Property name, e.g. "_MainTex".</param>
        /// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
        /// <param name="texture">Texture to set.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void SetTexture(int nameID, Texture texture);
        /// <summary>
        /// <para>Set a named texture.</para>
        /// </summary>
        /// <param name="propertyName">Property name, e.g. "_MainTex".</param>
        /// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
        /// <param name="texture">Texture to set.</param>
        public void SetTexture(string propertyName, Texture texture)
        {
            this.SetTexture(Shader.PropertyToID(propertyName), texture);
        }

        /// <summary>
        /// <para>Sets the placement offset of texture propertyName.</para>
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="offset"></param>
        public void SetTextureOffset(string propertyName, Vector2 offset)
        {
            INTERNAL_CALL_SetTextureOffset(this, propertyName, ref offset);
        }

        /// <summary>
        /// <para>Sets the placement scale of texture propertyName.</para>
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="scale"></param>
        public void SetTextureScale(string propertyName, Vector2 scale)
        {
            INTERNAL_CALL_SetTextureScale(this, propertyName, ref scale);
        }

        /// <summary>
        /// <para>Set a named vector value.</para>
        /// </summary>
        /// <param name="propertyName">Property name, e.g. "_WaveAndDistance".</param>
        /// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
        /// <param name="vector">Vector value to set.</param>
        public void SetVector(int nameID, Vector4 vector)
        {
            this.SetColor(nameID, new Color(vector.x, vector.y, vector.z, vector.w));
        }

        /// <summary>
        /// <para>Set a named vector value.</para>
        /// </summary>
        /// <param name="propertyName">Property name, e.g. "_WaveAndDistance".</param>
        /// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
        /// <param name="vector">Vector value to set.</param>
        public void SetVector(string propertyName, Vector4 vector)
        {
            this.SetColor(propertyName, new Color(vector.x, vector.y, vector.z, vector.w));
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
        /// <param name="name">Property name.</param>
        /// <param name="values">Array of values to set.</param>
        /// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
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
        /// <param name="name">Property name.</param>
        /// <param name="values">Array of values to set.</param>
        /// <param name="nameID">Property name ID, use Shader.PropertyToID to get it.</param>
        public void SetVectorArray(string name, Vector4[] values)
        {
            this.SetVectorArray(Shader.PropertyToID(name), values);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void SetVectorArrayImpl(int nameID, Vector4[] values);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void SetVectorArrayImplList(int nameID, object values);

        /// <summary>
        /// <para>The main material's color.</para>
        /// </summary>
        public Color color
        {
            get
            {
                return this.GetColor("_Color");
            }
            set
            {
                this.SetColor("_Color", value);
            }
        }

        /// <summary>
        /// <para>Defines how the material should interact with lightmaps and lightprobes.</para>
        /// </summary>
        public MaterialGlobalIlluminationFlags globalIlluminationFlags { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The material's texture.</para>
        /// </summary>
        public Texture mainTexture
        {
            get
            {
                return this.GetTexture("_MainTex");
            }
            set
            {
                this.SetTexture("_MainTex", value);
            }
        }

        /// <summary>
        /// <para>The texture offset of the main texture.</para>
        /// </summary>
        public Vector2 mainTextureOffset
        {
            get
            {
                return this.GetTextureOffset("_MainTex");
            }
            set
            {
                this.SetTextureOffset("_MainTex", value);
            }
        }

        /// <summary>
        /// <para>The texture scale of the main texture.</para>
        /// </summary>
        public Vector2 mainTextureScale
        {
            get
            {
                return this.GetTextureScale("_MainTex");
            }
            set
            {
                this.SetTextureScale("_MainTex", value);
            }
        }

        /// <summary>
        /// <para>How many passes are in this material (Read Only).</para>
        /// </summary>
        public int passCount { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Render queue of this material.</para>
        /// </summary>
        public int renderQueue { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The shader used by the material.</para>
        /// </summary>
        public Shader shader { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Additional shader keywords set by this material.</para>
        /// </summary>
        public string[] shaderKeywords { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

