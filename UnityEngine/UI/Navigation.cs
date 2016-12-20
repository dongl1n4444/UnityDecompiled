namespace UnityEngine.UI
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Serialization;

    /// <summary>
    /// <para>Structure storing details related to navigation.</para>
    /// </summary>
    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct Navigation : IEquatable<Navigation>
    {
        [FormerlySerializedAs("mode"), SerializeField]
        private Mode m_Mode;
        [FormerlySerializedAs("selectOnUp"), SerializeField]
        private Selectable m_SelectOnUp;
        [FormerlySerializedAs("selectOnDown"), SerializeField]
        private Selectable m_SelectOnDown;
        [FormerlySerializedAs("selectOnLeft"), SerializeField]
        private Selectable m_SelectOnLeft;
        [FormerlySerializedAs("selectOnRight"), SerializeField]
        private Selectable m_SelectOnRight;
        /// <summary>
        /// <para>Navigation mode.</para>
        /// </summary>
        public Mode mode
        {
            get
            {
                return this.m_Mode;
            }
            set
            {
                this.m_Mode = value;
            }
        }
        /// <summary>
        /// <para>Specify a Selectable UI GameObject to highlight when the Up arrow key is pressed.</para>
        /// </summary>
        public Selectable selectOnUp
        {
            get
            {
                return this.m_SelectOnUp;
            }
            set
            {
                this.m_SelectOnUp = value;
            }
        }
        /// <summary>
        /// <para>Specify a Selectable UI GameObject to highlight when the down arrow key is pressed.</para>
        /// </summary>
        public Selectable selectOnDown
        {
            get
            {
                return this.m_SelectOnDown;
            }
            set
            {
                this.m_SelectOnDown = value;
            }
        }
        /// <summary>
        /// <para>Specify a Selectable UI GameObject to highlight when the left arrow key is pressed.</para>
        /// </summary>
        public Selectable selectOnLeft
        {
            get
            {
                return this.m_SelectOnLeft;
            }
            set
            {
                this.m_SelectOnLeft = value;
            }
        }
        /// <summary>
        /// <para>Specify a Selectable UI GameObject to highlight when the right arrow key is pressed.</para>
        /// </summary>
        public Selectable selectOnRight
        {
            get
            {
                return this.m_SelectOnRight;
            }
            set
            {
                this.m_SelectOnRight = value;
            }
        }
        /// <summary>
        /// <para>Return a Navigation with sensible default values.</para>
        /// </summary>
        public static Navigation defaultNavigation
        {
            get
            {
                return new Navigation { m_Mode = Mode.Automatic };
            }
        }
        public bool Equals(Navigation other)
        {
            return ((((this.mode == other.mode) && (this.selectOnUp == other.selectOnUp)) && ((this.selectOnDown == other.selectOnDown) && (this.selectOnLeft == other.selectOnLeft))) && (this.selectOnRight == other.selectOnRight));
        }
        /// <summary>
        /// <para>Navigation mode. Used by Selectable.</para>
        /// </summary>
        [Flags]
        public enum Mode
        {
            None,
            Horizontal,
            Vertical,
            Automatic,
            Explicit
        }
    }
}

