namespace UnityEngine.Networking
{
    using System;
    using System.Text;
    using UnityEngine;

    /// <summary>
    /// <para>General purpose serializer for UNET (for reading byte arrays).</para>
    /// </summary>
    public class NetworkReader
    {
        private const int k_InitialStringBufferSize = 0x400;
        private const int k_MaxStringLength = 0x8000;
        private NetBuffer m_buf;
        private static Encoding s_Encoding;
        private static byte[] s_StringReaderBuffer;

        /// <summary>
        /// <para>Creates a new NetworkReader object.</para>
        /// </summary>
        /// <param name="buffer">A buffer to construct the reader with, this buffer is NOT copied.</param>
        public NetworkReader()
        {
            this.m_buf = new NetBuffer();
            Initialize();
        }

        public NetworkReader(NetworkWriter writer)
        {
            this.m_buf = new NetBuffer(writer.AsArray());
            Initialize();
        }

        /// <summary>
        /// <para>Creates a new NetworkReader object.</para>
        /// </summary>
        /// <param name="buffer">A buffer to construct the reader with, this buffer is NOT copied.</param>
        public NetworkReader(byte[] buffer)
        {
            this.m_buf = new NetBuffer(buffer);
            Initialize();
        }

        private static void Initialize()
        {
            if (s_Encoding == null)
            {
                s_StringReaderBuffer = new byte[0x400];
                s_Encoding = new UTF8Encoding();
            }
        }

        /// <summary>
        /// <para>Reads a boolean from the stream.</para>
        /// </summary>
        /// <returns>
        /// <para>The value read.</para>
        /// </returns>
        public bool ReadBoolean()
        {
            return (this.m_buf.ReadByte() == 1);
        }

        /// <summary>
        /// <para>Reads a byte from the stream.</para>
        /// </summary>
        /// <returns>
        /// <para>The value read.</para>
        /// </returns>
        public byte ReadByte()
        {
            return this.m_buf.ReadByte();
        }

        /// <summary>
        /// <para>Reads a number of bytes from the stream.</para>
        /// </summary>
        /// <param name="count">Number of bytes to read.</param>
        /// <returns>
        /// <para>Bytes read. (this is a copy).</para>
        /// </returns>
        public byte[] ReadBytes(int count)
        {
            if (count < 0)
            {
                throw new IndexOutOfRangeException("NetworkReader ReadBytes " + count);
            }
            byte[] buffer = new byte[count];
            this.m_buf.ReadBytes(buffer, (uint) count);
            return buffer;
        }

        /// <summary>
        /// <para>This read a 16-bit byte count and a array of bytes of that size from the stream.</para>
        /// </summary>
        /// <returns>
        /// <para>The bytes read from the stream.</para>
        /// </returns>
        public byte[] ReadBytesAndSize()
        {
            ushort count = this.ReadUInt16();
            if (count == 0)
            {
                return null;
            }
            return this.ReadBytes(count);
        }

        /// <summary>
        /// <para>Reads a char from the stream.</para>
        /// </summary>
        /// <returns>
        /// <para>Value read.</para>
        /// </returns>
        public char ReadChar()
        {
            return (char) this.m_buf.ReadByte();
        }

        /// <summary>
        /// <para>Reads a unity Color objects.</para>
        /// </summary>
        /// <returns>
        /// <para>The color read from the stream.</para>
        /// </returns>
        public Color ReadColor()
        {
            return new Color(this.ReadSingle(), this.ReadSingle(), this.ReadSingle(), this.ReadSingle());
        }

        /// <summary>
        /// <para>Reads a unity color32 objects.</para>
        /// </summary>
        /// <returns>
        /// <para>The colo read from the stream.</para>
        /// </returns>
        public Color32 ReadColor32()
        {
            return new Color32(this.ReadByte(), this.ReadByte(), this.ReadByte(), this.ReadByte());
        }

        /// <summary>
        /// <para>Reads a decimal from the stream.</para>
        /// </summary>
        /// <returns>
        /// <para>Value read.</para>
        /// </returns>
        public decimal ReadDecimal()
        {
            return new decimal(new int[] { this.ReadInt32(), this.ReadInt32(), this.ReadInt32(), this.ReadInt32() });
        }

