using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConCurrentBag
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Bag();

            Console.WriteLine("Main process has been completed!");
        }

        private static void Bag()
        {
            var bag = new ConcurrentBag<int>();
            var tasks = new List<Task>();

            for (var i = 0; i < 10; i++)
            {
                var item = i;
                tasks.Add(Task.Factory.StartNew((() =>
                {
                    bag.Add(item);
                    Console.WriteLine($"Thread {Task.CurrentId} has added {item}.");
                    if (bag.TryPeek(out var result))
                        Console.WriteLine($"Thread {Task.CurrentId} has peeked the value {result}.");
                })));
            }

            Task.WaitAll(tasks.ToArray());
        }
    }
}
