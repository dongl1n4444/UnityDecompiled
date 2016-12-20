namespace UnityEngine.PlaymodeTests
{
    using System;

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class EditModeTestAttribute : TestAttribute
    {
        public EditModeTestAttribute() : base(TestPlatform.EditMode)
        {
        }
    }
}