        /// <summary>
        /// <para>Reads a double from the stream.</para>
        /// </summary>
        /// <returns>
        /// <para>Value read.</para>
        /// </returns>
        public double ReadDouble()
        {
            return FloatConversion.ToDouble(this.ReadUInt64());
        }

        /// <summary>
        /// <para>Reads a reference to a GameObject from the stream.</para>
        /// </summary>
        /// <returns>
        /// <para>The GameObject referenced.</para>
        /// </returns>
        public GameObject ReadGameObject()
        {
            GameObject obj3;
            NetworkInstanceId netId = this.ReadNetworkId();
            if (netId.IsEmpty())
            {
                return null;
            }
            if (NetworkServer.active)
            {
                obj3 = NetworkServer.FindLocalObject(netId);
            }
            else
            {
                obj3 = ClientScene.FindLocalObject(netId);
            }
            if ((obj3 == null) && LogFilter.logDebug)
            {
                Debug.Log("ReadGameObject netId:" + netId + "go: null");
            }
            return obj3;
        }

        /// <summary>
        /// <para>Reads a signed 16 bit integer from the stream.</para>
        /// </summary>
        /// <returns>
        /// <para>Value read.</para>
        /// </returns>
        public short ReadInt16()
        {
            ushort num = 0;
            num = (ushort) (num | this.m_buf.ReadByte());
            num = (ushort) (num | ((ushort) (this.m_buf.ReadByte() << 8)));
            return (short) num;
        }

        /// <summary>
        /// <para>Reads a signed 32bit integer from the stream.</para>
        /// </summary>
        /// <returns>
        /// <para>Value read.</para>
        /// </returns>
        public int ReadInt32()
        {
            uint num = 0;
            num |= this.m_buf.ReadByte();
            num |= (uint) (this.m_buf.ReadByte() << 8);
            num |= (uint) (this.m_buf.ReadByte() << 0x10);
            num |= (uint) (this.m_buf.ReadByte() << 0x18);
            return (int) num;
        }

        /// <summary>
        /// <para>Reads a signed 64 bit integer from the stream.</para>
        /// </summary>
        /// <returns>
        /// <para>Value read.</para>
        /// </returns>
        public long ReadInt64()
        {
            ulong num = 0L;
            ulong num2 = this.m_buf.ReadByte();
            num |= num2;
            num2 = (ulong) (this.m_buf.ReadByte() << 8);
            num |= num2;
            num2 = (ulong) (this.m_buf.ReadByte() << 0x10);
            num |= num2;
            num2 = (ulong) (this.m_buf.ReadByte() << 0x18);
            num |= num2;
            num2 = (ulong) (this.m_buf.ReadByte() << 0x20);
            num |= num2;
            num2 = (ulong) (this.m_buf.ReadByte() << 40);
            num |= num2;
            num2 = (ulong) (this.m_buf.ReadByte() << 0x30);
            num |= num2;
            num2 = (ulong) (this.m_buf.ReadByte() << 0x38);
            num |= num2;
            return (long) num;
        }

        /// <summary>
        /// <para>Reads a unity Matrix4x4 object.</para>
        /// </summary>
        /// <returns>
        /// <para>The matrix read from the stream.</para>
        /// </returns>
        public Matrix4x4 ReadMatrix4x4()
        {
            return new Matrix4x4 { 
                m00 = this.ReadSingle(),
                m01 = this.ReadSingle(),
                m02 = this.ReadSingle(),
                m03 = this.ReadSingle(),
                m10 = this.ReadSingle(),
                m11 = this.ReadSingle(),
                m12 = this.ReadSingle(),
                m13 = this.ReadSingle(),
                m20 = this.ReadSingle(),
                m21 = this.ReadSingle(),
                m22 = this.ReadSingle(),
                m23 = this.ReadSingle(),
                m30 = this.ReadSingle(),
                m31 = this.ReadSingle(),
                m32 = this.ReadSingle(),
                m33 = this.ReadSingle()
            };
        }

        public TMsg ReadMessage<TMsg>() where TMsg: MessageBase, new()
        {
            TMsg local = Activator.CreateInstance<TMsg>();
            local.Deserialize(this);
            return local;
        }

