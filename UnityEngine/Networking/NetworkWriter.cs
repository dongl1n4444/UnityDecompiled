namespace UnityEngine.Networking
{
    using System;
    using System.Text;
    using UnityEngine;

    /// <summary>
    /// <para>General purpose serializer for UNET (for serializing data to byte arrays).</para>
    /// </summary>
    public class NetworkWriter
    {
        private const int k_MaxStringLength = 0x8000;
        private NetBuffer m_Buffer;
        private static Encoding s_Encoding;
        private static UIntFloat s_FloatConverter;
        private static byte[] s_StringWriteBuffer;

        /// <summary>
        /// <para>Creates a new NetworkWriter object.</para>
        /// </summary>
        /// <param name="buffer">A buffer to write into. This is not copied.</param>
        public NetworkWriter()
        {
            this.m_Buffer = new NetBuffer();
            if (s_Encoding == null)
            {
                s_Encoding = new UTF8Encoding();
                s_StringWriteBuffer = new byte[0x8000];
            }
        }

        /// <summary>
        /// <para>Creates a new NetworkWriter object.</para>
        /// </summary>
        /// <param name="buffer">A buffer to write into. This is not copied.</param>
        public NetworkWriter(byte[] buffer)
        {
            this.m_Buffer = new NetBuffer(buffer);
            if (s_Encoding == null)
            {
                s_Encoding = new UTF8Encoding();
                s_StringWriteBuffer = new byte[0x8000];
            }
        }

        /// <summary>
        /// <para>Returns the internal array of bytes the writer is using. This is NOT a copy.</para>
        /// </summary>
        /// <returns>
        /// <para>Internal buffer.</para>
        /// </returns>
        public byte[] AsArray() => 
            this.AsArraySegment().Array;

        internal ArraySegment<byte> AsArraySegment() => 
            this.m_Buffer.AsArraySegment();

        /// <summary>
        /// <para>This fills out the size header of a message begun with StartMessage(), so that it can be send using Send() functions.</para>
        /// </summary>
        public void FinishMessage()
        {
            this.m_Buffer.FinishMessage();
        }

        /// <summary>
        /// <para>Seeks to the start of the internal buffer.</para>
        /// </summary>
        public void SeekZero()
        {
            this.m_Buffer.SeekZero();
        }

        /// <summary>
        /// <para>This begins a new message, which should be completed with FinishMessage() once the payload has been written.</para>
        /// </summary>
        /// <param name="msgType">Message type.</param>
        public void StartMessage(short msgType)
        {
            this.SeekZero();
            this.m_Buffer.WriteByte2(0, 0);
            this.Write(msgType);
        }

        /// <summary>
        /// <para>Returns a copy of internal array of bytes the writer is using, it copies only the bytes used.</para>
        /// </summary>
        /// <returns>
        /// <para>Copy of data used by the writer.</para>
        /// </returns>
        public byte[] ToArray()
        {
            byte[] destinationArray = new byte[this.m_Buffer.AsArraySegment().Count];
            Array.Copy(this.m_Buffer.AsArraySegment().Array, destinationArray, this.m_Buffer.AsArraySegment().Count);
            return destinationArray;
        }

        public void Write(bool value)
        {
            if (value)
            {
                this.m_Buffer.WriteByte(1);
            }
            else
            {
                this.m_Buffer.WriteByte(0);
            }
        }

        public void Write(byte value)
        {
            this.m_Buffer.WriteByte(value);
        }

        public void Write(char value)
        {
            this.m_Buffer.WriteByte((byte) value);
        }

        public void Write(decimal value)
        {
            int[] bits = decimal.GetBits(value);
            this.Write(bits[0]);
            this.Write(bits[1]);
            this.Write(bits[2]);
            this.Write(bits[3]);
        }

        public void Write(double value)
        {
            s_FloatConverter.doubleValue = value;
            this.Write(s_FloatConverter.longValue);
        }

        public void Write(short value)
        {
            this.m_Buffer.WriteByte2((byte) (value & 0xff), (byte) ((value >> 8) & 0xff));
        }

        public void Write(int value)
        {
            this.m_Buffer.WriteByte4((byte) (value & 0xff), (byte) ((value >> 8) & 0xff), (byte) ((value >> 0x10) & 0xff), (byte) ((value >> 0x18) & 0xff));
        }

        public void Write(long value)
        {
            this.m_Buffer.WriteByte8((byte) (value & 0xffL), (byte) ((value >> 8) & 0xffL), (byte) ((value >> 0x10) & 0xffL), (byte) ((value >> 0x18) & 0xffL), (byte) ((value >> 0x20) & 0xffL), (byte) ((value >> 40) & 0xffL), (byte) ((value >> 0x30) & 0xffL), (byte) ((value >> 0x38) & 0xffL));
        }

        public void Write(sbyte value)
        {
            this.m_Buffer.WriteByte((byte) value);
        }

        public void Write(float value)
        {
            s_FloatConverter.floatValue = value;
            this.Write(s_FloatConverter.intValue);
        }

        public void Write(string value)
        {
            if (value == null)
            {
                this.m_Buffer.WriteByte2(0, 0);
            }
            else
            {
                int byteCount = s_Encoding.GetByteCount(value);
                if (byteCount >= 0x8000)
                {
                    throw new IndexOutOfRangeException("Serialize(string) too long: " + value.Length);
                }
                this.Write((ushort) byteCount);
                int num2 = s_Encoding.GetBytes(value, 0, value.Length, s_StringWriteBuffer, 0);
                this.m_Buffer.WriteBytes(s_StringWriteBuffer, (ushort) num2);
            }
        }

        public void Write(ushort value)
        {
            this.m_Buffer.WriteByte2((byte) (value & 0xff), (byte) ((value >> 8) & 0xff));
        }

        public void Write(uint value)
        {
            this.m_Buffer.WriteByte4((byte) (value & 0xff), (byte) ((value >> 8) & 0xff), (byte) ((value >> 0x10) & 0xff), (byte) ((value >> 0x18) & 0xff));
        }

        public void Write(ulong value)
        {
            this.m_Buffer.WriteByte8((byte) (value & ((ulong) 0xffL)), (byte) ((value >> 8) & ((ulong) 0xffL)), (byte) ((value >> 0x10) & ((ulong) 0xffL)), (byte) ((value >> 0x18) & ((ulong) 0xffL)), (byte) ((value >> 0x20) & ((ulong) 0xffL)), (byte) ((value >> 40) & ((ulong) 0xffL)), (byte) ((value >> 0x30) & ((ulong) 0xffL)), (byte) ((value >> 0x38) & ((ulong) 0xffL)));
        }

        public void Write(Color value)
        {
            this.Write(value.r);
            this.Write(value.g);
            this.Write(value.b);
            this.Write(value.a);
        }

        public void Write(Color32 value)
        {
            this.Write(value.r);
            this.Write(value.g);
            this.Write(value.b);
            this.Write(value.a);
        }

        /// <summary>
        /// <para>This writes a reference to a GameObject with a NetworkIdentity component to the stream. The object should have been spawned for clients to know about it.</para>
        /// </summary>
        /// <param name="value">The object to write.</param>
        public void Write(GameObject value)
        {
            if (value == null)
            {
                this.WritePackedUInt32(0);
            }
            else
            {
                NetworkIdentity component = value.GetComponent<NetworkIdentity>();
                if (component != null)
                {
                    this.Write(component.netId);
                }
                else
                {
                    if (LogFilter.logWarn)
                    {
                        Debug.LogWarning("NetworkWriter " + value + " has no NetworkIdentity");
                    }
                    this.WritePackedUInt32(0);
                }
            }
        }

        public void Write(Matrix4x4 value)
        {
            this.Write(value.m00);
            this.Write(value.m01);
            this.Write(value.m02);
            this.Write(value.m03);
            this.Write(value.m10);
            this.Write(value.m11);
            this.Write(value.m12);
            this.Write(value.m13);
            this.Write(value.m20);
            this.Write(value.m21);
            this.Write(value.m22);
            this.Write(value.m23);
            this.Write(value.m30);
            this.Write(value.m31);
            this.Write(value.m32);
            this.Write(value.m33);
        }

        public void Write(MessageBase msg)
        {
            msg.Serialize(this);
        }

        public void Write(NetworkHash128 value)
        {
            this.Write(value.i0);
            this.Write(value.i1);
            this.Write(value.i2);
            this.Write(value.i3);
            this.Write(value.i4);
            this.Write(value.i5);
            this.Write(value.i6);
            this.Write(value.i7);
            this.Write(value.i8);
            this.Write(value.i9);
            this.Write(value.i10);
            this.Write(value.i11);
            this.Write(value.i12);
            this.Write(value.i13);
            this.Write(value.i14);
            this.Write(value.i15);
        }

        public void Write(NetworkIdentity value)
        {
            if (value == null)
            {
                this.WritePackedUInt32(0);
            }
            else
            {
                this.Write(value.netId);
            }
        }

        public void Write(NetworkInstanceId value)
        {
            this.WritePackedUInt32(value.Value);
        }

        public void Write(NetworkSceneId value)
        {
            this.WritePackedUInt32(value.Value);
        }

        public void Write(Plane value)
        {
            this.Write(value.normal);
            this.Write(value.distance);
        }

        public void Write(Quaternion value)
        {
            this.Write(value.x);
            this.Write(value.y);
            this.Write(value.z);
            this.Write(value.w);
        }

        public void Write(Ray value)
        {
            this.Write(value.direction);
            this.Write(value.origin);
        }

        public void Write(Rect value)
        {
            this.Write(value.xMin);
            this.Write(value.yMin);
            this.Write(value.width);
            this.Write(value.height);
        }

        public void Write(Transform value)
        {
            if ((value == null) || (value.gameObject == null))
            {
                this.WritePackedUInt32(0);
            }
            else
            {
                NetworkIdentity component = value.gameObject.GetComponent<NetworkIdentity>();
                if (component != null)
                {
                    this.Write(component.netId);
                }
                else
                {
                    if (LogFilter.logWarn)
                    {
                        Debug.LogWarning("NetworkWriter " + value + " has no NetworkIdentity");
                    }
                    this.WritePackedUInt32(0);
                }
            }
        }

        public void Write(Vector2 value)
        {
            this.Write(value.x);
            this.Write(value.y);
        }

        public void Write(Vector3 value)
        {
            this.Write(value.x);
            this.Write(value.y);
            this.Write(value.z);
        }

        public void Write(Vector4 value)
        {
            this.Write(value.x);
            this.Write(value.y);
            this.Write(value.z);
            this.Write(value.w);
        }

        public void Write(byte[] buffer, int count)
        {
            if (count > 0xffff)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("NetworkWriter Write: buffer is too large (" + count + ") bytes. The maximum buffer size is 64K bytes.");
                }
            }
            else
            {
                this.m_Buffer.WriteBytes(buffer, (ushort) count);
            }
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            if (count > 0xffff)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("NetworkWriter Write: buffer is too large (" + count + ") bytes. The maximum buffer size is 64K bytes.");
                }
            }
            else
            {
                this.m_Buffer.WriteBytesAtOffset(buffer, (ushort) offset, (ushort) count);
            }
        }

        /// <summary>
        /// <para>This writes a 16-bit count and a array of bytes of that length to the stream.</para>
        /// </summary>
        /// <param name="buffer">Array of bytes to write.</param>
        /// <param name="count">Number of bytes from the array to write.</param>
        public void WriteBytesAndSize(byte[] buffer, int count)
        {
            if ((buffer == null) || (count == 0))
            {
                this.Write((ushort) 0);
            }
            else if (count > 0xffff)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("NetworkWriter WriteBytesAndSize: buffer is too large (" + count + ") bytes. The maximum buffer size is 64K bytes.");
                }
            }
            else
            {
                this.Write((ushort) count);
                this.m_Buffer.WriteBytes(buffer, (ushort) count);
            }
        }

        /// <summary>
        /// <para>This writes a 16-bit count and an array of bytes of that size to the stream.</para>
        /// </summary>
        /// <param name="buffer">Bytes to write.</param>
        public void WriteBytesFull(byte[] buffer)
        {
            if (buffer == null)
            {
                this.Write((ushort) 0);
            }
            else if (buffer.Length > 0xffff)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError("NetworkWriter WriteBytes: buffer is too large (" + buffer.Length + ") bytes. The maximum buffer size is 64K bytes.");
                }
            }
            else
            {
                this.Write((ushort) buffer.Length);
                this.m_Buffer.WriteBytes(buffer, (ushort) buffer.Length);
            }
        }

        /// <summary>
        /// <para>This writes the 32-bit value to the stream using variable-length-encoding.</para>
        /// </summary>
        /// <param name="value">Value to write.</param>
        public void WritePackedUInt32(uint value)
        {
            if (value <= 240)
            {
                this.Write((byte) value);
            }
            else if (value <= 0x8ef)
            {
                this.Write((byte) (((value - 240) / 0x100) + 0xf1));
                this.Write((byte) ((value - 240) % 0x100));
            }
            else if (value <= 0x108ef)
            {
                this.Write((byte) 0xf9);
                this.Write((byte) ((value - 0x8f0) / 0x100));
                this.Write((byte) ((value - 0x8f0) % 0x100));
            }
            else if (value <= 0xffffff)
            {
                this.Write((byte) 250);
                this.Write((byte) (value & 0xff));
                this.Write((byte) ((value >> 8) & 0xff));
                this.Write((byte) ((value >> 0x10) & 0xff));
            }
            else
            {
                this.Write((byte) 0xfb);
                this.Write((byte) (value & 0xff));
                this.Write((byte) ((value >> 8) & 0xff));
                this.Write((byte) ((value >> 0x10) & 0xff));
                this.Write((byte) ((value >> 0x18) & 0xff));
            }
        }

        /// <summary>
        /// <para>This writes the 64-bit value to the stream using variable-length-encoding.</para>
        /// </summary>
        /// <param name="value">Value to write.</param>
        public void WritePackedUInt64(ulong value)
        {
            if (value <= 240L)
            {
                this.Write((byte) value);
            }
            else if (value <= 0x8efL)
            {
                this.Write((byte) (((value - ((ulong) 240L)) / ((ulong) 0x100L)) + ((ulong) 0xf1L)));
                this.Write((byte) ((value - ((ulong) 240L)) % ((ulong) 0x100L)));
            }
            else if (value <= 0x108efL)
            {
                this.Write((byte) 0xf9);
                this.Write((byte) ((value - ((ulong) 0x8f0L)) / ((ulong) 0x100L)));
                this.Write((byte) ((value - ((ulong) 0x8f0L)) % ((ulong) 0x100L)));
            }
            else if (value <= 0xffffffL)
            {
                this.Write((byte) 250);
                this.Write((byte) (value & ((ulong) 0xffL)));
                this.Write((byte) ((value >> 8) & ((ulong) 0xffL)));
                this.Write((byte) ((value >> 0x10) & ((ulong) 0xffL)));
            }
            else if (value <= 0xffffffffL)
            {
                this.Write((byte) 0xfb);
                this.Write((byte) (value & ((ulong) 0xffL)));
                this.Write((byte) ((value >> 8) & ((ulong) 0xffL)));
                this.Write((byte) ((value >> 0x10) & ((ulong) 0xffL)));
                this.Write((byte) ((value >> 0x18) & ((ulong) 0xffL)));
            }
            else if (value <= 0xffffffffffL)
            {
                this.Write((byte) 0xfc);
                this.Write((byte) (value & ((ulong) 0xffL)));
                this.Write((byte) ((value >> 8) & ((ulong) 0xffL)));
                this.Write((byte) ((value >> 0x10) & ((ulong) 0xffL)));
                this.Write((byte) ((value >> 0x18) & ((ulong) 0xffL)));
                this.Write((byte) ((value >> 0x20) & ((ulong) 0xffL)));
            }
            else if (value <= 0xffffffffffffL)
            {
                this.Write((byte) 0xfd);
                this.Write((byte) (value & ((ulong) 0xffL)));
                this.Write((byte) ((value >> 8) & ((ulong) 0xffL)));
                this.Write((byte) ((value >> 0x10) & ((ulong) 0xffL)));
                this.Write((byte) ((value >> 0x18) & ((ulong) 0xffL)));
                this.Write((byte) ((value >> 0x20) & ((ulong) 0xffL)));
                this.Write((byte) ((value >> 40) & ((ulong) 0xffL)));
            }
            else if (value <= 0xffffffffffffffL)
            {
                this.Write((byte) 0xfe);
                this.Write((byte) (value & ((ulong) 0xffL)));
                this.Write((byte) ((value >> 8) & ((ulong) 0xffL)));
                this.Write((byte) ((value >> 0x10) & ((ulong) 0xffL)));
                this.Write((byte) ((value >> 0x18) & ((ulong) 0xffL)));
                this.Write((byte) ((value >> 0x20) & ((ulong) 0xffL)));
                this.Write((byte) ((value >> 40) & ((ulong) 0xffL)));
                this.Write((byte) ((value >> 0x30) & ((ulong) 0xffL)));
            }
            else
            {
                this.Write((byte) 0xff);
                this.Write((byte) (value & ((ulong) 0xffL)));
                this.Write((byte) ((value >> 8) & ((ulong) 0xffL)));
                this.Write((byte) ((value >> 0x10) & ((ulong) 0xffL)));
                this.Write((byte) ((value >> 0x18) & ((ulong) 0xffL)));
                this.Write((byte) ((value >> 0x20) & ((ulong) 0xffL)));
                this.Write((byte) ((value >> 40) & ((ulong) 0xffL)));
                this.Write((byte) ((value >> 0x30) & ((ulong) 0xffL)));
                this.Write((byte) ((value >> 0x38) & ((ulong) 0xffL)));
            }
        }

        /// <summary>
        /// <para>The current position of the internal buffer.</para>
        /// </summary>
        public short Position =>
            ((short) this.m_Buffer.Position);
    }
}

