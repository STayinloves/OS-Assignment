using System;
using System.Collections.Generic;

namespace MemoryAllocation
{
    internal class Partition
    {
        public Partition(int start, int size, bool status = false)
        {
            Start = start;
            Size = size;
            Status = status;
        }

        public int Start { get; }
        public int Size { get; set; }
        public bool Status { get; set; }
    }

    internal class Program
    {
        private const int MinSize = 2;

        public static List<Partition>
            FreePartitionTable = new List<Partition>();

        public static int nextSearchPoint;

        public static int NumberOfParition;

        public static Dictionary<int, int> TaskTable = new Dictionary<int, int>();

        public static int NumberOfTask;

        private static void Main(string[] args)
        {
            while (true)
            {
                Console.Write("Please input the number of Free partition: ");
                int.TryParse(Console.ReadLine()?.Trim(), out NumberOfParition);
                InitPartition();
                ShowStatusOfMemory();

                Console.WriteLine(
                    "Please input the algorithm's name. (NF BF exit)");
                var valid = true;
                switch (Console.ReadLine()?.Trim().ToUpper())
                {
                    case "NF":
                        NextFit();
                        break;
                    case "BF":
                        BestFit();
                        break;
                    case "EXIT":
                        return;
                    default:
                        valid = false;
                        break;
                }

                if (!valid)
                {
                    Console.WriteLine(
                        "Please input the right algorithm's name.");
                }
            }
        }

        private static void BestFit()
        {
            FreePartitionTable.Sort((a, b) => a.Size.CompareTo(b.Size));
            ShowStatusOfMemory();
            while (true)
            {
                Console.WriteLine("Please choose which type of action do you what?");
                Console.WriteLine("1. add task\t2. end task");
                var input = Console.ReadLine()?.Trim();
                if (input == "1")
                {
                    var newJob = GetJob();
                    if (newJob == -1)
                    {
                        return;
                    }

                    var index = -1;
                    var fitness = int.MaxValue;
                    var i = 0;
                    foreach (var partition in FreePartitionTable)
                    {
                        if (partition.Status == false && newJob <= partition.Size &&
                            fitness >= partition.Size - newJob)
                        {
                            fitness = partition.Size - newJob;
                            index = i;
                        }

                        i++;
                    }

                    if (index != -1)
                    {
                        AllocateMemory(index);
                    }
                    else
                    {
                        Console.WriteLine("Can not allocate memory for this task.");
                    }   
                }
                else if (input == "2")
                {
                    ShowStatusOfTasks();
                    Console.Write("Please input the ID of task: ");
                    int.TryParse(Console.ReadLine(), out var id);
                    if (TaskTable.ContainsKey(id))
                    {
                        RecyleMemory(id);
                    }
                    else
                    {
                        Console.WriteLine("This task id doesn't exsits");
                    }
                }
            }
        }

        private static void NextFit()
        {
            ShowStatusOfMemory();
            while (true)
            {
                Console.WriteLine("Please choose which type of action do you what?");
                Console.WriteLine("1. add task\t2. end task");
                var input = Console.ReadLine()?.Trim();
                if (input == "1")
                {
                    var newJob = GetJob();
                    if (newJob == -1)
                    {
                        return;
                    }

                    var i = 0;
                    var can = false;
                    while (i++ < FreePartitionTable.Count)
                    {
                        if (FreePartitionTable[nextSearchPoint].Status ==
                            false && FreePartitionTable[nextSearchPoint].Size >
                            newJob)
                        {
                            can = true;
                            break;
                        }
                        nextSearchPoint++;
                        if (nextSearchPoint == FreePartitionTable.Count)
                        {
                            nextSearchPoint = 0;
                        }
                    }

                    if (can)
                    {
                        AllocateMemory(nextSearchPoint);
                    }
                    else
                    {
                        Console.WriteLine("Can not allocate memory for this task.");
                    }   
                }
                else if (input == "2")
                {
                    ShowStatusOfTasks();
                    Console.Write("Please input the ID of task: ");
                    int.TryParse(Console.ReadLine(), out var id);
                    if (TaskTable.ContainsKey(id))
                    {
                        RecyleMemory(id);
                    }
                    else
                    {
                        Console.WriteLine("This task id doesn't exsits");
                    }
                }
            }
        }

        private static void AllocateMemory(int index)
        {
            FreePartitionTable[index].Status = true;
            TaskTable.Add(NumberOfTask++, FreePartitionTable[index].Start);
            ShowStatusOfMemory();
        }

        private static void RecyleMemory(int index)
        {
            var add = TaskTable[index];
            var pos = -1;
            var up = false;
            var down = false;

            for (var i = 0; i < FreePartitionTable.Count; i++)
            {
                if (FreePartitionTable[i].Start == add)
                {
                    pos = i;
                }
            }

            if (pos > 0 && FreePartitionTable[pos - 1].Status == false)
            {
                up = true;
            }

            if (pos < FreePartitionTable.Count - 1 && FreePartitionTable[pos + 1].Status == false)
            {
                down = true;
            }

            if (up && down)
            {
                FreePartitionTable[pos - 1].Size +=
                    FreePartitionTable[pos].Size +
                    FreePartitionTable[pos + 1].Size;
                FreePartitionTable.RemoveAt(pos);
                FreePartitionTable.RemoveAt(pos);
            }
            else if (up)
            {
                FreePartitionTable[pos - 1].Size += FreePartitionTable[pos].Size;
                FreePartitionTable.RemoveAt(pos);
            }
            else if (down)
            {
                FreePartitionTable[pos].Size += FreePartitionTable[pos + 1].Size;
                FreePartitionTable.RemoveAt(pos + 1);
            }

            TaskTable.Remove(index);
            Console.WriteLine(index + " has been recycled.");
        }

        private static int GetJob()
        {
            Console.Write("Please input the size of new Job(q to exit): ");

            var input = Console.ReadLine();
            if (input == "exit" || input == "q")
            {
                return -1;
            }

            int.TryParse(input, out var size);

            return size;
        }

        public static void InitPartition()
        {
            FreePartitionTable.Clear();
            TaskTable.Clear();
            NumberOfTask = 0;
            nextSearchPoint = 0;

            var rd = new Random();
            var pos = 0;
            for (var i = 0; i < NumberOfParition; i++)
            {
                var size = rd.Next(100) + MinSize;
                FreePartitionTable.Add(new Partition(pos, size));
                pos += size + 1;
            }
        }

        public static void ShowStatusOfMemory()
        {
            Console.WriteLine("Index\tStart\tSize\tStatus");
            var cnt = 0;
            foreach (var partition in FreePartitionTable)
            {
                Console.WriteLine(
                    $"{cnt++}\t{partition.Start}\t{partition.Size}\t{partition.Status}");
            }
        }

        public static void ShowStatusOfTasks()
        {
            Console.WriteLine("Index\tStart");
            foreach (var task in TaskTable)
            {
                Console.WriteLine( $"{task.Key}\t{task.Value}");
            }
        }
    }
}