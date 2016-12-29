namespace UnityEngine.UI
{
    using System;
    using UnityEngine;
    using UnityEngine.Serialization;

    /// <summary>
    /// <para>Structure to store the state of an animation transition on a Selectable.</para>
    /// </summary>
    [Serializable]
    public class AnimationTriggers
    {
        private const string kDefaultDisabledAnimName = "Disabled";
        private const string kDefaultNormalAnimName = "Normal";
        private const string kDefaultPressedAnimName = "Pressed";
        private const string kDefaultSelectedAnimName = "Highlighted";
        [FormerlySerializedAs("disabledTrigger"), SerializeField]
        private string m_DisabledTrigger = "Disabled";
        [FormerlySerializedAs("highlightedTrigger"), FormerlySerializedAs("m_SelectedTrigger"), SerializeField]
        private string m_HighlightedTrigger = "Highlighted";
        [FormerlySerializedAs("normalTrigger"), SerializeField]
        private string m_NormalTrigger = "Normal";
        [FormerlySerializedAs("pressedTrigger"), SerializeField]
        private string m_PressedTrigger = "Pressed";

        /// <summary>
        /// <para>Trigger to send to animator when entering disabled state.</para>
        /// </summary>
        public string disabledTrigger
        {
            get => 
                this.m_DisabledTrigger;
            set
            {
                this.m_DisabledTrigger = value;
            }
        }

        /// <summary>
        /// <para>Trigger to send to animator when entering highlighted state.</para>
        /// </summary>
        public string highlightedTrigger
        {
            get => 
                this.m_HighlightedTrigger;
            set
            {
                this.m_HighlightedTrigger = value;
            }
        }

        /// <summary>
        /// <para>Trigger to send to animator when entering normal state.</para>
        /// </summary>
        public string normalTrigger
        {
            get => 
                this.m_NormalTrigger;
            set
            {
                this.m_NormalTrigger = value;
            }
        }

        /// <summary>
        /// <para>Trigger to send to animator when entering pressed state.</para>
        /// </summary>
        public string pressedTrigger
        {
            get => 
                this.m_PressedTrigger;
            set
            {
                this.m_PressedTrigger = value;
            }
        }
    }
}

