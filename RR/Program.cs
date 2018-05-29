using System;
using System.Collections.Generic;
using System.Linq;

namespace RR
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var jobList = new List<Job>();

            Console.Write("Please input process number N: ");
            int.TryParse(Console.ReadLine(), out var processNum);
            Console.Write("Please input the size of time piece: ");
            int.TryParse(Console.ReadLine(), out var timePiece);

            Console.WriteLine("Please input each process's (name, arrived time, service time):");
            for (var i = 0; i < processNum; i++)
            {
                var input = Console.ReadLine()?.Split(' ');
                while (input == null || input.Length != 3)
                {
                    input = Console.ReadLine()?.Split(' ');
                }

                jobList.Add(new Job(input[0], int.Parse(input[1]), int.Parse(input[2])));
            }

            var runedNum = 0;
            var currentTime = 0;
            double totalWorkTime = 0;
            double totalWaitingTime = 0;
            double totalTurnaroundTime = 0;

            var readyList = new List<Job>();
            jobList.Sort((a, b) => b.ArrivalTime.CompareTo(a.ArrivalTime));

            Console.WriteLine("RunID\tName\tEndT\tWaitT\tTurnaroundT");
            while (runedNum < processNum)
            {
                while (jobList.Count != 0 && jobList.Last().ArrivalTime <= currentTime)
                {
                    readyList.Add(jobList.Last());
                    jobList.RemoveAt(jobList.Count - 1);
                }

                if (readyList.Count == 0)
                {
                    currentTime += timePiece;
                    continue;
                }

                var currentJob = readyList.First();
                readyList.RemoveAt(0);

                if ((currentJob.runAmount += timePiece) >= currentJob.ServiceTime)
                {
                    var overuse = currentJob.runAmount - currentJob.ServiceTime;
                    var endTime = currentTime + timePiece - overuse;
                    currentJob.WaitTime = endTime - currentJob.ArrivalTime;

                    var turnaroundTime = currentJob.WaitTime * 1.0 / currentJob.ServiceTime;

                    Console.WriteLine(
                        $"{runedNum}\t{currentJob.Name}\t{endTime}\t{currentJob.WaitTime}\t{turnaroundTime}");

                    totalWorkTime += currentJob.ServiceTime;
                    totalWaitingTime += currentJob.WaitTime;
                    totalTurnaroundTime += turnaroundTime;

                    runedNum++;
                }
                else
                {
                    readyList.Add(currentJob);
                }

                currentTime += timePiece;
            }

            Console.WriteLine("Avg workT: " + totalWorkTime / processNum);
            Console.WriteLine("Avg waitingT: " + totalWaitingTime / processNum);
            Console.WriteLine("Avg turnaroundT: " + totalTurnaroundTime / processNum);
            Console.WriteLine("CPU utilization: " + totalWorkTime * 100 / currentTime + "%");

            //Suspend the screen
            Console.ReadLine();
        }

        public class Job
        {
            public Job(string name, int arrivalTime, int serviceTime)
            {
                Name = name;
                ArrivalTime = arrivalTime;
                ServiceTime = serviceTime;
            }

            public string Name { get; }

            // arrival time of job
            public int ArrivalTime { get; }

            // serviceTime of job
            public int ServiceTime { get; }

            public int WaitTime { get; set; }

            public int runAmount { get; set; }
        }
    }
}