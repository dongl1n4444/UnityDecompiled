using System;

namespace Unity.IL2CPP.IoCServices
{
	public interface IIcallMappingService
	{
		string ResolveICallFunction(string icall);
	}
}
