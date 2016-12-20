namespace Unity.IL2CPP.StringLiterals
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Text;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;

    public class StringLiteralWriter
    {
        [Inject]
        public static IStatsService StatsService;
        [Inject]
        public static IStringLiteralCollection StringLiterals;

        private static void ToBytes(int value, byte[] bytes, int offset)
        {
            ToBytes((uint) value, bytes, offset);
        }

        private static void ToBytes(uint value, byte[] bytes, int offset)
        {
            bytes[offset] = (byte) (value & 0xff);
            bytes[offset + 1] = (byte) ((value >> 8) & 0xff);
            bytes[offset + 2] = (byte) ((value >> 0x10) & 0xff);
            bytes[offset + 3] = (byte) ((value >> 0x18) & 0xff);
        }

        public void Write(Stream stringLiteralStream, Stream stringLiteralDataStream)
        {
            ReadOnlyCollection<string> stringLiterals = StringLiterals.GetStringLiterals();
            int[] numArray = new int[stringLiterals.Count];
            List<byte> list = new List<byte>();
            for (int i = 0; i < stringLiterals.Count; i++)
            {
                numArray[i] = list.Count;
                string s = stringLiterals[i];
                list.AddRange(Encoding.UTF8.GetBytes(s));
                StatsService.RecordStringLiteral(s);
            }
            byte[] bytes = new byte[stringLiterals.Count * 8];
            for (int j = 0; j < stringLiterals.Count; j++)
            {
                string str2 = stringLiterals[j];
                ToBytes(Encoding.UTF8.GetByteCount(str2), bytes, j * 8);
                ToBytes(numArray[j], bytes, (j * 8) + 4);
            }
            stringLiteralStream.Write(bytes, 0, bytes.Length);
            stringLiteralDataStream.Write(list.ToArray(), 0, list.Count);
        }
    }
}