        /// <summary>
        /// <para>Reads a NetworkHash128 assetId.</para>
        /// </summary>
        /// <returns>
        /// <para>The assetId object read from the stream.</para>
        /// </returns>
        public NetworkHash128 ReadNetworkHash128()
        {
            NetworkHash128 hash;
            hash.i0 = this.ReadByte();
            hash.i1 = this.ReadByte();
            hash.i2 = this.ReadByte();
            hash.i3 = this.ReadByte();
            hash.i4 = this.ReadByte();
            hash.i5 = this.ReadByte();
            hash.i6 = this.ReadByte();
            hash.i7 = this.ReadByte();
            hash.i8 = this.ReadByte();
            hash.i9 = this.ReadByte();
            hash.i10 = this.ReadByte();
            hash.i11 = this.ReadByte();
            hash.i12 = this.ReadByte();
            hash.i13 = this.ReadByte();
            hash.i14 = this.ReadByte();
            hash.i15 = this.ReadByte();
            return hash;
        }

        /// <summary>
        /// <para>Reads a NetworkInstanceId from the stream.</para>
        /// </summary>
        /// <returns>
        /// <para>The NetworkInstanceId read.</para>
        /// </returns>
        public NetworkInstanceId ReadNetworkId()
        {
            return new NetworkInstanceId(this.ReadPackedUInt32());
        }

        /// <summary>
        /// <para>Reads a reference to a NetworkIdentity from the stream.</para>
        /// </summary>
        /// <returns>
        /// <para>The NetworkIdentity object read.</para>
        /// </returns>
        public NetworkIdentity ReadNetworkIdentity()
        {
            GameObject obj2;
            NetworkInstanceId netId = this.ReadNetworkId();
            if (netId.IsEmpty())
            {
                return null;
            }
            if (NetworkServer.active)
            {
                obj2 = NetworkServer.FindLocalObject(netId);
            }
            else
            {
                obj2 = ClientScene.FindLocalObject(netId);
            }
            if (obj2 == null)
            {
                if (LogFilter.logDebug)
                {
                    Debug.Log("ReadNetworkIdentity netId:" + netId + "go: null");
                }
                return null;
            }
            return obj2.GetComponent<NetworkIdentity>();
        }

        /// <summary>
        /// <para>Reads a 32-bit variable-length-encoded value.</para>
        /// </summary>
        /// <returns>
        /// <para>The 32 bit value read.</para>
        /// </returns>
        public uint ReadPackedUInt32()
        {
            byte num = this.ReadByte();
            if (num < 0xf1)
            {
                return num;
            }
            byte num3 = this.ReadByte();
            if ((num >= 0xf1) && (num <= 0xf8))
            {
                return (uint) ((240 + (0x100 * (num - 0xf1))) + num3);
            }
            byte num4 = this.ReadByte();
            if (num == 0xf9)
            {
                return (uint) ((0x8f0 + (0x100 * num3)) + num4);
            }
            byte num5 = this.ReadByte();
            if (num == 250)
            {
                return (uint) ((num3 + (num4 << 8)) + (num5 << 0x10));
            }
            byte num6 = this.ReadByte();
            if (num < 0xfb)
            {
                throw new IndexOutOfRangeException("ReadPackedUInt32() failure: " + num);
            }
            return (uint) (((num3 + (num4 << 8)) + (num5 << 0x10)) + (num6 << 0x18));
        }

        /// <summary>
        /// <para>Reads a 64-bit variable-length-encoded value.</para>
        /// </summary>
        /// <returns>
        /// <para>The 64 bit value read.</para>
        /// </returns>
        public ulong ReadPackedUInt64()
        {
            byte num = this.ReadByte();
            if (num < 0xf1)
            {
                return (ulong) num;
            }
            byte num3 = this.ReadByte();
            if ((num >= 0xf1) && (num <= 0xf8))
            {
                return ((((ulong) 240L) + (((ulong) 0x100L) * (num - 0xf1L))) + num3);
            }
            byte num4 = this.ReadByte();
            if (num == 0xf9)
            {
                return ((((ulong) 0x8f0L) + (0x100L * num3)) + num4);
            }
            byte num5 = this.ReadByte();
            if (num == 250)
            {
                return (ulong) ((num3 + (num4 << 8)) + (num5 << 0x10));
            }
            byte num6 = this.ReadByte();
            if (num == 0xfb)
            {
                return (ulong) (((num3 + (num4 << 8)) + (num5 << 0x10)) + (num6 << 0x18));
            }
            byte num7 = this.ReadByte();
            if (num == 0xfc)
            {
                return (ulong) ((((num3 + (num4 << 8)) + (num5 << 0x10)) + (num6 << 0x18)) + (num7 << 0x20));
            }
            byte num8 = this.ReadByte();
            if (num == 0xfd)
            {
                return (ulong) (((((num3 + (num4 << 8)) + (num5 << 0x10)) + (num6 << 0x18)) + (num7 << 0x20)) + (num8 << 40));
            }
            byte num9 = this.ReadByte();
            if (num == 0xfe)
            {
                return (ulong) ((((((num3 + (num4 << 8)) + (num5 << 0x10)) + (num6 << 0x18)) + (num7 << 0x20)) + (num8 << 40)) + (num9 << 0x30));
            }
            byte num10 = this.ReadByte();
            if (num != 0xff)
            {
                throw new IndexOutOfRangeException("ReadPackedUInt64() failure: " + num);
            }
            return (ulong) (((((((num3 + (num4 << 8)) + (num5 << 0x10)) + (num6 << 0x18)) + (num7 << 0x20)) + (num8 << 40)) + (num9 << 0x30)) + (num10 << 0x38));
        }

