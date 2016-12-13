using Mono.Cecil;
using Mono.Cecil.Cil;
using System;

namespace Unity.IL2CPP
{
	public sealed class ErrorInformation
	{
		private static readonly ErrorInformation _instance = new ErrorInformation();

		private TypeDefinition _type;

		private MethodDefinition _method;

		public static ErrorInformation CurrentlyProcessing
		{
			get
			{
				return ErrorInformation._instance;
			}
		}

		public TypeDefinition Type
		{
			get
			{
				return this._type;
			}
			set
			{
				this.Method = null;
				this.Instruction = null;
				this._type = value;
			}
		}

		public MethodDefinition Method
		{
			get
			{
				return this._method;
			}
			set
			{
				this.Instruction = null;
				this._method = value;
			}
		}

		public Instruction Instruction
		{
			get;
			set;
		}
	}
}
