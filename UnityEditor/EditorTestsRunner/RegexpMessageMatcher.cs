namespace UnityEditor.EditorTestsRunner
{
    using NUnit.Framework;
    using System;

    internal class RegexpMessageMatcher : MessageMatcher
    {
        public override void Assert(string message)
        {
            NUnit.Framework.Assert.That(message, Is.StringMatching(base.pattern));
        }
    }
}

