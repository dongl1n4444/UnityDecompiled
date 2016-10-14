using System;
using System.IO;

namespace Unity.IL2CPP.Metadata
{
	internal static class StreamExtensions
	{
		public static void AlignTo(this Stream stream, int alignment)
		{
			while (stream.Position % (long)alignment != 0L)
			{
				stream.WriteByte(0);
			}
		}

		public static void WriteShort(this Stream stream, short value)
		{
			stream.WriteByte((byte)(value & 255));
			stream.WriteByte((byte)(value >> 8 & 255));
		}

		public static void WriteUShort(this Stream stream, ushort value)
		{
			stream.WriteByte((byte)(value & 255));
			stream.WriteByte((byte)(value >> 8 & 255));
		}

		public static void WriteIntAsUShort(this Stream stream, int value)
		{
			if (value < 0)
			{
				throw new ArgumentException("Value of type 'int' is negative and cannot convert to ushort.", "value");
			}
			if (value > 32767)
			{
				throw new ArgumentException("Value of type 'int' is too large to convert to ushort.", "value");
			}
			stream.WriteUShort((ushort)value);
		}

		public static void WriteLongAsInt(this Stream stream, long value)
		{
			if (value > 2147483647L)
			{
				throw new ArgumentException("Value of type 'long' is too large to convert to int.", "value");
			}
			stream.WriteInt((int)value);
		}

		public static void WriteInt(this Stream stream, int value)
		{
			stream.WriteByte((byte)(value & 255));
			stream.WriteByte((byte)(value >> 8 & 255));
			stream.WriteByte((byte)(value >> 16 & 255));
			stream.WriteByte((byte)(value >> 24 & 255));
		}

		public static void WriteUInt(this Stream stream, uint value)
		{
			stream.WriteByte((byte)(value & 255u));
			stream.WriteByte((byte)(value >> 8 & 255u));
			stream.WriteByte((byte)(value >> 16 & 255u));
			stream.WriteByte((byte)(value >> 24 & 255u));
		}
	}
}
