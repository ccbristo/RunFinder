using System;
using System.Collections.Generic;
using System.Linq;

namespace RunFinder
{
    class Program
    {
        static void Main(string[] args)
        {
            var numbers = GenerateNumbers(1000000, 10000000).ToList();
            Console.WriteLine("Finding runs.");

            var runs = new SortedList<int, Run>();

            var timer = System.Diagnostics.Stopwatch.StartNew();
            foreach (var n in numbers)
            {
                int? index = runs.FindClosestIndex(n);
                Add(runs, n, index);
            }
            timer.Stop();
            Print(runs.Values);
            Console.WriteLine("Completed in {0}.", timer.Elapsed);
            Console.ReadLine();
        }

        private static IEnumerable<int> GenerateNumbers(int count, int range)
        {
            //return new[] { 1, 59, 12, 43, 4, 58, 5, 13, 46, 3, 6 };
            //return new[] { 1, 3, 5, 7, 6 };

            System.Diagnostics.Debug.Assert(count <= range, "Cannot generate more numbers than are in the range.");

            int tickCount = 51545819; // Environment.TickCount;
            Console.WriteLine("Seed: {0}", tickCount); // so we can re-run the exact same inputs again

            var random = new Random(tickCount);
            var numbers = new HashSet<int>();

            while (numbers.Count < count)
            {
                int n = random.Next(range);

                // build some bias into the number generation so that numbers tend to clump when collisions occur
                int sign = n % 2 == 0 ? 1 : -1;
                while (!numbers.Add(n))
                {
                    n += sign;
                }

                yield return n;
            }
        }

        private static void Add(SortedList<int, Run> runs, int n, int? index)
        {
            if (index == null)
            {
                runs.Add(n, new Run(n, n));
                return;
            }

            Run closest = runs.Values[index.Value];
            Run previous = null, next = null;

            int previousIndex = index.Value - 1;
            if (index.Value >= 1)
                previous = runs.Values[previousIndex];

            int nextIndex = index.Value + 1;
            if (nextIndex < runs.Count)
                next = runs.Values[nextIndex];

            if (previous != null &&
                previous.IsExtendedAboveBy(n) && closest.IsExtendedBelowBy(n))
            {
                // remove the high side first, changing the low side moves the high side down
                runs.RemoveAt(index.Value); // how much pressure are we putting on the GC by creating and destroying so many objects?
                runs.RemoveAt(previousIndex);
                runs.Add(previous.Min, new Run(previous.Min, closest.Max));
            }
            else if (next != null &&
                closest.IsExtendedAboveBy(n) && next.IsExtendedBelowBy(n))
            {
                // remove the high side first, changing the low side moves the high side down
                runs.RemoveAt(nextIndex - 1);
                runs.RemoveAt(index.Value);
                runs.Add(closest.Min, new Run(closest.Min, next.Max));
            }
            else if (closest.IsExtendedAboveBy(n))
            {
                runs.RemoveAt(index.Value);
                runs.Add(closest.Min, new Run(closest.Min, n));
            }
            else if (closest.IsExtendedBelowBy(n))
            {
                runs.RemoveAt(index.Value);
                runs.Add(n, new Run(n, closest.Max));
            }
            else
            {
                runs.Add(n, new Run(n, n));
            }
        }

        private static void Print(IEnumerable<Run> runs)
        {
            var result = string.Join(", ", runs.Select(r => r.ToString()).ToArray());
            Console.WriteLine(result);
        }
    }
}
