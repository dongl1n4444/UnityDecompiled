using Mono.Cecil;
using System;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;

namespace Unity.IL2CPP
{
	public class Local
	{
		private readonly TypeReference _type;

		private readonly string _expression;

		[Inject]
		public static INamingService Naming;

		public string IdentifierExpression
		{
			get
			{
				return Local.Naming.ForVariable(this._type) + " " + this._expression;
			}
		}

		public string Expression
		{
			get
			{
				return this._expression;
			}
		}

		public TypeReference Type
		{
			get
			{
				return this._type;
			}
		}

		public Local(TypeReference type, string expression)
		{
			this._type = type;
			this._expression = expression;
		}
	}
}
