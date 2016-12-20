namespace UnityEngine.UI.CoroutineTween
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class TweenRunner<T> where T: struct, ITweenValue
    {
        protected MonoBehaviour m_CoroutineContainer;
        protected IEnumerator m_Tween;

        public void Init(MonoBehaviour coroutineContainer)
        {
            this.m_CoroutineContainer = coroutineContainer;
        }

        [DebuggerHidden]
        private static IEnumerator Start(T tweenInfo)
        {
            return new <Start>c__Iterator0<T> { tweenInfo = tweenInfo };
        }

        public void StartTween(T info)
        {
            if (this.m_CoroutineContainer == null)
            {
                UnityEngine.Debug.LogWarning("Coroutine container not configured... did you forget to call Init?");
            }
            else
            {
                this.StopTween();
                if (!this.m_CoroutineContainer.gameObject.activeInHierarchy)
                {
                    info.TweenValue(1f);
                }
                else
                {
                    this.m_Tween = TweenRunner<T>.Start(info);
                    this.m_CoroutineContainer.StartCoroutine(this.m_Tween);
                }
            }
        }

        public void StopTween()
        {
            if (this.m_Tween != null)
            {
                this.m_CoroutineContainer.StopCoroutine(this.m_Tween);
                this.m_Tween = null;
            }
        }

        [CompilerGenerated]
        private sealed class <Start>c__Iterator0 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal bool $disposing;
            internal int $PC;
            internal float <elapsedTime>__0;
            internal float <percentage>__1;
            internal T tweenInfo;

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
                        if (this.tweenInfo.ValidTarget())
                        {
                            this.<elapsedTime>__0 = 0f;
                            while (this.<elapsedTime>__0 < this.tweenInfo.duration)
                            {
                                this.<elapsedTime>__0 += !this.tweenInfo.ignoreTimeScale ? Time.deltaTime : Time.unscaledDeltaTime;
                                this.<percentage>__1 = Mathf.Clamp01(this.<elapsedTime>__0 / this.tweenInfo.duration);
                                this.tweenInfo.TweenValue(this.<percentage>__1);
                                this.$current = null;
                                if (!this.$disposing)
                                {
                                    this.$PC = 1;
                                }
                                return true;
                            Label_00D5:;
                            }
                            this.tweenInfo.TweenValue(1f);
                            this.$PC = -1;
                            break;
                        }
                        break;

                    case 1:
                        goto Label_00D5;
                }
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }
    }
}

