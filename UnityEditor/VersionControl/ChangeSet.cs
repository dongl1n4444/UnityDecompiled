namespace UnityEditor.VersionControl
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Wrapper around a changeset description and ID.</para>
    /// </summary>
    public sealed class ChangeSet
    {
        /// <summary>
        /// <para>The ID of  the default changeset.</para>
        /// </summary>
        public static string defaultID = "-1";
        private IntPtr m_thisDummy;

        public ChangeSet()
        {
            this.InternalCreate();
        }

        public ChangeSet(string description)
        {
            this.InternalCreateFromString(description);
        }

        public ChangeSet(ChangeSet other)
        {
            this.InternalCopyConstruct(other);
        }

        public ChangeSet(string description, string revision)
        {
            this.InternalCreateFromStringString(description, revision);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator, ThreadAndSerializationSafe]
        public extern void Dispose();
        ~ChangeSet()
        {
            this.Dispose();
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator, ThreadAndSerializationSafe]
        private extern void InternalCopyConstruct(ChangeSet other);
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe, GeneratedByOldBindingsGenerator]
        private extern void InternalCreate();
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe, GeneratedByOldBindingsGenerator]
        private extern void InternalCreateFromString(string description);
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe, GeneratedByOldBindingsGenerator]
        private extern void InternalCreateFromStringString(string description, string changeSetID);

        /// <summary>
        /// <para>Description of a changeset.</para>
        /// </summary>
        [ThreadAndSerializationSafe]
        public string description { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Version control specific ID of a changeset.</para>
        /// </summary>
        [ThreadAndSerializationSafe]
        public string id { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }
    }
}

