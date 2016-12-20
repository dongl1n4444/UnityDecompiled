namespace UnityEngine.Networking
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEngine;

    internal class ChannelBuffer : IDisposable
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int <lastBufferedPerSecond>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int <numBufferedMsgsOut>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int <numBufferedPerSecond>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int <numBytesIn>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int <numBytesOut>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int <numMsgsIn>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int <numMsgsOut>k__BackingField;
        internal NetBuffer fragmentBuffer = new NetBuffer();
        private const int k_MaxFreePacketCount = 0x200;
        private const int k_PacketHeaderReserveSize = 100;
        private bool m_AllowFragmentation;
        private byte m_ChannelId;
        private NetworkConnection m_Connection;
        private ChannelPacket m_CurrentPacket;
        private bool m_Disposed;
        private bool m_IsBroken;
        private bool m_IsReliable;
        private float m_LastBufferedMessageCountTimer = Time.realtimeSinceStartup;
        private float m_LastFlushTime;
        private int m_MaxPacketSize;
        private int m_MaxPendingPacketCount;
        private Queue<ChannelPacket> m_PendingPackets;
        public const int MaxBufferedPackets = 0x200;
        public float maxDelay = 0.01f;
        public const int MaxPendingPacketCount = 0x10;
        internal static int pendingPacketCount;
        private bool readingFragment = false;
        private static NetworkWriter s_FragmentWriter = new NetworkWriter();
        private static List<ChannelPacket> s_FreePackets;
        private static NetworkWriter s_SendWriter = new NetworkWriter();

        public ChannelBuffer(NetworkConnection conn, int bufferSize, byte cid, bool isReliable, bool isSequenced)
        {
            this.m_Connection = conn;
            this.m_MaxPacketSize = bufferSize - 100;
            this.m_CurrentPacket = new ChannelPacket(this.m_MaxPacketSize, isReliable);
            this.m_ChannelId = cid;
            this.m_MaxPendingPacketCount = 0x10;
            this.m_IsReliable = isReliable;
            this.m_AllowFragmentation = isReliable && isSequenced;
            if (isReliable)
            {
                this.m_PendingPackets = new Queue<ChannelPacket>();
                if (s_FreePackets == null)
                {
                    s_FreePackets = new List<ChannelPacket>();
                }
            }
        }

        private ChannelPacket AllocPacket()
        {
            NetworkDetailStats.SetStat(NetworkDetailStats.NetworkDirection.Outgoing, 0x1f, "msg", pendingPacketCount);
            if (s_FreePackets.Count == 0)
            {
                return new ChannelPacket(this.m_MaxPacketSize, this.m_IsReliable);
            }
            ChannelPacket packet2 = s_FreePackets[s_FreePackets.Count - 1];
            s_FreePackets.RemoveAt(s_FreePackets.Count - 1);
            packet2.Reset();
            return packet2;
        }

        public void CheckInternalBuffer()
        {
            if (((Time.realtimeSinceStartup - this.m_LastFlushTime) > this.maxDelay) && !this.m_CurrentPacket.IsEmpty())
            {
                this.SendInternalBuffer();
                this.m_LastFlushTime = Time.realtimeSinceStartup;
            }
            if ((Time.realtimeSinceStartup - this.m_LastBufferedMessageCountTimer) > 1f)
            {
                this.lastBufferedPerSecond = this.numBufferedPerSecond;
                this.numBufferedPerSecond = 0;
                this.m_LastBufferedMessageCountTimer = Time.realtimeSinceStartup;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if ((!this.m_Disposed && disposing) && (this.m_PendingPackets != null))
            {
                while (this.m_PendingPackets.Count > 0)
                {
                    pendingPacketCount--;
                    ChannelPacket item = this.m_PendingPackets.Dequeue();
                    if (s_FreePackets.Count < 0x200)
                    {
                        s_FreePackets.Add(item);
                    }
                }
                this.m_PendingPackets.Clear();
            }
            this.m_Disposed = true;
        }

        private static void FreePacket(ChannelPacket packet)
        {
            NetworkDetailStats.SetStat(NetworkDetailStats.NetworkDirection.Outgoing, 0x1f, "msg", pendingPacketCount);
            if (s_FreePackets.Count < 0x200)
            {
                s_FreePackets.Add(packet);
            }
        }

        internal bool HandleFragment(NetworkReader reader)
        {
            if (reader.ReadByte() == 0)
            {
                if (!this.readingFragment)
                {
                    this.fragmentBuffer.SeekZero();
                    this.readingFragment = true;
                }
                byte[] buffer = reader.ReadBytesAndSize();
                this.fragmentBuffer.WriteBytes(buffer, (ushort) buffer.Length);
                return false;
            }
            this.readingFragment = false;
            return true;
        }

        private void QueuePacket()
        {
            pendingPacketCount++;
            this.m_PendingPackets.Enqueue(this.m_CurrentPacket);
            this.m_CurrentPacket = this.AllocPacket();
        }

        public bool Send(short msgType, MessageBase msg)
        {
            s_SendWriter.StartMessage(msgType);
            msg.Serialize(s_SendWriter);
            s_SendWriter.FinishMessage();
            this.numMsgsOut++;
            return this.SendWriter(s_SendWriter);
        }

        internal bool SendBytes(byte[] bytes, int bytesToSend)
        {
            NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Outgoing, 0x1c, "msg", 1);
            if (bytesToSend >= 0xffff)
            {
                if (LogFilter.logError)
                {
                    UnityEngine.Debug.LogError("ChannelBuffer:SendBytes cannot send packet larger than " + ((ushort) 0xffff) + " bytes");
                }
                return false;
            }
            if (bytesToSend <= 0)
            {
                if (LogFilter.logError)
                {
                    UnityEngine.Debug.LogError("ChannelBuffer:SendBytes cannot send zero bytes");
                }
                return false;
            }
            if (bytesToSend > this.m_MaxPacketSize)
            {
                if (this.m_AllowFragmentation)
                {
                    return this.SendFragmentBytes(bytes, bytesToSend);
                }
                if (LogFilter.logError)
                {
                    UnityEngine.Debug.LogError(string.Concat(new object[] { "Failed to send big message of ", bytesToSend, " bytes. The maximum is ", this.m_MaxPacketSize, " bytes on channel:", this.m_ChannelId }));
                }
                return false;
            }
            if (!this.m_CurrentPacket.HasSpace(bytesToSend))
            {
                if (this.m_IsReliable)
                {
                    if (this.m_PendingPackets.Count == 0)
                    {
                        if (!this.m_CurrentPacket.SendToTransport(this.m_Connection, this.m_ChannelId))
                        {
                            this.QueuePacket();
                        }
                        this.m_CurrentPacket.Write(bytes, bytesToSend);
                        return true;
                    }
                    if (this.m_PendingPackets.Count >= this.m_MaxPendingPacketCount)
                    {
                        if (!this.m_IsBroken && LogFilter.logError)
                        {
                            UnityEngine.Debug.LogError("ChannelBuffer buffer limit of " + this.m_PendingPackets.Count + " packets reached.");
                        }
                        this.m_IsBroken = true;
                        return false;
                    }
                    this.QueuePacket();
                    this.m_CurrentPacket.Write(bytes, bytesToSend);
                    return true;
                }
                if (!this.m_CurrentPacket.SendToTransport(this.m_Connection, this.m_ChannelId))
                {
                    if (LogFilter.logError)
                    {
                        UnityEngine.Debug.Log("ChannelBuffer SendBytes no space on unreliable channel " + this.m_ChannelId);
                    }
                    return false;
                }
                this.m_CurrentPacket.Write(bytes, bytesToSend);
                return true;
            }
            this.m_CurrentPacket.Write(bytes, bytesToSend);
            if (this.maxDelay == 0f)
            {
                return this.SendInternalBuffer();
            }
            return true;
        }

        internal bool SendFragmentBytes(byte[] bytes, int bytesToSend)
        {
            int sourceIndex = 0;
            while (bytesToSend > 0)
            {
                int length = Math.Min(bytesToSend, this.m_MaxPacketSize - 0x20);
                byte[] destinationArray = new byte[length];
                Array.Copy(bytes, sourceIndex, destinationArray, 0, length);
                s_FragmentWriter.StartMessage(0x11);
                s_FragmentWriter.Write((byte) 0);
                s_FragmentWriter.WriteBytesFull(destinationArray);
                s_FragmentWriter.FinishMessage();
                this.SendWriter(s_FragmentWriter);
                sourceIndex += length;
                bytesToSend -= length;
            }
            s_FragmentWriter.StartMessage(0x11);
            s_FragmentWriter.Write((byte) 1);
            s_FragmentWriter.FinishMessage();
            this.SendWriter(s_FragmentWriter);
            return true;
        }

        public bool SendInternalBuffer()
        {
            NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Outgoing, 0x1d, "msg", 1);
            if (!this.m_IsReliable || (this.m_PendingPackets.Count <= 0))
            {
                return this.m_CurrentPacket.SendToTransport(this.m_Connection, this.m_ChannelId);
            }
            while (this.m_PendingPackets.Count > 0)
            {
                ChannelPacket item = this.m_PendingPackets.Dequeue();
                if (!item.SendToTransport(this.m_Connection, this.m_ChannelId))
                {
                    this.m_PendingPackets.Enqueue(item);
                    break;
                }
                pendingPacketCount--;
                FreePacket(item);
                if (this.m_IsBroken && (this.m_PendingPackets.Count < (this.m_MaxPendingPacketCount / 2)))
                {
                    if (LogFilter.logWarn)
                    {
                        UnityEngine.Debug.LogWarning("ChannelBuffer recovered from overflow but data was lost.");
                    }
                    this.m_IsBroken = false;
                }
            }
            return true;
        }

        public bool SendWriter(NetworkWriter writer)
        {
            return this.SendBytes(writer.AsArraySegment().Array, writer.AsArraySegment().Count);
        }

        public bool SetOption(ChannelOption option, int value)
        {
            if (option != ChannelOption.MaxPendingBuffers)
            {
                if (option != ChannelOption.AllowFragmentation)
                {
                    if (option != ChannelOption.MaxPacketSize)
                    {
                        return false;
                    }
                    if (!this.m_CurrentPacket.IsEmpty() || (this.m_PendingPackets.Count > 0))
                    {
                        if (LogFilter.logError)
                        {
                            UnityEngine.Debug.LogError("Cannot set MaxPacketSize after sending data.");
                        }
                        return false;
                    }
                    if (value <= 0)
                    {
                        if (LogFilter.logError)
                        {
                            UnityEngine.Debug.LogError("Cannot set MaxPacketSize less than one.");
                        }
                        return false;
                    }
                    if (value > this.m_MaxPacketSize)
                    {
                        if (LogFilter.logError)
                        {
                            UnityEngine.Debug.LogError("Cannot set MaxPacketSize to greater than the existing maximum (" + this.m_MaxPacketSize + ").");
                        }
                        return false;
                    }
                    this.m_CurrentPacket = new ChannelPacket(value, this.m_IsReliable);
                    this.m_MaxPacketSize = value;
                    return true;
                }
            }
            else
            {
                if (!this.m_IsReliable)
                {
                    return false;
                }
                if ((value < 0) || (value >= 0x200))
                {
                    if (LogFilter.logError)
                    {
                        UnityEngine.Debug.LogError(string.Concat(new object[] { "Invalid MaxPendingBuffers for channel ", this.m_ChannelId, ". Must be greater than zero and less than ", 0x200 }));
                    }
                    return false;
                }
                this.m_MaxPendingPacketCount = value;
                return true;
            }
            this.m_AllowFragmentation = value != 0;
            return true;
        }

        public int lastBufferedPerSecond { get; private set; }

        public int numBufferedMsgsOut { get; private set; }

        public int numBufferedPerSecond { get; private set; }

        public int numBytesIn { get; private set; }

        public int numBytesOut { get; private set; }

        public int numMsgsIn { get; private set; }

        public int numMsgsOut { get; private set; }
    }
}

