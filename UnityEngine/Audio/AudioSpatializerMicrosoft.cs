namespace UnityEngine.Audio
{
    using System;
    using UnityEngine;

    /// <summary>
    /// <para>This component enables control over properties attached to AudioSource components for spatial sound in Unity.</para>
    /// </summary>
    [RequireComponent(typeof(AudioSource)), AddComponentMenu("Audio/Audio Spatializer/Audio Spatializer Microsoft", 30)]
    public sealed class AudioSpatializerMicrosoft : MonoBehaviour
    {
        [SerializeField]
        private RoomSize m_RoomSize = RoomSize.Small;

        private void Awake()
        {
            this.SetSpatializerFloats();
        }

        private void OnDidAnimateProperty()
        {
            this.SetSpatializerFloats();
        }

        private void OnValidate()
        {
            this.SetSpatializerFloats();
        }

        private void SetSpatializerFloats()
        {
            this.SetSpatializerRoomSize();
        }

        private void SetSpatializerRoomSize()
        {
            this.audioSource.SetSpatializerFloat(0, (float) this.m_RoomSize);
        }

        private AudioSource audioSource =>
            base.GetComponent<AudioSource>();

        /// <summary>
        /// <para>Describes room size to for audio effects.</para>
        /// </summary>
        public RoomSize roomSize
        {
            get => 
                this.m_RoomSize;
            set
            {
                this.m_RoomSize = value;
                this.SetSpatializerRoomSize();
            }
        }

        /// <summary>
        /// <para>Describes room size to for audio effects.</para>
        /// </summary>
        public enum RoomSize
        {
            Small,
            Medium,
            Large,
            Outdoors
        }
    }
}

