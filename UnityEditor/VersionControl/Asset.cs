namespace UnityEditor.VersionControl
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Text;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>This class containes information about the version control state of an asset.</para>
    /// </summary>
    public sealed class Asset
    {
        private GUID m_guid;

        public Asset(string clientPath)
        {
            this.InternalCreateFromString(clientPath);
        }

        internal string AllStateToString() => 
            AllStateToString(this.state);

        internal static string AllStateToString(States state)
        {
            StringBuilder builder = new StringBuilder();
            if (IsState(state, States.AddedLocal))
            {
                builder.AppendLine("Added Local");
            }
            if (IsState(state, States.AddedRemote))
            {
                builder.AppendLine("Added Remote");
            }
            if (IsState(state, States.CheckedOutLocal))
            {
                builder.AppendLine("Checked Out Local");
            }
            if (IsState(state, States.CheckedOutRemote))
            {
                builder.AppendLine("Checked Out Remote");
            }
            if (IsState(state, States.Conflicted))
            {
                builder.AppendLine("Conflicted");
            }
            if (IsState(state, States.DeletedLocal))
            {
                builder.AppendLine("Deleted Local");
            }
            if (IsState(state, States.DeletedRemote))
            {
                builder.AppendLine("Deleted Remote");
            }
            if (IsState(state, States.Local))
            {
                builder.AppendLine("Local");
            }
            if (IsState(state, States.LockedLocal))
            {
                builder.AppendLine("Locked Local");
            }
            if (IsState(state, States.LockedRemote))
            {
                builder.AppendLine("Locked Remote");
            }
            if (IsState(state, States.OutOfSync))
            {
                builder.AppendLine("Out Of Sync");
            }
            if (IsState(state, States.Synced))
            {
                builder.AppendLine("Synced");
            }
            if (IsState(state, States.Missing))
            {
                builder.AppendLine("Missing");
            }
            if (IsState(state, States.ReadOnly))
            {
                builder.AppendLine("ReadOnly");
            }
            return builder.ToString();
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator, ThreadAndSerializationSafe]
        public extern void Dispose();
        /// <summary>
        /// <para>Opens the assets in an associated editor.</para>
        /// </summary>
        public void Edit()
        {
            UnityEngine.Object target = this.Load();
            if (target != null)
            {
                AssetDatabase.OpenAsset(target);
            }
        }

        ~Asset()
        {
            this.Dispose();
        }

        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe, GeneratedByOldBindingsGenerator]
        private extern void InternalCreateFromString(string clientPath);
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe, GeneratedByOldBindingsGenerator]
        public extern bool IsChildOf(Asset other);
        public bool IsOneOfStates(States[] states)
        {
            foreach (States states2 in states)
            {
                if ((this.state & states2) != States.None)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsState(States state) => 
            IsState(this.state, state);

        internal static bool IsState(States isThisState, States partOfThisState) => 
            ((isThisState & partOfThisState) != States.None);

        /// <summary>
        /// <para>Loads the asset to memory.</para>
        /// </summary>
        public UnityEngine.Object Load()
        {
            if ((this.state == States.DeletedLocal) || this.isMeta)
            {
                return null;
            }
            return AssetDatabase.LoadAssetAtPath(this.path, typeof(UnityEngine.Object));
        }

        internal string StateToString() => 
            StateToString(this.state);

        internal static string StateToString(States state)
        {
            if (IsState(state, States.AddedLocal))
            {
                return "Added Local";
            }
            if (IsState(state, States.AddedRemote))
            {
                return "Added Remote";
            }
            if (IsState(state, States.CheckedOutLocal) && !IsState(state, States.LockedLocal))
            {
                return "Checked Out Local";
            }
            if (IsState(state, States.CheckedOutRemote) && !IsState(state, States.LockedRemote))
            {
                return "Checked Out Remote";
            }
            if (IsState(state, States.Conflicted))
            {
                return "Conflicted";
            }
            if (IsState(state, States.DeletedLocal))
            {
                return "Deleted Local";
            }
            if (IsState(state, States.DeletedRemote))
            {
                return "Deleted Remote";
            }
            if (IsState(state, States.Local))
            {
                return "Local";
            }
            if (IsState(state, States.LockedLocal))
            {
                return "Locked Local";
            }
            if (IsState(state, States.LockedRemote))
            {
                return "Locked Remote";
            }
            if (IsState(state, States.OutOfSync))
            {
                return "Out Of Sync";
            }
            return "";
        }

        /// <summary>
        /// <para>Gets the full name of the asset including extension.</para>
        /// </summary>
        [ThreadAndSerializationSafe]
        public string fullName { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Returns true if the asset is a folder.</para>
        /// </summary>
        [ThreadAndSerializationSafe]
        public bool isFolder { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Returns true if the assets is in the current project.</para>
        /// </summary>
        [ThreadAndSerializationSafe]
        public bool isInCurrentProject { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Returns true if the instance of the Asset class actually refers to a .meta file.</para>
        /// </summary>
        [ThreadAndSerializationSafe]
        public bool isMeta { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        internal bool IsUnderVersionControl =>
            ((this.IsState(States.Synced) || this.IsState(States.OutOfSync)) || this.IsState(States.AddedLocal));

        /// <summary>
        /// <para>Returns true if the asset is locked by the version control system.</para>
        /// </summary>
        [ThreadAndSerializationSafe]
        public bool locked { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Get the name of the asset.</para>
        /// </summary>
        [ThreadAndSerializationSafe]
        public string name { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Gets the path of the asset.</para>
        /// </summary>
        [ThreadAndSerializationSafe]
        public string path { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        public string prettyPath =>
            this.path;

        /// <summary>
        /// <para>Returns true is the asset is read only.</para>
        /// </summary>
        [ThreadAndSerializationSafe]
        public bool readOnly { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Gets the version control state of the asset.</para>
        /// </summary>
        [ThreadAndSerializationSafe]
        public States state { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Describes the various version control states an asset can have.</para>
        /// </summary>
        [Flags]
        public enum States
        {
            /// <summary>
            /// <para>The was locally added to version control.</para>
            /// </summary>
            AddedLocal = 0x100,
            /// <summary>
            /// <para>Remotely this asset was added to version control.</para>
            /// </summary>
            AddedRemote = 0x200,
            /// <summary>
            /// <para>The asset has been checked out on the local machine.</para>
            /// </summary>
            CheckedOutLocal = 0x10,
            /// <summary>
            /// <para>The asset has been checked out on a remote machine.</para>
            /// </summary>
            CheckedOutRemote = 0x20,
            /// <summary>
            /// <para>There is a conflict with the asset that needs to be resolved.</para>
            /// </summary>
            Conflicted = 0x400,
            /// <summary>
            /// <para>The asset has been deleted locally.</para>
            /// </summary>
            DeletedLocal = 0x40,
            /// <summary>
            /// <para>The asset has been deleted on a remote machine.</para>
            /// </summary>
            DeletedRemote = 0x80,
            /// <summary>
            /// <para>The asset is not under version control.</para>
            /// </summary>
            Local = 1,
            /// <summary>
            /// <para>The asset is locked by the local machine.</para>
            /// </summary>
            LockedLocal = 0x800,
            /// <summary>
            /// <para>The asset is locked by a remote machine.</para>
            /// </summary>
            LockedRemote = 0x1000,
            /// <summary>
            /// <para>This instance of the class actaully refers to a .meta file.</para>
            /// </summary>
            MetaFile = 0x8000,
            /// <summary>
            /// <para>The asset exists in version control but is missing on the local machine.</para>
            /// </summary>
            Missing = 8,
            /// <summary>
            /// <para>The version control state is unknown.</para>
            /// </summary>
            None = 0,
            /// <summary>
            /// <para>A newer version of the asset is available on the version control server.</para>
            /// </summary>
            OutOfSync = 4,
            /// <summary>
            /// <para>The asset is read only.</para>
            /// </summary>
            ReadOnly = 0x4000,
            /// <summary>
            /// <para>The asset is up to date.</para>
            /// </summary>
            Synced = 2,
            /// <summary>
            /// <para>The state of the asset is currently being queried from the version control server.</para>
            /// </summary>
            Updating = 0x2000
        }
    }
}

