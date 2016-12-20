namespace UnityEditor.VersionControl
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    /// <summary>
    /// <para>The plugin class describes a version control plugin and which configuratin options it has.</para>
    /// </summary>
    public sealed class Plugin
    {
        private string m_guid;
        private IntPtr m_thisDummy;

        internal Plugin()
        {
        }

        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe]
        public extern void Dispose();

        public static Plugin[] availablePlugins { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Configuration fields of the plugin.</para>
        /// </summary>
        public ConfigField[] configFields { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public string name { [MethodImpl(MethodImplOptions.InternalCall)] get; }
    }
}

