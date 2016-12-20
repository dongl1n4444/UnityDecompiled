namespace UnityEditor.AnimatedValues
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;

    /// <summary>
    /// <para>An animated float value.</para>
    /// </summary>
    [Serializable]
    public class AnimFloat : BaseAnimValue<float>
    {
        [SerializeField]
        private float m_Value;

        /// <summary>
        /// <para>Constructor.</para>
        /// </summary>
        /// <param name="value">Start Value.</param>
        /// <param name="callback"></param>
        public AnimFloat(float value) : base(value)
        {
        }

        /// <summary>
        /// <para>Constructor.</para>
        /// </summary>
        /// <param name="value">Start Value.</param>
        /// <param name="callback"></param>
        public AnimFloat(float value, UnityAction callback) : base(value, callback)
        {
        }

        /// <summary>
        /// <para>Type specific implementation of BaseAnimValue_1.GetValue.</para>
        /// </summary>
        /// <returns>
        /// <para>Current Value.</para>
        /// </returns>
        protected override float GetValue()
        {
            this.m_Value = Mathf.Lerp(base.start, base.target, base.lerpPosition);
            return this.m_Value;
        }
    }
}

