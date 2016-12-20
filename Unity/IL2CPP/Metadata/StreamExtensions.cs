namespace Unity.IL2CPP.Metadata
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;

    [Extension]
    internal static class StreamExtensions
    {
        [Extension]
        public static void AlignTo(Stream stream, int alignment)
        {
            while ((stream.Position % ((long) alignment)) != 0L)
            {
                stream.WriteByte(0);
            }
        }

        [Extension]
        public static void WriteInt(Stream stream, int value)
        {
            stream.WriteByte((byte) (value & 0xff));
            stream.WriteByte((byte) ((value >> 8) & 0xff));
            stream.WriteByte((byte) ((value >> 0x10) & 0xff));
            stream.WriteByte((byte) ((value >> 0x18) & 0xff));
        }

        [Extension]
        public static void WriteIntAsUShort(Stream stream, int value)
        {
            if (value < 0)
            {
                throw new ArgumentException("Value of type 'int' is negative and cannot convert to ushort.", "value");
            }
            if (value > 0x7fff)
            {
                throw new ArgumentException("Value of type 'int' is too large to convert to ushort.", "value");
            }
            WriteUShort(stream, (ushort) value);
        }

        [Extension]
        public static void WriteLongAsInt(Stream stream, long value)
        {
            if (value > 0x7fffffffL)
            {
                throw new ArgumentException("Value of type 'long' is too large to convert to int.", "value");
            }
            WriteInt(stream, (int) value);
        }

        [Extension]
        public static void WriteShort(Stream stream, short value)
        {
            stream.WriteByte((byte) (value & 0xff));
            stream.WriteByte((byte) ((value >> 8) & 0xff));
        }

        [Extension]
        public static void WriteUInt(Stream stream, uint value)
        {
            stream.WriteByte((byte) (value & 0xff));
            stream.WriteByte((byte) ((value >> 8) & 0xff));
            stream.WriteByte((byte) ((value >> 0x10) & 0xff));
            stream.WriteByte((byte) ((value >> 0x18) & 0xff));
        }

        [Extension]
        public static void WriteUShort(Stream stream, ushort value)
        {
            stream.WriteByte((byte) (value & 0xff));
            stream.WriteByte((byte) ((value >> 8) & 0xff));
        }
    }
}

