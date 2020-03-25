using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace WaitForTask
{
    internal class Program
    {
        public static void TaskWaiting()
        {
            var cts = new CancellationTokenSource();
            var token = cts.Token;

            var task = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("I take 5 seconds.");
                for (var i = 0; i < 5; i++)
                {
                    token.ThrowIfCancellationRequested();
                    Thread.Sleep(1000);
                }

                Console.WriteLine("I'm done!");
            }, token);

            Console.ReadKey();
            cts.Cancel();
        }

        public static void AnOtherTaskWaiting()
        {
            var cts = new CancellationTokenSource();
            var token = cts.Token;

            var task = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("An other task starts.");
                Thread.SpinWait(6000);
                Console.WriteLine("Task is completed.");
            }, token);

            Console.ReadKey();
            cts.Cancel();
        }

        public static void TaskStopWatch()
        {
            var cts = new CancellationTokenSource();
            var token = cts.Token;

            var task1 = new Task(() =>
            {
                Console.WriteLine("I take 5 seconds.");
                for (var i = 0; i < 5; i++)
                {
                    token.ThrowIfCancellationRequested();
                    Thread.Sleep(1000);
                }

                Console.WriteLine("I'm done!");
            }, token);

            task1.Start();

            var task2 = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("An other task starts.");
                Thread.Sleep(6000);
                Console.WriteLine("An other task is completed.");
            }, token);


            Task.WaitAny(new[] { task1, task2 }, 5000, token);

            Console.WriteLine($"Task 1 is in {task1.Status} status mode");
            Console.WriteLine($"Task 2 is in {task1.Status} status mode");

            Console.ReadKey();
            cts.Cancel();
        }

        private static void Main(string[] args)
        {
            //AnOtherTaskWaiting();
            //TaskWaiting();
            var timer = new Stopwatch();
            timer.Start();
            TaskStopWatch();
            timer.Stop();
            Console.WriteLine($"{timer.ElapsedMilliseconds / 1000}");
            Console.WriteLine("Main proccess completed!");
        }
    }
}
