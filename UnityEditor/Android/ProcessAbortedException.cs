namespace UnityEditor.Android
{
    using System;

    internal class ProcessAbortedException : Exception
    {
        public ProcessAbortedException(string message) : base(message)
        {
        }
    }
}

