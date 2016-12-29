namespace UnityEngine.PlaymodeTestsRunner.TestListBuilder
{
    using System;
    using System.Collections;
    using System.Reflection;
    using UnityEngine;

    internal class PlayModeTestsConstraints : TestsConstraint
    {
        public override bool IsClassATest(System.Type type) => 
            type.IsSubclassOf(typeof(MonoBehaviour));

        public override bool IsClassTestSupported() => 
            true;

        public override bool IsMethodATest(MethodInfo method)
        {
            System.Type returnType = method.ReturnType;
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

