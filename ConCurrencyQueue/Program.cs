using System;
using System.Collections.Concurrent;

namespace ConCurrencyQueue
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Queue();

            Console.WriteLine("Main process has been completed!");
        }

        private static void Queue()
        {
            var queue = new ConcurrentQueue<int>();
            //add item into the end of the queue
            queue.Enqueue(3);
            queue.Enqueue(2);
            queue.Enqueue(1);

            //peek the first item of the queue
            var success = queue.TryPeek(out var peek);
            if (success)
                Console.WriteLine($"The first item in queue is {peek}.");

            //peek and remove the first of the queue
            success = queue.TryDequeue(out var deQueue);
            if (success)
                Console.WriteLine($"The removed item is {deQueue}.");
        }
    }
}
