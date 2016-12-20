namespace UnityEngine.PlaymodeTests
{
    using System;

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class PlayModeTestAttribute : TestAttribute
    {
        public PlayModeTestAttribute() : base(TestPlatform.PlayMode)
        {
        }
    }
}

