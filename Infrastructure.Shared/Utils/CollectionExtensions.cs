using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Shared.Utils
{
    static public class CollectionExtensions
    {
        static public bool IsNullOrEmpty<T>(this ICollection<T> items)
        {
            if (items == null)
            {
                return true;
            }

            return items.Count == 0;
        }

        static public bool IsNullOrEmpty<T>(this IEnumerable<T> items)
        {
            if (items is ICollection<T> collection)
            {
                return collection.IsNullOrEmpty();
            }

            if (items == null)
            {
                return true;
            }

            return !items.Any();
        }
    }
}