        /// <summary>
        /// <para>Reads a unity Plane object.</para>
        /// </summary>
        /// <returns>
        /// <para>The plane read from the stream.</para>
        /// </returns>
        public Plane ReadPlane()
        {
            return new Plane(this.ReadVector3(), this.ReadSingle());
        }

        /// <summary>
        /// <para>Reads a Unity Quaternion object.</para>
        /// </summary>
        /// <returns>
        /// <para>The quaternion read from the stream.</para>
        /// </returns>
        public Quaternion ReadQuaternion()
        {
            return new Quaternion(this.ReadSingle(), this.ReadSingle(), this.ReadSingle(), this.ReadSingle());
        }

        /// <summary>
        /// <para>Reads a Unity Ray object.</para>
        /// </summary>
        /// <returns>
        /// <para>The ray read from the stream.</para>
        /// </returns>
        public Ray ReadRay()
        {
            return new Ray(this.ReadVector3(), this.ReadVector3());
        }

        /// <summary>
        /// <para>Reads a Unity Rect object.</para>
        /// </summary>
        /// <returns>
        /// <para>The rect read from the stream.</para>
        /// </returns>
        public Rect ReadRect()
        {
            return new Rect(this.ReadSingle(), this.ReadSingle(), this.ReadSingle(), this.ReadSingle());
        }

        /// <summary>
        /// <para>Reads a signed byte from the stream.</para>
        /// </summary>
        /// <returns>
        /// <para>Value read.</para>
        /// </returns>
        public sbyte ReadSByte()
        {
            return (sbyte) this.m_buf.ReadByte();
        }

        /// <summary>
        /// <para>Reads a NetworkSceneId from the stream.</para>
        /// </summary>
        /// <returns>
        /// <para>The NetworkSceneId read.</para>
        /// </returns>
        public NetworkSceneId ReadSceneId()
        {
            return new NetworkSceneId(this.ReadPackedUInt32());
        }

        /// <summary>
        /// <para>Reads a float from the stream.</para>
        /// </summary>
        /// <returns>
        /// <para>Value read.</para>
        /// </returns>
        public float ReadSingle()
        {
            return FloatConversion.ToSingle(this.ReadUInt32());
        }

        /// <summary>
        /// <para>Reads a string from the stream. (max of 32k bytes).</para>
        /// </summary>
        /// <returns>
        /// <para>Value read.</para>
        /// </returns>
        public string ReadString()
        {
            ushort count = this.ReadUInt16();
            if (count == 0)
            {
                return "";
            }
            if (count >= 0x8000)
            {
                throw new IndexOutOfRangeException("ReadString() too long: " + count);
            }
            while (count > s_StringReaderBuffer.Length)
            {
                s_StringReaderBuffer = new byte[s_StringReaderBuffer.Length * 2];
            }
            this.m_buf.ReadBytes(s_StringReaderBuffer, count);
            return new string(s_Encoding.GetChars(s_StringReaderBuffer, 0, count));
        }

