namespace UnityEditor.AnimatedValues
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;

    /// <summary>
    /// <para>Lerp from 0 - 1.</para>
    /// </summary>
    [Serializable]
    public class AnimBool : BaseAnimValue<bool>
    {
        [SerializeField]
        private float m_Value;

        /// <summary>
        /// <para>Constructor.</para>
        /// </summary>
        /// <param name="value">Start Value.</param>
        /// <param name="callback"></param>
        public AnimBool() : base(false)
        {
        }

        /// <summary>
        /// <para>Constructor.</para>
        /// </summary>
        /// <param name="value">Start Value.</param>
        /// <param name="callback"></param>
        public AnimBool(bool value) : base(value)
        {
        }

        /// <summary>
        /// <para>Constructor.</para>
        /// </summary>
        /// <param name="value">Start Value.</param>
        /// <param name="callback"></param>
        public AnimBool(UnityAction callback) : base(false, callback)
        {
        }

        /// <summary>
        /// <para>Constructor.</para>
        /// </summary>
        /// <param name="value">Start Value.</param>
        /// <param name="callback"></param>
        public AnimBool(bool value, UnityAction callback) : base(value, callback)
        {
        }

        /// <summary>
        /// <para>Returns a value between from and to depending on the current value of the bools animation.</para>
        /// </summary>
        /// <param name="from">Value to lerp from.</param>
        /// <param name="to">Value to lerp to.</param>
        public float Fade(float from, float to) => 
            Mathf.Lerp(from, to, this.faded);

        /// <summary>
        /// <para>Type specific implementation of BaseAnimValue_1.GetValue.</para>
        /// </summary>
        /// <returns>
        /// <para>Current value.</para>
        /// </returns>
        protected override bool GetValue()
        {
            float a = !base.target ? 1f : 0f;
            float b = 1f - a;
            this.m_Value = Mathf.Lerp(a, b, base.lerpPosition);
            return (this.m_Value > 0.5f);
        }

        /// <summary>
        /// <para>Retuns the float value of the tween.</para>
        /// </summary>
        public float faded
        {
            get
            {
                this.GetValue();
                return this.m_Value;
            }
        }
    }
}

