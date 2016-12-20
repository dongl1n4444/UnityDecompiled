namespace UnityEngine
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>Base class to derive custom property attributes from. Use this to create custom attributes for script variables.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited=true, AllowMultiple=false)]
    public abstract class PropertyAttribute : Attribute
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int <order>k__BackingField;

        protected PropertyAttribute()
        {
        }

        /// <summary>
        /// <para>Optional field to specify the order that multiple DecorationDrawers should be drawn in.</para>
        /// </summary>
        public int order { get; set; }
    }
}

