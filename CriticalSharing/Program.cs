
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CriticalSharing
{
    internal class BankAccount
    {
        public int Balance { get; private set; }

        public void Deposit(int amount)
        {
            lock (this)
            {
                Balance += amount;
            }
        }

        public void Withdraw(int amount)
        {
            lock (this)
            {
                Balance -= amount;
            }
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
