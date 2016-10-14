using Mono.Cecil;
using System;
using System.Collections.Generic;
using Unity.IL2CPP.Common;

namespace Unity.IL2CPP
{
	public class MethodUsage
	{
		private readonly HashSet<MethodReference> _methods = new HashSet<MethodReference>(new MethodReferenceComparer());

		public void AddMethod(MethodReference method)
		{
			this._methods.Add(method);
		}

		public IEnumerable<MethodReference> GetMethods()
		{
			return this._methods;
		}
	}
}
