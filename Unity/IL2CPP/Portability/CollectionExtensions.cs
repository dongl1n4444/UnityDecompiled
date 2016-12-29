namespace Unity.IL2CPP.Portability
{
    using System;
    using System.Collections.ObjectModel;
    using System.Runtime.CompilerServices;

    public static class CollectionExtensions
    {
        public static ReadOnlyCollection<T> AsReadOnlyPortable<T>(this T[] array) => 
            new ReadOnlyCollection<T>(array);
    }
}

