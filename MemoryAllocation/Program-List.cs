using System;
using System.Collections.Generic;
using System.Linq;

namespace MemoryAllocationL
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

        public static LinkedList<Partition>
            FreePartitionTable = new LinkedList<Partition>();

        public static int NumberOfParition;

        public static Dictionary<int, int> TaskTable = new Dictionary<int, int>();

        public static int NumberOfTask;

        private static void Mian(string[] args)
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
            var tmpList = FreePartitionTable.ToList();
            tmpList.Sort((a, b) => a.Size.CompareTo(b.Size));
            FreePartitionTable.Clear();
            foreach (var partition in tmpList)
            {
                FreePartitionTable.AddLast(partition);
            }

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

                    var flag = false;
                    var node = FreePartitionTable.First;
                    while(true)
                    {
                        if (node.Value.Status == false && newJob <= node.Value.Size)
                        {
                            flag = true;
                            break;
                        }

                        if (node.Next != null)
                        {
                            break;
                        }

                        node = node.Next;
                    }

                    if (flag)
                    {
                        AllocateMemory(node);
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

                    var pos = FreePartitionTable.First;
                    var can = false;
                    while (true)
                    {
                        if (pos.Value.Status == false && pos.Value.Size > newJob)
                        {
                            can = true;
                            break;
                        }
                        if (pos.Next == null)
                        {
                            break;
                        }

                        pos = pos.Next;
                    }

                    if (can)
                    {
                        AllocateMemory(pos);
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

        private static void AllocateMemory(LinkedListNode<Partition> node)
        {
            node.Value.Status = true;
            TaskTable.Add(NumberOfTask++, node.Value.Start);
            ShowStatusOfMemory();
        }

        private static void RecyleMemory(int index)
        {
            var add = TaskTable[index];
            LinkedListNode<Partition> pos = FreePartitionTable.First;
            var up = false;
            var down = false;

            
            for (var i = FreePartitionTable.First; i.Next != null; i = i.Next)
            {
                if (i.Value.Start == add)
                {
                    pos = i;
                }
            }

            if (pos.Previous != null && pos.Previous.Value.Status == false)
            {
                up = true;
            }

            if (pos.Next != null && pos.Next.Value.Status == false)
            {
                down = true;
            }

            if (up && down)
            {
                pos.Previous.Value.Size +=
                    pos.Value.Size +
                    pos.Next.Value.Size;
                FreePartitionTable.Remove(pos);
                FreePartitionTable.Remove(pos);
            }
            else if (up)
            {
                pos.Previous.Value.Size += pos.Value.Size;
                FreePartitionTable.Remove(pos);
            }
            else if (down)
            {
                pos.Value.Size += pos.Next.Value.Size;
                FreePartitionTable.Remove(pos.Next);
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

            var rd = new Random();
            var pos = 0;
            for (var i = 0; i < NumberOfParition; i++)
            {
                var size = rd.Next(100) + MinSize;
                FreePartitionTable.AddLast(new LinkedListNode<Partition>(new Partition(pos, size)));
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