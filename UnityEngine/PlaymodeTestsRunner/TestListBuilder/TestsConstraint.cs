namespace UnityEngine.PlaymodeTestsRunner.TestListBuilder
{
    using System;
    using System.Reflection;

    internal abstract class TestsConstraint
    {
        protected TestsConstraint()
        {
        }

        public virtual bool IsClassATest(Type type) => 
            false;

        public virtual bool IsClassTestSupported() => 
            false;

        public virtual bool IsMethodATest(MethodInfo method) => 
            false;

        public virtual bool IsMethodTestSupported() => 
            false;
    }
}

