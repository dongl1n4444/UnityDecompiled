namespace UnityEngine.TestTools.NUnitExtensions
{
    using NUnit.Framework.Internal;
    using System;
    using System.Collections;
    using System.Threading;
    using UnityEngine.TestTools.TestRunner;

    internal class TestDelegator
    {
        public static TestDelegator instance = new TestDelegator();
        private TestExecutionContext m_Context;
        private Exception m_Exception;
        private object m_Result;
        private ManualResetEvent m_Signal = new ManualResetEvent(false);
        private TestMethod m_TestMethod;

        public object DelegateTest(TestMethod testMethod, TestExecutionContext contex)
        {
            if (this.m_TestMethod != null)
            {
                throw new Exception("Action not executed yet");
            }
            this.m_Signal.Reset();
            this.m_TestMethod = testMethod;
            this.m_Context = contex;
            this.m_Signal.WaitOne();
            this.SetCurrentTestContext();
            this.m_TestMethod = null;
            if (this.m_Exception != null)
            {
                Exception exception = this.m_Exception;
                this.m_Exception = null;
                throw exception;
            }
            object result = this.m_Result;
            this.m_Result = null;
            return result;
        }

        public TestExecutionContext GetCurrentTestContext() => 
            this.m_Context;

        public IEnumerator GetTestEnumerator()
        {
            this.SetCurrentTestContext();
            return new TestEnumeratorWrapper(this.m_TestMethod).GetEnumerator(this.m_Context);
        }

        public bool HasTest() => 
            (this.m_TestMethod != null);

        public void RegisterResult(object result)
        {
            this.m_Result = result;
            this.m_Exception = null;
            this.m_Signal.Set();
        }

        public void RegisterResultException(Exception exception)
        {
            this.m_Result = null;
            this.m_Exception = exception;
            this.m_Signal.Set();
        }

        private void SetCurrentTestContext()
        {
            typeof(TestExecutionContext).GetProperty("CurrentContext").SetValue(null, this.GetCurrentTestContext(), null);
        }
    }
}

