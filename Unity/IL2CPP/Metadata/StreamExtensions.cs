namespace Unity.IL2CPP.Metadata
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;

    internal static class StreamExtensions
    {
        public static void AlignTo(this Stream stream, int alignment)
        {
            while ((stream.Position % ((long) alignment)) != 0L)
            {
                stream.WriteByte(0);
            }
        }

        public static void WriteInt(this Stream stream, int value)
        {
            stream.WriteByte((byte) (value & 0xff));
            stream.WriteByte((byte) ((value >> 8) & 0xff));
            stream.WriteByte((byte) ((value >> 0x10) & 0xff));
            stream.WriteByte((byte) ((value >> 0x18) & 0xff));
        }

        public static void WriteIntAsUShort(this Stream stream, int value)
        {
            if (value < 0)
            {
                throw new ArgumentException("Value of type 'int' is negative and cannot convert to ushort.", "value");
            }
            if (value > 0x7fff)
            {
                throw new ArgumentException("Value of type 'int' is too large to convert to ushort.", "value");
            }
            stream.WriteUShort((ushort) value);
        }

        public static void WriteLongAsInt(this Stream stream, long value)
        {
            if (value > 0x7fffffffL)
            {
                throw new ArgumentException("Value of type 'long' is too large to convert to int.", "value");
            }
            stream.WriteInt((int) value);
        }

        public static void WriteShort(this Stream stream, short value)
        {
            stream.WriteByte((byte) (value & 0xff));
            stream.WriteByte((byte) ((value >> 8) & 0xff));
        }

        public static void WriteUInt(this Stream stream, uint value)
        {
            stream.WriteByte((byte) (value & 0xff));
            stream.WriteByte((byte) ((value >> 8) & 0xff));
            stream.WriteByte((byte) ((value >> 0x10) & 0xff));
            stream.WriteByte((byte) ((value >> 0x18) & 0xff));
        }

        public static void WriteUShort(this Stream stream, ushort value)
        {
            stream.WriteByte((byte) (value & 0xff));
            stream.WriteByte((byte) ((value >> 8) & 0xff));
        }
    }
}

