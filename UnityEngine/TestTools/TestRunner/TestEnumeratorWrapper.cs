namespace UnityEngine.TestTools.TestRunner
{
    using NUnit.Framework.Internal;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class TestEnumeratorWrapper
    {
        private TestMethod m_TestMethod;

        public TestEnumeratorWrapper(TestMethod testMethod)
        {
            this.m_TestMethod = testMethod;
        }

        public IEnumerator GetEnumerator(TestExecutionContext context)
        {
            if (this.m_TestMethod.Method.ReturnType.Type == typeof(void))
            {
                return this.HandleVoidTest(context);
            }
            if (this.m_TestMethod.Method.ReturnType.Type == typeof(IEnumerator))
            {
                return this.HandleEnumerableTest(context);
            }
            object[] args = new object[] { this.m_TestMethod.Method.ReturnType, this.m_TestMethod.Method.Name, this.m_TestMethod.Method.TypeInfo.FullName };
            UnityEngine.Debug.LogWarningFormat("Return type {0} of {1} in {2} is not supported.", args);
            return null;
        }

        private IEnumerator HandleEnumerableTest(TestExecutionContext context) => 
            (this.m_TestMethod.Method.MethodInfo.Invoke(context.TestObject, this.m_TestMethod.parms?.OriginalArguments) as IEnumerator);

        [DebuggerHidden]
        private IEnumerator HandleVoidTest(TestExecutionContext context) => 
            new <HandleVoidTest>c__Iterator0 { 
                context = context,
                $this = this
            };

        [CompilerGenerated]
        private sealed class <HandleVoidTest>c__Iterator0 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal bool $disposing;
            internal int $PC;
            internal TestEnumeratorWrapper $this;
            internal TestExecutionContext context;

            [DebuggerHidden]
            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                this.$PC = -1;
                if (this.$PC == 0)
                {
                    this.$this.m_TestMethod.Method.MethodInfo.Invoke(this.context.TestObject, this.$this.m_TestMethod.parms?.OriginalArguments);
                }
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current =>
                this.$current;

            object IEnumerator.Current =>
                this.$current;
        }
    }
}

