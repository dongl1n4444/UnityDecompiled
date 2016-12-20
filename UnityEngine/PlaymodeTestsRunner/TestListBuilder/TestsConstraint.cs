namespace UnityEngine.PlaymodeTestsRunner.TestListBuilder
{
    using System;
    using System.Reflection;

    internal abstract class TestsConstraint
    {
        protected TestsConstraint()
        {
        }

        public virtual bool IsClassATest(Type type)
        {
            return false;
        }

        public virtual bool IsClassTestSupported()
        {
            return false;
        }

        public virtual bool IsMethodATest(MethodInfo method)
        {
            return false;
        }

        public virtual bool IsMethodTestSupported()
        {
            return false;
        }
    }
}

