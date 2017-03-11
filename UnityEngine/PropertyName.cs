namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Represents a string as an int for efficient lookup and comparison. Use this for common PropertyNames.
    /// 
    /// Internally stores just an int to represent the string. A PropertyName can be created from a string but can not be converted back to a string. The same string always results in the same int representing that string. Thus this is a very efficient string representation in both memory and speed when all you need is comparison.
    /// 
    /// PropertyName is serializable.
    /// 
    /// ToString() is only implemented for debugging purposes in the editor it returns "theName:3737" in the player it returns "Unknown:3737".</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), UsedByNativeCode]
    public struct PropertyName
    {
        internal int id;
        internal int conflictIndex;
        /// <summary>
        /// <para>Initializes the PropertyName using a string.</para>
        /// </summary>
        /// <param name="name"></param>
        public PropertyName(string name) : this(PropertyNameUtils.PropertyNameFromString(name))
        {
        }

        public PropertyName(PropertyName other)
        {
            this.id = other.id;
            this.conflictIndex = other.conflictIndex;
        }

        private PropertyName(int id)
        {
            this.id = id;
            this.conflictIndex = 0;
        }

        /// <summary>
        /// <para>Indicates whether the specified PropertyName is an Empty string.</para>
        /// </summary>
        /// <param name="prop"></param>
        public static bool IsNullOrEmpty(PropertyName prop) => 
            (prop.id == 0);

        public static bool operator ==(PropertyName lhs, PropertyName rhs) => 
            (lhs.id == rhs.id);

        public static bool operator !=(PropertyName lhs, PropertyName rhs) => 
            (lhs.id != rhs.id);

        /// <summary>
        /// <para>Returns the hash code for this PropertyName.</para>
        /// </summary>
        public override int GetHashCode() => 
            this.id;

        /// <summary>
        /// <para>Determines whether this instance and a specified object, which must also be a PropertyName object, have the same value.</para>
        /// </summary>
        /// <param name="other"></param>
        public override bool Equals(object other) => 
            ((other is PropertyName) && (this == ((PropertyName) other)));

        public static implicit operator PropertyName(string name) => 
            new PropertyName(name);

        public static implicit operator PropertyName(int id) => 
            new PropertyName(id);

        /// <summary>
        /// <para>For debugging purposes only. Returns the string value representing the string in the Editor.
        /// Returns "UnityEngine.PropertyName" in the player.</para>
        /// </summary>
        public override string ToString()
        {
            int num = PropertyNameUtils.ConflictCountForID(this.id);
            string str = $"{PropertyNameUtils.StringFromPropertyName(this)}:{this.id}";
            if (num <= 0)
            {
                return str;
            }
            StringBuilder builder = new StringBuilder(str);
            builder.Append(" conflicts with ");
            for (int i = 0; i < num; i++)
            {
                if (i != this.conflictIndex)
                {
                    PropertyName propertyName = new PropertyName(this.id) {
                        conflictIndex = i
                    };
                    builder.AppendFormat("\"{0}\"", PropertyNameUtils.StringFromPropertyName(propertyName));
                }
            }
            return builder.ToString();
        }
    }
}

