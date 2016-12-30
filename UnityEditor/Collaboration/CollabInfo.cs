namespace UnityEditor.Collaboration
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.SceneManagement;

    [StructLayout(LayoutKind.Sequential)]
    internal struct CollabInfo
    {
        private int m_Ready;
        private int m_Update;
        private int m_Publish;
        private int m_InProgress;
        private int m_Error;
        private int m_Maintenance;
        private int m_Conflict;
        private int m_Refresh;
        private string m_Tip;
        private string m_LastErrorMsg;
        public bool ready =>
            (this.m_Ready != 0);
        public bool update =>
            (this.m_Update != 0);
        public bool publish =>
            (this.m_Publish != 0);
        public bool inProgress =>
            (this.m_InProgress != 0);
        public bool error =>
            (this.m_Error != 0);
        public bool maintenance =>
            (this.m_Maintenance != 0);
        public bool conflict =>
            (this.m_Conflict != 0);
        public bool dirty =>
            SceneManager.GetActiveScene().isDirty;
        public bool refresh =>
            (this.m_Refresh != 0);
        public string tip =>
            this.m_Tip;
        public string lastErrorMsg =>
            this.m_LastErrorMsg;
    }
}

