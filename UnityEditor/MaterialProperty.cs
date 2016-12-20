namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Rendering;

    /// <summary>
    /// <para>Describes information and value of a single shader property.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class MaterialProperty
    {
        private Object[] m_Targets;
        private ApplyPropertyCallback m_ApplyPropertyCallback;
        private string m_Name;
        private string m_DisplayName;
        private object m_Value;
        private Vector4 m_TextureScaleAndOffset;
        private Vector2 m_RangeLimits;
        private PropType m_Type;
        private PropFlags m_Flags;
        private TextureDimension m_TextureDimension;
        private int m_MixedValueMask;
        /// <summary>
        /// <para>Material objects being edited by this property (Read Only).</para>
        /// </summary>
        public Object[] targets
        {
            get
            {
                return this.m_Targets;
            }
        }
        /// <summary>
        /// <para>Type of the property (Read Only).</para>
        /// </summary>
        public PropType type
        {
            get
            {
                return this.m_Type;
            }
        }
        /// <summary>
        /// <para>Name of the property (Read Only).</para>
        /// </summary>
        public string name
        {
            get
            {
                return this.m_Name;
            }
        }
        /// <summary>
        /// <para>Display name of the property (Read Only).</para>
        /// </summary>
        public string displayName
        {
            get
            {
                return this.m_DisplayName;
            }
        }
        /// <summary>
        /// <para>Flags that control how property is displayed (Read Only).</para>
        /// </summary>
        public PropFlags flags
        {
            get
            {
                return this.m_Flags;
            }
        }
        /// <summary>
        /// <para>Texture dimension (2D, Cubemap etc.) of the property (Read Only).</para>
        /// </summary>
        public TextureDimension textureDimension
        {
            get
            {
                return this.m_TextureDimension;
            }
        }
        /// <summary>
        /// <para>Min/max limits of a ranged float property (Read Only).</para>
        /// </summary>
        public Vector2 rangeLimits
        {
            get
            {
                return this.m_RangeLimits;
            }
        }
        /// <summary>
        /// <para>Does this property have multiple different values? (Read Only)</para>
        /// </summary>
        public bool hasMixedValue
        {
            get
            {
                return ((this.m_MixedValueMask & 1) != 0);
            }
        }
        public ApplyPropertyCallback applyPropertyCallback
        {
            get
            {
                return this.m_ApplyPropertyCallback;
            }
            set
            {
                this.m_ApplyPropertyCallback = value;
            }
        }
        internal int mixedValueMask
        {
            get
            {
                return this.m_MixedValueMask;
            }
        }
        public void ReadFromMaterialPropertyBlock(MaterialPropertyBlock block)
        {
            ShaderUtil.ApplyMaterialPropertyBlockToMaterialProperty(block, this);
        }

        public void WriteToMaterialPropertyBlock(MaterialPropertyBlock materialblock, int changedPropertyMask)
        {
            ShaderUtil.ApplyMaterialPropertyToMaterialPropertyBlock(this, changedPropertyMask, materialblock);
        }

        /// <summary>
        /// <para>Color value of the property.</para>
        /// </summary>
        public Color colorValue
        {
            get
            {
                if (this.m_Type == PropType.Color)
                {
                    return (Color) this.m_Value;
                }
                return Color.black;
            }
            set
            {
                if ((this.m_Type == PropType.Color) && (this.hasMixedValue || (value != ((Color) this.m_Value))))
                {
                    this.ApplyProperty(value);
                }
            }
        }
        /// <summary>
        /// <para>Vector value of the property.</para>
        /// </summary>
        public Vector4 vectorValue
        {
            get
            {
                if (this.m_Type == PropType.Vector)
                {
                    return (Vector4) this.m_Value;
                }
                return Vector4.zero;
            }
            set
            {
                if ((this.m_Type == PropType.Vector) && (this.hasMixedValue || (value != ((Vector4) this.m_Value))))
                {
                    this.ApplyProperty(value);
                }
            }
        }
        internal static bool IsTextureOffsetAndScaleChangedMask(int changedMask)
        {
            changedMask = changedMask >> 1;
            return (changedMask != 0);
        }

        /// <summary>
        /// <para>Float vaue of the property.</para>
        /// </summary>
        public float floatValue
        {
            get
            {
                if ((this.m_Type == PropType.Float) || (this.m_Type == PropType.Range))
                {
                    return (float) this.m_Value;
                }
                return 0f;
            }
            set
            {
                if (((this.m_Type == PropType.Float) || (this.m_Type == PropType.Range)) && (this.hasMixedValue || (value != ((float) this.m_Value))))
                {
                    this.ApplyProperty(value);
                }
            }
        }
        /// <summary>
        /// <para>Texture value of the property.</para>
        /// </summary>
        public Texture textureValue
        {
            get
            {
                if (this.m_Type == PropType.Texture)
                {
                    return (Texture) this.m_Value;
                }
                return null;
            }
            set
            {
                if ((this.m_Type == PropType.Texture) && (this.hasMixedValue || (value != ((Texture) this.m_Value))))
                {
                    this.m_MixedValueMask &= -2;
                    object previousValue = this.m_Value;
                    this.m_Value = value;
                    this.ApplyProperty(previousValue, 1);
                }
            }
        }
        public Vector4 textureScaleAndOffset
        {
            get
            {
                if (this.m_Type == PropType.Texture)
                {
                    return this.m_TextureScaleAndOffset;
                }
                return Vector4.zero;
            }
            set
            {
                if ((this.m_Type == PropType.Texture) && (this.hasMixedValue || (value != this.m_TextureScaleAndOffset)))
                {
                    this.m_MixedValueMask &= 1;
                    int changedPropertyMask = 0;
                    for (int i = 1; i < 5; i++)
                    {
                        changedPropertyMask |= ((int) 1) << i;
                    }
                    object textureScaleAndOffset = this.m_TextureScaleAndOffset;
                    this.m_TextureScaleAndOffset = value;
                    this.ApplyProperty(textureScaleAndOffset, changedPropertyMask);
                }
            }
        }
        private void ApplyProperty(object newValue)
        {
            this.m_MixedValueMask = 0;
            object previousValue = this.m_Value;
            this.m_Value = newValue;
            this.ApplyProperty(previousValue, 1);
        }

        private void ApplyProperty(object previousValue, int changedPropertyMask)
        {
            string name;
            if ((this.targets == null) || (this.targets.Length == 0))
            {
                throw new ArgumentException("No material targets provided");
            }
            Object[] targets = this.targets;
            if (targets.Length == 1)
            {
                name = targets[0].name;
            }
            else
            {
                object[] objArray1 = new object[] { targets.Length, " ", ObjectNames.NicifyVariableName(ObjectNames.GetClassName(targets[0])), "s" };
                name = string.Concat(objArray1);
            }
            bool flag = false;
            if (this.m_ApplyPropertyCallback != null)
            {
                flag = this.m_ApplyPropertyCallback(this, changedPropertyMask, previousValue);
            }
            if (!flag)
            {
                ShaderUtil.ApplyProperty(this, changedPropertyMask, "Modify " + this.displayName + " of " + name);
            }
        }
        public delegate bool ApplyPropertyCallback(MaterialProperty prop, int changeMask, object previousValue);

        /// <summary>
        /// <para>Flags that control how a MaterialProperty is displayed.</para>
        /// </summary>
        [Flags]
        public enum PropFlags
        {
            /// <summary>
            /// <para>Signifies that values of this property contain High Dynamic Range (HDR) data.</para>
            /// </summary>
            HDR = 0x10,
            /// <summary>
            /// <para>Do not show the property in the inspector.</para>
            /// </summary>
            HideInInspector = 1,
            /// <summary>
            /// <para>No flags are set.</para>
            /// </summary>
            None = 0,
            /// <summary>
            /// <para>Signifies that values of this property contain Normal (normalized vector) data.</para>
            /// </summary>
            Normal = 8,
            /// <summary>
            /// <para>Do not show UV scale/offset fields next to a texture.</para>
            /// </summary>
            NoScaleOffset = 4,
            /// <summary>
            /// <para>Texture value for this property will be queried from renderer's MaterialPropertyBlock, instead of from the material. This corresponds to the "[PerRendererData]" attribute in front of a property in the shader code.</para>
            /// </summary>
            PerRendererData = 2
        }

        /// <summary>
        /// <para>Material property type.</para>
        /// </summary>
        public enum PropType
        {
            Color,
            Vector,
            Float,
            Range,
            Texture
        }

        [Obsolete("Use UnityEngine.Rendering.TextureDimension instead", false)]
        public enum TexDim
        {
            Any = 6,
            Cube = 4,
            None = 0,
            Tex2D = 2,
            Tex3D = 3,
            Unknown = -1
        }
    }
}

