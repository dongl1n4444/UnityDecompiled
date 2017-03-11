namespace UnityEngine.TestTools.NUnitExtensions
{
    using NUnit.Framework.Interfaces;
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.TestTools.TestRunner;

    internal class TestListenerWrapper : ITestListener
    {
        private bool m_DelegateCalls;
        private TestFinishedEvent m_TestFinishedEvent;
        private TestStartedEvent m_TestStartedEvent;

        public TestListenerWrapper(TestStartedEvent testStartedEvent, TestFinishedEvent testFinishedEvent, bool delegateCalls)
        {
            this.m_TestStartedEvent = testStartedEvent;
            this.m_TestFinishedEvent = testFinishedEvent;
            this.m_DelegateCalls = delegateCalls;
        }

        public void TestFinished(ITestResult result)
        {
            <TestFinished>c__AnonStorey1 storey = new <TestFinished>c__AnonStorey1 {
                result = result,
                $this = this
            };
            if (this.m_DelegateCalls)
            {
                ActionDelegator.instance.Delegate(new Action(storey.<>m__0));
            }
            else
            {
                this.m_TestFinishedEvent.Invoke(storey.result);
            }
        }

        public void TestOutput(NUnit.Framework.Interfaces.TestOutput output)
        {
        }

        public void TestStarted(ITest test)
        {
            <TestStarted>c__AnonStorey0 storey = new <TestStarted>c__AnonStorey0 {
                test = test,
                $this = this
            };
            if (this.m_DelegateCalls)
            {
                ActionDelegator.instance.Delegate(new Action(storey.<>m__0));
            }
            else
            {
                this.m_TestStartedEvent.Invoke(storey.test);
            }
        }

        [CompilerGenerated]
        private sealed class <TestFinished>c__AnonStorey1
        {
            internal TestListenerWrapper $this;
            internal ITestResult result;

            internal void <>m__0()
            {
                this.$this.m_TestFinishedEvent.Invoke(this.result);
            }
        }

        [CompilerGenerated]
        private sealed class <TestStarted>c__AnonStorey0
        {
            internal TestListenerWrapper $this;
            internal ITest test;

            internal void <>m__0()
            {
                this.$this.m_TestStartedEvent.Invoke(this.test);
            }
        }
    }
}

