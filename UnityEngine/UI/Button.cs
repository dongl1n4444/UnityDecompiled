namespace UnityEngine.UI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.Serialization;

    /// <summary>
    /// <para>A standard button that can be clicked in order to trigger an event.</para>
    /// </summary>
    [AddComponentMenu("UI/Button", 30)]
    public class Button : Selectable, IPointerClickHandler, ISubmitHandler, IEventSystemHandler
    {
        [FormerlySerializedAs("onClick"), SerializeField]
        private ButtonClickedEvent m_OnClick = new ButtonClickedEvent();

        protected Button()
        {
        }

        [DebuggerHidden]
        private IEnumerator OnFinishSubmit() => 
            new <OnFinishSubmit>c__Iterator0 { $this = this };

        /// <summary>
        /// <para>Registered IPointerClickHandler callback.</para>
        /// </summary>
        /// <param name="eventData">Data passed in (Typically by the event system).</param>
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                this.Press();
            }
        }

        /// <summary>
        /// <para>Registered ISubmitHandler callback.</para>
        /// </summary>
        /// <param name="eventData">Data passed in (Typically by the event system).</param>
        public virtual void OnSubmit(BaseEventData eventData)
        {
            this.Press();
            if (this.IsActive() && this.IsInteractable())
            {
                this.DoStateTransition(Selectable.SelectionState.Pressed, false);
                base.StartCoroutine(this.OnFinishSubmit());
            }
        }

        private void Press()
        {
            if (this.IsActive() && this.IsInteractable())
            {
                this.m_OnClick.Invoke();
            }
        }

        /// <summary>
        /// <para>UnityEvent to be fired when the buttons is pressed.</para>
        /// </summary>
        public ButtonClickedEvent onClick
        {
            get => 
                this.m_OnClick;
            set
            {
                this.m_OnClick = value;
            }
        }

        [CompilerGenerated]
        private sealed class <OnFinishSubmit>c__Iterator0 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal bool $disposing;
            internal int $PC;
            internal Button $this;
            internal float <elapsedTime>__1;
            internal float <fadeTime>__1;

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
                        this.<fadeTime>__1 = this.$this.colors.fadeDuration;
                        this.<elapsedTime>__1 = 0f;
                        break;

                    case 1:
                        break;

                    default:
                        goto Label_00A9;
                }
                if (this.<elapsedTime>__1 < this.<fadeTime>__1)
                {
                    this.<elapsedTime>__1 += Time.unscaledDeltaTime;
                    this.$current = null;
                    if (!this.$disposing)
                    {
                        this.$PC = 1;
                    }
                    return true;
                }
                this.$this.DoStateTransition(this.$this.currentSelectionState, false);
                this.$PC = -1;
            Label_00A9:
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

        /// <summary>
        /// <para>Function definition for a button click event.</para>
        /// </summary>
        [Serializable]
        public class ButtonClickedEvent : UnityEvent
        {
        }
    }
}

