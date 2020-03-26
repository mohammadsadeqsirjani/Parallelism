using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace ProducerConsumerPattern
{
    internal static class Program
    {
        private static readonly BlockingCollection<int> messages =
            new BlockingCollection<int>(new ConcurrentBag<int>(), Environment.ProcessorCount);

        private static readonly CancellationTokenSource tokenSource = new CancellationTokenSource();

        private static readonly Random random = new Random();

        private static void Main(string[] args)
        {
            tokenSource.Token.Register(() => { Console.WriteLine("Request has been canceled!"); });
            Task.Factory.StartNew(ProducerAndConsumer, tokenSource.Token);

            Console.ReadKey();
            tokenSource.Cancel();
            Console.WriteLine("Main process has been completed!");
        }

        private static void ProducerAndConsumer()
        {
            var producer = Task.Factory.StartNew(RunProducer);
            var consumer = Task.Factory.StartNew(RunConsumer);

            try
            {
                Task.WaitAll(new[] {producer, consumer}, tokenSource.Token);
            }
            catch (AggregateException ae)
            {
                ae.Handle(err => true);
            }
        }

        private static void RunConsumer()
        {
            foreach (var item in messages.GetConsumingEnumerable())
            {
                tokenSource.Token.ThrowIfCancellationRequested();
                Console.WriteLine($"-{item}\t");
                Thread.Sleep(random.Next(1000));
            }
        }

        private static void RunProducer()
        {
            while (true)
            {
                tokenSource.Token.ThrowIfCancellationRequested();
                var i = random.Next(100);
                messages.Add(i);
                Console.WriteLine($"+{i}\t");
                Thread.Sleep(random.Next(10));
            }
        }
    }
}
