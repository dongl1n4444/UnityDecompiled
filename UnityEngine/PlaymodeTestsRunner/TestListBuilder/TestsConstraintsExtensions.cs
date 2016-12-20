namespace UnityEngine.PlaymodeTestsRunner.TestListBuilder
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.PlaymodeTestsRunner;

    [Extension]
    internal static class TestsConstraintsExtensions
    {
        [Extension]
        internal static TestsConstraint GetTestsConstraint(TestPlatform testPlatform)
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

