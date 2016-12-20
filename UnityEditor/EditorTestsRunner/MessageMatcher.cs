namespace UnityEditor.EditorTestsRunner
{
    using System;

    internal abstract class MessageMatcher
    {
        public string pattern;

        protected MessageMatcher()
        {
        }

        public virtual void Assert(string message)
        {
            throw new NotImplementedException();
        }
    }
}

