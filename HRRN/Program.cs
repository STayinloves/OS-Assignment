using System;
using System.Collections.Generic;
using System.Linq;

namespace HRRN
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var hrrnCmp = Comparer<Job>.Create((b, a) =>
            {
                if (a.WaitTime == b.WaitTime && a.WaitTime == 0)
                {
                    return a.ServiceTime.CompareTo(b.ServiceTime);
                }
                return ((a.WaitTime + a.ServiceTime) * 1.0 / a.ServiceTime).CompareTo(
                    (b.WaitTime + b.ServiceTime) * 1.0 / b.ServiceTime);
            });

            var jobList = new List<Job>();

            Console.Write("Please input process number N: ");
            int.TryParse(Console.ReadLine(), out var processNum);

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

            Console.WriteLine("RunID\tName\tBeginT\tEndT\tWaitT\tTurnaroundT");
            while (runedNum < processNum)
            {
                while(jobList.Count != 0 && jobList.Last().ArrivalTime <= currentTime)
                {
                    readyList.Add(jobList.Last());
                    jobList.RemoveAt(jobList.Count - 1);
                }

                if (readyList.Count == 0)
                {
                    currentTime = jobList.Last().ArrivalTime;
                    continue;
                }

                foreach (var job in readyList)
                {
                    job.WaitTime = currentTime - job.ArrivalTime;
                }

                readyList.Sort(hrrnCmp);
                var currentJob = readyList.Last();
                readyList.RemoveAt(readyList.Count - 1);

                var turnaroundTime = (currentJob.WaitTime + currentJob.ServiceTime) * 1.0 / currentJob.ServiceTime;

                Console.WriteLine(
                    $"{runedNum}\t{currentJob.Name}\t{currentTime}\t{currentTime + currentJob.ServiceTime}\t{currentJob.WaitTime}\t{turnaroundTime}");

                totalWorkTime += currentJob.ServiceTime;
                totalWaitingTime += currentJob.WaitTime;
                totalTurnaroundTime += turnaroundTime;

                currentTime += currentJob.ServiceTime;
                runedNum++;
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
        }
    }
}