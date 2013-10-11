using System;
using System.Linq;

namespace RunFinder
{
    [System.Diagnostics.DebuggerDisplay("{ToString()}")]
    public class Run : IComparable<Run>
    {
        // TODO: Assess the choice of using integers here.
        // would uints or longs be more appropriate?
        // possibly smaller, ie short?
        public int Min { get; private set; }
        public int Max { get; private set; }

        public Run(int min, int max)
        {
            this.Min = min;
            this.Max = max;
        }

        public override string ToString()
        {
            // possible optimization: memoize the result once it is computed
            // computation may be expensive if the run is large
            // if this computation is memoized, the class must be immutable or remember to clear
            // the memoized value if the inputs to the computation change
            var range = Enumerable.Range(Min, Max - Min + 1).Select(i => i.ToString()).ToArray();
            var numbers = string.Join(",", range);
            return "{" + numbers + "}";
        }

        public bool IsExtendedBy(int n)
        {
            return this.IsExtendedBelowBy(n) || IsExtendedAboveBy(n);
        }

        public bool IsExtendedBelowBy(int n)
        {
            return n == this.Min - 1;
        }

        public bool IsExtendedAboveBy(int n)
        {
            return this.Max + 1 == n;
        }

        public int CompareTo(Run other)
        {
            if (other == null)
                return 1;

            return this.Min.CompareTo(other.Min);
        }
    }
}
