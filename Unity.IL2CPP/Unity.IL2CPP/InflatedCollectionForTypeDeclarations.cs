using Mono.Cecil;
using System;
using Unity.IL2CPP.Common;
using Unity.IL2CPP.GenericsCollection;
using Unity.IL2CPP.IoCServices;

namespace Unity.IL2CPP
{
	public class InflatedCollectionForTypeDeclarations : InflatedCollection<GenericInstanceType>
	{
		private readonly IIl2CppTypeCollectorWriterService _il2CppTypeCollector;

		public InflatedCollectionForTypeDeclarations(IIl2CppTypeCollectorWriterService il2CppTypeCollector) : base(new TypeReferenceEqualityComparer())
		{
			this._il2CppTypeCollector = il2CppTypeCollector;
		}

		public override bool Add(GenericInstanceType item)
		{
			bool flag = base.Add(item);
			if (flag)
			{
				this._il2CppTypeCollector.Add(item, 0);
			}
			return flag;
		}
	}
}
