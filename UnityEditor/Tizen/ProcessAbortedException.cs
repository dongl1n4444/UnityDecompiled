namespace UnityEditor.Tizen
{
    using System;

    internal class ProcessAbortedException : Exception
    {
        public ProcessAbortedException(string message) : base(message)
        {
        }
    }
}

