namespace UnityEditor.Collaboration
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal class Revision
    {
        private string m_AuthorName;
        private string m_Author;
        private string m_Comment;
        private string m_RevisionID;
        private string m_Reference;
        private ulong m_TimeStamp;
        private Revision()
        {
        }

        public string authorName =>
            this.m_AuthorName;
        public string author =>
            this.m_Author;
        public string comment =>
            this.m_Comment;
        public string revisionID =>
            this.m_RevisionID;
        public string reference =>
            this.m_Reference;
        public ulong timeStamp =>
            this.m_TimeStamp;
    }
}

