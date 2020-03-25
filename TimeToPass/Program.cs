using System;
using System.Threading;
using System.Threading.Tasks;

namespace TimeToPass
{
    public class Program
    {
        public static void DisarmingBoom()
        {
            var bomb = new CancellationTokenSource();
            var token = bomb.Token;

            Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Press any key to disarm, you have 5 seconds.");
                var cancelled = token.WaitHandle.WaitOne(5000);
                Console.WriteLine(cancelled ? "Bomb disarmed." : "BOOM!");
            }, token);

            Console.ReadKey();
            bomb.Cancel();
        }

        public static void Main(string[] args)
        {
            DisarmingBoom();
            Console.WriteLine("Main Program completed!");
            Console.ReadKey();
        }
    }
}
