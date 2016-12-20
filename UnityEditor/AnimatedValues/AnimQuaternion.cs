namespace UnityEditor.AnimatedValues
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;

    /// <summary>
    /// <para>An animated Quaternion value.</para>
    /// </summary>
    [Serializable]
    public class AnimQuaternion : BaseAnimValue<Quaternion>
    {
        [SerializeField]
        private Quaternion m_Value;

        /// <summary>
        /// <para>Constructor.</para>
        /// </summary>
        /// <param name="value">Start Value.</param>
        /// <param name="callback"></param>
        public AnimQuaternion(Quaternion value) : base(value)
        {
        }

        /// <summary>
        /// <para>Constructor.</para>
        /// </summary>
        /// <param name="value">Start Value.</param>
        /// <param name="callback"></param>
        public AnimQuaternion(Quaternion value, UnityAction callback) : base(value, callback)
        {
        }

        /// <summary>
        /// <para>Type specific implementation of BaseAnimValue_1.GetValue.</para>
        /// </summary>
        /// <returns>
        /// <para>Current Value.</para>
        /// </returns>
        protected override Quaternion GetValue()
        {
            this.m_Value = Quaternion.Slerp(base.start, base.target, base.lerpPosition);
            return this.m_Value;
        }
    }
}

