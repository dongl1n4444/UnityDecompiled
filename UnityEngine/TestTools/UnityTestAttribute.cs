namespace UnityEngine.TestTools
{
    using NUnit.Framework;
    using NUnit.Framework.Interfaces;
    using NUnit.Framework.Internal;
    using NUnit.Framework.Internal.Builders;
    using NUnit.Framework.Internal.Commands;
    using System;
    using System.Collections;
    using UnityEngine.TestTools.NUnitExtensions;

    /// <summary>
    /// <para>Special type of a unit test that allows to yield from test in order to skip frames when the test is running.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class UnityTestAttribute : TestAttribute, IWrapTestMethod, ISimpleTestBuilder, ICommandWrapper
    {
        private readonly NUnitTestCaseBuilder _builder = new NUnitTestCaseBuilder();

        public TestMethod BuildFrom(IMethodInfo method, Test suite)
        {
            TestCaseParameters parms = null;
            if (method.ReturnType.IsType(typeof(IEnumerator)))
            {
                base.ExpectedResult = new object();
            }
            if (base.HasExpectedResult)
            {
                parms = new TestCaseParameters {
                    ExpectedResult = base.ExpectedResult
                };
            }
            return this._builder.BuildTestMethod(method, suite, parms);
        }

        public TestCommand Wrap(TestCommand command)
        {
            if (command is TestMethodCommand)
            {
                return new DelegatedTestMethodCommand((TestMethod) command.Test);
            }
            return null;
        }
    }
}

