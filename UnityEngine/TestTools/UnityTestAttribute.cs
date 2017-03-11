namespace UnityEngine.TestTools
{
    using NUnit.Framework;
    using NUnit.Framework.Interfaces;
    using NUnit.Framework.Internal;
    using NUnit.Framework.Internal.Builders;
    using NUnit.Framework.Internal.Commands;
    using System;
    using UnityEngine;
    using UnityEngine.TestTools.NUnitExtensions;

    /// <summary>
    /// <para>Special type of a unit test that allows to yield from test in order to skip frames when the test is running.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class UnityTestAttribute : CombiningStrategyAttribute, IWrapTestMethod, ISimpleTestBuilder, IImplyFixture, ICommandWrapper
    {
        private readonly NUnitTestCaseBuilder _builder;

        public UnityTestAttribute() : base((ICombiningStrategy) new UnityCombinatorialStrategy(), (IParameterDataProvider) new ParameterDataSourceProvider())
        {
            this._builder = new NUnitTestCaseBuilder();
        }

        internal static bool IsSupportedOnPlatform()
        {
            if (((Application.platform == RuntimePlatform.MetroPlayerARM) || (Application.platform == RuntimePlatform.MetroPlayerX64)) || ((Application.platform == RuntimePlatform.MetroPlayerX86) || (Application.platform == RuntimePlatform.WebGLPlayer)))
            {
                return false;
            }
            return true;
        }

        TestMethod ISimpleTestBuilder.BuildFrom(IMethodInfo method, Test suite)
        {
            TestCaseParameters parms = new TestCaseParameters {
                ExpectedResult = new object(),
                HasExpectedResult = true
            };
            TestMethod method2 = this._builder.BuildTestMethod(method, suite, parms);
            method2.parms.HasExpectedResult = false;
            if (!IsSupportedOnPlatform())
            {
                method2.RunState = RunState.Skipped;
                method2.Properties.Add("_SKIPREASON", "UnityTest not supported on " + Application.platform);
            }
            return method2;
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

