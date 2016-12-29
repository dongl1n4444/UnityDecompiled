namespace UnityEditor.Collaboration
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal class SoftLock
    {
        private string m_UserID;
        private string m_MachineID;
        private string m_DisplayName;
        private ulong m_TimeStamp;
        private SoftLock()
        {
        }

        public string userID =>
            this.m_UserID;
        public string machineID =>
            this.m_MachineID;
        public string displayName =>
            this.m_DisplayName;
        public ulong timeStamp =>
            this.m_TimeStamp;
    }
}

