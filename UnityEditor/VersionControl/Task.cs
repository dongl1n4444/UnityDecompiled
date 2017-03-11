namespace UnityEditor.VersionControl
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>A UnityEditor.VersionControl.Task is created almost everytime UnityEditor.VersionControl.Provider is ask to perform an action.</para>
    /// </summary>
    public sealed class Task
    {
        private IntPtr m_thisDummy;

        internal Task()
        {
        }

        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe, GeneratedByOldBindingsGenerator]
        public extern void Dispose();
        ~Task()
        {
            this.Dispose();
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern Asset[] Internal_GetAssetList();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern ChangeSet[] Internal_GetChangeSets();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern Message[] Internal_GetMessages();
        /// <summary>
        /// <para>Upon completion of a task a completion task will be performed if it is set.</para>
        /// </summary>
        /// <param name="action">Which completion action to perform.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void SetCompletionAction(CompletionAction action);
        /// <summary>
        /// <para>A blocking wait for the task to complete.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void Wait();

        /// <summary>
        /// <para>The result of some types of tasks.</para>
        /// </summary>
        public AssetList assetList
        {
            get
            {
                AssetList list = new AssetList();
                Asset[] assetArray = this.Internal_GetAssetList();
                foreach (Asset asset in assetArray)
                {
                    list.Add(asset);
                }
                return list;
            }
        }

        /// <summary>
        /// <para>List of changesets returned by some tasks.</para>
        /// </summary>
        public ChangeSets changeSets
        {
            get
            {
                ChangeSets sets = new ChangeSets();
                ChangeSet[] setArray = this.Internal_GetChangeSets();
                foreach (ChangeSet set in setArray)
                {
                    sets.Add(set);
                }
                return sets;
            }
        }

        /// <summary>
        /// <para>A short description of the current task.</para>
        /// </summary>
        public string description { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>May contain messages from the version control plugins.</para>
        /// </summary>
        public Message[] messages { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        public string progressMessage { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Progress of current task in precent.</para>
        /// </summary>
        public int progressPct { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Some task return result codes, these are stored here.</para>
        /// </summary>
        public int resultCode { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Total time spent in task since the task was started.</para>
        /// </summary>
        public int secondsSpent { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Get whether or not the task was completed succesfully.</para>
        /// </summary>
        public bool success { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Will contain the result of the Provider.ChangeSetDescription task.</para>
        /// </summary>
        public string text { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        public int userIdentifier { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}

