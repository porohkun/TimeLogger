using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace TimeLogger
{
    public static class Extensions
    {
        public static void Sort<T>(this ObservableCollection<T> collection, Comparison<T> comparison = null)
        {
            var sortableList = new List<T>(collection);
            if (comparison == null)
                sortableList.Sort();
            else
                sortableList.Sort(comparison);

            for (int i = 0; i < sortableList.Count; i++)
            {
                collection.Move(collection.IndexOf(sortableList[i]), i);
            }
        }

        public static void AddRange<T>(this Collection<T> collection, IEnumerable<T> items)
        {
            foreach (var item in items)
                collection.Add(item);
        }

        public static MainWindow MainWindow(this Application app)
        {
            return app.MainWindow as MainWindow;
        }
    }
}
