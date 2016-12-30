namespace UnityEngine.TestTools.NUnitExtensions
{
    using NUnit.Framework.Interfaces;
    using NUnit.Framework.Internal;
    using NUnit.Framework.Internal.Commands;
    using System;

    internal class DelegatedTestMethodCommand : TestCommand
    {
        private readonly TestMethod testMethod;

        public DelegatedTestMethodCommand(TestMethod testMethod) : base(testMethod)
        {
            this.testMethod = testMethod;
        }

        public override TestResult Execute(TestExecutionContext context)
        {
            this.RunTestMethod(context);
            context.CurrentResult.SetResult(ResultState.Success);
            return context.CurrentResult;
        }

        private object RunTestMethod(TestExecutionContext context) => 
            TestDelegator.instance.DelegateTest(this.testMethod, context);
    }
}

