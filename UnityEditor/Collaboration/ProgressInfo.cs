namespace UnityEditor.Collaboration
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal class ProgressInfo
    {
        private int m_JobId;
        private string m_Title;
        private string m_ExtraInfo;
        private ProgressType m_ProgressType;
        private int m_Percentage;
        private int m_CurrentCount;
        private int m_TotalCount;
        private int m_Completed;
        private int m_Cancelled;
        private int m_CanCancel;
        private string m_LastErrorString;
        private ulong m_LastError;
        private ProgressInfo()
        {
        }

        public int jobId =>
            this.m_JobId;
        public string title =>
            this.m_Title;
        public string extraInfo =>
            this.m_ExtraInfo;
        public int currentCount =>
            this.m_CurrentCount;
        public int totalCount =>
            this.m_TotalCount;
        public bool completed =>
            (this.m_Completed != 0);
        public bool cancelled =>
            (this.m_Cancelled != 0);
        public bool canCancel =>
            (this.m_CanCancel != 0);
        public string lastErrorString =>
            this.m_LastErrorString;
        public ulong lastError =>
            this.m_LastError;
        public int percentComplete
        {
            get
            {
                if ((this.m_ProgressType == ProgressType.Percent) || (this.m_ProgressType == ProgressType.Both))
                {
                    return this.m_Percentage;
                }
                if (this.m_ProgressType == ProgressType.Count)
                {
                    if (this.m_TotalCount == 0)
                    {
                        return 0;
                    }
                    return ((this.m_CurrentCount * 100) / this.m_TotalCount);
                }
                return 0;
            }
        }
        public bool isProgressTypeCount =>
            ((this.m_ProgressType == ProgressType.Count) || (this.m_ProgressType == ProgressType.Both));
        public bool isProgressTypePercent =>
            ((this.m_ProgressType == ProgressType.Percent) || (this.m_ProgressType == ProgressType.Both));
        public bool errorOccured =>
            (this.m_LastError != 0L);
        public enum ProgressType : uint
        {
            Both = 3,
            Count = 1,
            None = 0,
            Percent = 2
        }
    }
}

