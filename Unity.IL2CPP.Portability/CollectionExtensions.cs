using System;
using System.Collections.ObjectModel;

namespace Unity.IL2CPP.Portability
{
	public static class CollectionExtensions
	{
		public static ReadOnlyCollection<T> AsReadOnlyPortable<T>(this T[] array)
		{
			return new ReadOnlyCollection<T>(array);
		}
	}
}
