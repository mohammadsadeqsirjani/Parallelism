using System;
using System.Collections.Generic;
using System.Security;
using System.Threading;
using System.Threading.Tasks;

namespace InterlockOperation
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
    }
    internal static class Program
    {
        private static SpinLock _spinLock = new SpinLock(true);
        private static void CriticalSection()
        {
            var tasks = new List<Task>();
            var account = new BankAccount();
            var spinLock = new SpinLock();

            for (var i = 0; i < 10; i++)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (var j = 0; j < 1000; j++)
                    {
                        var lockTaken = false;
                        try
                        {
                            spinLock.Enter(ref lockTaken);
                            account.Deposit(100);
                        }
                        finally
                        {
                            if (lockTaken) spinLock.Exit();
                        }
                    }
                }));

                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (var j = 0; j < 1000; j++)
                    {
                        var lockTaken = false;
                        try
                        {
                            spinLock.Enter(ref lockTaken);
                            account.Withdraw(100);
                        }
                        finally
                        {
                            if (lockTaken) spinLock.Exit();
                        }
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());

            Console.WriteLine($"The final balance is {account.Balance}.");

        }

        private static void LockRecursion(int x)
        {
            var lockTaken = false;

            try
            {
                _spinLock.Enter(ref lockTaken);
            }
            catch (LockRecursionException err)
            {
                Console.WriteLine(err);
                throw;
            }
            finally
            {
                if (lockTaken)
                {
                    Console.WriteLine($"Took a lock, x = {x}");
                    LockRecursion(x - 1);
                    _spinLock.Exit();
                }
                else
                {
                    Console.WriteLine($"Failed to take  lock  , x = {x}");
                }
            }
        }


        private static void Main(string[] args)
        {
            //CriticalSection();
            LockRecursion(5);
            Console.WriteLine("Main process has been completed!");
            Console.ReadKey();
        }
    }
}