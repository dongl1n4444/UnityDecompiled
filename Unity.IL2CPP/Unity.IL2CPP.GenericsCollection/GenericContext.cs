using Mono.Cecil;
using System;

namespace Unity.IL2CPP.GenericsCollection
{
	public class GenericContext
	{
		private readonly GenericInstanceType _type;

		private readonly GenericInstanceMethod _method;

		public GenericInstanceType Type
		{
			get
			{
				return this._type;
			}
		}

		public GenericInstanceMethod Method
		{
			get
			{
				return this._method;
			}
		}

		public GenericContext(GenericInstanceType type, GenericInstanceMethod method)
		{
			this._type = type;
			this._method = method;
		}
	}
}
