using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public static class LinqExtensions
{
    public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> values)
    {
        foreach (var value in values)
            collection.Add(value);
    }

    public static void RemoveWhere<T>(this IList<T> list, Func<T, bool> predicate)
    {
        var removeList = list.Where(predicate).ToArray();
        foreach (var entry in removeList)
            list.Remove(entry);
    }
}
