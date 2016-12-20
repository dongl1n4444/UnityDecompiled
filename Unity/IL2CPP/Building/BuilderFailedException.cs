namespace Unity.IL2CPP.Building
{
    using System;

    public class BuilderFailedException : Exception
    {
        public BuilderFailedException(string failureReason) : base(failureReason)
        {
        }
    }
}

