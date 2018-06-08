using System;
using System.Threading;

namespace MultiThread
{
    class Program
    {
        static void Main(string[] args)
        {
            var drawThread =
                new Thread(Draw) {IsBackground = true};
            drawThread.Start();

            var frontThread =
                new Thread(Pause) {IsBackground = true};
            frontThread.Start();

            // suspend the screen
            Console.ReadKey();

            drawThread.Abort();
            Console.ReadKey();
        }

        public static void Draw()
        {
            var rd = new Random();
            while (true)
            {
                Console.Clear();
                Console.WriteLine(rd.Next().ToString().Substring(0, 4));
                Thread.Sleep(1000);
            }
        }

        public static void Pause()
        {
            var k =  Console.ReadKey();
            if (k.Key == ConsoleKey.Enter)
            {
                Console.WriteLine("Enter pressed, stop!");
            }
        }

    }
}
