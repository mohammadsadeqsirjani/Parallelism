using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace MutexLocking
{
    internal class BankAccount
    {
        private int _balance;

        public int Balance
        {
            get => _balance;
            private set => _balance = value;
        }

        public void Deposit(int amount)
        {
            _balance += amount;
        }

        public void Withdraw(int amount)
        {
            _balance -= amount;
        }

        public void Transfer(BankAccount where, int amount)
        {
            Balance -= amount;
            where.Balance += amount;
        }
    }

    internal static class Program
    {
        private static void CriticalSharing()
        {
            var tasks = new List<Task>();
            var sadeqAccount = new BankAccount();
            var sadraAccount = new BankAccount();
            var cts = new CancellationTokenSource();
            var token = cts.Token;
            var sadeqMutex = new Mutex();
            var sadraMutex = new Mutex();

            for (var i = 0; i < 10; i++)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (var j = 0; j < 1000; j++)
                    {
                        var haveLock = sadeqMutex.WaitOne();
                        try
                        {
                            token.ThrowIfCancellationRequested();
                            sadeqAccount.Deposit(1);
                        }
                        finally
                        {
                            if (haveLock) sadeqMutex.ReleaseMutex();
                        }
                    }
                }, token));

                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (var j = 0; j < 1000; j++)
                    {
                        var haveLock = sadraMutex.WaitOne();
                        try
                        {
                            token.ThrowIfCancellationRequested();
                            sadraAccount.Deposit(1);
                        }
                        finally
                        {
                            if (haveLock) sadraMutex.ReleaseMutex();
                        }
                    }
                }, token));

                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (var j = 0; j < 1000; j++)
                    {
                        var haveLock = WaitHandle.WaitAll(new[] { sadeqMutex, sadraMutex });
                        try
                        {
                            token.ThrowIfCancellationRequested();
                            sadraAccount.Transfer(sadeqAccount, 1);
                        }
                        finally
                        {
                            if (haveLock)
                            {
                                sadraMutex.ReleaseMutex();
                                sadeqMutex.ReleaseMutex();
                            }
                        }
                    }
                }, token));
            }

            Task.WaitAll(tasks.ToArray());

            Console.WriteLine($"The final sadeq balance is {sadeqAccount.Balance}");
            Console.WriteLine($"The final sadra balance is {sadraAccount.Balance}");

            Console.ReadKey();
            cts.Cancel();
        }

        private static void GlobalMutex()
        {
            const string application = "app";
            Mutex mutex;
            
            try
            {
                mutex = Mutex.OpenExisting(application);
                Console.WriteLine($"Sorry, {application} is already running.");
                return;
            }
            catch (WaitHandleCannotBeOpenedException e)
            {
                Console.WriteLine("We can run the program just fine.");
                mutex = new Mutex(false, application);
            }

            Console.ReadKey();
            mutex.ReleaseMutex();
        }

        private static void Main(string[] args)
        {
            //CriticalSharing();
            GlobalMutex();

            Console.WriteLine("Main process has been completed!");
            Console.ReadKey();
        }
    }
}