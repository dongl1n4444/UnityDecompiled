namespace Unity.IL2CPP.Building
{
    using System;

    public class CompilationResult : ProvideObjectResult
    {
        public TimeSpan Duration;
        public string InterestingOutput;
        public CompilationInvocation Invocation;
        public bool Success;
    }
}

