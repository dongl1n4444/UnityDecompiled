namespace UnityEngine
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>Mark a ScriptableObject-derived type to be automatically listed in the Assets/Create submenu, so that instances of the type can be easily created and stored in the project as ".asset" files.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=false)]
    public sealed class CreateAssetMenuAttribute : Attribute
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private string <fileName>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <menuName>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private int <order>k__BackingField;

        /// <summary>
        /// <para>The default file name used by newly created instances of this type.</para>
        /// </summary>
        public string fileName { get; set; }

        /// <summary>
        /// <para>The display name for this type shown in the Assets/Create menu.</para>
        /// </summary>
        public string menuName { get; set; }

        /// <summary>
        /// <para>The position of the menu item within the Assets/Create menu.</para>
        /// </summary>
        public int order { get; set; }
    }
}

