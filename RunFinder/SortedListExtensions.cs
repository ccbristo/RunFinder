using System.Collections.Generic;

namespace RunFinder
{
    public static class SortedListExtensions
    {
        public static int? FindClosestIndex(this SortedList<int, Run> list, int value)
        {
            int low = 0, high = list.Count - 1;

            if (list.Count == 0)
                return null;

            // TODO: Do not assume the list will not contain nulls
            if (list.Values[0].IsExtendedBy(value))
                return 0;

            int lastIndex = list.Count - 1;
            var last = list.Values[lastIndex];
            if (last.IsExtendedBy(value))
                return lastIndex;

            int mid = 0;
            while (low <= high)
            {
                mid = low + (high - low) / 2;

                int previousIndex = mid - 1;
                int nextIndex = mid + 1;

                if (previousIndex >= 0 && list.Values[previousIndex].IsExtendedBy(value))
                    return previousIndex;

                if (nextIndex <= lastIndex && list.Values[nextIndex].IsExtendedBy(value))
                    return nextIndex;

                if (list.Values[mid].IsExtendedBy(value))
                    return mid;

                int comparison = list.Values[mid].Min.CompareTo(value);
                if (comparison == 0)
                    return mid;
                else if (comparison > 0)
                    high = mid - 1;
                else
                    low = mid + 1;
            }

            return mid; // this is our best guess
        }
    }
}
