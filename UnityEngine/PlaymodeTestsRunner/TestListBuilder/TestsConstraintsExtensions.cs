namespace UnityEngine.PlaymodeTestsRunner.TestListBuilder
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.PlaymodeTestsRunner;

    internal static class TestsConstraintsExtensions
    {
        internal static TestsConstraint GetTestsConstraint(this TestPlatform testPlatform)
        {
            if (testPlatform != TestPlatform.EditMode)
            {
                if (testPlatform != TestPlatform.PlayMode)
                {
                    throw new NotImplementedException();
                }
            }
            else
            {
                return new EditModeTestsConstraints();
            }
            return new PlayModeTestsConstraints();
        }
    }
}

