using System;
using System.Threading.Tasks;

namespace ExceptionHandling
{
    internal class Program
    {
        public static void ExceptionHandler()
        {
            var task1 = Task.Factory.StartNew(() => throw new InvalidOperationException("Can't do this!") { Source = "task1" });

            var task2 = Task.Factory.StartNew(() => throw new AccessViolationException("Can't do this!") { Source = "task2" });

            try
            {
                Task.WaitAll(task1, task2);
            }
            catch (AggregateException ex)
            {
                //foreach (var error in ex.InnerExceptions)
                //{
                //    Console.WriteLine($"Exception {error.GetType()} from {error.Source}");
                //}

                ex.Handle(err =>
                {
                    if (!(err is InvalidOperationException))
                        return false;

                    Console.WriteLine("Invalid Operation Exception");
                    return true;
                });
            }
        }

        internal static void Main(string[] args)
        {
            try
            {
                ExceptionHandler();
            }
            catch (AggregateException ae)
            {
                foreach (var err in ae.InnerExceptions)
                {
                    Console.WriteLine($"Exception {err.GetType()} from {err.Source}");
                }
            }

            Console.WriteLine("Main process has been completed!");
            Console.ReadKey();
        }
    }
}