        /// <summary>
        /// <para>Reads a reference to a Transform from the stream.</para>
        /// </summary>
        /// <returns>
        /// <para>The transform object read.</para>
        /// </returns>
        public Transform ReadTransform()
        {
            NetworkInstanceId netId = this.ReadNetworkId();
            if (netId.IsEmpty())
            {
                return null;
            }
            GameObject obj2 = ClientScene.FindLocalObject(netId);
            if (obj2 == null)
            {
                if (LogFilter.logDebug)
                {
                    Debug.Log("ReadTransform netId:" + netId);
                }
                return null;
            }
            return obj2.transform;
        }

        /// <summary>
        /// <para>Reads an unsigned 16 bit integer from the stream.</para>
        /// </summary>
        /// <returns>
        /// <para>Value read.</para>
        /// </returns>
        public ushort ReadUInt16()
        {
            ushort num = 0;
            num = (ushort) (num | this.m_buf.ReadByte());
            return (ushort) (num | ((ushort) (this.m_buf.ReadByte() << 8)));
        }

        /// <summary>
        /// <para>Reads an unsigned 32 bit integer from the stream.</para>
        /// </summary>
        /// <returns>
        /// <para>Value read.</para>
        /// </returns>
        public uint ReadUInt32()
        {
            uint num = 0;
            num |= this.m_buf.ReadByte();
            num |= (uint) (this.m_buf.ReadByte() << 8);
            num |= (uint) (this.m_buf.ReadByte() << 0x10);
            return (num | ((uint) (this.m_buf.ReadByte() << 0x18)));
        }

        /// <summary>
        /// <para>Reads an unsigned 64 bit integer from the stream.</para>
        /// </summary>
        /// <returns>
        /// <para>Value read.</para>
        /// </returns>
        public ulong ReadUInt64()
        {
            ulong num = 0L;
            ulong num2 = this.m_buf.ReadByte();
            num |= num2;
            num2 = (ulong) (this.m_buf.ReadByte() << 8);
            num |= num2;
            num2 = (ulong) (this.m_buf.ReadByte() << 0x10);
            num |= num2;
            num2 = (ulong) (this.m_buf.ReadByte() << 0x18);
            num |= num2;
            num2 = (ulong) (this.m_buf.ReadByte() << 0x20);
            num |= num2;
            num2 = (ulong) (this.m_buf.ReadByte() << 40);
            num |= num2;
            num2 = (ulong) (this.m_buf.ReadByte() << 0x30);
            num |= num2;
            num2 = (ulong) (this.m_buf.ReadByte() << 0x38);
            return (num | num2);
        }

        /// <summary>
        /// <para>Reads a Unity Vector2 object.</para>
        /// </summary>
        /// <returns>
        /// <para>The vector read from the stream.</para>
        /// </returns>
        public Vector2 ReadVector2()
        {
            return new Vector2(this.ReadSingle(), this.ReadSingle());
        }

        /// <summary>
        /// <para>Reads a Unity Vector3 objects.</para>
        /// </summary>
        /// <returns>
        /// <para>The vector read from the stream.</para>
        /// </returns>
        public Vector3 ReadVector3()
        {
            return new Vector3(this.ReadSingle(), this.ReadSingle(), this.ReadSingle());
        }

        /// <summary>
        /// <para>Reads a Unity Vector4 object.</para>
        /// </summary>
        /// <returns>
        /// <para>The vector read from the stream.</para>
        /// </returns>
        public Vector4 ReadVector4()
        {
            return new Vector4(this.ReadSingle(), this.ReadSingle(), this.ReadSingle(), this.ReadSingle());
        }

        internal void Replace(byte[] buffer)
        {
            this.m_buf.Replace(buffer);
        }

        /// <summary>
        /// <para>Sets the current position of the reader's stream to the start of the stream.</para>
        /// </summary>
        public void SeekZero()
        {
            this.m_buf.SeekZero();
        }

        /// <summary>
        /// <para>Returns a string representation of the reader's buffer.</para>
        /// </summary>
        /// <returns>
        /// <para>Buffer contents.</para>
        /// </returns>
        public override string ToString()
        {
            return this.m_buf.ToString();
        }

        /// <summary>
        /// <para>The current length of the buffer.</para>
        /// </summary>
        public int Length
        {
            get
            {
                return this.m_buf.Length;
            }
        }

        /// <summary>
        /// <para>The current position within the buffer.</para>
        /// </summary>
        public uint Position
        {
            get
            {
                return this.m_buf.Position;
            }
        }
    }
}

