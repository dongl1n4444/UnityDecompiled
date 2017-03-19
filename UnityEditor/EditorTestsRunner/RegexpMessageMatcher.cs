namespace UnityEditor.EditorTestsRunner
{
    using NUnit.Framework;
    using System;

    internal class RegexpMessageMatcher : MessageMatcher
    {
        public override void Assert(string message)
        {
            NUnit.Framework.Assert.That(message, NUnit.Framework.Is.StringMatching(base.pattern));
        }
    }
}

