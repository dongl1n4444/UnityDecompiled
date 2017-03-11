namespace UnityEditor.VersionControl
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>This class describes the.</para>
    /// </summary>
    public sealed class ConfigField
    {
        private string m_guid;
        private IntPtr m_thisDummy;

        internal ConfigField()
        {
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator, ThreadAndSerializationSafe]
        public extern void Dispose();
        ~ConfigField()
        {
            this.Dispose();
        }

        /// <summary>
        /// <para>Descrition of the configuration field.</para>
        /// </summary>
        public string description { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>This is true if the configuration field is a password field.</para>
        /// </summary>
        public bool isPassword { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>This is true if the configuration field is required for the version control plugin to function correctly.</para>
        /// </summary>
        public bool isRequired { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Label that is displayed next to the configuration field in the editor.</para>
        /// </summary>
        public string label { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Name of the configuration field.</para>
        /// </summary>
        public string name { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }
    }
}

