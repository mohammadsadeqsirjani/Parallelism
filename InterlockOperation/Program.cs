using System;
using System.Collections.Generic;
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
            Interlocked.Add(ref _balance, amount);
        }

        public void Withdraw(int amount)
        {
            Interlocked.Add(ref _balance, -amount);    
        }
    }
    internal static class Program
    {
        private static void CriticalSection()
        {
            var tasks = new List<Task>();
            var account = new BankAccount();

            for (var i = 0; i < 10; i++)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (var j = 0; j < 1000; j++)
                    {
                        account.Deposit(100);
                    }
                }));

                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (var j = 0; j < 1000; j++)
                    {
                        account.Withdraw(100);
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());

            Console.WriteLine($"The final balance is {account.Balance}.");

        }
        private static void Main(string[] args)
        {
            CriticalSection();

            Console.WriteLine("Main process has been completed!");
            Console.ReadKey();
        }
    }
}
