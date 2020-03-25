using System;
using System.Collections.Concurrent;
using System.Security;
using System.Threading.Tasks;

namespace ConCurrentDictionary
{
    internal static class Program
    {
        private static ConcurrentDictionary<string, string> capitals = new ConcurrentDictionary<string, string>();

        private static void AddParis()
        {
            var success = capitals.TryAdd("France", "Paris");
            var who = Task.CurrentId.HasValue ? ("Task " + Task.CurrentId) : "Main Thread";

            Console.WriteLine($"{who} {(success ? "added" : "did not add")} the element.");
        }

        private static void Main(string[] args)
        {
            Task.Factory.StartNew(AddParis);
            AddParis();

            //capitals["Russia"] = "Leningrad";
            capitals.AddOrUpdate("Russia", "Moscow",
                (key, oldValue) => oldValue + " --> Moscow");
            Console.WriteLine($"The capital of Russia is {capitals["Russia"]}");

            //capitals["Sweden"] = "Uppsala";
            var capitalOfSweden = capitals.GetOrAdd("Sweden", "Stockholm");
            Console.WriteLine($"The capital of Sweden is {capitalOfSweden}");

            const string toRemove = "Russia";
            string removed;
            var didRemove = capitals.TryRemove(toRemove, out removed);
            if (didRemove)
            {
                Console.WriteLine($"We just remove {removed}");
            }
            else
            {
                Console.WriteLine($"Failed to remove the capital of {toRemove}");
            }


            Console.WriteLine("Main process has been completed!");
        }
    }
}
