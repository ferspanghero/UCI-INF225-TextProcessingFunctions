using System;
using System.Collections.Generic;

namespace TextProcessingFunctions.Core.Common
{
    /// <summary>
    /// Contains extension methods for IEnumerable and IEnumerable[T]
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        /// Iterates a collection and returns the current and the next items per iteration
        /// </summary>
        public static IEnumerable<Tuple<T, T>> IterateWithNextItem<T>(this IEnumerable<T> collection)
        {
            if (collection != null)
            {
                var enumerator = collection.GetEnumerator();

                if (enumerator.MoveNext())
                {
                    T currentItem = enumerator.Current;

                    while (enumerator.MoveNext())
                    {
                        T nextItem = enumerator.Current;

                        yield return new Tuple<T, T>(currentItem, nextItem);
                        
                        currentItem = nextItem;
                    }

                    // If it reaches the end of the collection, then the next item is filled with a default value
                    yield return new Tuple<T, T>(currentItem, default(T));
                }
            }
        }
    }
}
