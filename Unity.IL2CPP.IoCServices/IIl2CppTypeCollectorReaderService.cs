using System;
using System.Collections.Generic;

namespace Unity.IL2CPP.IoCServices
{
	public interface IIl2CppTypeCollectorReaderService
	{
		IDictionary<Il2CppTypeData, int> Items
		{
			get;
		}
	}
}
