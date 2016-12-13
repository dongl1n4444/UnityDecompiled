using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;

namespace Unity.IL2CPP.StringLiterals
{
	public class StringLiteralWriter
	{
		[Inject]
		public static IStringLiteralCollection StringLiterals;

		[Inject]
		public static IStatsService StatsService;

		public void Write(Stream stringLiteralStream, Stream stringLiteralDataStream)
		{
			IStringLiteralCollection stringLiterals = StringLiteralWriter.StringLiterals;
			ReadOnlyCollection<string> stringLiterals2 = stringLiterals.GetStringLiterals();
			int[] array = new int[stringLiterals2.Count];
			List<byte> list = new List<byte>();
			for (int i = 0; i < stringLiterals2.Count; i++)
			{
				array[i] = list.Count;
				string text = stringLiterals2[i];
				list.AddRange(Encoding.UTF8.GetBytes(text));
				StringLiteralWriter.StatsService.RecordStringLiteral(text);
			}
			byte[] array2 = new byte[stringLiterals2.Count * 8];
			for (int j = 0; j < stringLiterals2.Count; j++)
			{
				string s = stringLiterals2[j];
				StringLiteralWriter.ToBytes(Encoding.UTF8.GetByteCount(s), array2, j * 8);
				StringLiteralWriter.ToBytes(array[j], array2, j * 8 + 4);
			}
			stringLiteralStream.Write(array2, 0, array2.Length);
			stringLiteralDataStream.Write(list.ToArray(), 0, list.Count);
		}

		private static void ToBytes(int value, byte[] bytes, int offset)
		{
			StringLiteralWriter.ToBytes((uint)value, bytes, offset);
		}

		private static void ToBytes(uint value, byte[] bytes, int offset)
		{
			bytes[offset] = (byte)(value & 255u);
			bytes[offset + 1] = (byte)(value >> 8 & 255u);
			bytes[offset + 2] = (byte)(value >> 16 & 255u);
			bytes[offset + 3] = (byte)(value >> 24 & 255u);
		}
	}
}
