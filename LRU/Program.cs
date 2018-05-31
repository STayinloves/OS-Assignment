using System;
using System.Collections.Generic;
using System.Linq;
using T3 = System.Tuple<int, int, int>;

namespace PageReplacement
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.Write("Please input memory number N: ");
            int.TryParse(Console.ReadLine().Trim(), out var memoryNum);

            Console.WriteLine("Please input request array(enter to use example): ");
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                input = "7 0 1 2 0 3 0 4 2 3 0 3 2 1 2 0 1 7 0 1";
                Console.WriteLine("Example: " + input);
            }
            var pageRefs = input.Trim().Split(' ').Select(o => int.Parse(o)).ToList();

            Console.WriteLine("Please input the algorithm's name. (FIFO LRU CLOCK exit)");
            while (true)
            {
                var valid = true;
                switch (Console.ReadLine().Trim().ToUpper())
                {
                    case "LRU":
                        Lru(pageRefs, memoryNum);
                        break;
                    case "FIFO":
                        Fifo(pageRefs, memoryNum);
                        break;
                    case "CLOCK":
                        ImprovedClock(pageRefs, memoryNum);
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

        private static void ImprovedClock(IEnumerable<int> pageRefs, int memoryNum)
        {
            var hitCount = 0;
            var mem = new LinkedList<T3>();
            LinkedListNode<T3> hand = null;
            foreach (var pageRef in pageRefs)
            {
                var hit = false;

                if (hand == null)
                {
                    hand = new LinkedListNode<T3>(new T3(pageRef, 1, 0));
                    mem.AddLast(hand);
                }
                else
                {
                    // Find element in the list
                    for (var it = mem.First; it != null;)
                    {
                        if (it.Value.Item1 == pageRef)
                        {
                            it.Value = new T3(it.Value.Item1, 1, it.Value.Item3);
                            hit = true;
                            hitCount++;
                            break;
                        }

                        it = it.Next;
                    }

                    // Find element to be replaced
                    if (!hit)
                    {
                        var lenOfList = mem.Count;

                        if (lenOfList < memoryNum)
                        {
                            hand = mem.AddAfter(hand, new T3(pageRef, 1, 0));
                        }
                        else
                        {
                            var count = 0;
                            var found = false;
                            // First round
                            while (count++ < lenOfList)
                            {
                                if (hand.Value.Item2 == 0 && hand.Value.Item3 == 0)
                                {
                                    found = true;
                                    break;
                                }

                                if (hand == mem.Last)
                                {
                                    hand = mem.First;
                                }
                                else
                                {
                                    hand = hand.Next;
                                }
                            }

                            if (!found)
                            {
                                // Second round
                                count = 0;
                                while (count++ < lenOfList)
                                {
                                    if (hand.Value.Item2 == 0 && hand.Value.Item3 == 1)
                                    {
                                        found = true;
                                        break;
                                    }

                                    hand.Value = new T3(hand.Value.Item1, 0, hand.Value.Item3);

                                    if (hand == mem.Last)
                                    {
                                        hand = mem.First;
                                    }
                                    else
                                    {
                                        hand = hand.Next;
                                    }
                                }
                            }

                            if (!found)
                            {
                                // First round again
                                count = 0;
                                while (count++ < lenOfList)
                                {
                                    if (hand.Value.Item2 == 0 && hand.Value.Item3 == 0)
                                    {
                                        found = true;
                                        break;
                                    }

                                    if (hand == mem.Last)
                                    {
                                        hand = mem.First;
                                    }
                                    else
                                    {
                                        hand = hand.Next;
                                    }
                                }
                            }

                            if (!found)
                            {
                                // Second round again
                                count = 0;
                                while (count++ < lenOfList)
                                {
                                    if (hand.Value.Item2 == 0 && hand.Value.Item3 == 1)
                                    {
                                        found = true;
                                        break;
                                    }

                                    hand.Value = new T3(hand.Value.Item1, 0, hand.Value.Item3);

                                    if (hand == mem.Last)
                                    {
                                        hand = mem.First;
                                    }
                                    else
                                    {
                                        hand = hand.Next;
                                    }
                                }
                            }

                            if (found)
                            {
                                hand.Value = new T3(pageRef, 1, 0);
                            }
                        }
                    }
                }

                ShowPageResult(hit, pageRef, mem.Select(o => o.Item1).ToList());
            }

            Console.WriteLine($"Hit {hitCount} times");
        }

        private static void Fifo(IEnumerable<int> pageRefs, int memoryNum)
        {
            var hitCount = 0;
            var mem = new Queue<int>();
            foreach (var pageRef in pageRefs)
            {
                var hit = false;
                foreach (var i in mem)
                {
                    if (i == pageRef)
                    {
                        hit = true;
                        hitCount++;
                    }
                }

                if (!hit)
                {
                    if (mem.Count >= memoryNum)
                    {
                        mem.Dequeue();
                    }

                    mem.Enqueue(pageRef);
                }

                ShowPageResult(hit, pageRef, mem.ToList());
            }

            Console.WriteLine($"Hit {hitCount} times");
        }

        private static void Lru(IEnumerable<int> pageRefs, int memoryNum)
        {
            var hitCount = 0;
            var mem = new Dictionary<int, int>();
            foreach (var pageRef in pageRefs)
            {
                var hit = false;
                var leastRecentUsedKey = -1;
                foreach (var i in mem.ToList())
                {
                    if (i.Key == pageRef)
                    {
                        hit = true;
                        hitCount++;
                        mem[i.Key] = 0;
                    }
                    else
                    {
                        mem[i.Key]++;
                        if (leastRecentUsedKey == -1 || mem[i.Key] > mem[leastRecentUsedKey])
                        {
                            leastRecentUsedKey = i.Key;
                        }
                    }
                }

                if (!hit)
                {
                    if (mem.Count >= memoryNum)
                    {
                        mem.Remove(leastRecentUsedKey);
                    }

                    mem.Add(pageRef, 0);
                }

                ShowPageResult(hit, pageRef, mem.Keys.ToList());
            }

            Console.WriteLine($"Hit {hitCount} times");
        }

        private static void ShowPageResult(bool hit, int memory, List<int> memoryList)
        {
            Console.Write(memory);
            if (hit)
            {
                Console.WriteLine("[H]");
            }
            else
            {
                Console.WriteLine("[M]" + memoryList.Select(o => o.ToString()).Aggregate((a, b) => a + " " + b));
            }
        }
    }
}