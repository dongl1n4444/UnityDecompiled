namespace UnityEditor
{
    using System;
    using UnityEngine;

    /// <summary>
    /// <para>Used as input to ColorField to configure the HDR color ranges in the ColorPicker.</para>
    /// </summary>
    [Serializable]
    public class ColorPickerHDRConfig
    {
        /// <summary>
        /// <para>Maximum allowed color component value when using the ColorPicker.</para>
        /// </summary>
        [SerializeField]
        public float maxBrightness;
        /// <summary>
        /// <para>Maximum exposure value allowed in the Color Picker.</para>
        /// </summary>
        [SerializeField]
        public float maxExposureValue;
        /// <summary>
        /// <para>Minimum allowed color component value when using the ColorPicker.</para>
        /// </summary>
        [SerializeField]
        public float minBrightness;
        /// <summary>
        /// <para>Minimum exposure value allowed in the Color Picker.</para>
        /// </summary>
        [SerializeField]
        public float minExposureValue;
        private static readonly ColorPickerHDRConfig s_Temp = new ColorPickerHDRConfig(0f, 0f, 0f, 0f);

        internal ColorPickerHDRConfig(ColorPickerHDRConfig other)
        {
            this.minBrightness = other.minBrightness;
            this.maxBrightness = other.maxBrightness;
            this.minExposureValue = other.minExposureValue;
            this.maxExposureValue = other.maxExposureValue;
        }

        /// <summary>
        /// <para></para>
        /// </summary>
        /// <param name="minBrightness">Minimum brightness value allowed when using the Color Picker.</param>
        /// <param name="maxBrightness">Maximum brightness value allowed when using the Color Picker.</param>
        /// <param name="minExposureValue">Minimum exposure value used in the tonemapping section of the Color Picker.</param>
        /// <param name="maxExposureValue">Maximum exposure value used in the tonemapping section of the Color Picker.</param>
        public ColorPickerHDRConfig(float minBrightness, float maxBrightness, float minExposureValue, float maxExposureValue)
        {
            this.minBrightness = minBrightness;
            this.maxBrightness = maxBrightness;
            this.minExposureValue = minExposureValue;
            this.maxExposureValue = maxExposureValue;
        }

        internal static ColorPickerHDRConfig Temp(float minBrightness, float maxBrightness, float minExposure, float maxExposure)
        {
            s_Temp.minBrightness = minBrightness;
            s_Temp.maxBrightness = maxBrightness;
            s_Temp.minExposureValue = minExposure;
            s_Temp.maxExposureValue = maxExposure;
            return s_Temp;
        }
    }
}

