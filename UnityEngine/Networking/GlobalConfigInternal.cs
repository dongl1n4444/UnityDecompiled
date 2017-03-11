namespace UnityEngine.Networking
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    internal sealed class GlobalConfigInternal : IDisposable
    {
        internal IntPtr m_Ptr;

        public GlobalConfigInternal(GlobalConfig config)
        {
            this.InitWrapper();
            this.InitThreadAwakeTimeout(config.ThreadAwakeTimeout);
            this.InitReactorModel((byte) config.ReactorModel);
            this.InitReactorMaximumReceivedMessages(config.ReactorMaximumReceivedMessages);
            this.InitReactorMaximumSentMessages(config.ReactorMaximumSentMessages);
            this.InitMaxPacketSize(config.MaxPacketSize);
            this.InitMaxHosts(config.MaxHosts);
            if ((config.ThreadPoolSize == 0) || (config.ThreadPoolSize > 0xfe))
            {
                throw new ArgumentOutOfRangeException("Worker thread pool size should be >= 1 && < 254 (for server only)");
            }
            this.InitThreadPoolSize(config.ThreadPoolSize);
            this.InitMinTimerTimeout(config.MinTimerTimeout);
            this.InitMaxTimerTimeout(config.MaxTimerTimeout);
            this.InitMinNetSimulatorTimeout(config.MinNetSimulatorTimeout);
            this.InitMaxNetSimulatorTimeout(config.MaxNetSimulatorTimeout);
        }

        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe, GeneratedByOldBindingsGenerator]
        public extern void Dispose();
        ~GlobalConfigInternal()
        {
            this.Dispose();
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void InitMaxHosts(ushort size);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void InitMaxNetSimulatorTimeout(uint ms);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void InitMaxPacketSize(ushort size);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void InitMaxTimerTimeout(uint ms);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void InitMinNetSimulatorTimeout(uint ms);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void InitMinTimerTimeout(uint ms);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void InitReactorMaximumReceivedMessages(ushort size);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void InitReactorMaximumSentMessages(ushort size);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void InitReactorModel(byte model);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void InitThreadAwakeTimeout(uint ms);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void InitThreadPoolSize(byte size);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void InitWrapper();
    }
}

