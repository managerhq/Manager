using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagerServer
{
    public class FenwickTree
    {
        private decimal[] _tree;
        private DateTime[] _dates;
        private SortedList<DateTime, decimal> _moreDates = new();

        public void UpdateMany(Tuple<DateTime, decimal>[] values)
        {
            if (values == null || values.Length == 0) return;

            if (_dates == null)
            {
                _dates = [.. values.Select(x => x.Item1)];
                _tree = new decimal[_dates.Length + 1]; // 1-based index
                for (int i = 0; i < values.Length; i++) Update(i, values[i].Item2);
            }
            else
            {
                foreach (var e in values)
                {
                    int index = Array.BinarySearch(_dates, e.Item1);
                    if (index < 0)
                    {
                        _moreDates[e.Item1] = e.Item2.SafeAdd(_moreDates.GetValueOrDefault(e.Item1));
                    }
                    else
                    {
                        Update(index, e.Item2);
                    }
                }
            }
        }        

        private void Update(int index, decimal value)
        {
            index += 1; // Convert to 1-based index
            while (index < _tree.Length)
            {
                _tree[index] = _tree[index].SafeAdd(value);
                index += index & -index; // Move to next responsible index
            }
        }

        public decimal PrefixSum(DateTime date)
        {
            int index = FindPreviousIndex(date);
            if (index == -1) return 0; // No earlier date, return 0

            index += 1; // Convert to 1-based index
            var sum = 0m;
            while (index > 0)
            {
                sum = sum.SafeAdd(_tree[index]);
                index -= index & -index; // Move to parent
            }

            sum += _moreDates.TakeWhile(x => x.Key <= date).Select(x => x.Value).SafeSum();

            return sum;
        }

        public decimal RangeSum(DateTime startDate, DateTime endDate)
        {
            if (startDate == DateTime.MinValue) return PrefixSum(endDate);
            else return PrefixSum(endDate) - PrefixSum(startDate.AddDays(-1));
        }

        private int FindPreviousIndex(DateTime date)
        {
            if (_dates == null) return -1;
            int index = Array.BinarySearch(_dates, date);
            if (index >= 0) return index; // Exact match
            index = ~index - 1;           // Get previous date
            return (index >= 0) ? index : -1;
        }
    }
}