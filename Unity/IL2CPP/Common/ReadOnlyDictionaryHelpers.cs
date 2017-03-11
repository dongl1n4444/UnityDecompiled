namespace Unity.IL2CPP.Common
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Unity.IL2CPP.Portability;

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
            if ((array.Length - index) < collection.Count)
            {
                throw new ArgumentException();
            }
            ICollection is2 = collection as ICollection;
            if (is2 != null)
            {
                is2.CopyTo(array, index);
            }
            else
            {
                T[] localArray = array as T[];
                if (localArray != null)
                {
                    collection.CopyTo(localArray, index);
                }
                else
                {
                    Type elementType = array.GetType().GetElementType();
                    Type other = typeof(T);
                    if (!elementType.IsAssignableFromPortable(other) && !other.IsAssignableFromPortable(elementType))
                    {
                        throw new ArgumentException();
                    }
                    object[] objArray = array as object[];
                    if (objArray == null)
                    {
                        throw new ArgumentException();
                    }
                    try
                    {
                        foreach (T local in collection)
                        {
                            objArray[index++] = local;
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

