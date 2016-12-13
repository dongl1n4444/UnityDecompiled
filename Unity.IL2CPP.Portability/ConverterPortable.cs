using System;

namespace Unity.IL2CPP.Portability
{
	public delegate TOutput ConverterPortable<in TInput, out TOutput>(TInput input);
}
