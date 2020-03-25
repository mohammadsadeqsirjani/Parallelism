using System;
using System.Threading.Tasks;

namespace Task_Programming
{
    internal class Program
    {
        public static void Write(char c)
        {
            var i = 1000;
            while (i-- > 0)
            {
                Console.Write(c);
            }
        }

        public static void CreateAndStartSimpleTask()
        {
            Task.Factory.StartNew(() => Write('.'));

            var write = new Task(() => Write('?'));
            write.Start();

            Write('-');

            Console.ReadKey();
        }

        public static void Write(object o)
        {
            var i = 1000;
            while (i-- > 0)
            {
                Console.Write(o);
            }
        }

        public static void CreateAndStartAnOtherTypeOfSimpleTask()
        {
            var task = new Task(Write, "Hello");
            task.Start();

            Task.Factory.StartNew(Write, 132);
        }

        public static int TextLength(object o)
        {
            Console.WriteLine($"Task with id {Task.CurrentId} processing object {o} ...");
            return o.ToString().Length;
        }

        public static void CreateSimpleTaskWithReturnValue()
        {
            const string text1 = "testing";
            const string text2 = "this";

            var task1 = new Task<int>(TextLength, text1);
            task1.Start();

            var task2 = Task.Factory.StartNew(TextLength, text2);

            Console.WriteLine($"Length of '{text1}' is {task1.Result}");
            Console.WriteLine($"Length of '{text2}' is {task2.Result}");
        }

        public static void Main(string[] args)
        {
            CreateAndStartAnOtherTypeOfSimpleTask();
            CreateAndStartAnOtherTypeOfSimpleTask();
            CreateSimpleTaskWithReturnValue();

            Console.WriteLine("Main Program Done!");
            Console.ReadKey();
        }
    }
}
