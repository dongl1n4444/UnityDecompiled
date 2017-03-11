namespace Unity.IL2CPP.Running
{
    using System;

    public class RunnerFailedException : Exception
    {
        internal RunnerFailedException(string message) : base(message)
        {
        }
    }
}

