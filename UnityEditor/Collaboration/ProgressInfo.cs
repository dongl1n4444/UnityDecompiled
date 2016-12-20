namespace UnityEditor.Collaboration
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal class ProgressInfo
    {
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

        public string title
        {
            get
            {
                return this.m_Title;
            }
        }
        public string extraInfo
        {
            get
            {
                return this.m_ExtraInfo;
            }
        }
        public int currentCount
        {
            get
            {
                return this.m_CurrentCount;
            }
        }
        public int totalCount
        {
            get
            {
                return this.m_TotalCount;
            }
        }
        public bool completed
        {
            get
            {
                return (this.m_Completed != 0);
            }
        }
        public bool cancelled
        {
            get
            {
                return (this.m_Cancelled != 0);
            }
        }
        public bool canCancel
        {
            get
            {
                return (this.m_CanCancel != 0);
            }
        }
        public string lastErrorString
        {
            get
            {
                return this.m_LastErrorString;
            }
        }
        public ulong lastError
        {
            get
            {
                return this.m_LastError;
            }
        }
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
        public bool isProgressTypeCount
        {
            get
            {
                return ((this.m_ProgressType == ProgressType.Count) || (this.m_ProgressType == ProgressType.Both));
            }
        }
        public bool isProgressTypePercent
        {
            get
            {
                return ((this.m_ProgressType == ProgressType.Percent) || (this.m_ProgressType == ProgressType.Both));
            }
        }
        public bool errorOccured
        {
            get
            {
                return (this.m_LastError != 0L);
            }
        }
        public enum ProgressType : uint
        {
            Both = 3,
            Count = 1,
            None = 0,
            Percent = 2
        }
    }
}

