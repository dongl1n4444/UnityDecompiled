namespace UnityEngine.TestTools.NUnitExtensions
{
    using NUnit.Framework.Internal;
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using UnityEngine.TestTools.Logging;
    using UnityEngine.TestTools.TestRunner;

    internal class ActionDelegator
    {
        public static ActionDelegator instance = new ActionDelegator();
        private Func<object> m_Action;
        private TestExecutionContext m_Context;
        private Exception m_Exception;
        private object m_Result;
        private ManualResetEvent m_Signal = new ManualResetEvent(false);

        public object Delegate(Action action)
        {
            <Delegate>c__AnonStorey0 storey = new <Delegate>c__AnonStorey0 {
                action = action
            };
            return this.Delegate(new Func<object>(storey.<>m__0));
        }

        public object Delegate(Func<object> action)
        {
            if (this.m_Action != null)
            {
                throw new Exception("Action not executed yet");
            }
            this.m_Context = TestExecutionContext.CurrentContext;
            this.m_Signal.Reset();
            this.m_Action = action;
            this.m_Signal.WaitOne();
            this.SetCurrentTestContext();
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

        public void Execute(LogScope logScope)
        {
            try
            {
                this.SetCurrentTestContext();
                this.m_Result = this.m_Action();
                if (logScope.AnyFailingLogs())
                {
                    throw new UnhandledLogMessageException(logScope.FailingLogs.First<LogEvent>());
                }
                if (logScope.ExpectedLogs.Any<LogMatch>())
                {
                    throw new UnexpectedLogMessageException(LogScope.Current.ExpectedLogs.Peek());
                }
            }
            catch (Exception exception)
            {
                this.m_Exception = exception;
            }
            finally
            {
                this.m_Action = null;
                this.m_Signal.Set();
            }
        }

        public bool HasAction() => 
            (this.m_Action != null);

        private void SetCurrentTestContext()
        {
            typeof(TestExecutionContext).GetProperty("CurrentContext").SetValue(null, this.m_Context, null);
        }

        [CompilerGenerated]
        private sealed class <Delegate>c__AnonStorey0
        {
            internal Action action;

            internal object <>m__0()
            {
                this.action();
                return null;
            }
        }
    }
}

