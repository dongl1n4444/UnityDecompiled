namespace UnityEditor.Collaboration
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal class Change
    {
        private string m_Path;
        private Collab.CollabStates m_State;
        private RevertableStates m_RevertableState;
        private string m_RelatedTo;
        private string m_LocalStatus;
        private string m_RemoteStatus;
        private string m_ResolveStatus;
        private Change()
        {
        }

        public string path
        {
            get
            {
                return this.m_Path;
            }
        }
        public ulong state
        {
            get
            {
                return (ulong) this.m_State;
            }
        }
        public bool isRevertable
        {
            get
            {
                return ((this.m_RevertableState & RevertableStates.Revertable) == RevertableStates.Revertable);
            }
        }
        public ulong revertableState
        {
            get
            {
                return (ulong) this.m_RevertableState;
            }
        }
        public string relatedTo
        {
            get
            {
                return this.m_RelatedTo;
            }
        }
        public bool isMeta
        {
            get
            {
                return ((this.m_State & Collab.CollabStates.kCollabMetaFile) == Collab.CollabStates.kCollabMetaFile);
            }
        }
        public bool isConflict
        {
            get
            {
                return (((this.m_State & Collab.CollabStates.kCollabConflicted) == Collab.CollabStates.kCollabConflicted) || ((this.m_State & (Collab.CollabStates.kCollabNone | Collab.CollabStates.kCollabPendingMerge)) == (Collab.CollabStates.kCollabNone | Collab.CollabStates.kCollabPendingMerge)));
            }
        }
        public bool isFolderMeta
        {
            get
            {
                return ((this.m_State & Collab.CollabStates.kCollabFolderMetaFile) == Collab.CollabStates.kCollabFolderMetaFile);
            }
        }
        public bool isResolved
        {
            get
            {
                return ((((this.m_State & (Collab.CollabStates.kCollabNone | Collab.CollabStates.kCollabUseMine)) == (Collab.CollabStates.kCollabNone | Collab.CollabStates.kCollabUseMine)) || ((this.m_State & (Collab.CollabStates.kCollabNone | Collab.CollabStates.kCollabUseTheir)) == (Collab.CollabStates.kCollabNone | Collab.CollabStates.kCollabUseTheir))) || ((this.m_State & Collab.CollabStates.kCollabMerged) == Collab.CollabStates.kCollabMerged));
            }
        }
        public string localStatus
        {
            get
            {
                return this.m_LocalStatus;
            }
        }
        public string remoteStatus
        {
            get
            {
                return this.m_RemoteStatus;
            }
        }
        public string resolveStatus
        {
            get
            {
                return this.m_ResolveStatus;
            }
        }
        [Flags]
        public enum RevertableStates : ulong
        {
            InvalidRevertableState = 0x80000000L,
            NotRevertable = 2L,
            NotRevertable_File = 0x20L,
            NotRevertable_FileAdded = 0x80L,
            NotRevertable_Folder = 0x40L,
            NotRevertable_FolderAdded = 0x100L,
            NotRevertable_FolderContainsAdd = 0x200L,
            Revertable = 1L,
            Revertable_EmptyFolder = 0x10L,
            Revertable_File = 4L,
            Revertable_Folder = 8L
        }
    }
}

