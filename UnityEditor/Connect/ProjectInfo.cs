namespace UnityEditor.Connect
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct ProjectInfo
    {
        private int m_Valid;
        private int m_BuildAllowed;
        private int m_ProjectBound;
        private string m_ProjectGUID;
        private string m_ProjectName;
        private string m_OrganizationID;
        private string m_OrganizationName;
        private string m_OrganizationForeignKey;
        private int m_COPPA;
        private int m_COPPALock;
        private int m_MoveLock;
        public bool valid =>
            (this.m_Valid != 0);
        public bool buildAllowed =>
            (this.m_BuildAllowed != 0);
        public bool projectBound =>
            (this.m_ProjectBound != 0);
        public string projectGUID =>
            this.m_ProjectGUID;
        public string projectName =>
            this.m_ProjectName;
        public string organizationId =>
            this.m_OrganizationID;
        public string organizationName =>
            this.m_OrganizationName;
        public string organizationForeignKey =>
            this.m_OrganizationForeignKey;
        public COPPACompliance COPPA
        {
            get
            {
                if (this.m_COPPA == 1)
                {
                    return COPPACompliance.COPPACompliant;
                }
                if (this.m_COPPA == 2)
                {
                    return COPPACompliance.COPPANotCompliant;
                }
                return COPPACompliance.COPPAUndefined;
            }
        }
        public bool coppaLock =>
            (this.m_COPPALock != 0);
        public bool moveLock =>
            (this.m_MoveLock != 0);
    }
}

