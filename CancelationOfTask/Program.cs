using System;
using System.Threading;
using System.Threading.Tasks;

namespace CancelationOfTask
{
    internal static class Program
    {
        public static void CancellationTasks()
        {
            var cts = new CancellationTokenSource();
            var token = cts.Token;

            var task = Task.Factory.StartNew(() =>
            {
                var i = 0;
                while (true)
                {
                    if (token.IsCancellationRequested)
                    {
                        Console.WriteLine("Task has been canceled");
                        break;
                    }

                    Console.Write($"{i++}\t");
                }
            }, token);

            Console.ReadKey();
            cts.Cancel();
        }

        public static void OtherTypeOfCancellationTasks()
        {
            var cts = new CancellationTokenSource();
            var token = cts.Token;

            var task = new Task(() =>
            {
                var i = 0;
                while (true)
                {
                    if (token.IsCancellationRequested)
                    {
                        throw new OperationCanceledException();
                    }

                    Console.Write($"{i++}\t");
                }
            }, token);

            task.Start();

            Console.ReadKey();
            cts.Cancel();
        }

        public static void ManagedCancellationTasks()
        {
            var cts = new CancellationTokenSource();
            var token = cts.Token;

            token.Register(() =>
            {
                Console.WriteLine("Cancelation has been requested.");
            });

            var task = new Task(() =>
            {
                var i = 0;
                while (true)
                {
                    token.ThrowIfCancellationRequested();
                    Console.Write($"{i++}\t");
                }
            }, token);

            task.Start();

            Task.Factory.StartNew(() =>
            {
                token.WaitHandle.WaitOne();
                Console.WriteLine("\nWait handle released, cancelation has been requested.");
            }, token);

            Console.ReadKey();
            cts.Cancel();
        }

        private static void CompositeCancellationTasks()
        {
            var planned = new CancellationTokenSource();
            var preventative = new CancellationTokenSource();
            var emergency = new CancellationTokenSource();

            var paranoid = CancellationTokenSource.CreateLinkedTokenSource(
                  planned.Token
                , preventative.Token
                , emergency.Token);

            Task.Factory.StartNew(() =>
            {
                var i = 0;

                while (true)
                {
                    paranoid.Token.ThrowIfCancellationRequested();
                    Console.Write($"{i++}\t");
                    Thread.Sleep(1000);
                }
            }, paranoid.Token);

            Console.ReadKey();
            emergency.Cancel();

            Task.Factory.StartNew(() =>
            {
                paranoid.Token.WaitHandle.WaitOne();
                Console.WriteLine("Wait handle released, cancelation has been requested.");
            }, paranoid.Token);
        }

        private static void Main(string[] args)
        {
            CompositeCancellationTasks();

            Console.WriteLine("Main Program Completed!");
            Console.ReadKey();
        }
    }
}
