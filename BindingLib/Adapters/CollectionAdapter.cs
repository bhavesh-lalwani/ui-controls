using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BindingLib.Adapters
{
    public static class CollectionAdapter
    {
        public static ObservableCollection<T> ToObservable<T>(this IEnumerable<T> source)
            => new(source ?? throw new ArgumentNullException(nameof(source)));

        public static void ReloadFrom<T>(this ObservableCollection<T> col, IEnumerable<T> source)
        { col.Clear(); foreach (var item in source) col.Add(item); }
    }
}
