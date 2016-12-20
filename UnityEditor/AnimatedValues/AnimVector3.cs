namespace UnityEditor.AnimatedValues
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;

    /// <summary>
    /// <para>An animated Vector3 value.</para>
    /// </summary>
    [Serializable]
    public class AnimVector3 : BaseAnimValue<Vector3>
    {
        [SerializeField]
        private Vector3 m_Value;

        /// <summary>
        /// <para>Constructor.</para>
        /// </summary>
        /// <param name="value">Start Value.</param>
        /// <param name="callback"></param>
        public AnimVector3() : base(Vector3.zero)
        {
        }

        /// <summary>
        /// <para>Constructor.</para>
        /// </summary>
        /// <param name="value">Start Value.</param>
        /// <param name="callback"></param>
        public AnimVector3(Vector3 value) : base(value)
        {
        }

        /// <summary>
        /// <para>Constructor.</para>
        /// </summary>
        /// <param name="value">Start Value.</param>
        /// <param name="callback"></param>
        public AnimVector3(Vector3 value, UnityAction callback) : base(value, callback)
        {
        }

        /// <summary>
        /// <para>Type specific implementation of BaseAnimValue_1.GetValue.</para>
        /// </summary>
        /// <returns>
        /// <para>Current Value.</para>
        /// </returns>
        protected override Vector3 GetValue()
        {
            this.m_Value = Vector3.Lerp(base.start, base.target, base.lerpPosition);
            return this.m_Value;
        }
    }
}

