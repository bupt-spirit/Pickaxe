using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Pickaxe.Utility.ListExtension
{
    public static class ListExtension
    {
        public static void Resize<T>(this List<T> list, int size, T element)
        {
            var count = list.Count;
            if (size < count)
            {
                list.RemoveRange(size, count - size);
            }
            else if (size > count)
            {
                if (size > list.Capacity)
                    list.Capacity = size;
                list.AddRange(Enumerable.Repeat(element, size - count));
            }
        }

        public static void Resize<T>(this ObservableCollection<T> list, int size, T element)
        {
            var count = list.Count;
            if (size < count)
            {
                for (var i = count - 1; i >= size; --i)
                {
                    list.RemoveAt(i);
                }
            }
            else if (size > count)
            {
                for (var i = count; i < size; ++i)
                {
                    list.Add(element);
                }
            }
        }
    }
}
