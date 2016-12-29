namespace UnityEngine.PlaymodeTestsRunner.TestListBuilder
{
    using System;
    using System.Collections;
    using System.Reflection;

    internal class EditModeTestsConstraints : TestsConstraint
    {
        public override bool IsMethodATest(MethodInfo method)
        {
            Type returnType = method.ReturnType;
            if ((returnType != typeof(void)) && (returnType != typeof(IEnumerator)))
            {
                return false;
            }
            if (method.GetParameters().Length != 0)
            {
                return false;
            }
            return true;
        }

        public override bool IsMethodTestSupported() => 
            true;
    }
}

