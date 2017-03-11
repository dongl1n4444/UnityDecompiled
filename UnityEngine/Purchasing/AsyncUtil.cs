namespace UnityEngine.Purchasing
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    [AddComponentMenu("")]
    internal class AsyncUtil : MonoBehaviour, IAsyncUtil
    {
        [DebuggerHidden]
        private IEnumerator DoInvoke(Action a, int delayInSeconds) => 
            new <DoInvoke>c__Iterator0 { 
                delayInSeconds = delayInSeconds,
                a = a
            };

        public void Get(string url, Action<string> responseHandler, Action<string> errorHandler)
        {
            WWW request = new WWW(url);
            base.StartCoroutine(this.Process(request, responseHandler, errorHandler));
        }

        [DebuggerHidden]
        private IEnumerator Process(WWW request, Action<string> responseHandler, Action<string> errorHandler) => 
            new <Process>c__Iterator1 { 
                request = request,
                errorHandler = errorHandler,
                responseHandler = responseHandler
            };

        public void Schedule(Action a, int delayInSeconds)
        {
            base.StartCoroutine(this.DoInvoke(a, delayInSeconds));
        }

        [CompilerGenerated]
        private sealed class <DoInvoke>c__Iterator0 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal bool $disposing;
            internal int $PC;
            internal Action a;
            internal int delayInSeconds;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$disposing = true;
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.$current = new WaitForSeconds((float) this.delayInSeconds);
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        return true;

                    case 1:
                        this.a();
                        this.$PC = -1;
                        break;
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

        [CompilerGenerated]
        private sealed class <Process>c__Iterator1 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal bool $disposing;
            internal int $PC;
            internal Action<string> errorHandler;
            internal WWW request;
            internal Action<string> responseHandler;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$disposing = true;
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.$current = this.request;
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        return true;

                    case 1:
                        if (this.request.error == null)
                        {
                            this.responseHandler(this.request.text);
                            break;
                        }
                        this.errorHandler(this.request.error);
                        break;

                    default:
                        goto Label_008E;
                }
                this.$PC = -1;
            Label_008E:
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

