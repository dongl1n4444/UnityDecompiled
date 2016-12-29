namespace UnityEngine
{
    using System;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Used to communicate between scripting and the controller. Some parameters can be set in scripting and used by the controller, while other parameters are based on Custom Curves in Animation Clips and can be sampled using the scripting API.</para>
    /// </summary>
    [UsedByNativeCode]
    public sealed class AnimatorControllerParameter
    {
        internal bool m_DefaultBool;
        internal float m_DefaultFloat;
        internal int m_DefaultInt;
        internal string m_Name = "";
        internal AnimatorControllerParameterType m_Type;

        public override bool Equals(object o)
        {
            AnimatorControllerParameter parameter = o as AnimatorControllerParameter;
            return (((((parameter != null) && (this.m_Name == parameter.m_Name)) && ((this.m_Type == parameter.m_Type) && (this.m_DefaultFloat == parameter.m_DefaultFloat))) && (this.m_DefaultInt == parameter.m_DefaultInt)) && (this.m_DefaultBool == parameter.m_DefaultBool));
        }

        public override int GetHashCode() => 
            this.name.GetHashCode();

        /// <summary>
        /// <para>The default bool value for the parameter.</para>
        /// </summary>
        public bool defaultBool
        {
            get => 
                this.m_DefaultBool;
            set
            {
                this.m_DefaultBool = value;
            }
        }

        /// <summary>
        /// <para>The default bool value for the parameter.</para>
        /// </summary>
        public float defaultFloat
        {
            get => 
                this.m_DefaultFloat;
            set
            {
                this.m_DefaultFloat = value;
            }
        }

        /// <summary>
        /// <para>The default bool value for the parameter.</para>
        /// </summary>
        public int defaultInt
        {
            get => 
                this.m_DefaultInt;
            set
            {
                this.m_DefaultInt = value;
            }
        }

        /// <summary>
        /// <para>The name of the parameter.</para>
        /// </summary>
        public string name
        {
            get => 
                this.m_Name;
            set
            {
                this.m_Name = value;
            }
        }

        /// <summary>
        /// <para>Returns the hash of the parameter based on its name.</para>
        /// </summary>
        public int nameHash =>
            Animator.StringToHash(this.m_Name);

        /// <summary>
        /// <para>The type of the parameter.</para>
        /// </summary>
        public AnimatorControllerParameterType type
        {
            get => 
                this.m_Type;
            set
            {
                this.m_Type = value;
            }
        }
    }
}

