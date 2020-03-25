using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace ReadWriteLock
{
    internal static class Program
    {
        private static readonly ReaderWriterLockSlim _padlock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        private static readonly Random _random = new Random();

        private static void Main(string[] args)
        {
            //Locker();
            UpgradableLock();

            Console.WriteLine("Main process has been completed!");
            Console.ReadKey();
        }

        private static void Locker()
        {
            var x = 0;
            var tasks = new List<Task>();

            for (var i = 0; i < 10; i++)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    _padlock.EnterReadLock();
                    Console.WriteLine($"Entered read lock, x = {x}.");
                    Thread.Sleep(5000);
                    _padlock.ExitReadLock();
                    Console.WriteLine($"Exited read lock, x = {x}.");
                }));
            }

            try
            {
                Task.WaitAll(tasks.ToArray());
            }
            catch (AggregateException ae)
            {
                ae.Handle(e =>
                {
                    Console.WriteLine(e);
                    return true;
                });
            }

            for (var i = 0; i < 10; i++)
            {
                _padlock.EnterWriteLock();
                Console.WriteLine("Write lock acquired.");
                x = _random.Next(10);
                Console.WriteLine($"Set x = {x}");
                _padlock.ExitWriteLock();
                Console.WriteLine("Write lock released.");
            }
        }

        private static void UpgradableLock()
        {
            var x = 0;
            var tasks = new List<Task>();

            for (var i = 0; i < 10; i++)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    _padlock.EnterUpgradeableReadLock();

                    if (i % 2 == 0)
                    {
                        _padlock.EnterWriteLock();
                        x = _random.Next(10);
                        _padlock.ExitWriteLock();
                    }

                    Console.WriteLine($"{i}: Entered read lock, x = {x}.");
                    Thread.Sleep(1000);
                    _padlock.ExitUpgradeableReadLock();
                    Console.WriteLine($"{i}: Exited read lock, x = {x}.");
                }));
            }

            try
            {
                Task.WaitAll(tasks.ToArray());
            }
            catch (AggregateException ae)
            {
                ae.Handle(e =>
                {
                    Console.WriteLine(e);
                    return true;
                });
            }

            for (var i = 0; i < 10; i++)
            {
                _padlock.EnterWriteLock();
                Console.WriteLine("Write lock acquired.");
                x = _random.Next(10);
                Console.WriteLine($"Set x = {x}");
                _padlock.ExitWriteLock();
                Console.WriteLine("Write lock released.");
            }
        }
    }
}
