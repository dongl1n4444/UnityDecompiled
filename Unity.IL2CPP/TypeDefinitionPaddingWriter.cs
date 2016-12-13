using Mono.Cecil;
using System;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;

namespace Unity.IL2CPP
{
	public class TypeDefinitionPaddingWriter : IDisposable
	{
		[Inject]
		public static INamingService Naming;

		private readonly CppCodeWriter _writer;

		private readonly TypeDefinition _type;

		public TypeDefinitionPaddingWriter(CppCodeWriter writer, TypeDefinition type)
		{
			this._type = type;
			this._writer = writer;
			this.WritePaddingStart();
		}

		public void Dispose()
		{
			this.WritePaddingEnd();
		}

		private void WritePaddingStart()
		{
			if (this.NeedsPadding())
			{
				this._writer.WriteLine("union");
				this._writer.BeginBlock();
				this._writer.WriteLine("struct");
				this._writer.BeginBlock();
			}
		}

		private void WritePaddingEnd()
		{
			if (this.NeedsPadding())
			{
				this._writer.EndBlock(true);
				this._writer.WriteLine("uint8_t {0}[{1}];", new object[]
				{
					TypeDefinitionPaddingWriter.Naming.ForPadding(this._type),
					this._type.ClassSize
				});
				this._writer.EndBlock(true);
			}
		}

		private bool NeedsPadding()
		{
			return this._type.ClassSize > 0;
		}
	}
}
