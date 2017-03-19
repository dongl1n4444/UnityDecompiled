namespace UnityEditor.EditorTestsRunner
{
    using NUnit.Framework;
    using System;

    internal class SimpleMessageMatcher : MessageMatcher
    {
        public override void Assert(string message)
        {
            NUnit.Framework.Assert.That(message, NUnit.Framework.Is.EqualTo(base.pattern));
        }
    }
}

