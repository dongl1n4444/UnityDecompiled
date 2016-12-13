using System;

namespace Unity.IL2CPP.Common
{
	public class AssemblyNameParseInfo
	{
		internal const int PublicKeyTokenLength = 17;

		public string Name
		{
			get;
			internal set;
		}

		public string Culture
		{
			get;
			internal set;
		}

		public string HashValue
		{
			get;
			internal set;
		}

		public string PublicKey
		{
			get;
			internal set;
		}

		public char[] PublicKeyToken
		{
			get;
			internal set;
		}

		public uint HashAlgorithm
		{
			get;
			internal set;
		}

		public uint HashLength
		{
			get;
			internal set;
		}

		public uint Flags
		{
			get;
			internal set;
		}

		public ushort Major
		{
			get;
			internal set;
		}

		public ushort Minor
		{
			get;
			internal set;
		}

		public ushort Build
		{
			get;
			internal set;
		}

		public ushort Revision
		{
			get;
			internal set;
		}

		public AssemblyNameParseInfo()
		{
			this.PublicKeyToken = new char[17];
		}
	}
}
