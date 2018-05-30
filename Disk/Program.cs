using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disk
{
    class Program
    {
        static void Main(string[] args)
        {
            var diskRequestList = new List<int>{50, 40, 30, 18, 90, 100, 150, 300, 20, 200};
            const int currentDisk = 100;

            Console.WriteLine("Please input the algorithm's name. (FCFS SSTF SCAN CSCAN NSSCAN exit)");
            while (true)
            {
                var valid = true;
                switch (Console.ReadLine()?.Trim().ToUpper())
                {
                    case "FCFS":
                        Fcfs(diskRequestList, currentDisk);
                        break;
                    case "SSTF":
                        Sstf(diskRequestList, currentDisk);
                        break;
                    case "SCAN":
                        Scan(diskRequestList, currentDisk);
                        break;
                    case "CSCAN":
                        Cscan(diskRequestList, currentDisk);
                        break;
                    case "NSSCAN":
                        NStepScan(diskRequestList, currentDisk);
                        break;
                    case "EXIT":
                        return;
                    default:
                        valid = false;
                        break;
                }

                if (!valid)
                {
                    Console.WriteLine("Please input the right algorithm's name.");
                }
            }
        }
        //first Come First Serve
        private static void Fcfs(List<int> diskRequestList, int currentDisk)
        {
            var moveQueue = new List<int>();
            var moveDistance = 0;
            var current = currentDisk;
            foreach (var item in diskRequestList)
            {
                moveDistance += Math.Abs(current - item);
                current = item;
                moveQueue.Add(current);
            }
            ShowResult(moveQueue, moveDistance);
        }

        //n Step Scan
        private static void NStepScan(List<int> diskRequestList, int currentDisk)
        {
            Console.Write("Please input the number of step: ");
            int.TryParse(Console.ReadLine(), out var step);

            var moveQueue = new List<int>();
            var moveDistance = 0;
            var count = 0;
            while (count < diskRequestList.Count)
            {
                var range = Math.Min(step, diskRequestList.Count - count);
                if (range == 0)
                {
                    break;
                }

                var stepRange = diskRequestList.GetRange(count, range);
                var res = Scan(stepRange, currentDisk, true);

                currentDisk = res.Item3;
                moveDistance += res.Item3;
                moveQueue.AddRange(res.Item1);

                count += range;
            }

            ShowResult(moveQueue, moveDistance);
        }

        //Circular SCAN
        private static void Cscan(List<int> diskRequestList, int currentDisk)
        {
            var moveQueue = new List<int>();
            var moveDistance = 0;
            var current = currentDisk;

            var orderList = new List<int>();
            foreach (var item in diskRequestList)
            {
                orderList.Add(item);
            }
            orderList.Sort();

            var index = 0;
            while (orderList[index] < current)
            {
                index++;
            }

            for (var i = index; i < orderList.Count; i++)
            {
                moveDistance += Math.Abs(orderList[i] - current);
                current = orderList[i];
                moveQueue.Add(current);
            }

            for (var i = 0; i < index; i++)
            {
                moveDistance += Math.Abs(orderList[i] - current);
                current = orderList[i];
                moveQueue.Add(current);
            }
            ShowResult(moveQueue, moveDistance);
        }

        //SCAN
        private static Tuple<List<int>, int, int> Scan(List<int> diskRequestList, int currentDisk, bool isStep = false)
        {
            // assuming the increasing direction
            var forward = true;
            var moveQueue = new List<int>();
            var moveDistance = 0;

            while (diskRequestList.Count != 0)
            {
                if (forward)
                {
                    var temp = currentDisk;
                    var index = -1;
                    diskRequestList.Select((o, i) =>
                    {
                        if (o >= temp)
                        {
                            temp = o;
                            index = i;
                        }

                        return 0;
                    });

                    if (index == -1)
                    {
                        forward = false;
                    }
                    else
                    {
                        moveDistance += Math.Abs(currentDisk - diskRequestList[index]);
                        currentDisk = diskRequestList[index];
                        diskRequestList.RemoveAt(index);
                    }
                }
                else
                {
                    var temp = currentDisk;
                    var index = -1;
                    diskRequestList.Select((o, i) =>
                    {
                        if (o <= temp)
                        {
                            temp = o;
                            index = i;
                        }

                        return 0;
                    });

                    if (index == -1)
                    {
                        forward = true;
                    }
                    else
                    {
                        moveDistance += Math.Abs(currentDisk - diskRequestList[index]);
                        currentDisk = diskRequestList[index];
                        diskRequestList.RemoveAt(index);
                    }
                }
            }

            if (!isStep)
            {
                ShowResult(moveQueue, moveDistance);
            }

            return Tuple.Create(moveQueue, moveDistance, currentDisk);
        }

        //shortest Seek Time First
        private static void Sstf(List<int> diskRequestList, int currentDisk)
        {
            throw new NotImplementedException();
        }


        private static void ShowResult(List<int> moveQueue, int moveDistance)
        {
            Console.WriteLine(moveQueue.Select(o => o.ToString()).Aggregate((a, b) => a + "->" + b));
            Console.WriteLine("The total distance is " + moveDistance);
            Console.WriteLine("The avg distance is " + moveDistance * 1.0 / moveQueue.Count);
        }
    }
}
