namespace Unity.IL2CPP.Portability
{
    using System;
    using System.Collections.ObjectModel;
    using System.Runtime.CompilerServices;

    [Extension]
    public static class CollectionExtensions
    {
        [Extension]
        public static ReadOnlyCollection<T> AsReadOnlyPortable<T>(T[] array)
        {
            return new ReadOnlyCollection<T>(array);
        }
    }
}

