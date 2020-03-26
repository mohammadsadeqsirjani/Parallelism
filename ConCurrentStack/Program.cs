using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection.Metadata;

namespace ConCurrentStack
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Stack();

            Console.WriteLine("Main process has been completed!");
        }

        private static void Stack()
        {
            var stack = new ConcurrentStack<int>();

            //inserting item in the top of stack
            stack.Push(1);
            stack.Push(2);
            stack.Push(3);
            stack.Push(4);
            stack.Push(5);

            //peek and return the top item of stack
            var success = stack.TryPeek(out var peek);
            if (success)
                Console.WriteLine($"The front item in the stack is {peek}");

            //peek and pop and return the top item of stack
            success = stack.TryPop(out var pop);
            if (success)
                Console.WriteLine($"The removed item is {pop}");

            //push range item in the stack
            stack.PushRange(new[] {6, 7, 8, 9, 10});

            //pop range item in stack
            var collection = new int[15];
            if (stack.TryPopRange(collection, 0, 15) > 0)
            {
                var text = string.Join(", ", collection.Select(i => i.ToString()));
                Console.WriteLine($"Popped these items {text}.");
            }
        }
    }
}
