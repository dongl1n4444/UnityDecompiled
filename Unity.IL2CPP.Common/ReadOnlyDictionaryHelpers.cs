using System;
using System.Collections;
using System.Collections.Generic;

namespace Unity.IL2CPP.Common
{
	internal static class ReadOnlyDictionaryHelpers
	{
		internal static void CopyToNonGenericICollectionHelper<T>(ICollection<T> collection, Array array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (array.Rank != 1)
			{
				throw new ArgumentException();
			}
			if (array.GetLowerBound(0) != 0)
			{
				throw new ArgumentException();
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (array.Length - index < collection.Count)
			{
				throw new ArgumentException();
			}
			ICollection collection2 = collection as ICollection;
			if (collection2 != null)
			{
				collection2.CopyTo(array, index);
			}
			else
			{
				T[] array2 = array as T[];
				if (array2 != null)
				{
					collection.CopyTo(array2, index);
				}
				else
				{
					Type elementType = array.GetType().GetElementType();
					Type typeFromHandle = typeof(T);
					if (!elementType.IsAssignableFrom(typeFromHandle) && !typeFromHandle.IsAssignableFrom(elementType))
					{
						throw new ArgumentException();
					}
					object[] array3 = array as object[];
					if (array3 == null)
					{
						throw new ArgumentException();
					}
					try
					{
						foreach (T current in collection)
						{
							array3[index++] = current;
						}
					}
					catch (ArrayTypeMismatchException)
					{
						throw new ArgumentException();
					}
				}
			}
		}
	}
}
