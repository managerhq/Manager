using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagerServer
{
    public sealed class LazySortedList<T> where T : IComparable<T>
    {
        private readonly List<T> _list = new();
        private bool _isDirty = false;

        public void Add(T item)
        {
            _list.Add(item);
            _isDirty = true;
        }

        public bool Remove(T item)
        {
            if (_isDirty)
                Sort();

            int index = _list.BinarySearch(item);
            if (index < 0) return false;

            int last = _list.Count - 1;
            _list[index] = _list[last];
            _list.RemoveAt(last);
            _isDirty = true;
            return true;
        }

        public void Sort()
        {
            if (!_isDirty) return;
            _list.Sort();
            _isDirty = false;
        }

        public IEnumerable<T> Enumerate()
        {
            Sort();
            return _list;
        }

        public IReadOnlyList<T> Items => Enumerate().ToList();
    }
}
